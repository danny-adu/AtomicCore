using System.Text.RegularExpressions;

namespace AtomicCore.Validation
{
    /// <summary>
    /// 手机验证类
    /// </summary>
    public static class CellPhoneValidator
    {
        #region Variable

        /// <summary>
        /// 移动
        /// </summary>
        public const int Segment_10086 = 10086;
        /// <summary>
        /// 联通
        /// </summary>
        public const int Segment_10010 = 10010;
        /// <summary>
        /// 电信
        /// </summary>
        public const int Segment_10000 = 10000;

        /// <summary>
        /// 中国移动前三位
        /// </summary>
        private const string CON_GMS = "(?:134)|(?:135)|(?:136)|(?:137)|(?:138)|(?:139)|(?:150)|(?:151)|(?:152)|(?:157)|(?:158)|(?:159)";
        /// <summary>
        /// 3G(TD-SCDMA)-移动 
        /// </summary>
        private const string CON_3GTDSCDMA = "(?:147)|(?:178)|(?:182)|(?:183)|(?:184)|(?:187)|(?:188)|(?:198)";
        /// <summary>
        /// 中国联通
        /// </summary>
        private const string CON_UNICOM = "(?:130)|(?:131)|(?:132)|(?:145)|(?:155)|(?:156)|(?:166)|(?:175)|(?:176)";
        /// <summary>
        /// 3G(WCDMA)-联通
        /// </summary>
        private const string CON_3GWCDMA = "(?:185)|(?:186)";
        /// <summary>
        /// 3G(CDMA2000)--电信
        /// </summary>
        private const string CON_3GCDMA2000 = "(?:173)|(?:177)|(?:180)|(?:181)|(?:189)|(?:199)";
        /// <summary>
        /// 中国电信
        /// </summary>
        private const string CON_DIANXIN = "(?:133)|(?:153)";
        /// <summary>
        /// 虚拟运营商号段
        /// </summary>
        private const string CON_VIRTUAL = "(?:170)|(?:171)";

        #endregion

        #region Static Methods

        /// <summary>
        /// 是否是手机号码（全网段匹配）
        /// </summary>
        /// <param name="number">电话号码</param>
        /// <param name="isForceValid">是否强制认证（根据号段进行强验证）</param>
        /// <returns></returns>
        public static bool IsCellphone(string number, bool isForceValid = true)
        {
            if (string.IsNullOrEmpty(number) || 11 != number.Length)
                return false;

            if (isForceValid)
                return Regex.IsMatch(number, string.Format(@"^(?:{0}|{1}|{2}|{3}|{4}|{5})(?:\d){{8}}$", CON_GMS, CON_3GTDSCDMA, CON_UNICOM, CON_3GWCDMA, CON_3GCDMA2000, CON_DIANXIN), RegexOptions.IgnoreCase);
            else
                return Regex.IsMatch(number, @"^1[0-9]{10}$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 返回指定手机号码的网段标识
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static int GetSegment(string number)
        {
            if (string.IsNullOrEmpty(number) | number.Length != 11)
            {
                return 0;
            }

            //匹配移动
            if (Regex.IsMatch(number, string.Format(@"^(?:{0}|{1})(?:\d){{8}}$", CON_GMS, CON_3GTDSCDMA), RegexOptions.IgnoreCase))
            {
                return Segment_10086;
            }
            else if (Regex.IsMatch(number, string.Format(@"^(?:{0}|{1})(?:\d){{8}}$", CON_UNICOM, CON_3GWCDMA), RegexOptions.IgnoreCase))
            {
                return Segment_10010;
            }
            else if (Regex.IsMatch(number, string.Format(@"^(?:{0}|{1})(?:\d){{8}}$", CON_3GCDMA2000, CON_DIANXIN), RegexOptions.IgnoreCase))
            {
                return Segment_10000;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取验证的正则表达式
        /// </summary>
        /// <returns></returns>
        public static string GetValidateRegex()
        {
            return string.Format(@"^(?:{0}|{1}|{2}|{3}|{4}|{5})(?:\d){{8}}$", CON_GMS, CON_3GTDSCDMA, CON_UNICOM, CON_3GWCDMA, CON_3GCDMA2000, CON_DIANXIN);
        }

        #endregion
    }
}
