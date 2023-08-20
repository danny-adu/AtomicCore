using System.Collections.Generic;
using System.Text;

namespace AtomicCore.Integration.MssqlDbProvider
{
    /// <summary>
    /// Mssql条件表达式解析处理结果
    /// </summary>
    internal class Mssql2008ConditionNodeResult : ResultBase
    {
        #region Construtors

        /// <summary>
        /// 构造函数
        /// </summary>
        private Mssql2008ConditionNodeResult()
            : base()
        {
            this._textValue = new StringBuilder();
            this._parameters = new List<MssqlParameterDesc>();
        }

        #endregion

        #region Propertys

        private StringBuilder _textValue = null;
        private List<MssqlParameterDesc> _parameters = null;

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
        public IEnumerable<MssqlParameterDesc> Parameters
        {
            get { return this._parameters; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 新增参数
        /// </summary>
        /// <param name="item"></param>
        public void AddParameter(MssqlParameterDesc item)
        {
            this._parameters.Add(item);
        }

        /// <summary>
        /// 新增参数
        /// </summary>
        /// <param name="index"></param>
        /// <param name="items"></param>
        public void InsertParameterRange(int index, IEnumerable<MssqlParameterDesc> items)
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
        public static Mssql2008ConditionNodeResult Create()
        {
            return new Mssql2008ConditionNodeResult();
        }

        #endregion
    }
}
