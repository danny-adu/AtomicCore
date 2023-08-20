using Microsoft.Extensions.Configuration;
using System;

namespace AtomicCore.BlockChain.OMNINet
{
    /// <summary>
    /// coin rpc setting
    /// </summary>
    public class CoinRpcSetting
    {
        #region Propertys

        /// <summary>
        /// rpc url
        /// </summary>
        public string RpcUrl { get; set; }

        /// <summary>
        /// rpc userName
        /// </summary>
        public string RpcUserName { get; set; }

        /// <summary>
        /// rpc password
        /// </summary>
        public string RpcPassword { get; set; }

        /// <summary>
        /// rpc timeout (seconds)
        /// </summary>
        public int RpcTimeout { get; set; } = 60;

        /// <summary>
        /// wallet password
        /// </summary>
        public string WalletPassword { get; set; }

        #endregion

        #region Static Methods

        /// <summary>
        /// load from config
        /// </summary>
        /// <returns></returns>
        public static CoinRpcSetting LoadFromConfig()
        {
            CoinRpcSetting cfg;

            string fileName = "omni.json";
            string baseDir = System.IO.Directory.GetCurrentDirectory();
            string jsonPath = string.Format("{0}\\{1}", baseDir, fileName);
            if (!System.IO.File.Exists(jsonPath))
                throw new System.IO.FileNotFoundException($"config json file '{fileName}' not exists!");

            IConfiguration _configuration = new ConfigurationBuilder()
                    .SetBasePath(baseDir)
                    .AddJsonFile(jsonPath, optional: true, reloadOnChange: true)
                    .Build();
            cfg = _configuration.Get<CoinRpcSetting>();

            if (null == cfg)
                throw new Exception($"NETSTANDARD2.0+ OR NETFRAMEWORK...");

            return cfg;
        }

        #endregion
    }
}
