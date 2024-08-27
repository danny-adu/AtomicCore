using Microsoft.VisualStudio.TestTools.UnitTesting;
using AtomicCore.Integration.ClickHouseDbProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}