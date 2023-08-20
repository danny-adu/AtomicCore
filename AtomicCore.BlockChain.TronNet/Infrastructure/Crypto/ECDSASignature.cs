using System;
using System.IO;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Math;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// ECDSA Signature
    /// </summary>
    public class ECDSASignature
    {
        #region Variables

        private const string InvalidDERSignature = "Invalid DER signature";

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="r"></param>
        /// <param name="s"></param>
        public ECDSASignature(BigInteger r, BigInteger s)
        {
            R = r;
            S = s;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rs"></param>
        public ECDSASignature(BigInteger[] rs)
        {
            R = rs[0];
            S = rs[1];
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="derSig"></param>
        public ECDSASignature(byte[] derSig)
        {
            try
            {
                Asn1InputStream decoder = new Asn1InputStream(derSig);
                DerSequence seq = decoder.ReadObject() as DerSequence;
                if (seq == null || seq.Count != 2)
                    throw new FormatException(InvalidDERSignature);
                R = ((DerInteger)seq[0]).Value;
                S = ((DerInteger)seq[1]).Value;
            }
            catch (Exception ex)
            {
                throw new FormatException(InvalidDERSignature, ex);
            }
        }

        #endregion

        #region Propertys

        /// <summary>
        /// R
        /// </summary>
        public BigInteger R { get; }

        /// <summary>
        /// S
        /// </summary>
        public BigInteger S { get; }

        /// <summary>
        /// V
        /// </summary>
        public byte[] V { get; set; }

        /// <summary>
        /// Is Low S
        /// </summary>
        public bool IsLowS => S.CompareTo(ECKey.HALF_CURVE_ORDER) <= 0;

        #endregion

        #region Pubic Mehtods

        /// <summary>
        /// From DER
        /// </summary>
        /// <param name="sig"></param>
        /// <returns></returns>
        public static ECDSASignature FromDER(byte[] sig)
        {
            return new ECDSASignature(sig);
        }

        /// <summary>
        /// IsValidDER
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static bool IsValidDER(byte[] bytes)
        {
            try
            {
                FromDER(bytes);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Enforce LowS on the signature
        /// </summary>
        public ECDSASignature MakeCanonical()
        {
            if (!IsLowS)
                return new ECDSASignature(R, ECKey.CURVE_ORDER.Subtract(S));

            return this;
        }

        /// <summary>
        /// ToDER
        /// </summary>
        /// <returns>
        /// What we get back from the signer are the two components of a signature, r and s. To get a flat byte stream of the type used by Bitcoin we have to encode them using DER encoding, which is just a way to pack the two components into a structure.
        /// </returns>
        public byte[] ToDER()
        {
            // Usually 70-72 bytes.
            var bos = new MemoryStream(72);
            var seq = new DerSequenceGenerator(bos);
            seq.AddObject(new DerInteger(R));
            seq.AddObject(new DerInteger(S));
            seq.Close();

            return bos.ToArray();
        }

        /// <summary>
        /// ToByteArray
        /// </summary>
        /// <returns></returns>
        public byte[] ToByteArray()
        {
            return ByteArrary.Merge(BigIntegerToBytes(R, 32), BigIntegerToBytes(S, 32), this.V);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// BigInteger To Bytes
        /// </summary>
        /// <param name="b"></param>
        /// <param name="numBytes"></param>
        /// <returns></returns>
        private static byte[] BigIntegerToBytes(BigInteger b, int numBytes)
        {
            if (b == null) return null;
            var bytes = new byte[numBytes];
            var biBytes = b.ToByteArray();
            var start = (biBytes.Length == numBytes + 1) ? 1 : 0;
            var length = Math.Min(biBytes.Length, numBytes);
            Array.Copy(biBytes, start, bytes, numBytes - length, length);

            return bytes;
        }

        #endregion
    }
}
