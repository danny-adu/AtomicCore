namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// ContractABI
    /// </summary>
    public class ContractABI
    {
        /// <summary>
        /// Functions
        /// </summary>
        public FunctionABI[] Functions { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ConstructorABI Constructor { get; set; }

        /// <summary>
        /// Events
        /// </summary>
        public EventABI[] Events { get; set; }
    }
}