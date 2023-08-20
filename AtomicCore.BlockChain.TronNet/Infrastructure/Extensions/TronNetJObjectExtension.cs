using Google.Protobuf;
using Newtonsoft.Json.Linq;
using System;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet JObject Extension
    /// </summary>
    public static class TronNetJObjectExtension
    {
        #region Variables

        /// <summary>
        /// TRC20 - Transfer Method top 4 Bytes Hex
        /// </summary>
        private const string c_trc20Transfer = "a9059cbb";

        #endregion

        #region Public Methods

        /// <summary>
        /// JObject => Contract Type Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jobject"></param>
        /// <returns></returns>
        public static T ToContractValue<T>(this JObject jobject)
            where T : TronNetContractBaseValueJson, new()
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jobject.ToString());
        }

        #endregion

        #region Basic Property Getter

        /// <summary>
        /// Get Owner Address
        /// </summary>
        /// <param name="jobject"></param>
        /// <returns></returns>
        public static string GetOwnerAddress(this JObject jobject)
        {
            bool flag = jobject.TryGetValue("owner_address", out JToken token);
            if (!flag)
                return string.Empty;

            return token.ToString();
        }

        #endregion

        #region Trx && Trc10 Property Getter

        /// <summary>
        /// Get TRC10 ToAddress
        /// </summary>
        /// <param name="jobject"></param>
        /// <returns></returns>
        public static string GetTrc10ToAddress(this JObject jobject)
        {
            bool flag = jobject.TryGetValue("to_address", out JToken token);
            if (!flag)
                return string.Empty;

            return token.ToString();
        }

        /// <summary>
        /// Get Trc10 Asset Name
        /// </summary>
        /// <param name="jobject"></param>
        /// <returns></returns>
        public static string GetTrc10AssetName(this JObject jobject)
        {
            bool flag = jobject.TryGetValue("asset_name", out JToken token);
            if (!flag)
                return string.Empty;

            return token.ToString();
        }

        /// <summary>
        /// Get Trc10 OrigAmount
        /// </summary>
        /// <param name="jobject"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static ulong GetTrc10Amount(this JObject jobject)
        {
            bool flag = jobject.TryGetValue("amount", out JToken token);
            if (!flag)
                return 0UL;

            return Convert.ToUInt64(token.ToString());
        }

        /// <summary>
        /// Get Trc10 Amount
        /// </summary>
        /// <param name="jobject"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static decimal GetTrc10Amount(this JObject jobject, int decimals)
        {
            ulong origAmount = GetTrc10Amount(jobject);

            if (decimals <= 0)
                return origAmount;
            else
                return origAmount / (decimal)Math.Pow(10, decimals);
        }

        #endregion

        #region Trc20 Property Getter

        /// <summary>
        /// Get Contract Address
        /// </summary>
        /// <param name="jobject"></param>
        /// <returns></returns>
        public static string GetContractAddress(this JObject jobject)
        {
            bool flag = jobject.TryGetValue("contract_address", out JToken token);
            if (!flag)
                return string.Empty;

            return token.ToString();
        }

        /// <summary>
        /// Get Trc20 Data
        /// </summary>
        /// <param name="jobject"></param>
        /// <returns></returns>
        public static string GetTrc20Data(this JObject jobject)
        {
            bool flag = jobject.TryGetValue("data", out JToken token);
            if (!flag)
                return string.Empty;

            return token.ToString();
        }

        /// <summary>
        /// Get Trc20 ToEthAddress
        /// </summary>
        /// <param name="jobject"></param>
        /// <param name="isUpper"></param>
        /// <returns></returns>
        public static string GetTrc20ToEthAddress(this JObject jobject, bool isUpper = false)
        {
            string data = GetTrc20Data(jobject);

            if (string.IsNullOrEmpty(data))
                return string.Empty;
            if (!data.StartsWith(c_trc20Transfer, StringComparison.OrdinalIgnoreCase))
                return string.Empty;

            string hexAddress = data.Substring(30, 42);

            return TronNetECKey.ConvertToEthAddressFromHexAddress(hexAddress, isUpper);
        }

        /// <summary>
        /// Get Trc20 ToTronAddress
        /// </summary>
        /// <param name="jobject"></param>
        /// <param name="network"></param>
        /// <returns></returns>
        public static string GetTrc20ToTronAddress(this JObject jobject, TronNetwork network = TronNetwork.MainNet)
        {
            string data = GetTrc20Data(jobject);

            if (string.IsNullOrEmpty(data))
                return string.Empty;
            if (!data.StartsWith(c_trc20Transfer, StringComparison.OrdinalIgnoreCase))
                return string.Empty;

            string hexAddress = data.Substring(30, 42);

            return TronNetECKey.ConvertToTronAddressFromHexAddress(hexAddress, network);
        }

        /// <summary>
        /// Get Trc20 OrigAmount
        /// </summary>
        /// <param name="jobject"></param>
        /// <returns></returns>
        public static ulong GetTrc20Amount(this JObject jobject)
        {
            string data = GetTrc20Data(jobject);

            if (string.IsNullOrEmpty(data))
                return 0UL;
            if (!data.StartsWith(c_trc20Transfer, StringComparison.OrdinalIgnoreCase))
                return 0UL;
            if ((data.Length - 8) % 64 != 0)
                return 0UL;

            string removeMethodTopic = data.Substring(8);
            string amountHex = TronNetUntils.RemoveHexZero(removeMethodTopic.Substring(64, 64), TronNetHexCuteZeroStrategy.Left, 0, true);

            return Convert.ToUInt64(amountHex, 16);
        }

        /// <summary>
        /// Get Trc20 Amount
        /// </summary>
        /// <param name="jobject"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static decimal GetTrc20Amount(this JObject jobject, int decimals)
        {
            ulong origAmount = GetTrc20Amount(jobject);

            if (decimals <= 0)
                return origAmount;
            else
                return origAmount / (decimal)Math.Pow(10, decimals);
        }

        #endregion
    }
}
