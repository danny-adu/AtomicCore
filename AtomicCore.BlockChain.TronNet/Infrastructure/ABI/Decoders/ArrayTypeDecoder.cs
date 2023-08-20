using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// ArrayType Decoder
    /// </summary>
    public class ArrayTypeDecoder : TypeDecoder
    {
        #region Variables

        private AttributesToABIExtractor _attributesToABIExtractor = null;

        #endregion

        #region Constructor

        /// <summary>
        /// ArrayType Decoder
        /// </summary>
        /// <param name="elementType"></param>
        /// <param name="size"></param>
        public ArrayTypeDecoder(ABIType elementType, int size)
        {
            _attributesToABIExtractor = new AttributesToABIExtractor();

            Size = size;
            ElementType = elementType;
        }

        #endregion

        #region Propertys

        /// <summary>
        /// Size
        /// </summary>
        public int Size { get; protected set; }

        /// <summary>
        /// Element Type
        /// </summary>
        protected ABIType ElementType { get; set; }

        #endregion

        #region override Methods

        /// <summary>
        /// Decode
        /// </summary>
        /// <param name="encoded"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public override object Decode(byte[] encoded, Type type)
        {
            return Decode(encoded, type, Size);
        }

        /// <summary>
        /// Get Default Decoding Type
        /// </summary>
        /// <returns></returns>
        public override Type GetDefaultDecodingType()
        {
            return typeof(List<object>);
        }

        /// <summary>
        /// Is Supported Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override bool IsSupportedType(Type type)
        {
            return GetIListElementType(type) != null;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Decode
        /// </summary>
        /// <param name="encoded"></param>
        /// <param name="type"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        protected object Decode(byte[] encoded, Type type, int size)
        {
            if (ElementType.IsDynamic())
                return DecodeDynamicElementType(encoded, type, size);
            else
                return DecodeStaticElementType(encoded, type, size);
        }

        /// <summary>
        /// Decode Dynamic Element Type
        /// </summary>
        /// <param name="encoded"></param>
        /// <param name="type"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        protected virtual object DecodeDynamicElementType(byte[] encoded, Type type, int size)
        {
            IList decodedListOutput = (IList)Activator.CreateInstance(type);

            if (decodedListOutput == null)
                throw new Exception("Only types that implement IList<T> are supported to decode Array Types");

            Type elementType = GetIListElementType(type);

            if (elementType == null)
                throw new Exception("Only types that implement IList<T> are supported to decode Array Types");

            IntTypeDecoder intDecoder = new IntTypeDecoder();
            List<int> dataIndexes = new List<int>();

            int currentIndex = 0;
            while (currentIndex < size)
            {
                dataIndexes.Add(intDecoder.DecodeInt(encoded.Skip(currentIndex * 32).Take(32).ToArray()));
                currentIndex++;
            }

            currentIndex = 0;

            while (currentIndex < size)
            {
                int currentDataIndex = dataIndexes[currentIndex];
                int nextDataIndex = encoded.Length;
                if (currentIndex + 1 < dataIndexes.Count)
                    nextDataIndex = dataIndexes[currentIndex + 1];

                byte[] encodedElement =
                    encoded.Skip(currentDataIndex).Take(nextDataIndex - currentDataIndex).ToArray();

                DecodeAndAddElement(elementType, decodedListOutput, encodedElement);
                
                currentIndex++;
            }

            return decodedListOutput;
        }

        /// <summary>
        /// Decode And Add Element
        /// </summary>
        /// <param name="elementType"></param>
        /// <param name="decodedList"></param>
        /// <param name="encodedElement"></param>
        private void DecodeAndAddElement(Type elementType, IList decodedList, byte[] encodedElement)
        {
            if (ElementType is TupleType tupleTypeElement)
            {
                InitTupleElementComponents(elementType, tupleTypeElement);
                decodedList.Add(new ParameterDecoder().DecodeAttributes(encodedElement, elementType));
            }
            else
                decodedList.Add(ElementType.Decode(encodedElement, elementType));
        }

        /// <summary>
        /// Init Tuple Element Components
        /// </summary>
        /// <param name="elementType"></param>
        /// <param name="tupleTypeElement"></param>
        protected void InitTupleElementComponents(Type elementType, TupleType tupleTypeElement)
        {
            if (tupleTypeElement.Components == null)
                _attributesToABIExtractor.InitTupleComponentsFromTypeAttributes(elementType,
                    tupleTypeElement);
        }

        /// <summary>
        /// Decode Static Element Type
        /// </summary>
        /// <param name="encoded"></param>
        /// <param name="type"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        protected virtual object DecodeStaticElementType(byte[] encoded, Type type, int size)
        {
            IList decodedListOutput = (IList) Activator.CreateInstance(type);

            if (decodedListOutput == null)
                throw new Exception("Only types that implement IList<T> are supported to decoded Array Types");

            Type elementType = GetIListElementType(type);

            if (elementType == null)
                throw new Exception("Only types that implement IList<T> are supported to decoded Array Types");

            int currentIndex = 0;

            while (currentIndex != encoded.Length)
            {
                byte[] encodedElement = encoded.Skip(currentIndex).Take(ElementType.FixedSize).ToArray();
                DecodeAndAddElement(elementType, decodedListOutput, encodedElement);
                int newIndex = currentIndex + ElementType.FixedSize;
                currentIndex = newIndex;
            }

            return decodedListOutput;
        }

        /// <summary>
        /// Get IList Element Type
        /// </summary>
        /// <param name="listType"></param>
        /// <returns></returns>
        public static Type GetIListElementType(Type listType)
        {
#if DOTNET35
            var enumType = listType.GetTypeInfo().ImplementedInterfaces()
            .Where(i => i.GetTypeInfo().IsGenericType && (i.GenericTypeArguments().Length == 1))
            .FirstOrDefault(i => i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            return enumType?.GenericTypeArguments()[0];
#else
            var enumType = listType.GetTypeInfo().ImplementedInterfaces
            .Where(i => i.GetTypeInfo().IsGenericType && (i.GenericTypeArguments.Length == 1))
            .FirstOrDefault(i => i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            return enumType?.GenericTypeArguments[0];
#endif
        }

        #endregion
    }
}