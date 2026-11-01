using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using IdentityServer4.EntityFramework.Mappers;
using Xunit;
using E = IdentityServer4.EntityFramework.Entities;
using M = IdentityServer4.Models;

namespace IdentityServer4.EntityFramework.UnitTests.Mappers
{
    public class CollectionMappingTests
    {
        private static readonly DateTime Expiry = new DateTime(2030, 1, 2, 3, 4, 5, DateTimeKind.Utc);

        [Fact]
        public void ClientChildren_MapEachDirectionIndependently()
        {
            var model = ClientModel();
            var entity = model.ToEntity();
            Assert.Equal("code", entity.AllowedGrantTypes.Single().GrantType);
            Assert.Equal("scope", entity.AllowedScopes.Single().Scope);
            Assert.Equal("redirect", entity.RedirectUris.Single().RedirectUri);
            Assert.Equal("logout", entity.PostLogoutRedirectUris.Single().PostLogoutRedirectUri);
            Assert.Equal("origin", entity.AllowedCorsOrigins.Single().Origin);
            Assert.Equal("provider", entity.IdentityProviderRestrictions.Single().Provider);
            Assert.Equal("claim", entity.Claims.Single().Type);
            Assert.Equal("value", entity.Claims.Single().Value);
            Assert.Equal("key", entity.Properties.Single().Key);
            Assert.Equal("property", entity.Properties.Single().Value);
            AssertSecret(entity.ClientSecrets.Single());
            Assert.Equal(0, entity.Id);
            Assert.Equal(0, entity.ClientSecrets.Single().ClientId);
            Assert.Null(entity.ClientSecrets.Single().Client);

            var source = ClientEntity();
            var actual = source.ToModel();
            actual.Should().BeEquivalentTo(ClientModel());
            Assert.IsType<List<M.Secret>>(actual.ClientSecrets);
            Assert.IsType<List<M.ClientClaim>>(actual.Claims);
            Assert.IsType<List<string>>(actual.RedirectUris);
            actual.Properties["key"] = "changed";
            actual.ClientSecrets.Single().Value = "changed";
            Assert.Equal("property", source.Properties.Single().Value);
            Assert.Equal("secret", source.ClientSecrets.Single().Value);
            entity.ClientSecrets.Single().Value = "changed";
            Assert.Equal("secret", model.ClientSecrets.Single().Value);
        }

        [Fact]
        public void ResourceChildren_MapEachDirectionIndependently()
        {
            var apiModel = new M.ApiResource { UserClaims = new[] { "claim", "claim" }, Scopes = new[] { "scope", "scope" }, Properties = Properties(), ApiSecrets = new[] { ModelSecret() } };
            var api = apiModel.ToEntity();
            Assert.Equal(new[] { "claim", "claim" }, api.UserClaims.Select(x => x.Type));
            Assert.Equal(new[] { "scope", "scope" }, api.Scopes.Select(x => x.Scope));
            Assert.Equal("property", api.Properties.Single().Value);
            AssertSecret(api.Secrets.Single());
            var apiSource = new E.ApiResource
            {
                UserClaims = new List<E.ApiResourceClaim> { new E.ApiResourceClaim { Type = "claim" }, new E.ApiResourceClaim { Type = "claim" } },
                Scopes = new List<E.ApiResourceScope> { new E.ApiResourceScope { Scope = "scope" }, new E.ApiResourceScope { Scope = "scope" } },
                Properties = new List<E.ApiResourceProperty> { new E.ApiResourceProperty { Key = "key", Value = "property" } },
                Secrets = new List<E.ApiResourceSecret> { new E.ApiResourceSecret { Type = "type", Value = "secret", Description = "description", Expiration = Expiry } }
            };
            apiSource.ToModel().Should().BeEquivalentTo(apiModel);
            api.Secrets.Single().Value = "changed";
            Assert.Equal("secret", apiModel.ApiSecrets.Single().Value);

            var scopeModel = new M.ApiScope { UserClaims = new[] { "claim", "claim" }, Properties = Properties() };
            var scope = scopeModel.ToEntity();
            Assert.Equal(new[] { "claim", "claim" }, scope.UserClaims.Select(x => x.Type));
            Assert.Equal("property", scope.Properties.Single().Value);
            new E.ApiScope { UserClaims = new List<E.ApiScopeClaim> { new E.ApiScopeClaim { Type = "claim" }, new E.ApiScopeClaim { Type = "claim" } }, Properties = new List<E.ApiScopeProperty> { new E.ApiScopeProperty { Key = "key", Value = "property" } } }.ToModel().Should().BeEquivalentTo(scopeModel);

            var identityModel = new M.IdentityResource { UserClaims = new[] { "claim", "claim" }, Properties = Properties() };
            var identity = identityModel.ToEntity();
            Assert.Equal(new[] { "claim", "claim" }, identity.UserClaims.Select(x => x.Type));
            Assert.Equal("property", identity.Properties.Single().Value);
            new E.IdentityResource { UserClaims = new List<E.IdentityResourceClaim> { new E.IdentityResourceClaim { Type = "claim" }, new E.IdentityResourceClaim { Type = "claim" } }, Properties = new List<E.IdentityResourceProperty> { new E.IdentityResourceProperty { Key = "key", Value = "property" } } }.ToModel().Should().BeEquivalentTo(identityModel);
        }

        [Fact]
        public void NullCollections_BecomeEmptyListsOrDictionaries()
        {
            var client = new E.Client().ToModel();
            Assert.Empty(client.AllowedGrantTypes);
            Assert.IsType<List<string>>(client.AllowedScopes);
            Assert.Empty(client.AllowedScopes);
            Assert.Empty(client.ClientSecrets);
            Assert.Empty(client.Claims);
            Assert.Empty(client.Properties);
            foreach (var model in new object[] { new M.Client(), new M.ApiResource(), new M.ApiScope(), new M.IdentityResource() })
            {
                foreach (var property in model.GetType().GetProperties().Where(x => x.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(x.PropertyType)))
                {
                    // AllowedGrantTypes rejects a null assignment in the model itself.
                    if (property.Name != "AllowedGrantTypes") property.SetValue(model, null);
                }
                object entity = model is M.Client c ? (object)c.ToEntity() : model is M.ApiResource a ? a.ToEntity() : model is M.ApiScope s ? s.ToEntity() : ((M.IdentityResource)model).ToEntity();
                foreach (var property in entity.GetType().GetProperties().Where(x => typeof(IList).IsAssignableFrom(x.PropertyType)))
                    Assert.Empty(Assert.IsAssignableFrom<IEnumerable>(property.GetValue(entity)));
            }
        }

        [Fact]
        public void DuplicateAndCaseSensitiveProperties_PreserveDictionaryRules()
        {
            Assert.ThrowsAny<Exception>(() => new E.ApiScope { Properties = new List<E.ApiScopeProperty> { new E.ApiScopeProperty { Key = "same" }, new E.ApiScopeProperty { Key = "same" } } }.ToModel());
            Assert.ThrowsAny<Exception>(() => new E.ApiResource { Properties = new List<E.ApiResourceProperty> { new E.ApiResourceProperty { Key = "same" }, new E.ApiResourceProperty { Key = "same" } } }.ToModel());
            Assert.ThrowsAny<Exception>(() => new E.IdentityResource { Properties = new List<E.IdentityResourceProperty> { new E.IdentityResourceProperty { Key = "same" }, new E.IdentityResourceProperty { Key = "same" } } }.ToModel());
            Assert.Equal(2, new E.Client { Properties = new List<E.ClientProperty> { new E.ClientProperty { Key = "A" }, new E.ClientProperty { Key = "a" } } }.ToModel().Properties.Count);
            Assert.ThrowsAny<Exception>(() => new E.Client { Properties = new List<E.ClientProperty> { new E.ClientProperty { Key = null } } }.ToModel());
        }

        [Fact]
        public void NullChildren_PreserveNullElements()
        {
            var client = new E.Client { ClientSecrets = new List<E.ClientSecret> { null }, Claims = new List<E.ClientClaim> { null }, RedirectUris = new List<E.ClientRedirectUri> { null } }.ToModel();
            Assert.Null(client.ClientSecrets.Single());
            Assert.Null(client.Claims.Single());
            Assert.Null(client.RedirectUris.Single());
            var entity = new M.Client { ClientSecrets = new List<M.Secret> { null }, Claims = new List<M.ClientClaim> { null }, RedirectUris = new List<string> { null } }.ToEntity();
            Assert.Null(entity.ClientSecrets.Single());
            Assert.Null(entity.Claims.Single());
            Assert.Null(entity.RedirectUris.Single());
            Assert.ThrowsAny<Exception>(() => new E.Client { Properties = new List<E.ClientProperty> { null } }.ToModel());
        }

        [Fact]
        public void InvalidGrantTypes_StillFailModelValidation()
        {
            Assert.ThrowsAny<Exception>(() => new E.Client { AllowedGrantTypes = new List<E.ClientGrantType> { new E.ClientGrantType { GrantType = "code" }, new E.ClientGrantType { GrantType = "code" } } }.ToModel());
            Assert.ThrowsAny<Exception>(() => new E.Client { AllowedGrantTypes = new List<E.ClientGrantType> { new E.ClientGrantType { GrantType = "with space" } } }.ToModel());
            Assert.ThrowsAny<Exception>(() => new E.Client { AllowedGrantTypes = new List<E.ClientGrantType> { null } }.ToModel());
        }

        [Fact]
        public void ResourceNullChildren_PreserveNullElementsInEachDirection()
        {
            var apiModel = new E.ApiResource { Secrets = new List<E.ApiResourceSecret> { null }, Scopes = new List<E.ApiResourceScope> { null }, UserClaims = new List<E.ApiResourceClaim> { null } }.ToModel();
            Assert.Null(apiModel.ApiSecrets.Single());
            Assert.Null(apiModel.Scopes.Single());
            Assert.Null(apiModel.UserClaims.Single());
            var apiEntity = new M.ApiResource { ApiSecrets = new List<M.Secret> { null }, Scopes = new List<string> { null }, UserClaims = new List<string> { null } }.ToEntity();
            Assert.Null(apiEntity.Secrets.Single());
            Assert.Null(apiEntity.Scopes.Single());
            Assert.Null(apiEntity.UserClaims.Single());
            Assert.Null(new E.ApiScope { UserClaims = new List<E.ApiScopeClaim> { null } }.ToModel().UserClaims.Single());
            Assert.Null(new M.ApiScope { UserClaims = new List<string> { null } }.ToEntity().UserClaims.Single());
            Assert.Null(new E.IdentityResource { UserClaims = new List<E.IdentityResourceClaim> { null } }.ToModel().UserClaims.Single());
            Assert.Null(new M.IdentityResource { UserClaims = new List<string> { null } }.ToEntity().UserClaims.Single());
        }

        [Fact]
        public void ChildProperties_AreExplicitlyClassified()
        {
            AssertNames(typeof(M.Secret), "Description Value Expiration Type");
            AssertNames(typeof(M.ClientClaim), "Type Value ValueType");
            AssertNames(typeof(E.ClientSecret), "Id Description Value Expiration Type Created ClientId Client");
            AssertNames(typeof(E.ApiResourceSecret), "Id Description Value Expiration Type Created ApiResourceId ApiResource");
            AssertNames(typeof(E.ClientClaim), "Id Type Value ClientId Client");
            AssertNames(typeof(E.ClientProperty), "Id Key Value ClientId Client");
            AssertNames(typeof(E.ApiResourceProperty), "Id Key Value ApiResourceId ApiResource");
            AssertNames(typeof(E.ApiScopeProperty), "Id Key Value ScopeId Scope");
            AssertNames(typeof(E.IdentityResourceProperty), "Id Key Value IdentityResourceId IdentityResource");
            AssertNames(typeof(E.ApiResourceClaim), "Id Type ApiResourceId ApiResource");
            AssertNames(typeof(E.ApiScopeClaim), "Id Type ScopeId Scope");
            AssertNames(typeof(E.IdentityResourceClaim), "Id Type IdentityResourceId IdentityResource");
            AssertNames(typeof(E.ClientCorsOrigin), "Id Origin ClientId Client");
            AssertNames(typeof(E.ClientIdPRestriction), "Id Provider ClientId Client");
            AssertNames(typeof(E.ClientScope), "Id Scope ClientId Client");
            AssertNames(typeof(E.ClientGrantType), "Id GrantType ClientId Client");
            AssertNames(typeof(E.ClientRedirectUri), "Id RedirectUri ClientId Client");
            AssertNames(typeof(E.ClientPostLogoutRedirectUri), "Id PostLogoutRedirectUri ClientId Client");
            AssertNames(typeof(E.ApiResourceScope), "Id Scope ApiResourceId ApiResource");
        }

        private static void AssertNames(Type type, string names) => Assert.Equal(names.Split(' ').OrderBy(x => x), type.GetProperties().Select(x => x.Name).OrderBy(x => x));
        private static Dictionary<string, string> Properties() => new Dictionary<string, string> { ["key"] = "property" };
        private static M.Secret ModelSecret() => new M.Secret { Type = "type", Value = "secret", Description = "description", Expiration = Expiry };
        private static void AssertSecret(E.Secret secret)
        {
            Assert.Equal("type", secret.Type);
            Assert.Equal("secret", secret.Value);
            Assert.Equal("description", secret.Description);
            Assert.Equal(Expiry, secret.Expiration);
            Assert.Equal(0, secret.Id);
        }

        private static M.Client ClientModel() => new M.Client
        {
            IncludeJwtId = false,
            AllowedGrantTypes = new List<string> { "code" }, AllowedScopes = new List<string> { "scope" },
            RedirectUris = new List<string> { "redirect" }, PostLogoutRedirectUris = new List<string> { "logout" },
            AllowedCorsOrigins = new List<string> { "origin" }, IdentityProviderRestrictions = new List<string> { "provider" },
            Claims = new List<M.ClientClaim> { new M.ClientClaim("claim", "value") }, Properties = Properties(), ClientSecrets = new List<M.Secret> { ModelSecret() }
        };

        private static E.Client ClientEntity() => new E.Client
        {
            AllowedGrantTypes = new List<E.ClientGrantType> { new E.ClientGrantType { GrantType = "code" } },
            AllowedScopes = new List<E.ClientScope> { new E.ClientScope { Scope = "scope" } },
            RedirectUris = new List<E.ClientRedirectUri> { new E.ClientRedirectUri { RedirectUri = "redirect" } },
            PostLogoutRedirectUris = new List<E.ClientPostLogoutRedirectUri> { new E.ClientPostLogoutRedirectUri { PostLogoutRedirectUri = "logout" } },
            AllowedCorsOrigins = new List<E.ClientCorsOrigin> { new E.ClientCorsOrigin { Origin = "origin" } },
            IdentityProviderRestrictions = new List<E.ClientIdPRestriction> { new E.ClientIdPRestriction { Provider = "provider" } },
            Claims = new List<E.ClientClaim> { new E.ClientClaim { Type = "claim", Value = "value" } },
            Properties = new List<E.ClientProperty> { new E.ClientProperty { Key = "key", Value = "property" } },
            ClientSecrets = new List<E.ClientSecret> { new E.ClientSecret { Type = "type", Value = "secret", Description = "description", Expiration = Expiry } }
        };
    }
}
