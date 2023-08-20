using AtomicCore.DbProvider;
using AtomicCore.Integration.MssqlDbProvider;
using AtomicCore.Integration.MssqlDbProviderUnitTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;

namespace AtomicCore.Integration.MssqlDbProvider.Tests
{
    [TestClass()]
    public class Mssql2008DbProviderTests
    {
        /// <summary>
        /// 001分表
        /// </summary>
        private const string c_suffix_001 = "_001";
        /// <summary>
        /// 002分表
        /// </summary>
        private const string c_suffix_002 = "_002";
        /// <summary>
        /// 003分表
        /// </summary>
        private const string c_suffix_003 = "_003";

        public Mssql2008DbProviderTests()
        {
            AtomicKernel.Initialize();
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        [TestMethod()]
        public void InsertAsyncTest()
        {
            var ins1Result = BizDbRepository.Topic_QQS.InsertAsync(new Topic_QQS()
            {
                Name = "qq_001",
                QQ = "qq_000001",
                Sex = 1,
                IsDel = 0
            }, c_suffix_001).Result;

            var ins2Result = BizDbRepository.Topic_QQS.InsertAsync(new Topic_QQS()
            {
                Name = "qq_002",
                QQ = "qq_000002",
                Sex = 2,
                IsDel = 0
            }, c_suffix_002).Result;

            var ins3Result = BizDbRepository.Topic_QQS.InsertAsync(new Topic_QQS()
            {
                Name = "qq_003",
                QQ = "qq_000003",
                Sex = 3,
                IsDel = 0
            }, c_suffix_003).Result;

            Assert.IsTrue(ins1Result.IsAvailable() && ins2Result.IsAvailable() && ins3Result.IsAvailable());
        }

        /// <summary>
        /// 批量参入数据
        /// </summary>
        [TestMethod()]
        public void InsertBatchAsyncTest()
        {
            var ins1Result = BizDbRepository.Topic_QQS.InsertBatchAsync(new List<Topic_QQS>()
            {
                new Topic_QQS()
                {
                    Name = "qq_011",
                    QQ = "qq_000011",
                    Sex = 1,
                    IsDel = 0
                },
                new Topic_QQS()
                {
                    Name = "qq_111",
                    QQ = "qq_000111",
                    Sex = 1,
                    IsDel = 0
                }
            },
            c_suffix_001).Result;

            Assert.IsTrue(ins1Result.IsAvailable());
        }

        /// <summary>
        /// 更新操作（指定仅更新部分字段）
        /// </summary>
        [TestMethod()]
        public void UpdateAsyncTest()
        {
            var result = BizDbRepository.Topic_QQS.UpdateAsync(d =>
                d.ID == 1,
            up => new Topic_QQS()
            {
                Sex = 30
            }
            , c_suffix_003)
            .Result;

            Assert.IsTrue(result.IsAvailable());
        }

        /// <summary>
        /// 先获取再更新整个实体
        /// </summary>
        [TestMethod()]
        public void FetchAndUpdateAsyncTest()
        {
            var getResult = BizDbRepository.Topic_QQS.FetchAsync(d => d.Where(d => d.ID == 1), c_suffix_003).Result;
            if (!getResult.IsAvailable())
            {
                Assert.Fail();
                return;
            }
            if (null == getResult.Record)
            {
                Assert.Fail();
                return;
            }

            // 修改字段
            getResult.Record.Sex = getResult.Record.Sex - 1;

            var upResult = BizDbRepository.Topic_QQS.UpdateAsync(d =>
                d.ID == 1,
                getResult.Record,
                c_suffix_003
            ).Result;
            Assert.IsTrue(upResult.IsAvailable());
        }

        /// <summary>
        /// 通过任务调度模式
        /// </summary>
        [TestMethod()]
        public void UpdateTaskTest()
        {
            // 获取列表数据
            var listResult = BizDbRepository.Topic_QQS.FetchListAsync(d => d.Where(d => d.ID > 0), c_suffix_001).Result;
            if (!listResult.IsAvailable())
            {
                Assert.Fail();
                return;
            }
            if (null == listResult.Record)
            {
                Assert.Fail();
                return;
            }

            // 构建批量更新任务数据
            var taskList = new List<DbUpdateTaskData<Topic_QQS>>()
            {
                new DbUpdateTaskData<Topic_QQS>()
                {
                    WhereExp = d => d.ID == 2,
                    UpdatePropertys = up=>new Topic_QQS()
                    {
                        Sex = up.Sex + 1
                    }
                },
                new DbUpdateTaskData<Topic_QQS>()
                {
                    WhereExp = d => d.ID == 3,
                    UpdatePropertys = up=>new Topic_QQS()
                    {
                        Sex = up.Sex + 1
                    }
                }
            };

            var upResult = BizDbRepository.Topic_QQS.UpdateTaskAsync(taskList, false, c_suffix_001).Result;

            Assert.IsTrue(upResult.IsAvailable());
        }

        [TestMethod()]
        public void DeleteTest()
        {
            var delResult = BizDbRepository.Topic_QQS.DeleteAsync(d => d.ID > 0, c_suffix_001).Result;

            Assert.IsTrue(delResult.IsAvailable());
        }

        [TestMethod()]
        public void FetchAsyncTest()
        {
            var result = BizDbRepository.Topic_QQS.FetchAsync(o => o
                .Where(d => d.ID > 0), c_suffix_003).Result;

            Assert.IsTrue(result.IsAvailable());
        }

        /// <summary>
        /// 同步查询数据库列表数据
        /// </summary>
        [TestMethod()]
        public void FetchListTest()
        {
            var result = BizDbRepository.Topic_QQS.FetchList(o => o
                .Pager(1, int.MaxValue)
                .Where(d =>
                    d.ID > 0
                )
                .OrderBy(d => d.ID)
            );
            if (!result.IsAvailable())
            {
                Assert.Fail();
                return;
            }

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void FetchListAsyncTest()
        {
            var result = BizDbRepository.Topic_QQS.FetchListAsync(o => o
                .Pager(1, int.MaxValue)
                .Where(d =>
                    d.ID > 0
                )
                .OrderBy(d => d.ID)
                , c_suffix_003
            ).Result;
            if (!result.IsAvailable())
            {
                Assert.Fail();
                return;
            }

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void CalculateTest()
        {
            var result = BizDbRepository.Topic_QQS.Calculate(o => o
                .Count(d => 1, null)
                .Count(d => 1, null)
                .Sum(d => d.ID, null)
                .Where(d =>
                    d.ID > 0
                ),
                c_suffix_003
            );

            if (!result.IsAvailable())
            {
                Assert.Fail();
                return;
            }

            Assert.IsTrue(true);
        }
    }
}