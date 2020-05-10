using CleanArchitecture.Application.Common.Attributes;
using CleanArchitecture.Application.Common.Helpers;
using CleanArchitecture.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Common.Behaviours
{
    public class CachingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger _logger;
        private ICacheService CacheService { get; }

        public CachingBehaviour(ICacheService cacheService, ILogger<CachingBehaviour<TRequest, TResponse>> logger)
        {
            _logger = logger;
            CacheService = cacheService;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var cacheAttributes = request.GetType().GetCustomAttributes<CacheAttribute>().ToList();
            
            if (!cacheAttributes.Any())
            {
                return await next();
            }

            foreach (var cacheAttribute in cacheAttributes)
            {
                var propertyNames = Array.ConvertAll(cacheAttribute.PropertiesUsedForCacheKey?.Split(',') ?? throw new InvalidOperationException(), p => p.Trim());

                var propertyValues = request.GetType().GetProperties()
                    .Where(i => propertyNames.Any(c => c.Equals(i.Name, StringComparison.OrdinalIgnoreCase)) && i.GetValue(request) != null)
                    .Select(i => i.GetValue(request).ToString())
                    .ToArray();

                // Some values may be null, so ignore and continue
                if (propertyNames.Length != propertyValues.Length)
                {
                    _logger.LogInformation($"Request {typeof(TRequest).Name}: Did not cache response as propertyNames.Length != propertyValues.Length");
                    return await next();
                }

                var cacheKey = !string.IsNullOrEmpty(cacheAttribute.CacheKeyPrefix) ?
                    CacheHelper.GenerateKey(cacheAttribute.CacheKeyPrefix, propertyNames, propertyValues) :
                    CacheHelper.GenerateKey(typeof(TRequest), propertyNames, propertyValues);

                var cachedResponse = await CacheService.GetCacheValueAsync(cacheKey);

                if (cachedResponse != null)
                {
                    _logger.LogInformation($"Request {typeof(TRequest).Name} served from cache");
                    return (TResponse)cachedResponse;
                }

                var actualResponse = await next();
                await CacheService.SetCacheValueAsync(cacheKey, actualResponse, TimeSpan.FromSeconds(cacheAttribute.Duration));
                return actualResponse;
            }

            return await next();
        }
    }
}
