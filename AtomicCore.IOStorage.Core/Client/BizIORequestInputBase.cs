namespace AtomicCore.IOStorage.Core
{
    /// <summary>
    /// IO请求输入参数抽象类
    /// </summary>
    public abstract class BizIORequestInputBase
    {
        /// <summary>
        /// 请求APIKey(head内认证)
        /// </summary>
        public string APIKey { get; set; }
    }
}
