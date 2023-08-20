using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AtomicCore.BlockChain.ExplorerAPI.Tests
{
    [TestClass()]
    public class BtcExplorerClientTests
    {
        /// <summary>
        /// 0000000000000bae09a7a393a8acded75aa67e46cb81f7acaa5ad94f9eacd103
        /// </summary>
        [TestMethod()]
        public void GetSingleBlockTest()
        {
            IBtcExplorerClient cli = new BtcExplorerClient();

            var result = cli.GetSingleBlock("0000000000000bae09a7a393a8acded75aa67e46cb81f7acaa5ad94f9eacd103");

            Assert.IsTrue(null != result);
        }

        [TestMethod()]
        public void UnspentOutputsTest()
        {
            IBtcExplorerClient cli = new BtcExplorerClient();

            var result = cli.UnspentOutputs("1DeRF2bLsSFRjsFBuyhfjJ9BE5PD9Uy7gL");

            Assert.IsTrue(null != result);
        }

        /// <summary>
        /// Get Address Balance 1ARjWDkZ7kT9fwjPrjcQyvbXDkEySzKHwu
        /// </summary>
        [TestMethod()]
        public void GetAddressBalanceTest()
        {
            IBtcExplorerClient cli = new BtcExplorerClient();

            var result = cli.GetAddressBalance("1ARjWDkZ7kT9fwjPrjcQyvbXDkEySzKHwu");

            Assert.IsTrue(null != result);
        }

        /// <summary>
        /// https://api.blockchain.info/haskoin-store/btc/address/3A1YfC2VeYZjVyavBAKhbjuc8odurW9yzQ/transactions/full
        /// </summary>
        [TestMethod()]
        public void GetAddressTxsTest()
        {
            IBtcExplorerClient cli = new BtcExplorerClient();

            var result = cli.GetAddressTxs("1EXoDusjGwvnjZUyKkxZ4UHEf77z6A5S4P", 0, 50);

            Assert.IsTrue(null != result);
        }
    }
}