using System;

namespace CleanArchitecture.Application.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CacheAttribute : Attribute
    {
        public string PropertiesUsedForCacheKey;

        /// <summary>
        /// Duration in seconds. Defaults to 600 (10 minutes)
        /// </summary>
        public int Duration { get; set; } = 600;

        /// <summary>
        /// Prefix for cache key. If not set, request type will be used
        /// </summary>
        public string CacheKeyPrefix { get; set; }

        public CacheAttribute(string propertiesUsedForCacheKey)
        {
            if (string.IsNullOrEmpty(propertiesUsedForCacheKey))
            {
                throw new ArgumentNullException("PropertiesUsedForCacheKey must have a value");
            }

            PropertiesUsedForCacheKey = propertiesUsedForCacheKey;
        }
    }
}