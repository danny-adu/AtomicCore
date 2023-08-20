using System;
using Microsoft.Extensions.DependencyInjection;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Contract Client Factory Interface Implementation Class
    /// </summary>
    public class ContractClientFactory : IContractClientFactory
    {
        #region Variables

        /// <summary>
        /// Service Provider
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceProvider"></param>
        public ContractClientFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create Client Instance
        /// </summary>
        /// <param name="protocol"></param>
        /// <returns></returns>
        public IContractClient CreateClient(ContractProtocol protocol)
        {
            IContractClient client;
            switch (protocol)
            {
                case ContractProtocol.TRC20:
                    client = _serviceProvider.GetService<TRC20ContractClient>();
                    break;
                default:
                    throw new NotImplementedException();
            }

            return client;
        }

        #endregion
    }
}
