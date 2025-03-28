// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.Caching.Memory;

namespace ContentRatingAPI.Infrastructure.Data.Caching
{
    public class GenericInMemoryCache<T> : GenericCacheBase<T>
    {
        private readonly IMemoryCache memoryCache;
        private readonly MemoryCacheEntryOptions cacheEntryOptions;

        public GenericInMemoryCache(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
            cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
                .SetSize(2000)
                .SetSlidingExpiration(TimeSpan.FromSeconds(60));
        }

        public GenericInMemoryCache(IMemoryCache memoryCache, MemoryCacheEntryOptions cacheEntryOptions)
        {
            this.memoryCache = memoryCache;
            this.cacheEntryOptions = cacheEntryOptions;
        }

        public override void Remove<K>(K id)
        {
            memoryCache.Remove(GetKey(id));
        }

        public override T Set<K>(K id, T value)
        {
            return memoryCache.Set(GetKey(id), value, cacheEntryOptions);
        }

        public override bool TryGetValue<K>(K id, out T? value)
        {
            var result = memoryCache.TryGetValue(GetKey(id), out value);
            return result;
        }
    }
}
