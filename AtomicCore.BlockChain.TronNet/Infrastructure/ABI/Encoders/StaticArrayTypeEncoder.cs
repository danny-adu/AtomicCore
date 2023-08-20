using System;
using System.Collections;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// StaticArrayType Encoder
    /// </summary>
    public class StaticArrayTypeEncoder : ArrayTypeEncoder
    {
        #region Variables

        private readonly int arraySize;
        private readonly ABIType elementType;
        private readonly IntTypeEncoder intTypeEncoder;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="elementType"></param>
        /// <param name="arraySize"></param>
        public StaticArrayTypeEncoder(ABIType elementType, int arraySize)
        {
            this.elementType = elementType;
            this.arraySize = arraySize;
            intTypeEncoder = new IntTypeEncoder();
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// EncodeList
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public override byte[] EncodeList(IList l)
        {
            if (l.Count != arraySize)
                throw new Exception("List size (" + l.Count + ") != " + arraySize);
            
            if (elementType.IsDynamic())
            {
                byte[][] elems = new byte[arraySize + arraySize][];
                int currentSize = 0;
                for (int i = 0; i < l.Count; i++)
                {
                    elems[i] = intTypeEncoder.EncodeInt((l.Count * 32) + currentSize);
                    elems[i + l.Count] = elementType.Encode(l[i]);
                    currentSize += elems[i + l.Count].Length;
                }

                return ByteUtil.Merge(elems);
            }
            else
            {
                byte[][] elems = new byte[arraySize][];
                for (int i = 0; i < l.Count; i++)
                    elems[i] = elementType.Encode(l[i]);

                return ByteUtil.Merge(elems);
            }
        }

        /// <summary>
        /// EncodeListPacked
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public override byte[] EncodeListPacked(IList l)
        {
            if (l.Count != arraySize)
                throw new Exception("List size (" + l.Count + ") != " + arraySize);

            byte[][] elems = new byte[arraySize][];
            for (int i = 0; i < l.Count; i++)
                elems[i] = elementType.Encode(l[i]);

            return ByteUtil.Merge(elems);
        }

        #endregion
    }
}