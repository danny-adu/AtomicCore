using Microsoft.VisualStudio.TestTools.UnitTesting;
using AtomicCore.Integration.ClickHouseDbProvider;
using AtomicCore.Integration.ClickHouseDbProviderUnitTest;

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
            var insResult = BizClickHouseDbRepository.Member_UserBasics.Insert(new Member_UserBasics()
            {
                UserID = 2,
                UserName = "danny",
                UserAge = 21,
                UserCreateAt = DateTime.Now,
                UserIsBlock = false
            });

            Assert.IsTrue(insResult.IsAvailable());
        }

        [TestMethod()]
        public void InsertAsyncTest()
        {
            var insResult = BizClickHouseDbRepository.Member_UserBasics.InsertAsync(new Member_UserBasics()
            {
                UserID = 3,
                UserName = "mary",
                UserAge = 25,
                UserCreateAt = DateTime.Now,
                UserIsBlock = false
            }).Result;

            Assert.IsTrue(insResult.IsAvailable());
        }

        [TestMethod()]
        public void InsertBatchTest()
        {
            var list = new List<Member_UserBasics>()
            {
                new Member_UserBasics()
                {
                    UserID = 4,
                    UserName = "t4",
                    UserAge = 25,
                    UserCreateAt = DateTime.Now,
                    UserIsBlock = false
                },
                new Member_UserBasics()
                {
                    UserID = 5,
                    UserName = "t5",
                    UserAge = 25,
                    UserCreateAt = DateTime.Now,
                    UserIsBlock = false
                },
                new Member_UserBasics()
                {
                    UserID = 6,
                    UserName = "t6",
                    UserAge = 25,
                    UserCreateAt = DateTime.Now,
                    UserIsBlock = false
                }
            };

            var insResult = BizClickHouseDbRepository.Member_UserBasics.InsertBatch(list);

            Assert.IsTrue(insResult.IsAvailable());
        }

        [TestMethod()]
        public void InsertBatchAsyncTest()
        {
            var list = new List<Member_UserBasics>()
            {
                new Member_UserBasics()
                {
                    UserID = 7,
                    UserName = "t7",
                    UserAge = 25,
                    UserCreateAt = DateTime.Now,
                    UserIsBlock = false
                },
                new Member_UserBasics()
                {
                    UserID = 8,
                    UserName = "t8",
                    UserAge = 25,
                    UserCreateAt = DateTime.Now,
                    UserIsBlock = false
                },
                new Member_UserBasics()
                {
                    UserID = 9,
                    UserName = "t9",
                    UserAge = 25,
                    UserCreateAt = DateTime.Now,
                    UserIsBlock = false
                }
            };

            var insResult = BizClickHouseDbRepository.Member_UserBasics.InsertBatchAsync(list).Result;

            Assert.IsTrue(insResult.IsAvailable());
        }

        [TestMethod()]
        public void UpdateTest()
        {
            var result = BizClickHouseDbRepository.Member_UserBasics.Update(d => d.UserID == 1, up => new Member_UserBasics()
            {
                UserIsBlock = true
            });

            Assert.IsTrue(result.IsAvailable());
        }

        [TestMethod()]
        public void DeleteTest()
        {
            var result = BizClickHouseDbRepository.Member_UserBasics.Delete(d => d.UserID == 1);

            Assert.IsTrue(result.IsAvailable());
        }

        [TestMethod()]
        public void FetchTest()
        {
            var result = BizClickHouseDbRepository.Member_UserBasics.Fetch(o => o.Where(d => d.UserID == 1));

            Assert.IsTrue(result.IsAvailable());
        }
    }
}