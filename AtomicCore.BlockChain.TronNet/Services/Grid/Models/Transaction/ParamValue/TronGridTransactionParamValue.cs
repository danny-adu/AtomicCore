using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronGrid Transaction Param Value
    /// PS : The current data is the base class for all trading parameters
    /// </summary>
    public class TronGridTransactionParamValue : ITronGridTransactionParamValue
    {
        #region Variables

        /// <summary>
        /// contract_address
        /// </summary>
        private const string c_contract_address = "contract_address";

        /// <summary>
        /// current param value JObject
        /// </summary>
        private JObject _paramValue = null;

        #endregion

        #region Propertys

        /// <summary>
        /// owner_address
        /// </summary>
        [JsonProperty("owner_address"), JsonConverter(typeof(TronGridAddressBase58JsonConverter))]
        public string OwnerAddress { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// set json object
        /// </summary>
        /// <param name="obj"></param>
        public void SetJObject(JObject obj)
        {
            _paramValue = obj;
        }

        /// <summary>
        /// ParamValue Parse To Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Parse<T>()
            where T : ITronGridTransactionParamValue, new()
        {
            if (null == _paramValue)
                return default;

            return _paramValue.ToObject<T>();
        }

        /// <summary>
        /// include contract adddress
        /// </summary>
        /// <param name="contractAddress"></param>
        /// <param name="isBase58Checked"></param>
        /// <returns></returns>
        public bool IncludContractAddress(string contractAddress, bool isBase58Checked = true)
        {
            if (string.IsNullOrEmpty(contractAddress))
                return false;
            if (null == _paramValue)
                return false;

            if (_paramValue.TryGetValue(c_contract_address, StringComparison.OrdinalIgnoreCase, out JToken jt))
            {
                string cur_address = isBase58Checked ?
                    TronNetECKey.ConvertToTronAddressFromHexAddress(jt.ToString()) :
                    jt.ToString();

                return contractAddress.Equals(cur_address, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        #endregion
    }
}
