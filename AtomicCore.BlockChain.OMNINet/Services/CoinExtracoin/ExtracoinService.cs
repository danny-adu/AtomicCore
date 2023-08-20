namespace AtomicCore.BlockChain.OMNINet
{
    /// <summary>
    /// 额外币服务类
    /// </summary>
    public class ExtracoinService : CoinService, IExtracoinService
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="useTestnet">是否使用测试线路</param>
        public ExtracoinService(bool useTestnet = false)
            : base(useTestnet)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="daemonUrl">服务节点地址</param>
        /// <param name="rpcUsername">RPC接口用户名</param>
        /// <param name="rpcPassword">RPC接口密码</param>
        /// <param name="walletPassword">钱包密码</param>
        public ExtracoinService(string daemonUrl, string rpcUsername, string rpcPassword, string walletPassword)
            : base(daemonUrl, rpcUsername, rpcPassword, walletPassword)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="daemonUrl">服务节点地址</param>
        /// <param name="rpcUsername">RPC接口用户名</param>
        /// <param name="rpcPassword">RPC接口密码</param>
        /// <param name="walletPassword">钱包密码</param>
        /// <param name="rpcRequestTimeoutInSeconds">RPC请求超时时间</param>
        public ExtracoinService(string daemonUrl, string rpcUsername, string rpcPassword, string walletPassword, short rpcRequestTimeoutInSeconds)
            : base(daemonUrl, rpcUsername, rpcPassword, walletPassword, rpcRequestTimeoutInSeconds)
        {
        }

        /// <summary>
        /// 常量定义（继续点，有很多）
        /// </summary>
        public ExtracoinConstants.Constants Constants
        {
            get { return ExtracoinConstants.Constants.Instance; }
        }

        /// <summary>
        /// 先版获取各种难以程度
        /// </summary>
        public GetDifficulty2Response GetDifficulty2()
        {
            return this._rpcConnector.MakeRequest<GetDifficulty2Response>(RpcMethods.getdifficulty);
        }

        /// <summary>
        /// 获取区块信息
        /// </summary>
        /// <param name="blockHash"></param>
        /// <returns></returns>
        public GetBlockResponse2 GetBlock2(string blockHash)
        {
            return this._rpcConnector.MakeRequest<GetBlockResponse2>(RpcMethods.getblock, blockHash);
        }

        /// <summary>
        /// 根据区块索引获取区块信息
        /// </summary>
        /// <param name="number"></param>
        public GetBlockByNumberResponse GetBlockByNumber(uint number)
        {
            return this._rpcConnector.MakeRequest<GetBlockByNumberResponse>("getblockbynumber", number);
        }


        public GetTransactionResponse2 GetTransaction2(string txId)
        {
            return this._rpcConnector.MakeRequest<GetTransactionResponse2>("gettransaction", txId);
        }

        public GetInfoResponse2 GetInfo2()
        {
            return this._rpcConnector.MakeRequest<GetInfoResponse2>(RpcMethods.getinfo);
        }

        public GetRawMemPoolResponse[] GetRawMemPool2()
        {
            return this._rpcConnector.MakeRequest<GetRawMemPoolResponse[]>(RpcMethods.getrawmempool);
        }

        public string ImportPrivKey2(string privateKey, string label)
        {
            return _rpcConnector.MakeRequest<string>(RpcMethods.importprivkey, privateKey, label);
        }
    }
}
