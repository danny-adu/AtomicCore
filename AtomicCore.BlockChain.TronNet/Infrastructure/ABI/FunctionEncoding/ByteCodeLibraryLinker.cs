namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// ByteCode Library Linker
    /// </summary>
    public class ByteCodeLibraryLinker
    {
        #region Variables

        /// <summary>
        /// CONTAINS PLACEHOLDERS MESSAGE
        /// </summary>
        public static readonly string CONTAINS_PLACEHOLDERS_MESSAGE = 
            $"The byte code contains library address placeholders (prefix: '{ByteCodeConstants.LIBRARY_PLACEHOLDER_PREFIX}', suffix: '{ByteCodeConstants.LIBRARY_PLACEHOLDER_SUFFIX}').";

        #endregion

        #region Public Methods

        /// <summary>
        /// Ensure DoesNot Contain Placeholders
        /// </summary>
        /// <param name="byteCode"></param>
        public static void EnsureDoesNotContainPlaceholders(string byteCode)
        {
            if (ContainsPlaceholders(byteCode))
                throw new System.Exception(CONTAINS_PLACEHOLDERS_MESSAGE);
        }

        /// <summary>
        /// Contains Placeholders
        /// </summary>
        /// <param name="byteCode"></param>
        /// <returns></returns>
        public static bool ContainsPlaceholders(string byteCode)
        {
            if (string.IsNullOrEmpty(byteCode)) return false;

            //for efficiency only check for prefix
            return byteCode.Contains(ByteCodeConstants.LIBRARY_PLACEHOLDER_PREFIX);
        }

        /// <summary>
        /// LinkByte Code
        /// </summary>
        /// <param name="byteCode"></param>
        /// <param name="byteCodeLibraries"></param>
        /// <returns></returns>
        public string LinkByteCode(string byteCode, params ByteCodeLibrary[] byteCodeLibraries)
        {
            foreach (ByteCodeLibrary byteCodeLibrary in byteCodeLibraries)
            {
                var placeholder = CreatePlaceholder(byteCodeLibrary.PlaceholderKey);
                byteCode = byteCode.Replace(placeholder, byteCodeLibrary.Address.RemoveHexPrefix());
            }

            return byteCode;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Create Placeholder
        /// </summary>
        /// <param name="key"></param>
        /// <remarks></remarks>
        private static string CreatePlaceholder(string key)
        {
            return string.Format(
                "{0}{1}{2}",
                ByteCodeConstants.LIBRARY_PLACEHOLDER_PREFIX,
                key,
                ByteCodeConstants.LIBRARY_PLACEHOLDER_SUFFIX
            );
        }

        #endregion
    }
}