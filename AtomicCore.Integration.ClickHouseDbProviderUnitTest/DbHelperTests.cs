namespace AtomicCore.Integration.ClickHouseDbProviderUnitTest
{
    [TestClass()]
    public class DbHelperTests
    {
        #region DbHelper

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

        #endregion

        #region T4FileManager

        [TestMethod()]
        public void GenerateORMEntityTest()
        {
            string rootPath = AppDomain.CurrentDomain.BaseDirectory;
            int rm_idx = rootPath.LastIndexOf("bin");
            rootPath = rootPath.Substring(0, rm_idx);
            var tt = $"{rootPath}DataBase\\T4\\MssqlT4.tt";

            T4FileManager.GenerateORMEntity(tt);

            Assert.IsTrue(true);
        }

        #endregion
    }
}
