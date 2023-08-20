using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using AtomicCore.DbProvider;

namespace AtomicCore.Integration.MysqlDbProvider
{
    /// <summary>
    /// Mssql语句脚本解析
    /// </summary>
    internal class MysqlSentenceHandler : ExpressionVisitorBase
    {
        #region Variables

        private int _count_asNameIndex = 0;
        private int _sum_asNameIndex = 0;
        private int _max_asNameIndex = 0;
        private int _min_asNameIndex = 0;

        private IDbMappingHandler _dbMappingHandler = null;

        #endregion

        #region Constroutors

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbMappingHandler">数据映射接口实例</param>
        private MysqlSentenceHandler(IDbMappingHandler dbMappingHandler)
            : base()
        {
            this._dbMappingHandler = dbMappingHandler;
            this._result = MysqlSentenceResult.Create();
        }

        #endregion

        #region Propertys

        private MysqlSentenceResult _result = null;

        /// <summary>
        /// 最终解析的结果
        /// </summary>
        public MysqlSentenceResult Result
        {
            get { return this._result; }
        }

        #endregion

        #region ExpressionVisitor

        /// <summary>
        /// 解析方法解析表达式
        /// </summary>
        /// <param name="methodCallExp">方法表达式</param>
        /// <param name="isStackPush"></param>
        /// <returns></returns>
        protected override Expression VisitMethodCall(MethodCallExpression methodCallExp, bool isStackPush = true)
        {
            switch (methodCallExp.Method.Name)
            {
                case "Select":
                    #region Select

                    //执行该语句开始参数递归解析
                    IEnumerable<Expression> select_args = base.VisitExpressionList(methodCallExp.Arguments);
                    Expression select_exp = select_args.Last();
                    if (select_exp != null)
                    {
                        Expression select_func_lambdaExp = null;
                        if (select_exp is UnaryExpression)
                        {
                            //该方式负责直接在Select()中去写表达式的特例
                            UnaryExpression select_unary_temp = select_exp as UnaryExpression;
                            select_func_lambdaExp = select_unary_temp.Operand as LambdaExpression;
                        }
                        else
                        {
                            this._result.AppendError("尚未实现直接解析" + select_exp.NodeType.ToString() + "的特例");
                        }

                        //判断之前是否解析成功
                        if (this._result.IsAvailable())
                        {
                            MysqlSelectCombinedResult selectFieldResult = MysqlSelectCombinedHandler.ExecuteResolver(select_func_lambdaExp, this._dbMappingHandler);
                            //接收需要被查询的字段
                            this._result.SetSelectField(selectFieldResult.FieldMembers);
                        }
                    }
                    break;

                    #endregion
                case "Pager":
                    #region Pager

                    IEnumerable<Expression> pager_args = base.VisitExpressionList(methodCallExp.Arguments);//执行该语句开始参数递归解析
                    int currentPage = (int)ExpressionCalculater.GetValue(pager_args.First());
                    int pageSize = (int)ExpressionCalculater.GetValue(pager_args.Last());
                    if (currentPage <= 0)
                    {
                        currentPage = 1;
                    }
                    if (pageSize <= 0)
                    {
                        pageSize = 20;
                    }

                    //初始化解析容器
                    this._result.SetPageCondition(currentPage, pageSize);
                    break;

                    #endregion
                case "Where":
                    #region Where

                    IEnumerable<Expression> where_args = base.VisitExpressionList(methodCallExp.Arguments);//执行该语句开始参数递归解析
                    Expression where_exp = where_args.Last();
                    if (where_exp != null)
                    {
                        Expression where_func_lambdaExp = null;
                        if (where_exp is UnaryExpression)
                        {
                            //该方式负责直接在where()中去写表达式的特例
                            UnaryExpression where_unary_temp = where_exp as UnaryExpression;
                            where_func_lambdaExp = where_unary_temp.Operand as LambdaExpression;
                        }
                        else if (where_exp is MemberExpression)
                        {
                            //该方式负责传入where()中的变量的表达式树的解析的特例
                            object lambdaObject = ExpressionCalculater.GetValue(where_exp);
                            where_func_lambdaExp = lambdaObject as Expression;
                        }
                        else
                        {
                            this._result.AppendError("尚未实现直接解析" + where_exp.NodeType.ToString() + "的特例");
                        }

                        //判断之前是否解析成功
                        if (this._result.IsAvailable())
                        {
                            //获取Where条件解析后的条件模版与参数
                            MysqlWhereScriptResult result = MysqlWhereScriptHandler.ExecuteResolver(where_func_lambdaExp, this._dbMappingHandler, false);
                            if (result.IsAvailable())
                            {
                                this._result.SetWhereCondition(result.TextScript, result.Parameters);
                            }
                            else
                            {
                                this._result.CopyStatus(result);
                            }
                        }
                    }
                    break;

                    #endregion
                case "OrderBy":
                    #region OrderBy

                    IEnumerable<Expression> orderBy_args = base.VisitExpressionList(methodCallExp.Arguments);//执行该语句开始参数递归解析
                    Expression orderBy_exp = orderBy_args.Last();
                    if (orderBy_exp != null)
                    {
                        UnaryExpression orderBy_unary_temp = orderBy_exp as UnaryExpression;
                        LambdaExpression orderBy_lambda_temp = orderBy_unary_temp.Operand as LambdaExpression;
                        if (orderBy_lambda_temp.Body is MemberExpression && orderBy_lambda_temp.Parameters.Count > 0)
                        {
                            ParameterExpression orderBy_param_temp = orderBy_lambda_temp.Parameters.First();
                            MemberExpression orderBy_ma_temp = orderBy_lambda_temp.Body as MemberExpression;

                            //执行解析字段名称
                            DbColumnAttribute column = this._dbMappingHandler.GetDbColumnSingle(orderBy_param_temp.Type, orderBy_ma_temp.Member.Name);
                            this._result.SetOrderCondition(column.DbColumnName, true);
                        }
                    }
                    break;

                    #endregion
                case "OrderByDescending":
                    #region OrderByDescending

                    IEnumerable<Expression> orderByDes_args = base.VisitExpressionList(methodCallExp.Arguments);//执行该语句开始参数递归解析
                    Expression orderByDes_exp = orderByDes_args.Last();
                    if (orderByDes_exp != null)
                    {
                        UnaryExpression orderByDes_unary_temp = orderByDes_exp as UnaryExpression;
                        LambdaExpression orderByDes_lambda_temp = orderByDes_unary_temp.Operand as LambdaExpression;
                        if (orderByDes_lambda_temp.Body is MemberExpression && orderByDes_lambda_temp.Parameters.Count > 0)
                        {
                            ParameterExpression orderByDes_param_temp = orderByDes_lambda_temp.Parameters.First();
                            MemberExpression orderByDes_ma_temp = orderByDes_lambda_temp.Body as MemberExpression;

                            //执行解析字段名称
                            DbColumnAttribute column = this._dbMappingHandler.GetDbColumnSingle(orderByDes_param_temp.Type, orderByDes_ma_temp.Member.Name);
                            this._result.SetOrderCondition(column.DbColumnName, false);
                        }
                    }
                    break;

                    #endregion
                case "GroupBy":
                    #region GroupBy

                    IEnumerable<Expression> groupBy_args = base.VisitExpressionList(methodCallExp.Arguments);//执行该语句开始参数递归解析
                    Expression groupBy_exp = groupBy_args.Last();
                    if (groupBy_exp != null)
                    {
                        Expression tempExp = null;
                        //获取一元表达式的Operand中的表达式
                        if (groupBy_exp is UnaryExpression)
                        {
                            UnaryExpression unaryExp = groupBy_exp as UnaryExpression;
                            tempExp = unaryExp.Operand;
                        }

                        if (tempExp != null && tempExp is LambdaExpression)
                        {
                            LambdaExpression lambdaExp = tempExp as LambdaExpression;
                            string tempQuery = string.Empty;

                            if (lambdaExp.Body is MemberExpression)
                            {
                                MemberExpression memberExp = lambdaExp.Body as MemberExpression;

                                DbColumnAttribute column = this._dbMappingHandler.GetDbColumnSingle(memberExp.Member.ReflectedType, memberExp.Member.Name);
                                tempQuery = column.DbColumnName;
                            }

                            if (!string.IsNullOrEmpty(tempQuery))
                            {
                                this._result.SetGroupCondition(tempQuery);
                            }
                        }
                    }
                    break;

                    #endregion
                case "Count":
                    #region Count

                    IEnumerable<Expression> count_args = base.VisitExpressionList(methodCallExp.Arguments);//执行该语句开始参数递归解析
                    Expression count_exp = count_args.First();//Count表达式
                    Expression count_alias = count_args.Last();//Count别名

                    if (count_exp != null)
                    {
                        Expression tempExp = null;
                        //获取一元表达式的Operand中的表达式
                        if (count_exp is UnaryExpression)
                        {
                            UnaryExpression unaryExp = count_exp as UnaryExpression;
                            tempExp = unaryExp.Operand;
                        }
                        string aliasValue = string.Empty;
                        if (count_alias is ConstantExpression)
                        {
                            ConstantExpression aliasExp = count_alias as ConstantExpression;
                            aliasValue = null == aliasExp.Value ? string.Empty : aliasExp.Value.ToString();
                        }

                        if (tempExp != null && tempExp is LambdaExpression)
                        {
                            LambdaExpression lambdaExp = tempExp as LambdaExpression;
                            string tempQuery = string.Empty;

                            if (lambdaExp.Body is MemberExpression)
                            {
                                MemberExpression memberExp = lambdaExp.Body as MemberExpression;

                                DbColumnAttribute column = this._dbMappingHandler.GetDbColumnSingle(memberExp.Member.ReflectedType, memberExp.Member.Name);
                                tempQuery = column.DbColumnName;
                            }
                            else if (lambdaExp.Body is ConstantExpression)
                            {
                                tempQuery = ExpressionCalculater.GetValue(lambdaExp.Body).ToString();
                            }

                            if (!string.IsNullOrEmpty(tempQuery))
                            {
                                MysqlSelectField item = new MysqlSelectField();
                                item.DBFieldAsName = string.IsNullOrEmpty(aliasValue) ? ("count_" + (this._count_asNameIndex++)) : aliasValue;
                                item.DBSelectFragment = "count(" + tempQuery + ")";
                                this._result.SetSelectField(item);
                            }
                        }
                    }

                    break;

                    #endregion
                case "Sum":
                    #region Sum

                    IEnumerable<Expression> sum_args = base.VisitExpressionList(methodCallExp.Arguments);//执行该语句开始参数递归解析
                    Expression sum_exp = sum_args.First();//Count表达式
                    Expression sum_alias = sum_args.Last();//Count别名
                    if (sum_exp != null)
                    {
                        Expression tempExp = null;
                        //获取一元表达式的Operand中的表达式
                        if (sum_exp is UnaryExpression)
                        {
                            UnaryExpression unaryExp = sum_exp as UnaryExpression;
                            tempExp = unaryExp.Operand;
                        }
                        string aliasValue = string.Empty;
                        if (sum_alias is ConstantExpression)
                        {
                            ConstantExpression aliasExp = sum_alias as ConstantExpression;
                            aliasValue = null == aliasExp.Value ? string.Empty : aliasExp.Value.ToString();
                        }

                        if (tempExp != null && tempExp is LambdaExpression)
                        {
                            LambdaExpression lambdaExp = tempExp as LambdaExpression;
                            string tempQuery = string.Empty;

                            if (lambdaExp.Body is MemberExpression)
                            {
                                MemberExpression memberExp = lambdaExp.Body as MemberExpression;
                                DbColumnAttribute column = this._dbMappingHandler.GetDbColumnSingle(memberExp.Member.ReflectedType, memberExp.Member.Name);
                                tempQuery = column.DbColumnName;
                            }

                            if (!string.IsNullOrEmpty(tempQuery))
                            {
                                MysqlSelectField item = new MysqlSelectField();
                                item.DBFieldAsName = string.IsNullOrEmpty(aliasValue) ? ("sum_" + (this._sum_asNameIndex++)) : aliasValue;
                                item.DBSelectFragment = "sum(" + tempQuery + ")";
                                this._result.SetSelectField(item);
                            }
                        }
                    }
                    break;

                    #endregion
                case "Max":
                    #region Max

                    IEnumerable<Expression> max_args = base.VisitExpressionList(methodCallExp.Arguments);//执行该语句开始参数递归解析
                    Expression max_exp = max_args.First();//Count表达式
                    Expression max_alias = max_args.Last();//Count别名
                    if (max_exp != null)
                    {
                        Expression tempExp = null;
                        //获取一元表达式的Operand中的表达式
                        if (max_exp is UnaryExpression)
                        {
                            UnaryExpression unaryExp = max_exp as UnaryExpression;
                            tempExp = unaryExp.Operand;
                        }
                        string aliasValue = string.Empty;
                        if (max_alias is ConstantExpression)
                        {
                            ConstantExpression aliasExp = max_alias as ConstantExpression;
                            aliasValue = null == aliasExp.Value ? string.Empty : aliasExp.Value.ToString();
                        }

                        if (tempExp != null && tempExp is LambdaExpression)
                        {
                            LambdaExpression lambdaExp = tempExp as LambdaExpression;
                            string tempQuery = string.Empty;

                            if (lambdaExp.Body is MemberExpression)
                            {
                                MemberExpression memberExp = lambdaExp.Body as MemberExpression;
                                DbColumnAttribute column = this._dbMappingHandler.GetDbColumnSingle(memberExp.Member.ReflectedType, memberExp.Member.Name);
                                tempQuery = column.DbColumnName;
                            }

                            if (!string.IsNullOrEmpty(tempQuery))
                            {
                                MysqlSelectField item = new MysqlSelectField();
                                item.DBFieldAsName = string.IsNullOrEmpty(aliasValue) ? ("max_" + (this._max_asNameIndex++)) : aliasValue;
                                item.DBSelectFragment = "max(" + tempQuery + ")";
                                this._result.SetSelectField(item);
                            }
                        }
                    }
                    break;

                    #endregion
                case "Min":
                    #region Min

                    IEnumerable<Expression> min_args = base.VisitExpressionList(methodCallExp.Arguments);//执行该语句开始参数递归解析
                    Expression min_exp = min_args.First();//Count表达式
                    Expression min_alias = min_args.Last();//Count别名
                    if (min_exp != null)
                    {
                        Expression tempExp = null;
                        //获取一元表达式的Operand中的表达式
                        if (min_exp is UnaryExpression)
                        {
                            UnaryExpression unaryExp = min_exp as UnaryExpression;
                            tempExp = unaryExp.Operand;
                        }
                        string aliasValue = string.Empty;
                        if (min_alias is ConstantExpression)
                        {
                            ConstantExpression aliasExp = min_alias as ConstantExpression;
                            aliasValue = null == aliasExp.Value ? string.Empty : aliasExp.Value.ToString();
                        }

                        if (tempExp != null && tempExp is LambdaExpression)
                        {
                            LambdaExpression lambdaExp = tempExp as LambdaExpression;
                            string tempQuery = string.Empty;

                            if (lambdaExp.Body is MemberExpression)
                            {
                                MemberExpression memberExp = lambdaExp.Body as MemberExpression;
                                DbColumnAttribute column = this._dbMappingHandler.GetDbColumnSingle(memberExp.Member.ReflectedType, memberExp.Member.Name);
                                tempQuery = column.DbColumnName;
                            }

                            if (!string.IsNullOrEmpty(tempQuery))
                            {
                                MysqlSelectField item = new MysqlSelectField();
                                item.DBFieldAsName = string.IsNullOrEmpty(aliasValue) ? ("min_" + (this._min_asNameIndex++)) : aliasValue;
                                item.DBSelectFragment = "min(" + tempQuery + ")";
                                this._result.SetSelectField(item);
                            }
                        }
                    }
                    break;

                    #endregion
                default:
                    break;
            }
            //必须調用父类访问,否则无法继续表达式深度解析
            return base.VisitMethodCall(methodCallExp);
        }

        #endregion

        #region Stataic Methods

        /// <summary>
        /// 执行解析获取需要被查询的字段
        /// </summary>
        /// <param name="exp">表达式</param>
        /// <param name="dbMappingHandler">字段解析者实例</param>
        /// <returns></returns>
        public static MysqlSentenceResult ExecuteResolver(Expression exp, IDbMappingHandler dbMappingHandler)
        {
            //执行表达式解析 要被查询的字段
            MysqlSentenceHandler entity = new MysqlSentenceHandler(dbMappingHandler);
            entity.Visit(exp);
            return entity.Result;
        }

        #endregion
    }
}
