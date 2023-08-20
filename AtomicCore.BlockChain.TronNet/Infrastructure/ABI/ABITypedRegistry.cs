using System;
using System.Collections.Concurrent;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// ABITypedRegistry
    /// </summary>
    public static class ABITypedRegistry
    {
        #region Variables

        private static ConcurrentDictionary<Type, FunctionABI> _functionAbiRegistry = new ConcurrentDictionary<Type, FunctionABI>();
        private static AttributesToABIExtractor _abiExtractor = new AttributesToABIExtractor();

        #endregion

        #region Public Methods

        /// <summary>
        /// Get Function ABI
        /// </summary>
        /// <typeparam name="TFunctionMessage"></typeparam>
        /// <returns></returns>
        public static FunctionABI GetFunctionABI<TFunctionMessage>()
        {
            return GetFunctionABI(typeof(TFunctionMessage));
        }

        /// <summary>
        /// Get Function ABI
        /// </summary>
        /// <param name="functionABIType"></param>
        /// <returns></returns>
        public static FunctionABI GetFunctionABI(Type functionABIType)
        {
            if (!_functionAbiRegistry.ContainsKey(functionABIType))
            {
                var functionAbi = _abiExtractor.ExtractFunctionABI(functionABIType);
                _functionAbiRegistry[functionABIType] = functionAbi ?? throw new ArgumentException(functionABIType.ToString() + " is not a valid Function Type");
            }

            return _functionAbiRegistry[functionABIType];
        }

        #endregion
    }
}
