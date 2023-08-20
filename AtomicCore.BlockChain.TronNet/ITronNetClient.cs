namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Tron Client Interface
    /// </summary>
    public interface ITronNetClient
    {
        /// <summary>
        /// Tron Network Type Enum
        /// </summary>
        TronNetwork TronNetwork { get; }

        /// <summary>
        /// Get Rest Api
        /// </summary>
        /// <returns></returns>
        ITronNetRest GetRestAPI();

        /// <summary>
        /// Get Grid Api
        /// </summary>
        /// <returns></returns>
        ITronGridRest GetGridAPI();

        /// <summary>
        /// Grpc Channel Client
        /// </summary>
        /// <returns></returns>
        IGrpcChannelClient GetChannel();

        /// <summary>
        /// Get Wallet Interface Instance
        /// </summary>
        /// <returns></returns>
        ITronNetWalletClient GetWallet();

        /// <summary>
        /// Get Transaction Interface Instance
        /// </summary>
        /// <returns></returns>
        ITronNetTransactionClient GetTransaction();
    }
}
