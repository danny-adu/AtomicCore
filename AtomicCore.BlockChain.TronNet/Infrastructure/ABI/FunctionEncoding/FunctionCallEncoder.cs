using System;
using System.Reflection;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// FunctionCallEncoder
    /// </summary>
    public class FunctionCallEncoder : ParametersEncoder
    {
        #region Variables

        /// <summary>
        /// Hex Prefix (0x)
        /// </summary>
        private const string HEX_PREFIX = "0x";

        #endregion

        #region Public Methods

        /// <summary>
        /// EncodeRequest
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="functionInput"></param>
        /// <param name="sha3Signature"></param>
        /// <returns></returns>
        public string EncodeRequest<T>(T functionInput, string sha3Signature)
        {
            Type type = typeof(T);
            FunctionAttribute function = type.GetTypeInfo().GetCustomAttribute<FunctionAttribute>(true);
            if (function == null)
                throw new ArgumentException("Function Attribute is required", nameof(functionInput));

            byte[] encodedParameters = EncodeParametersFromTypeAttributes(type, functionInput);

            return EncodeRequest(sha3Signature, encodedParameters.ToHex());
        }

        /// <summary>
        /// EncodeRequest
        /// </summary>
        /// <param name="sha3Signature"></param>
        /// <param name="parameters"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public string EncodeRequest(string sha3Signature, Parameter[] parameters, params object[] values)
        {
            string parametersEncoded = string.Empty;
            if (values != null)
                parametersEncoded = EncodeParameters(parameters, values).ToHex();

            return EncodeRequest(sha3Signature, parametersEncoded);
        }

        /// <summary>
        /// EncodeRequest
        /// </summary>
        /// <param name="sha3Signature"></param>
        /// <param name="encodedParameters"></param>
        /// <returns></returns>
        public string EncodeRequest(string sha3Signature, string encodedParameters)
        {
            if (sha3Signature.StartsWith(HEX_PREFIX, StringComparison.OrdinalIgnoreCase))
                return string.Format("{0}{1}", sha3Signature, encodedParameters);
            else
                return string.Format("{0}{1}{2}", HEX_PREFIX, sha3Signature, encodedParameters);
        }

        /// <summary>
        /// EncodeRequest
        /// </summary>
        /// <param name="sha3Signature"></param>
        /// <returns></returns>
        public string EncodeRequest(string sha3Signature)
        {
            return EncodeRequest(sha3Signature, string.Empty);
        }

        #endregion
    }
}