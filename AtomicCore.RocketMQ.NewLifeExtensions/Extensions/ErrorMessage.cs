using NewLife.RocketMQ.Protocol;

namespace NewLife.RocketMQ
{
    /// <summary>
    /// 拓展发送信息
    /// </summary>
    public class ExtendedSendResult : SendResult
    {
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
