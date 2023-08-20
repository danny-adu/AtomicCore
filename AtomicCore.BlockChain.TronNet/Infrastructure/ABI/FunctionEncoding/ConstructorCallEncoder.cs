using System;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// ConstructorCall Encoder
    /// </summary>
    public class ConstructorCallEncoder : ParametersEncoder
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
        /// <param name="constructorInput"></param>
        /// <param name="contractByteCode"></param>
        /// <returns></returns>
        public string EncodeRequest<T>(T constructorInput, string contractByteCode)
        {
            Type type = typeof(T);
            byte[] encodedParameters = EncodeParametersFromTypeAttributes(type, constructorInput);

            return EncodeRequest(contractByteCode, encodedParameters.ToHex());
        }

        /// <summary>
        /// EncodeRequest
        /// </summary>
        /// <param name="contractByteCode"></param>
        /// <param name="parameters"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public string EncodeRequest(string contractByteCode, Parameter[] parameters, params object[] values)
        {
            string parametersEncoded = string.Empty;
            if (values != null)
                parametersEncoded = EncodeParameters(parameters, values).ToHex();

            return EncodeRequest(contractByteCode, parametersEncoded);
        }

        /// <summary>
        /// EncodeRequest
        /// </summary>
        /// <param name="contractByteCode"></param>
        /// <param name="encodedParameters"></param>
        /// <returns>0x0000000000</returns>
        public string EncodeRequest(string contractByteCode, string encodedParameters)
        {
            ByteCodeLibraryLinker.EnsureDoesNotContainPlaceholders(contractByteCode);

            if (contractByteCode.StartsWith(HEX_PREFIX, StringComparison.OrdinalIgnoreCase))
                return string.Format("{0}{1}", contractByteCode, encodedParameters);
            else
                return string.Format("{0}{1}{2}", HEX_PREFIX, contractByteCode, encodedParameters);
        }

        #endregion
    }
}