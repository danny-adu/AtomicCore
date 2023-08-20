using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtomicCore.Integration.MssqlDbProvider
{
    /// <summary>
    /// Mssql语句解析结果集
    /// </summary>
    internal sealed class Mssql2008SentenceResult : ResultBase
    {
        #region Variable

        /// <summary>
        /// 默认起始页
        /// </summary>
        public const int DEFAULT_CURRENTPAGE = 1;
        /// <summary>
        /// 默认每页多少条数据
        /// </summary>
        public const int DEFAULT_PAGESIZE = 20;

        #endregion

        #region Construtors

        /// <summary>
        /// 构造函数
        /// </summary>
        private Mssql2008SentenceResult()
            : base()
        {
            this._sqlPagerCondition = new KeyValuePair<int, int>(DEFAULT_CURRENTPAGE, DEFAULT_PAGESIZE);

            this._sqlOrderConditionList = new List<string>();
        }

        #endregion

        #region Propertys

        private List<MssqlSelectField> _sqlSelectFields = null;
        private KeyValuePair<int, int>? _sqlPagerCondition = null;//仅仅允许构造后一次
        private StringBuilder _sqlWhereConditionBuilder = null;//仅仅允许构造后一次
        private List<MssqlParameterDesc> _sqlQuerylParameters = null;
        private List<string> _sqlOrderConditionList = null;//排序集合(解析是从右向左边,所以每次添加需要Insert At 0)

        //private StringBuilder _sqlOrderConditionBuilder = null;//允许构造后多次叠加
        private StringBuilder _sqlGroupConditionBuilder = null;//允许构造后多次叠加

        /// <summary>
        /// Sql查询中出现的需要查询的字段
        /// </summary>
        public IEnumerable<MssqlSelectField> SqlSelectFields
        {
            get
            {
                return this._sqlSelectFields;
            }
        }

        /// <summary>
        /// 分页参数条件，Key:currentPage,Value:PageSize,默认指定1，20
        /// </summary>
        public KeyValuePair<int, int> SqlPagerCondition
        {
            get
            {
                return this._sqlPagerCondition.Value;
            }
        }

        /// <summary>
        /// Sql查询中where条件结果文本
        /// </summary>
        public string SqlWhereConditionText
        {
            get
            {
                if (this._sqlWhereConditionBuilder == null)
                {
                    return null;
                }
                else
                {
                    return this._sqlWhereConditionBuilder.ToString();
                }
            }
        }

        /// <summary>
        /// Sql查询中出现的参数集合
        /// </summary>
        public IEnumerable<MssqlParameterDesc> SqlQuerylParameters
        {
            get
            {
                return this._sqlQuerylParameters;
            }
        }

        /// <summary>
        /// Sql查询中OrderBy条件
        /// </summary>
        public string SqlOrderConditionText
        {
            get
            {
                if (!this._sqlOrderConditionList.Any())
                    return null;
                else
                    return string.Join(",", this._sqlOrderConditionList);
            }
        }

        /// <summary>
        /// Sql中分组条件
        /// </summary>
        public string SqlGroupConditionBuilder
        {
            get
            {
                if (this._sqlGroupConditionBuilder == null)
                    return null;
                else
                    return this._sqlGroupConditionBuilder.ToString();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 构造实例
        /// </summary>
        /// <returns></returns>
        public static Mssql2008SentenceResult Create()
        {
            return new Mssql2008SentenceResult();
        }

        /// <summary>
        /// 追加要被查询的字段
        /// </summary>
        /// <param name="field">出现要被查询的字段</param>
        public void SetSelectField(MssqlSelectField field)
        {
            if (field == null)
            {
                this.AppendError("被指定的查询字段不允许为null");
            }
            else
            {
                if (this._sqlSelectFields == null)
                {
                    this._sqlSelectFields = new List<MssqlSelectField>();
                }
                this._sqlSelectFields.Add(field);
            }
        }

        /// <summary>
        /// 追加需要被查询的字段
        /// </summary>
        /// <param name="fields">出现要被查询的字段</param>
        public void SetSelectField(IEnumerable<MssqlSelectField> fields)
        {
            if (fields == null || fields.Count() <= 0)
            {
                this.AppendError("必须指定1~多个被查询字段");
            }
            else
            {
                if (this._sqlSelectFields == null)
                {
                    this._sqlSelectFields = new List<MssqlSelectField>();
                }
                this._sqlSelectFields.AddRange(fields);
            }
        }

        /// <summary>
        /// 设置分页条件
        /// </summary>
        /// <param name="currentPage">当前页索引</param>
        /// <param name="pageSize">每页页容</param>
        public void SetPageCondition(int currentPage = 1, int pageSize = 20)
        {
            this._sqlPagerCondition = new KeyValuePair<int, int>(currentPage, pageSize);
        }

        /// <summary>
        /// 设置Where条件(不包含 where )
        /// </summary>
        /// <param name="whereText">where条件文本</param>
        /// <param name="parameters">where条件参数</param>
        public void SetWhereCondition(string whereText, IEnumerable<MssqlParameterDesc> parameters = null)
        {
            if (!string.IsNullOrEmpty(whereText))
            {
                if (this._sqlWhereConditionBuilder == null)
                {
                    this._sqlWhereConditionBuilder = new StringBuilder();
                }
                this._sqlWhereConditionBuilder.Append(whereText);
            }
            if (parameters != null)
            {
                if (this._sqlQuerylParameters == null)
                {
                    this._sqlQuerylParameters = new List<MssqlParameterDesc>();
                }
                this._sqlQuerylParameters.AddRange(parameters);
            }
        }

        /// <summary>
        /// 追加orderBy条件(不包含order by)
        /// </summary>
        /// <param name="fieldName">排序字段</param>
        /// <param name="isAsc">是否是正序</param>
        public void SetOrderCondition(string fieldName, bool isAsc)
        {
            this._sqlOrderConditionList.Insert(0, string.Format(" {0} {1}", fieldName, isAsc ? "asc" : "desc"));

            //if (this._sqlOrderConditionBuilder == null)
            //{
            //    this._sqlOrderConditionBuilder = new StringBuilder(" ");
            //    this._sqlOrderConditionBuilder.Append(fieldName);
            //    this._sqlOrderConditionBuilder.Append(" ");
            //    this._sqlOrderConditionBuilder.Append(isAsc ? "asc" : "desc");
            //}
            //else
            //{
            //    this._sqlOrderConditionBuilder.Append(",");
            //    this._sqlOrderConditionBuilder.Append(fieldName);
            //    this._sqlOrderConditionBuilder.Append(" ");
            //    this._sqlOrderConditionBuilder.Append(isAsc ? "asc" : "desc");
            //}
        }

        /// <summary>
        /// 设置排序条件(不包含group by)
        /// </summary>
        /// <param name="fieldName"></param>
        public void SetGroupCondition(string fieldName)
        {
            if (this._sqlGroupConditionBuilder == null)
            {
                this._sqlGroupConditionBuilder = new StringBuilder(" ");
                this._sqlGroupConditionBuilder.Append(fieldName);
            }
            else
            {
                this._sqlGroupConditionBuilder.Append(",");
                this._sqlGroupConditionBuilder.Append(fieldName);
            }
        }

        #endregion
    }
}
