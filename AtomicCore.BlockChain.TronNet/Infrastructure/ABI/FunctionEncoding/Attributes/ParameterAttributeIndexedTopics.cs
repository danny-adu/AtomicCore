using System.Collections.Generic;
using System.Reflection;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Parameter Attribute Indexed Topics
    /// </summary>
    public class ParameterAttributeIndexedTopics
    {
        #region Constructor

        /// <summary>
        /// ParameterAttributeIndexedTopics
        /// </summary>
        public ParameterAttributeIndexedTopics()
        {
            Topics = new List<object>();
        }

        #endregion

        #region Propertys

        /// <summary>
        /// ParameterAttribute
        /// </summary>
        public ParameterAttribute ParameterAttribute { get; set; }

        /// <summary>
        /// Topics
        /// </summary>
        public List<object> Topics { get; set; }

        /// <summary>
        /// PropertyInfo
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// GetTopicValues
        /// </summary>
        /// <returns></returns>
        public object[] GetTopicValues()
        {
            if (Topics == null || Topics.Count == 0) return null;
            return Topics.ToArray();
        }

        #endregion
    }
}