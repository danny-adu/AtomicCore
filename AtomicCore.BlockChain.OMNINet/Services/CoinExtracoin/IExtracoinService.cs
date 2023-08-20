namespace AtomicCore.BlockChain.OMNINet
{
    /// <summary>
    /// 额外拓展币种接口定义（Danny）
    /// </summary>
    public interface IExtracoinService : ICoinService, IExtracoinConstants
    {
        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        GetInfoResponse2 GetInfo2();

        /// <summary>
        /// 获取各种难度系数
        /// </summary>
        /// <returns></returns>
        GetDifficulty2Response GetDifficulty2();

        /// <summary>
        /// 获取区块信息
        /// </summary>
        /// <param name="blockHash"></param>
        /// <returns></returns>
        GetBlockResponse2 GetBlock2(string blockHash);

        /// <summary>
        /// 根据索引获取数据块
        /// </summary>
        /// <param name="number"></param>
        GetBlockByNumberResponse GetBlockByNumber(uint number);

        /// <summary>
        /// 根据账单ID或账单信息
        /// </summary>
        /// <param name="txId"></param>
        /// <returns></returns>
        GetTransactionResponse2 GetTransaction2(string txId);

        /// <summary>
        /// 获取内存池
        /// </summary>
        /// <returns></returns>
        GetRawMemPoolResponse[] GetRawMemPool2();

        /// <summary>
        /// 导入地址私匙
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        string ImportPrivKey2(string privateKey, string label);
    }
}
