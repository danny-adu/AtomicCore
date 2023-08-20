using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Options;
using System;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Wallet Client Implementation Class
    /// </summary>
    public class TronNetWalletClient : ITronNetWalletClient
    {
        #region Variables

        private readonly IGrpcChannelClient _channelClient;
        private readonly IOptions<TronNetOptions> _options;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="channelClient"></param>
        /// <param name="options"></param>
        public TronNetWalletClient(IGrpcChannelClient channelClient, IOptions<TronNetOptions> options)
        {
            _channelClient = channelClient;
            _options = options;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get Protocol
        /// </summary>
        /// <returns></returns>
        public Wallet.WalletClient GetProtocol()
        {
            Channel channel = _channelClient.GetProtocol();
            Wallet.WalletClient wallet = new Wallet.WalletClient(channel);

            return wallet;
        }

        /// <summary>
        /// Generate Account
        /// </summary>
        /// <returns></returns>
        public ITronNetAccount GenerateAccount()
        {
            TronNetECKey tronKey = TronNetECKey.GenerateKey(_options.Value.Network);

            return new TronNetAccount(tronKey);
        }

        /// <summary>
        /// Get Account
        /// </summary>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public ITronNetAccount GetAccount(string privateKey)
        {
            return new TronNetAccount(privateKey, _options.Value.Network);
        }

        /// <summary>
        /// Get Solidity Protocol
        /// </summary>
        /// <returns></returns>
        public WalletSolidity.WalletSolidityClient GetSolidityProtocol()
        {
            Channel channel = _channelClient.GetSolidityProtocol();
            WalletSolidity.WalletSolidityClient wallet = new WalletSolidity.WalletSolidityClient(channel);

            return wallet;
        }

        /// <summary>
        /// Parse Address
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public ByteString ParseAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address)) throw new ArgumentNullException(nameof(address));

            byte[] raw;
            if (address.StartsWith("T"))
                raw = Base58Encoder.DecodeFromBase58Check(address);
            else if (address.StartsWith("41"))
                raw = address.HexToByteArray();
            else if (address.StartsWith("0x"))
                raw = address.Substring(2).HexToByteArray();
            else
            {
                try
                {
                    raw = address.HexToByteArray();
                }
                catch (Exception)
                {
                    throw new ArgumentException($"Invalid address: " + address);
                }
            }

            return ByteString.CopyFrom(raw);
        }

        /// <summary>
        /// Get Metadata Headers
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public Metadata GetHeaders(string apiKey = null)
        {
            Metadata headers;
            if (string.IsNullOrEmpty(apiKey))
                headers = new Metadata
                {
                    { "TRON-PRO-API-KEY", _options.Value.ApiKey }
                };
            else
                headers = new Metadata
                {
                    { "TRON-PRO-API-KEY", apiKey }
                };

            return headers;
        }

        #endregion
    }
}
