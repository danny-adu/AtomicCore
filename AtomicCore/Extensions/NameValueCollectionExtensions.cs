using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace AtomicCore
{
    /// <summary>
    /// 键值对集合拓展
    /// </summary>
    public static partial class NameValueCollectionExtensions
    {
        /// <summary>
        /// NameValueCollection => IDictionary
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IDictionary<string, string> ToDictionary(this NameValueCollection source)
        {
            return source.AllKeys.ToDictionary(k => k, k => source[k]);
        }

        /// <summary>
        /// NameValueCollection => IDictionary
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IDictionary<string, string[]> ToDictionaryExt(this NameValueCollection source)
        {
            return source.AllKeys.ToDictionary(k => k, k => source.GetValues(k));
        }
    }
}
