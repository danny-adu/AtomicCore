using System;
using System.Collections.Generic;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TupleType Decoder
    /// </summary>
    public class TupleTypeDecoder : TypeDecoder
    {
        #region Variables

        private readonly ParameterDecoder parameterDecoder;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public TupleTypeDecoder()
        {
            parameterDecoder = new ParameterDecoder();
        }

        #endregion

        #region Propertys

        /// <summary>
        /// Components
        /// </summary>
        public Parameter[] Components { get; set; }

        #endregion

        #region Override Methods

        /// <summary>
        /// Decode
        /// </summary>
        /// <param name="encoded"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public override object Decode(byte[] encoded, Type type)
        {
            //TODO: do we need to check ? we always return a list of ParameterOutputs
            // if (!IsSupportedType(type)) throw new NotSupportedException(type + " is not supported");
            ParameterOutput[] decodingComponents = InitDefaultDecodingComponents();

            return parameterDecoder.DecodeOutput(encoded, decodingComponents);
        }

        /// <summary>
        /// Get Default DecodingType
        /// </summary>
        /// <returns></returns>
        public override Type GetDefaultDecodingType()
        {
            return typeof(List<ParameterOutput>);
        }

        /// <summary>
        /// Is SupportedType
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override bool IsSupportedType(Type type)
        {
            return (type == typeof(List<ParameterOutput>));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Init DefaultDecoding Components
        /// </summary>
        /// <returns></returns>
        public ParameterOutput[] InitDefaultDecodingComponents()
        {
            var decodingDefaultComponents = new List<ParameterOutput>();
            foreach (var component in Components)
            {
                ParameterOutput parameterOutput = new ParameterOutput
                {
                    Parameter = component
                };

                if (component.DecodedType == null)
                    parameterOutput.Parameter.DecodedType = component.ABIType.GetDefaultDecodingType();

                decodingDefaultComponents.Add(parameterOutput);
            }

            return decodingDefaultComponents.ToArray();
        }

        /// <summary>
        /// Decode
        /// </summary>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public List<ParameterOutput> Decode(byte[] encoded)
        {
            return Decode<List<ParameterOutput>>(encoded);
        }

        #endregion
    }
}