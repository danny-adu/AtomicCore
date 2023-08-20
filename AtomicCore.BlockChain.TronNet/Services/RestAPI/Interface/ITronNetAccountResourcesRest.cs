namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// To manipulate account resources. Bandwidth or energy.
    /// </summary>
    public interface ITronNetAccountResourcesRest
    {
        /// <summary>
        /// Query the resource information of an account(bandwidth,energy,etc)
        /// </summary>
        /// <param name="address">address</param>
        /// <param name="visible">Optional,whether the address is in base58 format</param>
        /// <returns></returns>
        TronNetAccountResourceJson GetAccountResource(string address, bool visible = true);

        /// <summary>
        /// Query bandwidth information.
        /// </summary>
        /// <param name="address">address</param>
        /// <param name="visible">Optional,whether the address is in base58 format</param>
        /// <returns></returns>
        TronNetAccountNetResourceJson GetAccountNet(string address, bool visible = true);

        /// <summary>
        /// Stake an amount of TRX to obtain bandwidth OR Energy and TRON Power (voting rights) .
        /// Optionally, user can stake TRX to grant Energy or Bandwidth to others. 
        /// Balance amount in the denomination of sun.s
        /// </summary>
        /// <param name="ownerAddress">Owner address</param>
        /// <param name="frozenBalance">TRX stake amount,Trx</param>
        /// <param name="frozenDuration">TRX stake duration, only be specified as 3 days</param>
        /// <param name="resource">TRX stake type, 'BANDWIDTH' or 'ENERGY'</param>
        /// <param name="receiverAddress"></param>
        /// <param name="permissionID">Optional, for multi-signature use</param>
        /// <param name="visible">Optional, Whether the address is in base58 format.</param>
        /// <returns></returns>
        TronNetCreateTransactionRestJson FreezeBalance(string ownerAddress, decimal frozenBalance, int frozenDuration, TronNetResourceType resource, string receiverAddress = null, int? permissionID = null, bool visible = true);

        /// <summary>
        /// Unstake TRX that has passed the minimum stake duration to release bandwidth and energy 
        /// and at the same time TRON Power will reduce and all votes will be canceled.
        /// </summary>
        /// <param name="ownerAddress">Owner address</param>
        /// <param name="resource">Stake TRX for 'BANDWIDTH' or 'ENERGY'</param>
        /// <param name="receiverAddress">Optional,the address that will lose the resource</param>
        /// <param name="permissionID">Optional, for multi-signature use</param>
        /// <param name="visible">Optional, Whether the address is in base58 format.</param>
        /// <returns></returns>
        TronNetCreateTransactionRestJson UnfreezeBalance(string ownerAddress, TronNetResourceType resource, string receiverAddress = null, int? permissionID = null, bool visible = true);

        /// <summary>
        /// Returns all resources delegations from an account to another account. 
        /// The fromAddress can be retrieved from the GetDelegatedResourceAccountIndex API.
        /// </summary>
        /// <param name="fromAddress">Energy from address</param>
        /// <param name="toAddress">Energy delegation information</param>
        /// <param name="visible">Optional, Whether the address is in base58 format.</param>
        /// <returns></returns>
        TronNetDelegatedResourceJson GetDelegatedResource(string fromAddress, string toAddress, bool visible = true);

        /// <summary>
        /// Query the energy delegation by an account. 
        /// i.e. list all addresses that have delegated resources to an account.
        /// </summary>
        /// <param name="address">address</param>
        /// <param name="visible">Optional, Whether the address is in base58 format.</param>
        /// <returns></returns>
        TronNetDelegatedResourceAccountJson GetDelegatedResourceAccountIndex(string address, bool visible = true);
    }
}
