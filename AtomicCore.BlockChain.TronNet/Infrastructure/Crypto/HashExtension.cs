using Org.BouncyCastle.Crypto.Digests;
using System.Text;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Hash Extension
    /// </summary>
    public static class HashExtension
    {
        /// <summary>
        /// To SHA256 Hash
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ToSHA256Hash(this byte[] value)
        {
            Sha256Digest digest = new Sha256Digest();
            digest.BlockUpdate(value, 0, value.Length);

            byte[] output = new byte[digest.GetDigestSize()];
            digest.DoFinal(output, 0);

            return output;
        }

        /// <summary>
        /// To Keccak Hash
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ToKeccakHash(this byte[] value)
        {
            KeccakDigest digest = new KeccakDigest(256);
            byte[] output = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(value, 0, value.Length);
            digest.DoFinal(output, 0);

            return output;
        }

        /// <summary>
        /// To Keccak sHash
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToKeccakHash(this string value)
        {
            byte[] input = Encoding.UTF8.GetBytes(value);
            byte[] output = input.ToKeccakHash();

            return output.ToHex();
        }
    }
}
