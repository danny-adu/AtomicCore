namespace AtomicCore.BlockChain.TronscanAPI
{
    /// <summary>
    /// Tronscan Contract Type
    /// </summary>
    public enum TronscanContractType
    {
        /// <summary>
        /// Account Create Contract
        /// </summary>
        AccountCreateContract = 0,

        /// <summary>
        /// Trx Transfer
        /// </summary>
        TransferContract = 1,

        /// <summary>
        /// Trc10 Transfer(Other Trc10 Token)
        /// </summary>
        TransferAssetContract = 2,

        /// <summary>
        /// Vote Asset Contract
        /// </summary>
        VoteAssetContract = 3,

        /// <summary>
        /// Vote Witness Contract
        /// </summary>
        VoteWitnessContract = 4,

        /// <summary>
        /// WitnessCreateContract
        /// </summary>
        WitnessCreateContract = 5,

        /// <summary>
        /// AssetIssueContract
        /// </summary>
        AssetIssueContract = 6,

        /// <summary>
        /// WitnessUpdateContract
        /// </summary>
        WitnessUpdateContract = 8,

        /// <summary>
        /// ParticipateAssetIssueContract
        /// </summary>
        ParticipateAssetIssueContract = 9,

        /// <summary>
        /// AccountUpdateContract
        /// </summary>
        AccountUpdateContract = 10,

        /// <summary>
        /// FreezeBalanceContract
        /// </summary>
        FreezeBalanceContract = 11,

        /// <summary>
        /// UnfreezeBalanceContract
        /// </summary>
        UnfreezeBalanceContract = 12,

        /// <summary>
        /// WithdrawBalanceContract
        /// </summary>
        WithdrawBalanceContract = 13,

        /// <summary>
        /// UnfreezeAssetContract
        /// </summary>
        UnfreezeAssetContract = 14,

        /// <summary>
        /// UpdateAssetContract
        /// </summary>
        UpdateAssetContract = 15,

        /// <summary>
        /// ProposalCreateContract
        /// </summary>
        ProposalCreateContract = 16,

        /// <summary>
        /// ProposalApproveContract
        /// </summary>
        ProposalApproveContract = 17,

        /// <summary>
        /// ProposalDeleteContract
        /// </summary>
        ProposalDeleteContract = 18,

        /// <summary>
        /// SetAccountIdContract
        /// </summary>
        SetAccountIdContract = 19,

        /// <summary>
        /// CustomContract
        /// </summary>
        CustomContract = 20,

        /// <summary>
        /// Create Smart Contract
        /// </summary>
        CreateSmartContract = 30,

        /// <summary>
        /// Trigger Smart Contract(eg:Trc20 and 721)
        /// </summary>
        TriggerSmartContract = 31,

        /// <summary>
        /// GetContract
        /// </summary>
        GetContract = 32,

        /// <summary>
        /// UpdateSettingContract
        /// </summary>
        UpdateSettingContract = 33,

        /// <summary>
        /// ExchangeCreateContract
        /// </summary>
        ExchangeCreateContract = 41,

        /// <summary>
        /// ExchangeInjectContract
        /// </summary>
        ExchangeInjectContract = 42,

        /// <summary>
        /// ExchangeWithdrawContract
        /// </summary>
        ExchangeWithdrawContract = 43,

        /// <summary>
        /// ExchangeTransactionContract
        /// </summary>
        ExchangeTransactionContract = 44,

        /// <summary>
        /// UpdateEnergyLimitContract
        /// </summary>
        UpdateEnergyLimitContract = 45,

        /// <summary>
        /// AccountPermissionUpdateContract
        /// </summary>
        AccountPermissionUpdateContract = 46,

        /// <summary>
        /// ClearABIContract
        /// </summary>
        ClearABIContract = 48,

        /// <summary>
        /// UpdateBrokerageContract
        /// </summary>
        UpdateBrokerageContract = 49,

        /// <summary>
        /// ShieldedTransferContract
        /// </summary>
        ShieldedTransferContract = 51,

        /// <summary>
        /// MarketSellAssetContract
        /// </summary>
        MarketSellAssetContract = 52,

        /// <summary>
        /// MarketCancelOrderContract
        /// </summary>
        MarketCancelOrderContract = 53
    }
}
