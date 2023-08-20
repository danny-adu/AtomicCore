using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Parameters Encoder
    /// </summary>
    public class ParametersEncoder
    {
        #region Variables

        private readonly IntTypeEncoder intTypeEncoder;
        private readonly AttributesToABIExtractor attributesToABIExtractor;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ParametersEncoder()
        {
            intTypeEncoder = new IntTypeEncoder();
            attributesToABIExtractor = new AttributesToABIExtractor();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Encode AbiTypes
        /// </summary>
        /// <param name="abiTypes"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public byte[] EncodeAbiTypes(ABIType[] abiTypes, params object[] values)
        {
            if ((values == null) && (abiTypes.Length > 0))
                throw new ArgumentNullException(nameof(values), "No values specified for encoding");

            if (values == null) return Array.Empty<byte>();

            if (values.Length > abiTypes.Length)
                throw new Exception("Too many arguments: " + values.Length + " > " + abiTypes.Length);

            int staticSize = 0;
            int dynamicCount = 0;
            // calculating static size and number of dynamic params
            for (int i = 0; i < values.Length; i++)
            {
                var abiType = abiTypes[i];
                int parameterSize = abiType.FixedSize;
                if (parameterSize < 0)
                {
                    dynamicCount++;
                    staticSize += 32;
                }
                else
                    staticSize += parameterSize;
            }

            byte[][] encodedBytes = new byte[values.Length + dynamicCount][];

            int currentDynamicPointer = staticSize;
            int currentDynamicCount = 0;
            for (int i = 0; i < values.Length; i++)
            {
                ABIType abiType = abiTypes[i];
                if (abiType.IsDynamic())
                {
                    byte[] dynamicValueBytes = abiType.Encode(values[i]);

                    encodedBytes[i] = intTypeEncoder.EncodeInt(currentDynamicPointer);
                    encodedBytes[values.Length + currentDynamicCount] = dynamicValueBytes;
                    currentDynamicCount++;
                    currentDynamicPointer += dynamicValueBytes.Length;
                }
                else
                {
                    try
                    {
                        encodedBytes[i] = abiType.Encode(values[i]);
                    }
                    catch (Exception ex)
                    {
                        throw new AbiEncodingException(i, abiType, values[i],
                            $"An error occurred encoding abi value. Order: '{i + 1}', Type: '{abiType.Name}', Value: '{values[i] ?? "null"}'.  Ensure the value is valid for the abi type.",
                            ex);
                    }
                }
            }

            return ByteUtil.Merge(encodedBytes);
        }

        /// <summary>
        /// EncodeParameters
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public byte[] EncodeParameters(Parameter[] parameters, params object[] values)
        {
            return EncodeAbiTypes(parameters.Select(x => x.ABIType).ToArray(), values);
        }

        /// <summary>
        /// EncodeParametersFromTypeAttributes
        /// </summary>
        /// <param name="type"></param>
        /// <param name="instanceValue"></param>
        /// <returns></returns>
        public byte[] EncodeParametersFromTypeAttributes(Type type, object instanceValue)
        {
            List<ParameterAttributeValue> parameterObjects = GetParameterAttributeValues(type, instanceValue);
            Parameter[] abiParameters = GetParametersInOrder(parameterObjects);
            object[] objectValues = GetValuesInOrder(parameterObjects);

            return EncodeParameters(abiParameters, objectValues);
        }

        /// <summary>
        /// GetValuesInOrder
        /// </summary>
        /// <param name="parameterObjects"></param>
        /// <returns></returns>
        public object[] GetValuesInOrder(List<ParameterAttributeValue> parameterObjects)
        {
            return parameterObjects
                .OrderBy(x => x.ParameterAttribute.Order)
                .Select(x => x.Value)
                .ToArray();
        }

        /// <summary>
        /// GetParametersInOrder
        /// </summary>
        /// <param name="parameterObjects"></param>
        /// <returns></returns>
        public Parameter[] GetParametersInOrder(List<ParameterAttributeValue> parameterObjects)
        {
            return parameterObjects
                .OrderBy(x => x.ParameterAttribute.Order)
                .Select(x => x.ParameterAttribute.Parameter)
                .ToArray();
        }

        /// <summary>
        /// GetParameterAttributeValues
        /// </summary>
        /// <param name="type"></param>
        /// <param name="instanceValue"></param>
        /// <returns></returns>
        public List<ParameterAttributeValue> GetParameterAttributeValues(Type type, object instanceValue)
        {
            IEnumerable<PropertyInfo> properties = PropertiesExtractor.GetPropertiesWithParameterAttribute(type);
            List<ParameterAttributeValue> parameterObjects = new List<ParameterAttributeValue>();

            foreach (PropertyInfo property in properties)
            {
                ParameterAttribute parameterAttribute = property.GetCustomAttribute<ParameterAttribute>(true);
#if DOTNET35
                object propertyValue = property.GetValue(instanceValue, null);
#else
                object propertyValue = property.GetValue(instanceValue);
#endif

                attributesToABIExtractor.InitTupleComponentsFromTypeAttributes(property.PropertyType, parameterAttribute.Parameter.ABIType);

                if (parameterAttribute.Parameter.ABIType is TupleType tupleType)
                    propertyValue = GetTupleComponentValuesFromTypeAttributes(property.PropertyType, propertyValue);

                parameterObjects.Add(new ParameterAttributeValue
                {
                    ParameterAttribute = parameterAttribute,
                    Value = propertyValue
                });
            }

            return parameterObjects;
        }

        /// <summary>
        /// GetTupleComponentValuesFromTypeAttributes
        /// </summary>
        /// <param name="type"></param>
        /// <param name="instanceValue"></param>
        /// <returns></returns>
        public object[] GetTupleComponentValuesFromTypeAttributes(Type type, object instanceValue)
        {
            IEnumerable<PropertyInfo> properties = PropertiesExtractor.GetPropertiesWithParameterAttribute(type);

            IOrderedEnumerable<PropertyInfo> propertiesInOrder = properties
                .Where(x => x.IsDefined(typeof(ParameterAttribute), true))
                .OrderBy(x => x.GetCustomAttribute<ParameterAttribute>(true).Order);

            List<object> parameterObjects = new List<object>();

            foreach (PropertyInfo property in propertiesInOrder)
            {
                ParameterAttribute parameterAttribute = property.GetCustomAttribute<ParameterAttribute>(true);

#if DOTNET35
                object propertyValue = property.GetValue(instanceValue, null);
#else
                object propertyValue = property.GetValue(instanceValue);
#endif

                if (parameterAttribute.Parameter.ABIType is TupleType)
                    propertyValue = GetTupleComponentValuesFromTypeAttributes(property.PropertyType, propertyValue);

                parameterObjects.Add(propertyValue);
            }

            return parameterObjects.ToArray();
        }

        #endregion
    }
}

