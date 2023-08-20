using System;
using System.Collections.Generic;
using System.Linq;

namespace AtomicCore
{
    /// <summary>
    /// 返回的结果集的抽象定义
    /// </summary>
    public abstract class ResultBase : IResult
    {
        #region Constructors

        /// <summary>
        /// 构造函数
        /// </summary>
        public ResultBase()
        {
            this._errors = new List<string>();
            this._exceptions = new List<Exception>();
        }

        #endregion

        #region Propertys

        private readonly List<string> _errors = null;
        private readonly List<Exception> _exceptions = null;

        /// <summary>
        /// 业务错误信息
        /// </summary>
        public string[] Errors
        {
            get { return this._errors.ToArray(); }
        }

        /// <summary>
        /// 代码异常集合
        /// </summary>
        public Exception[] Exceptions
        {
            get { return this._exceptions.ToArray(); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 该实例是否可用,默认为无异常无错误信息为可用,支持重写
        /// </summary>
        /// <returns></returns>
        public virtual bool IsAvailable()
        {
            return this._errors.Count == 0 && this._exceptions.Count == 0;
        }

        /// <summary>
        /// 追加一个错误信息
        /// </summary>
        /// <param name="errorMsg"></param>
        public virtual void AppendError(string errorMsg)
        {
            if (!string.IsNullOrEmpty(errorMsg))
            {
                this._errors.Add(errorMsg);
            }
        }

        /// <summary>
        /// 追加多个错误信息
        /// </summary>
        /// <param name="errorMsgs"></param>
        public virtual void AppendError(IEnumerable<string> errorMsgs)
        {
            if (errorMsgs != null && errorMsgs.Count() > 0)
            {
                this._errors.AddRange(errorMsgs);
            }
        }

        /// <summary>
        /// 追加一个异常信息
        /// </summary>
        /// <param name="ex"></param>
        public virtual void AppendException(Exception ex)
        {
            if (ex != null)
            {
                this._exceptions.Add(ex);
            }
        }

        /// <summary>
        /// 追加多个异常信息
        /// </summary>
        /// <param name="exs"></param>
        public virtual void AppendException(IEnumerable<Exception> exs)
        {
            if (exs != null && exs.Count() > 0)
            {
                this._exceptions.AddRange(exs);
            }
        }

        /// <summary>
        /// 拷贝结果集基础状态(Error and Execption)
        /// </summary>
        /// <param name="source">需要拷贝的数据源</param>
        public virtual void CopyStatus(IResult source)
        {
            this._errors.AddRange(source.Errors);
            this._exceptions.AddRange(source.Exceptions);
        }

        ///// <summary>
        ///// 返回默认的第一个错误信息
        ///// </summary>
        ///// <param name="format">错误模版</param>
        ///// <returns></returns>
        //public virtual string GetError(string format = null)
        //{
        //    if (string.IsNullOrEmpty(format))
        //        return this.Errors.FirstOrDefault();
        //    else
        //        return string.Format(format, this.Errors.FirstOrDefault());
        //}

        #endregion
    }
}
