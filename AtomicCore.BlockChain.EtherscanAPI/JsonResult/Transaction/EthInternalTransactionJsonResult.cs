using Newtonsoft.Json;

namespace AtomicCore.BlockChain.EtherscanAPI
{
    /// <summary>
    /// 内部交易结果集合
    /// </summary>
    public sealed class EthInternalTransactionJsonResult : EthTransactionJsonResult
    {
        /// <summary>
        /// 内部调用方式
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// 跟踪ID
        /// </summary>
        [JsonProperty("traceId")]
        public int TraceId { get; set; }

        /// <summary>
        /// 错误代码信息
        /// </summary>
        [JsonProperty("errCode")]
        public string ErrCode { get; set; }
    }
}
