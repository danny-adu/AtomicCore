// Copyright (c) 2014 George Kimionis
// Distributed under the GPLv3 software license, see the accompanying file LICENSE or http://opensource.org/licenses/GPL-3.0

namespace AtomicCore.BlockChain.OMNINet
{
    public interface IRpcConnector
    {
        T MakeRequest<T>(string method, params object[] parameters);

        T MakeRequest<T>(RpcMethods method, params object[] parameters);
    }
}