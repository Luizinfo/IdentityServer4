// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.EntityFramework.Entities;

namespace IdentityServer4.EntityFramework.Mappers
{
    /// <summary>
    /// Extension methods to map to/from entity/model for identity resources.
    /// </summary>
    public static class IdentityResourceMappers
    {
        /// <summary>
        /// Maps an entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static Models.IdentityResource ToModel(this IdentityResource entity)
        {
            return entity == null ? null : new Models.IdentityResource
            {
                Enabled = entity.Enabled,
                Name = entity.Name,
                DisplayName = entity.DisplayName,
                Description = entity.Description,
                Required = entity.Required,
                Emphasize = entity.Emphasize,
                ShowInDiscoveryDocument = entity.ShowInDiscoveryDocument,
                UserClaims = MappingCollections.ToList(entity.UserClaims, x => x?.Type),
                Properties = MappingCollections.ToDictionary(entity.Properties)
            };
        }

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static IdentityResource ToEntity(this Models.IdentityResource model)
        {
            return model == null ? null : new IdentityResource
            {
                Enabled = model.Enabled,
                Name = model.Name,
                DisplayName = model.DisplayName,
                Description = model.Description,
                Required = model.Required,
                Emphasize = model.Emphasize,
                ShowInDiscoveryDocument = model.ShowInDiscoveryDocument,
                UserClaims = MappingCollections.ToList(model.UserClaims, x => x == null ? null : new IdentityResourceClaim { Type = x }),
                Properties = MappingCollections.ToList(model.Properties, x => new IdentityResourceProperty { Key = x.Key, Value = x.Value })
            };
        }
    }
}
