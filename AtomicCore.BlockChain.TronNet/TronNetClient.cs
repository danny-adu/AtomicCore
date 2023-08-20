using Microsoft.Extensions.Options;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// ITronClient Interface Impl
    /// </summary>
    public class TronNetClient : ITronNetClient
    {
        #region Variables

        private readonly IOptions<TronNetOptions> _options;
        private readonly ITronNetRest _restApiClient;
        private readonly ITronGridRest _gridApiClient;
        private readonly IGrpcChannelClient _channelClient;
        private readonly ITronNetWalletClient _walletClient;
        private readonly ITronNetTransactionClient _transactionClient;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        /// <param name="restApiClient"></param>
        /// <param name="gridApiClient"></param>
        /// <param name="channelClient"></param>
        /// <param name="walletClient"></param>
        /// <param name="transactionClient"></param>
        public TronNetClient(
            IOptions<TronNetOptions> options,
            ITronNetRest restApiClient,
            ITronGridRest gridApiClient,
            IGrpcChannelClient channelClient,
            ITronNetWalletClient walletClient,
            ITronNetTransactionClient transactionClient
        )
        {
            _options = options;
            _restApiClient = restApiClient;
            _gridApiClient = gridApiClient;
            _channelClient = channelClient;
            _walletClient = walletClient;
            _transactionClient = transactionClient;
        }

        #endregion

        #region Propertys

        /// <summary>
        /// Tron Network Type Enum
        /// </summary>
        public TronNetwork TronNetwork => _options.Value.Network;

        #endregion

        #region Public Methods

        /// <summary>
        /// Grpc Channel Client
        /// </summary>
        /// <returns></returns>
        public IGrpcChannelClient GetChannel()
        {
            return _channelClient;
        }

        /// <summary>
        /// Get Wallet Interface Instance
        /// </summary>
        /// <returns></returns>
        public ITronNetWalletClient GetWallet()
        {
            return _walletClient;
        }

        /// <summary>
        /// Get Transaction Interface Instance
        /// </summary>
        /// <returns></returns>
        public ITronNetTransactionClient GetTransaction()
        {
            return _transactionClient;
        }

        /// <summary>
        /// Get Rest API
        /// </summary>
        /// <returns></returns>
        public ITronNetRest GetRestAPI()
        {
            return _restApiClient;
        }

        /// <summary>
        /// Get Grid Api
        /// </summary>
        /// <returns></returns>
        public ITronGridRest GetGridAPI()
        {
            return _gridApiClient;
        }

        #endregion
    }
}
