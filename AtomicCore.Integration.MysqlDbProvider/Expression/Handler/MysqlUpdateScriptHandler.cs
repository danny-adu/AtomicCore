using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using AtomicCore.DbProvider;

namespace AtomicCore.Integration.MysqlDbProvider
{
    /// <summary>
    /// MySql下解析需要被Update的字段
    /// </summary>
    internal class MysqlUpdateScriptHandler : ExpressionVisitorBase
    {
        #region Constructors

        private IDbMappingHandler _dbMappingHanlder = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbMappingHanlder"></param>
        private MysqlUpdateScriptHandler(IDbMappingHandler dbMappingHanlder)
        {
            this._dbMappingHanlder = dbMappingHanlder;
            this._result = MysqlUpdateScriptResult.Create(dbMappingHanlder);
        }

        #endregion

        #region Propertys

        private MysqlUpdateScriptResult _result = null;

        /// <summary>
        /// 解析后的结果集
        /// </summary>
        public MysqlUpdateScriptResult Result
        {
            get { return this._result; }
        }

        #endregion

        #region ExpressionVisitor

        /// <summary>
        /// 访问成员值指定
        /// </summary>
        /// <param name="assignment"></param>
        /// <param name="isStackPush"></param>
        /// <returns></returns>
        protected override MemberAssignment VisitMemberAssignment(MemberAssignment assignment, bool isStackPush = true)
        {
            if (ExpressionCalculater.IsExistsParameters(assignment.Expression))
            {
                //如果包含参数，则需要计算出更新表达式
                MysqlConditionNodeResult result = MysqlConditionNodeHandler.ExecuteResolver(this._dbMappingHanlder, assignment.Expression, false);
                if (result.IsAvailable())
                {
                    this._result.AddFieldMember(assignment.Member, result.TextValue, result.Parameters);
                }
            }
            else
            {
                //如果不包含参数，则直接计算出更新的值
                string paramName = MysqlGrammarRule.GetUniqueIdentifier();
                string paramText = MysqlGrammarRule.GenerateParamName(paramName);
                object updateValue = ExpressionCalculater.GetValue(assignment.Expression);

                MysqlParameterDesc item = new MysqlParameterDesc(paramName, updateValue);
                this._result.AddFieldMember(assignment.Member, paramText, new List<MysqlParameterDesc>() { item });
            }
            return base.VisitMemberAssignment(assignment);
        }


        #endregion

        #region Methods

        /// <summary>
        /// 执行解析获取需要被查询的字段
        /// </summary>
        /// <param name="exp">表达式</param>
        /// <param name="dbMappingHanlder">字段解析者实例</param>
        /// <returns></returns>
        public static MysqlUpdateScriptResult ExecuteResolver(Expression exp, IDbMappingHandler dbMappingHanlder)
        {
            //执行表达式解析 要被查询的字段
            MysqlUpdateScriptHandler entity = new MysqlUpdateScriptHandler(dbMappingHanlder);
            entity.Visit(exp);
            return entity.Result;
        }

        #endregion
    }
}
