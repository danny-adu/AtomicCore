/*
 *  该类的主要职责就是将要解析字符解析出来 
 */
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AtomicCore.DbProvider;

namespace AtomicCore.Integration.MssqlDbProvider
{
    /// <summary>
    /// SqlServicer中解析需要select的内容架构解析
    /// </summary>
    internal sealed class Mssql2008SelectCombinedHandler : ExpressionVisitorBase
    {
        #region Constructors

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbMappingHandler"></param>
        private Mssql2008SelectCombinedHandler(IDbMappingHandler dbMappingHandler)
        {
            this._result = Mssql2008SelectCombinedResult.Create(dbMappingHandler);
        }

        #endregion

        #region Propertys

        private Mssql2008SelectCombinedResult _result = null;

        /// <summary>
        /// 解析后的结果集
        /// </summary>
        public Mssql2008SelectCombinedResult Result
        {
            get { return this._result; }
        }

        #endregion

        #region ExpressionVisitor

        /// <summary>
        /// 访问成员对象初始化重写
        /// </summary>
        /// <param name="node"></param>
        /// <param name="isStackPush"></param>
        /// <returns></returns>
        protected override Expression VisitMemberInit(MemberInitExpression node, bool isStackPush = true)
        {
            IEnumerable<MemberBinding> bindings = this.VisitBindingList(node.Bindings);
            if (bindings.Count() > 0)
            {
                foreach (var item in bindings)
                    this._result.AddFieldMember(item.Member);

                return node.Reduce();
            }

            return base.VisitMemberInit(node);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 执行解析获取需要被查询的字段
        /// </summary>
        /// <param name="exp">表达式</param>
        /// <param name="dbMappingHandler">字段解析者实例</param>
        /// <returns></returns>
        public static Mssql2008SelectCombinedResult ExecuteResolver(Expression exp, IDbMappingHandler dbMappingHandler)
        {
            //执行表达式解析 要被查询的字段
            Mssql2008SelectCombinedHandler entity = new Mssql2008SelectCombinedHandler(dbMappingHandler);
            entity.Visit(exp);

            return entity.Result;
        }

        #endregion
    }
}
