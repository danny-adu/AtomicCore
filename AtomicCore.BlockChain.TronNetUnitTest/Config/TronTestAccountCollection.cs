namespace AtomicCore.BlockChain.TronNet.Tests
{
    /// <summary>
    /// 测试账户集合
    /// </summary>
    public static class TronTestAccountCollection
    {
        /// <summary>
        /// 主网USDT合约地址
        /// </summary>
        public const string MainNetUsdtAddress = "TR7NHqjeKQxGTCi8q8ZY4pL8otSzgjLj6t";

        /// <summary>
        /// 测试网的USDT合约地址
        /// </summary>
        public const string TestNetUsdtAddress = "TB7whW3J9jb5Amoi4R6WgTtMbWPeqMBjSw";

        /// <summary>
        /// 测试账户 Main
        /// </summary>
        public static readonly TronTestAccount TestMain = new()
        {
            Address = "TEfiVcH2MF43NDXLpxmy6wRpaMxnZuc4iX",
            PirvateKey = "1bf5134ffaedae943b8d2b2d5a19fd067210dd7ebf9ead392681a651b53eef75"
        };

        /// <summary>
        /// 测试账户A
        /// </summary>
        public static readonly TronTestAccount TestA = new()
        {
            Address = "TYBp938TjQyndAcmmHVoq6eJBXoqi1yDuZ",
            PirvateKey = "d4e8e4d2a7603fe917f40fb87eabb9fadbccff61462e71056926592b66ccaf2f"
        };

        /// <summary>
        /// 测试账户B
        /// </summary>
        public static readonly TronTestAccount TestB = new()
        {
            Address = "TUoxWVFJDm6UrVJJw2UVfU94mP4PGvfq1A",
            PirvateKey = "488b83bf2bbc379db8f2de405f70d7349ede018beb707104f168e17ee6b00cb9"
        };

        /// <summary>
        /// 测试账户C
        /// </summary>
        public static readonly TronTestAccount TestC = new()
        {
            Address = "TMMXv4i8RjXaZREBphEek5yH1FPdkFA5qR",
            PirvateKey = "e3b928691fac7c6a58d29ca250f6e44a491ecd2fd94f3cb189a55e1680ee813a"
        };
    }
}
