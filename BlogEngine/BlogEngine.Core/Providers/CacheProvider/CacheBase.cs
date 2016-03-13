using System;
using System.Collections;
using System.Web.Caching;

namespace BlogEngine.Core.Providers.CacheProvider
{
    /// <summary>
    /// Cache Base
    /// </summary>
    public abstract class CacheBase : IEnumerable
    {
        /// <summary>
        /// Count
        /// </summary>
        public abstract int Count { get; }
        /// <summary>
        /// Physical Memory Limit
        /// </summary>
        public abstract long EffectivePercentagePhysicalMemoryLimit { get; }
        ///<summary>
        ///</summary>
        public abstract long EffectivePrivateBytesLimit { get; }
        ///<summary>
        ///</summary>
        ///<param name="key"></param>
        public abstract object this[string key] { get; set; }
        ///<summary>
        ///</summary>
        ///<param name="key"></param>
        ///<param name="value"></param>
        ///<param name="dependencies"></param>
        ///<param name="absoluteExpiration"></param>
        ///<param name="slidingExpiration"></param>
        ///<param name="priority"></param>
        ///<param name="onRemoveCallback"></param>
        ///<returns></returns>
        public abstract object Add(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback);
        ///<summary>
        ///</summary>
        ///<param name="key"></param>
        ///<returns></returns>
        public abstract object Get(string key);
        ///<summary>
        ///</summary>
        ///<returns></returns>
        public abstract IDictionaryEnumerator GetEnumerator();
        ///<summary>
        ///</summary>
        ///<param name="key"></param>
        ///<param name="value"></param>
        public abstract void Insert(string key, object value);
        ///<summary>
        ///</summary>
        ///<param name="key"></param>
        ///<param name="value"></param>
        ///<param name="dependencies"></param>
        public abstract void Insert(string key, object value, CacheDependency dependencies);
        ///<summary>
        ///</summary>
        ///<param name="key"></param>
        ///<param name="value"></param>
        ///<param name="dependencies"></param>
        ///<param name="absoluteExpiration"></param>
        ///<param name="slidingExpiration"></param>
        public abstract void Insert(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration);
        ///<summary>
        ///</summary>
        ///<param name="key"></param>
        ///<param name="value"></param>
        ///<param name="dependencies"></param>
        ///<param name="absoluteExpiration"></param>
        ///<param name="slidingExpiration"></param>
        ///<param name="priority"></param>
        ///<param name="onRemoveCallback"></param>
        public abstract void Insert(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback);
        ///<summary>
        ///</summary>
        ///<param name="key"></param>
        ///<returns></returns>
        public abstract object Remove(string key);

        /// <summary>
        /// Reset cache provider
        /// </summary>
        public abstract void Reset();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
