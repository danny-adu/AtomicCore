using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AtomicCore.DbProvider;

namespace AtomicCore.Integration.MysqlDbProvider
{
    /// <summary>
    /// Where表达式解析结果集
    /// </summary>
    internal class MysqlWhereScriptResult : ResultBase
    {
        #region Constructors

        /// <summary>
        /// 构造函数
        /// </summary>
        private MysqlWhereScriptResult()
            : base()
        {
            this._textScript = new StringBuilder();
            this._parameters = new List<MysqlParameterDesc>();
        }

        #endregion

        #region Propertys

        private StringBuilder _textScript = null;
        private List<MysqlParameterDesc> _parameters = null;


        /// <summary>
        /// 参数列表
        /// </summary>
        public List<MysqlParameterDesc> Parameters
        {
            get { return _parameters; }
        }

        /// <summary>
        /// 解析后的脚本
        /// </summary>
        public string TextScript
        {
            get { return _textScript.ToString(); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <returns></returns>
        public static MysqlWhereScriptResult Create()
        {
            return new MysqlWhereScriptResult();
        }

        /// <summary>
        /// 追加脚本文本
        /// </summary>
        /// <param name="appendText"></param>
        public void AppendTextScript(string appendText)
        {
            this._textScript.Append(appendText);
        }

        /// <summary>
        /// 新增参数
        /// </summary>
        /// <param name="item"></param>
        public void AddParameter(MysqlParameterDesc item)
        {
            this._parameters.Add(item);
        }

        /// <summary>
        /// 追加参数集合
        /// </summary>
        /// <param name="items"></param>
        public void AddParameter(IEnumerable<MysqlParameterDesc> items)
        {
            this._parameters.AddRange(items);
        }

        #endregion
    }
}
