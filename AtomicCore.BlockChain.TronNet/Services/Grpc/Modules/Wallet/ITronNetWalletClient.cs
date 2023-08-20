using Google.Protobuf;
using Grpc.Core;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Wallet Client Interface
    /// </summary>
    public interface ITronNetWalletClient
    {
        /// <summary>
        /// Get Protocol
        /// </summary>
        /// <returns></returns>
        Wallet.WalletClient GetProtocol();

        /// <summary>
        /// Get Solidity Protocol
        /// </summary>
        /// <returns></returns>
        WalletSolidity.WalletSolidityClient GetSolidityProtocol();

        /// <summary>
        /// Generatee Account
        /// </summary>
        /// <returns></returns>
        ITronNetAccount GenerateAccount();

        /// <summary>
        /// Get Account From PrivateKey HexString
        /// </summary>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        ITronNetAccount GetAccount(string privateKey);

        /// <summary>
        /// Parse Address
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        ByteString ParseAddress(string address);

        /// <summary>
        /// Get Metadata Headers
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        Metadata GetHeaders(string apiKey = null);
    }
}
