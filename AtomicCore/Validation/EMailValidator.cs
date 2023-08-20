using System.Text.RegularExpressions;

namespace AtomicCore.Validation
{
    /// <summary>
    /// 邮箱验证
    /// </summary>
    public static class EMailValidator
    {
        #region Variable

        /// <summary>
        /// 不带中文的正则验证
        /// </summary>
        private const string c_regexWithoutCN = @"^\w+(?:[-+.]\w+)*@\w+(?:[-.]\w+)*\.\w+(?:[-.]\w+)*$";
        /// <summary>
        /// 带中文的正则验证
        /// </summary>
        private const string c_regexWithCN = @"^[\w\u4e00-\u9fa5]+(?:[-+.][\w\u4e00-\u9fa5]+)*@[\w\u4e00-\u9fa5]+(?:[-.][\w\u4e00-\u9fa5]+)*\.[\w\u4e00-\u9fa5]+(?:[-.][\w\u4e00-\u9fa5]+)*$";

        #endregion

        #region Static Methods

        /// <summary>
        /// 验证邮箱
        /// </summary>
        /// <param name="email"></param>
        /// <param name="isCompatibleWithCN"></param>
        /// <returns></returns>
        public static bool IsEMail(string email, bool isCompatibleWithCN = true)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            if (!isCompatibleWithCN)
            {
                return Regex.IsMatch(email, c_regexWithoutCN, RegexOptions.IgnoreCase);
            }
            else
            {
                return Regex.IsMatch(email, c_regexWithCN, RegexOptions.IgnoreCase);
            }
        }

        /// <summary>
        /// 获取验证的正则表达式
        /// </summary>
        /// <param name="includeCN">是否包含中文邮箱</param>
        /// <returns></returns>
        public static string GetValidateRegex(bool includeCN = false)
        {
            return includeCN ? c_regexWithCN : c_regexWithoutCN;
        }

        #endregion
    }
}
