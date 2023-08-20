using System.Linq;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// StringBytes32 Decoder
    /// </summary>
    public class StringBytes32Decoder : ICustomRawDecoder<string>
    {
        /// <summary>
        /// Decode
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public string Decode(byte[] output)
        {
            if (output.Length > 32)
                //assuming that first 32 is the data index as this is the raw data
                return new StringTypeDecoder().Decode(output.Skip(32).ToArray());
            else
                return new Bytes32TypeDecoder().Decode<string>(output);
        }
    }
}
