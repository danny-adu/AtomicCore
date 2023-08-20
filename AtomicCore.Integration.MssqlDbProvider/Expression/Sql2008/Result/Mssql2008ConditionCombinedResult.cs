using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AtomicCore.Integration.MssqlDbProvider
{
    /// <summary>
    /// SqlSerivce下条件组合解析结果集
    /// </summary>
    internal class Mssql2008ConditionCombinedResult : ResultBase
    {
        #region Construtors

        /// <summary>
        /// 构造函数
        /// </summary>
        public Mssql2008ConditionCombinedResult()
            : base()
        {
            this._architectureTemp = new StringBuilder();
            this._architectureParams = new Dictionary<string, Expression>();
        }

        #endregion

        #region Propertys

        private StringBuilder _architectureTemp = null;
        private Dictionary<string, Expression> _architectureParams = null;

        /// <summary>
        /// 模版语句（包含占位符）
        /// </summary>
        public string ArchitectureTemp
        {
            get { return this._architectureTemp.ToString(); }
        }

        /// <summary>
        /// 模版占位符参数（Key:为Text中的占位符，Value:为Text中占位符对应的Expression）
        /// </summary>
        public IEnumerable<KeyValuePair<string, Expression>> ArchitectureParams
        {
            get { return this._architectureParams; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 构造实例
        /// </summary>
        /// <returns></returns>
        public static Mssql2008ConditionCombinedResult Create()
        {
            return new Mssql2008ConditionCombinedResult();
        }

        /// <summary>
        /// 追加框架模版的脚本
        /// </summary>
        /// <param name="val">需要被追加的模版字符串</param>
        public void AppendArchitectureTemp(string val)
        {
            this._architectureTemp.Append(val);
        }

        /// <summary>
        /// 追加框架的占位符参数与占位符的具体对应的表达式
        /// </summary>
        /// <param name="paramName">占位符参数名称</param>
        /// <param name="paramExp">占位符的表达式值</param>
        public void AppendArchitectureParameter(string paramName, Expression paramExp)
        {
            this._architectureParams.Add(paramName, paramExp);
        }

        #endregion
    }
}
