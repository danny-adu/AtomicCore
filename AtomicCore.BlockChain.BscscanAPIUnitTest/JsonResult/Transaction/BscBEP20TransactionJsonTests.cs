using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AtomicCore.BlockChain.BscscanAPI.Tests
{
    [TestClass()]
    public class BscBEP20TransactionJsonTests
    {
        [TestMethod()]
        public void GetToAddressTest()
        {
            BscBEP20TransactionJson json = new BscBEP20TransactionJson()
            {
                TxInput = "0xa9059cbb00000000000000000000000094eb91817700513779b7b99938c29f8862bc410700000000000000000000000000000000000000000000002a5a058fc295ed0000"
            };

            string toAddress = json.GetToAddress();


            Assert.IsTrue(toAddress.Length == 42);
        }

        [TestMethod()]
        public void GetTransferAmountTest()
        {
            BscBEP20TransactionJson json = new BscBEP20TransactionJson()
            {
                TxInput = "0xa9059cbb00000000000000000000000094eb91817700513779b7b99938c29f8862bc410700000000000000000000000000000000000000000000002a5a058fc295ed0000",
                TokenDecimal = 18
            };

            decimal amount = json.GetTransferAmount();


            Assert.IsTrue(amount > decimal.Zero);
        }
    }
}