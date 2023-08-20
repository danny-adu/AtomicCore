using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AtomicCore
{
    /// <summary>
    /// 表达式参数访问
    /// </summary>
    internal sealed class ExpressionParameterVisitor : ExpressionVisitor
    {
        #region Constroutors

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="exp">需要访问解析的表达式</param>
        public ExpressionParameterVisitor(Expression exp)
        {
            this._parameterTypes = new List<Type>();
            this.Visit(exp);
        }

        #endregion

        #region Propertys

        private List<Type> _parameterTypes = null;

        /// <summary>
        /// 当前表达式中存在的参数的个数
        /// </summary>
        public IEnumerable<Type> ParameterTypes
        {
            get { return this._parameterTypes; }
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// 访问参数
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (!this._parameterTypes.Exists(d => d.FullName == node.Type.FullName))
            {
                this._parameterTypes.Add(node.Type);
            }

            return base.VisitParameter(node);
        }

        #endregion
    }
}
