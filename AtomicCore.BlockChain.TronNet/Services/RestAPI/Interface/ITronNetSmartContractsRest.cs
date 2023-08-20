namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Smart contract related APIs. Calling, triggering, or updating settings.
    /// </summary>
    public interface ITronNetSmartContractsRest
    {
        /// <summary>
        /// Queries a contract's information from the blockchain. Returns SmartContract object.
        /// </summary>
        /// <param name="contractAddress">Contract address</param>
        /// <param name="visible">Optional, is address in visible format(base58check) or hex?</param>
        /// <returns></returns>
        TronNetContractMetaDataJson GetContract(string contractAddress, bool visible = true);


    }
}
