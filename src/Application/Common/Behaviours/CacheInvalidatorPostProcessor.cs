using CleanArchitecture.Application.Common.Attributes;
using CleanArchitecture.Application.Common.Helpers;
using CleanArchitecture.Application.Common.Interfaces;
using MediatR.Pipeline;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Common.Behaviours
{
    public class CacheInvalidatorPostProcessor<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
    {
        private readonly ICacheService _cacheService;

        public CacheInvalidatorPostProcessor(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
        {
            var cacheInvalidateAttribute = request.GetType().GetCustomAttribute<CacheInvalidateAttribute>();

            if (cacheInvalidateAttribute == null)
            {
                return Task.CompletedTask;
            }

            var propertyNames = Array.ConvertAll(cacheInvalidateAttribute.PropertiesUsedForCacheKey?.Split(',') ?? throw new InvalidOperationException(), p => p.Trim());
            var propertyMapToNames = Array.ConvertAll(cacheInvalidateAttribute.MatchToProperties?.Split(',') ?? throw new InvalidOperationException(), p => p.Trim());

            var propertyValues = request.GetType().GetProperties()
                .Where(i => propertyMapToNames.Any(c => c.Equals(i.Name, StringComparison.OrdinalIgnoreCase)))
                .Select(i => i.GetValue(request).ToString())
                .ToArray();

            var cacheKey = !string.IsNullOrEmpty(cacheInvalidateAttribute.CacheKeyPrefix)
                ? CacheHelper.GenerateKey(cacheInvalidateAttribute.CacheKeyPrefix, propertyNames, propertyValues)
                : CacheHelper.GenerateKey(cacheInvalidateAttribute.CachedQuery, propertyNames, propertyValues);

            _cacheService.RemoveCacheValue(cacheKey);

            return Task.CompletedTask;
        }
    }
}