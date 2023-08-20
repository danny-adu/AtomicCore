using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// ABIEncode
    /// </summary>
    public class ABIEncode
    {
        /// <summary>
        /// GetSha3ABIEncodedPacked
        /// </summary>
        /// <param name="abiValues"></param>
        /// <returns></returns>
        public byte[] GetSha3ABIEncodedPacked(params ABIValue[] abiValues)
        {
            return GetABIEncodedPacked(abiValues).ToKeccakHash();
        }

        /// <summary>
        /// GetSha3ABIEncodedPacked
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public byte[] GetSha3ABIEncodedPacked(params object[] values)
        {
            return GetABIEncodedPacked(values).ToKeccakHash();
        }

        /// <summary>
        /// GetSha3ABIEncoded
        /// </summary>
        /// <param name="abiValues"></param>
        /// <returns></returns>
        public byte[] GetSha3ABIEncoded(params ABIValue[] abiValues)
        {
            return GetABIEncoded(abiValues).ToKeccakHash();
        }

        /// <summary>
        /// GetSha3ABIEncoded
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public byte[] GetSha3ABIEncoded(params object[] values)
        {
            return GetABIEncoded(values).ToKeccakHash();
        }

        /// <summary>
        /// GetSha3ABIParamsEncodedPacked
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public byte[] GetSha3ABIParamsEncodedPacked<T>(T input)
        {
            return GetABIParamsEncodedPacked<T>(input).ToKeccakHash();
        }

        /// <summary>
        /// GetSha3ABIParamsEncoded
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public byte[] GetSha3ABIParamsEncoded<T>(T input)
        {
            return GetABIParamsEncoded<T>(input).ToKeccakHash();
        }

        /// <summary>
        /// GetABIEncodedPacked
        /// </summary>
        /// <param name="abiValues"></param>
        /// <returns></returns>
        public byte[] GetABIEncodedPacked(params ABIValue[] abiValues)
        {
            List<byte> result = new List<byte>();
            foreach (ABIValue abiValue in abiValues)
                result.AddRange(abiValue.ABIType.EncodePacked(abiValue.Value));

            return result.ToArray();
        }

        /// <summary>
        /// GetABIEncoded
        /// </summary>
        /// <param name="abiValues"></param>
        /// <returns></returns>
        public byte[] GetABIEncoded(params ABIValue[] abiValues)
        {
            List<Parameter> parameters = new List<Parameter>();
            List<object> values = new List<object>();

            int order = 1;
            foreach (var abiValue in abiValues)
            {
                parameters.Add(new Parameter(abiValue.ABIType.Name, order));
                values.Add(abiValue.Value);

                order++;
            }

            return new ParametersEncoder().EncodeParameters(parameters.ToArray(), values.ToArray());
        }

        /// <summary>
        /// GetABIEncoded
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public byte[] GetABIEncoded(params object[] values)
        {
            return GetABIEncoded(ConvertValuesToDefaultABIValues(values).ToArray());
        }

        /// <summary>
        /// GetABIParamsEncodedPacked
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public byte[] GetABIParamsEncodedPacked<T>(T input)
        {
            Type type = typeof(T);
            ParametersEncoder parametersEncoder = new ParametersEncoder();
            IOrderedEnumerable<ParameterAttributeValue> parameterObjects = parametersEncoder
                .GetParameterAttributeValues(type, input)
                .OrderBy(x => x.ParameterAttribute.Order);

            List<byte> result = new List<byte>();

            foreach (ParameterAttributeValue abiParameter in parameterObjects)
            {
                ABIType abiType = abiParameter.ParameterAttribute.Parameter.ABIType;
                object value = abiParameter.Value;

                result.AddRange(abiType.EncodePacked(value));
            }

            return result.ToArray();
        }

        /// <summary>
        /// GetABIParamsEncoded
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public byte[] GetABIParamsEncoded<T>(T input)
        {
            Type type = typeof(T);

            return new ParametersEncoder().EncodeParametersFromTypeAttributes(type, input);
        }

        /// <summary>
        /// ConvertValuesToDefaultABIValues
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private List<ABIValue> ConvertValuesToDefaultABIValues(params object[] values)
        {
            List<ABIValue> abiValues = new List<ABIValue>();
            foreach (object value in values)
            {
                if (value.IsNumber())
                {
                    BigInteger bigInt = BigInteger.Parse(value.ToString());
                    if (bigInt >= 0)
                        abiValues.Add(new ABIValue(new IntType("uint256"), value));
                    else
                        abiValues.Add(new ABIValue(new IntType("int256"), value));
                }

                if (value is string)
                    abiValues.Add(new ABIValue(new StringType(), value));

                if (value is bool)
                    abiValues.Add(new ABIValue(new BoolType(), value));

                if (value is byte[])
                    abiValues.Add(new ABIValue(new BytesType(), value));
            }

            return abiValues;
        }

        /// <summary>
        /// GetABIEncodedPacked
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public byte[] GetABIEncodedPacked(params object[] values)
        {
            List<ABIValue> abiValues = ConvertValuesToDefaultABIValues(values);

            return GetABIEncodedPacked(abiValues.ToArray());
        }
    }
}