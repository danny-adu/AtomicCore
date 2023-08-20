namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Grpc Channel Client Interface
    /// </summary>
    public interface IGrpcChannelClient
    {
        /// <summary>
        /// Get Protocol
        /// </summary>
        /// <returns></returns>
        Grpc.Core.Channel GetProtocol();

        /// <summary>
        /// Get Solidity Protocol
        /// </summary>
        /// <returns></returns>
        Grpc.Core.Channel GetSolidityProtocol();
    }
}
