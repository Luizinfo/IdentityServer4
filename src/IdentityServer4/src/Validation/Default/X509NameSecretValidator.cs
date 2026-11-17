using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using Microsoft.Extensions.Logging;
using static IdentityServer4.IdentityServerConstants;

namespace IdentityServer4.Validation
{
    /// <summary>
    /// Validator for an X.509 certificate based client secret using the common name
    /// </summary>
    public class X509NameSecretValidator : ISecretValidator
    {
        private readonly ILogger<X509NameSecretValidator> _logger;
        private readonly IdentityServerOptions _options;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="logger"></param>
        public X509NameSecretValidator(ILogger<X509NameSecretValidator> logger)
            : this(logger, new IdentityServerOptions())
        {
        }

        /// <summary>
        /// Creates a validator using the configured CA trust policy.
        /// </summary>
        public X509NameSecretValidator(ILogger<X509NameSecretValidator> logger, IdentityServerOptions options)
        {
            _logger = logger;
            _options = options;
        }

        /// <inheritdoc/>
        public Task<SecretValidationResult> ValidateAsync(IEnumerable<Secret> secrets, ParsedSecret parsedSecret)
        {
            var fail = Task.FromResult(new SecretValidationResult { Success = false });

            if (parsedSecret.Type != ParsedSecretTypes.X509Certificate)
            {
                _logger.LogDebug("X509 name secret validator cannot process {type}", parsedSecret.Type ?? "null");
                return fail;
            }

            if (!(parsedSecret.Credential is X509Certificate2 cert))
            {
                throw new InvalidOperationException("Credential is not a x509 certificate.");
            }

            var name = cert.Subject;
            if (name == null)
            {
                _logger.LogWarning("No subject/name found in X509 certificate.");
                return fail;
            }

            var nameSecrets = secrets.Where(s => s.Type == SecretTypes.X509CertificateName);
            if (!nameSecrets.Any())
            {
                _logger.LogDebug("No x509 name secrets configured for client.");
                return fail;
            }

            using var chain = new X509Chain();
            chain.ChainPolicy = _options.MutualTls.ClientCertificateChainPolicy.Clone();
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;
            chain.ChainPolicy.ApplicationPolicy.Add(new Oid("1.3.6.1.5.5.7.3.2")); // TLS client authentication
            if (!chain.Build(cert) || chain.ChainElements.Count < 2)
            {
                _logger.LogWarning("Subject-name client authentication requires a certificate issued by a trusted CA.");
                return fail;
            }

            foreach (var nameSecret in nameSecrets)
            {
                if (name.Equals(nameSecret.Value, StringComparison.Ordinal))
                {
                    var result = new SecretValidationResult
                    {
                        Success = true,
                        Confirmation = cert.CreateThumbprintCnf()
                    };

                    return Task.FromResult(result);
                }
            }

            _logger.LogDebug("No matching x509 name secret found.");
            return fail;
        }
    }
}
