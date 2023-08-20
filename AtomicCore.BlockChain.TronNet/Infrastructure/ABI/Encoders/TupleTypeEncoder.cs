using System;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TupleType Encoder
    /// </summary>
    public class TupleTypeEncoder : ITypeEncoder
    {
        #region Variables

        private readonly ParametersEncoder parametersEncoder;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public TupleTypeEncoder()
        {
            parametersEncoder = new ParametersEncoder();
        }

        #endregion

        #region Propertys

        /// <summary>
        /// Components
        /// </summary>
        public Parameter[] Components { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Encode
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] Encode(object value)
        {
            if (!(value == null || value is object[]))
                return parametersEncoder.EncodeParametersFromTypeAttributes(value.GetType(), value);

            var input = value as object[];
            return parametersEncoder.EncodeParameters(Components, input);
        }

        /// <summary>
        /// EncodePacked
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] EncodePacked(object value)
        {
            //this may be throw excetion!!!
            return Encode(value);
        }

        #endregion
    }
}