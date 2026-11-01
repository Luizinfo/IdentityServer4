using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdentityServer4.EntityFramework.Mappers;
using Xunit;
using E = IdentityServer4.EntityFramework.Entities;
using M = IdentityServer4.Models;

namespace IdentityServer4.EntityFramework.UnitTests.Mappers
{
    public class MappingBehaviorTests
    {
        private const string ResourceScalars = "Enabled Name DisplayName Description ShowInDiscoveryDocument";
        private const string ResourceCollections = "UserClaims Properties";
        private const string ClientScalars = "Enabled ClientId ProtocolType RequireClientSecret ClientName Description ClientUri LogoUri RequireConsent AllowRememberConsent AlwaysIncludeUserClaimsInIdToken RequirePkce AllowPlainTextPkce RequireRequestObject AllowAccessTokensViaBrowser FrontChannelLogoutUri FrontChannelLogoutSessionRequired BackChannelLogoutUri BackChannelLogoutSessionRequired AllowOfflineAccess IdentityTokenLifetime AccessTokenLifetime AuthorizationCodeLifetime ConsentLifetime AbsoluteRefreshTokenLifetime SlidingRefreshTokenLifetime RefreshTokenUsage UpdateAccessTokenClaimsOnRefresh RefreshTokenExpiration AccessTokenType EnableLocalLogin IncludeJwtId AlwaysSendClientClaims ClientClaimsPrefix PairWiseSubjectSalt UserSsoLifetime UserCodeType DeviceCodeLifetime";
        private const string ClientCollections = "ClientSecrets AllowedGrantTypes RedirectUris PostLogoutRedirectUris AllowedScopes AllowedIdentityTokenSigningAlgorithms IdentityProviderRestrictions Claims AllowedCorsOrigins Properties";
        private const string GrantScalars = "Key Type SubjectId SessionId ClientId Description CreationTime Expiration ConsumedTime Data";

        public static IEnumerable<object[]> ScalarCases()
        {
            foreach (var name in ClientScalars.Split(' '))
                foreach (var reverse in new[] { false, true })
                    foreach (var zero in new[] { false, true })
                        yield return new object[] { "Client", name, reverse, zero };
            foreach (var group in new[] { "ApiResource", "ApiScope", "IdentityResource", "PersistedGrant" })
            {
                var names = group == "PersistedGrant" ? GrantScalars : ResourceScalars;
                if (group == "ApiScope" || group == "IdentityResource") names += " Required Emphasize";
                foreach (var name in names.Split(' '))
                    foreach (var reverse in new[] { false, true })
                        foreach (var zero in new[] { false, true })
                            yield return new object[] { group, name, reverse, zero };
            }
        }

        [Theory]
        [MemberData(nameof(ScalarCases))]
        public void ScalarFields_MapIndependentlyInEachDirection(string group, string name, bool reverse, bool zero)
        {
            var modelType = typeof(M.Client).Assembly.GetType("IdentityServer4.Models." + group);
            var entityType = typeof(E.Client).Assembly.GetType("IdentityServer4.EntityFramework.Entities." + group);
            var source = Activator.CreateInstance(reverse ? modelType : entityType);
            var sourceProperty = source.GetType().GetProperty(name);
            sourceProperty.SetValue(source, TestValue(sourceProperty.PropertyType, name, zero));
            var result = Convert(group, source, reverse);
            var actual = result.GetType().GetProperty(name).GetValue(result);
            var expected = sourceProperty.GetValue(source);
            if (expected != null && (expected.GetType().IsEnum || actual.GetType().IsEnum))
                Assert.Equal(System.Convert.ToInt32(expected), System.Convert.ToInt32(actual));
            else
                Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("Client")]
        [InlineData("ApiResource")]
        [InlineData("ApiScope")]
        [InlineData("IdentityResource")]
        [InlineData("PersistedGrant")]
        public void PublicProperties_AreExplicitlyClassified(string group)
        {
            var modelType = typeof(M.Client).Assembly.GetType("IdentityServer4.Models." + group);
            var entityType = typeof(E.Client).Assembly.GetType("IdentityServer4.EntityFramework.Entities." + group);
            var fields = group == "Client" ? ClientScalars + " " + ClientCollections :
                group == "PersistedGrant" ? GrantScalars : ResourceScalars + " " + ResourceCollections;
            if (group == "ApiScope" || group == "IdentityResource") fields += " Required Emphasize";
            if (group == "ApiResource") fields += " ApiSecrets Scopes AllowedAccessTokenSigningAlgorithms";
            Assert.Equal(fields.Split(' ').OrderBy(x => x), modelType.GetProperties().Select(x => x.Name).OrderBy(x => x));
            var entityFields = fields.Replace("ApiSecrets", "Secrets");
            if (group == "Client" || group == "ApiResource") entityFields += " Id Created Updated LastAccessed NonEditable";
            if (group == "IdentityResource") entityFields += " Id Created Updated NonEditable";
            if (group == "ApiScope") entityFields += " Id";
            Assert.Equal(entityFields.Split(' ').OrderBy(x => x), entityType.GetProperties().Select(x => x.Name).OrderBy(x => x));
        }

        [Fact]
        public void NullRoots_ReturnNull()
        {
            Assert.Null(((E.Client)null).ToModel());
            Assert.Null(((M.Client)null).ToEntity());
            Assert.Null(((E.ApiResource)null).ToModel());
            Assert.Null(((M.ApiResource)null).ToEntity());
            Assert.Null(((E.ApiScope)null).ToModel());
            Assert.Null(((M.ApiScope)null).ToEntity());
            Assert.Null(((E.IdentityResource)null).ToModel());
            Assert.Null(((M.IdentityResource)null).ToEntity());
            Assert.Null(((E.PersistedGrant)null).ToModel());
            Assert.Null(((M.PersistedGrant)null).ToEntity());
        }

        [Fact]
        public void GrantUpdate_PreservesInstanceAndCopiesAllFieldsIncludingNull()
        {
            var entity = new E.PersistedGrant();
            var same = entity;
            var model = new M.PersistedGrant();
            foreach (var name in GrantScalars.Split(' '))
            {
                var property = typeof(M.PersistedGrant).GetProperty(name);
                property.SetValue(model, TestValue(property.PropertyType, name, false));
            }
            model.UpdateEntity(entity);
            Assert.Same(same, entity);
            foreach (var name in GrantScalars.Split(' '))
                Assert.Equal(typeof(M.PersistedGrant).GetProperty(name).GetValue(model), typeof(E.PersistedGrant).GetProperty(name).GetValue(entity));
            new M.PersistedGrant().UpdateEntity(entity);
            Assert.Null(entity.Key);
            Assert.Null(entity.Expiration);
            Assert.Null(entity.ConsumedTime);
            Assert.Null(entity.Data);
        }

        [Fact]
        public void ClaimValueType_IsStringAfterReadingEntity()
        {
            var model = new M.Client { Claims = new List<M.ClientClaim> { new M.ClientClaim("number", "42", ClaimValueTypes.Integer) } };
            var entity = model.ToEntity();
            Assert.Equal("number", entity.Claims.Single().Type);
            Assert.Equal("42", entity.Claims.Single().Value);
            Assert.Equal(ClaimValueTypes.String, entity.ToModel().Claims.Single().ValueType);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("RS256,,RS256, ES256 ,rs256,")]
        public void SigningAlgorithms_PreserveTokenWhitespaceAndCase(string text)
        {
            var expected = String.IsNullOrWhiteSpace(text) ? new string[0] : text.Trim().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
            var client = new E.Client { AllowedIdentityTokenSigningAlgorithms = text }.ToModel();
            var resource = new E.ApiResource { AllowedAccessTokenSigningAlgorithms = text }.ToModel();
            Assert.Equal(expected, client.AllowedIdentityTokenSigningAlgorithms);
            Assert.Equal(expected, resource.AllowedAccessTokenSigningAlgorithms);
            Assert.IsType<List<string>>(client.AllowedIdentityTokenSigningAlgorithms);
            Assert.IsType<List<string>>(resource.AllowedAccessTokenSigningAlgorithms);
        }

        [Fact]
        public void SigningAlgorithms_WriteInCollectionOrder()
        {
            var values = new List<string> { "RS256", null, "", " ES256 ", "RS256" };
            Assert.Equal("RS256,,, ES256 ,RS256", new M.Client { AllowedIdentityTokenSigningAlgorithms = values }.ToEntity().AllowedIdentityTokenSigningAlgorithms);
            Assert.Equal("RS256,,, ES256 ,RS256", new M.ApiResource { AllowedAccessTokenSigningAlgorithms = values }.ToEntity().AllowedAccessTokenSigningAlgorithms);
            Assert.Null(new M.Client { AllowedIdentityTokenSigningAlgorithms = null }.ToEntity().AllowedIdentityTokenSigningAlgorithms);
            Assert.Null(new M.Client { AllowedIdentityTokenSigningAlgorithms = new List<string> { null } }.ToEntity().AllowedIdentityTokenSigningAlgorithms);
            Assert.Null(new M.ApiResource { AllowedAccessTokenSigningAlgorithms = new List<string> { null } }.ToEntity().AllowedAccessTokenSigningAlgorithms);
            Assert.Null(new M.ApiResource().ToEntity().AllowedAccessTokenSigningAlgorithms);
        }

        private static object TestValue(Type type, string name, bool zero)
        {
            if (Nullable.GetUnderlyingType(type) != null) return zero ? null : TestValue(Nullable.GetUnderlyingType(type), name, false);
            if (type == typeof(string)) return zero ? null : "mapped-" + name;
            if (type == typeof(bool)) return !zero;
            if (type == typeof(int)) return zero ? 0 : 937;
            if (type == typeof(DateTime)) return zero ? default(DateTime) : new DateTime(2030, 2, 3, 4, 5, 6, DateTimeKind.Utc);
            if (type.IsEnum) return Enum.ToObject(type, zero ? 0 : 937);
            throw new InvalidOperationException("Unclassified scalar " + name);
        }

        private static object Convert(string group, object source, bool reverse)
        {
            switch (group)
            {
                case "Client": return reverse ? (object)((M.Client)source).ToEntity() : ((E.Client)source).ToModel();
                case "ApiResource": return reverse ? (object)((M.ApiResource)source).ToEntity() : ((E.ApiResource)source).ToModel();
                case "ApiScope": return reverse ? (object)((M.ApiScope)source).ToEntity() : ((E.ApiScope)source).ToModel();
                case "IdentityResource": return reverse ? (object)((M.IdentityResource)source).ToEntity() : ((E.IdentityResource)source).ToModel();
                case "PersistedGrant": return reverse ? (object)((M.PersistedGrant)source).ToEntity() : ((E.PersistedGrant)source).ToModel();
                default: throw new ArgumentOutOfRangeException(nameof(group));
            }
        }

        [Fact]
        public void ExplicitNullMembers_OverwriteDefaultsAndNullGrantUpdateIsNoOp()
        {
            var client = new E.Client { ProtocolType = null, ClientSecrets = new List<E.ClientSecret> { new E.ClientSecret { Type = null } } }.ToModel();
            Assert.Null(client.ProtocolType);
            Assert.Null(client.ClientSecrets.Single().Type);
            Assert.Null(new E.ApiResource { Secrets = new List<E.ApiResourceSecret> { new E.ApiResourceSecret { Type = null } } }.ToModel().ApiSecrets.Single().Type);
            Assert.Null(new M.Client { ProtocolType = null, ClientSecrets = new List<M.Secret> { new M.Secret { Type = null } } }.ToEntity().ClientSecrets.Single().Type);
            var grant = new E.PersistedGrant { Key = "keep" };
            ((M.PersistedGrant)null).UpdateEntity(grant);
            Assert.Equal("keep", grant.Key);
            new M.PersistedGrant().UpdateEntity(null);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void EmptyAndWhitespaceMembers_AreNotReplacedByDefaults(string value)
        {
            var entity = new E.Client { ProtocolType = value, ClientSecrets = new List<E.ClientSecret> { new E.ClientSecret { Type = value } } };
            Assert.Equal(value, entity.ToModel().ProtocolType);
            Assert.Equal(value, entity.ToModel().ClientSecrets.Single().Type);
            var model = new M.Client { ProtocolType = value, ClientSecrets = new List<M.Secret> { new M.Secret { Type = value } } };
            Assert.Equal(value, model.ToEntity().ProtocolType);
            Assert.Equal(value, model.ToEntity().ClientSecrets.Single().Type);
            Assert.Equal(value, new E.ApiResource { Secrets = new List<E.ApiResourceSecret> { new E.ApiResourceSecret { Type = value } } }.ToModel().ApiSecrets.Single().Type);
            Assert.Equal(value, new M.ApiResource { ApiSecrets = new List<M.Secret> { new M.Secret { Type = value } } }.ToEntity().Secrets.Single().Type);
        }
    }
}
