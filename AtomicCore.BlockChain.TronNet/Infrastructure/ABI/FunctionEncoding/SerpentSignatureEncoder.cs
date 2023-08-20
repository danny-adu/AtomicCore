using System.Linq;
using System.Text;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// SerpentSignature sEncoder
    /// </summary>
    public class SerpentSignatureEncoder : SignatureEncoder
    {
        /// <summary>
        /// Generate Signature
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override string GenerateSignature(string name, Parameter[] parameters)
        {
            StringBuilder signature = new StringBuilder();
            signature.Append(name);
            signature.Append(' ');

            string[] paramSignature = parameters
                .OrderBy(x => x.Order)
                .Select(x => x.SerpentSignature)
                .ToArray();

            signature.Append(string.Join(string.Empty, paramSignature));

            return signature.ToString();
        }
    }
}