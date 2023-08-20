using System.Linq;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Encoder Decoder Helpers
    /// </summary>
    public class EncoderDecoderHelpers
    {
        /// <summary>
        /// GetNumberOfBytes
        /// </summary>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public static int GetNumberOfBytes(byte[] encoded)
        {
            var intDecoder = new IntTypeDecoder();
            var numberOfBytesEncoded = encoded.Take(32);
            return intDecoder.DecodeInt(numberOfBytesEncoded.ToArray());
        }
    }
}