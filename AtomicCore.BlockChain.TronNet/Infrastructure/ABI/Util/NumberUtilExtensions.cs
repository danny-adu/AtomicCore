namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// NumberExtensions
    /// </summary>
    public static class NumberExtensions
    {
        /// <summary>
        /// IsNumber
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumber(this object value)
        {
            return value is sbyte
                   || value is byte
                   || value is short
                   || value is ushort
                   || value is int
                   || value is uint
                   || value is long
                   || value is ulong
                   || value is float
                   || value is double
                   || value is decimal;
        }
    }
}