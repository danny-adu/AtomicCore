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

        [TestMethod()]
        public void GetDbViewsTest()
        {
            var tbs = DbHelper.GetDbViews(T4Config.global_ConnStr, T4Config.global_DbName);

            Assert.IsTrue(null != tbs);
        }

        [TestMethod()]
        public void GetDbColumnsTest()
        {
            var cols = DbHelper.GetDbColumns(T4Config.global_ConnStr, T4Config.global_DbName, "Member_UserBasics");

            Assert.IsTrue(null != cols);
        }
    }
}
