using Google.Protobuf;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Transaction Extension
    /// </summary>
    public static class TronNetTransactionExtension
    {
        /// <summary>
        /// Calc TXID
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public static string GetTxid(this Transaction transaction)
        {
            var txid = transaction.RawData.ToByteArray().ToSHA256Hash().ToHex();

            return txid;
        }

    }
}
