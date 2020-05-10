using System;

namespace CleanArchitecture.Application.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class CacheInvalidateAttribute : Attribute
    {
        private string _matchToProperties;

        public Type CachedQuery { get; set; }
        public string PropertiesUsedForCacheKey { get; set; }

        /// <summary>
        /// If property names on invalidation object have different names to cached request, map them here
        /// </summary>
        public string MatchToProperties
        {
            get => string.IsNullOrEmpty(_matchToProperties) ? PropertiesUsedForCacheKey : _matchToProperties;
            set => _matchToProperties = value;
        }

        /// <summary>
        /// Prefix for cache key. If not set, request type will be used
        /// </summary>
        public string CacheKeyPrefix { get; set; }
    }
}