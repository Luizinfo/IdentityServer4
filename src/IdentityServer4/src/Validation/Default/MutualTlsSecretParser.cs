// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Configuration;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Validation
{
    /// <summary>
    /// Parses secret according to MTLS spec
    /// </summary>
    public class MutualTlsSecretParser : ISecretParser
    {
        private readonly IdentityServerOptions _options;
        private readonly ILogger<MutualTlsSecretParser> _logger;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        public MutualTlsSecretParser(IdentityServerOptions options, ILogger<MutualTlsSecretParser> logger)
        {
            _options = options;
            _logger = logger;
        }

        /// <summary>
        /// Name of authentication method (blank to suppress in discovery since we do special handling)
        /// </summary>
        public string AuthenticationMethod => String.Empty;

        /// <summary>
        /// Parses the HTTP context
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<ParsedSecret> ParseAsync(HttpContext context)
        {
            _logger.LogDebug("Start parsing for client id in post body");

            if (!context.Request.HasApplicationFormContentType())
            {
                _logger.LogDebug("Content type is not a form");
                return null;
            }

            var body = await context.Request.ReadFormAsync();

            if (body != null)
            {
                var id = body["client_id"].FirstOrDefault();

                // client id must be present
                if (!String.IsNullOrWhiteSpace(id))
                {
                    if (id.Length > _options.InputLengthRestrictions.ClientId)
                    {
                        _logger.LogError("Client ID exceeds maximum length.");
                        return null;
                    }
                    
                    var clientCertificate = await context.Connection.GetClientCertificateAsync();
                    
                    if (clientCertificate is null)
                    {
                        _logger.LogDebug("Client certificate not present");
                        return null;
                    }

                    // Certificates can also reach the ordinary protocol endpoints through
                    // a TLS listener or proxy. The mTLS alias middleware is not sufficient.
                    var scheme = _options.MutualTls.ClientCertificateAuthenticationScheme;
                    var schemes = context.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();
                    if (await schemes.GetSchemeAsync(scheme) == null)
                    {
                        _logger.LogError("The configured client certificate authentication scheme is not registered.");
                        return null;
                    }

                    var authentication = await context.AuthenticateAsync(scheme);
                    if (!authentication.Succeeded)
                    {
                        _logger.LogDebug("Client certificate authentication failed.");
                        return null;
                    }
                    
                    return new ParsedSecret
                    {
                        Id = id,
                        Credential = clientCertificate,
                        Type = IdentityServerConstants.ParsedSecretTypes.X509Certificate
                    };
                }
            }

            _logger.LogDebug("No post body found");
            return null;
        }
    }
}
