using AtomicCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AtomicCore.Tests
{
    [TestClass()]
    public class UrlEncoderTests
    {
        [TestMethod()]
        public void UrlEncodeTest()
        {
            var str = UrlEncoder.UrlEncode("https://api-goerli.etherscan.io/api?module=account&action=balance&apikey=N4KR7R89K78AWKPBZY6WD27DDTTDB8YJ8W&address=0x29aAe16abfDC4C6F119E86D09ab8603D491c5d5F&tag=latest");

            Assert.IsTrue(!str.Contains("&"));
        }

        [TestMethod()]
        public void UrlDecodeTest()
        {
            var str = UrlEncoder.UrlEncode("https://api-goerli.etherscan.io/api?module=account&action=balance&apikey=N4KR7R89K78AWKPBZY6WD27DDTTDB8YJ8W&address=0x29aAe16abfDC4C6F119E86D09ab8603D491c5d5F&tag=latest");

            string newStr = UrlEncoder.UrlDecode(str);

            Assert.IsTrue(newStr.Contains("&"));
        }
    }
}