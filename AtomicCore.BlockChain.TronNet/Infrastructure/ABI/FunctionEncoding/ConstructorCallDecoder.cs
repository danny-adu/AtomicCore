using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// ConstructorCall Decoder
    /// </summary>
    public class ConstructorCallDecoder : ParameterDecoder
    {
        #region Variables

        /// <summary>
        /// Hex Prefix (0x)
        /// </summary>
        private const string HEX_PREFIX = "0x";

        #endregion

        #region Public Methods

        /// <summary>
        /// DecodeConstructor Parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="deploymentObject"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public T DecodeConstructorParameters<T>(T deploymentObject, string data)
        {
            ByteCodeSwarmExtractor swarmExtractor = new ByteCodeSwarmExtractor();
            if (swarmExtractor.HasSwarmAddress(data))
                return DecodeConstructorParameters(
                    deploymentObject,
                    swarmExtractor.GetByteCodeIncludingSwarmAddressPart(data),
                    data
                );
            else
            {
                var properties = PropertiesExtractor.GetPropertiesWithParameterAttribute(typeof(T));
                if (properties.Any())
                    throw new Exception(
                        "Data supplied does not include a swarm address, to locate the constructor parameters");
                else
                    return deploymentObject;
            }
        }

        /// <summary>
        /// DecodeConstructor Parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public T DecodeConstructorParameters<T>(string data) where T : new()
        {
            return DecodeConstructorParameters(new T(), data);
        }

        /// <summary>
        /// DecodeConstructor Parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="deploymentByteCode"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public T DecodeConstructorParameters<T>(string deploymentByteCode, string data) where T : new()
        {
            return DecodeConstructorParameters(new T(), deploymentByteCode, data);
        }

        /// <summary>
        /// DecodeConstructor Parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="deploymentObject"></param>
        /// <param name="deploymentByteCode"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public T DecodeConstructorParameters<T>(T deploymentObject, string deploymentByteCode, string data)
        {
            if (!deploymentByteCode.StartsWith(HEX_PREFIX, StringComparison.OrdinalIgnoreCase)) deploymentByteCode = string.Format("{0}{1}", HEX_PREFIX, deploymentByteCode);
            if (!data.StartsWith(HEX_PREFIX, StringComparison.OrdinalIgnoreCase))
                data = string.Format("{0}{1}", HEX_PREFIX, data);

            if ((data == HEX_PREFIX) || (data == deploymentByteCode)) return deploymentObject;
            if (data.StartsWith(deploymentByteCode, StringComparison.OrdinalIgnoreCase))
                data = data.Substring(deploymentByteCode.Length);

            Type type = typeof(T);
            IEnumerable<PropertyInfo> properties = PropertiesExtractor.GetPropertiesWithParameterAttribute(type);

            return DecodeAttributes<T>(data, deploymentObject, properties.ToArray());
        }

        #endregion
    }
}