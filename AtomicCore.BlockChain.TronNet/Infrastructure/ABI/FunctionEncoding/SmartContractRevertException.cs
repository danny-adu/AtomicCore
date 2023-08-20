using System;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// SmartContract Revert Exception
    /// </summary>
    public class SmartContractRevertException : Exception
    {
        private const string ERROR_PREFIX = "Smart contract error: ";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message"></param>
        public SmartContractRevertException(string message) : base(ERROR_PREFIX + message)
        {

        }
    }
}