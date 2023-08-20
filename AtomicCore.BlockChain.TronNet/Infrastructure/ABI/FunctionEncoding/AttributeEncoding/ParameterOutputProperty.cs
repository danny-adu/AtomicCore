using System.Collections.Generic;
using System.Reflection;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// ParameterOutputProperty
    /// </summary>
    public class ParameterOutputProperty : ParameterOutput
    {
        /// <summary>
        /// PropertyInfo
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }

        /// <summary>
        /// ChildrenProperties
        /// </summary>
        public List<ParameterOutputProperty> ChildrenProperties { get; set; }
    }
}