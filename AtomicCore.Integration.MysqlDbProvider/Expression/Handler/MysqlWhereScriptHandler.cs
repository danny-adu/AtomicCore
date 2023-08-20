﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using AtomicCore.DbProvider;

namespace AtomicCore.Integration.MysqlDbProvider
{
    /// <summary>
    /// MySql的where条件脚本解析对象(中间协调调度类)
    /// (重构组合調用MssqlConditionCombinedHandler与MssqlConditionNodeHandler)
    /// </summary>
    internal static class MysqlWhereScriptHandler
    {
        /// <summary>
        /// 执行解析（如果传入null,则返回默认的实例）
        /// </summary>
        /// <param name="exp">表达式</param>
        /// <param name="dbMappingHandler">数据库字段映射接口</param>
        /// <param name="isFieldWithTableName">是否在字段前面加前缀,如果需要请设置为true</param>
        /// <returns></returns>
        public static MysqlWhereScriptResult ExecuteResolver(Expression exp, IDbMappingHandler dbMappingHandler, bool isFieldWithTableName)
        {
            MysqlWhereScriptResult result = MysqlWhereScriptResult.Create();

            if (null == exp)
                return result;

            if (ExpressionType.Lambda != exp.NodeType)
            {
                result.AppendError("请传入Where Lambda表达式!");
                return result;
            }


            //初始化
            LambdaExpression lambdaExp = exp as LambdaExpression;
            if (lambdaExp.Parameters.Count == 1 && typeof(IDbModel).IsAssignableFrom(lambdaExp.Parameters[0].Type) && lambdaExp.ReturnType == typeof(bool))
            {
                //执行解析
                MysqlConditionCombinedResult resolver = MysqlConditionCombinedHandler.ExecuteResolver(exp, dbMappingHandler, isFieldWithTableName);
                if (!resolver.IsAvailable())
                {
                    result.CopyStatus(resolver);
                }
                string architectureTemp = resolver.ArchitectureTemp;
                int architectureParamNumber = resolver.ArchitectureParams.Count();
                if (architectureParamNumber > 0)
                {
                    List<string> parseList = new List<string>();
                    IEnumerator<KeyValuePair<string, Expression>> paramEnumerator = resolver.ArchitectureParams.GetEnumerator();
                    while (paramEnumerator.MoveNext())
                    {
                        Expression itemExp = paramEnumerator.Current.Value;

                        //判断该表达式是否包含参数，采用不同的解析方式
                        Type modelType = null;
                        if (ExpressionCalculater.IsExistsParameters(itemExp, out modelType))
                        {
                            #region 如果包含参数，采用包含参数的解析方式进行解析

                            MysqlConditionNodeResult cur_node = MysqlConditionNodeHandler.ExecuteResolver(dbMappingHandler, itemExp, isFieldWithTableName);
                            if (!cur_node.IsAvailable())
                            {
                                result.CopyStatus(cur_node);
                            }
                            parseList.Add(cur_node.TextValue);
                            result.Parameters.InsertRange(0, cur_node.Parameters);

                            #endregion
                        }
                        else
                        {
                            #region 如果不包含参数，则直接进行值计算

                            object objValue = ExpressionCalculater.GetValue(itemExp);

                            string paramName = MysqlGrammarRule.GetUniqueIdentifier();
                            string valueText = MysqlGrammarRule.GenerateParamName(paramName);
                            parseList.Add(valueText);
                            result.Parameters.Add(new MysqlParameterDesc(paramName, objValue));

                            #endregion
                        }
                    }
                    architectureTemp = string.Format(architectureTemp, parseList.ToArray());
                }

                //将解析好的值对应结果
                result.AppendTextScript(architectureTemp);
            }
            else
            {
                result.AppendError("Lambda中参数必须为IModel接口实体，返回值必须是bool值");
            }

            return result;
        }
    }
}
