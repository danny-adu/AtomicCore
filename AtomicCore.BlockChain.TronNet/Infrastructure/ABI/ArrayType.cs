using System;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// ArrayType
    /// </summary>
    public abstract class ArrayType : ABIType
    {
        #region Variables

        internal ABIType ElementType;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        protected ArrayType(string name) : base(name)
        {
            InitialiseElementType(name);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public new static ArrayType CreateABIType(string typeName)
        {
            var indexFirstBracket = typeName.IndexOf("[", StringComparison.Ordinal);
            var indexSecondBracket = typeName.IndexOf("]", indexFirstBracket, StringComparison.Ordinal);

            if (indexFirstBracket + 1 == indexSecondBracket)
                return new DynamicArrayType(typeName);
            else
            {
                return new StaticArrayType(typeName);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// InitialiseElementType
        /// </summary>
        /// <param name="name"></param>
        private void InitialiseElementType(string name)
        {
            var indexFirstBracket = name.IndexOf("[", StringComparison.Ordinal);
            var elementTypeName = name.Substring(0, indexFirstBracket);
            var indexSecondBracket = name.IndexOf("]", indexFirstBracket, StringComparison.Ordinal);
          
            var subDim = indexSecondBracket + 1 == name.Length ? "" : name.Substring(indexSecondBracket + 1);
            ElementType = ABIType.CreateABIType(elementTypeName + subDim);
        }

        #endregion
    }
}