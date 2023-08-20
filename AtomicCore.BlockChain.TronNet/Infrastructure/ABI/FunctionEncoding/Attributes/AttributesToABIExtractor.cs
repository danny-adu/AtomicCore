using System;
using System.Collections.Generic;
using System.Reflection;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// AttributesToABIExtractor
    /// </summary>
    public class AttributesToABIExtractor
    {
        /// <summary>
        /// Extract ContractABI
        /// </summary>
        /// <param name="contractMessagesTypes"></param>
        /// <returns></returns>
        public ContractABI ExtractContractABI(params Type[] contractMessagesTypes)
        {
            ContractABI contractABI = new ContractABI();
            List<FunctionABI> functions = new List<FunctionABI>();
            List<EventABI> events = new List<EventABI>();

            foreach (var contractMessageType in contractMessagesTypes)
            {
                if (FunctionAttribute.IsFunctionType(contractMessageType))
                    functions.Add(ExtractFunctionABI(contractMessageType));

                if (EventAttribute.IsEventType(contractMessageType))
                    events.Add(ExtractEventABI(contractMessageType));
            }

            contractABI.Functions = functions.ToArray();
            contractABI.Events = events.ToArray();

            return contractABI;
        }

        /// <summary>
        /// Extract FunctionABI
        /// </summary>
        /// <param name="contractMessageType"></param>
        /// <returns></returns>
        public FunctionABI ExtractFunctionABI(Type contractMessageType)
        {
            if (FunctionAttribute.IsFunctionType(contractMessageType))
            {
                FunctionAttribute functionAttribute = FunctionAttribute.GetAttribute(contractMessageType);
                FunctionABI functionABI = new FunctionABI(functionAttribute.Name, false)
                {
                    InputParameters = ExtractParametersFromAttributes(contractMessageType)
                };

                if (functionAttribute.DTOReturnType != null)
                {
                    functionABI.OutputParameters = ExtractParametersFromAttributes(contractMessageType);
                }
                else if (functionAttribute.ReturnType != null)
                {
                    var parameter = new Parameter(functionAttribute.ReturnType);
                    functionABI.OutputParameters = new Parameter[] { parameter };
                }

                return functionABI;
            }

            return null;
        }

        /// <summary>
        /// Extract EventABI
        /// </summary>
        /// <param name="contractMessageType"></param>
        /// <returns></returns>
        public EventABI ExtractEventABI(Type contractMessageType)
        {
            if (EventAttribute.IsEventType(contractMessageType))
            {
                EventAttribute eventAttribute = EventAttribute.GetAttribute(contractMessageType);
                EventABI eventABI = new EventABI(eventAttribute.Name, eventAttribute.IsAnonymous)
                {
                    InputParameters = ExtractParametersFromAttributes(contractMessageType)
                };

                return eventABI;
            }

            return null;
        }

        /// <summary>
        /// Extract ParametersFromAttributes
        /// </summary>
        /// <param name="contractMessageType"></param>
        /// <returns></returns>
        public Parameter[] ExtractParametersFromAttributes(Type contractMessageType)
        {
            IEnumerable<PropertyInfo> properties = PropertiesExtractor.GetPropertiesWithParameterAttribute(type: contractMessageType);
            List<Parameter> parameters = new List<Parameter>();

            foreach (PropertyInfo property in properties)
            {
                ParameterAttribute parameterAttribute = property.GetCustomAttribute<ParameterAttribute>(true);
                
                InitTupleComponentsFromTypeAttributes(property.PropertyType, parameterAttribute.Parameter.ABIType);
                
                parameters.Add(parameterAttribute.Parameter);   
            }

            return parameters.ToArray();
        }

        /// <summary>
        /// InitTuple ComponentsFromTypeAttributes
        /// </summary>
        /// <param name="type"></param>
        /// <param name="abiType"></param>
        public void InitTupleComponentsFromTypeAttributes(Type type, ABIType abiType)
        {
            if (abiType is TupleType abiTupleType)
            {
                IEnumerable<PropertyInfo> properties = PropertiesExtractor.GetPropertiesWithParameterAttribute(type);
                List<Parameter> parameterObjects = new List<Parameter>();

                foreach (PropertyInfo property in properties)
                {
                    ParameterAttribute parameterAttribute = property.GetCustomAttribute<ParameterAttribute>(true);
                    parameterAttribute.Parameter.DecodedType = property.PropertyType;
                    InitTupleComponentsFromTypeAttributes(property.PropertyType, parameterAttribute.Parameter.ABIType);
 
                    parameterObjects.Add(parameterAttribute.Parameter);
                }

                abiTupleType.SetComponents(parameterObjects.ToArray());
            }

            ArrayType abiArrayType = abiType as ArrayType;

            while (abiArrayType != null)
            {
                var arrayListType = ArrayTypeDecoder.GetIListElementType(type);
                if(arrayListType == null) throw new Exception("Only types that implement IList<T> are supported for encoding");

                if (abiArrayType.ElementType is TupleType arrayTupleType)
                {
                    InitTupleComponentsFromTypeAttributes(arrayListType, arrayTupleType);
                    abiArrayType = null;
                }
                else
                    abiArrayType = abiArrayType.ElementType as ArrayType;
            }
        }
    }
}