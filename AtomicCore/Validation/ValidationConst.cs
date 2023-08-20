namespace AtomicCore.Validation
{
    /// <summary>
    /// 正则表达式验证常量
    /// </summary>
    public static class ValidationConst
    {
        /// <summary>
        /// 小数点后二位
        /// </summary>
        public const string DotNet_DecimalF2 = @"^(?:(?:[0])|(?:[1-9][0-9]*))(?:\.[0-9]{1,2})?$";

        /// <summary>
        /// 匹配所有的数字都必须是不重复的（即：所有的数字只能出现一次）
        /// </summary>
        public const string DotNet_NotRepeatNumber = @"^(?!.*?(\d).*?\1.*?$)\d+$";
    }
}
