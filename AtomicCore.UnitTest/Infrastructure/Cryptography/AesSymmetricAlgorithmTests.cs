using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Security.Cryptography;
using System.Text;

namespace AtomicCore.Tests
{
    [TestClass()]
    public class AesSymmetricAlgorithmTests
    {
        public AesSymmetricAlgorithmTests()
        {
            AtomicCore.AtomicKernel.Initialize();
        }

        [TestMethod()]
        public void CryptoJs()
        {
            string KEY = "abcdefgabcdefg12";

            byte[] orig_bys = UTF8Encoding.UTF8.GetBytes("1234afd");

            string result = null;
            using (var rDel = new RijndaelManaged())
            {
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;

                rDel.KeySize = 128;
                rDel.Key = UTF8Encoding.UTF8.GetBytes(KEY);

                byte[] cipherBytes;
                using (ICryptoTransform transform = rDel.CreateEncryptor())
                    cipherBytes = transform.TransformFinalBlock(orig_bys, 0, orig_bys.Length);

                result = Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);
            }

            Assert.IsTrue(!string.IsNullOrEmpty(result));
        }

        [TestMethod()]
        public void EncryptTest()
        {
            IEncryptAlgorithm encrypt = AtomicCore.AtomicKernel.Dependency.Resolve<IEncryptAlgorithm>(CryptoMethods.AES);

            var result = encrypt.Encrypt("1234afd", "abcdefgabcdefg12");

            Assert.IsTrue(!string.IsNullOrEmpty(result));
        }

        [TestMethod()]
        public void DecryptTest()
        {
            IEncryptAlgorithm encrypt = AtomicCore.AtomicKernel.Dependency.Resolve<IEncryptAlgorithm>(CryptoMethods.AES);
            IDecryptAlgorithm decrypt = AtomicCore.AtomicKernel.Dependency.Resolve<IDecryptAlgorithm>(CryptoMethods.AES);

            var encryptResult = encrypt.Encrypt("C#AES加密字符串", "ae125efkk4454eeff444ferfkny6oxi8");

            var result = decrypt.Decrypt(encryptResult, "ae125efkk4454eeff444ferfkny6oxi8");

            Assert.IsTrue(!string.IsNullOrEmpty(result));
        }
    }
}