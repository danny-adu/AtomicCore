using System;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// StaticArrayType
    /// </summary>
    public class StaticArrayType : ArrayType
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        public StaticArrayType(string name) : base(name)
        {
            IntialiseSize(name);
            Decoder = new ArrayTypeDecoder(ElementType, Size);
            Encoder = new StaticArrayTypeEncoder(ElementType, Size);
        }

        #endregion

        #region Propertys

        /// <summary>
        /// Size
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// CanonicalName
        /// </summary>
        public override string CanonicalName => ElementType.CanonicalName + "[" + Size + "]";

        /// <summary>
        /// FixedSize
        /// </summary>
        public override int FixedSize => ElementType.FixedSize * Size;

        #endregion

        #region Private Methods

        /// <summary>
        /// IntialiseSize
        /// </summary>
        /// <param name="name"></param>
        private void IntialiseSize(string name)
        {
            var indexFirstBracket = name.IndexOf("[", StringComparison.Ordinal);
            var indexSecondBracket = name.IndexOf("]", indexFirstBracket, StringComparison.Ordinal);
            var arraySize = name.Substring(indexFirstBracket + 1, indexSecondBracket - (indexFirstBracket + 1));
            Size = int.Parse(arraySize);
        }

        #endregion
    }
}