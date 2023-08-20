using AtomicCore.Integration.MssqlDbProviderUnitTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AtomicCore.Integration.MssqlDbProvider.Tests
{
    [TestClass()]
    public class Mssql2008DbProcedurerTests
    {
        public Mssql2008DbProcedurerTests()
        {
            AtomicKernel.Initialize();
        }

        [TestMethod()]
        public void ExecuteAsyncTest()
        {
            var result = BizDbProcedures.SP_GetFirstPYAsync("你好").Result;

            Assert.IsTrue(result.IsAvailable());
        }
    }
}