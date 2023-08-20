using System;

namespace AtomicCore
{
    /// <summary>
    /// UUID Handler
    /// </summary>
    public static class UUIDHandler
    {
        /// <summary>
        /// Get 16-bit unique string based on GUID
        /// </summary>
        /// <returns></returns>
        public static string GuidTo16String()
        {
            long i = 1;

            foreach (byte b in Guid.NewGuid().ToByteArray())
                i *= b + 1;

            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }

        /// <summary>
        /// Get 32-bit unique string based on GUID
        /// </summary>
        /// <returns></returns>
        public static string GuidTo32String()
        {
            return Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// Get 19-digit unique sequence of numbers based on GUID
        /// </summary>
        /// <returns></returns>
        public static long GuidToLongID()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();

            return BitConverter.ToInt64(buffer, 0);
        }

        /// <summary>
        /// Generates 22 unique numbers available concurrently
        /// </summary>
        /// <returns></returns>
        public static string GenerateUniqueID()
        {
            System.Threading.Thread.Sleep(1); //保证yyyyMMddHHmmssffff唯一
            Random d = new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0));
            string strUnique = DateTime.Now.ToString("yyyyMMddHHmmssffff") + d.Next(1000, 9999);

            return strUnique;
        }
    }
}
