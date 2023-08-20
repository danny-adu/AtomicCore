using AtomicCore.DbProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;

namespace AtomicCore.Integration.MysqlDbProviderUnitTest
{
    [TestClass]
    public class MysqlDbProviderTests
    {
        #region Constructors

        public MysqlDbProviderTests()
        {
            AtomicKernel.Initialize();
        }

        #endregion

        #region 测试T4模版

        /// <summary>
        /// 数据库初始化
        /// </summary>
        [TestMethod]
        public void DbInit()
        {
            //初始化数据库
            DbHelper.CreateDatebase(T4Config.global_ConnStr);

            //创建表
            //创建数据库实例，指定文件位置 
            MySqlCommand cmdCreateTable = null;
            using (MySqlConnection conn = new MySqlConnection(T4Config.global_ConnStr))
            {
                conn.Open();

                //测试主表
                string Topic_QQS = "CREATE TABLE IF NOT EXISTS `Topic_QQS`(`id` int(11) not null primary key,`qq` varchar(50) not null ,`text` varchar(50) not null ,`isdel` bit not null)ENGINE=InnoDB DEFAULT CHARSET=utf8;";
                cmdCreateTable = new MySqlCommand(Topic_QQS, conn);
                cmdCreateTable.ExecuteNonQuery();//如果表不存在，创建数据表

                //测试从表
                string Topic_QQS_Ext = "CREATE TABLE IF NOT EXISTS `Topic_QQS_Ext`(`qq` varchar(50) not null primary key,`name` varchar(50) not null ,`sex` int(11) not null)ENGINE=InnoDB DEFAULT CHARSET=utf8;";
                cmdCreateTable = new MySqlCommand(Topic_QQS_Ext, conn);
                cmdCreateTable.ExecuteNonQuery();//如果表不存在，创建数据表
            }
        }

        [TestMethod]
        public void Database()
        {
            T4FileManager.GenerateORMEntity(@"D:\SVN_Pros\Atomic-Master-2.0\AtomicCore.Integration.MysqlDbProviderUnitTest\DataBase\T4\MssqlT4.tt");
        }

        #endregion

        #region Mysql基础功能测试

        [TestMethod]
        public void TestCreateMethod()
        {
            DbSingleRecord<Topic_QQS> insertResult = BizDbRepository.Topic_QQS.Insert(new Topic_QQS()
            {
                qq = "10001",
                text = "test",
                isdel = true
            });

            Assert.IsTrue(insertResult.IsAvailable());
        }

        /// <summary>
        /// 更新指定部分字段
        /// </summary>
        [TestMethod]
        public void TestUpdateSinglePartial()
        {
            DbNonRecord upResult = BizDbRepository.Topic_QQS.Update(d =>
                d.qq == "10001",
            up => new Topic_QQS()
            {
                text = "update partial success"
            });

            Assert.IsTrue(upResult.IsAvailable());
        }

        /// <summary>
        /// 更新模型的所有字段
        /// </summary>
        [TestMethod]
        public void TestUpdateSingleFull()
        {
            DbSingleRecord<Topic_QQS> getResult = BizDbRepository.Topic_QQS.Fetch(o => o.Where(d => d.qq == "10001").OrderByDescending(d => d.id));
            if (!getResult.IsAvailable())
            {
                Assert.Fail(getResult.Errors.First());
                return;
            }
            if (null == getResult.Record)
            {
                Assert.Fail("数据不存在,无法进行如下测试");
                return;
            }

            Topic_QQS model = getResult.Record;
            model.text = "update full success";

            DbNonRecord upResult = BizDbRepository.Topic_QQS.Update(d => d.qq == "10001", model);

            Assert.IsTrue(upResult.IsAvailable());
        }

        /// <summary>
        /// 批量更新任务
        /// </summary>
        [TestMethod]
        public void TestUpdateTask()
        {
            List<DbUpdateTaskData<Topic_QQS>> taskList = new List<DbUpdateTaskData<Topic_QQS>>();
            taskList.Add(new DbUpdateTaskData<Topic_QQS>()
            {
                WhereExp = d => d.qq == "171331668",
                UpdatePropertys = up => new Topic_QQS()
                {
                    text = "task update 171331668",
                    isdel = true
                }
            });
            taskList.Add(new DbUpdateTaskData<Topic_QQS>()
            {
                WhereExp = d => d.qq == "442543251",
                UpdatePropertys = up => new Topic_QQS()
                {
                    text = "task update 442543251",
                    isdel = true
                }
            });

            DbNonRecord result = BizDbRepository.Topic_QQS.UpdateTask(taskList, true);

            Assert.IsTrue(result.IsAvailable());
        }

        /// <summary>
        /// 删除操作
        /// </summary>
        [TestMethod]
        public void TestDelete()
        {
            DbNonRecord delResult = BizDbRepository.Topic_QQS.Delete(d => d.qq == "10001");
            if (!delResult.IsAvailable())
                Assert.Fail();
            else
                Assert.IsTrue(true);
        }

        [TestMethod()]
        public void FetchTest()
        {
            DbSingleRecord<Topic_QQS> result = BizDbRepository.Topic_QQS.Fetch(o => o
                .Where(d => d.isdel)
                .OrderBy(d => d.qq)
            );
            if (!result.IsAvailable())
                Assert.Fail();
            else
                Assert.IsTrue(true);
        }

        [TestMethod()]
        public void FetchListTest()
        {
            DbCollectionRecord<Topic_QQS> result = BizDbRepository.Topic_QQS.FetchList(o => o
                .Pager(1, int.MaxValue)
                .Where(d => !d.isdel && d.id > 0)
                .OrderBy(d => d.id)
            );
            if (!result.IsAvailable())
                Assert.Fail();
            else
                Assert.IsTrue(true);
        }

        [TestMethod()]
        public void CalculateTest()
        {
            DbCalculateRecord result = BizDbRepository.Topic_QQS.Calculate(o => o
                .Select(d => new Topic_QQS() { id = d.id })
                .Sum(d => d.id, "total")
                .Where(d => d.id > 0)
                .GroupBy(d => d.id)
            );
            if (!result.IsAvailable())
                Assert.Fail();
            else
                Assert.IsTrue(true);
        }

        [TestMethod()]
        public void SqlExt_Contains()
        {
            DbCollectionRecord<Topic_QQS> result = BizDbRepository.Topic_QQS.FetchList(o => o
                .Where(d =>
                    d.id > 0 &&
                    d.text.Contains("171331668")
                )
                .OrderBy(d => d.id)
            );
            if (!result.IsAvailable())
                Assert.Fail();
            else
                Assert.IsTrue(true);
        }

        [TestMethod()]
        public void SqlExt_DbIn()
        {
            string caseins = "1,2,3,4,5,6,7,8,9,10";
            DbCollectionRecord<Topic_QQS> result = BizDbRepository.Topic_QQS.FetchList(o => o
                .Where(d =>
                    d.id > 0 &&
                    d.SqlIn(d.id, caseins)
                )
            );
            if (!result.IsAvailable())
                Assert.Fail();
            else
                Assert.IsTrue(true);
        }

        [TestMethod()]
        public void SqlExt_DbNotIn()
        {
            string caseins = "1,2";
            DbCollectionRecord<Topic_QQS> result = BizDbRepository.Topic_QQS.FetchList(o => o
                .Where(d =>
                    d.id > 0 &&
                    d.SqlNotIn(d.id, caseins)
                )
            );
            if (!result.IsAvailable())
                Assert.Fail();
            else
                Assert.IsTrue(true);
        }

        [TestMethod()]
        public void SqlExt_DbSubQuery()
        {
            DbCollectionRecord<Topic_QQS> result = BizDbRepository.Topic_QQS.FetchList(o => o
                .Where(d =>
                    !d.isdel &&
                    d.id > 0 &&
                    d.SqlSubQuery<Topic_QQS_Ext>(child =>
                        child.qq == d.qq &&
                        child.name == d.text,
                        child =>
                            child.sex > 0 &&
                            child.name != "" &&
                            d.id > 0
                        , true
                    )
                )
            );
            if (!result.IsAvailable())
                Assert.Fail();
            else
                Assert.IsTrue(true);
        }

        #endregion
    }
}
