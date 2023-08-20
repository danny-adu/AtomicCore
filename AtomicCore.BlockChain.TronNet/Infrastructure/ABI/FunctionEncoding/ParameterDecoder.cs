using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Parameter Decoder
    /// </summary>
    public class ParameterDecoder
    {
        #region Variables

        /// <summary>
        /// Hex Prefix (0x)
        /// </summary>
        private const string HEX_PREFIX = "0x";

        /// <summary>
        /// attributes to abiExtractor
        /// </summary>
        private readonly AttributesToABIExtractor attributesToABIExtractor;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ParameterDecoder()
        {
            attributesToABIExtractor = new AttributesToABIExtractor();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// DecodeAttributes
        /// </summary>
        /// <param name="output"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public object DecodeAttributes(byte[] output, Type objectType)
        {
            IEnumerable<PropertyInfo> properties = PropertiesExtractor.GetPropertiesWithParameterAttribute(objectType);
            object objectResult = Activator.CreateInstance(objectType);

            return DecodeAttributes(output, objectResult, properties.ToArray());
        }

        /// <summary>
        /// DecodeAttributes
        /// </summary>
        /// <param name="output"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public object DecodeAttributes(string output, Type objectType)
        {
            return DecodeAttributes(output.HexToByteArray(), objectType);
        }

        /// <summary>
        /// DecodeAttributes
        /// </summary>
        /// <param name="output"></param>
        /// <param name="result"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public object DecodeAttributes(byte[] output, object result, params PropertyInfo[] properties)
        {
            if (output == null || output.Length == 0) return result;
            List<ParameterOutputProperty> parameterObjects = GetParameterOutputsFromAttributes(properties);
            ParameterOutputProperty[] orderedParameters = parameterObjects.OrderBy(x => x.Parameter.Order).ToArray();
            List<ParameterOutput> parameterResults = DecodeOutput(output, orderedParameters);

            foreach (ParameterOutput parameterResult in parameterResults)
            {
                ParameterOutputProperty parameter = (ParameterOutputProperty)parameterResult;
                PropertyInfo propertyInfo = parameter.PropertyInfo;
                object decodedResult = parameter.Result;

                if (parameter.Parameter.ABIType is TupleType tupleType)
                {
                    decodedResult = Activator.CreateInstance(propertyInfo.PropertyType);
                    AssingValuesFromPropertyList(decodedResult, parameter);
                }
#if DOTNET35
                propertyInfo.SetValue(result, decodedResult, null);
#else
                propertyInfo.SetValue(result, decodedResult);
#endif
            }

            return result;
        }

        /// <summary>
        /// DecodeAttributes
        /// </summary>
        /// <param name="output"></param>
        /// <param name="result"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public object DecodeAttributes(string output, object result, params PropertyInfo[] properties)
        {
            if (output == HEX_PREFIX)
                return result;

            return DecodeAttributes(output.HexToByteArray(), result, properties);
        }

        /// <summary>
        /// DecodeAttributes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="output"></param>
        /// <param name="result"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public T DecodeAttributes<T>(string output, T result, params PropertyInfo[] properties)
        {
            return (T)DecodeAttributes(output, (object)result, properties);
        }

        /// <summary>
        /// AssingValuesFromPropertyList
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="result"></param>
        public void AssingValuesFromPropertyList(object instance, ParameterOutputProperty result)
        {
            if (result.Parameter.ABIType is TupleType)
            {
                List<ParameterOutputProperty> childrenProperties = result.ChildrenProperties;
                if (result.Result != null)
                {
                    List<ParameterOutput> outputResult = (List<ParameterOutput>)result.Result;

                    foreach (ParameterOutput parameterOutput in outputResult)
                    {
                        var childrenProperty =
                            childrenProperties.FirstOrDefault(x =>
                                x.Parameter.Order == parameterOutput.Parameter.Order);

                        if (childrenProperty != null)
                        {
                            var decodedResult = parameterOutput.Result;
                            if (childrenProperty.Parameter.ABIType is TupleType)
                            {
                                //Adding the result to the children property for assignment to the instance
                                childrenProperty.Result = parameterOutput.Result;
                                //creating a new instance of our object property
                                decodedResult = Activator.CreateInstance(childrenProperty.PropertyInfo.PropertyType);
                                AssingValuesFromPropertyList(decodedResult, childrenProperty);
                            }
#if DOTNET35
                            childrenProperty.PropertyInfo.SetValue(instance, decodedResult, null);
#else
                            childrenProperty.PropertyInfo.SetValue(instance, decodedResult);
#endif
                        }
                    }
                }

            }
        }

        /// <summary>
        /// GetParameterOutputsFromAttributes
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public List<ParameterOutputProperty> GetParameterOutputsFromAttributes(PropertyInfo[] properties)
        {
            List<ParameterOutputProperty> parameterObjects = new List<ParameterOutputProperty>();

            foreach (PropertyInfo property in properties)
                if (property.IsDefined(typeof(ParameterAttribute), true))
                {
#if DOTNET35
                    var parameterAttribute =
                        (ParameterAttribute)property.GetCustomAttributes(typeof(ParameterAttribute), true)[0];
#else
                    ParameterAttribute parameterAttribute = property.GetCustomAttribute<ParameterAttribute>(true);
#endif
                    ParameterOutputProperty parameterOutputProperty = new ParameterOutputProperty
                    {
                        Parameter = parameterAttribute.Parameter,
                        PropertyInfo = property,
                    };

                    if (parameterAttribute.Parameter.ABIType is TupleType tupleType)
                    {
                        attributesToABIExtractor.InitTupleComponentsFromTypeAttributes(property.PropertyType,
                            tupleType);
                        parameterOutputProperty.ChildrenProperties =
                            GetParameterOutputsFromAttributes(property.PropertyType);
                    }
                    else if (parameterAttribute.Parameter.ABIType is ArrayType arrayType)
                    {
                        if (arrayType.ElementType is TupleType tupleTypeElement)
                        {
#if NETSTANDARD1_1 || PCL && !NET35
                            Type type = property.PropertyType.GenericTypeArguments.FirstOrDefault();
#else
                            Type type = property.PropertyType.GetGenericArguments().FirstOrDefault();
#endif
                            if (type == null) throw new Exception("Tuple array has to decode to a IList<T>: " + parameterAttribute.Parameter.Name);

                            attributesToABIExtractor.InitTupleComponentsFromTypeAttributes(type,
                                tupleTypeElement);
                        }

                        parameterAttribute.Parameter.DecodedType = property.PropertyType;
                    }
                    else
                        parameterAttribute.Parameter.DecodedType = property.PropertyType;

                    parameterObjects.Add(parameterOutputProperty);

                }
            return parameterObjects;
        }

        /// <summary>
        /// GetParameterOutputsFromAttributes
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<ParameterOutputProperty> GetParameterOutputsFromAttributes(Type type)
        {
            IEnumerable<PropertyInfo> properties = PropertiesExtractor.GetPropertiesWithParameterAttribute(type);

            return GetParameterOutputsFromAttributes(properties.ToArray());
        }

        /// <summary>
        /// DecodeDefaultData
        /// </summary>
        /// <param name="data"></param>
        /// <param name="inputParameters"></param>
        /// <returns></returns>
        public List<ParameterOutput> DecodeDefaultData(byte[] data, params Parameter[] inputParameters)
        {
            List<ParameterOutput> parameterOutputs = new List<ParameterOutput>();

            foreach (Parameter inputParameter in inputParameters)
            {
                inputParameter.DecodedType = inputParameter.ABIType.GetDefaultDecodingType();
                parameterOutputs.Add(new ParameterOutput
                {
                    Parameter = inputParameter
                });
            }

            return DecodeOutput(data, parameterOutputs.ToArray());
        }

        /// <summary>
        /// DecodeDefaultData
        /// </summary>
        /// <param name="data"></param>
        /// <param name="inputParameters"></param>
        /// <returns></returns>
        public List<ParameterOutput> DecodeDefaultData(string data, params Parameter[] inputParameters)
        {
            return DecodeDefaultData(data.HexToByteArray(), inputParameters);
        }

        /// <summary>
        /// DecodeOutput
        /// </summary>
        /// <param name="outputBytes"></param>
        /// <param name="outputParameters"></param>
        /// <returns></returns>
        public List<ParameterOutput> DecodeOutput(byte[] outputBytes, params ParameterOutput[] outputParameters)
        {
            Array.Sort(outputParameters, (x, y) => x.Parameter.Order.CompareTo(y.Parameter.Order));

            int currentIndex = 0;
            foreach (ParameterOutput outputParam in outputParameters)
            {
                var param = outputParam.Parameter;
                if (param.ABIType.IsDynamic())
                {
                    outputParam.DataIndexStart =
                        EncoderDecoderHelpers.GetNumberOfBytes(outputBytes.Skip(currentIndex).ToArray());
                    currentIndex += 32;
                }
                else
                {
                    var bytes = outputBytes.Skip(currentIndex).Take(param.ABIType.FixedSize).ToArray();
                    outputParam.Result = param.ABIType.Decode(bytes, outputParam.Parameter.DecodedType);

                    currentIndex += param.ABIType.FixedSize;
                }
            }

            ParameterOutput currentDataItem = null;
            foreach (
                ParameterOutput nextDataItem in outputParameters.Where(outputParam => outputParam.Parameter.ABIType.IsDynamic()))
            {
                if (currentDataItem != null)
                {
                    byte[] bytes =
                        outputBytes.Skip(currentDataItem.DataIndexStart).Take(nextDataItem.DataIndexStart - currentDataItem.DataIndexStart).ToArray();
                    currentDataItem.Result = currentDataItem.Parameter.ABIType.Decode(bytes, currentDataItem.Parameter.DecodedType);
                }

                currentDataItem = nextDataItem;
            }

            if (currentDataItem != null)
            {
                byte[] bytes = outputBytes.Skip(currentDataItem.DataIndexStart).ToArray();
                currentDataItem.Result = currentDataItem.Parameter.ABIType.Decode(bytes, currentDataItem.Parameter.DecodedType);
            }

            return outputParameters.ToList();
        }

        /// <summary>
        /// DecodeOutput
        /// </summary>
        /// <param name="output"></param>
        /// <param name="outputParameters"></param>
        /// <returns></returns>
        public List<ParameterOutput> DecodeOutput(string output, params ParameterOutput[] outputParameters)
        {
            byte[] outputBytes = output.HexToByteArray();

            return DecodeOutput(outputBytes, outputParameters);
        }

        #endregion
    }
}