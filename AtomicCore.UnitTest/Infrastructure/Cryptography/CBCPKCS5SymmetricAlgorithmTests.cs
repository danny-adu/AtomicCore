using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Security.Cryptography;
using System.Text;

namespace AtomicCore.Tests
{
    [TestClass()]
    public class CBCPKCS5SymmetricAlgorithmTests
    {
        public CBCPKCS5SymmetricAlgorithmTests()
        {
            AtomicCore.AtomicKernel.Initialize();
        }

        [TestMethod()]
        public void SaltEncryptTest()
        {
            var cls = new ClsCrypto("123456");

            var encrypt = cls.Encrypt("1234afd");

            Assert.IsTrue(!string.IsNullOrEmpty(encrypt));
        }

        [TestMethod()]
        public void SaltDecryptTest()
        {
            var cls = new ClsCrypto("123456");

            var dencrypt = cls.Decrypt("EmPhSwSlopxv93BPOaQ3bQ==");

            Assert.IsTrue(!string.IsNullOrEmpty(dencrypt));
        }

        [TestMethod()]
        public void EncryptTest()
        {
            IEncryptAlgorithm encrypt = AtomicCore.AtomicKernel.Dependency.Resolve<IEncryptAlgorithm>(CryptoMethods.CBCPKCS5);

            var result = encrypt.Encrypt("1234afd", "123456");

            Assert.IsTrue(!string.IsNullOrEmpty(result));
        }

        [TestMethod()]
        public void DecryptTest()
        {
            IDecryptAlgorithm encrypt = AtomicCore.AtomicKernel.Dependency.Resolve<IDecryptAlgorithm>(CryptoMethods.CBCPKCS5);

            var result = encrypt.Decrypt("w9J5WCAYfPDL8r8gvT8i9A==", "123456");

            Assert.IsTrue(!string.IsNullOrEmpty(result));
        }

        public class ClsCrypto
        {
            private const int iterations = 6;
            private const int keyLength = 256;
            private const int blockSize = 128;

            private RijndaelManaged myRijndael = new RijndaelManaged();
            private byte[] salt = System.Text.Encoding.UTF8.GetBytes("insight123resultxyz");

            public ClsCrypto(string strPassword)
            {
                // salt
                var rfc2898 = new Rfc2898DeriveBytes(System.Text.Encoding.UTF8.GetBytes(strPassword), salt, iterations);
                Span<byte> keyVectorData = rfc2898.GetBytes(keyLength / 8 + blockSize / 8);
                var key = keyVectorData.Slice(0, keyLength / 8).ToArray();
                var iv = keyVectorData.Slice(keyLength / 8).ToArray();

                myRijndael.BlockSize = blockSize;
                myRijndael.KeySize = keyLength;
                myRijndael.IV = iv;

                myRijndael.Padding = PaddingMode.PKCS7;
                myRijndael.Mode = CipherMode.CBC;
                myRijndael.Key = key;
            }

            public string Encrypt(string strPlainText)
            {
                byte[] strText = new System.Text.UTF8Encoding().GetBytes(strPlainText);
                ICryptoTransform transform = myRijndael.CreateEncryptor();
                byte[] cipherText = transform.TransformFinalBlock(strText, 0, strText.Length);

                return Convert.ToBase64String(cipherText);
            }

            public string Decrypt(string encryptedText)
            {
                byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                var decryptor = myRijndael.CreateDecryptor(myRijndael.Key, myRijndael.IV);
                byte[] originalBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

                return Encoding.UTF8.GetString(originalBytes);
            }

            //public static byte[] HexStringToByteArray(string strHex)
            //{
            //    dynamic r = new byte[strHex.Length / 2];
            //    for (int i = 0; i <= strHex.Length - 1; i += 2)
            //    {
            //        r[i / 2] = Convert.ToByte(Convert.ToInt32(strHex.Substring(i, 2), 16));
            //    }
            //    return r;
            //}

            //private byte[] GenerateKey(string strPassword, int cb = 128 / 8)
            //{
            //    Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(System.Text.Encoding.UTF8.GetBytes(strPassword), salt, iterations);

            //    return rfc2898.GetBytes(cb);
            //}

            //private byte[] GenerateIV(string strPassword, int cb = 128 / 8)
            //{
            //    Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(System.Text.Encoding.UTF8.GetBytes(strPassword), salt, iterations);

            //    return rfc2898.GetBytes(cb);
            //}
        }
    }
}