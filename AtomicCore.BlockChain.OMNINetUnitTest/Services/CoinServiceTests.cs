using Microsoft.VisualStudio.TestTools.UnitTesting;
using AtomicCore.BlockChain.OMNINet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtomicCore.BlockChain.OMNINet.Tests
{
    [TestClass()]
    public class CoinServiceTests
    {
        [TestMethod()]
        public void SendRawTransactionTest()
        {
            IOMNICoinService service = new OMNICoinService();
            string txid = service.OMNISendRawTransaction("02000000018ed43d7449da13967ce1c681c3394d8ec7338e66f167cdb9b968267649994765010000006b483045022100ef49428e2577c18989695f4bb3fecd98e50698c70cdb3f75ce39a0b76105339c02204db3d5ff5c7df0cddb8a465a287d11243b7d22a8cb68b2f7516c7913be357133012102a34253f8f51004431acdbe3dc9bbc254d00385db7bd6e80db6b0be1f5394d4ceffffffff02d00700000000000017a91422e11bdd6cfb8d599cea5b6a658e617570f574a8876ef96500000000001976a914f4e117d7c7ad8a66e00a4559c005d1015bece01388ac00000000");

            Assert.IsTrue(!string.IsNullOrEmpty(txid));
        }

        /// <summary>
        /// 正式网的交易
        /// </summary>
        [TestMethod()]
        public void GetRawTransactionTest()
        {
            IOMNICoinService service = new OMNICoinService("http://btc.intoken.club", "adu", "xoNXu1WlPYU6hHTW", "xoNXu1WlPYU6hHTW");

            var result = service.GetRawTransaction("29a0331fa42ae86b17622c987be0222df1c83b94712d885c84ca9106ac349c66", 1);

            Assert.IsTrue(result.VSize > 0);
        }
    }
}