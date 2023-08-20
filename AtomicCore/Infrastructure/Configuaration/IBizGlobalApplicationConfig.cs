namespace AtomicCore
{
    /// <summary>
    /// 全局应用程序配置接口
    /// </summary>
    public interface IBizGlobalApplicationConfig
    {
        /// <summary>
        /// 是否是开发调试
        /// </summary>
        bool IsDevelopment { get; }

        /// <summary>
        /// 应用程序根目录地址
        /// eg -> IHostEnvironment.ContentRootPath
        /// </summary>
        string ApplicationRootPath { get; }

        /// <summary>
        /// 对称加密解密Key
        /// </summary>
        string SymmetryKey { get; }

        /// <summary>
        /// 获取当前入口程序的版本号
        /// </summary>
        /// <returns></returns>
        string GetVersion();
    }
}
