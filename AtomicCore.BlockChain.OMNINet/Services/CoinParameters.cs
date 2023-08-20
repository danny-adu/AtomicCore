using System;
using System.Diagnostics;

namespace AtomicCore.BlockChain.OMNINet
{
    /// <summary>
    /// digital currency parameter definition
    /// </summary>
    public class CoinParameters
    {
        #region Constructor

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="coinService"></param>
        /// <param name="daemonUrl"></param>
        /// <param name="rpcUsername"></param>
        /// <param name="rpcPassword"></param>
        /// <param name="walletPassword"></param>
        /// <param name="rpcRequestTimeoutInSeconds"></param>
        public CoinParameters(ICoinService coinService,
            string daemonUrl,
            string rpcUsername,
            string rpcPassword,
            string walletPassword,
            short rpcRequestTimeoutInSeconds)
        {
            #region 设置RPC请求URL地址

            CoinRpcSetting rpcSetting = null;

            if (!string.IsNullOrWhiteSpace(daemonUrl))
            {
                DaemonUrl = daemonUrl;
                RpcUsername = rpcUsername;
                RpcPassword = rpcPassword;
                WalletPassword = walletPassword;

                // this will force the CoinParameters.SelectedDaemonUrl dynamic property to automatically pick the daemonUrl defined above
                UseTestnet = false;

                // ignore config files
                IgnoreConfigFiles = true;
            }
            else
                rpcSetting = CoinRpcSetting.LoadFromConfig();

            #endregion

            #region 设置或读取RPC请求超时设置

            if (IgnoreConfigFiles)
                if (rpcRequestTimeoutInSeconds > 0)
                    RpcRequestTimeoutInSeconds = rpcRequestTimeoutInSeconds;
                else
                    RpcRequestTimeoutInSeconds = 60;
            else
                RpcRequestTimeoutInSeconds = (short)rpcSetting.RpcTimeout;

            #endregion

            #region 配置加载策略

            if (IgnoreConfigFiles && (string.IsNullOrWhiteSpace(DaemonUrl) || string.IsNullOrWhiteSpace(RpcUsername) || string.IsNullOrWhiteSpace(RpcPassword)))
            {
                throw new Exception(string.Format("One or more required parameters, as defined in {0}, were not found in the configuration file!", GetType().Name));
            }

            if (IgnoreConfigFiles && Debugger.IsAttached && string.IsNullOrWhiteSpace(WalletPassword))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[WARNING] The wallet password is either null or empty");
                Console.ResetColor();
            }

            if (!IgnoreConfigFiles)
            {
                //获取值
                DaemonUrl = rpcSetting.RpcUrl;
                if (string.IsNullOrEmpty(DaemonUrl))
                    throw new Exception("DaemonUrl is null or empty");

                DaemonUrlTestnet = DaemonUrl;
                RpcUsername = rpcSetting.RpcUserName;
                RpcPassword = rpcSetting.RpcPassword;
                WalletPassword = rpcSetting.WalletPassword;
                if (string.IsNullOrEmpty(WalletPassword))
                    WalletPassword = string.Empty;
            }

            #endregion

            #region 货币配置加载

            if (coinService is ExtracoinService)
            {
                #region BTC COIN

                CoinShortName = "BTC";//modify
                CoinLongName = "Bit Coin ";//modify
                IsoCurrencyCode = "XBT";//比特币是XBT,X开头即可

                TransactionSizeBytesContributedByEachInput = 148;
                TransactionSizeBytesContributedByEachOutput = 34;
                TransactionSizeFixedExtraSizeInBytes = 10;

                FreeTransactionMaximumSizeInBytes = 1000;
                FreeTransactionMinimumOutputAmountInCoins = 0.01M;
                FreeTransactionMinimumPriority = 57600000;
                FeePerThousandBytesInCoins = 0.0001M;
                MinimumTransactionFeeInCoins = 0.0001M;
                MinimumNonDustTransactionAmountInCoins = 0.0000543M;

                TotalCoinSupplyInCoins = 21000000;
                EstimatedBlockGenerationTimeInMinutes = 10;
                BlocksHighestPriorityTransactionsReservedSizeInBytes = 50000;

                BaseUnitName = "Satoshi";
                BaseUnitsPerCoin = 100000000;
                CoinsPerBaseUnit = 0.00000001M;

                #endregion
            }
            else if (coinService is BlackCoinService)
            {
                #region BLOCK COIN

                CoinShortName = "HBC";//modify
                CoinLongName = "Blackcoin ";//modify
                IsoCurrencyCode = "XHB";//比特币是XBT,X开头即可

                TransactionSizeBytesContributedByEachInput = 148;
                TransactionSizeBytesContributedByEachOutput = 34;
                TransactionSizeFixedExtraSizeInBytes = 10;

                FreeTransactionMaximumSizeInBytes = 1000;
                FreeTransactionMinimumOutputAmountInCoins = 0.01M;
                FreeTransactionMinimumPriority = 57600000;
                FeePerThousandBytesInCoins = 0.0001M;
                MinimumTransactionFeeInCoins = 0.0001M;
                MinimumNonDustTransactionAmountInCoins = 0.0000543M;

                TotalCoinSupplyInCoins = 21000000;
                EstimatedBlockGenerationTimeInMinutes = 10;
                BlocksHighestPriorityTransactionsReservedSizeInBytes = 50000;

                BaseUnitName = "Satoshi";
                BaseUnitsPerCoin = 100000000;
                CoinsPerBaseUnit = 0.00000001M;

                #endregion
            }
            else if (coinService is OMNICoinService)
            {
                #region USDT COIN

                CoinShortName = "USDT";//modify
                CoinLongName = "TetherUS ";//modify
                IsoCurrencyCode = "XUSDT";//比特币是XBT,X开头即可

                TransactionSizeBytesContributedByEachInput = 148;
                TransactionSizeBytesContributedByEachOutput = 34;
                TransactionSizeFixedExtraSizeInBytes = 10;

                FreeTransactionMaximumSizeInBytes = 1000;
                FreeTransactionMinimumOutputAmountInCoins = 0.01M;
                FreeTransactionMinimumPriority = 57600000;
                FeePerThousandBytesInCoins = 0.0001M;
                MinimumTransactionFeeInCoins = 0.0001M;
                MinimumNonDustTransactionAmountInCoins = 0.0000543M;

                TotalCoinSupplyInCoins = 21000000;
                EstimatedBlockGenerationTimeInMinutes = 10;
                BlocksHighestPriorityTransactionsReservedSizeInBytes = 50000;

                BaseUnitName = "Satoshi";
                BaseUnitsPerCoin = 100000000;
                CoinsPerBaseUnit = 0.00000001M;

                #endregion
            }
            else
                throw new Exception("Unknown coin!");

            #endregion

            #region Invalid configuration / Missing parameters

            if (RpcRequestTimeoutInSeconds <= 0)
            {
                throw new Exception("RpcRequestTimeoutInSeconds must be greater than zero");
            }

            if (string.IsNullOrWhiteSpace(DaemonUrl)
                || string.IsNullOrWhiteSpace(RpcUsername)
                || string.IsNullOrWhiteSpace(RpcPassword))
            {
                throw new Exception(string.Format("One or more required parameters, as defined in {0}, were not found in the configuration file!", GetType().Name));
            }

            #endregion
        }

        #endregion

        #region Propertys

        /// <summary>
        /// 忽略配置文件
        /// </summary>
        public bool IgnoreConfigFiles { get; private set; }

        /// <summary>
        /// 货币单位,例如:Satoshi Koinu Litetoshi
        /// </summary>
        public string BaseUnitName { get; private set; }

        /// <summary>
        /// 个人货币ichu单位
        /// </summary>
        public uint BaseUnitsPerCoin { get; private set; }

        /// <summary>
        /// 区块索引优先交易保留的字节大小
        /// </summary>
        public int BlocksHighestPriorityTransactionsReservedSizeInBytes { get; private set; }

        /// <summary>
        /// Block Maximum Size In Bytes
        /// </summary>
        public int BlockMaximumSizeInBytes { get; private set; }

        /// <summary>
        /// 货币缩写,例如:比特币 BTC,夺宝币 DBC
        /// </summary>
        public string CoinShortName { get; private set; }

        /// <summary>
        /// 货币全称,例如:比特币 Bitcoin
        /// </summary>
        public string CoinLongName { get; private set; }

        /// <summary>
        /// 货币个人基础单位
        /// </summary>
        public decimal CoinsPerBaseUnit { get; private set; }

        /// <summary>
        /// 估计每分钟生成数据块的数量
        /// </summary>
        public double EstimatedBlockGenerationTimeInMinutes { get; set; }

        /// <summary>
        /// 估计前一天生成数据块的数量
        /// </summary>
        public int ExpectedNumberOfBlocksGeneratedPerDay
        {
            get
            {
                return (int)EstimatedBlockGenerationTimeInMinutes * GlobalConstants.MinutesInADay;
            }
        }

        public decimal FeePerThousandBytesInCoins { get; set; }
        public short FreeTransactionMaximumSizeInBytes { get; set; }
        public decimal FreeTransactionMinimumOutputAmountInCoins { get; set; }
        public int FreeTransactionMinimumPriority { get; set; }

        public string IsoCurrencyCode { get; set; }
        public decimal MinimumNonDustTransactionAmountInCoins { get; set; }
        public decimal MinimumTransactionFeeInCoins { get; set; }
        public decimal OneBaseUnitInCoins
        {
            get { return CoinsPerBaseUnit; }
        }
        public uint OneCoinInBaseUnits
        {
            get { return BaseUnitsPerCoin; }
        }


        public ulong TotalCoinSupplyInCoins { get; set; }
        public int TransactionSizeBytesContributedByEachInput { get; set; }
        public int TransactionSizeBytesContributedByEachOutput { get; set; }
        public int TransactionSizeFixedExtraSizeInBytes { get; set; }


        /// <summary>
        /// RPC线路地址
        /// </summary>
        public string DaemonUrl { get; private set; }
        /// <summary>
        /// 线路地址（测试）
        /// </summary>
        public string DaemonUrlTestnet { get; private set; }
        /// <summary>
        /// RPC用户名
        /// </summary>
        public string RpcUsername { get; private set; }
        /// <summary>
        /// RPC密码
        /// </summary>
        public string RpcPassword { get; private set; }
        /// <summary>
        /// 钱包密码
        /// </summary>
        public string WalletPassword { get; private set; }
        /// <summary>
        /// RPC连接超时设置
        /// </summary>
        public short RpcRequestTimeoutInSeconds { get; private set; }
        /// <summary>
        /// 是否是用户测试线路
        /// </summary>
        public bool UseTestnet { get; set; }

        /// <summary>
        /// RPC线路选项适配器
        /// </summary>
        public string SelectedDaemonUrl
        {
            get { return !UseTestnet ? DaemonUrl : DaemonUrlTestnet; }
        }

        #endregion
    }
}
