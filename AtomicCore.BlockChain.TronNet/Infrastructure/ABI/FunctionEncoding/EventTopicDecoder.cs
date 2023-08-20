using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// EventTopic Decoder
    /// </summary>
    public class EventTopicDecoder : ParameterDecoder
    {
        #region Variables

        /// <summary>
        /// is anonymous event
        /// </summary>
        private readonly bool _isAnonymousEvent;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public EventTopicDecoder() : this(false)
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="isAnonymousEvent"></param>
        public EventTopicDecoder(bool isAnonymousEvent)
        {
            _isAnonymousEvent = isAnonymousEvent;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// DecodeTopics
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="topics"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public T DecodeTopics<T>(object[] topics, string data) where T : new()
        {
            T result = new T();

            return DecodeTopics(result, topics, data);
        }

        /// <summary>
        /// DecodeDefaultTopics
        /// </summary>
        /// <param name="eventABI"></param>
        /// <param name="topics"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public List<ParameterOutput> DecodeDefaultTopics(EventABI eventABI, object[] topics, string data)
        {
            return DecodeDefaultTopics(eventABI.InputParameters, topics, data);
        }

        /// <summary>
        /// DecodeDefaultTopics
        /// </summary>
        /// <param name="inputParameters"></param>
        /// <param name="topics"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public List<ParameterOutput> DecodeDefaultTopics(Parameter[] inputParameters, object[] topics, string data)
        {
            List<ParameterOutput> parameterOutputs = new List<ParameterOutput>();

            Parameter[] indexedParameters = inputParameters
                .Where(x => x.Indexed == true)
                .OrderBy(x => x.Order)
                .ToArray();

            Parameter[] dataParameters = inputParameters
                .Where(x => x.Indexed == false)
                .OrderBy(x => x.Order)
                .ToArray();

            // Take one off topics count to skip signature
            int topicCount = topics.Length - 1;
            int indexedPropertiesCount = indexedParameters.Length;

            if (indexedPropertiesCount != topicCount)
                throw new Exception($"Number of indexes don't match the number of topics. Indexed Properties {indexedPropertiesCount}, Topics : {topicCount}");

            int topicNumber = 0;
            foreach (var topic in topics)
            {
                //skip the first one as it is the signature
                if (topicNumber > 0)
                {
                    Parameter parameter = indexedParameters[topicNumber - 1];

                    //skip dynamic types as the topic value is the sha3 keccak
                    if (!parameter.ABIType.IsDynamic())
                        parameterOutputs.Add(DecodeDefaultData(topic.ToString(), parameter).FirstOrDefault());
                    else
                    {
                        ParameterOutput parameterOutput = new ParameterOutput() 
                        { 
                            Parameter = parameter, 
                            Result = topic.ToString() 
                        };

                        parameterOutputs.Add(parameterOutput);
                    }
                }

                topicNumber++;
            }

            parameterOutputs.AddRange(DecodeDefaultData(data, dataParameters.ToArray()));

            return parameterOutputs;
        }

        /// <summary>
        /// DecodeTopics
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventDTO"></param>
        /// <param name="topics"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public T DecodeTopics<T>(T eventDTO, object[] topics, string data)
        {
            Type type = typeof(T);

            IEnumerable<PropertyInfo> properties = PropertiesExtractor.GetPropertiesWithParameterAttribute(type);

            PropertyInfo[] indexedProperties = properties
                .Where(x => 
                    x.GetCustomAttribute<ParameterAttribute>(true).Parameter.Indexed == true
                )
                .OrderBy(x => x.GetCustomAttribute<ParameterAttribute>(true).Order)
                .ToArray();

            PropertyInfo[] dataProperties = properties
                .Where(x => 
                    x.GetCustomAttribute<ParameterAttribute>(true).Parameter.Indexed == false
                )
                .OrderBy(x => x.GetCustomAttribute<ParameterAttribute>(true).Order)
                .ToArray();

            // Take one off topics count to skip signature if event is not anonymous
            int topicCount = !_isAnonymousEvent ? (topics.Length - 1) : topics.Length;
            int indexedPropertiesCount = indexedProperties.Length;

            if (indexedPropertiesCount != topicCount)
                throw new Exception($"Number of indexes don't match the number of topics. Indexed Properties {indexedPropertiesCount}, Topics : {topicCount}");

            int topicNumber = 0;
            foreach (var topic in topics)
            {
                //skip the first one as it is the signature for not anonymous events
                if (!_isAnonymousEvent && topicNumber == 0)
                {
                    topicNumber++;
                    continue;
                }

                PropertyInfo property = _isAnonymousEvent ? indexedProperties[topicNumber] : indexedProperties[topicNumber - 1];
                ParameterAttribute attribute = property.GetCustomAttribute<ParameterAttribute>(true);

                //skip dynamic types as the topic value is the sha3 keccak
                if (!attribute.Parameter.ABIType.IsDynamic())
                    eventDTO = DecodeAttributes(topic.ToString(), eventDTO, property);
                else
                {
                    if (property.PropertyType != typeof(string))
                        throw new Exception(
                            "Indexed Dynamic Types (string, arrays) value is the Keccak SHA3 of the value, the property type of " +
                            property.Name + "should be a string");
#if DOTNET35
                    property.SetValue(eventDTO, topic.ToString(), null);
#else
                    property.SetValue(eventDTO, topic.ToString());
#endif
                }

                topicNumber++;
            }

            eventDTO = DecodeAttributes(data, eventDTO, dataProperties.ToArray());

            return eventDTO;
        }

        #endregion
    }
}
