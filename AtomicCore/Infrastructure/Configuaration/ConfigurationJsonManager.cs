using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AtomicCore
{
    /// <summary>
    /// 配置管理类
    /// </summary>
    public static class ConfigurationJsonManager
    {
        #region Variable

        /// <summary>
        /// env key - ASPNETCORE_ENVIRONMENT
        /// </summary>
        private const string c_env_development_key = "ASPNETCORE_ENVIRONMENT";

        /// <summary>
        /// env value - ASPNETCORE_ENVIRONMENT
        /// </summary>
        private const string c_env_development_val = "Development";

        /// <summary>
        /// appsettings.json
        /// </summary>
        private const string c_appsettings_main_fileName = "appsettings.json";

        /// <summary>
        /// appsettings.Development.json
        /// </summary>
        private const string c_appsettings_development_fileName = "appsettings.Development.json";

        /// <summary>
        /// appsettings.Production.json
        /// </summary>
        private const string c_appsettings_production_fileName = "appsettings.Production.json";

        /// <summary>
        /// connections.json
        /// </summary>
        private const string c_connections_main_fileName = "connections.json";

        /// <summary>
        /// connections.Development.json
        /// </summary>
        private const string c_connections_development_fileName = "connections.Development.json";

        /// <summary>
        /// connections.Production.json
        /// </summary>
        private const string c_connections_production_fileName = "connections.Production.json";

        /// <summary>
        /// connections.json -> connectionString
        /// </summary>
        private const string c_connectionString = "connectionString";

        /// <summary>
        /// connections.json -> providerName
        /// </summary>
        private const string c_providerName = "providerName";

        #endregion

        #region Constructor

        /// <summary>
        /// 静态构造
        /// </summary>
        static ConfigurationJsonManager()
        {
            ResetRootDir(null);
        }

        #endregion

        #region Propertys

        /// <summary>
        /// .NET CORE项目中的appsettings.json
        /// </summary>
        public static IConfiguration AppSettings { get; private set; }

        /// <summary>
        /// 数据库链接字符串
        /// </summary>
        public static IDictionary<string, ConnectionStringJsonSettings> ConnectionStrings { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reset Root
        /// </summary>
        /// <param name="dirPath">root dir path</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void ResetRootDir(string dirPath = null)
        {
            //runtime environment
            bool isDevelopment = false;
            string env_val = Environment.GetEnvironmentVariable(c_env_development_key);
            if (c_env_development_val.Equals(env_val, StringComparison.OrdinalIgnoreCase))
                isDevelopment = true;

            //base dir
            string baseDir = System.IO.Directory.GetCurrentDirectory();
            if (!string.IsNullOrEmpty(dirPath))
                baseDir = Path.Combine(baseDir, dirPath);

            //main config setting
            string appsetting_main_path = Path.Combine(baseDir, c_appsettings_main_fileName);
            string connections_main_path = Path.Combine(baseDir, c_connections_main_fileName);

            //extra appsetting configuration
            string appsetting_extra_path;
            if (isDevelopment)
                appsetting_extra_path = Path.Combine(baseDir, c_appsettings_development_fileName);
            else
                appsetting_extra_path = Path.Combine(baseDir, c_appsettings_production_fileName);

            //extra connection configuration
            string connection_extra_path;
            if (isDevelopment)
                connection_extra_path = Path.Combine(baseDir, c_connections_development_fileName);
            else
                connection_extra_path = Path.Combine(baseDir, c_connections_production_fileName);

            //check config file exists
            bool appsetting_main_existed = true;
            bool appsetting_extra_existed = true;
            bool connection_main_existed = true;
            bool connection_extra_existed = true;
            //Console.WriteLine($"[ConfigurationJsonManager] --> check appsetting.json file existed, the path is '{appsetting_main_path}'");
            //Console.WriteLine($"[ConfigurationJsonManager] --> check appsetting.{(isDevelopment ? "Development" : "Production")}.json file existed, the path is '{appsetting_extra_path}'");
            //Console.WriteLine($"[ConfigurationJsonManager] --> check connection.json file existed, the path is '{connections_main_path}'");
            //Console.WriteLine($"[ConfigurationJsonManager] --> check connection.{(isDevelopment ? "Development" : "Production")}.json file existed, the path is '{connection_extra_path}'");
            if (!File.Exists(appsetting_main_path))
            {
                Console.WriteLine($"[ConfigurationJsonManager] --> The '{c_appsettings_main_fileName}' file does not exist, please check if the file is added to the project and set to 'copy if newer'!");
                appsetting_main_existed = false;
            }
            if (!File.Exists(appsetting_extra_path))
            {
                Console.WriteLine($"[ConfigurationJsonManager] --> The 'appsetting.{(isDevelopment ? "Development" : "Production")}.json' file does not exist, please check if the file is added to the project and set to 'copy if newer'!");
                appsetting_extra_existed = false;
            }
            if (!File.Exists(connections_main_path))
            {
                Console.WriteLine($"[ConfigurationJsonManager]  --> The {c_connections_main_fileName} file does not exist, please check if the file is added in the project and set to 'copy if newer'!");
                connection_main_existed = false;
            }
            if (!File.Exists(connection_extra_path))
            {
                Console.WriteLine($"[ConfigurationJsonManager]  --> The connections.{(isDevelopment ? "Development" : "Production")}.json file does not exist, please check if the file is added in the project and set to 'copy if newer'!");
                connection_extra_existed = false;
            }

            //initial property value
            ConnectionStrings = new Dictionary<string, ConnectionStringJsonSettings>();

            //loading appsettings
            if (appsetting_main_existed)
            {
                //load configuration builder and compile
                var appSettingBuilder = new ConfigurationBuilder()
                        .SetBasePath(baseDir)
                        .AddJsonFile(c_appsettings_main_fileName, optional: true, reloadOnChange: true);
                if (appsetting_extra_existed)
                    appSettingBuilder.AddJsonFile(appsetting_extra_path, optional: true, reloadOnChange: true);

                //set 'AppSettings' property value
                AppSettings = appSettingBuilder.Build();
            }

            //loading connection
            if (connection_main_existed)
            {
                //load configuration builder
                var connectionBuilder = new ConfigurationBuilder()
                        .SetBasePath(baseDir)
                        .AddJsonFile(c_connections_main_fileName, optional: true, reloadOnChange: true);
                if (connection_extra_existed)
                    connectionBuilder.AddJsonFile(connection_extra_path, optional: true, reloadOnChange: true);

                //configuration compile
                var connectionRoot = connectionBuilder.Build();

                List<IConfigurationSection> childSections = connectionRoot.GetChildren().ToList();
                if (null != childSections && childSections.Any())
                {
                    ConnectionStringJsonSettings jsonSetting;
                    foreach (IConfigurationSection child in childSections)
                    {
                        string each_connectionString = child[c_connectionString];
                        string each_providerName = child[c_providerName];

                        if (string.IsNullOrEmpty(each_connectionString) || string.IsNullOrEmpty(each_providerName))
                            continue;

                        jsonSetting = new ConnectionStringJsonSettings()
                        {
                            Name = child.Key,
                            ConnectionString = each_connectionString,
                            ProviderName = each_providerName
                        };

                        ConnectionStrings.Add(child.Key, jsonSetting);
                    }
                }
            }
        }

        #endregion
    }
}
