using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Options;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace IdentityServer4.EntityFramework.IntegrationTests.Stores
{
    public class ExplicitMappingRelationalTests
    {
        [Fact]
        public async Task ClientGraph_PersistsWithGeneratedKeysAndReadsInNewContext()
        {
            using var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            var options = new DbContextOptionsBuilder<ConfigurationDbContext>().UseSqlite(connection).Options;
            var model = new Client
            {
                ClientId = "explicit-client", AllowedGrantTypes = GrantTypes.Code,
                AllowedScopes = new List<string> { "scope" }, RedirectUris = new List<string> { "https://client.test/callback" },
                PostLogoutRedirectUris = new List<string> { "https://client.test/logout" }, AllowedCorsOrigins = new List<string> { "https://client.test" },
                IdentityProviderRestrictions = new List<string> { "provider" }, ClientSecrets = new List<Secret> { new Secret("hashed-secret") },
                Claims = new List<ClientClaim> { new ClientClaim("tenant", "one") }, Properties = new Dictionary<string, string> { ["key"] = "value" },
                AllowedIdentityTokenSigningAlgorithms = new List<string> { "RS256", "ES256" }
            };
            int id;
            using (var context = new ConfigurationDbContext(options, new ConfigurationStoreOptions()))
            {
                await context.Database.EnsureCreatedAsync();
                var entity = model.ToEntity();
                context.Clients.Add(entity);
                await context.SaveChangesAsync();
                id = entity.Id;
                Assert.True(id > 0);
                Assert.True(entity.ClientSecrets.Single().Id > 0);
                Assert.Equal(id, entity.ClientSecrets.Single().ClientId);
                Assert.Same(entity, entity.ClientSecrets.Single().Client);
            }
            using (var context = new ConfigurationDbContext(options, new ConfigurationStoreOptions()))
            {
                var actual = await new ClientStore(context, NullLogger<ClientStore>.Instance).FindClientByIdAsync(model.ClientId);
                Assert.Single(await context.Clients.ToListAsync());
                Assert.Equal(id, (await context.Clients.SingleAsync()).Id);
                Assert.Equal(model.AllowedScopes, actual.AllowedScopes);
                Assert.Equal(model.RedirectUris, actual.RedirectUris);
                Assert.Equal(model.PostLogoutRedirectUris, actual.PostLogoutRedirectUris);
                Assert.Equal(model.AllowedCorsOrigins, actual.AllowedCorsOrigins);
                Assert.Equal(model.IdentityProviderRestrictions, actual.IdentityProviderRestrictions);
                Assert.Equal(model.AllowedGrantTypes, actual.AllowedGrantTypes);
                Assert.Equal(model.AllowedIdentityTokenSigningAlgorithms, actual.AllowedIdentityTokenSigningAlgorithms);
                Assert.Equal("hashed-secret", actual.ClientSecrets.Single().Value);
                Assert.Equal("one", actual.Claims.Single().Value);
                Assert.Equal("value", actual.Properties["key"]);
            }
        }

        [Fact]
        public async Task ResourceGraphs_PersistAndReadAllChildrenInNewContext()
        {
            using var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            var options = new DbContextOptionsBuilder<ConfigurationDbContext>().UseSqlite(connection).Options;
            using (var context = new ConfigurationDbContext(options, new ConfigurationStoreOptions()))
            {
                await context.Database.EnsureCreatedAsync();
                context.ApiResources.Add(new ApiResource("api")
                {
                    Scopes = { "scope" }, UserClaims = { "api-claim" }, ApiSecrets = { new Secret("api-secret") },
                    Properties = { ["api-key"] = "api-value" }, AllowedAccessTokenSigningAlgorithms = { "RS256" }
                }.ToEntity());
                context.ApiScopes.Add(new ApiScope("scope") { UserClaims = { "scope-claim" }, Properties = { ["scope-key"] = "scope-value" } }.ToEntity());
                context.IdentityResources.Add(new IdentityResource("identity", new[] { "identity-claim" }) { Properties = { ["identity-key"] = "identity-value" } }.ToEntity());
                await context.SaveChangesAsync();
            }
            using (var context = new ConfigurationDbContext(options, new ConfigurationStoreOptions()))
            {
                var api = await context.ApiResources.Include(x => x.Scopes).Include(x => x.UserClaims).Include(x => x.Secrets).Include(x => x.Properties).SingleAsync();
                var scope = await context.ApiScopes.Include(x => x.UserClaims).Include(x => x.Properties).SingleAsync();
                var identity = await context.IdentityResources.Include(x => x.UserClaims).Include(x => x.Properties).SingleAsync();
                Assert.True(api.Id > 0 && scope.Id > 0 && identity.Id > 0);
                Assert.True(api.Secrets.Single().Id > 0);
                Assert.Equal(api.Id, api.Secrets.Single().ApiResourceId);
                Assert.Equal(api.Id, api.Scopes.Single().ApiResourceId);
                Assert.Equal(api.Id, api.UserClaims.Single().ApiResourceId);
                Assert.Equal(api.Id, api.Properties.Single().ApiResourceId);
                Assert.Equal(scope.Id, scope.UserClaims.Single().ScopeId);
                Assert.Equal(scope.Id, scope.Properties.Single().ScopeId);
                Assert.Equal(identity.Id, identity.UserClaims.Single().IdentityResourceId);
                Assert.Equal(identity.Id, identity.Properties.Single().IdentityResourceId);
                Assert.Equal("api-secret", api.ToModel().ApiSecrets.Single().Value);
                Assert.Equal("scope", api.ToModel().Scopes.Single());
                Assert.Equal("api-claim", api.ToModel().UserClaims.Single());
                Assert.Equal("api-value", api.ToModel().Properties["api-key"]);
                Assert.Equal("RS256", api.ToModel().AllowedAccessTokenSigningAlgorithms.Single());
                Assert.Equal("scope-claim", scope.ToModel().UserClaims.Single());
                Assert.Equal("scope-value", scope.ToModel().Properties["scope-key"]);
                Assert.Equal("identity-claim", identity.ToModel().UserClaims.Single());
                Assert.Equal("identity-value", identity.ToModel().Properties["identity-key"]);
            }
        }

        [Fact]
        public async Task ExistingGrant_UpdatesSingleRowAndClearsNullableValues()
        {
            using var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            var options = new DbContextOptionsBuilder<PersistedGrantDbContext>().UseSqlite(connection).Options;
            var updated = new PersistedGrant
            {
                Key = "grant-key", Type = "new-type", SubjectId = "new-subject", SessionId = "new-session",
                ClientId = "new-client", Description = "new-description", CreationTime = new DateTime(2030, 1, 2),
                Expiration = null, ConsumedTime = null, Data = "new-data"
            };
            using (var context = new PersistedGrantDbContext(options, new OperationalStoreOptions()))
            {
                await context.Database.EnsureCreatedAsync();
                var entity = new PersistedGrant
                {
                    Key = updated.Key, Type = "old-type", ClientId = "old-client", Data = "old-data",
                    CreationTime = new DateTime(2020, 1, 2), Expiration = new DateTime(2021, 1, 2), ConsumedTime = new DateTime(2020, 2, 3)
                }.ToEntity();
                context.PersistedGrants.Add(entity);
                await context.SaveChangesAsync();
                await new PersistedGrantStore(context, NullLogger<PersistedGrantStore>.Instance).StoreAsync(updated);
                Assert.Same(entity, context.PersistedGrants.Local.Single());
                Assert.Equal(EntityState.Unchanged, context.Entry(entity).State);
            }
            using (var context = new PersistedGrantDbContext(options, new OperationalStoreOptions()))
            {
                var actual = (await context.PersistedGrants.SingleAsync()).ToModel();
                Assert.Equal(updated.Key, actual.Key);
                Assert.Equal(updated.Type, actual.Type);
                Assert.Equal(updated.SubjectId, actual.SubjectId);
                Assert.Equal(updated.SessionId, actual.SessionId);
                Assert.Equal(updated.ClientId, actual.ClientId);
                Assert.Equal(updated.Description, actual.Description);
                Assert.Equal(updated.CreationTime, actual.CreationTime);
                Assert.Null(actual.Expiration);
                Assert.Null(actual.ConsumedTime);
                Assert.Equal(updated.Data, actual.Data);
            }
        }
    }
}
