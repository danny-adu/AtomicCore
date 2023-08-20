using System.Collections.Generic;
using System.Linq.Expressions;
using AtomicCore.DbProvider;

namespace AtomicCore.Integration.MssqlDbProvider
{
    /// <summary>
    /// SqlServer下解析需要被Update的字段
    /// </summary>
    internal class Mssql2008UpdateScriptHandler : ExpressionVisitorBase
    {
        #region Constructors

        private readonly IDbMappingHandler _dbMappingHanlder = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbMappingHanlder"></param>
        private Mssql2008UpdateScriptHandler(IDbMappingHandler dbMappingHanlder)
        {
            this._dbMappingHanlder = dbMappingHanlder;
            this._result = Mssql2008UpdateScriptResult.Create(dbMappingHanlder);
        }

        #endregion

        #region Propertys

        private readonly Mssql2008UpdateScriptResult _result = null;

        /// <summary>
        /// 解析后的结果集
        /// </summary>
        public Mssql2008UpdateScriptResult Result
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
                Mssql2008ConditionNodeResult result = Mssql2008ConditionNodeHandler.ExecuteResolver(this._dbMappingHanlder, assignment.Expression, false);
                if (result.IsAvailable())
                    this._result.AddFieldMember(assignment.Member, result.TextValue, result.Parameters);
            }
            else
            {
                //如果不包含参数，则直接计算出更新的值
                string paramName = MssqlGrammarRule.GetUniqueIdentifier();
                string paramText = MssqlGrammarRule.GenerateParamName(paramName);
                object updateValue = ExpressionCalculater.GetValue(assignment.Expression);

                MssqlParameterDesc item = new MssqlParameterDesc(paramName, updateValue);
                this._result.AddFieldMember(assignment.Member, paramText, new List<MssqlParameterDesc>() { item });
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
        public static Mssql2008UpdateScriptResult ExecuteResolver(Expression exp, IDbMappingHandler dbMappingHanlder)
        {
            //执行表达式解析 要被查询的字段
            Mssql2008UpdateScriptHandler entity = new Mssql2008UpdateScriptHandler(dbMappingHanlder);
            entity.Visit(exp);

            return entity.Result;
        }

        #endregion
    }
}
