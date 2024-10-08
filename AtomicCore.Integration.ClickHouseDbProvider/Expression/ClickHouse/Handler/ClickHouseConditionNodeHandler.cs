﻿/*
 * 该类的主要职责是处理表达式中常量与参数并存的表达式解析，解析的结果为 sql的表达式（例如：[字段名称]+1），
 * 它主要针对 包含有参数的计算，例如：
 * 1.对参数的名称的替换，
 * 2.以及输出参数的字符串形式
 */
using System.Collections.Generic;
using System.Linq.Expressions;
using AtomicCore.DbProvider;

namespace AtomicCore.Integration.ClickHouseDbProvider
{
    /// <summary>
    /// sql下处理表达式中常量与参数并存的表达式解析，解析的结果为 sql的表达式（例如：[字段名称]+1）
    /// </summary>
    internal class ClickHouseConditionNodeHandler : ExpressionVisitorBase
    {
        #region Constructors

        /// <summary>
        /// 是否在字段前面追加表名
        /// </summary>
        private readonly bool _isFieldWithTableName = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbmapper">Db映射提供者</param>
        /// <param name="isFieldWithTableName">是否在字段前面追加表名</param>
        private ClickHouseConditionNodeHandler(IDbMappingHandler dbmapper, bool isFieldWithTableName)
            : base()
        {
            this.DBMapperProvider = dbmapper;
            this._isFieldWithTableName = isFieldWithTableName;

            this.Result = ClickHouseConditionNodeResult.Create();
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
        public ClickHouseConditionNodeResult Result { get; set; }

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

            string leftValueText;
            string rightValueText;
            //List<ClickHouseParameterDesc> leftParamsList;
            //List<ClickHouseParameterDesc> rightParamsList;

            #region 判断左节点是否包含参数

            if (ExpressionCalculater.IsExistsParameters(node.Left))
            {
                ClickHouseConditionNodeResult leftNodeConditonResult = ExecuteResolver(this.DBMapperProvider, node.Left, this._isFieldWithTableName);
                if (!leftNodeConditonResult.IsAvailable())
                {
                    this.Result.CopyStatus(leftNodeConditonResult);
                    this.ExpressionChains.Pop();
                    return node.Reduce();
                }

                leftValueText = leftNodeConditonResult.TextValue;
                //leftParamsList = new List<ClickHouseParameterDesc>(leftNodeConditonResult.Parameters);
            }
            else
            {
                // 直接拼接
                object leftObjectValue = ExpressionCalculater.GetValue(node.Left);
                leftValueText = ClickHouseGrammarRule.GetSqlText(leftObjectValue);


                // clickhouse 不支持参数化
                //string paramName = ClickHouseGrammarRule.GetUniqueIdentifier();
                //leftValueText = ClickHouseGrammarRule.GenerateParamName(paramName);
                //leftParamsList = new List<ClickHouseParameterDesc>
                //{
                //    new ClickHouseParameterDesc(paramName, leftObjectValue)
                //};
            }

            #endregion

            #region 判断右节点是否包含参数

            if (ExpressionCalculater.IsExistsParameters(node.Right))
            {
                ClickHouseConditionNodeResult rightNodeConditonResult = ExecuteResolver(this.DBMapperProvider, node.Right, this._isFieldWithTableName);
                if (!rightNodeConditonResult.IsAvailable())
                {
                    this.Result.CopyStatus(rightNodeConditonResult);
                    this.ExpressionChains.Pop();
                    return node.Reduce();
                }

                rightValueText = rightNodeConditonResult.TextValue;
                //rightParamsList = new List<ClickHouseParameterDesc>(rightNodeConditonResult.Parameters);
            }
            else
            {
                // 同类直接拼接
                object rightObjectValue = ExpressionCalculater.GetValue(node.Right);
                rightValueText = ClickHouseGrammarRule.GetSqlText(rightObjectValue);

                // clickhouse 不支持参数化
                //string paramName = ClickHouseGrammarRule.GetUniqueIdentifier();
                //rightValueText = ClickHouseGrammarRule.GenerateParamName(paramName);
                //rightParamsList = new List<ClickHouseParameterDesc>();
                //rightParamsList.Add(new ClickHouseParameterDesc(paramName, rightObjectValue));
            }

            #endregion

            #region 构建计算

            string textFormat = "({0}{1}{2})";
            if (node.NodeType == ExpressionType.Add)
                this.Result.AppendText(string.Format(textFormat, leftValueText, "+", rightValueText));
            else if (node.NodeType == ExpressionType.Subtract)
                this.Result.AppendText(string.Format(textFormat, leftValueText, "-", rightValueText));
            else if (node.NodeType == ExpressionType.Multiply)
                this.Result.AppendText(string.Format(textFormat, leftValueText, "*", rightValueText));
            else if (node.NodeType == ExpressionType.Divide)
                this.Result.AppendText(string.Format(textFormat, leftValueText, "/", rightValueText));
            else if (node.NodeType == ExpressionType.Modulo)
                this.Result.AppendText(string.Format(textFormat, leftValueText, "%", rightValueText));
            else if (node.NodeType == ExpressionType.And)
                this.Result.AppendText(string.Format(textFormat, leftValueText, "&", rightValueText));
            else if (node.NodeType == ExpressionType.Or)
                this.Result.AppendText(string.Format(textFormat, leftValueText, "|", rightValueText));
            else
                this.Result.AppendError("暂不支持" + node.NodeType.ToString() + "方法的解析");

            #endregion

            #region 组合参数

            //this.Result.InsertParameterRange(0, leftParamsList);
            //this.Result.InsertParameterRange(0, rightParamsList);

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
                        tablePreName = this.DBMapperProvider.GetDbTableName((node.Expression as ParameterExpression).Type);

                    this.Result.AppendText(ClickHouseGrammarRule.GenerateFieldWrapped(columnAttr.DbColumnName, tablePreName));
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
        public static ClickHouseConditionNodeResult ExecuteResolver(IDbMappingHandler dbMappingHandler, Expression expression, bool isFieldWithTableName)
        {
            //构造解析者执行解析
            ClickHouseConditionNodeHandler entity = new ClickHouseConditionNodeHandler(dbMappingHandler, isFieldWithTableName);
            entity.Visit(expression);

            return entity.Result;
        }

        #endregion
    }
}
