using System;
using System.Collections.Generic;
using System.Reflection;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Parameter Indexed Topic Extractor
    /// </summary>
    public static class ParameterIndexedTopicExtractor
    {
        /// <summary>
        /// GetParameterIndexedTopics
        /// </summary>
        /// <param name="type"></param>
        /// <param name="instanceValue"></param>
        /// <returns></returns>
        public static List<ParameterAttributeIndexedTopics> GetParameterIndexedTopics(Type type, object instanceValue)
        {
            IEnumerable<PropertyInfo> properties = PropertiesExtractor.GetPropertiesWithParameterAttribute(type);
            List<ParameterAttributeIndexedTopics> parameterObjects = new List<ParameterAttributeIndexedTopics>();

            foreach (PropertyInfo property in properties)
            {
                ParameterAttribute parameterAttribute = property.GetCustomAttribute<ParameterAttribute>(true);

                if (parameterAttribute.Parameter.Indexed)
                {
                    parameterObjects.Add(new ParameterAttributeIndexedTopics
                    {
                        ParameterAttribute = parameterAttribute,
                        PropertyInfo = property
                    });
                }
            }

            return parameterObjects;
        }
    }
}