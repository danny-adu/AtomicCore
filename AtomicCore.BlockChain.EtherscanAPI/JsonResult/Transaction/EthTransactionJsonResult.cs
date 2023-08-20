using Newtonsoft.Json;
using System;
using System.Numerics;

namespace AtomicCore.BlockChain.EtherscanAPI
{
    /// <summary>
    /// ETH交易基础抽象JSON结果集
    /// </summary>
    public abstract class EthTransactionJsonResult
    {
        /// <summary>
        /// 是否正常
        /// </summary>
        [JsonProperty("isError"), JsonConverter(typeof(BizStringIntToBoolJsonConverter))]
        public bool IsError { get; set; }

        /// <summary>
        /// 交易HASH
        /// </summary>
        [JsonProperty("hash")]
        public string TransactionHash { get; set; }

        /// <summary>
        /// 发起地址
        /// </summary>
        [JsonProperty("from")]
        public string From { get; set; }

        /// <summary>
        /// 到账地址
        /// </summary>
        [JsonProperty("to")]
        public string To { get; set; }

        /// <summary>
        /// 交易额度,以太坊单位为WEI,代币单位为deciamls约定
        /// </summary>
        [JsonProperty("value")]
        public BigInteger Value { get; set; }

        /// <summary>
        /// 燃气价
        /// </summary>
        [JsonProperty("gasPrice")]
        public BigInteger GasPrice { get; set; }

        /// <summary>
        /// 燃气量
        /// </summary>
        [JsonProperty("gas")]
        public BigInteger Gas { get; set; }

        /// <summary>
        /// 已使用燃料量
        /// </summary>
        [JsonProperty("gasUsed")]
        public BigInteger GasUsed { get; set; }

        /// <summary>
        /// 交易拓展数据
        /// </summary>
        [JsonProperty("input")]
        public string Input { get; set; }

        /// <summary>
        /// 合约地址
        /// </summary>
        [JsonProperty("contractAddress")]
        public string ContractAddress { get; set; }

        /// <summary>
        /// 交易确认状态
        /// </summary>
        [JsonProperty("txreceipt_status"), JsonConverter(typeof(BizStringIntToBoolJsonConverter))]
        public bool TxReceiptStatus { get; set; }

        /// <summary>
        /// 区块高度
        /// </summary>
        [JsonProperty("blockNumber"), JsonConverter(typeof(BizStringToULongJsonConverter))]
        public ulong BlockNumber { get; set; }

        /// <summary>
        /// 时间戳(1970-01-01 UTC) => 区块出块UTC时间
        /// </summary>
        [JsonProperty("timeStamp"), JsonConverter(typeof(BizTimestampToDateTimeJsonConverter))]
        public DateTime BlockTime { get; set; }
    }
}
