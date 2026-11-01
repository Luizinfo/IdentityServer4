// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Security.Claims;

namespace IdentityServer4.EntityFramework.Mappers
{
    /// <summary>
    /// Extension methods to map to/from entity/model for clients.
    /// </summary>
    public static class ClientMappers
    {
        /// <summary>
        /// Maps an entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static Models.Client ToModel(this Entities.Client entity)
        {
            return entity == null ? null : new Models.Client
            {
                Enabled = entity.Enabled,
                ClientId = entity.ClientId,
                ProtocolType = entity.ProtocolType,
                RequireClientSecret = entity.RequireClientSecret,
                ClientName = entity.ClientName,
                Description = entity.Description,
                ClientUri = entity.ClientUri,
                LogoUri = entity.LogoUri,
                RequireConsent = entity.RequireConsent,
                AllowRememberConsent = entity.AllowRememberConsent,
                AlwaysIncludeUserClaimsInIdToken = entity.AlwaysIncludeUserClaimsInIdToken,
                RequirePkce = entity.RequirePkce,
                AllowPlainTextPkce = entity.AllowPlainTextPkce,
                RequireRequestObject = entity.RequireRequestObject,
                AllowAccessTokensViaBrowser = entity.AllowAccessTokensViaBrowser,
                FrontChannelLogoutUri = entity.FrontChannelLogoutUri,
                FrontChannelLogoutSessionRequired = entity.FrontChannelLogoutSessionRequired,
                BackChannelLogoutUri = entity.BackChannelLogoutUri,
                BackChannelLogoutSessionRequired = entity.BackChannelLogoutSessionRequired,
                AllowOfflineAccess = entity.AllowOfflineAccess,
                IdentityTokenLifetime = entity.IdentityTokenLifetime,
                AccessTokenLifetime = entity.AccessTokenLifetime,
                AuthorizationCodeLifetime = entity.AuthorizationCodeLifetime,
                ConsentLifetime = entity.ConsentLifetime,
                AbsoluteRefreshTokenLifetime = entity.AbsoluteRefreshTokenLifetime,
                SlidingRefreshTokenLifetime = entity.SlidingRefreshTokenLifetime,
                RefreshTokenUsage = (Models.TokenUsage)entity.RefreshTokenUsage,
                UpdateAccessTokenClaimsOnRefresh = entity.UpdateAccessTokenClaimsOnRefresh,
                RefreshTokenExpiration = (Models.TokenExpiration)entity.RefreshTokenExpiration,
                AccessTokenType = (Models.AccessTokenType)entity.AccessTokenType,
                EnableLocalLogin = entity.EnableLocalLogin,
                IncludeJwtId = entity.IncludeJwtId,
                AlwaysSendClientClaims = entity.AlwaysSendClientClaims,
                ClientClaimsPrefix = entity.ClientClaimsPrefix,
                PairWiseSubjectSalt = entity.PairWiseSubjectSalt,
                UserSsoLifetime = entity.UserSsoLifetime,
                UserCodeType = entity.UserCodeType,
                DeviceCodeLifetime = entity.DeviceCodeLifetime,
                ClientSecrets = MappingCollections.ToList(entity.ClientSecrets, SecretMapping.ToModel),
                AllowedGrantTypes = MappingCollections.ToList(entity.AllowedGrantTypes, x => x?.GrantType),
                RedirectUris = MappingCollections.ToList(entity.RedirectUris, x => x?.RedirectUri),
                PostLogoutRedirectUris = MappingCollections.ToList(entity.PostLogoutRedirectUris, x => x?.PostLogoutRedirectUri),
                AllowedScopes = MappingCollections.ToList(entity.AllowedScopes, x => x?.Scope),
                AllowedIdentityTokenSigningAlgorithms = SigningAlgorithmMapping.FromText(entity.AllowedIdentityTokenSigningAlgorithms),
                IdentityProviderRestrictions = MappingCollections.ToList(entity.IdentityProviderRestrictions, x => x?.Provider),
                Claims = MappingCollections.ToList(entity.Claims, x => x == null ? null : new Models.ClientClaim(x.Type, x.Value, ClaimValueTypes.String)),
                AllowedCorsOrigins = MappingCollections.ToList(entity.AllowedCorsOrigins, x => x?.Origin),
                Properties = MappingCollections.ToDictionary(entity.Properties)
            };
        }

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Entities.Client ToEntity(this Models.Client model)
        {
            return model == null ? null : new Entities.Client
            {
                Enabled = model.Enabled,
                ClientId = model.ClientId,
                ProtocolType = model.ProtocolType,
                RequireClientSecret = model.RequireClientSecret,
                ClientName = model.ClientName,
                Description = model.Description,
                ClientUri = model.ClientUri,
                LogoUri = model.LogoUri,
                RequireConsent = model.RequireConsent,
                AllowRememberConsent = model.AllowRememberConsent,
                AlwaysIncludeUserClaimsInIdToken = model.AlwaysIncludeUserClaimsInIdToken,
                RequirePkce = model.RequirePkce,
                AllowPlainTextPkce = model.AllowPlainTextPkce,
                RequireRequestObject = model.RequireRequestObject,
                AllowAccessTokensViaBrowser = model.AllowAccessTokensViaBrowser,
                FrontChannelLogoutUri = model.FrontChannelLogoutUri,
                FrontChannelLogoutSessionRequired = model.FrontChannelLogoutSessionRequired,
                BackChannelLogoutUri = model.BackChannelLogoutUri,
                BackChannelLogoutSessionRequired = model.BackChannelLogoutSessionRequired,
                AllowOfflineAccess = model.AllowOfflineAccess,
                IdentityTokenLifetime = model.IdentityTokenLifetime,
                AccessTokenLifetime = model.AccessTokenLifetime,
                AuthorizationCodeLifetime = model.AuthorizationCodeLifetime,
                ConsentLifetime = model.ConsentLifetime,
                AbsoluteRefreshTokenLifetime = model.AbsoluteRefreshTokenLifetime,
                SlidingRefreshTokenLifetime = model.SlidingRefreshTokenLifetime,
                RefreshTokenUsage = (int)model.RefreshTokenUsage,
                UpdateAccessTokenClaimsOnRefresh = model.UpdateAccessTokenClaimsOnRefresh,
                RefreshTokenExpiration = (int)model.RefreshTokenExpiration,
                AccessTokenType = (int)model.AccessTokenType,
                EnableLocalLogin = model.EnableLocalLogin,
                IncludeJwtId = model.IncludeJwtId,
                AlwaysSendClientClaims = model.AlwaysSendClientClaims,
                ClientClaimsPrefix = model.ClientClaimsPrefix,
                PairWiseSubjectSalt = model.PairWiseSubjectSalt,
                UserSsoLifetime = model.UserSsoLifetime,
                UserCodeType = model.UserCodeType,
                DeviceCodeLifetime = model.DeviceCodeLifetime,
                ClientSecrets = MappingCollections.ToList(model.ClientSecrets, SecretMapping.ToEntity<Entities.ClientSecret>),
                AllowedGrantTypes = MappingCollections.ToList(model.AllowedGrantTypes, x => x == null ? null : new Entities.ClientGrantType { GrantType = x }),
                RedirectUris = MappingCollections.ToList(model.RedirectUris, x => x == null ? null : new Entities.ClientRedirectUri { RedirectUri = x }),
                PostLogoutRedirectUris = MappingCollections.ToList(model.PostLogoutRedirectUris, x => x == null ? null : new Entities.ClientPostLogoutRedirectUri { PostLogoutRedirectUri = x }),
                AllowedScopes = MappingCollections.ToList(model.AllowedScopes, x => x == null ? null : new Entities.ClientScope { Scope = x }),
                AllowedIdentityTokenSigningAlgorithms = SigningAlgorithmMapping.ToText(model.AllowedIdentityTokenSigningAlgorithms),
                IdentityProviderRestrictions = MappingCollections.ToList(model.IdentityProviderRestrictions, x => x == null ? null : new Entities.ClientIdPRestriction { Provider = x }),
                Claims = MappingCollections.ToList(model.Claims, x => x == null ? null : new Entities.ClientClaim { Type = x.Type, Value = x.Value }),
                AllowedCorsOrigins = MappingCollections.ToList(model.AllowedCorsOrigins, x => x == null ? null : new Entities.ClientCorsOrigin { Origin = x }),
                Properties = MappingCollections.ToList(model.Properties, x => new Entities.ClientProperty { Key = x.Key, Value = x.Value })
            };
        }
    }
}
