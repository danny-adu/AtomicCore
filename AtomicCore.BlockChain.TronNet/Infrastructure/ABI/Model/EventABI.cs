using System.Linq;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// EventABI
    /// </summary>
    public class EventABI
    {
        #region Variables

        private readonly SignatureEncoder signatureEncoder;
        private string sha3Signature;
        private int? numberOfIndexes;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        public EventABI(string name) : this(name, false)
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isAnonymous"></param>
        public EventABI(string name, bool isAnonymous)
        {
            Name = name;
            IsAnonymous = isAnonymous;
            signatureEncoder = new SignatureEncoder();
        }

        #endregion

        #region Propertys

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// IsAnonymous
        /// </summary>
        public bool IsAnonymous { get; set; }

        /// <summary>
        /// InputParameters
        /// </summary>
        public Parameter[] InputParameters { get; set; }

        /// <summary>
        /// Sha3Signature - ReadOnly
        /// </summary>
        public string Sha3Signature
        {
            get
            {
                if (sha3Signature != null) 
                    return sha3Signature;

                sha3Signature = signatureEncoder.GenerateSha3Signature(Name, InputParameters);

                return sha3Signature;
            }
        }

        /// <summary>
        /// NumberOfIndexes - ReadOnly
        /// </summary>
        public int NumberOfIndexes
        {
            get
            {
                if(numberOfIndexes == null)
                    numberOfIndexes = InputParameters.Count(x => x.Indexed == true);

                return numberOfIndexes.Value;
            }
        }

        #endregion
    }
}