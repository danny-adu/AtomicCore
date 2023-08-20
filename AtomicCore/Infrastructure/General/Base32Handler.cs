using System;
using System.Collections.Generic;
using System.Linq;

namespace AtomicCore
{
    /// <summary>
    /// 十进制 与 Base32 互转
    /// </summary>
    public static class Base32Handler
    {
        /// <summary>
        /// 10进制转32进制运算
        /// </summary>
        /// <param name="dec_num"></param>
        /// <returns></returns>
        public static string IntToB32(long dec_num)
        {
            List<int> indexs = new List<int>();
            while (dec_num >= 1)
            {
                int index = Convert.ToInt16(dec_num - (dec_num / 32) * 32);
                indexs.Add(index);
                dec_num /= 32;
            }

            if (indexs.Count <= 0)
                return string.Empty;

            indexs.Reverse();
            return string.Join(string.Empty, indexs.Select(s => Base32Map[s]));
        }

        /// <summary>
        /// 32进制转10进制
        /// </summary>
        /// <param name="b32_str"></param>
        /// <returns></returns>
        public static long B32ToInt(string b32_str)
        {
            long a = 0;
            int power = b32_str.Length - 1;

            for (int i = 0; i <= power; i++)
                a += Base32MapReversal[b32_str[power - i].ToString()] * Convert.ToInt32(Math.Pow(32, i));

            return a;
        }

        /// <summary>
        /// 32进制码表
        /// </summary>
        public static Dictionary<int, string> Base32Map = new Dictionary<int, string>()
        {
            { 0  ,"0" },
            { 1  ,"1" },
            { 2  ,"2" },
            { 3  ,"3" },
            { 4  ,"4" },
            { 5  ,"5" },
            { 6  ,"6" },
            { 7  ,"7" },
            { 8  ,"8" },
            { 9  ,"9" },
            { 10  ,"A" },
            { 11  ,"B" },
            { 12  ,"C" },
            { 13  ,"D" },
            { 14  ,"E" },
            { 15  ,"F" },
            { 16  ,"G" },
            { 17  ,"H" },
            { 18  ,"J" },
            { 19  ,"K" },
            { 20  ,"M" },
            { 21  ,"N" },
            { 22  ,"P" },
            { 23  ,"Q" },
            { 24  ,"R" },
            { 25  ,"S" },
            { 26  ,"T" },
            { 27  ,"U" },
            { 28  ,"V" },
            { 29  ,"W" },
            { 30  ,"X" },
            { 31  ,"Y" },
            { 32  ,"Z" }
        };

        /// <summary>
        /// 32进制逆转表
        /// </summary>
        public static Dictionary<string, int> Base32MapReversal
        {
            get
            {
                return Enumerable.Range(0, Base32Map.Count()).ToDictionary(i => Base32Map[i], i => i);
            }
        }
    }
}
