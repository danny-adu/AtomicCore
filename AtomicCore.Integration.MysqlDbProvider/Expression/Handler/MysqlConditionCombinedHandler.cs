/*
 * 该类的主要职责是将where表达式解析按标准的SQL语句的where条件进行分割，分割的界定暂定为 and 与 or
 * 解析后的格式为: ({0}={1} and {2}>={3}) or ({4}<{5}) ,还有一个占位符的表达式集合
 * 
 * 条件拓展查询方法在这里进行拓展
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using AtomicCore.DbProvider;

namespace AtomicCore.Integration.MysqlDbProvider
{
    /// <summary>
    /// MySql中Where表达式语句架构解析, 文本模版 与 占位符与表达式集合
    /// </summary>
    internal class MysqlConditionCombinedHandler : ExpressionVisitorBase
    {
        #region Constructors

        /// <summary>
        /// 转移符
        /// </summary>
        private const string c_escape = "/";

        /// <summary>
        /// 全局占位符计数器
        /// </summary>
        private int _textIndex = 0;
        /// <summary>
        /// 数据字段映射接口
        /// </summary>
        private IDbMappingHandler _dbMappingHandler = null;

        /// <summary>
        /// 字段前缀(DbSubQuery中使用到了)
        /// </summary>
        private bool _isFieldWithTableName = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbMappingHandler">数据字段映射接口</param>
        /// <param name="isFieldWithTableName">字段前面加字段前缀</param>
        private MysqlConditionCombinedHandler(IDbMappingHandler dbMappingHandler, bool isFieldWithTableName)
        {
            this._dbMappingHandler = dbMappingHandler;
            this._isFieldWithTableName = isFieldWithTableName;

            this._result = MysqlConditionCombinedResult.Create();
        }

        #endregion

        #region Propertys

        private MysqlConditionCombinedResult _result = null;

        /// <summary>
        /// 框架解析的结果
        /// </summary>
        public MysqlConditionCombinedResult Result
        {
            get { return this._result; }
        }

        #endregion

        #region ExpressionVisitor

        /// <summary>
        /// 访问二元表达式(优先处理andAlso与orElse运算,如果遇到这2类运算则采取截断递归式解析)
        /// </summary>
        /// <param name="node"></param>
        /// <param name="isStackPush"></param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression node, bool isStackPush = true)
        {
            this.ExpressionChains.Push(node);

            Expression expression = null;
            if (node.NodeType == ExpressionType.AndAlso)
            {
                #region AndAlso解析

                //右节点是否还是需要解析二元表达式 and or
                bool isRightAndOrBinaryExp = this.IsBinaryAndORExpression(node.Right);

                //拼接And Sql语句
                if (isRightAndOrBinaryExp)
                {
                    this._result.AppendArchitectureTemp("(");
                    this.Visit(node.Left);
                    this._result.AppendArchitectureTemp(")");
                }
                else
                {
                    this.Visit(node.Left);
                }
                this._result.AppendArchitectureTemp(" and ");
                if (isRightAndOrBinaryExp)
                {
                    this._result.AppendArchitectureTemp("(");
                    this.Visit(node.Right);
                    this._result.AppendArchitectureTemp(")");
                }
                else
                {
                    this.Visit(node.Right);
                }
                expression = node.Reduce();

                #endregion
            }
            else if (node.NodeType == ExpressionType.OrElse)
            {
                #region OrElse 解析

                //左节点是否还是需要解析二元表达式 and or
                bool isLeftAndOrBinaryExp = this.IsBinaryAndORExpression(node.Left);
                //右节点是否还是需要解析二元表达式 and or
                bool isRightAndOrBinaryExp = this.IsBinaryAndORExpression(node.Right);

                if (isLeftAndOrBinaryExp || isRightAndOrBinaryExp)
                {
                    this._result.AppendArchitectureTemp("(");
                    this.Visit(node.Left);
                    this._result.AppendArchitectureTemp(")");
                }
                else
                {
                    this.Visit(node.Left);
                }
                this._result.AppendArchitectureTemp(" or ");
                if (isLeftAndOrBinaryExp || isRightAndOrBinaryExp)
                {
                    this._result.AppendArchitectureTemp("(");
                    this.Visit(node.Right);
                    this._result.AppendArchitectureTemp(")");
                }
                else
                {
                    this.Visit(node.Right);
                }
                expression = node.Reduce();

                #endregion
            }
            else
            {
                #region 条件表达式归纳(这里的expression必须调用base.VisitBinary,否则无法正常解析而进入死循环)

                if (node.NodeType == ExpressionType.Equal)
                {
                    this.AppendCondition(node.Left, node.Right, "=");
                    expression = base.VisitBinary(node, false);
                }
                else if (node.NodeType == ExpressionType.GreaterThan)
                {
                    this.AppendCondition(node.Left, node.Right, ">");
                    expression = base.VisitBinary(node, false);
                }
                else if (node.NodeType == ExpressionType.GreaterThanOrEqual)
                {
                    this.AppendCondition(node.Left, node.Right, ">=");
                    expression = base.VisitBinary(node, false);
                }
                else if (node.NodeType == ExpressionType.LessThan)
                {
                    this.AppendCondition(node.Left, node.Right, "<");
                    expression = base.VisitBinary(node, false);
                }
                else if (node.NodeType == ExpressionType.LessThanOrEqual)
                {
                    this.AppendCondition(node.Left, node.Right, "<=");
                    expression = base.VisitBinary(node, false);
                }
                else if (node.NodeType == ExpressionType.NotEqual)
                {
                    this.AppendCondition(node.Left, node.Right, "!=");
                    expression = base.VisitBinary(node, false);
                }
                else
                {
                    expression = base.VisitBinary(node, false);
                }

                #endregion
            }

            this.ExpressionChains.Pop();
            return expression;
        }

        /// <summary>
        /// 成员访问
        /// </summary>
        /// <param name="node"></param>
        /// <param name="isStackPush"></param>
        /// <returns></returns>
        protected override Expression VisitMemberAccess(MemberExpression node, bool isStackPush = true)
        {
            if (node.Expression is ParameterExpression)
            {
                //获取上一次表达式树节点是否是andalso 或  orelse
                Expression exp = this.ExpressionChains.Peek();

                //Query查询（eg : d => d.id > 0 && !d.isdel && d.isdel）
                if ((exp.NodeType == ExpressionType.AndAlso || exp.NodeType == ExpressionType.OrElse) && node.Member.MemberType == MemberTypes.Property && (node.Member as PropertyInfo).PropertyType == typeof(bool))
                {
                    this.ExpressionChains.Push(node);

                    BinaryExpression binaryExp = Expression.MakeBinary(ExpressionType.Equal, node, Expression.Constant(true));
                    this.Visit(binaryExp);

                    this.ExpressionChains.Pop();
                    return node.Reduce();//这里直接return即可,因为下面再继续解析也只是解析到表达式的参数级,直接返回更加快速有效
                }
                //Query查询（eg : d => d.isdel）
                else if (exp.NodeType == ExpressionType.Lambda && node.Member.MemberType == MemberTypes.Property && (node.Member as PropertyInfo).PropertyType == typeof(bool))
                {
                    this.ExpressionChains.Push(node);

                    BinaryExpression binaryExp = Expression.MakeBinary(ExpressionType.Equal, node, Expression.Constant(true));
                    this.Visit(binaryExp);

                    this.ExpressionChains.Pop();
                    return node.Reduce();//这里直接return即可,因为下面再继续解析也只是解析到表达式的参数级,直接返回更加快速有效
                }
            }

            return base.VisitMemberAccess(node);
        }

        /// <summary>
        /// 访问一元表达式
        /// </summary>
        /// <param name="node"></param>
        /// <param name="isStackPush"></param>
        /// <returns></returns>
        protected override Expression VisitUnary(UnaryExpression node, bool isStackPush = true)
        {
            if (node.NodeType == ExpressionType.Not)
            {
                if (!(node.Operand is MemberExpression) || !typeof(IDbModel).IsAssignableFrom((node.Operand as MemberExpression).Member.DeclaringType))
                    return base.VisitUnary(node);

                //开始规则解析
                this.ExpressionChains.Push(node);

                BinaryExpression binaryExp = Expression.MakeBinary(ExpressionType.Equal, node.Operand, Expression.Constant(false));
                this.Visit(binaryExp);

                this.ExpressionChains.Pop();
                return node.Reduce();
            }

            return base.VisitUnary(node);
        }

        /// <summary>
        /// 访问方法表达式
        /// </summary>
        /// <param name="methodCallExp"></param>
        /// <param name="isStackPush"></param>
        /// <returns></returns>
        protected override Expression VisitMethodCall(MethodCallExpression methodCallExp, bool isStackPush = true)
        {
            Expression expression = null;

            //Sql查询方法原型拓展Mssql2008QueryExpands
            if (null == methodCallExp.Object)
            {
                //在Mssql下非查询拓展的直接返回
                if (methodCallExp.Method.DeclaringType != typeof(DbModelExtensions))
                    return base.VisitMethodCall(methodCallExp);
                //如果第一个参数不是参数类型,必然不是SQL查询拓展
                if (methodCallExp.Arguments.First().NodeType != ExpressionType.Parameter)
                    return base.VisitMethodCall(methodCallExp);

                #region Sql拓展查询方法(这里有表达式链进出)

                this.ExpressionChains.Push(methodCallExp);

                //判断类型是否相符
                ParameterExpression objExp = methodCallExp.Arguments.First() as ParameterExpression;
                if (!typeof(IDbModel).IsAssignableFrom(objExp.Type))
                    return base.VisitMethodCall(methodCallExp, false);

                switch (methodCallExp.Method.Name)
                {
                    case "SqlIn":
                        #region DbIn

                        //获取指定的字段
                        MemberExpression sqlin_memberExp = null;
                        if (methodCallExp.Arguments[1].NodeType == ExpressionType.MemberAccess)
                        {
                            sqlin_memberExp = methodCallExp.Arguments[1] as MemberExpression;
                        }
                        else if (methodCallExp.Arguments[1].NodeType == ExpressionType.Convert)
                        {
                            UnaryExpression sqlin_assignExp = methodCallExp.Arguments[1] as UnaryExpression;
                            if (sqlin_assignExp == null)
                            {
                                this._result.AppendError("方法的第一个参数必须为指定的具体实例的参数类型");
                                return base.VisitMethodCall(methodCallExp, false);
                            }
                            sqlin_memberExp = sqlin_assignExp.Operand as MemberExpression;
                        }
                        else
                        {
                            this._result.AppendError("尚未实现" + methodCallExp.Arguments[1].NodeType.ToString() + "的解析");
                            return base.VisitMethodCall(methodCallExp, false);
                        }
                        if (sqlin_memberExp == null)
                        {
                            this._result.AppendError("方法的第一个参数必须是 d.Property 的参数类型");
                            return base.VisitMethodCall(methodCallExp, false);
                        }
                        string sqlin_placeHolder_assignName = "{" + (this._textIndex++) + "}";
                        this._result.AppendArchitectureParameter(sqlin_placeHolder_assignName, sqlin_memberExp);


                        //获取需要in的字符串
                        string sqlIn_Value = ExpressionCalculater.GetValue(methodCallExp.Arguments.Last()) as string;
                        if (string.IsNullOrEmpty(sqlIn_Value))
                            return base.VisitMethodCall(methodCallExp, false);

                        string[] sqlIn_arrs = sqlIn_Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (sqlIn_arrs.Length <= 0)
                            return base.VisitMethodCall(methodCallExp, false);


                        if (sqlin_memberExp.Type == typeof(String) || sqlin_memberExp.Type == typeof(Guid))
                        {
                            string joinCase = string.Join("','", sqlIn_arrs);
                            this._result.AppendArchitectureTemp(sqlin_placeHolder_assignName + " in('" + joinCase + "')");
                        }
                        else
                        {
                            this._result.AppendArchitectureTemp(sqlin_placeHolder_assignName + " in(" + sqlIn_Value.ToString() + ")");
                        }

                        #endregion
                        break;
                    case "SqlNotIn":
                        #region DbNotIn

                        //获取指定的字段
                        MemberExpression sqlnotin_memberExp = null;
                        if (methodCallExp.Arguments[1].NodeType == ExpressionType.MemberAccess)
                        {
                            sqlnotin_memberExp = methodCallExp.Arguments[1] as MemberExpression;
                        }
                        else if (methodCallExp.Arguments[1].NodeType == ExpressionType.Convert)
                        {
                            UnaryExpression sqlin_assignExp = methodCallExp.Arguments[1] as UnaryExpression;
                            if (sqlin_assignExp == null)
                            {
                                this._result.AppendError("方法的第一个参数必须为指定的具体实例的参数类型");
                                return base.VisitMethodCall(methodCallExp, false);
                            }
                            sqlnotin_memberExp = sqlin_assignExp.Operand as MemberExpression;
                        }
                        else
                        {
                            this._result.AppendError("尚未实现" + methodCallExp.Arguments[1].NodeType.ToString() + "的解析");
                            return base.VisitMethodCall(methodCallExp, false);
                        }
                        if (sqlnotin_memberExp == null)
                        {
                            this._result.AppendError("方法的第一个参数必须是 d.Property 的参数类型");
                            return base.VisitMethodCall(methodCallExp, false);
                        }
                        string sqlnotin_placeHolder_assignName = "{" + (this._textIndex++) + "}";
                        this._result.AppendArchitectureParameter(sqlnotin_placeHolder_assignName, sqlnotin_memberExp);


                        //获取需要in的字符串
                        string sqlnotIn_Value = ExpressionCalculater.GetValue(methodCallExp.Arguments.Last()) as string;
                        if (string.IsNullOrEmpty(sqlnotIn_Value))
                            return base.VisitMethodCall(methodCallExp, false);

                        string[] sqlnotIn_arrs = sqlnotIn_Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (sqlnotIn_arrs.Length <= 0)
                            return base.VisitMethodCall(methodCallExp, false);


                        if (sqlnotin_memberExp.Type == typeof(String) || sqlnotin_memberExp.Type == typeof(Guid))
                        {
                            string joinCase = string.Join("','", sqlnotIn_arrs);
                            this._result.AppendArchitectureTemp(sqlnotin_placeHolder_assignName + " not in('" + joinCase + "')");
                        }
                        else
                        {
                            this._result.AppendArchitectureTemp(sqlnotin_placeHolder_assignName + " not in(" + sqlnotIn_Value.ToString() + ")");
                        }

                        #endregion
                        break;
                    case "SqlSubQuery":
                        #region DbSubQuery

                        StringBuilder joinBuilder = new StringBuilder();
                        StringBuilder childWhereBuilder = new StringBuilder();

                        #region 1.解析级联条件

                        MysqlConditionCombinedResult joinResult = MysqlConditionCombinedHandler.ExecuteResolver(methodCallExp.Arguments[1], this._dbMappingHandler, true);
                        if (joinResult.IsAvailable())
                        {
                            List<object> joinParams = new List<object>();
                            foreach (var itemParam in joinResult.ArchitectureParams.OrderBy(d => d.Key))
                            {
                                Expression itemExp = itemParam.Value;

                                //判断该表达式是否包含参数，采用不同的解析方式
                                Type modelType = null;
                                if (ExpressionCalculater.IsExistsParameters(itemExp, out modelType))
                                {
                                    #region 如果包含参数，采用包含参数的解析方式进行解析

                                    MysqlConditionNodeResult cur_node = MysqlConditionNodeHandler.ExecuteResolver(this._dbMappingHandler, itemExp, true);
                                    if (!cur_node.IsAvailable())
                                    {
                                        this._result.AppendError(string.Join(" ", cur_node.Errors));
                                        return base.VisitMethodCall(methodCallExp, false);
                                    }
                                    joinParams.Add(cur_node.TextValue);

                                    #endregion
                                }
                                else
                                {
                                    #region 如果不包含参数，则直接进行值计算

                                    object objValue = ExpressionCalculater.GetValue(itemExp);

                                    string paramName = MysqlGrammarRule.GetUniqueIdentifier();
                                    string valueText = MysqlGrammarRule.GenerateParamName(paramName);
                                    joinParams.Add(string.Format("{0}.{1}", string.Format("[{0}]", this._dbMappingHandler.GetDbTableName(modelType)), valueText));

                                    #endregion
                                }
                            }
                            joinBuilder.Append(string.Format(joinResult.ArchitectureTemp, joinParams.ToArray()));
                        }
                        else
                        {
                            this._result.AppendError(string.Join(" ", joinResult.Errors));
                            return base.VisitMethodCall(methodCallExp, false);
                        }

                        #endregion

                        #region 2.解析子查询的条件

                        Expression childExps = methodCallExp.Arguments[2];
                        if (childExps is UnaryExpression)
                        {
                            childExps = (childExps as UnaryExpression).Operand;
                        }
                        if (ExpressionType.Lambda != childExps.NodeType)
                        {
                            this._result.AppendError(string.Format("条件表达式为{0}类型,无法解析", childExps.NodeType));
                            return base.VisitMethodCall(methodCallExp, false);
                        }

                        //是否需要改造该方法 用于追加表明 否则解析d会错误
                        MysqlWhereScriptResult whereCondtionResult = MysqlWhereScriptHandler.ExecuteResolver(childExps, this._dbMappingHandler, true);
                        if (whereCondtionResult.IsAvailable())
                        {
                            childWhereBuilder.Append(whereCondtionResult.TextScript);
                            if (whereCondtionResult.Parameters.Count > 0)
                            {
                                foreach (var item in whereCondtionResult.Parameters)
                                {
                                    childWhereBuilder.Replace(MysqlGrammarRule.GenerateParamName(item.Name), MysqlGrammarRule.GetSqlText(item.Value));
                                }
                            }
                        }
                        else
                        {
                            this._result.AppendError(string.Join(" ", joinResult.Errors));
                            return base.VisitMethodCall(methodCallExp, false);
                        }

                        #endregion

                        #region 第三个参数 是包含查询还是否

                        bool subMode = Convert.ToBoolean(ExpressionCalculater.GetValue(methodCallExp.Arguments.Last()));

                        #endregion

                        Type[] childTableTypes = methodCallExp.Method.GetGenericArguments();

                        this.Result.AppendArchitectureTemp(string.Format(" {0} EXISTS(SELECT * FROM {1} where {2} and {3})"
                           , true.Equals(subMode) ? string.Empty : "Not"
                           , MysqlGrammarRule.GenerateTableWrapped(this._dbMappingHandler.GetDbTableName(childTableTypes.First()))
                           , joinBuilder.ToString()
                           , childWhereBuilder.ToString()));

                        #endregion
                        break;
                    default:
                        break;
                }

                expression = methodCallExp.Reduce();
                this.ExpressionChains.Pop();
                return expression;

                #endregion
            }
            else
            {
                //当前实例可能是参数 也可能是属性实例
                if (methodCallExp.Object.NodeType == ExpressionType.MemberAccess)
                {
                    #region 属性实例原生方法(这里有表达式链进出)，例如Contains

                    MemberExpression objExp = methodCallExp.Object as MemberExpression;

                    //判断它的宿主是memberExpression并且成员为属性
                    if (objExp != null && objExp.Member.MemberType == System.Reflection.MemberTypes.Property)
                    {
                        this.ExpressionChains.Push(methodCallExp);

                        //实例方法拓展
                        switch (methodCallExp.Method.Name)
                        {
                            case "Contains":
                                #region Contains

                                //申请参数化索引位
                                string instacne_contains_assignName = "{" + (this._textIndex++) + "}";
                                string instance_contains_parameterName = "{" + (this._textIndex++) + "}";

                                //获取like匹配值,并且判断是否需要转译
                                object sqlLike_contains_expVal = null;//最终Like查询的匹配值
                                bool sqlLike_contains_isReplace = this.RepleceSpecialString(methodCallExp.Arguments.First(), out sqlLike_contains_expVal);//是否转译过?

                                if (sqlLike_contains_isReplace)
                                {
                                    this._result.AppendArchitectureTemp(string.Format("{0} like CONCAT('%',{1},'%') ESCAPE '{2}' ",
                                        instacne_contains_assignName,
                                        instance_contains_parameterName,
                                        c_escape));
                                }
                                else
                                {
                                    this._result.AppendArchitectureTemp(string.Format("{0} like CONCAT('%',{1},'%') ",
                                        instacne_contains_assignName,
                                        instance_contains_parameterName));
                                }

                                //带入参数值
                                this._result.AppendArchitectureParameter(instacne_contains_assignName, methodCallExp.Object);
                                this._result.AppendArchitectureParameter(instance_contains_parameterName, Expression.Constant(sqlLike_contains_expVal));

                                expression = methodCallExp.Reduce();
                                break;

                            #endregion
                            default:
                                expression = base.VisitMethodCall(methodCallExp, false);
                                break;
                        }

                        expression = methodCallExp.Reduce();
                        this.ExpressionChains.Pop();
                        return expression;
                    }
                    else
                    {
                        return base.VisitMethodCall(methodCallExp);
                    }

                    #endregion
                }
                else
                {
                    //(这里调用父方法)
                    return base.VisitMethodCall(methodCallExp);
                }
            }
        }

        #endregion

        #region Static Mehtods

        /// <summary>
        /// 解析表达式,生成结果集对象
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="dbMappingHandler">数据字段映射接口</param>
        /// <param name="isFieldWithTableName">是否在字段前面加前缀,如果需要请设置为true</param>
        /// <returns></returns>
        public static MysqlConditionCombinedResult ExecuteResolver(Expression expression, IDbMappingHandler dbMappingHandler, bool isFieldWithTableName)
        {
            MysqlConditionCombinedHandler entity = new MysqlConditionCombinedHandler(dbMappingHandler, isFieldWithTableName);
            entity.Visit(expression);
            return entity.Result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 追加新增条件
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="operators"></param>
        private void AppendCondition(Expression left, Expression right, string operators)
        {
            string placeHolder_Left = "{" + (this._textIndex++) + "}";
            string placeHolder_Right = "{" + (this._textIndex++) + "}";

            //拼接条件Sql语句
            this._result.AppendArchitectureTemp(placeHolder_Left + operators + placeHolder_Right);
            this._result.AppendArchitectureParameter(placeHolder_Left, left);
            this._result.AppendArchitectureParameter(placeHolder_Right, right);
        }

        /// <summary>
        /// 判断是否是二元表达式的AndAlso或OrElse运算表达式
        /// </summary>
        /// <param name="exp">当前表达式</param>
        /// <returns></returns>
        private bool IsBinaryAndORExpression(Expression exp)
        {
            if (exp is BinaryExpression)
            {
                BinaryExpression curExp = exp as BinaryExpression;
                return curExp.NodeType == ExpressionType.AndAlso || curExp.NodeType == ExpressionType.OrElse;
            }
            return false;
        }

        /// <summary>
        /// 指定需要被转译的字符集
        /// </summary>
        /// <remarks>
        /// NATIVE/ASCII编码互转,eg:http://tool.chinaz.com/tools/native_ascii.aspx
        /// </remarks>
        private const string c_replaceChar = @"[\u005f\u0025\u005b\u005e\u002f]";
        /// <summary>
        /// 指定需要被转译的字符集 和 转译符/
        /// </summary>
        /// <remarks>
        /// NATIVE/ASCII编码互转,eg:http://tool.chinaz.com/tools/native_ascii.aspx
        /// </remarks>
        private const string c_replaceCharFull = @"[\u005f\u0025\u005b\u005e\u002f\u007e]";

        /// <summary>
        /// 将Sql Like 查询中的匹配条件值进行转换
        /// </summary>
        /// <param name="argmentExp"></param>
        /// <param name="replaceObject"></param>
        /// <returns></returns>
        private bool RepleceSpecialString(Expression argmentExp, out object replaceObject)
        {
            if (null == argmentExp)
                throw new ArgumentException("argmentExp");

            object original = ExpressionCalculater.GetValue(argmentExp);
            if (null == original)
                throw new ArgumentException("original");

            //如果不存在这些需要被转译的符号,则直接返回原值
            bool flag = false;
            if (Regex.IsMatch(original.ToString(), c_replaceChar, RegexOptions.IgnoreCase))
            {
                replaceObject = Regex.Replace(original.ToString(), c_replaceCharFull, this.ReplaceSpecialStringFunc, RegexOptions.IgnoreCase);
                flag = true;
            }
            else
            {
                replaceObject = original;
            }
            return flag;
        }

        /// <summary>
        /// 匹配Like查询中的特殊字符串,前面加转义符
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        private string ReplaceSpecialStringFunc(Match match)
        {
            return string.Format("{0}{1}", c_escape, match.Value);
        }

        #endregion
    }
}
