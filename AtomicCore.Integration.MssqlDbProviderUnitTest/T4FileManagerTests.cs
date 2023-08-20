using AtomicCore.Integration.MssqlDbProviderUnitTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AtomicCore.Integration.MssqlDbProvider.Tests
{
    [TestClass()]
    public class T4FileManagerTests
    {
        public T4FileManagerTests()
        {
            AtomicKernel.Initialize();
        }

        [TestMethod()]
        public void GenerateORMEntityTest()
        {
            T4FileManager.GenerateORMEntity("D:\\GitHub_Pros\\AtomicCore\\AtomicCore.Integration.MssqlDbProviderUnitTest\\DataBase\\T4\\MssqlT4.tt");

            Assert.IsTrue(true);
        }
    }
}
