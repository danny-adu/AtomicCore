using Microsoft.VisualStudio.TestTools.UnitTesting;
using AtomicCore.BlockChain.TronNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtomicCore.BlockChain.TronNet.Tests
{
    [TestClass()]
    public class TronNetRestTests
    {
        #region Variables

        private readonly TronTestRecord _record;
        private readonly ITronNetRest _restAPI;

        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        public TronNetRestTests()
        {
            _record = TronTestServiceExtension.GetMainRecord();
            _restAPI = _record.TronClient.GetRestAPI();
        }

        #endregion

        #region ITronAddressUtilitiesRestAPI

        [TestMethod()]
        public void GenerateAddressTest()
        {
            var result = _restAPI.GenerateAddress();
            Assert.IsTrue(result.IsAvailable());

            string tronAddress = TronNetECKey.ConvertToTronAddressFromHexAddress(result.HexAddress);
            Assert.IsTrue(tronAddress.Equals(result.Address));
        }

        [TestMethod()]
        public void CreateAddressTest()
        {
            //TestNet
            TronTestRecord shatasnet = TronTestServiceExtension.GetTestRecord();
            ITronNetRest testRestAPI = shatasnet.TronClient.GetRestAPI();

            var result = testRestAPI.CreateAddress("123456");

            Assert.IsTrue(result.IsAvailable());
        }

        [TestMethod()]
        public void ValidateAddressTest()
        {
            var result = _restAPI.ValidateAddress("TEfiVcH2MF43NDXLpxmy6wRpaMxnZuc4iX");

            Assert.IsTrue(result.Result);
        }

        #endregion

        #region ITronNetAccountRest

        [TestMethod()]
        public void CreateAccountTest()
        {
            //Create Offline Address
            TronNetECKey newAddress = TronNetECKey.GenerateKey();
            string address = newAddress.GetPublicAddress();
            string privateKey = newAddress.GetPrivateKey();
            Assert.IsTrue(!string.IsNullOrEmpty(privateKey));

            //TestNet
            TronTestRecord shatasnet = TronTestServiceExtension.GetTestRecord();
            ITronNetRest testRestAPI = shatasnet.TronClient.GetRestAPI();

            TronNetCreateTransactionRestJson result = testRestAPI.CreateAccount(TronTestAccountCollection.TestMain.Address, address);

            Assert.IsTrue(result.IsAvailable());
        }

        [TestMethod()]
        public void GetAccountTest()
        {
            //TestNet
            TronTestRecord shatasnet = TronTestServiceExtension.GetTestRecord();
            ITronNetRest testRestAPI = shatasnet.TronClient.GetRestAPI();

            TronNetAccountBalanceJson account = testRestAPI.GetAccount(TronTestAccountCollection.TestMain.Address);

            Assert.IsTrue(account.IsAvailable());
        }

        [TestMethod()]
        public void UpdateAccountTest()
        {
            //TestNet
            TronTestRecord shatasnet = TronTestServiceExtension.GetTestRecord();
            ITronNetRest testRestAPI = shatasnet.TronClient.GetRestAPI();

            TronNetCreateTransactionRestJson result = testRestAPI.UpdateAccount(Guid.NewGuid().ToString("N"), TronTestAccountCollection.TestMain.Address);

            Assert.IsTrue(result.IsAvailable());
        }

        [TestMethod()]
        public void GetAccountBalanceTest()
        {
            //TestNet
            TronTestRecord shatasnet = TronTestServiceExtension.GetTestRecord();
            ITronNetRest testRestAPI = shatasnet.TronClient.GetRestAPI();

            TronNetBlockAccountBalanceJson result = testRestAPI.GetAccountBalance(TronTestAccountCollection.TestMain.Address, "000000000111bc16322bbbb248140dccd7e35cce956969679f52cf55060f7f86", 17939478);

            Assert.IsTrue(result.IsAvailable());
        }

        #endregion

        #region ITronNetAccountResourcesRests

        [TestMethod()]
        public void GetAccountResourceTest()
        {
            //TestNet
            TronTestRecord shatasnet = TronTestServiceExtension.GetTestRecord();
            ITronNetRest testRestAPI = shatasnet.TronClient.GetRestAPI();

            var result = testRestAPI.GetAccountResource(TronTestAccountCollection.TestMain.Address);

            Assert.IsTrue(null != result);
        }

        [TestMethod()]
        public void GetAccountNetTest()
        {
            //TestNet
            TronTestRecord shatasnet = TronTestServiceExtension.GetTestRecord();
            ITronNetRest testRestAPI = shatasnet.TronClient.GetRestAPI();

            var result = testRestAPI.GetAccountNet(TronTestAccountCollection.TestMain.Address);

            Assert.IsTrue(null != result);
        }

        [TestMethod()]
        public void FreezeBalanceTest()
        {
            //TestNet
            TronTestRecord shatasnet = TronTestServiceExtension.GetTestRecord();
            ITronNetRest testRestAPI = shatasnet.TronClient.GetRestAPI();

            var result = testRestAPI.FreezeBalance(
                TronTestAccountCollection.TestMain.Address,
                1,
                3,
                TronNetResourceType.ENERGY
            );

            Assert.IsTrue(result.IsAvailable());
        }

        [TestMethod()]
        public void UnfreezeBalanceTest()
        {
            //TestNet
            TronTestRecord shatasnet = TronTestServiceExtension.GetTestRecord();
            ITronNetRest testRestAPI = shatasnet.TronClient.GetRestAPI();

            var result = testRestAPI.UnfreezeBalance(
                TronTestAccountCollection.TestMain.Address,
                TronNetResourceType.ENERGY
            );

            Assert.IsTrue(result.IsAvailable());
        }

        [TestMethod()]
        public void GetDelegatedResourceTest()
        {
            //TestNet
            TronTestRecord shatasnet = TronTestServiceExtension.GetTestRecord();
            ITronNetRest testRestAPI = shatasnet.TronClient.GetRestAPI();

            var result = testRestAPI.GetDelegatedResource(
                TronTestAccountCollection.TestMain.Address,
                TronTestAccountCollection.TestA.Address
            );

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void GetDelegatedResourceAccountIndexTest()
        {
            //TestNet
            TronTestRecord shatasnet = TronTestServiceExtension.GetTestRecord();
            ITronNetRest testRestAPI = shatasnet.TronClient.GetRestAPI();

            TronNetDelegatedResourceAccountJson result = testRestAPI.GetDelegatedResourceAccountIndex(
                TronTestAccountCollection.TestMain.Address
            );
            Assert.IsTrue(result.IsAvailable());

            string json_text = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            Assert.IsTrue(!string.IsNullOrEmpty(json_text));
        }

        #endregion

        #region ITronTransactionsRest

        [TestMethod()]
        public void CreateTransactionTest()
        {
            TronTestRecord shatasnet = TronTestServiceExtension.GetTestRecord();
            ITronNetRest testRestAPI = shatasnet.TronClient.GetRestAPI();

            TronNetCreateTransactionRestJson result = testRestAPI.CreateTransaction(
                TronTestAccountCollection.TestMain.Address,
                TronTestAccountCollection.TestA.Address,
                1
            );

            Assert.IsTrue(!string.IsNullOrEmpty(result.TxID));
        }

        [TestMethod()]
        public void GetTransactionSignTest()
        {
            TronTestRecord shatasnet = TronTestServiceExtension.GetTestRecord();
            ITronNetRest testRestAPI = shatasnet.TronClient.GetRestAPI();

            TronNetCreateTransactionRestJson createTransaction = testRestAPI.CreateTransaction(
                TronTestAccountCollection.TestMain.Address,
                TronTestAccountCollection.TestA.Address,
                1
            );
            Assert.IsTrue(createTransaction.IsAvailable());

            var result = testRestAPI.GetTransactionSign(TronTestAccountCollection.TestMain.PirvateKey, createTransaction);

            Assert.IsTrue(result.IsAvailable());
        }

        /// <summary>
        /// API ERROR
        /// </summary>
        [TestMethod()]
        public void BroadcastTransactionTest()
        {
            TronTestRecord shatasnet = TronTestServiceExtension.GetTestRecord();
            ITronNetRest testRestAPI = shatasnet.TronClient.GetRestAPI();

            string from = TronTestAccountCollection.TestMain.Address;
            string to = TronTestAccountCollection.TestA.Address;
            string from_priv = TronTestAccountCollection.TestMain.PirvateKey;

            //create transaction
            TronNetCreateTransactionRestJson createTransaction = _restAPI.CreateTransaction(
                "TK7XWSuRi5PxYDUQ53L43baio7ZBWukcGm",
                "TEEBzBuyVvE2YT1ub3xHe9UtcfwXtS1KeV",
                1
            );
            Assert.IsTrue(!string.IsNullOrEmpty(createTransaction.TxID));

            //sign transaction
            TronNetSignedTransactionRestJson signTransaction = _restAPI.GetTransactionSign(from_priv, createTransaction);

            //broadcast transaction
            TronNetResultJson result = testRestAPI.BroadcastTransaction(signTransaction, signTransaction.Signature);
            Assert.IsTrue(result.Result);
        }

        #endregion

        #region ITronQueryNetworkRestAPI

        [TestMethod()]
        public void GetBlockByNumTest()
        {
            TronNetBlockJson result = _restAPI.GetBlockByNum(200);

            Assert.IsTrue(!string.IsNullOrEmpty(result.BlockID));
        }

        [TestMethod()]
        public void GetBlockByIdTest()
        {
            TronNetBlockJson result = _restAPI.GetBlockById("0000000001f9f486548b36b7a54732ffc070b4311247cb88999cf7cef5c1bfa2");

            Assert.IsTrue(!string.IsNullOrEmpty(result.BlockID));
        }

        [TestMethod()]
        public void GetBlockByLatestNumTest()
        {
            TronNetBlockListJson result = _restAPI.GetBlockByLatestNum(10);

            Assert.IsTrue(result.Blocks != null);
        }

        [TestMethod()]
        public void GetBlockByLimitNextTest()
        {
            TronNetBlockListJson result = _restAPI.GetBlockByLimitNext(10, 11);

            Assert.IsTrue(result.Blocks != null);
        }

        [TestMethod()]
        public void GetNowBlockTest()
        {
            TronNetBlockDetailsJson result = _restAPI.GetNowBlock();

            Assert.IsTrue(result != null);
        }

        [TestMethod()]
        public void GetTransactionByIDTest()
        {
            string txid = "ca8d10f2b141a3a8d8e31453ff50716258d873c89fd189f6abce92effaa1960d";

            TronNetTransactionRestJson rest_txInfo = _restAPI.GetTransactionByID(txid);

            TronNetContractJson contractJson = rest_txInfo.RawData.Contract.FirstOrDefault();
            Assert.IsNotNull(contractJson);

            string ownerAddress = contractJson.Parameter.Value.GetOwnerAddress();
            Assert.IsTrue(!string.IsNullOrEmpty(ownerAddress));

            TronNetTriggerSmartContractJson valueJson = contractJson.Parameter.Value.ToContractValue<TronNetTriggerSmartContractJson>();
            Assert.IsNotNull(valueJson);

            string toEthAddress = valueJson.GetToEthAddress();
            Assert.IsTrue("0x10b6bb9e59f3e7b139a3e23c340eabc841817976".Equals(toEthAddress, StringComparison.OrdinalIgnoreCase));

            string toTronAddress = valueJson.GetToTronAddress();
            Assert.IsTrue("TBVaidbMvnXovzHJV7TTxeZ5Tkehxrx5UW".Equals(toTronAddress, StringComparison.OrdinalIgnoreCase));

            ulong amount = valueJson.GetOriginalAmount();
            Assert.IsTrue(amount > 0);

            Assert.IsTrue(!string.IsNullOrEmpty(rest_txInfo.TxID));
        }

        [TestMethod()]
        public void GetTransactionInfoByIdTest()
        {
            //TRX => f337385642d56a981fe8938049e3765e6abcca53ac9412a327b1906df272bdc1
            //TRC10 => c43d19f4517ce1a4c31c66eb4d9b41409caddc3be1cb733e28e941b3220e1b2d
            //TRC20 => f2eb864b3058b708d082b4aecf6573bc5606c741b51cf8870da30dd56e4aae40

            TronNetTransactionInfoJson result = _restAPI.GetTransactionInfoById("f2eb864b3058b708d082b4aecf6573bc5606c741b51cf8870da30dd56e4aae40");

            Assert.IsTrue(!string.IsNullOrEmpty(result.TxID));
        }

        [TestMethod()]
        public void ListNodesTest()
        {
            var result = _restAPI.ListNodes();

            Assert.IsTrue(null != result);
        }

        [TestMethod()]
        public void GetNodeInfoTest()
        {
            TronNetNodeOverviewJson result = _restAPI.GetNodeInfo();

            Assert.IsTrue(null != result);
        }

        [TestMethod()]
        public void GetChainParametersTest()
        {
            TronNetChainParameterOverviewJson result = _restAPI.GetChainParameters();

            Assert.IsTrue(null != result);
        }

        [TestMethod()]
        public void GetBlockBalanceTest()
        {
            TronNetBlockBalanceJson result = _restAPI.GetBlockBalance("0000000001fdce09a9d5c86b14ac81c6f89f63abfe31fbf64fe869ea1f8baff5", 33410569, true);
            if (!result.IsAvailable())
                Assert.IsTrue(!string.IsNullOrEmpty(result.Error));

            result.Error = null;
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(result);

            Assert.IsTrue(null != result);
        }

        #endregion

        #region ITronNetTRC10TokenRest

        [TestMethod()]
        public void GetAssetIssueByAccountTest()
        {
            //TestNet
            ////TronNetAssetCollectionJson result = _restAPI.GetAssetIssueByAccount("TXLL4wzNZicjNZDcE9KM987dSaxpffWjkq");
            ////Assert.IsTrue(null != result.AssetIssue && result.AssetIssue.Any());

            //TestNet
            TronTestRecord shatasnet = TronTestServiceExtension.GetTestRecord();
            ITronNetRest testRestAPI = shatasnet.TronClient.GetRestAPI();
            TronNetAssetCollectionJson test_result = testRestAPI.GetAssetIssueByAccount("TEhn1qUkP28puJjeVeo9TK27zu2gJEACin");
            Assert.IsTrue(null != test_result.AssetIssue && test_result.AssetIssue.Any());
        }

        [TestMethod()]
        public void GetAssetIssueByIdTest()
        {
            TronNetAssetInfoJson result = _restAPI.GetAssetIssueById(1000001);

            Assert.IsTrue("1000001".Equals(result.ID));
        }

        [TestMethod()]
        public void GetAssetIssueListTest()
        {
            TronNetAssetCollectionJson result = _restAPI.GetAssetIssueList();

            Assert.IsTrue(null != result.AssetIssue && result.AssetIssue.Any());
        }

        [TestMethod()]
        public void GetPaginatedAssetIssueListTest()
        {
            TronNetAssetCollectionJson result = _restAPI.GetPaginatedAssetIssueList(1, 1);

            Assert.IsTrue(null != result.AssetIssue && result.AssetIssue.Any());
        }

        [TestMethod()]
        public void TransferAssetTest()
        {
            TronTestRecord shatasnet = TronTestServiceExtension.GetTestRecord();
            ITronNetRest testRestAPI = shatasnet.TronClient.GetRestAPI();

            //create transactin
            TronNetCreateTransactionRestJson createTransactionResult = testRestAPI.TransferAsset(TronTestAccountCollection.TestMain.Address, TronTestAccountCollection.TestA.Address, "1000962", 10000);
            Assert.IsTrue(createTransactionResult.IsAvailable());
        }

        [TestMethod()]
        public void CreateAssetIssueTest()
        {
            TronTestRecord shatasnet = TronTestServiceExtension.GetTestRecord();
            ITronNetRest testRestAPI = shatasnet.TronClient.GetRestAPI();

            //create transactin
            TronNetCreateTransactionRestJson createTransactionResult = testRestAPI.CreateAssetIssue(TronTestAccountCollection.TestA.Address, "HuZiToken", 2, "HZT", 2100000000, 1, 1, DateTime.Now.AddDays(1), DateTime.Now.AddDays(2), "hu hu hu", "http://www.google.com", 10000, 10000, new TronNetFrozenSupplyJson()
            {
                FrozenAmount = 1,
                FrozenDays = 2
            });
            Assert.IsTrue(createTransactionResult.IsAvailable());
        }








        #endregion

        #region ITronNetSmartContractsRest

        [TestMethod()]
        public void GetContractTest()
        {
            var mainnetAPI = _record.TronClient.GetRestAPI();
            var usdt = mainnetAPI.GetContract("TR7NHqjeKQxGTCi8q8ZY4pL8otSzgjLj6t");
            Assert.IsTrue(usdt.IsAvailable());


            ////TestNet
            //TronTestRecord shatasnet = TronTestServiceExtension.GetTestRecord();
            //ITronNetRest testRestAPI = shatasnet.TronClient.GetRestAPI();

            ////abi index 26 
            //TronNetContractMetaDataJson result = testRestAPI.GetContract("TB7whW3J9jb5Amoi4R6WgTtMbWPeqMBjSw");

            //Assert.IsTrue(result.IsAvailable());
        }

        #endregion
    }
}