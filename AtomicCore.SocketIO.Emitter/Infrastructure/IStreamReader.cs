using System.IO;

namespace AtomicCore.SocketIO.Emitter
{
    /// <summary>
    /// IStream Reader
    /// </summary>
    internal interface IStreamReader
    {
        /// <summary>
        /// Read To End
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        byte[] ReadToEnd(Stream stream);
    }
}