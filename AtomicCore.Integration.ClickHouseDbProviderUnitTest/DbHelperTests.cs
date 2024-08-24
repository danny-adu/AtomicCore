namespace AtomicCore.Integration.ClickHouseDbProviderUnitTest
{
    [TestClass()]
    public class DbHelperTests
    {
        [TestMethod()]
        public void GetDbTablesTest()
        {
            var tbs = DbHelper.GetDbTables(T4Config.global_ConnStr, T4Config.global_DbName);

            Assert.IsTrue(null != tbs);
        }
    }
}
