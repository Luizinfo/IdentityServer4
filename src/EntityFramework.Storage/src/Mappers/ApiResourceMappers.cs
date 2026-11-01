// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.EntityFramework.Entities;

namespace IdentityServer4.EntityFramework.Mappers
{
    /// <summary>
    /// Extension methods to map to/from entity/model for API resources.
    /// </summary>
    public static class ApiResourceMappers
    {
        /// <summary>
        /// Maps an entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static Models.ApiResource ToModel(this ApiResource entity)
        {
            return entity == null ? null : new Models.ApiResource
            {
                Enabled = entity.Enabled,
                Name = entity.Name,
                DisplayName = entity.DisplayName,
                Description = entity.Description,
                ShowInDiscoveryDocument = entity.ShowInDiscoveryDocument,
                ApiSecrets = MappingCollections.ToList(entity.Secrets, SecretMapping.ToModel),
                Scopes = MappingCollections.ToList(entity.Scopes, x => x?.Scope),
                UserClaims = MappingCollections.ToList(entity.UserClaims, x => x?.Type),
                Properties = MappingCollections.ToDictionary(entity.Properties),
                AllowedAccessTokenSigningAlgorithms = SigningAlgorithmMapping.FromText(entity.AllowedAccessTokenSigningAlgorithms)
            };
        }

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static ApiResource ToEntity(this Models.ApiResource model)
        {
            return model == null ? null : new ApiResource
            {
                Enabled = model.Enabled,
                Name = model.Name,
                DisplayName = model.DisplayName,
                Description = model.Description,
                ShowInDiscoveryDocument = model.ShowInDiscoveryDocument,
                Secrets = MappingCollections.ToList(model.ApiSecrets, SecretMapping.ToEntity<ApiResourceSecret>),
                Scopes = MappingCollections.ToList(model.Scopes, x => x == null ? null : new ApiResourceScope { Scope = x }),
                UserClaims = MappingCollections.ToList(model.UserClaims, x => x == null ? null : new ApiResourceClaim { Type = x }),
                Properties = MappingCollections.ToList(model.Properties, x => new ApiResourceProperty { Key = x.Key, Value = x.Value }),
                AllowedAccessTokenSigningAlgorithms = SigningAlgorithmMapping.ToText(model.AllowedAccessTokenSigningAlgorithms)
            };
        }
    }
}
