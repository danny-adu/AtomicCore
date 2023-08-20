using Microsoft.VisualStudio.TestTools.UnitTesting;
using AtomicCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtomicCore.Tests
{
    [TestClass()]
    public class HttpProtocolTests
    {
        /// <summary>
        /// https://api-goerli.etherscan.io/api?module=account&action=balance&address=0x29aAe16abfDC4C6F119E86D09ab8603D491c5d5F&tag=latest&apikey=GCA37TEKH4D8CPKBG2F2NE6VQD129A5T1H
        /// </summary>
        [TestMethod()]
        public void HttpGetByIPTest()
        {
            Assert.Fail();
        }
    }
}