using System.Collections.Generic;
using System.Linq;
using AtomicCore.DbProvider;

namespace AtomicCore.Integration.MysqlDbProvider
{
    /// <summary>
    /// Mysql存储过程执行输入参数类
    /// </summary>
    public sealed  class Mysql2008DbExecuteInput : DbExecuteInputBase
    {
        #region Constructors

        /// <summary>
        /// 构造函数
        /// </summary>
        private Mysql2008DbExecuteInput() : base() { }

        #endregion

        #region Methods

        private List<object> _parameters = null;

        /// <summary>
        /// 新增参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        /// <param name="direction">参数类型</param>
        public void AddParameter(string name, object value, MysqlParameterDirection direction = MysqlParameterDirection.Input)
        {
            MysqlParameterDesc parameter = new MysqlParameterDesc(name, value, direction);
            if (this._parameters == null)
            {
                this._parameters = new List<object>();
            }
            this._parameters.Add(parameter);
        }

        /// <summary>
        /// 编辑参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        public void UpdateParameter(string name, object value)
        {
            this.UpdateParameter(name, value, MysqlParameterDirection.None);
        }

        /// <summary>
        /// 编辑参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        /// <param name="direction">参数类型</param>
        public void UpdateParameter(string name, object value, MysqlParameterDirection direction = MysqlParameterDirection.Input)
        {
            if (this._parameters == null)
            {
                return;
            }
            else
            {
                object findItem = this._parameters.FirstOrDefault(d => (d as MysqlParameterDesc).Name == name);
                if (null == findItem)
                {
                    return;
                }
                else
                {
                    MysqlParameterDesc parameter = findItem as MysqlParameterDesc;
                    parameter.Value = value;
                    if (direction != MysqlParameterDirection.None)
                    {
                        parameter.Direction = direction;
                    }
                }
            }
        }

        /// <summary>
        /// 根据参数名称获取参数值
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public override object GetParamValue(string paramName)
        {
            if (string.IsNullOrEmpty(paramName) || null == this._parameters)
            {
                return null;
            }

            IEnumerable<MysqlParameterDesc> realParams = this._parameters.Cast<MysqlParameterDesc>();
            if (null == realParams)
            {
                return null;
            }
            MysqlParameterDesc signParam = realParams.FirstOrDefault(d => d.Name == paramName);
            if (null == signParam)
            {
                return null;
            }
            return signParam.Value;
        }

        /// <summary>
        /// 返回当前输入的参数装箱集合
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<object> GetParameterCollection()
        {
            return this._parameters;
        }

        /// <summary>
        /// 创建一新的实例
        /// </summary>
        /// <returns></returns>
        public static Mysql2008DbExecuteInput Create()
        {
            return new Mysql2008DbExecuteInput();
        }

        #endregion
    }
}
