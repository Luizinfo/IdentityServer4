using System;
using System.Collections.Generic;
using System.Linq;

namespace IdentityServer4.EntityFramework.Mappers
{
    internal static class SigningAlgorithmMapping
    {
        internal static string ToText(ICollection<string> algorithms)
        {
            if (algorithms == null || algorithms.Count == 0) return null;
            // A single null item historically produces null, rather than an empty string.
            return algorithms.Count == 1 ? algorithms.First() : string.Join(",", algorithms);
        }

        internal static List<string> FromText(string algorithms)
        {
            return string.IsNullOrWhiteSpace(algorithms)
                ? new List<string>()
                : algorithms.Trim().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
        }
    }
}
