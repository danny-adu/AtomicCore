using System;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Parameter
    /// </summary>
    public class Parameter
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="order"></param>
        /// <param name="internalType"></param>
        /// <param name="serpentSignature"></param>
        public Parameter(string type, string name = null, int order = 1, string internalType = null, string serpentSignature = null)
        {
            Name = name;
            Type = type;
            Order = order;
            InternalType = internalType;
            SerpentSignature = serpentSignature;
            ABIType = ABIType.CreateABIType(type);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="order"></param>
        /// <param name="internalType"></param>
        public Parameter(string type, int order, string internalType = null) : this(type, null, order, internalType)
        {

        }

        #endregion

        #region Propertys

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Type
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// ABIType
        /// </summary>
        public ABIType ABIType { get; private set; }

        /// <summary>
        /// Order
        /// </summary>
        public int Order { get; private set; }

        /// <summary>
        /// InternalType
        /// </summary>
        public string InternalType { get; private set; }

        /// <summary>
        /// DecodedType
        /// </summary>
        public Type DecodedType { get; set; }

        /// <summary>
        /// Indexed
        /// </summary>
        public bool Indexed { get; set; }

        /// <summary>
        /// SerpentSignature
        /// </summary>
        public string SerpentSignature { get; private set; }

        #endregion
    }

}