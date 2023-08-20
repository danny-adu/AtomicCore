using System.Numerics;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Trc20 Transfer Method
    /// </summary>
    [Function("transfer", "bool")]
    public class TransferFunction : FunctionMessage
    {
        /// <summary>
        /// To Address
        /// </summary>
        [Parameter("address", "_to", 1)]
        public string To { get; set; }

        /// <summary>
        /// Transfer Amount
        /// </summary>
        [Parameter("uint256", "_value", 2)]
        public BigInteger TokenAmount { get; set; }
    }
}
