using System;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Generic ABI type
    /// </summary>
    public abstract class ABIType
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        public ABIType(string name)
        {
            Name = name;
        }

        #endregion

        #region Propertys

        /// <summary>
        /// Decoder
        /// </summary>
        protected ITypeDecoder Decoder { get; set; }

        /// <summary>
        /// Encoder
        /// </summary>
        protected ITypeEncoder Encoder { get; set; }

        /// <summary>
        /// The type name as it was specified in the interface description
        /// </summary>
        public virtual string Name { get; }

        /// <summary>
        /// The canonical type name (used for the method signature creation)
        /// E.g. 'int' - canonical 'int256'
        /// </summary>
        public virtual string CanonicalName => Name;

        /// <summary>
        /// fixed size in bytes or negative value if the type is dynamic
        /// </summary>
        public virtual int FixedSize => 32;

        #endregion

        #region Public Methods

        /// <summary>
        /// CreateABIType
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static ABIType CreateABIType(string typeName)
        {
            if (typeName == "tuple")
                return new TupleType();

            if (typeName.Contains("["))
                return ArrayType.CreateABIType(typeName);
            if ("bool".Equals(typeName))
                return new BoolType();
            if (typeName.StartsWith("int", StringComparison.Ordinal) || typeName.StartsWith("uint", StringComparison.Ordinal))
                return new IntType(typeName);
            if ("address".Equals(typeName))
                return new AddressType();
            if ("string".Equals(typeName))
                return new StringType();
            if ("bytes".Equals(typeName))
                return new BytesType();
            if (typeName.StartsWith("bytes", StringComparison.Ordinal))
            {
                int size = Convert.ToInt32(typeName.Substring(5));
                if (size == 32)
                    return new Bytes32Type(typeName);
                else
                    return new BytesElementaryType(typeName, size);
            }

            throw new ArgumentException("Unknown type: " + typeName);
        }

        /// <summary>
        /// Decode
        /// </summary>
        /// <param name="encoded"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object Decode(byte[] encoded, Type type)
        {
            return Decoder.Decode(encoded, type);
        }

        /// <summary>
        /// Decode
        /// </summary>
        /// <param name="encoded"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object Decode(string encoded, Type type)
        {
            return Decoder.Decode(encoded, type);
        }

        /// <summary>
        /// Decode
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public T Decode<T>(string encoded)
        {
            return Decoder.Decode<T>(encoded);
        }

        /// <summary>
        /// Decode
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public T Decode<T>(byte[] encoded)
        {
            return Decoder.Decode<T>(encoded);
        }

        /// <summary>
        /// Encode
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] Encode(object value)
        {
            return Encoder.Encode(value);
        }

        /// <summary>
        /// EncodePacked
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] EncodePacked(object value)
        {
            return Encoder.EncodePacked(value);
        }

        /// <summary>
        /// GetDefaultDecodingType
        /// </summary>
        /// <returns></returns>
        public Type GetDefaultDecodingType()
        {
            return Decoder.GetDefaultDecodingType();
        }

        /// <summary>
        /// IsDynamic
        /// </summary>
        /// <returns></returns>
        public bool IsDynamic()
        {
            return FixedSize < 0;
        }

        /// <summary>
        /// Override ToString Method
        /// Return Name Value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}
