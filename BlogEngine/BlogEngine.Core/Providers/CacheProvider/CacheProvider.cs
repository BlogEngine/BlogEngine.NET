using System;
using System.Collections;
using System.Web.Caching;

namespace BlogEngine.Core.Providers.CacheProvider
{
    /// <summary>
    /// Abstracts away access to the cache.
    /// </summary>
    public class CacheProvider : CacheBase
    {
        private readonly Cache _cache;
        private readonly string _keyPrefix = Blog.CurrentInstance.Id + "_";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        public CacheProvider(Cache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// Gets the number of items stored in the cache.
        /// </summary>
        public override int Count
        {
            get { return _cache.Count; }
        }

        /// <summary>
        /// Gets the percentage of physical memory that can be consumed by an application before ASP.NET starts removing items from the cache.
        /// </summary>
        public override long EffectivePercentagePhysicalMemoryLimit
        {
            get { return _cache.EffectivePercentagePhysicalMemoryLimit; }
        }

        /// <summary>
        /// Gets the number of bytes available for the cache.
        /// </summary>
        public override long EffectivePrivateBytesLimit
        {
            get { return _cache.EffectivePrivateBytesLimit; }
        }

        /// <summary>
        /// Gets or sets the cache item at the specified key.
        /// </summary>
        /// <param name="key">The cache key used to reference the item. It will be prefixed with the current blog instance's id.</param>
        /// <returns>The specified cache item.</returns>
        public override object this[string key]
        {
            get
            {
                return _cache[_keyPrefix + key];
            }
            set
            {
                _cache[_keyPrefix + key] = value;
            }
        }

        /// <summary>
        /// Adds the specified item to the cache.
        /// </summary>
        /// <param name="key">The cache key used to reference the item. It will be prefixed with the current blog instance's id.</param>
        /// <param name="value">The item to be added to the cache.</param>
        /// <param name="dependencies">The file or cache key dependencies for the item. When any dependency changes, the object becomes invalid and is removed from the cache. If there are no dependencies, this parameter contains null.</param>
        /// <param name="absoluteExpiration">The time at which the added object expires and is removed from the cache. If you are using sliding expiration, the absoluteExpiration parameter must be System.Web.Caching.Cache.NoAbsoluteExpiration.</param>
        /// <param name="slidingExpiration">The interval between the time the added object was last accessed and the time at which that object expires. If this value is the equivalent of 20 minutes, the object expires and is removed from the cache 20 minutes after it is last accessed. If you are using absolute expiration, the slidingExpiration parameter must be System.Web.Caching.Cache.NoSlidingExpiration.</param>
        /// <param name="priority">The relative cost of the object, as expressed by the System.Web.Caching.CacheItemPriority enumeration. The cache uses this value when it evicts objects; objects with a lower cost are removed from the cache before objects with a higher cost.</param>
        /// <param name="onRemoveCallback">A delegate that, if provided, is called when an object is removed from the cache. You can use this to notify applications when their objects are deleted from the cache.</param>
        /// <returns>An object that represents the item that was added if the item was previously stored in the cache; otherwise, null.</returns>
        public override object Add(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback)
        {
            return _cache.Add(_keyPrefix + key, value, dependencies, absoluteExpiration, slidingExpiration, priority, onRemoveCallback);
        }

        /// <summary>
        /// Retrieves the specified item from the cache.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override object Get(string key)
        {
            return _cache.Get(_keyPrefix + key);
        }

        /// <summary>
        /// Retrieves a dictionary enumerator used to iterate through the key settings and their values contained in the cache.
        /// </summary>
        /// <returns></returns>
        public override IDictionaryEnumerator GetEnumerator()
        {
            return _cache.GetEnumerator();
        }

        /// <summary>
        /// Inserts an item into the cache with a cache key to reference its location, using default values provided by the System.Web.Caching.CacheItemPriority enumeration.
        /// </summary>
        /// <param name="key">The cache key used to reference the item. It will be prefixed with the current blog instance's id.</param>
        /// <param name="value">The object to be inserted into the cache.</param>
        public override void Insert(string key, object value)
        {
            _cache.Insert(_keyPrefix + key, value);
        }

        /// <summary>
        /// Inserts an object into the cache that has file or key dependencies.
        /// </summary>
        /// <param name="key">The cache key used to reference the item. It will be prefixed with the current blog instance's id.</param>
        /// <param name="value">The object to be inserted into the cache.</param>
        /// <param name="dependencies">The file or cache key dependencies for the inserted object. When any dependency changes, the object becomes invalid and is removed from the cache. If there are no dependencies, this parameter contains null.</param>
        public override void Insert(string key, object value, CacheDependency dependencies)
        {
            _cache.Insert(_keyPrefix + key, value, dependencies);
        }

        /// <summary>
        /// Inserts an object into the cache with dependencies and expiration policies.
        /// </summary>
        /// <param name="key">The cache key used to reference the item. It will be prefixed with the current blog instance's id.</param>
        /// <param name="value">The object to be inserted into the cache.</param>
        /// <param name="dependencies">The file or cache key dependencies for the inserted object. When any dependency changes, the object becomes invalid and is removed from the cache. If there are no dependencies, this parameter contains null.</param>
        /// <param name="absoluteExpiration">The time at which the added object expires and is removed from the cache. If you are using sliding expiration, the absoluteExpiration parameter must be System.Web.Caching.Cache.NoAbsoluteExpiration.</param>
        /// <param name="slidingExpiration">The interval between the time the added object was last accessed and the time at which that object expires. If this value is the equivalent of 20 minutes, the object expires and is removed from the cache 20 minutes after it is last accessed. If you are using absolute expiration, the slidingExpiration parameter must be System.Web.Caching.Cache.NoSlidingExpiration.</param>
        public override void Insert(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            _cache.Insert(_keyPrefix + key, value, dependencies, absoluteExpiration, slidingExpiration);
        }

        /// <summary>
        /// Inserts an object into the cache object together with dependencies, expiration policies, and a delegate that you can use to notify the application before the item is removed from the cache.
        /// </summary>
        /// <param name="key">The cache key used to reference the item. It will be prefixed with the current blog instance's id.</param>
        /// <param name="value">The object to be inserted into the cache.</param>
        /// <param name="dependencies">The file or cache key dependencies for the inserted object. When any dependency changes, the object becomes invalid and is removed from the cache. If there are no dependencies, this parameter contains null.</param>
        /// <param name="absoluteExpiration">The time at which the added object expires and is removed from the cache. If you are using sliding expiration, the absoluteExpiration parameter must be System.Web.Caching.Cache.NoAbsoluteExpiration.</param>
        /// <param name="slidingExpiration">The interval between the time the added object was last accessed and the time at which that object expires. If this value is the equivalent of 20 minutes, the object expires and is removed from the cache 20 minutes after it is last accessed. If you are using absolute expiration, the slidingExpiration parameter must be System.Web.Caching.Cache.NoSlidingExpiration.</param>
        /// <param name="priority">The cost of the object relative to other items stored in the cache, as expressed by the System.Web.Caching.CacheItemPriority enumeration. This value is used by the cache when it evicts objects; objects with a lower cost are removed from the cache before objects with a higher cost.</param>
        /// <param name="onRemoveCallback">A delegate that, if provided, will be called when an object is removed from the cache. You can use this to notify applications when their objects are deleted from the cache.</param>
        public override void Insert(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback)
        {
            _cache.Insert(_keyPrefix + key, value, dependencies, absoluteExpiration, slidingExpiration, priority, onRemoveCallback);
        }

        /// <summary>
        /// Removes the specified item from the application's cache object.
        /// </summary>
        /// <param name="key">The cache key used to reference the item. It will be prefixed with the current blog instance's id.</param>
        /// <returns></returns>
        public override object Remove(string key)
        {
            return _cache.Remove(_keyPrefix + key);
        }

        /// <summary>
        /// Reset cache provider
        /// </summary>
        public override void Reset()
        {
            var keys = new System.Collections.Specialized.StringCollection();
            foreach (var item in _cache)
            {
                var x = (DictionaryEntry)item;
                keys.Add(x.Key.ToString());
            }
            if (keys.Count > 0)
            {
                foreach (var key in keys)
                {
                    if (key.StartsWith(_keyPrefix))
                    {
                        // Utils.Log(key);
                        _cache.Remove(key);
                    }
                }
            }
        }
    }
}
