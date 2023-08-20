namespace AtomicCore.SocketIO.Emitter
{
    /// <summary>
    /// Emitter Options
    /// </summary>
    public class EmitterOptions
    {
        /// <summary>
        /// 主机IP
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// KEY
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 请求版本
        /// </summary>
		public EVersion Version = EVersion.V0_9_9;

        /// <summary>
        /// EV版本
        /// </summary>
        public enum EVersion 
        { 
            /// <summary>
            /// Version 0.9.9
            /// </summary>
            V0_9_9, 

            /// <summary>
            /// Version 1.4.4
            /// </summary>
            V1_4_4 
        };
    }
}