using System.Collections;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// DynamicArrayType Encoder
    /// </summary>
    public class DynamicArrayTypeEncoder : ArrayTypeEncoder
    {
        #region Variables

        private readonly ABIType _elementType;
        private readonly IntTypeEncoder _intTypeEncoder;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="elementType"></param>
        public DynamicArrayTypeEncoder(ABIType elementType)
        {
            this._elementType = elementType;
            _intTypeEncoder = new IntTypeEncoder();
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
            if (_elementType.IsDynamic())
            {
                byte[][] elems = new byte[l.Count + 1 + l.Count][];
                elems[0] = _intTypeEncoder.EncodeInt(l.Count);

                int currentSize = 0;
                for (int i = 0; i < l.Count; i++)
                {
                    elems[i + 1] = _intTypeEncoder.EncodeInt((l.Count * 32) + currentSize); //location element
                    elems[i + 1 + l.Count] = _elementType.Encode(l[i]);
                    currentSize += elems[i + 1 + l.Count].Length;
                }

                return ByteUtil.Merge(elems);
            }
            else
            {
                byte[][] elems = new byte[l.Count + 1][];
                elems[0] = _intTypeEncoder.EncodeInt(l.Count);
                for (int i = 0; i < l.Count; i++)
                    elems[i + 1] = _elementType.Encode(l[i]);

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
            byte[][] elems = new byte[l.Count][];
            for (int i = 0; i < l.Count; i++)
                elems[i] = _elementType.Encode(l[i]);

            return ByteUtil.Merge(elems);
        }

        #endregion
    }
}