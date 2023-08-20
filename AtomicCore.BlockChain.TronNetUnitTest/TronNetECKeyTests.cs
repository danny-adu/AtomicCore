using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AtomicCore.BlockChain.TronNet.Tests
{
    [TestClass()]
    public class TronNetECKeyTests
    {
        [TestMethod()]
        public void ValidTronAddressTest()
        {
            var result = TronNetECKey.ValidTronAddress("THtLMnXkNqpJb1WCeLxbmpUix4M65W9999");

            Assert.IsTrue(result);
        }
    }
}