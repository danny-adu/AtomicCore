using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using System;
using System.Linq;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Crypto.Digests;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// ECKey based on the implementation of bitcoinj, NBitcoin
    /// </summary>
    public class ECKey
    {
        #region Variables

        public static readonly BigInteger HALF_CURVE_ORDER;
        public static readonly BigInteger CURVE_ORDER;
        public static readonly ECDomainParameters CURVE;
        private static readonly X9ECParameters _secp256k1;
        private static readonly SecureRandom _secureRandom = new SecureRandom();
        private static readonly BigInteger PRIME;

        private readonly ECKeyParameters _Key;
        private byte[] _publicKey;
        private ECPublicKeyParameters _ecPublicKeyParameters;
        private ECDomainParameters _DomainParameter;

        #endregion

        #region Constructor

        /// <summary>
        /// Static Constructor
        /// </summary>
        static ECKey()
        {
            //using Bouncy
            _secp256k1 = SecNamedCurves.GetByName("secp256k1");
            CURVE = new ECDomainParameters(_secp256k1.Curve, _secp256k1.G, _secp256k1.N, _secp256k1.H);
            HALF_CURVE_ORDER = _secp256k1.N.ShiftRight(1);
            CURVE_ORDER = _secp256k1.N;
            PRIME = new BigInteger(1,
               Org.BouncyCastle.Utilities.Encoders.Hex.Decode(
                   "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFC2F"));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="vch"></param>
        /// <param name="isPrivate"></param>
        public ECKey(byte[] vch, bool isPrivate)
        {
            if (isPrivate)
                _Key = new ECPrivateKeyParameters(new BigInteger(1, vch), DomainParameter);
            else
            {
                var q = Secp256k1.Curve.DecodePoint(vch);
                _Key = new ECPublicKeyParameters("EC", q, DomainParameter);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ECKey()
        {
            var generator = new ECKeyPairGenerator("EC");
            generator.Init(new ECKeyGenerationParameters(CURVE, _secureRandom));
            var pair = generator.GenerateKeyPair();
            _Key = (ECPrivateKeyParameters)pair.Private;
        }

        #endregion

        #region Propertys

        /// <summary>
        /// PrivateKey
        /// </summary>
        public ECPrivateKeyParameters PrivateKey => _Key as ECPrivateKeyParameters;

        /// <summary>
        /// Secp256k1
        /// </summary>
        public static X9ECParameters Secp256k1 => _secp256k1;

        /// <summary>
        /// DomainParameter
        /// </summary>
        public ECDomainParameters DomainParameter
        {
            get
            {
                if (_DomainParameter == null)
                    _DomainParameter = new ECDomainParameters(Secp256k1.Curve, Secp256k1.G, Secp256k1.N, Secp256k1.H);
                return _DomainParameter;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get PubKey NoPrefix
        /// </summary>
        /// <returns></returns>
        public byte[] GetPubKeyNoPrefix()
        {
            byte[] pubKey = GetPubKey();
            byte[] arr = new byte[pubKey.Length - 1];

            //remove the prefix
            Array.Copy(pubKey, 1, arr, 0, arr.Length);

            return arr;
        }

        /// <summary>
        /// Get PubKey
        /// </summary>
        /// <returns></returns>
        public byte[] GetPubKey()
        {
            if (_publicKey != null) return _publicKey;

            ECPoint q = GetPublicKeyParameters().Q;
            q = q.Normalize();

            _publicKey =
            Secp256k1.Curve.CreatePoint(q.XCoord.ToBigInteger(), q.YCoord.ToBigInteger()).GetEncoded(false);

            return _publicKey;
        }

        /// <summary>
        /// Get PublicKey Parameters
        /// </summary>
        /// <returns></returns>
        public ECPublicKeyParameters GetPublicKeyParameters()
        {
            if (_ecPublicKeyParameters == null)
            {
                if (_Key is ECPublicKeyParameters parameters)
                    _ecPublicKeyParameters = parameters;
                else
                {
                    ECPoint q = Secp256k1.G.Multiply(PrivateKey.D);
                    _ecPublicKeyParameters = new ECPublicKeyParameters("EC", q, DomainParameter);

                }
            }

            return _ecPublicKeyParameters;
        }

        /// <summary>
        /// Recover From Signature
        /// </summary>
        /// <param name="recId"></param>
        /// <param name="sig"></param>
        /// <param name="message"></param>
        /// <param name="compressed"></param>
        /// <returns></returns>
        public static ECKey RecoverFromSignature(int recId, ECDSASignature sig, byte[] message, bool compressed)
        {
            if (recId < 0)
                throw new ArgumentException("recId should be positive");
            if (sig.R.SignValue < 0)
                throw new ArgumentException("r should be positive");
            if (sig.S.SignValue < 0)
                throw new ArgumentException("s should be positive");
            if (message == null)
                throw new ArgumentNullException("message");

            X9ECParameters curve = Secp256k1;

            // 1.0 For j from 0 to h   (h == recId here and the loop is outside this function)
            //   1.1 Let x = r + jn

            var n = curve.N;
            var i = BigInteger.ValueOf((long)recId / 2);
            var x = sig.R.Add(i.Multiply(n));

            //   1.2. Convert the integer x to an octet string X of length mlen using the conversion routine
            //        specified in Section 2.3.7, where mlen = ⌈(log2 p)/8⌉ or mlen = ⌈m/8⌉.
            //   1.3. Convert the octet string (16 set binary digits)||X to an elliptic curve point R using the
            //        conversion routine specified in Section 2.3.4. If this conversion routine outputs “invalid”, then
            //        do another iteration of Step 1.
            //
            // More concisely, what these points mean is to use X as a compressed public key.

            //using bouncy and Q value of Point

            if (x.CompareTo(PRIME) >= 0)
                return null;

            // Compressed keys require you to know an extra bit of data about the y-coord as there are two possibilities.
            // So it's encoded in the recId.
            var R = DecompressKey(x, (recId & 1) == 1);
            //   1.4. If nR != point at infinity, then do another iteration of Step 1 (callers responsibility).

            if (!R.Multiply(n).IsInfinity)
                return null;

            //   1.5. Compute e from M using Steps 2 and 3 of ECDSA signature verification.
            var e = new BigInteger(1, message);
            //   1.6. For k from 1 to 2 do the following.   (loop is outside this function via iterating recId)
            //   1.6.1. Compute a candidate public key as:
            //               Q = mi(r) * (sR - eG)
            //
            // Where mi(x) is the modular multiplicative inverse. We transform this into the following:
            //               Q = (mi(r) * s ** R) + (mi(r) * -e ** G)
            // Where -e is the modular additive inverse of e, that is z such that z + e = 0 (mod n). In the above equation
            // ** is point multiplication and + is point addition (the EC group operator).
            //
            // We can find the additive inverse by subtracting e from zero then taking the mod. For example the additive
            // inverse of 3 modulo 11 is 8 because 3 + 8 mod 11 = 0, and -3 mod 11 = 8.

            BigInteger eInv = BigInteger.Zero.Subtract(e).Mod(n);
            BigInteger rInv = sig.R.ModInverse(n);
            BigInteger srInv = rInv.Multiply(sig.S).Mod(n);
            BigInteger eInvrInv = rInv.Multiply(eInv).Mod(n);
            ECPoint q = ECAlgorithms.SumOfTwoMultiplies(curve.G, eInvrInv, R, srInv);
            q = q.Normalize();
            if (compressed)
            {
                q = Secp256k1.Curve.CreatePoint(q.XCoord.ToBigInteger(), q.YCoord.ToBigInteger());
                return new ECKey(q.GetEncoded(true), false);
            }

            return new ECKey(q.GetEncoded(false), false);
        }

        /// <summary>
        /// Do Sign
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private ECDSASignature DoSign(byte[] input)
        {
            if (input.Length != 32)
                throw new ArgumentException(
                    "Expected 32 byte input to " + "ECDSA signature, not " + input.Length);

            if (PrivateKey == null)
                throw new MissingPrivateKeyException();

            ECDsaSigner signer = new ECDsaSigner(new HMacDsaKCalculator(new Sha256Digest()));
            ECPrivateKeyParameters privKeyParams = new ECPrivateKeyParameters(PrivateKey.D, CURVE);
            signer.Init(true, privKeyParams);

            BigInteger[] components = signer.GenerateSignature(input);

            return new ECDSASignature(components[0], components[1]).MakeCanonical();

        }

        /// <summary>
        /// Sign
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public ECDSASignature Sign(byte[] hash)
        {
            ECDSASignature sig = DoSign(hash);
            byte[] thisKey = GetPubKey();

            int recId = CalculateRecId(sig, hash, thisKey);
            sig.V = new byte[] { (byte)recId };

            return sig;
        }

        /// <summary>
        /// Verify
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="sig"></param>
        /// <returns></returns>
        public bool Verify(byte[] hash, ECDSASignature sig)
        {
            ECDsaSigner signer = new ECDsaSigner();
            signer.Init(false, GetPublicKeyParameters());

            return signer.VerifySignature(hash, sig.R, sig.S);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// CalculateRecId
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="hash"></param>
        /// <param name="uncompressedPublicKey"></param>
        /// <returns></returns>
        private static int CalculateRecId(ECDSASignature signature, byte[] hash, byte[] uncompressedPublicKey)
        {
            int recId = -1;

            for (int i = 0; i < 4; i++)
            {
                ECKey rec = ECKey.RecoverFromSignature(i, signature, hash, false);
                if (rec != null)
                {
                    byte[] k = rec.GetPubKey();
                    if (k != null && k.SequenceEqual(uncompressedPublicKey))
                    {
                        recId = i;
                        break;
                    }
                }
            }
            if (recId == -1)
                throw new Exception("Could not construct a recoverable key. This should never happen.");

            return recId;
        }

        /// <summary>
        /// Decompress Key
        /// </summary>
        /// <param name="xBN"></param>
        /// <param name="yBit"></param>
        /// <returns></returns>
        private static ECPoint DecompressKey(BigInteger xBN, bool yBit)
        {
            ECCurve curve = Secp256k1.Curve;
            byte[] compEnc = X9IntegerConverter.IntegerToBytes(xBN, 1 + X9IntegerConverter.GetByteLength(curve));
            compEnc[0] = (byte)(yBit ? 0x03 : 0x02);

            return curve.DecodePoint(compEnc);
        }

        #endregion
    }
}
