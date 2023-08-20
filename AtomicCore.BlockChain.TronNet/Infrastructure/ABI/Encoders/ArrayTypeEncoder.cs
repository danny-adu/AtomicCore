using System;
using System.Collections;
using System.Linq;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// ArrayType Encoder
    /// </summary>
    public abstract class ArrayTypeEncoder : ITypeEncoder
    {
        #region Public Methods

        /// <summary>
        /// Encode
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] Encode(object value)
        {
            if ((value is IEnumerable array) && !(value is string))
                return EncodeList(array.Cast<object>().ToList());

            throw new Exception("Array value expected for type");
        }

        /// <summary>
        /// EncodePacked
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] EncodePacked(object value)
        {
            if ((value is IEnumerable array) && !(value is string))
                return EncodeListPacked(array.Cast<object>().ToList());

            throw new Exception("Array value expected for type");
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// EncodeList
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public abstract byte[] EncodeList(IList l);

        /// <summary>
        /// EncodeListPacked
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public abstract byte[] EncodeListPacked(IList l);

        #endregion 
    }
}