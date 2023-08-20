using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.BouncyCastle.Math;

namespace AtomicCore.BlockChain.TronNet.Tests
{
    [TestClass()]
    public class ECKeyTests
    {
        [TestMethod()]
        public void GetPubKeyTest()
        {
            var privateKey = "F43EBCC94E6C257EDBE559183D1A8778B2D5A08040902C0F0A77A3343A1D0EA5";

            var prvKey = privateKey.HexToByteArray();
            var ecKey = new ECKey(prvKey, true);

            var publicKey0 = ecKey.GetPubKey();
            var publicKey1 = private2PublicDemo(prvKey);

            Assert.IsTrue(PasswordEquals(publicKey0, publicKey1));
        }

        private byte[] private2PublicDemo(byte[] privateKey)
        {
            var privKey = new BigInteger(1, privateKey);
            var point = ECKey.CURVE.G.Multiply(privKey);
            return point.GetEncoded();
        }

        /// <summary>
        /// 比较两个字节数组是否相等
        /// </summary>
        /// <param name="b1">byte数组1</param>
        /// <param name="b2">byte数组2</param>
        /// <returns>是否相等</returns>
        private bool PasswordEquals(byte[] b1, byte[] b2)
        {
            if (b1.Length != b2.Length) return false;
            if (b1 == null || b2 == null) return false;
            for (int i = 0; i < b1.Length; i++)
                if (b1[i] != b2[i])
                    return false;
            return true;
        }
    }
}