using System.Numerics;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Contract Message Base Class
    /// </summary>
    public abstract class ContractMessageBase
    {
        /// <summary>
        /// Amount
        /// </summary>
        public BigInteger AmountToSend { get; set; }

        /// <summary>
        /// Gas
        /// </summary>
        public BigInteger? Gas { get; set; }

        /// <summary>
        /// Gas Price
        /// </summary>
        public BigInteger? GasPrice { get; set; }

        /// <summary>
        /// From Address
        /// </summary>
        public string FromAddress { get; set; }

        /// <summary>
        /// Nonce
        /// </summary>
        public BigInteger? Nonce { get; set; }
    }

}
