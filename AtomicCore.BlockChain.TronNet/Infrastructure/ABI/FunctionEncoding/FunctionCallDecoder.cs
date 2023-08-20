using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// FunctionCall Decoder
    /// </summary>
    public class FunctionCallDecoder : ParameterDecoder
    {
        #region Variables

        /// <summary>
        /// Hex Prefix (0x)
        /// </summary>
        private const string HEX_PREFIX = "0x";

        #endregion

        #region Public Methods

        /// <summary>
        /// Is Data For Function
        /// </summary>
        /// <param name="sha3Signature"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool IsDataForFunction(string sha3Signature, string data)
        {
            sha3Signature = sha3Signature.EnsureHexPrefix();
            data = data.EnsureHexPrefix();

            if (data == HEX_PREFIX) return false;

            if (string.Equals(data.ToLower(), sha3Signature.ToLower(), StringComparison.Ordinal)) return true;

            if (data.ToLower().StartsWith(sha3Signature.ToLower())) return true;

            return false;
        }

        /// <summary>
        /// Decode FunctionInput
        /// </summary>
        /// <param name="sha3Signature"></param>
        /// <param name="data"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public List<ParameterOutput> DecodeFunctionInput(string sha3Signature, string data,
            params Parameter[] parameters)
        {
            sha3Signature = sha3Signature.EnsureHexPrefix();
            data = data.EnsureHexPrefix();

            if (!IsDataForFunction(sha3Signature, data)) return null;

            if (data.StartsWith(sha3Signature))
                data = data.Substring(sha3Signature.Length); //4 bytes?

            return DecodeDefaultData(data, parameters);
        }

        /// <summary>
        /// DecodeFunctionInput
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sha3Signature"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public T DecodeFunctionInput<T>(string sha3Signature, string data) where T : new()
        {
            return DecodeFunctionInput(new T(), sha3Signature, data);
        }

        /// <summary>
        /// DecodeFunctionInput
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="functionInput"></param>
        /// <param name="sha3Signature"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public T DecodeFunctionInput<T>(T functionInput, string sha3Signature, string data)
        {
            sha3Signature = sha3Signature.EnsureHexPrefix();
            data = data.EnsureHexPrefix();

            if ((data == HEX_PREFIX) || (data == sha3Signature)) return functionInput;

            if (data.StartsWith(sha3Signature))
                data = data.Substring(sha3Signature.Length);

            Type type = typeof(T);
            IEnumerable<PropertyInfo> properties = PropertiesExtractor.GetPropertiesWithParameterAttribute(type);
            DecodeAttributes(data, functionInput, properties.ToArray());

            return functionInput;
        }

        /// <summary>
        /// DecodeFunctionError
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public ErrorFunction DecodeFunctionError(string output)
        {
            if (ErrorFunction.IsErrorData(output))
                return DecodeFunctionInput<ErrorFunction>(ErrorFunction.ERROR_FUNCTION_ID, output);

            return null;
        }

        /// <summary>
        /// DecodeFunctionErrorMessage
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public string DecodeFunctionErrorMessage(string output)
        {
            ErrorFunction error = DecodeFunctionError(output);
            return error?.Message;
        }

        /// <summary>
        /// ThrowIfErrorOnOutput
        /// </summary>
        /// <param name="output"></param>
        public void ThrowIfErrorOnOutput(string output)
        {
            ErrorFunction error = DecodeFunctionError(output);
            if (error != null)
                throw new SmartContractRevertException(error.Message);
        }

        /// <summary>
        /// DecodeFunctionOutput
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="output"></param>
        /// <returns></returns>
        public T DecodeFunctionOutput<T>(string output) where T : new()
        {
            if (output == HEX_PREFIX) 
                return default;

            ThrowIfErrorOnOutput(output);

            T result = new T();
            DecodeFunctionOutput(result, output);

            return result;
        }

        /// <summary>
        /// DecodeFunctionOutput
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="functionOutputResult"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public T DecodeFunctionOutput<T>(T functionOutputResult, string output)
        {
            if (output == HEX_PREFIX)
                return functionOutputResult;

            ThrowIfErrorOnOutput(output);

            Type type = typeof(T);
            FunctionOutputAttribute function = FunctionOutputAttribute.GetAttribute<T>();
            if (function == null)
                throw new ArgumentException($"Unable to decode to '{typeof(T).Name}' because the type does not apply attribute '[{nameof(FunctionOutputAttribute)}]'.");

            IEnumerable<PropertyInfo> properties = PropertiesExtractor.GetPropertiesWithParameterAttribute(type);
            DecodeAttributes(output, functionOutputResult, properties.ToArray());

            return functionOutputResult;
        }

        /// <summary>
        /// Decodes the output of a function using either a FunctionOutputAttribute  (T)
        /// or the parameter casted to the type T, only one outputParameter should be used in this scenario.
        /// </summary>
        public T DecodeOutput<T>(string output, params Parameter[] outputParameter) where T : new()
        {
            if (output == HEX_PREFIX) 
                return default;

            ThrowIfErrorOnOutput(output);

            FunctionOutputAttribute function = FunctionOutputAttribute.GetAttribute<T>();
            if (function == null)
            {
                if (outputParameter != null)
                {
                    if (outputParameter.Length > 1)
                        throw new Exception(
                            "Only one output parameter supported to be decoded this way, use a FunctionOutputAttribute or define each outputparameter");

                    return DecodeSimpleTypeOutput<T>(outputParameter[0], output);
                }

                return default;
            }

            return DecodeFunctionOutput<T>(output);
        }

        /// <summary>
        /// DecodeSimpleTypeOutput
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="outputParameter"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public T DecodeSimpleTypeOutput<T>(Parameter outputParameter, string output)
        {
            if (output == HEX_PREFIX) 
                return default;

            ThrowIfErrorOnOutput(output);

            if (outputParameter != null)
            {
                outputParameter.DecodedType = typeof(T);
                ParameterOutput parmeterOutput = new ParameterOutput
                {
                    Parameter = outputParameter
                };

                List<ParameterOutput> results = DecodeOutput(output, parmeterOutput);

                if (results.Any())
                    return (T)results[0].Result;
            }

            return default;
        }

        #endregion
    }
}