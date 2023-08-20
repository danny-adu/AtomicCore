using AtomicCore.BlockChain.TronNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace AtomicCore.BlockChain.TronNet.Tests
{
    [TestClass()]
    public class TronGridRestTests
    {
        #region Variables

        private readonly TronTestRecord _record;
        private readonly ITronGridRest _gridApiClient;

        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        public TronGridRestTests()
        {
            _record = TronTestServiceExtension.GetMainRecord();
            _gridApiClient = _record.TronClient.GetGridAPI();
        }

        #endregion

        #region ITronGridAccountRest

        [TestMethod()]
        public void GetAccountTest()
        {
            var result = _gridApiClient.GetAccount("TK7XWSuRi5PxYDUQ53L43baio7ZBWukcGm");

            Assert.IsTrue(result.IsAvailable());
        }

        [TestMethod()]
        public void GetTransactionsTest()
        {
            var hex_address = TronNetECKey.ConvertToHexAddress("TEEBzBuyVvE2YT1ub3xHe9UtcfwXtS1KeV");

            var result = _gridApiClient.GetTransactions("TK7XWSuRi5PxYDUQ53L43baio7ZBWukcGm", new TronGridTransactionQuery()
            {
                Limit = 20,
                OnlyTo = true
            });

            var item = result.Data.FirstOrDefault(d => d.RawData.Contract.Any(d => d.Type == TronNetContractType.TriggerSmartContract));
            if (null != item)
            {
                var contract = item.RawData.Contract.FirstOrDefault(d => d.Type == TronNetContractType.TriggerSmartContract);
                var paramValue = contract.Parameter.Value;

                if (paramValue.IncludContractAddress("TR7NHqjeKQxGTCi8q8ZY4pL8otSzgjLj6t"))
                {
                    var trc20Info = paramValue.Parse<TronGridTriggerSmartContractInfo>();

                    string toAddress = trc20Info.GetToAddress();
                    var rawAmount = trc20Info.GetRawAmount();
                    var amount = trc20Info.GetAmount(6);
                }
            }

            Assert.IsTrue(result.IsAvailable());
        }

        [TestMethod()]
        public void GetTrc20TransactionsTest()
        {
            var result = _gridApiClient.GetTrc20Transactions("TK7XWSuRi5PxYDUQ53L43baio7ZBWukcGm");

            Assert.IsTrue(result.IsAvailable());
        }


        #endregion

        #region ITronGridTRC10Rest

        [TestMethod()]
        public void GetTrc10ListTest()
        {
            var result = _gridApiClient.GetTrc10List();

            Assert.IsTrue(result.IsAvailable());
        }

        [TestMethod()]
        public void GetTrc10ListByNameTest()
        {
            var result = _gridApiClient.GetTrc10ListByName("USDT");

            Assert.IsTrue(result.IsAvailable());
        }

        [TestMethod()]
        public void GetTrc10ListByIdentifierTest()
        {
            var result = _gridApiClient.GetTrc10ListByIdentifier("1000006");

            Assert.IsTrue(result.IsAvailable());
        }

        #endregion


    }
}