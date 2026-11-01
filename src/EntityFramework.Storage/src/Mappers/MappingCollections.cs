using System;
using System.Collections.Generic;
using System.Linq;

namespace IdentityServer4.EntityFramework.Mappers
{
    internal static class MappingCollections
    {
        internal static List<TDestination> ToList<TSource, TDestination>(IEnumerable<TSource> source, Func<TSource, TDestination> convert)
        {
            return source == null ? new List<TDestination>() : source.Select(convert).ToList();
        }

        internal static Dictionary<string, string> ToDictionary<TProperty>(IEnumerable<TProperty> source)
            where TProperty : Entities.Property
        {
            return source == null ? new Dictionary<string, string>() : source.ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
