namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// ParameterOutput Model
    /// </summary>
    public class ParameterOutput
    {
        /// <summary>
        /// Parameter
        /// </summary>
        public Parameter Parameter { get; set; }

        /// <summary>
        /// DataIndexStart
        /// </summary>
        public int DataIndexStart { get; set; }

        /// <summary>
        /// Result
        /// </summary>
        public object Result { get; set; }
        
    }
}