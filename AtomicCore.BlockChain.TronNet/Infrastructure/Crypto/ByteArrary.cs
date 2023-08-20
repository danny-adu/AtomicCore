using System;
using System.Collections.Generic;
using System.Linq;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Byte Array Class
    /// </summary>
    public static class ByteArrary
    {
        #region Variables

        private const string HEX_PREFIX = "0x";
        private static readonly byte[] Empty = Array.Empty<byte>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Byte => Hex
        /// </summary>
        /// <param name="value"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string ToHex(this byte[] value, bool prefix = false)
        {
            string strPrex = prefix ? HEX_PREFIX : string.Empty;

            return strPrex + string.Concat(value.Select(b => b.ToString("x2")).ToArray());
        }

        /// <summary>
        /// Has HexPrefix
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasHexPrefix(this string value)
        {
            return value.StartsWith(HEX_PREFIX, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Is Hex
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsHex(this string value)
        {
            bool isHex;
            foreach (var c in value.RemoveHexPrefix())
            {
                isHex = (c >= '0' && c <= '9') ||
                         (c >= 'a' && c <= 'f') ||
                         (c >= 'A' && c <= 'F');

                if (!isHex)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Remove HexPrefix
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string RemoveHexPrefix(this string value)
        {
            return value.Substring(value.StartsWith(HEX_PREFIX, StringComparison.OrdinalIgnoreCase) ? 2 : 0);
        }

        /// <summary>
        /// Is The Same Hex
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool IsTheSameHex(this string first, string second)
        {
            return string.Equals(EnsureHexPrefix(first).ToLower(), EnsureHexPrefix(second).ToLower(),
                StringComparison.Ordinal);
        }

        /// <summary>
        /// Ensure HexPrefix
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EnsureHexPrefix(this string value)
        {
            if (value == null) return null;
            if (!value.HasHexPrefix())
                return string.Format("{0}{1}", HEX_PREFIX, value);

            return value;
        }

        /// <summary>
        /// Ensure HexPrefix
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string[] EnsureHexPrefix(this string[] values)
        {
            if (values != null)
                foreach (var value in values)
                    value.EnsureHexPrefix();

            return values;
        }

        /// <summary>
        /// ToHex Compact
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToHexCompact(this byte[] value)
        {
            return ToHex(value).TrimStart('0');
        }

        /// <summary>
        /// Hex To ByteArray Internal
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static byte[] HexToByteArrayInternal(string value)
        {
            byte[] bytes;
            if (string.IsNullOrEmpty(value))
                bytes = Empty;
            else
            {
                int string_length = value.Length;
                int character_index = value.StartsWith(HEX_PREFIX, StringComparison.Ordinal) ? 2 : 0;
                int number_of_characters = string_length - character_index;

                bool add_leading_zero = false;
                if (0 != number_of_characters % 2)
                {
                    add_leading_zero = true;
                    number_of_characters += 1;
                }

                bytes = new byte[number_of_characters / 2];
                int write_index = 0;
                if (add_leading_zero)
                {
                    bytes[write_index++] = FromCharacterToByte(value[character_index], character_index);
                    character_index += 1;
                }

                for (int read_index = character_index; read_index < value.Length; read_index += 2)
                {
                    byte upper = FromCharacterToByte(value[read_index], read_index, 4);
                    byte lower = FromCharacterToByte(value[read_index + 1], read_index + 1);

                    bytes[write_index++] = (byte)(upper | lower);
                }
            }

            return bytes;
        }

        /// <summary>
        /// Hex To ByteArray
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] HexToByteArray(this string value)
        {
            try
            {
                return HexToByteArrayInternal(value);
            }
            catch (FormatException ex)
            {
                throw new FormatException(string.Format(
                    "String '{0}' could not be converted to byte array (not hex?).", value), ex);
            }
        }

        /// <summary>
        /// From Character To Byte
        /// </summary>
        /// <param name="character"></param>
        /// <param name="index"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        private static byte FromCharacterToByte(char character, int index, int shift = 0)
        {
            byte value = (byte)character;
            if (0x40 < value && 0x47 > value || 0x60 < value && 0x67 > value)
            {
                if (0x40 == (0x40 & value))
                    if (0x20 == (0x20 & value))
                        value = (byte)((value + 0xA - 0x61) << shift);
                    else
                        value = (byte)((value + 0xA - 0x41) << shift);
            }
            else if (0x29 < value && 0x40 > value)
            {
                value = (byte)((value - 0x30) << shift);
            }
            else
            {
                throw new FormatException(string.Format(
                    "Character '{0}' at index '{1}' is not valid alphanumeric character.", character, index));
            }

            return value;
        }

        /// <summary>
        /// Merge
        /// </summary>
        /// <param name="arrays"></param>
        /// <returns></returns>
        public static byte[] Merge(params byte[][] arrays)
        {
            return MergeToEnum(arrays).ToArray();
        }

        /// <summary>
        /// Merge To Enum
        /// </summary>
        /// <param name="arrays"></param>
        /// <returns></returns>
        private static IEnumerable<byte> MergeToEnum(params byte[][] arrays)
        {
            foreach (var a in arrays)
                foreach (var b in a)
                    yield return b;
        }

        #endregion
    }
}
