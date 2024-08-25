using Microsoft.VisualStudio.TestTools.UnitTesting;
using AtomicCore.Integration.ClickHouseDbProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtomicCore.Integration.ClickHouseDbProvider.Tests
{
    [TestClass()]
    public class ClickHouseDbProviderTests
    {
        public ClickHouseDbProviderTests()
        {
            AtomicCore.AtomicKernel.Initialize();
        }

        [TestMethod()]
        public void InsertTest()
        {


            Assert.Fail();
        }
    }
}