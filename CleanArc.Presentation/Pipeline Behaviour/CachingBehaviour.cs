using CleanArc.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace CleanArc.Application.Pipeline_Behaviour
{
    public class CachingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IDistributedCache _cache;
        public CachingBehaviour(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (request is not ICacheableQuery cacheableQuery)
            {
                return await next();
            }

            var cachedResponse = await _cache.GetStringAsync(cacheableQuery.CacheKey);
            if (!string.IsNullOrEmpty(cachedResponse))
            {
                var response = JsonSerializer.Deserialize<TResponse>(cachedResponse);
                return response!;
            }
            var result = await next();
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheableQuery.CacheDuration ?? TimeSpan.FromMinutes(5)
            };
            var serializedResponse = JsonSerializer.Serialize(result);
            await _cache.SetStringAsync(cacheableQuery.CacheKey, serializedResponse, options,cancellationToken);
            return result;
        }
    }
}
