using System;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet ParamValue Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TronNetParamValueAttribute: Attribute
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public TronNetParamValueAttribute(TronNetContractType type)
        {
            ContractType = type;
        }

        #endregion

        #region Propertys

        /// <summary>
        /// ContractType
        /// </summary>
        public TronNetContractType ContractType { get; }

        #endregion
    }
}
