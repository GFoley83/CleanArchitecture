using System;
using System.Linq;
using System.Text;

namespace CleanArchitecture.Application.Common.Helpers
{
    public static class CacheHelper
    {
        public static string GenerateKey(Type cachedQueryType, string propertiesUsedAsKeys, string propertyValues)
        {
            var propertiesToUseAsCacheKeys = Array.ConvertAll(propertiesUsedAsKeys.Split(","), p => p.Trim());
            var valuesForPropertyKeys = Array.ConvertAll(propertyValues.Split(","), p => p.Trim());

            return GenerateKey(cachedQueryType.Name, propertiesToUseAsCacheKeys,
                valuesForPropertyKeys);
        }

        public static string GenerateKey(string cacheKey, string propertiesUsedAsKeys, string propertyValues)
        {
            var propertiesToUseAsCacheKeys = Array.ConvertAll(propertiesUsedAsKeys.Split(","), p => p.Trim());
            var valuesForPropertyKeys = Array.ConvertAll(propertyValues.Split(","), p => p.Trim());

            return GenerateKey(cacheKey, propertiesToUseAsCacheKeys,
                valuesForPropertyKeys);
        }

        public static string GenerateKey(Type cachedQueryType, string[] propertiesUsedAsKeys, string[] propertyValue)
        {
            return GenerateKey(cachedQueryType.Name, propertiesUsedAsKeys,
                propertyValue);
        }

        public static string GenerateKey(string cacheKey, string[] propertiesUsedAsKeys, string[] propertyValues)
        {
            if (propertiesUsedAsKeys.Length != propertyValues.Length)
            {
                throw new ArgumentException("Arrays propertiesUsedAsKeys and propertyValues should be the same length");
            }

            var key = new StringBuilder();

            key.Append(cacheKey);

            var propertiesToUseAsCacheKeys = Array.ConvertAll(propertiesUsedAsKeys, p => p.Trim()).ToList();
            var valuesForPropertyKeys = Array.ConvertAll(propertyValues, p => p.Trim()).ToList();

            for (var index = 0; index < propertiesToUseAsCacheKeys.Count; index++)
            {
                var property = propertiesToUseAsCacheKeys[index];
                var propertyValue = valuesForPropertyKeys[index];
                key.Append('|').Append(property).Append('|').Append(propertyValue);
            }

            return key.ToString();
        }
    }
}