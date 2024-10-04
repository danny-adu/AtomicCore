using NewLife.RocketMQ.Protocol;
using System;
using System.Threading;

namespace NewLife.RocketMQ
{
    /// <summary>
    /// transaction producer impl
    /// </summary>
    public class TransactionProducer : Producer
    {
        /// <summary>
        /// 发送事务消息（半消息）
        /// </summary>
        /// <param name="message"></param>
        /// <param name="localTransaction"></param>
        /// <returns></returns>
        public SendResult PublishWithTransaction(Message message, Func<bool> localTransaction)
        {
            // 构造事务消息的请求头
            var header = CreateTransactionHeader(message);

            for (var i = 0; i <= RetryTimesWhenSendFailed; i++)
            {
                // 性能埋点
                using var span = Tracer?.NewSpan($"mq:{Name}:PublishWithTransaction", message.BodyString);
                try
                {
                    // 选择队列并获取Broker客户端
                    var mq = SelectQueue();
                    mq.Topic = Topic;
                    header.QueueId = mq.QueueId;

                    var bk = GetBroker(mq.BrokerName);

                    // 发送半消息
                    var rs = bk.Invoke(RequestCode.SEND_MESSAGE_V2, message.Body, header.GetProperties(), true);

                    // 从返回的 Header 中获取 TransactionId
                    var transactionId = rs.Header.ExtFields.TryGetValue("transactionId", out var transId) ? transId : null;

                    if (string.IsNullOrEmpty(transactionId))
                        throw new Exception("Transaction ID is missing in the response.");

                    // 执行本地事务
                    var success = localTransaction();

                    // 提交或回滚事务
                    if (success)
                    {
                        CommitTransaction(transactionId);
                    }
                    else
                    {
                        RollbackTransaction(transactionId);
                    }

                    return new SendResult
                    {
                        Queue = mq,
                        Header = rs.Header,
                        Status = success ? SendStatus.SendOK : SendStatus.SendError
                    };
                }
                catch (Exception ex)
                {
                    // 如果网络异常，则延迟重发
                    if (i < RetryTimesWhenSendFailed)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    span?.SetError(ex, message);
                    throw;
                }
            }

            return null;
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        /// <param name="transactionId"></param>
        private void CommitTransaction(string transactionId)
        {
            var header = new EndTransactionRequestHeader
            {
                TransactionId = transactionId,
                CommitOrRollback = 0  // 0表示提交
            };

            // 向Broker发送提交事务的请求
            var bk = GetBroker("broker-name");  // 使用实际的broker名称
            bk.Invoke(RequestCode.END_TRANSACTION, null, header.GetProperties(), false);
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        /// <param name="transactionId"></param>
        private void RollbackTransaction(string transactionId)
        {
            var header = new EndTransactionRequestHeader
            {
                TransactionId = transactionId,
                CommitOrRollback = 1  // 1表示回滚
            };

            // 向Broker发送回滚事务的请求
            var bk = GetBroker("broker-name");  // 使用实际的broker名称
            bk.Invoke(RequestCode.END_TRANSACTION, null, header.GetProperties(), false);
        }

        /// <summary>
        /// 创建事务消息的请求头
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private SendMessageRequestHeader CreateTransactionHeader(Message message)
        {
            return new SendMessageRequestHeader
            {
                ProducerGroup = Group,
                Topic = Topic,
                SysFlag = 0,
                BornTimestamp = DateTime.UtcNow.ToLong(),
                Flag = message.Flag,
                Properties = message.GetProperties(),
                ReconsumeTimes = 0,
                UnitMode = UnitMode,
                DefaultTopic = "TBW102"
            };
        }
    }
}
