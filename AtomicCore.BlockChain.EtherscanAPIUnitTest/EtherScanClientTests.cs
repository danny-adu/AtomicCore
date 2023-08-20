using AtomicCore.BlockChain.EtherscanAPI;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AtomicCore.BlockChain.EtherscanAPI.Tests
{
    [TestClass()]
    public class EtherScanClientTests
    {
        private readonly IEtherScanClient _client = new EtherScanClient("N4KR7R89K78AWKPBZY6WD27DDTTDB8YJ8W");

        [TestMethod()]
        public void UseWebAgent()
        {
            IEtherScanClient _client = new EtherScanClient("N4KR7R89K78AWKPBZY6WD27DDTTDB8YJ8W", EtherScanClient.c_eth_goerli, "http://129.226.189.12/Remote/Get?url={0}");

            var result = _client.GetBalance("0x29aAe16abfDC4C6F119E86D09ab8603D491c5d5F");

            Assert.IsTrue(result.Status == EtherscanJsonStatus.Success);
        }

        [TestMethod()]
        public void GetGasOracleTest()
        {
            EtherscanSingleResult<EthGasOracleJsonResult> result = this._client.GetGasOracle();

            Assert.IsTrue(result.Status == EtherscanJsonStatus.Success);
        }

        [TestMethod()]
        public void GoerliTransactionTest()
        {
            IEtherScanClient _client = new EtherScanClient("N4KR7R89K78AWKPBZY6WD27DDTTDB8YJ8W", EtherScanClient.c_eth_goerli);
            var result = _client.GetNormalTransactions("0x29aAe16abfDC4C6F119E86D09ab8603D491c5d5F");

            Assert.IsTrue(result.Status == EtherscanJsonStatus.Success);
        }

        [TestMethod()]
        public void GetBalanceTest()
        {
            EtherscanSingleResult<decimal> result = this._client.GetBalance("0xcF62baF1237124d11740D4c89eF088C501FA102A");

            Assert.IsTrue(result.Status == EtherscanJsonStatus.Success);

            result = this._client.GetBalance("0xcF62baF1237124d11740D4c89eF088C501FA102A", "0xA2b4C0Af19cC16a6CfAcCe81F192B024d625817D", 9);

            Assert.IsTrue(result.Status == EtherscanJsonStatus.Success);
        }

        [TestMethod()]
        public void GetTransactionsTest()
        {
            EtherscanListResult<EthNormalTransactionJsonResult> result = this._client.GetNormalTransactions("0xcF62baF1237124d11740D4c89eF088C501FA102A");

            Assert.IsTrue(result.Status == EtherscanJsonStatus.Success);
        }

        [TestMethod()]
        public void GetInternalTransactionsTest()
        {
            EtherscanListResult<EthInternalTransactionJsonResult> result = this._client.GetInternalTransactions("0x2c1ba59d6f58433fb1eaee7d20b26ed83bda51a3");

            Assert.IsTrue(result.Status == EtherscanJsonStatus.Success);
        }

        [TestMethod()]
        public void GetERC20TransactionsTest()
        {
            EtherscanListResult<EthErc20TransactionJsonResult> result = this._client.GetERC20Transactions(
                "0x849A02be4c2ec8BbD06052C5A0Cd51147994Ad96",
                "0xdAC17F958D2ee523a2206206994597C13D831ec7"
            );

            Assert.IsTrue(result.Status == EtherscanJsonStatus.Success);
        }

        [TestMethod()]
        public void GetContractAbiTest()
        {
            EtherscanSingleResult<string> result = this._client.GetContractAbi("0xfeffbc959961b6e24cbaf8a91a6ca6abd1c3ffc5");

            Assert.IsTrue(result.Status == EtherscanJsonStatus.Success);
        }

        [TestMethod()]
        public void GetBlockNumberTest()
        {
            var result = this._client.GetBlockNumber();

            Assert.IsTrue(result.Result>= 0);
        }

        [TestMethod()]
        public void GetTransactionCountTest()
        {
            var result = this._client.GetTransactionCount("0xa9C1de6B74bF9ed9710871bc3274b7E2fB12F363");

            Assert.IsTrue(result.Result >= 0);
        }
    }
}