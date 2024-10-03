using AtomicCore.Rocketmq.ClientUnitTest;
using System.Text;

namespace Org.Apache.Rocketmq.Tests
{
    [TestClass()]
    public class ProducerTests
    {
        private const string c_topices = "test";

        [TestMethod()]
        public void BeginTransactionTest()
        {
            /* 配置 NameServer 等参数 */
            var config = new ClientConfig.Builder()
                .SetEndpoints("127.0.0.1:9876")
                .Build();

            // 实例化my transaction checker
            var transactionChecker = new MyTransactionChecker();
            var producer = new Producer.Builder()
                .SetClientConfig(config)
                .SetTopics(c_topices)
                .SetTransactionChecker(transactionChecker)
                .Build()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            // 开启事务
            var transaction = producer.BeginTransaction();

            // 创建消息
            var message = new Message.Builder()
                .SetTopic(c_topices)
                .SetTag("TagA")
                .SetBody(Encoding.UTF8.GetBytes("事务消息内容"))
                .Build();

            try
            {
                // 发送事务消息
                var sendReceipt = producer.Send(message, transaction)
                    .ConfigureAwait(false).GetAwaiter().GetResult();

                // 执行本地事务逻辑
                bool transactionSuccess = ExecuteLocalTransaction();

                // 根据本地事务结果提交或回滚消息
                if (transactionSuccess)
                {
                    transaction.Commit().ConfigureAwait(false).GetAwaiter().GetResult();
                }
                else
                {
                    transaction.Rollback().ConfigureAwait(false).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("事务消息发送或处理失败: " + ex.Message);
                transaction.Rollback().ConfigureAwait(false).GetAwaiter().GetResult();
            }

            Assert.Fail();
        }

        private bool ExecuteLocalTransaction()
        {
            // 模拟本地事务处理，返回 true 表示成功，false 表示失败
            return true;
        }
    }
}