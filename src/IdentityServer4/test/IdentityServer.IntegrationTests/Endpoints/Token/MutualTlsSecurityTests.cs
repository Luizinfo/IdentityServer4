using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using Xunit;

namespace IdentityServer.IntegrationTests.Endpoints.Token
{
    public class MutualTlsSecurityTests
    {
        [Theory]
        [InlineData("/connect/token", false, true, true, false)]
        [InlineData("/connect/mtls/token", false, true, true, false)]
        [InlineData("/connect/token", true, true, true, true)]
        [InlineData("/connect/mtls/token", true, true, true, true)]
        [InlineData("/connect/token", true, false, true, false)]
        [InlineData("/connect/mtls/token", true, false, true, false)]
        [InlineData("/connect/token", false, true, false, true)]
        [InlineData("/connect/mtls/token", false, true, false, true)]
        [InlineData("/connect/token", false, false, false, false)]
        [InlineData("/connect/mtls/token", false, false, false, false)]
        [InlineData("/connect/token", true, true, true, false, true)]
        [InlineData("/connect/mtls/token", true, true, true, false, true)]
        public async Task Certificate_authentication_is_required_on_every_token_endpoint(
            string path, bool caIssued, bool registerHandler, bool subjectNameSecret, bool expectedSuccess, bool expired = false)
        {
            using var rootKey = RSA.Create(2048);
            var rootRequest = new CertificateRequest("CN=Review Test CA", rootKey, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            rootRequest.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));
            rootRequest.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign, true));
            using var root = rootRequest.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(2));

            using var key = RSA.Create(2048);
            var request = new CertificateRequest("CN=Review Test Client", key, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            request.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, true));
            request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, true));
            request.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(new OidCollection { new Oid("1.3.6.1.5.5.7.3.2") }, true));
            using var certificate = caIssued
                ? request.Create(root, DateTimeOffset.UtcNow.AddHours(-2), expired ? DateTimeOffset.UtcNow.AddHours(-1) : DateTimeOffset.UtcNow.AddDays(1), RandomNumberGenerator.GetBytes(16))
                : request.CreateSelfSigned(DateTimeOffset.UtcNow.AddHours(-1), DateTimeOffset.UtcNow.AddDays(1));

            using var host = new HostBuilder().ConfigureWebHost(web =>
            {
                web.UseTestServer();
                web.ConfigureServices(services =>
                {
                    services.AddIdentityServer(options =>
                    {
                        options.MutualTls.Enabled = true;
                        options.MutualTls.ClientCertificateChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
                        options.MutualTls.ClientCertificateChainPolicy.CustomTrustStore.Add(root);
                        // Synthetic CA has no CRL endpoint. Production keeps the online default.
                        options.MutualTls.ClientCertificateChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                    })
                    .AddDeveloperSigningCredential(persistKey: false)
                    .AddInMemoryApiScopes(new[] { new ApiScope("api") })
                    .AddInMemoryClients(new[] { new Client
                    {
                        ClientId = "client",
                        AllowedGrantTypes = GrantTypes.ClientCredentials,
                        AllowedScopes = { "api" },
                        ClientSecrets = { new Secret(subjectNameSecret ? certificate.Subject : certificate.Thumbprint)
                        {
                            Type = subjectNameSecret ? IdentityServerConstants.SecretTypes.X509CertificateName : IdentityServerConstants.SecretTypes.X509CertificateThumbprint
                        }}
                    }})
                    .AddMutualTlsSecretValidators();

                    if (registerHandler)
                    {
                        services.AddAuthentication().AddCertificate(options =>
                        {
                            options.AllowedCertificateTypes = CertificateTypes.All;
                            options.ChainTrustValidationMode = X509ChainTrustMode.CustomRootTrust;
                            options.CustomTrustStore.Add(root);
                            options.RevocationMode = X509RevocationMode.NoCheck;
                        });
                    }
                });
                web.Configure(app =>
                {
                    app.Use(async (context, next) => { context.Connection.ClientCertificate = certificate; await next(); });
                    app.UseIdentityServer();
                });
            }).Build();
            await host.StartAsync();
            using var http = host.GetTestClient();
            http.BaseAddress = new Uri("https://server");
            using var response = await http.PostAsync(path, new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = "client", ["grant_type"] = "client_credentials", ["scope"] = "api"
            }));
            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal(expectedSuccess, response.IsSuccessStatusCode);
            if (expectedSuccess)
            {
                Assert.NotNull(JObject.Parse(body)["access_token"]);
            }
            else
            {
                Assert.Contains(response.StatusCode, new[] { HttpStatusCode.BadRequest, HttpStatusCode.Forbidden });
                Assert.DoesNotContain("access_token", body);
            }
            await host.StopAsync();
        }
    }
}
