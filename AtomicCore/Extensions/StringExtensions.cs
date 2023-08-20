using System;
using System.Text;

namespace AtomicCore
{
    /// <summary>
    /// string类型原型拓展方法
    /// </summary>
    public static partial class StringExtensions
    {
        private static readonly char[] s_splitChars = new char[] { 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

        /// <summary>
        /// 【AtomicCore】获取指定字符串从头开始截图的前几个字符串
        /// </summary>
        /// <param name="instance">字符串实例</param>
        /// <param name="charNums">指定的截取数</param>
        /// <returns></returns>
        public static string Top(this string instance, int charNums)
        {
            if (string.IsNullOrEmpty(instance))
                throw new ArgumentNullException("instance");
            if (charNums <= 0)
                throw new ArgumentException("charNums must be greater than zero");

            if (instance.Length >= charNums)
            {
                return instance.Substring(0, charNums);
            }
            else
            {
                return instance;
            }
        }

        /// <summary>
        /// 位移字符串
        /// </summary>
        /// <param name="s">字符串实例</param>
        /// <param name="offSet">位置数</param>
        /// <returns></returns>
        public static string Shift(this string s, int offSet)
        {
            StringBuilder strB = new StringBuilder();

            string[] arr = s.Split(s_splitChars, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < arr.Length; i++)
            {
                int temp = Convert.ToInt32(arr[i], 16) - offSet;
                strB.Append(GetASCIIChar(temp));
            }
            return strB.ToString();
        }

        /// <summary>
        /// 获取ascii字符
        /// </summary>
        /// <param name="asciiCode"></param>
        /// <returns></returns>
        public static string GetASCIIChar(int asciiCode)
        {
            if (asciiCode >= 0 && asciiCode <= 255)
            {
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                byte[] byteArray = new byte[] { (byte)asciiCode };
                string strCharacter = asciiEncoding.GetString(byteArray);
                return (strCharacter);
            }
            else
                return string.Empty;
        }
    }
}
