using System.Reflection;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// ParameterAttributeValue
    /// </summary>
    public class ParameterAttributeValue
    {
        /// <summary>
        /// ParameterAttribute
        /// </summary>
        public ParameterAttribute ParameterAttribute { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// PropertyInfo
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }
    }
}