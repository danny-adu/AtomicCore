using System.Threading.Tasks;

namespace AtomicCore.SocketIO.Emitter
{
    /// <summary>
    /// IEmitter
    /// </summary>
    public interface IEmitter
    {
        /// <summary>
        /// IN
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        IEmitter In(string room);

        /// <summary>
        /// TO
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        IEmitter To(string room);

        /// <summary>
        /// OF
        /// </summary>
        /// <param name="nsp"></param>
        /// <returns></returns>
        IEmitter Of(string nsp);

        /// <summary>
        /// EMIT
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        IEmitter Emit(params object[] args);

        /// <summary>
        /// Emit Async
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        Task<IEmitter> EmitAsync(params object[] args);
    }
}