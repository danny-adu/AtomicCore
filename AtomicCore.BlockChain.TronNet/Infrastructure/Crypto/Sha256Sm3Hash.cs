using Google.Protobuf;
using Org.BouncyCastle.Crypto.Digests;
using System;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Sha256 Sm3 Hash
    /// </summary>
    public class Sha256Sm3Hash
    {
        #region Variables

        public static int LENGTH = 32; // bytes
        public static Sha256Sm3Hash ZERO_HASH = Wrap(new byte[LENGTH]);

        private byte[] _bytes;

        #endregion

        #region Public Methods

        /// <summary>
        /// Sha256 Sm3 Hash
        /// </summary>
        /// <param name="rawHashBytes"></param>
        public Sha256Sm3Hash(byte[] rawHashBytes)
        {
            CheckArgument(rawHashBytes.Length == LENGTH);
            _bytes = rawHashBytes;
        }

        /// <summary>
        /// Get Bytes
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            return _bytes;
        }

        /// <summary>
        /// Wrap
        /// </summary>
        /// <param name="rawHashBytes"></param>
        /// <returns></returns>
        public static Sha256Sm3Hash Wrap(byte[] rawHashBytes)
        {
            return new Sha256Sm3Hash(rawHashBytes);
        }

        /// <summary>
        /// Wrap
        /// </summary>
        /// <param name="rawHashByteString"></param>
        /// <returns></returns>
        public static Sha256Sm3Hash Wrap(ByteString rawHashByteString)
        {
            return Wrap(rawHashByteString.ToByteArray());
        }

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="contents"></param>
        /// <returns></returns>
        public static Sha256Sm3Hash Create(byte[] contents)
        {
            return Of(contents);
        }

        /// <summary>
        /// Of
        /// </summary>
        /// <param name="contents"></param>
        /// <returns></returns>
        public static Sha256Sm3Hash Of(byte[] contents)
        {
            return Wrap(Hash(contents));
        }

        /// <summary>
        /// Hash
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] Hash(byte[] input)
        {
            return Hash(input, 0, input.Length);
        }

        /// <summary>
        /// Hash
        /// </summary>
        /// <param name="input"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] Hash(byte[] input, int offset, int length)
        {
            Sha256Digest digest = new Sha256Digest();
            digest.BlockUpdate(input, offset, length);

            byte[] output = new byte[digest.GetDigestSize()];
            digest.DoFinal(output, 0);

            return output;

        }

        /// <summary>
        /// CheckArgument
        /// </summary>
        /// <param name="result"></param>
        private void CheckArgument(bool result)
        {
            if (!result) throw new ArgumentException();
        }

        #endregion
    }
}
