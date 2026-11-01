namespace IdentityServer4.EntityFramework.Mappers
{
    internal static class SecretMapping
    {
        internal static Models.Secret ToModel(Entities.Secret source)
        {
            return source == null ? null : new Models.Secret
            {
                Description = source.Description,
                Value = source.Value,
                Expiration = source.Expiration,
                Type = source.Type
            };
        }

        internal static TSecret ToEntity<TSecret>(Models.Secret source) where TSecret : Entities.Secret, new()
        {
            return source == null ? null : new TSecret
            {
                Description = source.Description,
                Value = source.Value,
                Expiration = source.Expiration,
                Type = source.Type
            };
        }
    }
}
