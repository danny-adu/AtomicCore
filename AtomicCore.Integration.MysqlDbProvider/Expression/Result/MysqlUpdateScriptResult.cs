using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using AtomicCore.DbProvider;

namespace AtomicCore.Integration.MysqlDbProvider
{
    /// <summary>
    /// MySql下需要被更新的字段结果
    /// </summary>
    internal class MysqlUpdateScriptResult : ResultBase
    {
        #region Constructors

        private IDbMappingHandler _dbMappingHanlder = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbMappingHanlder">需要字段解析者</param>
        private MysqlUpdateScriptResult(IDbMappingHandler dbMappingHanlder)
            : base()
        {
            if (dbMappingHanlder != null)
            {
                this._fieldMembers = new List<MysqlUpdateField>();
                this._dbMappingHanlder = dbMappingHanlder;
            }
            else
            {
                this.AppendError("IDBMappingHandler接口实例不允许为null");
            }
        }

        #endregion

        #region Propertys

        private List<MysqlUpdateField> _fieldMembers = null;

        /// <summary>
        /// 要被查询的字段集合
        /// </summary>
        public IEnumerable<MysqlUpdateField> FieldMembers
        {
            get { return this._fieldMembers; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 构造实例
        /// </summary>
        /// <param name="dbMappingHanlder"></param>
        /// <returns></returns>
        public static MysqlUpdateScriptResult Create(IDbMappingHandler dbMappingHanlder)
        {
            return new MysqlUpdateScriptResult(dbMappingHanlder);
        }

        /// <summary>
        /// 新增要被查询的字段
        /// </summary>
        /// <param name="memberInfo">IModel中的成员信息</param>
        /// <param name="RightTextFragment"></param>
        /// <param name="parameterItem"></param>
        public void AddFieldMember(MemberInfo memberInfo, string RightTextFragment, IEnumerable<MysqlParameterDesc> parameterItem = null)
        {
            if (this.IsAvailable() && memberInfo is PropertyInfo)
            {
                PropertyInfo p = memberInfo as PropertyInfo;
                MysqlUpdateField item = new MysqlUpdateField();
                item.PropertyItem = p;
                item.UpdateTextFragment = RightTextFragment;
                item.Parameter = parameterItem;

                this._fieldMembers.Add(item);
            }
        }

        #endregion
    }
}
