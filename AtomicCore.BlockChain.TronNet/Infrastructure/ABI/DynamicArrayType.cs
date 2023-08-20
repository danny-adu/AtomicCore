namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// DynamicArrayType
    /// </summary>
    public class DynamicArrayType : ArrayType
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        public DynamicArrayType(string name) : base(name)
        {
            Decoder = new DynamicArrayTypeDecoder(ElementType);
            Encoder = new DynamicArrayTypeEncoder(ElementType);
        }

        #endregion

        #region Propertys

        /// <summary>
        /// CanonicalName
        /// </summary>
        public override string CanonicalName => ElementType.CanonicalName + "[]";

        /// <summary>
        /// FixedSize
        /// </summary>
        public override int FixedSize => -1;

        #endregion
    }
}