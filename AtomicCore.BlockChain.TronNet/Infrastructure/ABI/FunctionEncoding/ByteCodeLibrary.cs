namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// ByteCode Library
    /// </summary>
    public class ByteCodeLibrary
    {
        #region Propertys

        /// <summary>
        /// Address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// PlaceholderKey
        /// </summary>
        public string PlaceholderKey { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create From Path
        /// </summary>
        /// <param name="path">The full path of the solidity file for example: "C:/MyLibrary.sol"</param>
        /// <param name="libraryName">The name of the library "MyLibrary" not "MyLibrary.sol"</param>
        /// <param name="libraryAddress"></param>
        /// <returns></returns>
        public static ByteCodeLibrary CreateFromPath(string path, string libraryName, string libraryAddress)
        {
            path = string.Format("{0}:{1}", path.Replace("\\", "/"), libraryName);
            string placeHolderKey = path.ToKeccakHash().Substring(0, 34);

            return new ByteCodeLibrary()
            {
                Address = libraryAddress,
                PlaceholderKey = placeHolderKey
            };
        }

        #endregion
    }
}