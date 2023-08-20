using System;
using System.Reflection;

namespace AtomicCore
{
    /// <summary>
    /// 全局应用程序配置类
    /// </summary>
    public class BizGlobalApplicationConfig : IBizGlobalApplicationConfig
    {
        #region Propertys

        /// <summary>
        /// 是否是开发调试
        /// </summary>
        public bool IsDevelopment { get; set; }

        /// <summary>
        /// 应用程序根目录地址
        /// eg -> IHostEnvironment.ContentRootPath
        /// </summary>
        public string ApplicationRootPath { get; set; }

        /// <summary>
        /// 对称加密解密Key
        /// </summary>
        public string SymmetryKey { get; set; }

        #endregion

        #region Pulic Methods

        /// <summary>
        /// 获取当前入口程序的版本号
        /// </summary>
        /// <returns></returns>
        public string GetVersion()
        {
            Version version = Assembly.GetEntryAssembly().GetName().Version;
            return string.Format(
                "{0}.{1}",
                version.Major,
                version.Minor
            );
        }

        #endregion
    }
}
