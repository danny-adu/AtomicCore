using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// TronNet Untils
    /// </summary>
    public static class TronNetUntils
    {
        #region Variables

        /// <summary>
        /// tron sun value
        /// </summary>
        private static long _sun_unit = 1_000_000L;

        /// <summary>
        /// utc time at 1970-01-01
        /// </summary>
        public static readonly DateTime s_utcTimeAt1970 = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1), TimeZoneInfo.Utc);

        /// <summary>
        /// local time at 1970-01-01
        /// </summary>
        public static readonly DateTime s_localAt1970 = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1), TimeZoneInfo.Local);

        #endregion

        #region Amount Calc

        /// <summary>
        /// Trx to Sun
        /// </summary>
        /// <param name="trx"></param>
        /// <returns></returns>
        public static long TRXToSun(decimal trx)
        {
            return Convert.ToInt64(trx * _sun_unit);
        }

        /// <summary>
        /// Sun to Trx
        /// </summary>
        /// <param name="sun"></param>
        /// <returns></returns>
        public static decimal SunToTRX(long sun)
        {
            return Convert.ToDecimal(sun) / _sun_unit;
        }

        /// <summary>
        /// amount to value
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static long AmountToValue(decimal amount, int decimals)
        {
            return (long)(amount * (decimal)Math.Pow(10, decimals));
        }

        /// <summary>
        /// value to amount
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static decimal ValueToAmount(long value, int decimals)
        {
            return value / (decimal)Math.Pow(10, decimals);
        }

        #endregion

        #region Remove Hex Zero

        /// <summary>
        /// Remove Hex Zero
        /// </summary>
        /// <param name="hexStr">hex string</param>
        /// <param name="strategy">select 'TronNetHexCuteZeroStrategy' enum type</param>
        /// <param name="keepLen">specify overall length</param>
        /// <param name="hexEncoding">hex encoding</param>
        /// <returns></returns>
        public static string RemoveHexZero(string hexStr, TronNetHexCuteZeroStrategy strategy, int keepLen = 0, bool hexEncoding = true)
        {
            if (string.IsNullOrEmpty(hexStr))
                return string.Empty;
            if (hexStr.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                hexStr = hexStr.Substring(2);

            string tmp;
            if (TronNetHexCuteZeroStrategy.Left == strategy)
            {
                tmp = Regex.Replace(hexStr, @"^0+", string.Empty, RegexOptions.IgnoreCase);

                if (keepLen > 0)
                    tmp = tmp.PadLeft(keepLen, '0');
            }
            else if (TronNetHexCuteZeroStrategy.Right == strategy)
            {
                if (keepLen <= 0)
                    throw new Exception(string.Format("rigthLen must be greater than zero in right strategy mode,current value is '{0}'", keepLen));

                tmp = hexStr.Substring(0, keepLen);
            }
            else
                return string.Empty;

            if (hexEncoding)
                return string.Format("0x{0}", tmp);
            else
                return tmp;
        }

        #endregion

        #region Hex Str => Byte && Byte=>Hex Str

        /// <summary>
        /// hex string -> String
        /// </summary>
        /// <param name="hexString">hex string</param>
        /// <param name="encoding">encoding</param>
        /// <returns></returns>
        public static string HexStrToString(string hexString, Encoding encoding = null)
        {
            if (null == encoding)
                encoding = Encoding.UTF8;

            byte[] buffer = HexStrToBuffer(hexString.RemoveHexPrefix());

            return encoding.GetString(buffer);
        }

        /// <summary>
        /// string to hex string
        /// </summary>
        /// <param name="str">string</param>
        /// <param name="isPrefix">include prefix</param>
        /// <param name="encoding">encoding</param>
        /// <returns></returns>
        public static string StringToHexString(string str, bool isPrefix = true, Encoding encoding = null)
        {
            if (null == encoding)
                encoding = Encoding.UTF8;

            if (isPrefix)
                return string.Format("0x{0}", BufferToHexStr(encoding.GetBytes(str)));
            else
                return BufferToHexStr(encoding.GetBytes(str));
        }

        /// <summary> 
        /// hex string -> string buffer 
        /// </summary> 
        /// <param name="hexString">hex string</param> 
        /// <returns></returns> 
        public static byte[] HexStrToBuffer(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";

            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);

            return returnBytes;
        }

        /// <summary> 
        /// string buffer -> hex string
        /// </summary> 
        /// <param name="bytes">string buffer array</param> 
        /// <returns></returns> 
        public static string BufferToHexStr(byte[] bytes)
        {
            StringBuilder strBuilder = new StringBuilder();
            if (bytes != null)
                for (int i = 0; i < bytes.Length; i++)
                    strBuilder.Append(bytes[i].ToString("X2"));

            return strBuilder.ToString();
        }

        #endregion

        #region Chinese=>Hex Str && Hex Str=>Chinese

        /// <summary> 
        /// chinese string -> hex string
        /// </summary> 
        /// <param name="chinese">chinese string</param> 
        /// <param name="encoding">encoding, such as "utf-8", "gb2312"</param> 
        /// <param name="fenge">whether each character is separated by a comma</param> 
        /// <returns></returns> 
        public static string ChineseStrToHexStr(string chinese, Encoding encoding = null, bool fenge = false)
        {
            if ((chinese.Length % 2) != 0)
                chinese += " ";

            if (null == encoding)
                encoding = Encoding.UTF8;
            byte[] bytes = encoding.GetBytes(chinese);

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                strBuilder.Append(string.Format("{0:X}", bytes[i]));
                if (fenge && (i != bytes.Length - 1))
                    strBuilder.Append(string.Format("{0}", ","));
            }

            return strBuilder.ToString().ToLower();
        }

        ///<summary> 
        /// hex string -> chinese string
        /// </summary> 
        /// <param name="hex">hex string</param> 
        /// <param name="encoding">encoding, such as "utf-8", "gb2312"</param> 
        /// <returns></returns> 
        public static string HexStrChineseStr(string hex, Encoding encoding = null)
        {
            if (hex == null)
                throw new ArgumentNullException("hex");

            hex = hex.Replace(",", "");
            hex = hex.Replace("\n", "");
            hex = hex.Replace("\\", "");
            hex = hex.Replace(" ", "");
            if (hex.Length % 2 != 0)
                hex += "20";// key of space 

            //need to convert hex to byte array.
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
                try
                {
                    //every two characters is a byte.
                    bytes[i] = byte.Parse(hex.Substring(i * 2, 2),
                    System.Globalization.NumberStyles.HexNumber);
                }
                catch
                {
                    // Rethrow an exception with custom message. 
                    throw new ArgumentException("hex is not a valid hex number!", "hex");
                }

            if (null == encoding)
                encoding = Encoding.UTF8;

            return encoding.GetString(bytes);
        }

        #endregion

        #region Hex->UINT64 

        /// <summary>
        /// hex string -> ulong (Warning : this method may exceed the UINT64 index range)
        /// </summary>
        /// <param name="hexStr"></param>
        /// <returns></returns>
        public static ulong HexStrToULong(string hexStr)
        {
            ulong u64;
            try
            {
                u64 = Convert.ToUInt64(hexStr, 16);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return u64;
        }

        #endregion

        #region Hex->BigInt

        /// <summary>
        /// hex stirng -> BigInteger
        /// </summary>
        /// <param name="hex">hex string</param>
        /// <param name="isHexLittleEndian">Indicates whether the hex is in reverse order</param>
        /// <returns></returns>
        public static BigInteger HexStrToBigInteger(string hex, bool isHexLittleEndian = false)
        {
            if ("0x0".Equals(hex, StringComparison.OrdinalIgnoreCase)) return BigInteger.Zero;

            byte[] encoded = HexToByteArray(hex);

            if (BitConverter.IsLittleEndian != isHexLittleEndian)
            {
                List<byte> listEncoded = encoded.ToList();
                listEncoded.Insert(0, 0x00);
                encoded = listEncoded.ToArray().Reverse().ToArray();
            }

            return new BigInteger(encoded);
        }

        #endregion

        #region Timestamp

        /// <summary>
        /// local time -> second timestamp
        /// </summary>
        /// <param name="localTime"></param>
        /// <returns></returns>
        public static long LocalDatetimeToSecondTimestamp(DateTime localTime)
        {
            if (localTime <= s_localAt1970)
                return 0;

            return (long)localTime.Subtract(s_localAt1970).TotalSeconds;
        }

        /// <summary>
        /// local time -> millisecond timestamp
        /// </summary>
        /// <param name="localTime"></param>
        /// <returns></returns>
        public static long LocalDatetimeToMillisecondTimestamp(DateTime localTime)
        {
            if (localTime <= s_localAt1970)
                return 0;

            return (long)localTime.Subtract(s_localAt1970).TotalMilliseconds;
        }

        #endregion

        #region Private Method

        /// <summary>
        /// Empty array
        /// </summary>
        private static readonly byte[] s_byteEmpty = new byte[0];

        /// <summary>
        /// Convert Hex string to byte array
        /// </summary>
        /// <param name="hexStr">hex string</param>
        /// <returns></returns>
        private static byte[] HexToByteArray(string hexStr)
        {
            byte[] bytes;
            if (string.IsNullOrEmpty(hexStr))
                bytes = s_byteEmpty;
            else
            {
                int string_length = hexStr.Length;
                int character_index = (hexStr.StartsWith("0x", StringComparison.Ordinal)) ? 2 : 0;
                // Does the string define leading HEX indicator '0x'. Adjust starting index accordingly.               
                int number_of_characters = string_length - character_index;

                bool add_leading_zero = false;
                if (0 != (number_of_characters % 2))
                {
                    add_leading_zero = true;

                    number_of_characters += 1; // Leading '0' has been striped from the string presentation.
                }

                bytes = new byte[number_of_characters / 2]; // Initialize our byte array to hold the converted string.

                int write_index = 0;
                if (add_leading_zero)
                {
                    bytes[write_index++] = FromCharacterToByte(hexStr[character_index], character_index);
                    character_index += 1;
                }

                for (int read_index = character_index; read_index < hexStr.Length; read_index += 2)
                {
                    byte upper = FromCharacterToByte(hexStr[read_index], read_index, 4);
                    byte lower = FromCharacterToByte(hexStr[read_index + 1], read_index + 1);

                    bytes[write_index++] = (byte)(upper | lower);
                }
            }

            return bytes;
        }

        /// <summary>
        /// Transfer Char to byte
        /// </summary>
        /// <param name="character"></param>
        /// <param name="index"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        private static byte FromCharacterToByte(char character, int index, int shift = 0)
        {
            byte value = (byte)character;
            if (((0x40 < value) && (0x47 > value)) || ((0x60 < value) && (0x67 > value)))
            {
                if (0x40 == (0x40 & value))
                {
                    if (0x20 == (0x20 & value))
                        value = (byte)(((value + 0xA) - 0x61) << shift);
                    else
                        value = (byte)(((value + 0xA) - 0x41) << shift);
                }
            }
            else if ((0x29 < value) && (0x40 > value))
                value = (byte)((value - 0x30) << shift);
            else
                throw new InvalidOperationException(String.Format("Character '{0}' at index '{1}' is not valid alphanumeric character.", character, index));

            return value;
        }

        #endregion
    }
}
