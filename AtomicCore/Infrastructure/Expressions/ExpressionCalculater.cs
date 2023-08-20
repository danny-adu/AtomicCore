using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AtomicCore
{
    /// <summary>
    /// 表达式计算帮助类
    /// </summary>
    public static class ExpressionCalculater
    {
        /// <summary>
        /// string type
        /// </summary>
        private static readonly Type STRINGTYPE = typeof(string);

        /// <summary>
        /// 判断表达式中是否包含的有参数
        /// </summary>
        /// <param name="exp">需要被判断的表达式</param>
        /// <returns></returns>
        public static bool IsExistsParameters(Expression exp)
        {
            ExpressionParameterVisitor entity = new ExpressionParameterVisitor(exp);
            return entity.ParameterTypes.Count() > 0;
        }

        /// <summary>
        /// 判断表达式中是否包含的有参数
        /// </summary>
        /// <param name="exp">需要被判断的表达式</param>
        /// <param name="paramType">参数类型</param>
        /// <returns></returns>
        public static bool IsExistsParameters(Expression exp, out Type paramType)
        {
            ExpressionParameterVisitor entity = new ExpressionParameterVisitor(exp);
            if (entity.ParameterTypes.Count() > 0)
            {
                paramType = entity.ParameterTypes.First();
                return true;
            }
            else
            {
                paramType = null;
                return false;
            }
        }

        /// <summary>
        /// 计算表达式的值
        /// </summary>
        /// <param name="expression">需要被计算的表达式</param>
        /// <param name="args">参与表达式计算的参数列表</param>
        /// <returns></returns>
        public static object GetValue(Expression expression, params object[] args)
        {
            // 常量表达式(快速处理方案)
            if (expression is ConstantExpression constantExp)
            {
                if (STRINGTYPE.IsAssignableFrom(constantExp.Type) && null == constantExp.Value)
                    return string.Empty;

                return constantExp.Value;
            }

            // 表达式万能处理流程（性能可能稍差一点）
            if (!(expression is LambdaExpression lambdaExp))
            {
                List<ParameterExpression> parameters = null;
                if (null != args && args.Length > 0)
                {
                    parameters = new List<ParameterExpression>();
                    foreach (var arg in args)
                        parameters.Add(Expression.Parameter(arg.GetType()));
                }

                lambdaExp = Expression.Lambda(expression, parameters);
            }

            // 判断表达式参数
            object[] invoken_args = null;
            if (null != lambdaExp.Parameters && lambdaExp.Parameters.Count > 0)
                if (null == args || args.Length < lambdaExp.Parameters.Count)
                {
                    int p_index = 0;
                    int max_index = args.Length - 1;
                    var po_list = new List<object>();
                    foreach (var p in lambdaExp.Parameters)
                    {
                        if (p_index <= max_index)
                            po_list.Add(args[p_index]);
                        else
                            po_list.Add(GetDefaultValue(p.Type));

                        p_index++;
                    }

                    invoken_args = po_list.ToArray();
                }
                else
                    invoken_args = args;

            // 生成lambda表达式
            var currentDelegate = lambdaExp.Compile();
            object retVal = currentDelegate.DynamicInvoke(invoken_args);
            var retType = currentDelegate.Method.ReturnParameter.ParameterType;

            // 强制将null字符串转化为string.Empty
            if (STRINGTYPE.IsAssignableFrom(retType) && null == retVal)
                retVal = string.Empty;

            return retVal;
        }

        /// <summary>
        /// 返回默认值
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static object GetDefaultValue(Type targetType)
        {
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }
    }
}
