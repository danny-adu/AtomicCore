using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AtomicCore.Tests
{
    [TestClass()]
    public class UUIDHandlerTests
    {
        [TestMethod()]
        public void GuidTo16StringTest()
        {
            string uuid = UUIDHandler.GuidTo16String();

            Assert.IsTrue(uuid.Length == 16);
        }

        [TestMethod()]
        public void GuidTo32StringTest()
        {
            string uuid = UUIDHandler.GuidTo32String();

            Assert.IsTrue(uuid.Length == 32);
        }

        [TestMethod()]
        public void GuidToLongIDTest()
        {
            long uuid = UUIDHandler.GuidToLongID();

            Assert.IsTrue(uuid > 0L);
        }

        [TestMethod()]
        public void GenerateUniqueIDTest()
        {
            string uuid = UUIDHandler.GenerateUniqueID();

            Assert.IsTrue(uuid.Length == 22);
        }
    }
}