using System.Collections.Generic;
using System.Linq;
using AtomicCore.DbProvider;

namespace AtomicCore.Integration.ClickHouseDbProvider
{
    /// <summary>
    /// Mssql存储过程执行输入参数类
    /// </summary>
    public sealed class ClickHouseDbExecuteInput : DbExecuteInputBase
    {
        #region Constructors

        /// <summary>
        /// 构造函数
        /// </summary>
        private ClickHouseDbExecuteInput() : base() { }

        #endregion

        #region Methods

        private List<object> _parameters = null;

        /// <summary>
        /// 新增参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        /// <param name="direction">参数类型</param>
        public void AddParameter(string name, object value, ClickHouseParameterDirection direction = ClickHouseParameterDirection.Input)
        {
            ClickHouseParameterDesc parameter = new ClickHouseParameterDesc(name, value, direction);
            if (this._parameters == null)
                this._parameters = new List<object>();
            this._parameters.Add(parameter);
        }

        /// <summary>
        /// 新增参数(先string类型参数)
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        /// <param name="size">长度</param>
        /// <param name="direction">参数类型</param>
        public void AddParameter(string name, object value, int size, ClickHouseParameterDirection direction = ClickHouseParameterDirection.Input)
        {
            ClickHouseParameterDesc parameter = new ClickHouseParameterDesc(name, value, direction, size);
            if (this._parameters == null)
                this._parameters = new List<object>();
            this._parameters.Add(parameter);
        }

        /// <summary>
        /// 新增参数(先decimal类型参数)
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        /// <param name="precision">数字的最大位数</param>
        /// <param name="scale">解析的小数位</param>
        /// <param name="direction">参数类型</param>
        public void AddParameter(string name, object value, byte precision, byte scale, ClickHouseParameterDirection direction = ClickHouseParameterDirection.Input)
        {
            ClickHouseParameterDesc parameter = new ClickHouseParameterDesc(name, value, direction, precision, scale);
            if (this._parameters == null)
                this._parameters = new List<object>();
            this._parameters.Add(parameter);
        }

        /// <summary>
        /// 编辑参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        /// <param name="direction">参数类型</param>
        public void UpdateParameter(string name, object value, ClickHouseParameterDirection direction = ClickHouseParameterDirection.Input)
        {
            if (this._parameters == null)
                return;
            else
            {
                object findItem = this._parameters.FirstOrDefault(d => (d as ClickHouseParameterDesc).Name == name);
                if (null == findItem)
                    return;
                else
                {
                    ClickHouseParameterDesc parameter = findItem as ClickHouseParameterDesc;
                    parameter.Value = value;
                    if (direction != ClickHouseParameterDirection.None)
                        parameter.Direction = direction;
                }
            }
        }

        /// <summary>
        /// 编辑参数（针对STRING类型）
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        /// <param name="size">长度</param>
        /// <param name="direction">参数类型</param>
        public void UpdateParameter(string name, object value, int size, ClickHouseParameterDirection direction = ClickHouseParameterDirection.Input)
        {
            if (this._parameters == null)
                return;
            else
            {
                object findItem = this._parameters.FirstOrDefault(d => (d as ClickHouseParameterDesc).Name == name);
                if (null == findItem)
                    return;
                else
                {
                    ClickHouseParameterDesc parameter = findItem as ClickHouseParameterDesc;
                    parameter.Value = value;
                    if (direction != ClickHouseParameterDirection.None)
                        parameter.Direction = direction;

                    parameter.Size = size;
                }
            }
        }

        /// <summary>
        /// 编辑参数（针对Decimal类型）
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        /// <param name="precision">数字最大位数</param>
        /// <param name="scale">小数范围</param>
        /// <param name="direction">参数类型</param>
        public void UpdateParameter(string name, object value, byte precision, byte scale, ClickHouseParameterDirection direction = ClickHouseParameterDirection.Input)
        {
            if (this._parameters == null)
                return;
            else
            {
                object findItem = this._parameters.FirstOrDefault(d => (d as ClickHouseParameterDesc).Name == name);
                if (null == findItem)
                    return;
                else
                {
                    ClickHouseParameterDesc parameter = findItem as ClickHouseParameterDesc;
                    parameter.Value = value;
                    if (direction != ClickHouseParameterDirection.None)
                        parameter.Direction = direction;

                    parameter.Precision = precision;
                    parameter.Scale = scale;
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
                return null;

            IEnumerable<ClickHouseParameterDesc> realParams = this._parameters.Cast<ClickHouseParameterDesc>();
            if (null == realParams)
                return null;

            ClickHouseParameterDesc signParam = realParams.FirstOrDefault(d => d.Name == paramName);
            if (null == signParam)
                return null;

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
        public static ClickHouseDbExecuteInput Create()
        {
            return new ClickHouseDbExecuteInput();
        }

        #endregion
    }
}
