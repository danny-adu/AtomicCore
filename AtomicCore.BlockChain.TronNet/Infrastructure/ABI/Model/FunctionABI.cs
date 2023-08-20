namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// FunctionABI
    /// </summary>
    public class FunctionABI
    {
        #region Variables

        private readonly SignatureEncoder signatureEncoder;
        private string sha3Signature;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="constant"></param>
        /// <param name="serpent"></param>
        public FunctionABI(string name, bool constant, bool serpent = false)
        {
            Name = name;
            Serpent = serpent;
            Constant = constant;
            signatureEncoder = serpent ? new SerpentSignatureEncoder() : new SignatureEncoder();
        }

        #endregion

        #region Propertys

        /// <summary>
        /// Serpent
        /// </summary>
        public bool Serpent { get; private set; }

        /// <summary>
        /// Constant
        /// </summary>
        public bool Constant { get; private set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// InputParameters
        /// </summary>
        public Parameter[] InputParameters { get; set; }

        /// <summary>
        /// OutputParameters
        /// </summary>
        public Parameter[] OutputParameters { get; set; }

        /// <summary>
        /// Sha3Signature - ReadOnly
        /// </summary>
        public string Sha3Signature
        {
            get
            {
                if (sha3Signature != null) 
                    return sha3Signature;

                sha3Signature = signatureEncoder.GenerateSha3Signature(Name, InputParameters, 4);

                return sha3Signature;
            }
        }

        #endregion
    }
}