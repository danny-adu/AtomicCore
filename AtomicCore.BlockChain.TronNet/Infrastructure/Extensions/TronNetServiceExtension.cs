using System;
using Microsoft.Extensions.DependencyInjection;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Tron Net Service Extension
    /// </summary>
    public static class TronNetServiceExtension
    {
        /// <summary>
        /// Register TronNet Interface
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setupAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddTronNet(this IServiceCollection services, Action<TronNetOptions> setupAction)
        {
            //Load options
            TronNetOptions options = new TronNetOptions();
            setupAction(options);

            //Register REST Interface
            services.AddTransient<ITronNetRest, TronNetRest>();
            services.AddTransient<ITronNetAccountResourcesRest, TronNetRest>();
            services.AddTransient<ITronNetAccountRest, TronNetRest>();
            services.AddTransient<ITronNetAddressUtilitiesRest, TronNetRest>();
            services.AddTransient<ITronNetQueryNetworkRest, TronNetRest>();
            services.AddTransient<ITronNetTransactionsRest, TronNetRest>();
            services.AddTransient<ITronNetTRC10TokenRest, TronNetRest>();
            services.AddTransient<ITronNetSmartContractsRest, TronNetRest>();

            //Register GRID Interface
            services.AddTransient<ITronGridRest, TronGridRest>();
            services.AddTransient<ITronGridAccountRest, TronGridRest>();
            services.AddTransient<ITronGridTRC10Rest, TronGridRest>();
            services.AddTransient<ITronGridContractRest, TronGridRest>();
            services.AddTransient<ITronGridEventRest, TronGridRest>();

            //Register Other Interface
            services.AddTransient<ITronNetTransactionClient, TronNetTransactionClient>();
            services.AddTransient<IGrpcChannelClient, GrpcChannelClient>();
            services.AddTransient<ITronNetClient, TronNetClient>();
            services.AddTransient<ITronNetWalletClient, TronNetWalletClient>();
            services.AddSingleton<IContractClientFactory, ContractClientFactory>();
            services.AddTransient<TRC20ContractClient>();
            services.Configure(setupAction);

            return services;
        }
    }
}
