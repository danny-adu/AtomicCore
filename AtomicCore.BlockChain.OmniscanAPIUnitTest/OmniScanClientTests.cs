using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AtomicCore.BlockChain.OmniscanAPI.Tests
{
    [TestClass()]
    public class OmniScanClientTests
    {
        private const string AGENT_GET = "http://agent.intoken.club/Remote/Get?url={0}";
        private const string AGENT_POST = "http://agent.intoken.club/Remote/Post?url={0}&contentType={1}";

        private readonly IOmniScanClient client;

        public OmniScanClientTests()
        {
            client = new OmniScanClient(AGENT_GET, AGENT_POST);
        }

        [TestMethod()]
        public void GetAddressBTCTest()
        {
            var result = client.GetAddressBTC("19dENFt4wVwos6xtgwStA6n8bbA57WCS58");

            Assert.IsTrue(null != result);
        }

        [TestMethod()]
        public void GetAddressV1Test()
        {
            var result = client.GetAddressV1("1LifmNfXAeMYnyFgySGyQpnJBSguWcuJdW");

            Assert.IsTrue(null != result);
        }

        [TestMethod()]
        public void GetAddressV2Test()
        {
            var result = client.GetAddressV2(new string[] { "1KYiKJEfdJtap9QX2v9BXJMpz2SfU4pgZw", "1FoWyxwPXuj4C6abqwhjDWdz6D4PZgYRjA" });

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public void GetAddressDetailsTest()
        {
            var result = client.GetAddressDetails("1KYiKJEfdJtap9QX2v9BXJMpz2SfU4pgZw");

            Assert.IsTrue(!string.IsNullOrEmpty(result.Address));
        }

        [TestMethod()]
        public void DesignatingCurrenciesTest()
        {
            var result = client.DesignatingCurrencies(1);

            Assert.IsTrue(null != result);
        }

        [TestMethod()]
        public void GetHistoryTest()
        {
            var result = client.GetHistory(3, 0);

            Assert.IsTrue(null != result);
        }

        [TestMethod()]
        public void ListByOwnerTest()
        {
            var result = client.ListByOwner(new string[] { "1ARjWDkZ7kT9fwjPrjcQyvbXDkEySzKHwu" });

            Assert.IsTrue(null != result);
        }

        [TestMethod()]
        public void ListActiveCrowdSalesTest()
        {
            var result = client.ListActiveCrowdSales(1);

            Assert.IsTrue(null != result);
        }

        [TestMethod()]
        public void ListbyecosystemTest()
        {
            var result = client.ListByEcosystem(1);

            Assert.IsTrue(null != result);
        }

        [TestMethod()]
        public void PropertyListTest()
        {
            var result = client.PropertyList();

            Assert.IsTrue(null != result);
        }

        [TestMethod()]
        public void SearchAddressTest()
        {
            var result = client.Search("1ARjWDkZ7kT9fwjPrjcQyvbXDkEySzKHwu");

            Assert.IsTrue(null != result);
        }

        [TestMethod()]
        public void SearchAssetTest()
        {
            var result = client.Search("3");

            Assert.IsTrue(null != result);
        }

        [TestMethod()]
        public void SearchTxTest()
        {
            var result = client.Search("ccc0c74b4875b20b3c00409520142cc24865d6d5aba355c8f2f21de7f2b65fa4");

            Assert.IsTrue(null != result);
        }

        [TestMethod()]
        public void GetTxListTest()
        {
            //3A1YfC2VeYZjVyavBAKhbjuc8odurW9yzQ
            //1DeRF2bLsSFRjsFBuyhfjJ9BE5PD9Uy7gL

            var result = client.GetTxList("1EXoDusjGwvnjZUyKkxZ4UHEf77z6A5S4P");

            Assert.IsTrue(null != result);
        }

        [TestMethod()]
        public void PushTxTest()
        {
            var result = client.PushTx("01000000010b7ad3946c6fcd15d39fd4312c4f8dfd68c51a61071c2575cfb0722c4403eef1000000008a47304402206fe4eabf81cc4c6c3cee3dedc6ea9be46bb685c1b61da577bb9aead8188b80b702201404284c52ad2e857bd3931fb0e1646575d1e72c93b2cdcd80a7e0c7bee1c191014104ad90e5b6bc86b3ec7fac2c5fbda7423fc8ef0d58df594c773fa05e2c281b2bfe877677c668bd13603944e34f4818ee03cadd81a88542b8b4d5431264180e2c28ffffffff0470170000000000001976a9143b0000000000000001000000009d828d9600000088ac70170000000000001976a9143c791cc99255509d85788e2bf0e0f6e8b389b3cf88ac40355d5f000000001976a914946cb2e08075bcbaf157e47bcb67eb2b2339d24288ac70170000000000001976a914946cb2e08075bcbaf157e47bcb67eb2b2339d24288ac00000000");

            Assert.IsTrue(null != result);
        }

        [TestMethod()]
        public void GetTxTest()
        {
            var result = client.GetTx("7483abeff1149a9d4da07fd0e33ff55102e4327db23da3c76ba6877bd2112d51");

            Assert.IsTrue(null != result);
        }
    }
}