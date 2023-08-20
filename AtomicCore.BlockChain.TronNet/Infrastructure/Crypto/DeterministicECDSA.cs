using System;
using System.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Deterministic ECDSA
    /// </summary>
    internal class DeterministicECDSA : ECDsaSigner
    {
        #region Variables

        private readonly IDigest _digest;
        private byte[] _buffer = Array.Empty<byte>();

        #endregion

        #region Constructor

        /// <summary>
        /// Deterministic ECDSA
        /// </summary>
        public DeterministicECDSA()
            : base(new HMacDsaKCalculator(new Sha256Digest()))

        {
            _digest = new Sha256Digest();
        }

        /// <summary>
        /// Deterministic ECDSA
        /// </summary>
        /// <param name="digest"></param>
        public DeterministicECDSA(Func<IDigest> digest)
            : base(new HMacDsaKCalculator(digest()))
        {
            _digest = digest();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set Private Key
        /// </summary>
        /// <param name="ecKey"></param>
        public void SetPrivateKey(ECPrivateKeyParameters ecKey)
        {
            Init(true, ecKey);
        }

        /// <summary>
        /// Sign
        /// </summary>
        /// <returns></returns>
        public byte[] Sign()
        {
            var hash = new byte[_digest.GetDigestSize()];
            _digest.BlockUpdate(_buffer, 0, _buffer.Length);
            _digest.DoFinal(hash, 0);
            _digest.Reset();
            return SignHash(hash);
        }

        /// <summary>
        /// Sign Hash
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public byte[] SignHash(byte[] hash)
        {
            return new ECDSASignature(GenerateSignature(hash)).ToDER();
        }

        /// <summary>
        /// Update Buffer
        /// </summary>
        /// <param name="buf"></param>
        public void Update(byte[] buf)
        {
            _buffer = _buffer.Concat(buf).ToArray();
        }

        #endregion
    }

}
