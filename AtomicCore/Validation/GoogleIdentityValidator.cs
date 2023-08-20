using System;
using System.Text;
using System.Text.RegularExpressions;

namespace AtomicCore.Validation
{
    /// <summary>
    /// 谷歌身份验证类
    /// </summary>
    public static class GoogleIdentityValidator
    {
        /// <summary>
        /// 生成谷歌身份证号
        /// </summary>
        /// <param name="keyNumber">服务端约定的10位数字</param>
        /// <returns></returns>
        public static string GenerateIdentity(string keyNumber)
        {
            if (!Regex.IsMatch(keyNumber, @"^\d{10}$", RegexOptions.IgnoreCase))
                throw new ArgumentException("keyNumber must be 10 length");

            GoogleBase32Encoder enc = new GoogleBase32Encoder();
            return enc.Encode(Encoding.ASCII.GetBytes(keyNumber));
        }

        /// <summary>
        /// 生成谷歌身份证号
        /// </summary>
        /// <param name="randomNumber">随机数字长度必须小于等于10位数</param>
        /// <returns></returns>
        public static string GenerateIdentity(int randomNumber)
        {
            string origNum = randomNumber.ToString();
            if (origNum.Length <= 0)
            {
                throw new ArgumentException("randomNumber must be 10 length");
            }

            if (origNum.Length > 10)
            {
                origNum = origNum.Substring(0, 10);
            }
            else if (origNum.Length < 10)
            {
                origNum = origNum.PadLeft(10, '0');
            }
            else
            {
                //NOTING
            }

            GoogleBase32Encoder enc = new GoogleBase32Encoder();
            return enc.Encode(Encoding.ASCII.GetBytes(origNum));
        }

        /// <summary>
        /// 根据身份证还原随机数
        /// </summary>
        /// <param name="secret">谷歌身份证字符串</param>
        /// <returns></returns>
        public static string ReductionIdentity(string secret)
        {
            if (string.IsNullOrEmpty(secret))
                throw new ArgumentNullException("secret");

            GoogleBase32Encoder enc = new GoogleBase32Encoder();
            return Encoding.ASCII.GetString(enc.Decode(secret));
        }

        /// <summary>
        /// 是否通过谷歌身份验证
        /// </summary>
        /// <param name="randomNumber">服务端约定的10位数字</param>
        /// <param name="password">用户身份器实时生成的动态密码</param>
        /// <returns></returns>
        public static bool Validate(int randomNumber, string password)
        {
            string origNum = randomNumber.ToString();
            if (origNum.Length <= 0)
            {
                throw new ArgumentException("randomNumber must be 10 length");
            }

            if (origNum.Length > 10)
            {
                origNum = origNum.Substring(0, 10);
            }
            else if (origNum.Length < 10)
            {
                origNum = origNum.PadLeft(10, '0');
            }
            else
            {
                //NOTING
            }

            return GoogleTimeBasedOneTimePassword.IsValid(origNum, password);
        }

        /// <summary>
        /// 是否通过谷歌身份验证
        /// </summary>
        /// <param name="keyNumber">服务端约定的10位数字</param>
        /// <param name="password">用户身份器实时生成的动态密码</param>
        /// <returns></returns>
        public static bool Validate(string keyNumber, string password)
        {
            if (!Regex.IsMatch(keyNumber, @"^\d{10}$", RegexOptions.IgnoreCase))
                throw new ArgumentException("keyNumber must be 10 length");

            return GoogleTimeBasedOneTimePassword.IsValid(keyNumber, password);
        }
    }
}
