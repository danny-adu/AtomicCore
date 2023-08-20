using System;

namespace AtomicCore
{
    /// <summary>
    /// 根据时间生成动态密码
    /// </summary>
    internal static class GoogleTimeBasedOneTimePassword
    {
        /// <summary>
        /// 1970-1-1 utc
        /// </summary>
        public static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        ///// <summary>
        ///// 缓存集合
        ///// </summary>
        //private static Dictionary<string, DateTime> _cache;

        ///// <summary>
        ///// 静态构造
        ///// </summary>
        //static BizTimeBasedOneTimePassword()
        //{
        //    _cache = new Dictionary<string, DateTime>();
        //}

        /// <summary>
        /// 获取密码字符串
        /// </summary>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static string GetPassword(string secret)
        {
            return GetPassword(secret, GetCurrentCounter());
        }

        /// <summary>
        /// 获取密码字符串
        /// </summary>
        /// <param name="secret"></param>
        /// <param name="epoch"></param>
        /// <param name="timeStep"></param>
        /// <returns></returns>
        public static string GetPassword(string secret, DateTime epoch, int timeStep)
        {
            long counter = GetCurrentCounter(DateTime.UtcNow, epoch, timeStep);

            return GetPassword(secret, counter);
        }

        /// <summary>
        /// 获取密码字符串
        /// </summary>
        /// <param name="secret"></param>
        /// <param name="now"></param>
        /// <param name="epoch"></param>
        /// <param name="timeStep"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static string GetPassword(string secret, DateTime now, DateTime epoch, int timeStep, int digits)
        {
            long counter = GetCurrentCounter(now, epoch, timeStep);

            return GetPassword(secret, counter, digits);
        }

        /// <summary>
        /// 获取密码字符串
        /// </summary>
        /// <param name="secret"></param>
        /// <param name="counter"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        private static string GetPassword(string secret, long counter, int digits = 6)
        {
            return GoogleHashedOneTimePassword.GeneratePassword(secret, counter, digits);
        }

        /// <summary>
        /// 计算返回当前与标准UTC的1970年的返回的以30为模的计数
        /// </summary>
        /// <returns></returns>
        private static long GetCurrentCounter()
        {
            return GetCurrentCounter(DateTime.UtcNow, UNIX_EPOCH, 30);
        }

        /// <summary>
        /// 计算返回当前与标准UTC的1970年的返回的的计数
        /// </summary>
        /// <param name="now"></param>
        /// <param name="epoch"></param>
        /// <param name="timeStep"></param>
        /// <returns></returns>
        private static long GetCurrentCounter(DateTime now, DateTime epoch, int timeStep)
        {
            return (long)(now - epoch).TotalSeconds / timeStep;
        }

        ///// <summary>
        ///// 清空缓存
        ///// </summary>
        //private static void CleanCache()
        //{
        //    List<string> keysToRemove = new List<string>(_cache.Count);

        //    foreach (KeyValuePair<string, DateTime> pair in _cache)
        //    {
        //        if ((DateTime.Now - pair.Value).TotalMinutes > 2)
        //        {
        //            keysToRemove.Add(pair.Key);
        //        }
        //    }

        //    foreach (string key in keysToRemove)
        //    {
        //        _cache.Remove(key);
        //    }
        //}

        /// <summary>
        /// 是否验证通过
        /// </summary>
        /// <param name="secret"></param>
        /// <param name="password"></param>
        /// <param name="checkAdjacentIntervals"></param>
        /// <returns></returns>
        public static bool IsValid(string secret, string password, int checkAdjacentIntervals = 1)
        {
            // Keeping a cache of the secret/password combinations that have been requested allows us to
            // make this a real one time use system. Once a secret/password combination has been tested,
            // it cannot be tested again until after it is no longer valid.
            // See http://tools.ietf.org/html/rfc6238#section-5.2 for more info.

            //CleanCache();

            //string cache_key = string.Format("{0}_{1}", secret, password);

            //if (_cache.ContainsKey(cache_key))
            //{
            //    throw new BizOneTimePasswordException("You cannot use the same secret/iterationNumber combination more than once.");
            //}

            //_cache.Add(cache_key, DateTime.Now);

            if (password == GetPassword(secret))
                return true;

            for (int i = 1; i <= checkAdjacentIntervals; i++)
            {
                if (password == GetPassword(secret, GetCurrentCounter() + i))
                    return true;

                if (password == GetPassword(secret, GetCurrentCounter() - i))
                    return true;
            }

            return false;
        }
    }
}
