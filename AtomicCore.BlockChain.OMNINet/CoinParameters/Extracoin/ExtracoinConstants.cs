namespace AtomicCore.BlockChain.OMNINet
{
    /// <summary>
    /// ExtracoinConstants
    /// </summary>
    public class ExtracoinConstants
    {
        /// <summary>
        /// Constants
        /// </summary>
        public sealed class Constants : CoinConstants<Constants>
        {
            /// <summary>
            /// 1 btc to satoshis
            /// </summary>
            public readonly int OneBitcoinInSatoshis = 100000000;

            /// <summary>
            /// 1 satoshi to btc
            /// </summary>
            public readonly decimal OneSatoshiInBTC = 0.00000001M;

            /// <summary>
            /// satoshis per bitcoin
            /// </summary>
            public readonly int SatoshisPerBitcoin = 100000000;

            /// <summary>
            /// symbol
            /// </summary>
            public readonly string Symbol = "฿";
        }
    }
}
