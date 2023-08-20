using Grpc.Core;
using Microsoft.Extensions.Options;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Grpc Channel Client
    /// </summary>
    public class GrpcChannelClient : IGrpcChannelClient
    {
        #region Variables

        private readonly IOptions<TronNetOptions> _options;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public GrpcChannelClient(IOptions<TronNetOptions> options)
        {
            _options = options;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get Protocol
        /// </summary>
        /// <returns></returns>
        public Channel GetProtocol()
        {
            return new Channel(
                _options.Value.Channel.Host,
                _options.Value.Channel.Port,
                ChannelCredentials.Insecure
            );
        }

        /// <summary>
        /// Get Solidity Protocol
        /// </summary>
        /// <returns></returns>
        public Channel GetSolidityProtocol()
        {
            return new Channel(
                _options.Value.SolidityChannel.Host,
                _options.Value.SolidityChannel.Port, 
                ChannelCredentials.Insecure
            );
        }

        #endregion
    }
}
