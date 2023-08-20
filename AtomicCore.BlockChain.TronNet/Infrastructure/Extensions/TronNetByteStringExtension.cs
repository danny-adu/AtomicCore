using Google.Protobuf;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet ByteString Extension
    /// </summary>
    public static class TronNetByteStringExtension
    {
        /// <summary>
        /// ByteString -> TronAddress
        /// </summary>
        /// <param name="byteString"></param>
        /// <param name="network"></param>
        /// <returns></returns>
        public static string GetTronAddress(this ByteString byteString, TronNetwork network)
        {
            if(byteString.IsEmpty)
                return string.Empty;

            return Base58Encoder.EncodeFromHex(byteString.ToByteArray(), (byte)network);
        }
    }
}
