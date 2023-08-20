using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace AtomicCore.IOStorage.StoragePort
{
    /// <summary>
    /// IOStorage Config配置信息
    /// </summary>
    public class BizIOStorageConfig
    {
        #region Variable

        /// <summary>
        /// 日志配置
        /// </summary>
        private const string LOGO_TAG = "IOStorage Config";

        /// <summary>
        /// 配置节点中的总节点
        /// </summary>
        private const string ROOT_NAME = "IOStorage";

        /// <summary>
        /// Env Access Token
        /// </summary>
        private const string ENV_IOSTORAGE_APPTOKEN = "IOSTORAGE_APPTOKEN";

        /// <summary>
        /// Env Save Path Root DirName
        /// </summary>
        private const string ENV_IOSTORAGE_SAVEROOTDIR = "IOSTORAGE_SAVEROOTDIR";

        /// <summary>
        /// Env Allow Save Exts
        /// </summary>
        private const string ENV_IOSTORAGE_ALLOWFILEEXTS = "IOSTORAGE_ALLOWFILEEXTS";

        /// <summary>
        /// Env Allow Save Max Size Limit
        /// </summary>
        private const string ENV_IOSTORAGE_ALLOWFILEMBSIZELIMIT = "IOSTORAGE_ALLOWFILEMBSIZELIMIT";

        #endregion

        #region Propertys

        /// <summary>
        /// APP应用TOKEN
        /// </summary>
        public string AppToken { get; set; }

        /// <summary>
        /// 存储根路径
        /// </summary>
        public string SaveRootDir { get; set; }

        /// <summary>
        /// 允许存储文件格式要求
        /// </summary>
        public string AllowFileExts { get; set; }

        /// <summary>
        /// 允许文件存储的最大限制（单位:MB）
        /// </summary>
        public string AllowFileMBSizeLimit { get; set; }

        #endregion

        #region Static Methods

        /// <summary>
        /// 创建新的实例
        /// </summary>
        /// <param name="SrvProvider"></param>
        /// <returns></returns>
        public static BizIOStorageConfig Create(IServiceProvider SrvProvider)
        {
            if (null == SrvProvider)
                return null;

            IConfiguration cfg = (IConfiguration)SrvProvider.GetService(typeof(IConfiguration));

            return Create(cfg);
        }

        /// <summary>
        /// 创建新的实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static BizIOStorageConfig Create(IConfiguration configuration)
        {
            BizIOStorageConfig config = null;

            // 1.日志对象初始化
            var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
                .SetMinimumLevel(LogLevel.Information)
                .AddConsole()
            );
            ILogger logger = loggerFactory.CreateLogger<BizIOStorageConfig>();

            // 2.首先从命令行变量读取相关参数【执行运行 或 DOCKER】
            var env_vars = Environment.GetEnvironmentVariables();
            if (null != env_vars && env_vars.Count > 0)
            {
                // 2.1.读取命令行参数
                string accessToken = env_vars.Contains(ENV_IOSTORAGE_APPTOKEN) ? env_vars[ENV_IOSTORAGE_APPTOKEN].ToString() : "1";
                string saveRootDir = env_vars.Contains(ENV_IOSTORAGE_SAVEROOTDIR) ? env_vars[ENV_IOSTORAGE_SAVEROOTDIR].ToString() : string.Empty;
                string allowFileExts = env_vars.Contains(ENV_IOSTORAGE_ALLOWFILEEXTS) ? env_vars[ENV_IOSTORAGE_ALLOWFILEEXTS].ToString() : "1";
                string allowSizeMaxLimit = env_vars.Contains(ENV_IOSTORAGE_ALLOWFILEMBSIZELIMIT) ? env_vars[ENV_IOSTORAGE_ALLOWFILEMBSIZELIMIT].ToString() : string.Empty;

                // 2.2.判断核心参数一定必须有值
                if (!string.IsNullOrEmpty(saveRootDir))
                {
                    if (string.IsNullOrEmpty(saveRootDir))
                        saveRootDir = "uploads";
                    if (string.IsNullOrEmpty(allowFileExts))
                        allowFileExts = ".jpg,.jpeg,.gif,.xls,.xlsx,.doc,.docx,.ppt,.pptx,.wgt,.apk,.bmp,.png,.psd,.txt,.pdf";
                    if (string.IsNullOrEmpty(allowSizeMaxLimit))
                        allowSizeMaxLimit = "50";

                    logger.LogInformation($"[{LOGO_TAG}] -> accessToken -> {accessToken}");
                    logger.LogInformation($"[{LOGO_TAG}] -> saveRootDir -> {saveRootDir}");
                    logger.LogInformation($"[{LOGO_TAG}] -> allowFileExts -> {allowFileExts}");
                    logger.LogInformation($"[{LOGO_TAG}] -> allowSizeMaxLimit -> [{allowSizeMaxLimit}]M");

                    config = new BizIOStorageConfig()
                    {
                        AppToken = accessToken,
                        SaveRootDir = saveRootDir,
                        AllowFileExts = allowFileExts,
                        AllowFileMBSizeLimit = allowSizeMaxLimit
                    };

                    if (null != logger)
                        logger.LogInformation($"[{LOGO_TAG}] -> adaptive loading data from startup parameters!");
                }
                else
                    if (null != logger)
                        logger.LogWarning($"[{LOGO_TAG}] -> unable to load properly from command parameters due to really critical parameters!");
            }

            // 3.从appsetting中读取数据(代码配置级)
            if (null == config && null != configuration)
            {
                IConfigurationSection rootSec = configuration.GetSection(ROOT_NAME);
                if (null != rootSec)
                {
                    config = rootSec.Get<BizIOStorageConfig>();

                    if (null != config && null != logger)
                    {
                        logger.LogInformation($"[{LOGO_TAG}] -> adaptive loading data from appsettings.json!");
                        logger.LogInformation($"[{LOGO_TAG}] -> accessToken -> {config.AppToken}");
                        logger.LogInformation($"[{LOGO_TAG}] -> saveRootDir -> {config.SaveRootDir}");
                        logger.LogInformation($"[{LOGO_TAG}] -> allowFileExts -> {config.AllowFileExts}");
                        logger.LogInformation($"[{LOGO_TAG}] -> allowSizeMaxLimit -> [{config.AllowFileMBSizeLimit}]M");
                    }
                }
            }

            // 4.如果还为空,则启用本地值（默认本地）
            if (null == config)
            {
                string random_token = Guid.NewGuid().ToString("N");
                config = new BizIOStorageConfig()
                {
                    AppToken = random_token,
                    SaveRootDir = "uploads",
                    AllowFileExts = ".jpg,.jpeg,.gif,.xls,.xlsx,.doc,.docx,.ppt,.pptx,.wgt,.apk,.bmp,.png,.psd,.txt,.pdf",
                    AllowFileMBSizeLimit = "[50]M"
                };
                logger.LogWarning($"[{LOGO_TAG}] -> the randwom access token is '{random_token}'");
                logger.LogInformation($"[{LOGO_TAG}] -> redis initializes default local connection!");
            }

            return config;
        }

        #endregion
    }
}
