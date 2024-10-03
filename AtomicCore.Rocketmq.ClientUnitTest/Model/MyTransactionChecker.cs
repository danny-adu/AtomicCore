using Org.Apache.Rocketmq;

namespace AtomicCore.Rocketmq.ClientUnitTest
{
    public class MyTransactionChecker : ITransactionChecker
    {
        public TransactionResolution Check(MessageView message)
        {
            // 执行本地事务检查逻辑，例如从数据库检查事务是否已经提交
            bool isCommitted = CheckTransactionStatus(message.MessageId);

            // 根据检查结果返回事务的状态
            if (isCommitted)
            {
                return TransactionResolution.Commit;
            }
            else
            {
                return TransactionResolution.Rollback;
            }
        }

        private bool CheckTransactionStatus(string messageId)
        {
            // 模拟检查事务状态
            // 这里可以连接数据库或者其他持久化存储来获取事务的实际状态
            return true;  // 假设事务成功提交
        }
    }
}
