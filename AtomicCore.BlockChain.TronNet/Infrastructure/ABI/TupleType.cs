using System.Linq;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TupleType
    /// </summary>
    public class TupleType : ABIType
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="components"></param>
        public void SetComponents(Parameter[] components)
        {
            this.Components = components;
            ((TupleTypeEncoder) Encoder).Components = components;
            ((TupleTypeDecoder) Decoder).Components = components;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public TupleType() : base("tuple")
        {
            Decoder = new TupleTypeDecoder();
            Encoder = new TupleTypeEncoder();
        }

        #endregion

        #region Propertys

        /// <summary>
        /// Components
        /// </summary>
        public Parameter[] Components { get; protected set; }

        /// <summary>
        /// FixedSize
        /// </summary>
        public override int FixedSize {
            get
            {
                if (Components == null) 
                    return -1;

                if (Components.Any(x => x.ABIType.IsDynamic())) 
                    return -1;

                return Components.Sum(x => x.ABIType.FixedSize);
            }
        }

        #endregion
    }
}