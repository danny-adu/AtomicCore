// Copyright (c) 2014 George Kimionis
// Distributed under the GPLv3 software license, see the accompanying file LICENSE or http://opensource.org/licenses/GPL-3.0

namespace AtomicCore.BlockChain.OMNINet
{
    /// <summary>
    /// Coin Service Extension Methods
    /// </summary>
    public static class CoinServiceExtensionMethods
    {
        /// <summary>
        /// SwitchNetworks
        /// </summary>
        /// <param name="coinService"></param>
        public static void SwitchNetworks(this ICoinService coinService)
        {
            coinService.Parameters.UseTestnet = !coinService.Parameters.UseTestnet;
        }
    }
}