using System;
using System.IO;
using System.Text;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Base58 Encoder Provider
    /// </summary>
    public static class Base58Encoder
    {
        #region Variables

        public static readonly char[] Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz".ToCharArray();
        private static readonly int[] Indexes = new int[128];

        #endregion

        #region Constructor

        /// <summary>
        /// Static Constructor
        /// </summary>
        static Base58Encoder()
        {
            for (int i = 0; i < Indexes.Length; i++)
                Indexes[i] = -1;

            for (int i = 0; i < Alphabet.Length; i++)
                Indexes[Alphabet[i]] = i;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Div mode 58
        /// </summary>
        /// <param name="number"></param>
        /// <param name="startAt"></param>
        /// <returns></returns>
        private static byte Divmod58(byte[] number, int startAt)
        {
            int remainder = 0;
            for (int i = startAt; i < number.Length; i++)
            {
                int digit256 = number[i] & 0xFF;
                int temp = remainder * 256 + digit256;

                number[i] = (byte)(temp / 58);
                remainder = temp % 58;
            }

            return (byte)remainder;
        }

        /// <summary>
        /// Div mod 256
        /// </summary>
        /// <param name="number58"></param>
        /// <param name="startAt"></param>
        /// <returns></returns>
        private static byte Divmod256(byte[] number58, int startAt)
        {
            int remainder = 0;
            for (int i = startAt; i < number58.Length; i++)
            {
                int digit58 = (int)number58[i] & 0xFF;
                int temp = remainder * 58 + digit58;

                number58[i] = (byte)(temp / 256);
                remainder = temp % 256;
            }

            return (byte)remainder;
        }

        /// <summary>
        /// Copy Of Range
        /// </summary>
        /// <param name="source"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        private static byte[] CopyOfRange(byte[] source, int from, int to)
        {
            byte[] range = new byte[to - from];
            Array.Copy(source, from, range, 0, range.Length);

            return range;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Decode From Base58Check (bytes length is 21)
        /// </summary>
        /// <param name="addressBase58"></param>
        /// <returns></returns>
        public static byte[] DecodeFromBase58Check(string addressBase58)
        {
            if (string.IsNullOrWhiteSpace(addressBase58)) return null;

            byte[] decodeCheck = Decode(addressBase58);
            if (decodeCheck.Length <= 4) return null;

            byte[] decodeData = new byte[decodeCheck.Length - 4];
            Array.Copy(decodeCheck, 0, decodeData, 0, decodeData.Length);
            byte[] hash0 = Hash(Hash(decodeData));

            bool valid = true;
            for (int i = 0; i < 4; i++)
            {
                if (hash0[i] != decodeCheck[decodeData.Length + i])
                {
                    valid = false;
                    break;
                }
            }

            return valid ? decodeData : null;
        }

        /// <summary>
        /// Twice Hash Calc
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] TwiceHash(byte[] input)
        {
            return Hash(Hash(input));
        }

        /// <summary>
        /// Hash
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] Hash(byte[] input)
        {
            System.Security.Cryptography.SHA256Managed hash = new System.Security.Cryptography.SHA256Managed();

            byte[] hashBytes;
            using (MemoryStream stream = new MemoryStream(input))
            {
                try
                {
                    hashBytes = hash.ComputeHash(stream);
                }
                finally
                {
                    hash.Clear();
                }
            }

            return hashBytes;
        }

        /// <summary>
        /// Encode From Hex
        /// </summary>
        /// <param name="hexAddress"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string EncodeFromHex(string hexAddress, byte prefix)
        {
            byte[] hexBytes = hexAddress.HexToByteArray();
            byte[] addressBytes = new byte[21];
            Array.Copy(hexBytes, hexBytes.Length - 20, addressBytes, 1, 20);

            addressBytes[0] = prefix;
            byte[] hash = TwiceHash(addressBytes);
            byte[] bytes = new byte[4];
            Array.Copy(hash, bytes, 4);

            byte[] addressChecksum = new byte[25];
            Array.Copy(addressBytes, 0, addressChecksum, 0, 21);
            Array.Copy(bytes, 0, addressChecksum, 21, 4);

            return Encode(addressChecksum);
        }

        /// <summary>
        /// Encode From Hex
        /// </summary>
        /// <param name="hexBytes"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string EncodeFromHex(byte[] hexBytes, byte prefix)
        {
            byte[] addressBytes = new byte[21];
            Array.Copy(hexBytes, hexBytes.Length - 20, addressBytes, 1, 20);

            addressBytes[0] = prefix;
            byte[] hash = TwiceHash(addressBytes);
            byte[] bytes = new byte[4];
            Array.Copy(hash, bytes, 4);

            byte[] addressChecksum = new byte[25];
            Array.Copy(addressBytes, 0, addressChecksum, 0, 21);
            Array.Copy(bytes, 0, addressChecksum, 21, 4);

            return Encode(addressChecksum);
        }

        /// <summary>
        /// Encode
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Encode(byte[] input)
        {
            if (input.Length == 0)
                return string.Empty;

            input = CopyOfRange(input, 0, input.Length);

            // Count leading zeroes.
            int zeroCount = 0;
            while (zeroCount < input.Length && input[zeroCount] == 0)
                ++zeroCount;

            // The actual encoding.
            byte[] temp = new byte[input.Length * 2];
            int j = temp.Length;

            int startAt = zeroCount;
            while (startAt < input.Length)
            {
                byte mod = Divmod58(input, startAt);
                if (input[startAt] == 0)
                    ++startAt;

                temp[--j] = (byte)Alphabet[mod];
            }

            // Strip extra '1' if there are some after decoding.
            while (j < temp.Length && temp[j] == Alphabet[0])
                ++j;

            // Add as many leading '1' as there were leading zeros.
            while (--zeroCount >= 0)
                temp[--j] = (byte)Alphabet[0];

            byte[] output = CopyOfRange(temp, j, temp.Length);

            return Encoding.ASCII.GetString(output);
        }

        /// <summary>
        /// Decode (bytes length is 25)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] Decode(string input)
        {
            if (input.Length == 0)
                return Array.Empty<byte>();

            // Transform the String to a base58 byte sequence
            byte[] input58 = new byte[input.Length];
            for (int i = 0; i < input.Length; ++i)
            {
                char c = input[i];

                int digit58 = -1;
                if (c >= 0 && c < 128)
                    digit58 = Indexes[c];
                if (digit58 < 0)
                    throw new ArgumentOutOfRangeException("Illegal character " + c + " at " + i);

                input58[i] = (byte)digit58;
            }

            // Count leading zeroes
            int zeroCount = 0;
            while (zeroCount < input58.Length && input58[zeroCount] == 0)
                ++zeroCount;

            // The encoding
            byte[] temp = new byte[input.Length];
            int j = temp.Length;

            int startAt = zeroCount;
            while (startAt < input58.Length)
            {
                byte mod = Divmod256(input58, startAt);
                if (input58[startAt] == 0)
                    ++startAt;

                temp[--j] = mod;
            }

            // Do no add extra leading zeroes, move j to first non null byte.
            while (j < temp.Length && temp[j] == 0)
                ++j;

            return CopyOfRange(temp, j - zeroCount, temp.Length);
        }

        #endregion
    }
}
