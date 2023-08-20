using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;

namespace AtomicCore.BlockChain.OmniscanAPI
{
    /// <summary>
    /// Memory Cache Provider
    /// </summary>
    internal static class OmniCacheProvider
    {
        #region Variable

        /// <summary>
        /// global cache key
        /// </summary>
        private static readonly object s_globalCache_key = new object();

        /// <summary>
        /// global cache instance
        /// </summary>
        private static readonly IMemoryCache s_globalCache_instance = new MemoryCache(Options.Create(new MemoryCacheOptions()));

        /// <summary>
        /// business cache instance
        /// </summary>
        private static readonly IMemoryCache s_businessCache_instance = new MemoryCache(Options.Create(new MemoryCacheOptions()));

        #endregion

        #region Public Methods

        /// <summary>
        /// generate cache key
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="paramData"></param>
        /// <returns></returns>
        public static string GenerateCacheKey(string methodName, params string[] paramData)
        {
            if (string.IsNullOrEmpty(methodName))
                throw new ArgumentNullException(nameof(methodName));

            string origText;
            if (null == paramData || paramData.Length <= 0)
                origText = $"OmniscanAPI-{methodName}";
            else
                origText = $"OmniscanAPI-{methodName}:{string.Join(",", paramData)}";

            return AtomicCore.MD5Handler.Generate(origText.ToLower(), false);
        }

        /// <summary>
        /// del cache data
        /// </summary>
        /// <param name="key"></param>
        public static void Delete(string key)
        {
            if (string.IsNullOrEmpty(key))
                return;

            s_businessCache_instance.Remove(key);
        }

        /// <summary>
        /// get cache data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool Get<T>(string key, out T value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            return s_businessCache_instance.TryGetValue(key, out value);
        }

        /// <summary>
        /// 创建缓存
        /// </summary>
        /// <param name="key">缓存KEY</param>
        /// <param name="value">缓存数据</param>
        /// <param name="mode">缓存模型</param>
        /// <param name="expriedTime">过期时间</param>
        public static void Set(string key, object value, OmniCacheMode mode = OmniCacheMode.None, TimeSpan? expriedTime = null)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            object ck_value = s_businessCache_instance.Get(key);

            if (null == ck_value)
            {
                object dynamicKey = GetDynamicKey(key);
                lock (dynamicKey)
                {
                    if (null == ck_value)
                    {
                        using (ICacheEntry cacheEntry = s_businessCache_instance.CreateEntry(key))
                        {
                            cacheEntry.Value = value;

                            if (mode == OmniCacheMode.AbsoluteExpired)
                            {
                                if (null == expriedTime)
                                    throw new ArgumentNullException(nameof(expriedTime));

                                cacheEntry.AbsoluteExpiration = new DateTimeOffset(DateTime.Now.Add(expriedTime.Value));
                            }
                            else if (mode == OmniCacheMode.SlideExpired)
                            {
                                if (null == expriedTime)
                                    throw new ArgumentNullException(nameof(expriedTime));

                                cacheEntry.SlidingExpiration = expriedTime;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// get dynamic key
        /// </summary>
        /// <param name="methodKey">use nameof(function name)</param>
        /// <returns></returns>
        private static object GetDynamicKey(object methodKey)
        {
            return GetDynamicKey(methodKey, TimeSpan.FromMinutes(3));
        }

        /// <summary>
        /// get dynamic key
        /// </summary>
        /// <param name="methodKey">use nameof(function name)</param>
        /// <param name="slidingExpiration">sliding expiration time</param>
        /// <returns></returns>
        private static object GetDynamicKey(object methodKey, TimeSpan slidingExpiration)
        {
            object dynamicKey = s_globalCache_instance.Get(methodKey);
            if (null == dynamicKey)
            {
                lock (s_globalCache_key)
                {
                    if (null == dynamicKey)
                    {
                        using (var cacheEntry = s_globalCache_instance.CreateEntry(methodKey))
                        {
                            cacheEntry.Value = new object();
                            cacheEntry.SlidingExpiration = slidingExpiration;
                        }

                        if (!s_globalCache_instance.TryGetValue(methodKey, out dynamicKey))
                            throw new Exception("DynamicKey Create Fail...");
                    }
                }
            }

            return dynamicKey;
        }

        #endregion
    }
}
