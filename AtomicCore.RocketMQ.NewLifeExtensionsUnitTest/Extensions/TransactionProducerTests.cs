using Microsoft.VisualStudio.TestTools.UnitTesting;
using NewLife.RocketMQ;
using NewLife.RocketMQ.Protocol;
using System.Text;

namespace NewLife.RocketMQ.Tests
{
    [TestClass()]
    public class TransactionProducerTests
    {
        const string rocketServer = "192.168.1.95:9876";
        const string topic = "order_test";

        [TestMethod()]
        public void PublishWithTransactionTest()
        {
            // 创建生产者实例
            var producer = new NewLife.RocketMQ.TransactionProducer
            {
                // 指定 NameServer 地址
                NameServerAddress = rocketServer,

                // 设置生成者所属的组名
                Group = "producer_group",

                //// 设置要订阅的 Topic
                Topic = topic,


            };

            // 启动生产者
            producer.Start();

            // 发送事务消息
            var message = new Message
            {
                Body = Encoding.UTF8.GetBytes($"Test transactional message => {DateTime.UtcNow.Ticks}")
            };
            var result = producer.PublishWithTransaction(message, () =>
            {
                // 执行本地事务
                return true; // 根据本地事务的结果返回 true 或 false
            });

            Assert.IsTrue(result.MsgId != null);
        }

        [TestMethod()]
        public void CommitTransactionTest()
        {
            // 创建生产者实例
            var producer = new NewLife.RocketMQ.TransactionProducer
            {
                // 指定 NameServer 地址
                NameServerAddress = rocketServer,

                // 设置生成者所属的组名
                Group = "producer_group",

                //// 设置要订阅的 Topic
                Topic = topic,


            };

            // 启动生产者
            producer.Start();

            producer.CommitTransaction("AC1800030018723279CF1144938D000A");

            Assert.Fail();
        }
    }
}