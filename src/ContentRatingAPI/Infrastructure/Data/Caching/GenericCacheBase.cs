// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRatingAPI.Infrastructure.Data.Caching
{
    public abstract class GenericCacheBase<T>
    {
        protected static readonly string Key = typeof(T).FullName ?? nameof(T);

        public abstract T Set<K>(K id, T value);
        public abstract bool TryGetValue<K>(K id, out T? value);
        public abstract void Remove<K>(K id);

        protected virtual string GetKey<K>(K id)
        {
            return Key + id!.ToString();
        }
    }
}
