using System.Collections.Generic;
using System.Linq.Expressions;
using AtomicCore.DbProvider;

namespace AtomicCore.Integration.ClickHouseDbProvider
{
    /// <summary>
    /// SqlServer下解析需要被Update的字段
    /// </summary>
    internal class ClickHouseUpdateScriptHandler : ExpressionVisitorBase
    {
        #region Constructors

        private readonly IDbMappingHandler _dbMappingHanlder = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbMappingHanlder"></param>
        private ClickHouseUpdateScriptHandler(IDbMappingHandler dbMappingHanlder)
        {
            this._dbMappingHanlder = dbMappingHanlder;
            this._result = ClickHouseUpdateScriptResult.Create(dbMappingHanlder);
        }

        #endregion

        #region Propertys

        private readonly ClickHouseUpdateScriptResult _result = null;

        /// <summary>
        /// 解析后的结果集
        /// </summary>
        public ClickHouseUpdateScriptResult Result
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
                ClickHouseConditionNodeResult result = ClickHouseConditionNodeHandler.ExecuteResolver(this._dbMappingHanlder, assignment.Expression, false);
                if (result.IsAvailable())
                    this._result.AddFieldMember(assignment.Member, result.TextValue, result.Parameters);
            }
            else
            {
                //如果不包含参数，则直接计算出更新的值(ClickHouse不支持参数化查询，所以这里直接拼接即可)
                object updateValue = ExpressionCalculater.GetValue(assignment.Expression);
                var rightTextFragment = ClickHouseGrammarRule.GetSqlText(updateValue);
                this._result.AddFieldMember(assignment.Member, rightTextFragment);


                ////string paramName = ClickHouseGrammarRule.GetUniqueIdentifier();
                ////string paramText = ClickHouseGrammarRule.GenerateParamName(paramName);
                ////object updateValue = ExpressionCalculater.GetValue(assignment.Expression);

                ////ClickHouseParameterDesc item = new ClickHouseParameterDesc(paramName, updateValue);
                ////this._result.AddFieldMember(assignment.Member, paramText, new List<ClickHouseParameterDesc>() { item });
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
        public static ClickHouseUpdateScriptResult ExecuteResolver(Expression exp, IDbMappingHandler dbMappingHanlder)
        {
            //执行表达式解析 要被查询的字段
            ClickHouseUpdateScriptHandler entity = new ClickHouseUpdateScriptHandler(dbMappingHanlder);
            entity.Visit(exp);

            return entity.Result;
        }

        #endregion
    }
}
