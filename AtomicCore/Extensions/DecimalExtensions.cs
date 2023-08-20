using System.Text;

namespace AtomicCore
{
    /// <summary>
    /// Decimal类型拓展类
    /// </summary>
    public static partial class DecimalExtensions
    {
        /// <summary>
        /// Decimal的有效值
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="digit">小数点后几位</param>
        /// <remarks>#0.######</remarks>
        /// <returns></returns>
        public static string ToString(this decimal value, int digit)
        {
            StringBuilder format = new StringBuilder("#0");
            if (digit > 0)
            {
                format.Append('.');
                for (int i = 0; i < digit; i++)
                    format.Append('#');
            }

            return value.ToString(format.ToString());
        }

        ///// <summary>
        ///// 固定保留小数位数（非四舍五入或任何进位机制）
        ///// </summary>
        ///// <param name="_this"></param>
        ///// <param name="decimals"></param>
        ///// <returns></returns>
        //public static decimal Retention(this decimal _this, int decimals)
        //{
        //    return decimal.Parse(_this.ToString(string.Format("F{0}", decimals)));
        //}
    }
}
