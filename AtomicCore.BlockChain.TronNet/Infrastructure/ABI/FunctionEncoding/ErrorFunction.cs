namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Error Function
    /// </summary>
    [Function("Error")]
    public class ErrorFunction
    {
        #region Variables

        /// <summary>
        /// error function id
        /// </summary>
        public const string ERROR_FUNCTION_ID = "0x08c379a0";

        #endregion

        #region Propertys

        /// <summary>
        /// Message
        /// </summary>
        [Parameter("string")]
        public string Message { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// IsErrorData
        /// </summary>
        /// <param name="dataHex"></param>
        /// <returns></returns>
        public static bool IsErrorData(string dataHex)
        {
            return dataHex.StartsWith(ERROR_FUNCTION_ID);
        }

        #endregion
    }
}