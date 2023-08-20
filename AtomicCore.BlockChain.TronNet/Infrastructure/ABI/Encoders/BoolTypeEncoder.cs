using System;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// BoolType Encoder
    /// </summary>
    public class BoolTypeEncoder : ITypeEncoder
    {
        #region Variables

        private readonly IntTypeEncoder _intTypeEncoder;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public BoolTypeEncoder()
        {
            _intTypeEncoder = new IntTypeEncoder(false, 8);
        }

        #endregion

        #region Public Methods

        public byte[] Encode(object value)
        {
            if (!(value is bool))
                throw new Exception("Wrong value for bool type: " + value);

            return _intTypeEncoder.Encode((bool)value ? 1 : 0);
        }

        public byte[] EncodePacked(object value)
        {
            if (!(value is bool))
                throw new Exception("Wrong value for bool type: " + value);

            return _intTypeEncoder.EncodePacked((bool)value ? 1 : 0);
        }

        #endregion
    }
}