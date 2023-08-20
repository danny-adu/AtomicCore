using System;
using System.Text.RegularExpressions;

namespace AtomicCore.Validation
{
    /// <summary>
    /// 身份证格式规则验证类
    /// </summary>
    public static class IDCardValidator
    {
        #region Variable

        /// <summary>
        /// 中国标准的行政区(2位)
        /// </summary>
        private const string regex_district = "^(?:11)|(?:12)|(?:13)|(?:14)|(?:15)|(?:21)|(?:22)|(?:23)|(?:31)|(?:32)|(?:33)|(?:34)|(?:35)|(?:36)|(?:37)|(?:41)|(?:42)|(?:43)|(?:44)|(?:45)|(?:46)|(?:50)|(?:51)|(?:52)|(?:53)|(?:54)|(?:61)|(?:62)|(?:63)|(?:64)|(?:65)|(?:71)||(?:81)||(?:82)$";

        #endregion

        #region Static Methods

        /// <summary>
        /// 身份证验证（15位~18位）
        /// </summary>
        /// <param name="idCard">身份证号码</param>
        /// <returns></returns>
        public static bool IsIDCard(string idCard)
        {
            if (idCard.Length == 18)
            {
                bool check = CheckIDCard18(idCard);
                return check;
            }
            else if (idCard.Length == 15)
            {
                bool check = CheckIDCard15(idCard);
                return check;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 18位身份证验证
        /// </summary>
        /// <param name="idCard">身份证号</param>
        /// <returns></returns>
        private static bool CheckIDCard18(string idCard)
        {
            long n = 0;
            if (long.TryParse(idCard.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(idCard.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证
            }
            string top2 = idCard.Remove(2);
            if (!Regex.IsMatch(top2, regex_district, RegexOptions.IgnoreCase))
            {
                return false;//省份验证
            }
            string birth = idCard.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = idCard.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != idCard.Substring(17, 1).ToLower())
            {
                return false;//校验码验证
            }
            return true;//符合GB11643-1999标准
        }

        /// <summary>
        /// 15位身份证验证
        /// </summary>
        /// <param name="idCard">身份证号</param>
        /// <returns></returns>
        private static bool CheckIDCard15(string idCard)
        {
            long n = 0;
            if (long.TryParse(idCard, out n) == false || n < Math.Pow(10, 14))
            {
                return false;//数字验证
            }
            //1-2位 省、自治区、直辖市代码；
            //3-4位地级市、盟、自治州代码；
            //5-6位县、县级市、区代码；
            string top2 = idCard.Remove(2);
            if (!Regex.IsMatch(top2, regex_district, RegexOptions.IgnoreCase))
            {
                return false;//省份验证
            }
            //7-12位出生年月日,比如670401代表1967年4月1日,与18位的第一个区别；
            //13-15位为顺序号，其中15位男为单数，女为双数； 
            string birth = idCard.Substring(6, 6).Insert(4, "-").Insert(2, "-").Insert(0, "19");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            return true;//符合15位身份证标准
        }

        #endregion
    }
}
