using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Tron Rest API Implementation Class
    /// </summary>
    public class TronNetRest : ITronNetRest
    {
        #region Variables

        private const string c_apiKeyName = "TRON-PRO-API-KEY";
        private readonly IOptions<TronNetOptions> _options;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public TronNetRest(IOptions<TronNetOptions> options)
        {
            _options = options;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Create Full Node Rest Url
        /// </summary>
        /// <param name="actionUrl">接口URL</param>
        /// <returns></returns>
        private string CreateFullNodeRestUrl(string actionUrl)
        {
            return string.Format(
                "{0}{1}",
                this._options.Value.FullNodeRestAPI,
                actionUrl
            );
        }

        /// <summary>
        /// Create Solidity Node Rest Url
        /// </summary>
        /// <param name="actionUrl"></param>
        /// <returns></returns>
        private string CreateSolidityNodeRestUrl(string actionUrl)
        {
            return string.Format(
                "{0}{1}",
                this._options.Value.SolidityNodeRestAPI,
                actionUrl
            );
        }

        /// <summary>
        /// Create Super Node Rest Url
        /// </summary>
        /// <param name="actionUrl"></param>
        /// <returns></returns>
        private string CreateSuperNodeRestUrl(string actionUrl)
        {
            return string.Format(
                "{0}{1}",
                this._options.Value.SuperNodeRestAPI,
                actionUrl
            );
        }

        /// <summary>
        /// Rest Get Json Result
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string RestGetJson(string url)
        {
            string resp;
            try
            {
                //head api key
                Dictionary<string, string> heads = new Dictionary<string, string>()
                {
                    { "Accept","application/json"}
                };

                string apiKey = this._options.Value.ApiKey;
                if (!string.IsNullOrEmpty(apiKey))
                    heads.Add(c_apiKeyName, apiKey);

                resp = HttpProtocol.HttpGet(url, heads: heads);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resp;
        }

        /// <summary>
        /// Rest Post Json Request
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string RestPostJson(string url)
        {
            string resp;
            try
            {
                //head api key
                Dictionary<string, string> heads = null;
                string apiKey = this._options.Value.ApiKey;
                if (!string.IsNullOrEmpty(apiKey))
                    heads = new Dictionary<string, string>()
                    {
                        { c_apiKeyName,apiKey}
                    };

                resp = HttpProtocol.HttpPost(url, null, heads: heads);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resp;
        }

        /// <summary>
        /// Rest Post Json Request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">rest url</param>
        /// <param name="json">json data</param>
        /// <returns></returns>
        private string RestPostJson<T>(string url, T json)
        {
            string resp;
            try
            {
                //post data
                string post_data = Newtonsoft.Json.JsonConvert.SerializeObject(json);

                //head api key
                Dictionary<string, string> heads = null;
                string apiKey = this._options.Value.ApiKey;
                if (!string.IsNullOrEmpty(apiKey))
                    heads = new Dictionary<string, string>()
                    {
                        { c_apiKeyName,apiKey}
                    };

                resp = HttpProtocol.HttpPost(url, post_data, heads: heads);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resp;
        }

        /// <summary>
        /// StringJson Parse to Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resp"></param>
        /// <returns></returns>
        private T ObjectParse<T>(string resp)
            where T : TronNetValidRestJson, new()
        {
            if (resp.Contains("\"Error\""))
            {
                JObject errorJson = JObject.Parse(resp);
                if (errorJson.ContainsKey("Error"))
                {
                    return new T()
                    {
                        Error = errorJson.GetValue("Error").ToString()
                    };
                }
            }

            T jsonResult;
            try
            {
                jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(resp);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return jsonResult;
        }

        #endregion

        #region ITronAddressUtilitiesRest

        /// <summary>
        /// Generates a random private key and address pair. 
        /// use offline code generation
        /// </summary>
        /// <returns>Returns a private key, the corresponding address in hex,and base58</returns>
        public TronNetAddressKeyPairRestJson GenerateAddress()
        {
            TronNetECKey newKey = TronNetECKey.GenerateKey();

            return new TronNetAddressKeyPairRestJson()
            {
                Address = newKey.GetPublicAddress(),
                PrivateKey = newKey.GetPrivateKey(),
                HexAddress = newKey.GetHexAddress()
            };
        }

        /// <summary>
        /// Create address from a specified password string (NOT PRIVATE KEY)
        /// Risk Warning : there is a security risk. 
        /// This interface service has been shutdown by the Trongrid. 
        /// Please use the offline mode or the node deployed by yourself.
        /// </summary>
        /// <param name="passphrase"></param>
        /// <returns></returns>
        public TronNetAddressBase58CheckRestJson CreateAddress(string passphrase)
        {
            if (string.IsNullOrEmpty(passphrase))
                throw new ArgumentNullException(nameof(passphrase));

            string passphraseHex = Encoding.UTF8.GetBytes(passphrase).ToHex();
            string url = CreateFullNodeRestUrl("/wallet/createaddress");
            string resp = this.RestPostJson(url, new { value = passphraseHex });
            TronNetAddressBase58CheckRestJson restJson = ObjectParse<TronNetAddressBase58CheckRestJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Validates address, returns either true or false.
        /// </summary>
        /// <param name="address">Tron Address</param>
        /// <param name="visible">Optional,whether the address is in base58 format</param>
        /// <returns></returns>
        public TronNetAddressValidRestJson ValidateAddress(string address, bool visible = true)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException(nameof(address));

            //variables
            string post_address = visible ? address : TronNetECKey.ConvertToHexAddress(address);
            string url = CreateFullNodeRestUrl("/wallet/validateaddress");
            string resp = this.RestPostJson(url, new { address = post_address, visible });
            TronNetAddressValidRestJson restJson = ObjectParse<TronNetAddressValidRestJson>(resp);

            return restJson;
        }

        #endregion

        #region ITronNetAccountRest

        /// <summary>
        /// Create an account. Uses an already activated account to create a new account
        /// </summary>
        /// <param name="ownerAddress">Owner_address is an activated account，converted to a hex String.If the owner_address has enough bandwidth obtained by freezing TRX, then creating an account will only consume bandwidth , otherwise, 0.1 TRX will be burned to pay for bandwidth, and at the same time, 1 TRX will be required to be created.</param>
        /// <param name="accountAddress">account_address is the address of the new account, converted to a hex string, this address needs to be calculated in advance</param>
        /// <param name="permissionID">Optional,whether the address is in base58 format</param>
        /// <param name="visible">Optional,for multi-signature use</param>
        /// <returns></returns>
        public TronNetCreateTransactionRestJson CreateAccount(string ownerAddress, string accountAddress, int? permissionID = null, bool visible = true)
        {
            if (string.IsNullOrEmpty(ownerAddress))
                throw new ArgumentNullException(nameof(ownerAddress));
            if (string.IsNullOrEmpty(accountAddress))
                throw new ArgumentNullException(nameof(accountAddress));

            //variables
            string post_owner_address = visible ? ownerAddress : TronNetECKey.ConvertToHexAddress(ownerAddress);
            string post_account_address = visible ? accountAddress : TronNetECKey.ConvertToHexAddress(accountAddress);

            //create request data
            dynamic reqData = new
            {
                owner_address = post_owner_address,
                account_address = post_account_address,
                visible
            };
            if (null != permissionID)
                reqData.permission_id = permissionID;

            string url = CreateFullNodeRestUrl("/wallet/createaccount");
            string resp = this.RestPostJson(url, reqData);
            TronNetCreateTransactionRestJson restJson = ObjectParse<TronNetCreateTransactionRestJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Query information about an account,Including balances, stake, votes and time, etc.
        /// </summary>
        /// <param name="address">address should be converted to a hex string</param>
        /// <param name="visible">Optional,whether the address is in base58 format</param>
        public TronNetAccountBalanceJson GetAccount(string address, bool visible = true)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException(nameof(address));

            //variables
            string post_address = visible ? address : TronNetECKey.ConvertToHexAddress(address);

            //create request data
            dynamic reqData = new
            {
                address = post_address,
                visible
            };

            string url = CreateFullNodeRestUrl("/wallet/getaccount");
            string resp = this.RestPostJson(url, reqData);
            TronNetAccountBalanceJson restJson = ObjectParse<TronNetAccountBalanceJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Modify account name
        /// </summary>
        /// <param name="accountName">Account_name is the name of the account</param>
        /// <param name="ownerAddress">Owner_address is the account address to be modified</param>
        /// <param name="permissionID">Optional,for multi-signature use</param>
        /// <param name="visible">Optional,whether the address is in base58 format</param>
        /// <returns></returns>
        public TronNetCreateTransactionRestJson UpdateAccount(string accountName, string ownerAddress, int? permissionID = null, bool visible = true)
        {
            if (string.IsNullOrEmpty(accountName))
                throw new ArgumentNullException(nameof(accountName));
            if (string.IsNullOrEmpty(ownerAddress))
                throw new ArgumentNullException(nameof(ownerAddress));

            //variables
            string hex_account_name = TronNetUntils.StringToHexString(accountName, true, Encoding.UTF8);
            string post_owner_address = visible ? ownerAddress : TronNetECKey.ConvertToHexAddress(ownerAddress);

            //create request data
            dynamic reqData = new
            {
                account_name = hex_account_name,
                owner_address = post_owner_address,
                visible
            };
            if (null != permissionID)
                reqData.permission_id = permissionID;

            string url = CreateFullNodeRestUrl("/wallet/updateaccount");
            string resp = this.RestPostJson(url, reqData);
            TronNetCreateTransactionRestJson restJson = ObjectParse<TronNetCreateTransactionRestJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Update the account's permission.
        /// </summary>
        /// <param name="ownerAddress"></param>
        /// <param name="actives"></param>
        /// <param name="owner"></param>
        /// <param name="witness"></param>
        /// <param name="visible"></param>
        /// <returns></returns>
        public TronNetCreateTransactionRestJson AccountPermissionUpdate(string ownerAddress, TronNetAccountOperatePermissionJson actives, TronNetAccountOperatePermissionJson owner, TronNetAccountOperatePermissionJson witness, bool visible = true)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the account balance in a specific block.
        /// (Note: At present, the interface data can only be queried through 
        /// the following official nodes 47.241.20.47 & 161.117.85.97 &161.117.224.116 &161.117.83.38)
        /// </summary>
        /// <param name="address">tron address</param>
        /// <param name="blockHash">block hash</param>
        /// <param name="blockHeight">block height</param>
        /// <param name="visible">Optional,whether the address is in base58 format</param>
        /// <returns></returns>
        public TronNetBlockAccountBalanceJson GetAccountBalance(string address, string blockHash, ulong blockHeight, bool visible = true)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException(nameof(address));
            if (string.IsNullOrEmpty(blockHash))
                throw new ArgumentNullException(nameof(blockHash));
            if (blockHeight <= 0)
                throw new ArgumentException("block height must be greater than 0");

            //variables
            string post_address = visible ? address : TronNetECKey.ConvertToHexAddress(address);

            //create request data
            dynamic reqData = new
            {
                account_identifier = new
                {
                    address = post_address
                },
                block_identifier = new
                {
                    hash = blockHash,
                    number = blockHeight
                },
                visible
            };

            string url = CreateFullNodeRestUrl("/wallet/getaccountbalance");
            string resp = this.RestPostJson(url, reqData);
            TronNetBlockAccountBalanceJson restJson = ObjectParse<TronNetBlockAccountBalanceJson>(resp);

            return restJson;
        }

        #endregion

        #region ITronNetAccountResourcesRest

        /// <summary>
        /// Query the resource information of an account(bandwidth,energy,etc)
        /// </summary>
        /// <param name="address">address</param>
        /// <param name="visible">Optional,whether the address is in base58 format</param>
        /// <returns></returns>
        public TronNetAccountResourceJson GetAccountResource(string address, bool visible = true)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException(nameof(address));

            //variables
            string post_address = visible ? address : TronNetECKey.ConvertToHexAddress(address);

            //create request data
            dynamic reqData = new
            {
                address = post_address,
                visible
            };

            string url = CreateFullNodeRestUrl("/wallet/getaccountresource");
            string resp = this.RestPostJson(url, reqData);
            TronNetAccountResourceJson restJson = ObjectParse<TronNetAccountResourceJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Query bandwidth information.
        /// </summary>
        /// <param name="address">address</param>
        /// <param name="visible">Optional,whether the address is in base58 format</param>
        /// <returns></returns>
        public TronNetAccountNetResourceJson GetAccountNet(string address, bool visible = true)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException(nameof(address));

            //variables
            string post_address = visible ? address : TronNetECKey.ConvertToHexAddress(address);

            //create request data
            dynamic reqData = new
            {
                address = post_address,
                visible
            };

            string url = CreateFullNodeRestUrl("/wallet/getaccountnet");
            string resp = this.RestPostJson(url, reqData);
            TronNetAccountNetResourceJson restJson = ObjectParse<TronNetAccountNetResourceJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Stake an amount of TRX to obtain bandwidth OR Energy and TRON Power (voting rights) .
        /// Optionally, user can stake TRX to grant Energy or Bandwidth to others. 
        /// Balance amount in the denomination of sun.s
        /// </summary>
        /// <param name="ownerAddress">Owner address</param>
        /// <param name="frozenBalance">TRX stake amount,Trx</param>
        /// <param name="frozenDuration">TRX stake duration, only be specified as 3 days</param>
        /// <param name="resource">TRX stake type, 'BANDWIDTH' or 'ENERGY'</param>
        /// <param name="receiverAddress"></param>
        /// <param name="permissionID">Optional, for multi-signature use</param>
        /// <param name="visible">Optional, Whether the address is in base58 format.</param>
        /// <returns></returns>
        public TronNetCreateTransactionRestJson FreezeBalance(string ownerAddress, decimal frozenBalance, int frozenDuration, TronNetResourceType resource, string receiverAddress = null, int? permissionID = null, bool visible = true)
        {
            if (string.IsNullOrEmpty(ownerAddress))
                throw new ArgumentNullException(nameof(ownerAddress));
            if (frozenBalance <= decimal.Zero)
                throw new ArgumentException("frozen trx balance must be greater than 0");
            if (frozenDuration < 3)
                throw new ArgumentException("frozen duration day must be greater than 0");

            //variables
            string post_owner_address = visible ? ownerAddress : TronNetECKey.ConvertToHexAddress(ownerAddress);

            //create request data
            dynamic reqData = new ExpandoObject();
            reqData.owner_address = post_owner_address;
            reqData.frozen_balance = (long)(frozenBalance * 1000000);
            reqData.frozen_duration = frozenDuration;
            reqData.resource = resource.ToString().ToUpper();
            reqData.visible = visible;
            if (!string.IsNullOrEmpty(receiverAddress))
            {
                string post_receiver_adddress = visible ? receiverAddress : TronNetECKey.ConvertToHexAddress(receiverAddress);
                reqData.receiver_address = post_receiver_adddress;
            }
            if (null != permissionID)
                reqData.permission_id = permissionID.Value;

            string url = CreateFullNodeRestUrl("/wallet/freezebalance");
            string resp = this.RestPostJson(url, reqData);
            TronNetCreateTransactionRestJson restJson = ObjectParse<TronNetCreateTransactionRestJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Unstake TRX that has passed the minimum stake duration to release bandwidth and energy 
        /// and at the same time TRON Power will reduce and all votes will be canceled.
        /// </summary>
        /// <param name="ownerAddress">Owner address</param>
        /// <param name="resource">Stake TRX for 'BANDWIDTH' or 'ENERGY'</param>
        /// <param name="receiverAddress">Optional,the address that will lose the resource</param>
        /// <param name="permissionID">Optional, for multi-signature use</param>
        /// <param name="visible">Optional, Whether the address is in base58 format.</param>
        /// <returns></returns>
        public TronNetCreateTransactionRestJson UnfreezeBalance(string ownerAddress, TronNetResourceType resource, string receiverAddress = null, int? permissionID = null, bool visible = true)
        {
            if (string.IsNullOrEmpty(ownerAddress))
                throw new ArgumentNullException(nameof(ownerAddress));

            //variables
            string post_owner_address = visible ? ownerAddress : TronNetECKey.ConvertToHexAddress(ownerAddress);

            //create request data
            dynamic reqData = new
            {
                owner_address = post_owner_address,
                resource = resource.ToString().ToUpper(),
                visible
            };
            if (!string.IsNullOrEmpty(receiverAddress))
            {
                string post_receiver_adddress = visible ? receiverAddress : TronNetECKey.ConvertToHexAddress(receiverAddress);
                reqData.receiver_address = post_receiver_adddress;
            }
            if (null != permissionID)
                reqData.permission_id = permissionID.Value;

            string url = CreateFullNodeRestUrl("/wallet/unfreezebalance");
            string resp = this.RestPostJson(url, reqData);
            TronNetCreateTransactionRestJson restJson = ObjectParse<TronNetCreateTransactionRestJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Returns all resources delegations from an account to another account. 
        /// The fromAddress can be retrieved from the GetDelegatedResourceAccountIndex API.
        /// </summary>
        /// <param name="fromAddress">Energy from address</param>
        /// <param name="toAddress">Energy delegation information</param>
        /// <param name="visible">Optional, Whether the address is in base58 format.</param>
        /// <returns></returns>
        public TronNetDelegatedResourceJson GetDelegatedResource(string fromAddress, string toAddress, bool visible = true)
        {
            if (string.IsNullOrEmpty(fromAddress))
                throw new ArgumentNullException(nameof(fromAddress));
            if (string.IsNullOrEmpty(toAddress))
                throw new ArgumentNullException(nameof(toAddress));

            //variables
            string post_from_address = visible ? fromAddress : TronNetECKey.ConvertToHexAddress(fromAddress);
            string post_to_address = visible ? toAddress : TronNetECKey.ConvertToHexAddress(toAddress);

            //create request data
            dynamic reqData = new
            {
                fromAddress = post_from_address,
                toAddress = post_to_address,
                visible
            };

            string url = CreateFullNodeRestUrl("/wallet/getdelegatedresource");
            string resp = this.RestPostJson(url, reqData);
            TronNetDelegatedResourceJson restJson = ObjectParse<TronNetDelegatedResourceJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Query the energy delegation by an account. 
        /// i.e. list all addresses that have delegated resources to an account.
        /// </summary>
        /// <param name="address">address</param>
        /// <param name="visible">Optional, Whether the address is in base58 format.</param>
        /// <returns></returns>
        public TronNetDelegatedResourceAccountJson GetDelegatedResourceAccountIndex(string address, bool visible = true)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException(nameof(address));

            //variables
            string post_address = visible ? address : TronNetECKey.ConvertToHexAddress(address);

            //create request data
            dynamic reqData = new
            {
                value = post_address,
                visible
            };

            string url = CreateFullNodeRestUrl("/wallet/getdelegatedresourceaccountindex");
            string resp = this.RestPostJson(url, reqData);
            TronNetDelegatedResourceAccountJson restJson = ObjectParse<TronNetDelegatedResourceAccountJson>(resp);

            return restJson;
        }

        #endregion

        #region ITronTransactionsRest

        /// <summary>
        /// Get Transaction Sign
        /// Offline Signature
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="createTransaction"></param>
        /// <param name="visible"></param>
        /// <returns></returns>
        [Obsolete("Remote service has been removed")]
        public TronNetSignedTransactionRestJson GetTransactionSign(string privateKey, TronNetCreateTransactionRestJson createTransaction, bool visible = true)
        {
            throw new NotImplementedException("Remote service has been removed");

            ////if (string.IsNullOrEmpty(privateKey))
            ////    throw new ArgumentNullException(nameof(privateKey));
            ////if (null == createTransaction)
            ////    throw new ArgumentException("createTransaction is null");

            //////create request data
            ////dynamic reqData = new
            ////{
            ////    transaction = new
            ////    {
            ////        //txID = createTransaction.TxID,
            ////        raw_data = createTransaction.RawData,
            ////        //raw_data_hex = createTransaction.RawDataHex,
            ////        visible
            ////    },
            ////    privateKey
            ////};

            ////string url = CreateFullNodeRestUrl("/wallet/gettransactionsign");
            ////string resp = this.RestPostJson(url, reqData);
            ////TronNetSignedTransactionRestJson restJson = ObjectParse<TronNetSignedTransactionRestJson>(resp);

            ////return restJson;
        }

        /// <summary>
        /// Broadcast Transaction
        /// </summary>
        /// <param name="signedTransaction">SignedTransaction Object</param>
        /// <param name="signature">signature</param>
        /// <param name="visible">Optional.Whehter the address is in base58 format</param>
        /// <returns></returns>
        public TronNetResultJson BroadcastTransaction(TronNetSignedTransactionRestJson signedTransaction, string[] signature, bool visible = true)
        {
            throw new NotImplementedException("Remote service has been removed");




            ////create request data
            //dynamic reqData = new
            //{
            //    txID = createTransaction.TxID,
            //    visible,
            //    raw_data = Newtonsoft.Json.JsonConvert.SerializeObject(createTransaction.RawData),
            //    raw_data_hex = createTransaction.RawDataHex,
            //    signature
            //};

            //string url = CreateFullNodeRestUrl("/wallet/broadcasttransaction");
            //string resp = this.RestPostJson(url, reqData);
            //TronNetResultJson restJson = ObjectParse<TronNetResultJson>(resp);

            //return restJson;
        }

        /// <summary>
        /// Broadcast Hex
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        [Obsolete("Remote service has been removed")]
        public TronNetResultJson BroadcastHex(string hex)
        {
            throw new NotImplementedException("Remote service has been removed");
        }

        /// <summary>
        /// Easy Transfer
        /// </summary>
        /// <param name="passPhrase"></param>
        /// <param name="toAddress"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [Obsolete("Remote service has been removed")]
        public TronNetEasyTransferJson EasyTransfer(string passPhrase, string toAddress, ulong amount)
        {
            throw new NotImplementedException("Remote service has been removed");
        }

        /// <summary>
        /// Easy Transfer By Private
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="toAddress"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [Obsolete("Remote service has been removed")]
        public TronNetEasyTransferJson EasyTransferByPrivate(string privateKey, string toAddress, ulong amount)
        {
            throw new NotImplementedException("Remote service has been removed");
        }

        /// <summary>
        /// Create a TRX transfer transaction. 
        /// If to_address does not exist, then create the account on the blockchain.
        /// </summary>
        /// <param name="ownerAddress">To_address is the transfer address</param>
        /// <param name="toAddress">Owner_address is the transfer address</param>
        /// <param name="amount">Amount is the transfer amount,the unit is trx</param>
        /// <param name="permissionID">Optional, for multi-signature use</param>
        /// <param name="visible">Optional.Whehter the address is in base58 format</param>
        /// <returns></returns>
        public TronNetCreateTransactionRestJson CreateTransaction(string ownerAddress, string toAddress, decimal amount, int? permissionID = null, bool visible = true)
        {
            if (string.IsNullOrEmpty(ownerAddress))
                throw new ArgumentNullException(nameof(ownerAddress));
            if (string.IsNullOrEmpty(toAddress))
                throw new ArgumentNullException(nameof(toAddress));
            if (amount <= decimal.Zero)
                throw new ArgumentException("amount must be greater than zero");

            //address to hex
            string post_owner_address = visible ? ownerAddress : TronNetECKey.ConvertToHexAddress(ownerAddress);
            string post_to_address = visible ? toAddress : TronNetECKey.ConvertToHexAddress(toAddress);
            long trx_of_sun = (long)(amount * 1000000);

            //create request data
            dynamic reqData = new
            {
                owner_address = post_owner_address,
                to_address = post_to_address,
                amount = trx_of_sun,
                visible
            };
            if (null != permissionID)
                reqData.permission_id = permissionID.Value;

            string url = CreateFullNodeRestUrl("/wallet/createtransaction");
            string resp = this.RestPostJson(url, reqData);
            TronNetCreateTransactionRestJson restJson = ObjectParse<TronNetCreateTransactionRestJson>(resp);

            return restJson;
        }

        #endregion

        #region ITronQueryNetworkRest

        /// <summary>
        /// Get Block By Number
        /// </summary>
        /// <param name="blockHeight">block number</param>
        /// <param name="visible">Optional,whether the address is in base58 format</param>
        /// <returns></returns>
        public TronNetBlockJson GetBlockByNum(ulong blockHeight, bool visible = true)
        {
            string url = CreateFullNodeRestUrl("/wallet/getblockbynum");
            string resp = this.RestPostJson(url, new { num = blockHeight, visible });
            TronNetBlockJson restJson = ObjectParse<TronNetBlockJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Get Block By Hash(ID)
        /// </summary>
        /// <param name="blockID">block hash</param>
        /// <param name="visible">Optional,whether the address is in base58 format</param>
        /// <returns></returns>
        public TronNetBlockJson GetBlockById(string blockID, bool visible = true)
        {
            if (string.IsNullOrEmpty(blockID))
                throw new ArgumentNullException(nameof(blockID));

            string url = CreateFullNodeRestUrl("/wallet/getblockbyid");
            string resp = this.RestPostJson(url, new { value = blockID, visible });
            TronNetBlockJson restJson = ObjectParse<TronNetBlockJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Get a list of block objects by last blocks
        /// </summary>
        /// <param name="lastNum">Specify the last few blocks</param>
        /// <param name="visible">Optional,whether the address is in base58 format</param>
        /// <returns></returns>
        public TronNetBlockListJson GetBlockByLatestNum(ulong lastNum, bool visible = true)
        {
            string url = CreateFullNodeRestUrl("/wallet/getblockbylatestnum");
            string resp = this.RestPostJson(url, new { num = lastNum, visible });
            TronNetBlockListJson restJson = ObjectParse<TronNetBlockListJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Returns the list of Block Objects included in the 'Block Height' range specified.
        /// </summary>
        /// <param name="startNum">Starting block height, including this block.</param>
        /// <param name="endNum">Ending block height, excluding that block.</param>
        /// <param name="visible">Optional,whether the address is in base58 format</param>
        /// <returns></returns>
        public TronNetBlockListJson GetBlockByLimitNext(ulong startNum, ulong endNum, bool visible = true)
        {
            string url = CreateFullNodeRestUrl("/wallet/getblockbylimitnext");
            string resp = this.RestPostJson(url, new { startNum, endNum, visible });
            TronNetBlockListJson restJson = ObjectParse<TronNetBlockListJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Query the latest block information
        /// </summary>
        /// <param name="visible">Optional,whether the address is in base58 format</param>
        /// <returns></returns>
        public TronNetBlockDetailsJson GetNowBlock(bool visible = true)
        {
            string url = CreateFullNodeRestUrl(string.Format("/wallet/getnowblock?visible={0}", visible.ToString().ToLower()));
            string resp = this.RestGetJson(url);
            TronNetBlockDetailsJson restJson = ObjectParse<TronNetBlockDetailsJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Get Transaction By Txid
        /// </summary>
        /// <param name="txid">txid</param>
        /// <param name="visible">Optional,whether the address is in base58 format</param>
        /// <returns></returns>
        public TronNetTransactionRestJson GetTransactionByID(string txid, bool visible = true)
        {
            if (string.IsNullOrEmpty(txid))
                throw new ArgumentNullException(nameof(txid));

            string url = CreateFullNodeRestUrl("/wallet/gettransactionbyid");
            string resp = this.RestPostJson(url, new { value = txid, visible });
            TronNetTransactionRestJson restJson = ObjectParse<TronNetTransactionRestJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Get Transaction Info By Txid
        /// </summary>
        /// <param name="txid">txid</param>
        /// <param name="visible">Optional,whether the address is in base58 format</param>
        /// <returns></returns>
        public TronNetTransactionInfoJson GetTransactionInfoById(string txid, bool visible = true)
        {
            if (string.IsNullOrEmpty(txid))
                throw new ArgumentNullException(nameof(txid));

            string url = CreateFullNodeRestUrl("/wallet/gettransactioninfobyid");
            string resp = this.RestPostJson(url, new { value = txid, visible });
            TronNetTransactionInfoJson restJson = ObjectParse<TronNetTransactionInfoJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Get TransactionInfo By BlockHeight
        /// </summary>
        /// <param name="blockHeight">block number</param>
        /// <param name="visible">Optional,whether the address is in base58 format</param>
        /// <returns></returns>
        [System.Obsolete("Please use method 'GetBlockByLimitNext' instead")]
        public TronNetTransactionInfoJson GetTransactionInfoByBlockNum(ulong blockHeight, bool visible = true)
        {
            throw new NotImplementedException("Please use method 'GetBlockByLimitNext' instead");
        }

        /// <summary>
        /// Return List of Nodes
        /// </summary>
        /// <returns></returns>
        public TronNetNodeJson ListNodes()
        {
            string url = CreateFullNodeRestUrl("/wallet/listnodes");
            string resp = this.RestGetJson(url);
            TronNetNodeJson restJson = ObjectParse<TronNetNodeJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Get NodeInfo
        /// </summary>
        /// <returns></returns>
        public TronNetNodeOverviewJson GetNodeInfo()
        {
            string url = CreateFullNodeRestUrl("/wallet/getnodeinfo");
            string resp = this.RestGetJson(url);
            TronNetNodeOverviewJson restJson = ObjectParse<TronNetNodeOverviewJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Get Chain Parameters
        /// </summary>
        /// <returns></returns>
        public TronNetChainParameterOverviewJson GetChainParameters()
        {
            string url = CreateFullNodeRestUrl("/wallet/getchainparameters");
            string resp = this.RestGetJson(url);
            TronNetChainParameterOverviewJson restJson = ObjectParse<TronNetChainParameterOverviewJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Get Block's Account Balance Change
        /// 47.241.20.47 & 161.117.85.97 &161.117.224.116 &161.117.83.38
        /// </summary>
        /// <param name="blockHash"></param>
        /// <param name="blockHeight"></param>
        /// <param name="visible"></param>
        /// <returns></returns>
        public TronNetBlockBalanceJson GetBlockBalance(string blockHash, ulong blockHeight, bool visible = true)
        {
            if (string.IsNullOrEmpty(blockHash))
                throw new ArgumentNullException(nameof(blockHash));
            if (blockHeight <= 0)
                throw new ArgumentException("blockHeight must be greater than zero");

            string url = CreateSuperNodeRestUrl("/wallet/getblockbalance");
            string resp = this.RestPostJson(url, new { hash = blockHash, number = blockHeight, visible });
            TronNetBlockBalanceJson restJson = ObjectParse<TronNetBlockBalanceJson>(resp);

            return restJson;
        }

        #endregion

        #region ITronNetTRC10TokenRest

        /// <summary>
        /// Get Asset Issue By Account
        /// </summary>
        /// <param name="tronAddress"></param>
        /// <param name="visible"></param>
        /// <returns></returns>
        public TronNetAssetCollectionJson GetAssetIssueByAccount(string tronAddress, bool visible = true)
        {
            string address = visible ? tronAddress : TronNetECKey.ConvertToHexAddress(tronAddress);

            //create request data
            dynamic reqData = new
            {
                address,
                visible
            };

            string url = CreateFullNodeRestUrl("/wallet/getassetissuebyaccount");
            string resp = this.RestPostJson(url, reqData);
            TronNetAssetCollectionJson restJson = ObjectParse<TronNetAssetCollectionJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Get AssetIssue By Id
        /// </summary>
        /// <param name="assertID">asset id</param>
        /// <param name="visible">Optional,whether the address is in base58 format</param>
        /// <returns></returns>
        public TronNetAssetInfoJson GetAssetIssueById(int assertID, bool visible = true)
        {
            string url = CreateFullNodeRestUrl("/wallet/getassetissuebyid");
            string resp = this.RestPostJson(url, new { value = assertID, visible });
            TronNetAssetInfoJson restJson = ObjectParse<TronNetAssetInfoJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Get AssetIssue List
        /// </summary>
        /// <param name="visible">Optional,whether the address is in base58 format</param>
        /// <returns></returns>
        public TronNetAssetCollectionJson GetAssetIssueList(bool visible = true)
        {
            string url = CreateFullNodeRestUrl(string.Format("/wallet/getassetissuelist?visible={0}", visible.ToString().ToLower()));
            string resp = this.RestGetJson(url);
            TronNetAssetCollectionJson restJson = ObjectParse<TronNetAssetCollectionJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Get Paginated AssentIssue LIst
        /// </summary>
        /// <param name="offset">page offset</param>
        /// <param name="limit">page limit</param>
        /// <param name="visible">Optional,whether the address is in base58 format</param>
        /// <returns></returns>
        public TronNetAssetCollectionJson GetPaginatedAssetIssueList(int offset, int limit, bool visible = true)
        {
            string url = CreateFullNodeRestUrl("/wallet/getpaginatedassetissuelist");
            string resp = this.RestPostJson(url, new { offset, limit, visible });
            TronNetAssetCollectionJson restJson = ObjectParse<TronNetAssetCollectionJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Transfer Asset
        /// </summary>
        /// <param name="ownerAddress">Owner address</param>
        /// <param name="toAddress">receiving address</param>
        /// <param name="assetName">Token id</param>
        /// <param name="amount">amount</param>
        /// <param name="permission_id">Optional, for multi-signature use</param>
        /// <param name="visible">Optional, Whether the address is in base58 format.</param>
        /// <returns></returns>
        public TronNetCreateTransactionRestJson TransferAsset(string ownerAddress, string toAddress, string assetName, ulong amount, int? permissionID = null, bool visible = true)
        {
            if (string.IsNullOrEmpty(ownerAddress))
                throw new ArgumentNullException(nameof(ownerAddress));
            if (string.IsNullOrEmpty(toAddress))
                throw new ArgumentNullException(nameof(toAddress));
            if (string.IsNullOrEmpty(assetName))
                throw new ArgumentNullException(nameof(assetName));
            if (amount <= decimal.Zero)
                throw new ArgumentException("amount must be greater than 0");

            //variables
            string post_owner_address = visible ? ownerAddress : TronNetECKey.ConvertToHexAddress(ownerAddress);
            string post_to_address = visible ? toAddress : TronNetECKey.ConvertToHexAddress(toAddress);
            bool has_asset_id = int.TryParse(assetName, out _);
            if (!has_asset_id)
                throw new ArgumentException("assert name error");

            //create request data
            dynamic reqData = new
            {
                owner_address = post_owner_address,
                to_address = post_to_address,
                asset_name = assetName,
                amount,
                visible
            };
            if (null != permissionID)
                reqData.permission_id = permissionID.Value;

            string url = CreateFullNodeRestUrl("/wallet/transferasset");
            string resp = this.RestPostJson(url, reqData);
            TronNetCreateTransactionRestJson restJson = ObjectParse<TronNetCreateTransactionRestJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Create AssetIssue
        /// </summary>
        /// <param name="ownerAddress">Owner Address</param>
        /// <param name="tokenName">Token Name</param>
        /// <param name="tokenPrecision">Token Precision</param>
        /// <param name="tokenAbbr">Token Abbr</param>
        /// <param name="totalSupply">Token Total Supply</param>
        /// <param name="trxNum">
        /// Define the price by the ratio of trx_num/num(The unit of 'trx_num' is SUN)
        /// </param>
        /// <param name="num">
        /// Define the price by the ratio of trx_num/num
        /// </param>
        /// <param name="startTime">ICO StartTime</param>
        /// <param name="endTime">ICO EndTime</param>
        /// <param name="tokenDescription">Token Description</param>
        /// <param name="tokenUrl">Token Official Website Url</param>
        /// <param name="freeAssetNetLimit">Token Free Asset Net Limit</param>
        /// <param name="publicFreeAssetNetLimit">Token Public Free Asset Net Limit</param>
        /// <param name="frozenSupply">Token Frozen Supply</param>
        /// <param name="visible">Optional, Whether the address is in base58 format.</param>
        /// <returns></returns>
        public TronNetCreateTransactionRestJson CreateAssetIssue(string ownerAddress, string tokenName, int tokenPrecision, string tokenAbbr, ulong totalSupply, ulong trxNum, ulong num, DateTime startTime, DateTime endTime, string tokenDescription, string tokenUrl, ulong freeAssetNetLimit, ulong publicFreeAssetNetLimit, TronNetFrozenSupplyJson frozenSupply, bool visible = true)
        {
            if (string.IsNullOrEmpty(ownerAddress))
                throw new ArgumentNullException(nameof(ownerAddress));
            if (string.IsNullOrEmpty(tokenName))
                throw new ArgumentNullException(nameof(tokenName));
            if (tokenPrecision <= 0)
                tokenPrecision = 0;
            if (string.IsNullOrEmpty(tokenAbbr))
                throw new ArgumentNullException(nameof(tokenAbbr));
            if (totalSupply <= 0)
                throw new ArgumentException("'totalSupply' must be greater to zero");
            if (string.IsNullOrEmpty(tokenUrl))
                tokenUrl = string.Empty;
            if (string.IsNullOrEmpty(tokenDescription))
                tokenDescription = string.Empty;
            if (null == frozenSupply)
                throw new ArgumentNullException(nameof(frozenSupply));

            string post_owner_address = visible ? ownerAddress : TronNetECKey.ConvertToHexAddress(ownerAddress);
            string hex_token_name = TronNetUntils.StringToHexString(tokenName, true, Encoding.UTF8);
            string hex_token_abbr = TronNetUntils.StringToHexString(tokenAbbr, true, Encoding.UTF8);
            long start_time = TronNetUntils.LocalDatetimeToMillisecondTimestamp(startTime);
            long end_time = TronNetUntils.LocalDatetimeToMillisecondTimestamp(endTime);

            string hex_token_desc = TronNetUntils.StringToHexString(tokenDescription, true, Encoding.UTF8);
            string hex_url = TronNetUntils.StringToHexString(tokenUrl, true, Encoding.UTF8);

            //create request data
            dynamic reqData = new
            {
                owner_address = post_owner_address,
                name = hex_token_name,
                precision = tokenPrecision,
                abbr = hex_token_abbr,
                total_supply = totalSupply,
                trx_num = trxNum,
                num,
                start_time,
                end_time,
                description = hex_token_desc,
                url = hex_url,
                free_asset_net_limit = freeAssetNetLimit,
                public_free_asset_net_limit = publicFreeAssetNetLimit,
                frozen_supply = frozenSupply,
                visible
            };

            string url = CreateFullNodeRestUrl("/wallet/createassetissue");
            string resp = this.RestPostJson(url, reqData);
            TronNetCreateTransactionRestJson restJson = ObjectParse<TronNetCreateTransactionRestJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Participate AssetIssue
        /// </summary>
        /// <param name="toAddress">to address</param>
        /// <param name="ownerAddress">owner address</param>
        /// <param name="amount">amount</param>
        /// <param name="assetName">asset name</param>
        /// <param name="visible">Optional, Whether the address is in base58 format.</param>
        /// <returns></returns>
        public TronNetCreateTransactionRestJson ParticipateAssetIssue(string toAddress, string ownerAddress, ulong amount, string assetName, bool visible = true)
        {
            if (string.IsNullOrEmpty(toAddress))
                throw new ArgumentNullException(nameof(toAddress));
            if (string.IsNullOrEmpty(ownerAddress))
                throw new ArgumentNullException(nameof(ownerAddress));
            if (string.IsNullOrEmpty(assetName))
                throw new ArgumentNullException(nameof(assetName));
            if (amount <= decimal.Zero)
                throw new ArgumentException("amount must be greater than 0");

            //variables
            string post_owner_address = visible ? ownerAddress : TronNetECKey.ConvertToHexAddress(ownerAddress);
            string post_to_address = visible ? toAddress : TronNetECKey.ConvertToHexAddress(toAddress);
            bool has_asset_id = int.TryParse(assetName, out _);
            if (!has_asset_id)
                throw new ArgumentException("assert name error");

            //create request data
            dynamic reqData = new
            {
                owner_address = post_owner_address,
                to_address = post_to_address,
                asset_name = assetName,
                amount,
                visible
            };

            string url = CreateFullNodeRestUrl("/wallet/participateassetissue");
            string resp = this.RestPostJson(url, reqData);
            TronNetCreateTransactionRestJson restJson = ObjectParse<TronNetCreateTransactionRestJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Unstake a token that has passed the minimum freeze duration.
        /// </summary>
        /// <param name="ownerAddress">owner address</param>
        /// <param name="permissionID">permission id</param>
        /// <param name="visible">Optional, Whether the address is in base58 format.</param>
        /// <returns></returns>
        public TronNetCreateTransactionRestJson UnfreezeAsset(string ownerAddress, int? permissionID = null, bool visible = true)
        {
            if (string.IsNullOrEmpty(ownerAddress))
                throw new ArgumentNullException(nameof(ownerAddress));

            //variables
            string post_owner_address = visible ? ownerAddress : TronNetECKey.ConvertToHexAddress(ownerAddress);

            //create request data
            dynamic reqData = new
            {
                owner_address = post_owner_address,
            };
            if (null != permissionID)
                reqData.permission_id = permissionID.Value;

            string url = CreateFullNodeRestUrl("/wallet/unfreezeasset");
            string resp = this.RestPostJson(url, reqData);
            TronNetCreateTransactionRestJson restJson = ObjectParse<TronNetCreateTransactionRestJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Update basic TRC10 token information.
        /// </summary>
        /// <param name="ownerAddress"></param>
        /// <param name="tokenDescription"></param>
        /// <param name="tokenUrl"></param>
        /// <param name="newLimit"></param>
        /// <param name="newPublicLimit"></param>
        /// <param name="permissionID"></param>
        /// <param name="visible"></param>
        /// <returns></returns>
        public TronNetCreateTransactionRestJson UpdateAsset(string ownerAddress, string tokenDescription, string tokenUrl, int newLimit, int newPublicLimit, int? permissionID, bool visible = true)
        {
            if (string.IsNullOrEmpty(ownerAddress))
                throw new ArgumentNullException(nameof(ownerAddress));
            if (string.IsNullOrEmpty(tokenDescription))
                throw new ArgumentNullException(nameof(tokenDescription));
            if (string.IsNullOrEmpty(tokenUrl))
                throw new ArgumentNullException(nameof(tokenUrl));

            string post_owner_address = visible ? ownerAddress : TronNetECKey.ConvertToHexAddress(ownerAddress);
            string hex_token_desc = TronNetUntils.StringToHexString(tokenDescription, true, Encoding.UTF8);
            string hex_url = TronNetUntils.StringToHexString(tokenUrl, true, Encoding.UTF8);

            //create request data
            dynamic reqData = new
            {
                owner_address = post_owner_address,
                description = hex_token_desc,
                url = hex_url,
                free_asset_net_limit = newLimit,
                public_free_asset_net_limit = newPublicLimit,
                visible
            };
            if (null != permissionID)
                reqData.permission_id = permissionID.Value;

            string url = CreateFullNodeRestUrl("/wallet/updateasset");
            string resp = this.RestPostJson(url, reqData);
            TronNetCreateTransactionRestJson restJson = ObjectParse<TronNetCreateTransactionRestJson>(resp);

            return restJson;
        }

        /// <summary>
        /// Easy TRC10 token transfer. Create a TRC10 transfer transaction and broadcast directly.
        /// </summary>
        /// <param name="passPhrase"></param>
        /// <param name="toAddress"></param>
        /// <param name="assetId"></param>
        /// <param name="amount"></param>
        /// <param name="visible"></param>
        /// <returns></returns>
        [Obsolete("Remote service has been removed")]
        public TronNetCreateTransactionRestJson EasyTransferAsset(string passPhrase, string toAddress, string assetId, ulong amount, bool visible = true)
        {
            throw new NotImplementedException("Remote service has been removed");
        }

        /// <summary>
        /// TRC10 token easy transfer. Broadcast the created transaction directly.
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="toAddress"></param>
        /// <param name="assetId"></param>
        /// <param name="amount"></param>
        /// <param name="visible"></param>
        /// <returns></returns>
        [Obsolete("Remote service has been removed")]
        public TronNetCreateTransactionRestJson EasyTransferAssetByPrivate(string privateKey, string toAddress, string assetId, ulong amount, bool visible = true)
        {
            throw new NotImplementedException("Remote service has been removed");
        }

        #endregion

        #region ITronNetSmartContractsRest

        /// <summary>
        /// Queries a contract's information from the blockchain. Returns SmartContract object.
        /// </summary>
        /// <param name="contractAddress">Contract address</param>
        /// <param name="visible">Optional, is address in visible format(base58check) or hex?</param>
        /// <returns></returns>
        public TronNetContractMetaDataJson GetContract(string contractAddress, bool visible = true)
        {
            if (string.IsNullOrEmpty(contractAddress))
                throw new ArgumentNullException(nameof(contractAddress));

            //variables
            string value = visible ? contractAddress : TronNetECKey.ConvertToHexAddress(contractAddress);

            //create request data
            dynamic reqData = new
            {
                value,
                visible
            };

            string url = CreateFullNodeRestUrl("/wallet/getcontract");
            string resp = this.RestPostJson(url, reqData);
            TronNetContractMetaDataJson restJson = ObjectParse<TronNetContractMetaDataJson>(resp);

            return restJson;
        }

        #endregion

        #region ITronNetOffLineRest

        /// <summary>
        /// Get Transaction Sign
        /// Offline Signature
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="createTransaction"></param>
        /// <returns></returns>
        public TronNetSignedTransactionRestJson GetTransactionOffLineSign(string privateKey, TronNetCreateTransactionRestJson createTransaction)
        {
            if (string.IsNullOrEmpty(privateKey))
                throw new ArgumentNullException(nameof(privateKey));
            if (null == createTransaction)
                throw new ArgumentException("createTransaction is null");

            /* Restore ECKey From Private Key */
            ECKey ecKey = new ECKey(privateKey.HexToByteArray(), true);

            /* hash sign */
            string raw_json = Newtonsoft.Json.JsonConvert.SerializeObject(createTransaction.RawData);
            byte[] raw_data_bytes = Encoding.UTF8.GetBytes(raw_json);
            byte[] hash_bytes = raw_data_bytes.ToSHA256Hash();
            byte[] sign_bytes = ecKey.Sign(hash_bytes).ToByteArray();
            string sign = sign_bytes.ToHex();

            TronNetSignedTransactionRestJson restJson = new TronNetSignedTransactionRestJson()
            {
                TxID = createTransaction.TxID,
                RawData = createTransaction.RawData,
                RawDataHex = createTransaction.RawDataHex,
                Visible = createTransaction.Visible,
                Signature = new string[] { sign }
            };
            restJson.Signature = new string[] { sign };
            return restJson;
        }

        #endregion
    }
}
