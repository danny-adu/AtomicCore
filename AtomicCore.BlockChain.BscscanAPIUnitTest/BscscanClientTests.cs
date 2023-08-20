using Microsoft.VisualStudio.TestTools.UnitTesting;
using AtomicCore.BlockChain.BscscanAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nethereum.Util;

namespace AtomicCore.BlockChain.BscscanAPI.Tests
{
    [TestClass()]
    public class BscscanClientTests
    {
        #region Variables

        private readonly IBscscanClient client;

        #endregion

        #region Constructor

        public BscscanClientTests()
        {
            client = new BscscanClient();
            client.SetApiKeyToken("Y18IQ48GQSKKNDZGUN5TAUDEHD84VS8ZQY");
        }

        #endregion

        #region IBscAccounts

        [TestMethod()]
        public void GetBalanceTest()
        {
            var result = client.GetBalance("0x0702383c8dd23081d1962c72EeDB72902c731940");

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }


        [TestMethod()]
        public void GetBalanceRawTest()
        {
            var result = client.GetBalanceRaw("0x0702383c8dd23081d1962c72EeDB72902c731940");

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }

        [TestMethod()]
        public void GetBalanceListTest()
        {
            var result = client.GetBalanceList(new string[]
            {
                "0x0000000000000000000000000000000000001004",
                "0x0702383c8dd23081d1962c72EeDB72902c731940"
            });

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }

        [TestMethod()]
        public void GetNormalTransactionByAddressTest()
        {
            var result = client.GetNormalTransactionByAddress("0x0702383c8dd23081d1962c72EeDB72902c731940");

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }

        [TestMethod()]
        public void GetInternalTransactionByAddressTest()
        {
            ////var hbi = System.Numerics.BigInteger.Parse("358424896370793");
            ////var amount = UnitConversion.Convert.FromWei(hbi, UnitConversion.EthUnit.Ether);

            var result = client.GetInternalTransactionByAddress("0x091dD81C8B9347b30f1A4d5a88F92d6F2A42b059");

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }

        [TestMethod()]
        public void GetInternalTransactionByHashTest()
        {
            var result = client.GetInternalTransactionByHash("0xe03b3199a41733cf167201f3b31cf77076689944d42f7a4a37f8b4d377ed1336");

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }

        [TestMethod()]
        public void GetBEP20TransactionByAddressTest()
        {
            var result = client.GetBEP20TransactionByAddress("0x0702383c8dd23081d1962c72EeDB72902c731940", "0xe9e7cea3dedca5984780bafc599bd69add087d56");

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }

        [TestMethod()]
        public void GetBEP721TransactionByAddressTest()
        {
            var result = client.GetBEP721TransactionByAddress("0x785D43bd5Bd506ca3B28b017394f14Ec04F9CCC9", "0xf51fb8de65f85cb18a2558c1d3769835f526f36c");

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }

        [TestMethod()]
        public void GetMinedBlockListByAddressTest()
        {
            var result = client.GetMinedBlockListByAddress("0x78f3adfc719c99674c072166708589033e2d9afe", "blocks");

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }

        #endregion

        #region IBscBlocks

        [TestMethod()]
        public void GetContractABITest()
        {
            var result = client.GetContractABI("0xe9e7cea3dedca5984780bafc599bd69add087d56");

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }

        #endregion

        #region IBscTransactions

        [TestMethod()]
        public void GetTransactionReceiptStatusTest()
        {
            var result = client.GetTransactionReceiptStatus("0xd0d1d4b745a221527ef38a5fbf22509ed5a5fb104efbab28d924b2fb9c19e93b");

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }

        #endregion

        #region IBscBlocks

        [TestMethod()]
        public void GetBlockRewardByNumberTest()
        {
            var result = client.GetBlockRewardByNumber(13467768);

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }

        [TestMethod()]
        public void GetBlockEstimatedByNumberTest()
        {
            var result = client.GetBlockEstimatedByNumber(14154145);

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }

        [TestMethod()]
        public void GetBlockNumberByTimestampTest()
        {
            var result = client.GetBlockNumberByTimestamp(1601510400, BscClosest.Before);

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }




        #endregion

        #region IBscGethProxy

        [TestMethod()]
        public void GetBlockNumberTest()
        {
            var result = client.GetBlockNumber();

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }

        [TestMethod()]
        public void GetBlockSimpleByNumberTest()
        {
            var result = client.GetBlockSimpleByNumber(10556486);

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }

        [TestMethod()]
        public void GetBlockFullByNumberTest()
        {
            var result = client.GetBlockFullByNumber(10556486);

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }

        [TestMethod()]
        public void GetBlockTransactionCountByNumberTest()
        {
            var result = client.GetBlockTransactionCountByNumber(10556486);

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }

        [TestMethod()]
        public void GetTransactionByHashTest()
        {
            var result = client.GetTransactionByHash("0x9983332a52df5ad1dabf8fa81b1642e9383f302a399c532fc47ecb6a7a967166");

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }

        [TestMethod()]
        public void GetTransactionByBlockNumberAndIndexTest()
        {
            var result = client.GetTransactionByBlockNumberAndIndex(10556486, 1);

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }

        [TestMethod()]
        public void GetTransactionCountTest()
        {
            var result = client.GetTransactionCount("0x4430b3230294D12c6AB2aAC5C2cd68E80B16b581");

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }

        [TestMethod()]
        public void SendRawTransactionTest()
        {
            var result = client.SendRawTransaction("");

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }

        [TestMethod()]
        public void GetTransactionReceiptTest()
        {
            var result = client.GetTransactionReceipt("0x2122b2317d6cf409846f80e829c1e45ecb30306907ba0a00a02730c78890739f");

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }

        [TestMethod()]
        public void CallTest()
        {
            var result = client.Call(
                "0xAEEF46DB4855E25702F8237E8f403FddcaF931C0",
                "0x70a08231000000000000000000000000e16359506c028e51f16be38986ec5746251e9724"
            );

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }

        [TestMethod()]
        public void GetCodeTest()
        {
            var result = client.GetCode(
                "0x0e09fabb73bd3ade0a17ecc321fd13a19e81ce82",
                BscBlockTag.Latest
            );

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }

        [TestMethod()]
        public void GetStorageAtTest()
        {
            var result = client.GetStorageAt(
                "0x0e09fabb73bd3ade0a17ecc321fd13a19e81ce82",
                "0x0",
                BscBlockTag.Latest
            );

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }

        [TestMethod()]
        public void GasPriceTest()
        {
            var result = client.GasPrice();

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }

        [TestMethod()]
        public void EstimateGasTest()
        {
            var result = client.EstimateGas(
                "0x4e71d92d",
                "0xEeee7341f206302f2216e39D715B96D8C6901A1C",
                "0xff22",
                "0x51da038cc",
                "0x5f5e0ff"
            );

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }


        #endregion

        #region IBscTokens

        [TestMethod()]
        public void GetBEP20TotalSupplyTest()
        {
            var result = client.GetBEP20TotalSupply("0xe9e7cea3dedca5984780bafc599bd69add087d56");

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }

        [TestMethod()]
        public void GetBEP20CirculatingSupplyTest()
        {
            var result = client.GetBEP20CirculatingSupply("0xe9e7cea3dedca5984780bafc599bd69add087d56");

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }

        [TestMethod()]
        public void GetBEP20BalanceOfTest()
        {
            var result = client.GetBEP20BalanceOf("0x89e73303049ee32919903c09e8de5629b84f59eb", "0xe9e7cea3dedca5984780bafc599bd69add087d56");

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }

        #endregion

        #region IBscGasTracker

        [TestMethod()]
        public void GetGasOracleTest()
        {
            var result = client.GetGasOracle();

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }

        #endregion

        #region IBscStats

        [TestMethod()]
        public void GetBNBTotalSupplyTest()
        {
            var result = client.GetBNBTotalSupply();

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }

        [TestMethod()]
        public void GetBscValidatorListTest()
        {
            var result = client.GetBscValidatorList();

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }

        [TestMethod()]
        public void GetBNBLastPriceTest()
        {
            var result = client.GetBNBLastPrice();

            Assert.IsTrue(result.Status == BscscanJsonStatus.Success);
        }

        #endregion

    }
}