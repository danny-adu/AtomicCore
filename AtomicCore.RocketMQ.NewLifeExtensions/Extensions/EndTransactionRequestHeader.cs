using System.Collections.Generic;

namespace NewLife.RocketMQ
{
    /// <summary>
    /// endTransacion request header
    /// </summary>
    public class EndTransactionRequestHeader
    {
        /// <summary>事务ID</summary>
        public string TransactionId { get; set; }

        /// <summary>提交或回滚的标志，0为提交，1为回滚</summary>
        public int CommitOrRollback { get; set; }

        /// <summary>转换为属性字典</summary>
        public IDictionary<string, string> GetProperties()
        {
            return new Dictionary<string, string>
            {
                { "transactionId", TransactionId },
                { "commitOrRollback", CommitOrRollback.ToString() }
            };
        }
    }
}
