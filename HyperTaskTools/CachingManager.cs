using Microsoft.Extensions.Caching.Memory;
using System;

namespace HyperTaskTools
{
    public class CachingManager
    {
        private MemoryCache _memoryCache;
        private long _durationMinutes;

        /*public CachingManager(long cacheSizeLimit, long durationMinutes)
        {
            this._memoryCache = new MemoryCache(new MemoryCacheOptions()
            {
                SizeLimit = cacheSizeLimit
                // TODO: Actually calculate how much memory we have vs how much we need 
                // but for development purpose we don't need a limit
            });
            this._durationMinutes = durationMinutes;
        }*/

        public CachingManager()
        {
            this._memoryCache = new MemoryCache(new MemoryCacheOptions()
            {
                SizeLimit = 100000
                // TODO: Actually calculate how much memory we have vs how much we need 
                // but for development purpose we don't need a limit
            });
            this._durationMinutes = 1 * 60 * 24;
        }

        /// <summary>
        /// Writes Key Value Pair to Cache
        /// Updates the existing record if already exists
        /// </summary>
        /// <param name="Key">Key to associate Value with in Cache</param>
        /// <param name="Value">Value to be stored in Cache associated with Key</param>
        public void Set(string Key, object Value, long? durationMinutes = null, int? size = 1)
        {
            if (Value != null)
                _memoryCache.Set(Key, Value, new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(durationMinutes ?? this._durationMinutes),
                    Size = size
                });
        }

        /// <summary>
        /// Writes Key Value Pair to Cache
        /// Updates the existing record if already exists
        /// </summary>
        /// <param name="Key">Key to associate Value with in Cache</param>
        /// <param name="Value">Value to be stored in Cache associated with Key</param>
        public void TrySet(string Key, object Value, long? durationMinutes = null, int? size = 1)
        {
            try
            {
                Set(Key, Value, durationMinutes, size);
            }
            catch (Exception ex)
            {
                Logger.Warn("Unable to set caching", ex);
            }
        }

        /// <summary>
        /// Returns Value stored in Cache
        /// </summary>
        /// <param name="Key"></param>
        /// <returns>Value stored in cache</returns>
        public object Get(string Key)
        {
            return _memoryCache.Get(Key);
        }

        /// <summary>
        /// Returns Value stored in Cache, null if non existent
        /// </summary>
        /// <param name="Key"></param>
        /// <returns>Value stored in cache</returns>
        public object TryGet(string Key)
        {
            try
            {
                return _memoryCache.Get(Key);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Remove a caching group
        /// </summary>
        /// <param name="startKey"></param>
        /*public void RemoveWildcardKey(string startKey)
        {
            List<string> cacheKeys = _memoryCache.Select(kvp => kvp.Key).ToList();
            foreach (string cacheKey in cacheKeys)
            {
                if (cacheKey.StartsWith(startKey))
                    _memoryCache.Remove(cacheKey);
            }
        }*/

        /// <summary>
        /// Remove a specific caching key
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }
    }
}
