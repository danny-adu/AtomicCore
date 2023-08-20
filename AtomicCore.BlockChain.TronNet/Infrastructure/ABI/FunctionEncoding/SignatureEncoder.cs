using System.Linq;
using System.Text;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Signature Encoder
    /// </summary>
    public class SignatureEncoder
    {
        /// <summary>
        /// Generate Sha3 Signature
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string GenerateSha3Signature(string name, Parameter[] parameters)
        {
            string signature = GenerateSignature(name, parameters);

            return signature.ToKeccakHash();
        }

        /// <summary>
        /// Generate Sha3 Signature
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        /// <param name="numberOfFirstBytes"></param>
        /// <returns></returns>
        public string GenerateSha3Signature(string name, Parameter[] parameters, int numberOfFirstBytes)
        {
            return GenerateSha3Signature(name, parameters).Substring(0, numberOfFirstBytes * 2);
        }

        /// <summary>
        /// Generate Signature
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual string GenerateSignature(string name, Parameter[] parameters)
        {
            StringBuilder signature = new StringBuilder();
            signature.Append(name);
            signature.Append(GenerateParametersSignature(parameters));

            return signature.ToString();
        }

        /// <summary>
        /// Generate Parameters Signature
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual string GenerateParametersSignature(Parameter[] parameters)
        {
            StringBuilder signature = new StringBuilder();
            signature.Append('(');
            if (parameters != null)
            {
                string[] paramslist = parameters.OrderBy(x => x.Order).Select(GenerateParameteSignature).ToArray();
                string paramNames = string.Join(",", paramslist);
                signature.Append(paramNames);
            }
            signature.Append(')');

            return signature.ToString();
        }

        /// <summary>
        /// Generate Paramete Signature
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public virtual string GenerateParameteSignature(Parameter parameter)
        {
            if (parameter.ABIType is TupleType tupleType)
                return GenerateParametersSignature(tupleType.Components);

            ArrayType arrayType = parameter.ABIType as ArrayType;

            while (arrayType != null)
                if (arrayType.ElementType is TupleType arrayTupleType)
                    return string.Format(
                        "{0}{1}",
                        GenerateParametersSignature(arrayTupleType.Components),
                        parameter.ABIType.CanonicalName.Replace("tuple", string.Empty)
                    );
                else
                    arrayType = arrayType.ElementType as ArrayType;

            return parameter.ABIType.CanonicalName;
        }
    }
}