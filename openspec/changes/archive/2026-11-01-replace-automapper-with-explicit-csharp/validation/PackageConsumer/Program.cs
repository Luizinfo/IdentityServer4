using System;
using System.Collections.Generic;
using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Options;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

var assembly = typeof(ClientMappers).Assembly;
if (assembly.GetName().Version.Major != 11) throw new Exception("Unexpected storage assembly version.");
if (assembly.GetReferencedAssemblies().Any(x => x.Name == "AutoMapper")) throw new Exception("AutoMapper assembly dependency found.");
if (assembly.GetExportedTypes().Any(x => x.Name.EndsWith("MapperProfile", StringComparison.Ordinal))) throw new Exception("Mapper profiles still exported.");
if (typeof(Microsoft.Extensions.DependencyInjection.TokenCleanupHost).Assembly.GetName().Version.Major != 11) throw new Exception("Unexpected EntityFramework assembly version.");

using var connection = new SqliteConnection("Data Source=:memory:");
await connection.OpenAsync();
var options = new DbContextOptionsBuilder<PersistedGrantDbContext>().UseSqlite(connection).Options;
using (var context = new PersistedGrantDbContext(options, new OperationalStoreOptions()))
{
    await context.Database.EnsureCreatedAsync();
    var store = new PersistedGrantStore(context, NullLogger<PersistedGrantStore>.Instance);
    await store.StoreAsync(new PersistedGrant { Key = "key", Type = "type", ClientId = "client", CreationTime = new DateTime(2030, 1, 2), Data = "old", Expiration = new DateTime(2031, 1, 2) });
    await store.StoreAsync(new PersistedGrant { Key = "key", Type = "type", ClientId = "client", CreationTime = new DateTime(2030, 1, 2), Data = "new", Expiration = null });
}
using (var context = new PersistedGrantDbContext(options, new OperationalStoreOptions()))
{
    var grant = (await context.PersistedGrants.SingleAsync()).ToModel();
    if (grant.Data != "new" || grant.Expiration != null) throw new Exception("Grant update failed.");
}

var client = new Client { ClientId = "client", AllowedGrantTypes = GrantTypes.Code, AllowedScopes = { "scope" }, ClientSecrets = { new Secret("hash") }, Properties = { ["key"] = "value" } }.ToEntity().ToModel();
if (client.AllowedScopes.Single() != "scope" || client.ClientSecrets.Single().Value != "hash" || client.Properties["key"] != "value") throw new Exception("Client mapping failed.");
var api = new ApiResource("api") { Scopes = { "scope" }, ApiSecrets = { new Secret("api-hash") }, AllowedAccessTokenSigningAlgorithms = new List<string> { "RS256" } }.ToEntity().ToModel();
if (api.ApiSecrets.Single().Value != "api-hash" || api.AllowedAccessTokenSigningAlgorithms.Single() != "RS256") throw new Exception("API mapping failed.");
if (new ApiScope("scope") { UserClaims = { "claim" } }.ToEntity().ToModel().UserClaims.Single() != "claim") throw new Exception("Scope mapping failed.");
if (new IdentityResource("identity", new[] { "claim" }).ToEntity().ToModel().UserClaims.Single() != "claim") throw new Exception("Identity mapping failed.");
Console.WriteLine("Package consumer: EntityFramework 11.0.0 and Storage 11.0.0; five mapping groups and SQLite grant update passed; no AutoMapper assembly reference.");
