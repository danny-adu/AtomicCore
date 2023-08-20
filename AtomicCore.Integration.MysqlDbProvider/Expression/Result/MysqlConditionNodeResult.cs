using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AtomicCore.DbProvider;

namespace AtomicCore.Integration.MysqlDbProvider
{
    /// <summary>
    /// Mssql条件表达式解析处理结果
    /// </summary>
    internal class MysqlConditionNodeResult : ResultBase
    {
        #region Construtors

        /// <summary>
        /// 构造函数
        /// </summary>
        private MysqlConditionNodeResult()
            : base()
        {
            this._textValue = new StringBuilder();
            this._parameters = new List<MysqlParameterDesc>();
        }

        #endregion

        #region Propertys

        private StringBuilder _textValue = null;
        private List<MysqlParameterDesc> _parameters = null;

        /// <summary>
        /// 文本值
        /// </summary>
        public string TextValue
        {
            get { return this._textValue.ToString(); }
        }

        /// <summary>
        /// 参数集合
        /// </summary>
        public IEnumerable<MysqlParameterDesc> Parameters
        {
            get { return this._parameters; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 新增参数
        /// </summary>
        /// <param name="item"></param>
        public void AddParameter(MysqlParameterDesc item)
        {
            this._parameters.Add(item);
        }

        /// <summary>
        /// 新增参数
        /// </summary>
        /// <param name="index"></param>
        /// <param name="items"></param>
        public void InsertParameterRange(int index, IEnumerable<MysqlParameterDesc> items)
        {
            this._parameters.InsertRange(index, items);
        }

        /// <summary>
        /// 追加文本字符串
        /// </summary>
        /// <param name="val"></param>
        public void AppendText(string val)
        {
            this._textValue.Append(val);
        }

        /// <summary>
        /// 构造结果集实例
        /// </summary>
        /// <returns></returns>
        public static MysqlConditionNodeResult Create()
        {
            return new MysqlConditionNodeResult();
        }

        #endregion
    }
}
