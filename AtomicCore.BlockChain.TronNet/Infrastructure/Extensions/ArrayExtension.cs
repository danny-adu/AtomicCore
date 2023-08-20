using System.Linq;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Array Extension
    /// </summary>
    public static class ArrayExtension
    {
        /// <summary>
        /// Sub Array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static T[] SubArray<T>(this T[] array, int offset, int length)
        {
            return array.Skip(offset)
                        .Take(length)
                        .ToArray();
        }
    }
}
