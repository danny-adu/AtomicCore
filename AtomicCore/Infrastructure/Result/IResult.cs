using System;
using System.Collections.Generic;

namespace AtomicCore
{
    /// <summary>
    /// 通用结果集接口
    /// </summary>
    public interface IResult
    {
        /// <summary>
        /// 业务错误信息
        /// </summary>
        string[] Errors { get; }

        /// <summary>
        /// 代码异常集合
        /// </summary>
        Exception[] Exceptions { get; }

        /// <summary>
        /// 是否无异常正常可用
        /// </summary>
        /// <returns></returns>
        bool IsAvailable();

        /// <summary>
        /// 新增一个错误信息
        /// </summary>
        /// <param name="errorMsg"></param>
        void AppendError(string errorMsg);

        /// <summary>
        /// 新增一组错误信息
        /// </summary>
        /// <param name="errorMsgs"></param>
        void AppendError(IEnumerable<string> errorMsgs);

        /// <summary>
        /// 新增一个异常对象
        /// </summary>
        /// <param name="ex"></param>
        void AppendException(Exception ex);

        /// <summary>
        /// 新增一组异常对象
        /// </summary>
        /// <param name="exs"></param>
        void AppendException(IEnumerable<Exception> exs);

        /// <summary>
        /// 拷贝错误或异常信息
        /// </summary>
        /// <param name="source"></param>
        void CopyStatus(IResult source);
    }
}
