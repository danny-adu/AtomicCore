using System.Collections.Generic;
using System.Reflection;
using AtomicCore.DbProvider;

namespace AtomicCore.Integration.ClickHouseDbProvider
{
    /// <summary>
    /// SqlServer下要查询的字段的检索解析结果
    /// </summary>
    internal class ClickHouseSelectCombinedResult : ResultBase
    {
        #region Constructors

        private IDbMappingHandler _dbMappingHanlder = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbMappingHandler">数据映射处理接口实例</param>
        private ClickHouseSelectCombinedResult(IDbMappingHandler dbMappingHandler)
            : base()
        {
            if (null == dbMappingHandler)
            {
                this.AppendError("IDBMappingHandler接口实例不允许为null");
            }
            else
            {
                this._fieldMembers = new List<ClickHouseSelectField>();
                this._dbMappingHanlder = dbMappingHandler;
            }
        }

        #endregion

        #region Propertys

        private List<ClickHouseSelectField> _fieldMembers = null;

        /// <summary>
        /// 要被查询的字段集合
        /// </summary>
        public IEnumerable<ClickHouseSelectField> FieldMembers
        {
            get { return this._fieldMembers; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 构造实例
        /// </summary>
        /// <param name="dbMappingHandler"></param>
        /// <returns></returns>
        public static ClickHouseSelectCombinedResult Create(IDbMappingHandler dbMappingHandler)
        {
            ClickHouseSelectCombinedResult result = new ClickHouseSelectCombinedResult(dbMappingHandler);
            return result;
        }

        /// <summary>
        /// 新增要被查询的字段
        /// </summary>
        /// <param name="memberInfo">IModel中的成员信息</param>
        public void AddFieldMember(MemberInfo memberInfo)
        {
            if (this.IsAvailable() && memberInfo is PropertyInfo)
            {
                PropertyInfo p = memberInfo as PropertyInfo;
                DbColumnAttribute column = this._dbMappingHanlder.GetDbColumnSingle(p.ReflectedType, p.Name);
                if (null == column)
                {
                    this.AppendError(string.Format("无法解析{0}类中的{1}属性,原因：无映射关系", p.ReflectedType.FullName, p.Name));
                    return;
                }

                ClickHouseSelectField item = new ClickHouseSelectField();
                item.DBFieldAsName = column.DbColumnName;
                item.DBSelectFragment = column.DbColumnName;
                item.IsModelProperty = true;

                this._fieldMembers.Add(item);
            }
        }

        #endregion
    }
}
