/*
 * 该类的主要职责是处理表达式中常量与参数并存的表达式解析，解析的结果为 sql的表达式（例如：[字段名称]+1），
 * 它主要针对 包含有参数的计算，例如：
 * 1.对参数的名称的替换，
 * 2.以及输出参数的字符串形式
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using AtomicCore.DbProvider;

namespace AtomicCore.Integration.MysqlDbProvider
{
    /// <summary>
    /// MySql下处理表达式中常量与参数并存的表达式解析，解析的结果为 sql的表达式（例如：[字段名称]+1）
    /// </summary>
    internal class MysqlConditionNodeHandler : ExpressionVisitorBase
    {
        #region Constructors

        /// <summary>
        /// 是否在字段前面追加表名
        /// </summary>
        private bool _isFieldWithTableName = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbmapper">Db映射提供者</param>
        /// <param name="isFieldWithTableName">是否在字段前面追加表名</param>
        private MysqlConditionNodeHandler(IDbMappingHandler dbmapper, bool isFieldWithTableName)
            : base()
        {
            this.DBMapperProvider = dbmapper;
            this._isFieldWithTableName = isFieldWithTableName;

            this.Result = MysqlConditionNodeResult.Create();
        }

        #endregion

        #region Propertys

        /// <summary>
        /// Db映射提供者
        /// </summary>
        public IDbMappingHandler DBMapperProvider { get; private set; }

        /// <summary>
        /// 条件解析结果
        /// </summary>
        public MysqlConditionNodeResult Result { get; set; }

        #endregion

        #region ExpressionVisitor

        /// <summary>
        /// 访问二元表达式
        /// </summary>
        /// <param name="node"></param>
        /// <param name="isStackPush"></param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression node, bool isStackPush = true)
        {
            this.ExpressionChains.Push(node);

            string leftValueText = null;
            string rightValueText = null;
            List<MysqlParameterDesc> leftParamsList = null;
            List<MysqlParameterDesc> rightParamsList = null;

            #region 判断左节点是否包含参数

            if (ExpressionCalculater.IsExistsParameters(node.Left))
            {
                MysqlConditionNodeResult leftNodeConditonResult = MysqlConditionNodeHandler.ExecuteResolver(this.DBMapperProvider, node.Left, this._isFieldWithTableName);
                if (!leftNodeConditonResult.IsAvailable())
                {
                    this.Result.CopyStatus(leftNodeConditonResult);
                    this.ExpressionChains.Pop();
                    return node.Reduce();
                }

                leftValueText = leftNodeConditonResult.TextValue;
                leftParamsList = new List<MysqlParameterDesc>(leftNodeConditonResult.Parameters);
            }
            else
            {
                object leftObjectValue = ExpressionCalculater.GetValue(node.Left);

                string paramName = MysqlGrammarRule.GetUniqueIdentifier();
                leftValueText = MysqlGrammarRule.GenerateParamName(paramName);
                leftParamsList = new List<MysqlParameterDesc>();
                leftParamsList.Add(new MysqlParameterDesc(paramName, leftObjectValue));
            }

            #endregion

            #region 判断右节点是否包含参数

            if (ExpressionCalculater.IsExistsParameters(node.Right))
            {
                MysqlConditionNodeResult rightNodeConditonResult = MysqlConditionNodeHandler.ExecuteResolver(this.DBMapperProvider, node.Right, this._isFieldWithTableName);
                if (!rightNodeConditonResult.IsAvailable())
                {
                    this.Result.CopyStatus(rightNodeConditonResult);
                    this.ExpressionChains.Pop();
                    return node.Reduce();
                }

                rightValueText = rightNodeConditonResult.TextValue;
                rightParamsList = new List<MysqlParameterDesc>(rightNodeConditonResult.Parameters);
            }
            else
            {
                object rightObjectValue = ExpressionCalculater.GetValue(node.Right);

                string paramName = MysqlGrammarRule.GetUniqueIdentifier();
                rightValueText = MysqlGrammarRule.GenerateParamName(paramName);
                rightParamsList = new List<MysqlParameterDesc>();
                rightParamsList.Add(new MysqlParameterDesc(paramName, rightObjectValue));
            }

            #endregion

            #region 构建计算

            string textFormat = "({0}{1}{2})";
            if (node.NodeType == ExpressionType.Add)
            {
                this.Result.AppendText(string.Format(textFormat, leftValueText, "+", rightValueText));
            }
            else if (node.NodeType == ExpressionType.Subtract)
            {
                this.Result.AppendText(string.Format(textFormat, leftValueText, "-", rightValueText));
            }
            else if (node.NodeType == ExpressionType.Multiply)
            {
                this.Result.AppendText(string.Format(textFormat, leftValueText, "*", rightValueText));
            }
            else if (node.NodeType == ExpressionType.Divide)
            {
                this.Result.AppendText(string.Format(textFormat, leftValueText, "/", rightValueText));
            }
            else if (node.NodeType == ExpressionType.Modulo)
            {
                this.Result.AppendText(string.Format(textFormat, leftValueText, "%", rightValueText));
            }
            else if (node.NodeType == ExpressionType.And)
            {
                this.Result.AppendText(string.Format(textFormat, leftValueText, "&", rightValueText));
            }
            else if (node.NodeType == ExpressionType.Or)
            {
                this.Result.AppendText(string.Format(textFormat, leftValueText, "|", rightValueText));
            }
            else
            {
                this.Result.AppendError("暂不支持" + node.NodeType.ToString() + "方法的解析");
            }

            #endregion

            #region 组合参数

            this.Result.InsertParameterRange(0, leftParamsList);
            this.Result.InsertParameterRange(0, rightParamsList);

            #endregion

            this.ExpressionChains.Pop();
            return node.Reduce();
        }

        /// <summary>
        /// 访问成员表达式
        /// </summary>
        /// <param name="node"></param>
        /// <param name="isStackPush"></param>
        /// <returns></returns>
        protected override Expression VisitMemberAccess(MemberExpression node, bool isStackPush = true)
        {
            //如果下一级访问的节点类型是参数类型,那么需要作为字段解析而存在
            if (null != node.Expression && ExpressionType.Parameter == node.Expression.NodeType && typeof(IDbModel).IsAssignableFrom(node.Member.DeclaringType))
            {
                DbColumnAttribute columnAttr = this.DBMapperProvider.GetDbColumnSingle(node.Member.DeclaringType, node.Member.Name);
                if (null != columnAttr)
                {
                    string tablePreName = null;
                    if (this._isFieldWithTableName)
                    {
                        tablePreName = this.DBMapperProvider.GetDbTableName((node.Expression as ParameterExpression).Type);
                    }

                    this.Result.AppendText(MysqlGrammarRule.GenerateFieldWrapped(columnAttr.DbColumnName, tablePreName));
                }
            }

            return base.VisitMemberAccess(node);
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// 执行解析带参数的表达式
        /// </summary>
        /// <param name="dbMappingHandler">字段解析接口实例</param>
        /// <param name="expression">表达式</param>
        /// <param name="isFieldWithTableName">是否发在字段前面追加表名</param>
        /// <returns></returns>
        public static MysqlConditionNodeResult ExecuteResolver(IDbMappingHandler dbMappingHandler, Expression expression, bool isFieldWithTableName)
        {
            //构造解析者执行解析
            MysqlConditionNodeHandler entity = new MysqlConditionNodeHandler(dbMappingHandler, isFieldWithTableName);
            entity.Visit(expression);
            return entity.Result;
        }

        #endregion
    }
}
