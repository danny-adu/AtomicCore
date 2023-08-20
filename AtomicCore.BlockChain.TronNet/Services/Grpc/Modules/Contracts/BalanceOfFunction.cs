namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Trc20 BalanceOf Method
    /// </summary>
    [Function("balanceOf", "uint256")]
    public class BalanceOfFunction : FunctionMessage
    {
        [Parameter("address", "owner", 1)]
        public string Owner { get; set; }

    }

    /// <summary>
    /// Trc20 BalanceOf Result
    /// </summary>
    [FunctionOutput]
    public class BalanceOfFunctionOutput : IFunctionOutputDTO
    {
        [Parameter("uint256", 1)]
        public long Balance { get; set; }
    }
}
