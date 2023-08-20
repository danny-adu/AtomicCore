using System;

namespace AtomicCore.Integration.MssqlDbProvider
{
    /// <summary>
    /// Mssql参数类型枚举定义
    /// </summary>
    [Flags]
    public enum MssqlParameterDirection
    {
        /// <summary>
        /// 不设置该参数
        /// </summary>
        None = 0,

        /// <summary>
        /// 参数是输入参数。
        /// </summary>
        Input = 1,

        /// <summary>
        /// 参数是输出参数。
        /// </summary>
        Output = 2,

        /// <summary>
        /// 参数既能输入，也能输出
        /// </summary>
        InputOutput = MssqlParameterDirection.Input | MssqlParameterDirection.Output
    }
}
