using AtomicCore.DbProvider;
using ClickHouse.Client.ADO;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AtomicCore.Integration.ClickHouseDbProvider
{
    /// <summary>
    /// ClickHouse Null Engine
    /// </summary>
    /// <typeparam name="M"></typeparam>
    public class ClickHouseNullEngine<M> : ClickHouseTableEngineBase, IClickHouseTableEngine<M>
        where M : IDbModel, new()
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbConnString"></param>
        /// <param name="dbMappingHandler"></param>
        public ClickHouseNullEngine(IDbConnectionString dbConnString, IDbMappingHandler dbMappingHandler)
            : base(dbConnString, dbMappingHandler)
        {
            
        }

        #endregion

        #region IDBRepository<M>

        /// <summary>
        /// 更新操作（局部更新）
        /// </summary>
        /// <param name="whereExp">需要被更新的条件</param>
        /// <param name="updatePropertys">需要被替换或更新的属性</param>
        /// <returns></returns>
        public DbNonRecord Update(Expression<Func<M, bool>> whereExp, Expression<Func<M, M>> updatePropertys)
        {
            DbNonRecord result = new DbNonRecord();

            string dbString = this._dbConnectionStringHandler.GetConnection();
            if (string.IsNullOrEmpty(dbString))
                throw new Exception("dbString is null");

            Type modelT = typeof(M);
            ClickHouseWhereScriptResult whereResult = null;
            ClickHouseUpdateScriptResult updatePropertyResult = null;

            #region 解析where条件

            //允许null，即不设置任何条件
            if (whereExp != null)
            {
                Expression where_func_lambdaExp = null;
                if (whereExp is LambdaExpression)
                {
                    //在方法参数上直接写条件
                    where_func_lambdaExp = whereExp;
                }
                else if (whereExp is MemberExpression)
                {
                    //通过条件组合的模式
                    object lambdaObject = ExpressionCalculater.GetValue(whereExp);
                    where_func_lambdaExp = lambdaObject as Expression;
                }
                else
                {
                    result.AppendError("尚未实现直接解析" + whereExp.NodeType.ToString() + "的特例");
                    return result;
                }

                //解析Where条件
                whereResult = ClickHouseWhereScriptHandler.ExecuteResolver(where_func_lambdaExp, this._dbMappingHandler, false);
                if (!whereResult.IsAvailable())
                {
                    result.CopyStatus(whereResult);
                    return result;
                }
            }

            #endregion

            #region 解析需要被更新的字段

            if (updatePropertys != null)
            {
                if (updatePropertys is LambdaExpression && updatePropertys.Body.NodeType == ExpressionType.MemberInit)
                {
                    updatePropertyResult = ClickHouseUpdateScriptHandler.ExecuteResolver(updatePropertys, this._dbMappingHandler);
                    if (!updatePropertyResult.IsAvailable())
                    {
                        result.CopyStatus(updatePropertyResult);
                        return result;
                    }
                }
                else
                {
                    result.AppendError("updatePropertys表达式格式异常,表达式格式必须是MemberInit,例如：d => new News() { Content = d.Content + \":已变更\" }");
                    return result;
                }
            }
            else
            {
                result.AppendError("updatePropertys不允许为null,至少指定一个需要被修改的列");
                return result;
            }

            #endregion

            #region 开始拼装Sql语句

            //获取所有的数据源列
            DbColumnAttribute[] colums = this._dbMappingHandler.GetDbColumnCollection(modelT);

            // 获取当前表或试图名
            string tableName = this._dbMappingHandler.GetDbTableName(modelT);

            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append($"update {ClickHouseGrammarRule.GenerateTableWrapped(tableName)} set ");
            foreach (var item in updatePropertyResult.FieldMembers)
            {
                //自增长的自动跳过
                if (colums.Any(d => d.PropertyNameMapping == item.PropertyItem.Name && d.IsDbGenerated))
                    continue;

                string cur_field = colums.First(d => d.PropertyNameMapping == item.PropertyItem.Name).DbColumnName;

                sqlBuilder.Append($"{ClickHouseGrammarRule.GenerateFieldWrapped(cur_field)}={item.UpdateTextFragment},");
            }
            sqlBuilder.Replace(",", " ", sqlBuilder.Length - 1, 1);
            if (whereResult != null)
            {
                sqlBuilder.Append("where ");
                sqlBuilder.Append(whereResult.TextScript);
            }
            sqlBuilder.Append(";");

            //初始化Debug
            result.DebugInit(sqlBuilder, ClickHouseGrammarRule.C_ParamChar);

            #endregion

            #region 执行Sql语句

            using (DbConnection connection = new ClickHouseConnection(dbString))
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.Connection = connection;
                    command.CommandText = sqlBuilder.ToString();

                    //尝试打开数据库连结
                    if (ClickHouseDbHelper.TryOpenDbConnection(connection, ref result))
                    {
                        try
                        {
                            result.AffectedRow = command.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            result.AppendError("sql语句执行异常," + command.CommandText);
                            result.AppendException(ex);

                            command.Dispose();
                            connection.Close();
                            connection.Dispose();

                            return result;
                        }
                    }
                }
            }

            #endregion

            return result;
        }

        /// <summary>
        /// 更新操作（整体更新）
        /// </summary>
        /// <param name="whereExp">需要被更新的条件</param>
        /// <param name="model">需要被整体替换的实体</param>
        /// <returns></returns>
        public DbNonRecord Update(Expression<Func<M, bool>> whereExp, M model)
        {
            throw new NotImplementedException("ClickHouse Not Supported");

            //DbNonRecord result = new DbNonRecord();

            //string dbString = this._dbConnectionStringHandler.GetConnection();
            //if (string.IsNullOrEmpty(dbString))
            //    throw new Exception("dbString is null");

            //Type modelT = typeof(M);
            //ClickHouseWhereScriptResult whereResult = null;

            //#region 验证需要被修改的实体是否为null

            //if (model == null)
            //{
            //    result.AppendError("修改数据时候的Model为空");
            //    return result;
            //}

            //#endregion

            //#region 解析where条件

            ////允许null，即不设置任何条件
            //if (whereExp != null)
            //{
            //    Expression where_func_lambdaExp = null;
            //    if (whereExp is LambdaExpression)
            //    {
            //        //在方法参数上直接写条件
            //        where_func_lambdaExp = whereExp;
            //    }
            //    else if (whereExp is MemberExpression)
            //    {
            //        //通过条件组合的模式
            //        object lambdaObject = ExpressionCalculater.GetValue(whereExp);
            //        where_func_lambdaExp = lambdaObject as Expression;
            //    }
            //    else
            //    {
            //        result.AppendError("尚未实现直接解析" + whereExp.NodeType.ToString() + "的特例");
            //        return result;
            //    }

            //    //执行where解析
            //    whereResult = ClickHouseWhereScriptHandler.ExecuteResolver(where_func_lambdaExp, this._dbMappingHandler, false);
            //    if (!whereResult.IsAvailable())
            //    {
            //        result.CopyStatus(whereResult);
            //        return result;
            //    }
            //}

            //#endregion

            //#region 开始拼接Sql语句

            //DbColumnAttribute[] columns = this._dbMappingHandler.GetDbColumnCollection(modelT, d => !d.IsDbGenerated);

            //// 获取当前表或试图名
            //string tableName = this._dbMappingHandler.GetDbTableName(modelT);
            //if (!string.IsNullOrEmpty(suffix))
            //    tableName = $"{tableName}{suffix}";

            //List<DbParameter> parameters = new List<DbParameter>();
            //DbParameter cur_parameter = null;
            //StringBuilder sqlBuilder = new StringBuilder("update ");
            //sqlBuilder.Append("[");
            //sqlBuilder.Append(tableName);
            //sqlBuilder.Append("]");
            //sqlBuilder.Append(" set ");
            //foreach (var item in columns)
            //{
            //    PropertyInfo p = modelT.GetProperty(item.PropertyNameMapping);
            //    if (p != null)
            //    {
            //        string parameterName = string.Format("set_{0}", item.DbColumnName);
            //        object parameterVal = p.GetValue(model, null);

            //        sqlBuilder.Append("[");
            //        sqlBuilder.Append(item.DbColumnName);
            //        sqlBuilder.Append("]");
            //        sqlBuilder.Append("=");
            //        sqlBuilder.Append("@");
            //        sqlBuilder.Append(parameterName);
            //        sqlBuilder.Append(",");

            //        cur_parameter = new ClickHouseDbParameter(ClickHouseGrammarRule.GenerateParamName(parameterName), parameterVal);
            //        parameters.Add(cur_parameter);
            //    }
            //}
            //sqlBuilder.Replace(",", " ", sqlBuilder.Length - 1, 1);
            //if (whereResult != null)
            //{
            //    sqlBuilder.Append("where ");
            //    sqlBuilder.Append(whereResult.TextScript);
            //    foreach (var item in whereResult.Parameters)
            //    {
            //        cur_parameter = new ClickHouseDbParameter(item.Name, item.Value);
            //        parameters.Add(cur_parameter);
            //    }
            //}
            //sqlBuilder.Append(";");
            ////初始化Debug
            //result.DebugInit(sqlBuilder, ClickHouseGrammarRule.C_ParamChar, parameters.ToArray());

            //#endregion

            //#region 执行Sql语句

            //using (DbConnection connection = new ClickHouseConnection(dbString))
            //{
            //    using (DbCommand command = new SqlCommand())
            //    {
            //        command.Connection = connection;
            //        command.CommandText = sqlBuilder.ToString();
            //        foreach (DbParameter item in parameters)
            //            command.Parameters.Add(item);

            //        //尝试打开数据库连结
            //        if (this.TryOpenDbConnection(connection, ref result))
            //        {
            //            try
            //            {
            //                result.AffectedRow = command.ExecuteNonQuery();
            //            }
            //            catch (Exception ex)
            //            {
            //                result.AppendError("sql语句执行异常," + command.CommandText);
            //                result.AppendException(ex);

            //                command.Dispose();
            //                connection.Close();
            //                connection.Dispose();

            //                return result;
            //            }
            //        }
            //    }
            //}

            //#endregion

            //return result;
        }

        /// <summary>
        /// 批量更新任务（在一个conn.open里执行多个更新,避免多次开关造成性能损失）
        /// </summary>
        /// <param name="taskList">任务数据</param>
        /// <param name="enableSqlTransaction">是否启动SQL事务（对于单例调用最好启用，对于外层套用事务的不需要启动）</param>
        /// <returns></returns>
        public DbNonRecord UpdateTask(IEnumerable<DbUpdateTaskData<M>> taskList, bool enableSqlTransaction = false)
        {
            throw new NotImplementedException("ClickHouse Not Supported");

            //DbNonRecord result = new DbNonRecord();

            //if (null == taskList || !taskList.Any())
            //    return result;

            //string dbString = this._dbConnectionStringHandler.GetConnection();
            //if (string.IsNullOrEmpty(dbString))
            //    throw new Exception("dbString is null");

            //#region 定义全局参数

            //Type modelT = typeof(M);

            //#endregion

            //#region 开始循环解析任务

            //List<DbUpdateTaskSqlData> sqlDataList = new List<DbUpdateTaskSqlData>();

            //foreach (var task in taskList)
            //{
            //    #region 跳过循环特殊条件

            //    if (null == task.WhereExp || null == task.UpdatePropertys)
            //        continue;

            //    #endregion

            //    #region 条件解析

            //    Expression where_func_lambdaExp = null;
            //    if (task.WhereExp is LambdaExpression)
            //    {
            //        //在方法参数上直接写条件
            //        where_func_lambdaExp = task.WhereExp;
            //    }
            //    else if (task.WhereExp is MemberExpression)
            //    {
            //        //通过条件组合的模式
            //        object lambdaObject = ExpressionCalculater.GetValue(task.WhereExp);
            //        where_func_lambdaExp = lambdaObject as Expression;
            //    }
            //    else
            //    {
            //        result.AppendError("尚未实现直接解析" + task.WhereExp.NodeType.ToString() + "的特例");
            //        return result;
            //    }

            //    //解析Where条件
            //    ClickHouseWhereScriptResult whereResult = ClickHouseWhereScriptHandler.ExecuteResolver(where_func_lambdaExp, this._dbMappingHandler, false);
            //    if (!whereResult.IsAvailable())
            //    {
            //        result.CopyStatus(whereResult);
            //        return result;
            //    }

            //    #endregion

            //    #region 更新字段解析

            //    ClickHouseUpdateScriptResult updatePropertyResult = null;
            //    if (task.UpdatePropertys is LambdaExpression && task.UpdatePropertys.Body.NodeType == ExpressionType.MemberInit)
            //    {
            //        updatePropertyResult = ClickHouseUpdateScriptHandler.ExecuteResolver(task.UpdatePropertys, this._dbMappingHandler);
            //        if (!updatePropertyResult.IsAvailable())
            //        {
            //            result.CopyStatus(updatePropertyResult);
            //            return result;
            //        }
            //    }
            //    else
            //    {
            //        result.AppendError("updatePropertys表达式格式异常,表达式格式必须是MemberInit,例如：d => new News() { Content = d.Content + \":已变更\" }");
            //        return result;
            //    }

            //    #endregion

            //    #region 开始拼装Sql语句

            //    //获取所有的数据源列
            //    DbColumnAttribute[] colums = this._dbMappingHandler.GetDbColumnCollection(modelT);

            //    // 获取当前表或试图名
            //    string tableName = this._dbMappingHandler.GetDbTableName(modelT);
            //    if (!string.IsNullOrEmpty(suffix))
            //        tableName = $"{tableName}{suffix}";

            //    List<DbParameter> parameters = new List<DbParameter>();
            //    DbParameter cur_parameter = null;
            //    StringBuilder sqlBuilder = new StringBuilder("update ");
            //    sqlBuilder.Append("[");
            //    sqlBuilder.Append(tableName);
            //    sqlBuilder.Append("]");
            //    sqlBuilder.Append(" set ");
            //    foreach (var item in updatePropertyResult.FieldMembers)
            //    {
            //        //自增长的自动跳过
            //        if (colums.Any(d => d.PropertyNameMapping == item.PropertyItem.Name && d.IsDbGenerated))
            //            continue;

            //        string cur_field = colums.First(d => d.PropertyNameMapping == item.PropertyItem.Name).DbColumnName;

            //        sqlBuilder.Append(" ");
            //        sqlBuilder.Append("[");
            //        sqlBuilder.Append(cur_field);
            //        sqlBuilder.Append("]");
            //        sqlBuilder.Append("=");
            //        sqlBuilder.Append(item.UpdateTextFragment);
            //        sqlBuilder.Append(",");

            //        foreach (var pitem in item.Parameter)
            //        {
            //            cur_parameter = new ClickHouseDbParameter(pitem.Name, pitem.Value);
            //            parameters.Add(cur_parameter);
            //        }
            //    }
            //    sqlBuilder.Replace(",", " ", sqlBuilder.Length - 1, 1);
            //    if (whereResult != null)
            //    {
            //        sqlBuilder.Append("where ");
            //        sqlBuilder.Append(whereResult.TextScript);
            //        foreach (var item in whereResult.Parameters)
            //        {
            //            cur_parameter = new ClickHouseDbParameter(item.Name, item.Value);
            //            parameters.Add(cur_parameter);
            //        }
            //    }
            //    sqlBuilder.Append(";");

            //    #endregion

            //    #region 填充SqlDataList

            //    sqlDataList.Add(new DbUpdateTaskSqlData()
            //    {
            //        SqlText = sqlBuilder.ToString(),
            //        SqlParameters = parameters.ToArray()
            //    });

            //    #endregion
            //}

            //#endregion

            //#region 执行Sql语句

            //using (ClickHouseConnection connection = new ClickHouseConnection(dbString))
            //{
            //    using (SqlCommand command = new SqlCommand())
            //    {
            //        command.Connection = connection;

            //        //尝试打开数据库连结
            //        if (this.TryOpenDbConnection(connection, ref result))
            //        {
            //            //判断是否需要开启事务
            //            if (enableSqlTransaction)
            //                command.Transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

            //            //开始循环执行Sql
            //            foreach (var sql in sqlDataList)
            //            {
            //                command.CommandText = sql.SqlText;
            //                command.Parameters.Clear();
            //                command.Parameters.AddRange(sql.SqlParameters);

            //                try
            //                {
            //                    result.AffectedRow += command.ExecuteNonQuery();
            //                }
            //                catch (Exception ex)
            //                {
            //                    result.AppendError("sql语句执行异常," + command.CommandText);
            //                    result.AppendException(ex);

            //                    if (enableSqlTransaction && null != command.Transaction)
            //                        command.Transaction.Rollback();

            //                    command.Dispose();
            //                    connection.Close();
            //                    connection.Dispose();

            //                    return result;
            //                }
            //            }

            //            if (enableSqlTransaction && null != command.Transaction)
            //                command.Transaction.Commit();
            //        }
            //    }
            //}

            //#endregion

            //return result;
        }

        /// <summary>
        /// 删除操作
        /// </summary>
        /// <param name="deleteExp">删除条件</param>
        /// <returns></returns>
        public DbNonRecord Delete(Expression<Func<M, bool>> deleteExp)
        {
            DbNonRecord result = new DbNonRecord();
            if (null == deleteExp)
            {
                result.AppendError("不允许传入null条件进行删除，此行为属于非法行为！");
                return result;
            }

            string dbString = this._dbConnectionStringHandler.GetConnection();
            if (string.IsNullOrEmpty(dbString))
                throw new Exception("dbString is null");

            Type modelT = typeof(M);

            #region 解析条件语句

            Expression where_func_lambdaExp;
            if (deleteExp is LambdaExpression)
            {
                //在方法参数上直接写条件
                where_func_lambdaExp = deleteExp;
            }
            else if (deleteExp is MemberExpression)
            {
                //通过条件组合的模式
                object lambdaObject = ExpressionCalculater.GetValue(deleteExp);
                where_func_lambdaExp = lambdaObject as Expression;
            }
            else
            {
                result.AppendError("尚未实现直接解析" + deleteExp.NodeType.ToString() + "的特例");
                return result;
            }

            //执行where解析
            ClickHouseWhereScriptResult whereResult = ClickHouseWhereScriptHandler.ExecuteResolver(where_func_lambdaExp, this._dbMappingHandler, false);
            if (!whereResult.IsAvailable())
            {
                result.CopyStatus(whereResult);
                return result;
            }

            #endregion

            #region 拼接Sql语句

            // 获取当前表或试图名
            string tableName = this._dbMappingHandler.GetDbTableName(modelT);

            // 这只适用于批量删除，不适合频繁的行级别操作
            StringBuilder sqlBuilder = new StringBuilder($"alter table {ClickHouseGrammarRule.GenerateTableWrapped(tableName)} delete where {whereResult.TextScript};");

            // 删除前查询可能会受影响函数（不准确）
            StringBuilder countBuilder = new StringBuilder($"select count(*) from {ClickHouseGrammarRule.GenerateTableWrapped(tableName)} where {whereResult.TextScript};");

            //初始化Debug
            result.DebugInit(sqlBuilder, ClickHouseGrammarRule.C_ParamChar);

            #endregion

            #region 开始执行Sql语句

            using (DbConnection connection = new ClickHouseConnection(dbString))
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.Connection = connection;
                    command.CommandText = sqlBuilder.ToString();

                    //尝试打开数据库连结
                    if (ClickHouseDbHelper.TryOpenDbConnection(connection, ref result))
                    {
                        // 执行待删除的数据
                        try
                        {
                            command.CommandText = countBuilder.ToString();
                            result.AffectedRow = Convert.ToInt32(command.ExecuteScalar());
                        }
                        catch (Exception ex)
                        {
                            result.AppendError("sql语句执行异常," + command.CommandText);
                            result.AppendException(ex);

                            command.Dispose();
                            connection.Close();
                            connection.Dispose();

                            return result;
                        }

                        // 执行真实数据删除
                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            result.AppendError("sql语句执行异常," + command.CommandText);
                            result.AppendException(ex);

                            command.Dispose();
                            connection.Close();
                            connection.Dispose();

                            return result;
                        }
                    }
                }
            }

            #endregion

            return result;
        }

        #endregion

        #region IDbAsyncProvider<M>

        /// <summary>
        /// 更新操作（局部更新）
        /// </summary>
        /// <param name="whereExp">需要被更新的条件</param>
        /// <param name="updatePropertys">需要被替换或更新的属性</param>
        /// <returns></returns>
        public Task<DbNonRecord> UpdateAsync(Expression<Func<M, bool>> whereExp, Expression<Func<M, M>> updatePropertys)
        {
            throw new NotImplementedException("ClickHouse Not Supported");

            //DbNonRecord result = new DbNonRecord();

            //string dbString = this._dbConnectionStringHandler.GetConnection();
            //if (string.IsNullOrEmpty(dbString))
            //    throw new Exception("dbString is null");

            //Type modelT = typeof(M);
            //ClickHouseWhereScriptResult whereResult = null;
            //ClickHouseUpdateScriptResult updatePropertyResult = null;

            //#region 解析where条件

            ////允许null，即不设置任何条件
            //if (whereExp != null)
            //{
            //    Expression where_func_lambdaExp = null;
            //    if (whereExp is LambdaExpression)
            //    {
            //        //在方法参数上直接写条件
            //        where_func_lambdaExp = whereExp;
            //    }
            //    else if (whereExp is MemberExpression)
            //    {
            //        //通过条件组合的模式
            //        object lambdaObject = ExpressionCalculater.GetValue(whereExp);
            //        where_func_lambdaExp = lambdaObject as Expression;
            //    }
            //    else
            //    {
            //        result.AppendError("尚未实现直接解析" + whereExp.NodeType.ToString() + "的特例");
            //        return result;
            //    }

            //    //解析Where条件
            //    whereResult = ClickHouseWhereScriptHandler.ExecuteResolver(where_func_lambdaExp, this._dbMappingHandler, false);
            //    if (!whereResult.IsAvailable())
            //    {
            //        result.CopyStatus(whereResult);
            //        return result;
            //    }
            //}

            //#endregion

            //#region 解析需要被更新的字段

            //if (updatePropertys != null)
            //{
            //    if (updatePropertys is LambdaExpression && updatePropertys.Body.NodeType == ExpressionType.MemberInit)
            //    {
            //        updatePropertyResult = ClickHouseUpdateScriptHandler.ExecuteResolver(updatePropertys, this._dbMappingHandler);
            //        if (!updatePropertyResult.IsAvailable())
            //        {
            //            result.CopyStatus(updatePropertyResult);
            //            return result;
            //        }
            //    }
            //    else
            //    {
            //        result.AppendError("updatePropertys表达式格式异常,表达式格式必须是MemberInit,例如：d => new News() { Content = d.Content + \":已变更\" }");
            //        return result;
            //    }
            //}
            //else
            //{
            //    result.AppendError("updatePropertys不允许为null,至少指定一个需要被修改的列");
            //    return result;
            //}

            //#endregion

            //#region 开始拼装Sql语句

            ////获取所有的数据源列
            //DbColumnAttribute[] colums = this._dbMappingHandler.GetDbColumnCollection(modelT);

            //// 获取表名
            //string tableName = this._dbMappingHandler.GetDbTableName(modelT);
            //if (!string.IsNullOrEmpty(suffix))
            //    tableName = $"{tableName}{suffix}";

            //List<DbParameter> parameters = new List<DbParameter>();
            //DbParameter cur_parameter = null;
            //StringBuilder sqlBuilder = new StringBuilder("update ");
            //sqlBuilder.Append("[");
            //sqlBuilder.Append(tableName);
            //sqlBuilder.Append("]");
            //sqlBuilder.Append(" set ");
            //foreach (var item in updatePropertyResult.FieldMembers)
            //{
            //    //自增长的自动跳过
            //    if (colums.Any(d => d.PropertyNameMapping == item.PropertyItem.Name && d.IsDbGenerated))
            //    {
            //        continue;
            //    }

            //    string cur_field = colums.First(d => d.PropertyNameMapping == item.PropertyItem.Name).DbColumnName;

            //    sqlBuilder.Append(" ");
            //    sqlBuilder.Append("[");
            //    sqlBuilder.Append(cur_field);
            //    sqlBuilder.Append("]");
            //    sqlBuilder.Append("=");
            //    sqlBuilder.Append(item.UpdateTextFragment);
            //    sqlBuilder.Append(",");

            //    foreach (var pitem in item.Parameter)
            //    {
            //        cur_parameter = new ClickHouseDbParameter(pitem.Name, pitem.Value);
            //        parameters.Add(cur_parameter);
            //    }
            //}
            //sqlBuilder.Replace(",", " ", sqlBuilder.Length - 1, 1);
            //if (whereResult != null)
            //{
            //    sqlBuilder.Append("where ");
            //    sqlBuilder.Append(whereResult.TextScript);
            //    foreach (var item in whereResult.Parameters)
            //    {
            //        cur_parameter = new ClickHouseDbParameter(item.Name, item.Value);
            //        parameters.Add(cur_parameter);
            //    }
            //}
            //sqlBuilder.Append(";");

            ////初始化Debug
            //result.DebugInit(sqlBuilder, ClickHouseGrammarRule.C_ParamChar, parameters.ToArray());

            //#endregion

            //#region 执行Sql语句

            //using (DbConnection connection = new ClickHouseConnection(dbString))
            //{
            //    using (DbCommand command = new SqlCommand())
            //    {
            //        command.Connection = connection;
            //        command.CommandText = sqlBuilder.ToString();
            //        foreach (DbParameter item in parameters)
            //            command.Parameters.Add(item);

            //        // 尝试打开数据库链接
            //        try
            //        {
            //            await connection.OpenAsync();
            //        }
            //        catch (Exception ex)
            //        {
            //            result.AppendException(ex);

            //            await command.DisposeAsync();
            //            await connection.CloseAsync();
            //            await connection.DisposeAsync();

            //            return result;
            //        }

            //        try
            //        {
            //            result.AffectedRow = await command.ExecuteNonQueryAsync();
            //        }
            //        catch (Exception ex)
            //        {
            //            result.AppendError("sql语句执行异常," + command.CommandText);
            //            result.AppendException(ex);

            //            await command.DisposeAsync();
            //            await connection.CloseAsync();
            //            await connection.DisposeAsync();

            //            return result;
            //        }
            //    }
            //}

            //#endregion

            //return result;
        }

        /// <summary>
        /// 更新操作（整体更新）
        /// </summary>
        /// <param name="whereExp">需要被更新的条件</param>
        /// <param name="model">需要被整体替换的实体</param>
        /// <returns></returns>
        public Task<DbNonRecord> UpdateAsync(Expression<Func<M, bool>> whereExp, M model)
        {
            throw new NotImplementedException("ClickHouse Not Supported");

            //DbNonRecord result = new DbNonRecord();

            //string dbString = this._dbConnectionStringHandler.GetConnection();
            //if (string.IsNullOrEmpty(dbString))
            //    throw new Exception("dbString is null");

            //Type modelT = typeof(M);
            //ClickHouseWhereScriptResult whereResult = null;

            //#region 验证需要被修改的实体是否为null

            //if (model == null)
            //{
            //    result.AppendError("修改数据时候的Model为空");
            //    return result;
            //}

            //#endregion

            //#region 解析where条件

            ////允许null，即不设置任何条件
            //if (whereExp != null)
            //{
            //    Expression where_func_lambdaExp = null;
            //    if (whereExp is LambdaExpression)
            //    {
            //        //在方法参数上直接写条件
            //        where_func_lambdaExp = whereExp;
            //    }
            //    else if (whereExp is MemberExpression)
            //    {
            //        //通过条件组合的模式
            //        object lambdaObject = ExpressionCalculater.GetValue(whereExp);
            //        where_func_lambdaExp = lambdaObject as Expression;
            //    }
            //    else
            //    {
            //        result.AppendError("尚未实现直接解析" + whereExp.NodeType.ToString() + "的特例");
            //        return result;
            //    }

            //    //执行where解析
            //    whereResult = ClickHouseWhereScriptHandler.ExecuteResolver(where_func_lambdaExp, this._dbMappingHandler, false);
            //    if (!whereResult.IsAvailable())
            //    {
            //        result.CopyStatus(whereResult);
            //        return result;
            //    }
            //}

            //#endregion

            //#region 开始拼接Sql语句

            //DbColumnAttribute[] columns = this._dbMappingHandler.GetDbColumnCollection(modelT, d => !d.IsDbGenerated);

            //// 获取表名
            //string tableName = this._dbMappingHandler.GetDbTableName(modelT);
            //if (!string.IsNullOrEmpty(suffix))
            //    tableName = $"{tableName}{suffix}";

            //List<DbParameter> parameters = new List<DbParameter>();
            //DbParameter cur_parameter = null;
            //StringBuilder sqlBuilder = new StringBuilder("update ");
            //sqlBuilder.Append("[");
            //sqlBuilder.Append(tableName);
            //sqlBuilder.Append("]");
            //sqlBuilder.Append(" set ");
            //foreach (var item in columns)
            //{
            //    PropertyInfo p = modelT.GetProperty(item.PropertyNameMapping);
            //    if (p != null)
            //    {
            //        string parameterName = string.Format("set_{0}", item.DbColumnName);
            //        object parameterVal = p.GetValue(model, null);

            //        sqlBuilder.Append("[");
            //        sqlBuilder.Append(item.DbColumnName);
            //        sqlBuilder.Append("]");
            //        sqlBuilder.Append("=");
            //        sqlBuilder.Append("@");
            //        sqlBuilder.Append(parameterName);
            //        sqlBuilder.Append(",");

            //        cur_parameter = new ClickHouseDbParameter(ClickHouseGrammarRule.GenerateParamName(parameterName), parameterVal);
            //        parameters.Add(cur_parameter);
            //    }
            //}
            //sqlBuilder.Replace(",", " ", sqlBuilder.Length - 1, 1);
            //if (whereResult != null)
            //{
            //    sqlBuilder.Append("where ");
            //    sqlBuilder.Append(whereResult.TextScript);
            //    foreach (var item in whereResult.Parameters)
            //    {
            //        cur_parameter = new ClickHouseDbParameter(item.Name, item.Value);
            //        parameters.Add(cur_parameter);
            //    }
            //}
            //sqlBuilder.Append(";");

            ////初始化Debug
            //result.DebugInit(sqlBuilder, ClickHouseGrammarRule.C_ParamChar, parameters.ToArray());

            //#endregion

            //#region 执行Sql语句

            //using (DbConnection connection = new ClickHouseConnection(dbString))
            //{
            //    using (DbCommand command = new SqlCommand())
            //    {
            //        command.Connection = connection;
            //        command.CommandText = sqlBuilder.ToString();
            //        foreach (DbParameter item in parameters)
            //            command.Parameters.Add(item);

            //        // 尝试打开数据库链接
            //        try
            //        {
            //            await connection.OpenAsync();
            //        }
            //        catch (Exception ex)
            //        {
            //            result.AppendException(ex);

            //            await command.DisposeAsync();
            //            await connection.CloseAsync();
            //            await connection.DisposeAsync();

            //            return result;
            //        }

            //        // 执行查询语句
            //        try
            //        {
            //            result.AffectedRow = await command.ExecuteNonQueryAsync();
            //        }
            //        catch (Exception ex)
            //        {
            //            result.AppendError("sql语句执行异常," + command.CommandText);
            //            result.AppendException(ex);

            //            await command.DisposeAsync();
            //            await connection.CloseAsync();
            //            await connection.DisposeAsync();

            //            return result;
            //        }
            //    }
            //}

            //#endregion

            //return result;
        }

        /// <summary>
        /// 批量更新任务（在一个conn.open里执行多个更新,避免多次开关造成性能损失）
        /// </summary>
        /// <param name="taskList">任务数据</param>
        /// <param name="enableSqlTransaction">是否启动SQL事务（对于单例调用最好启用，对于外层套用事务的不需要启动）</param>
        /// <returns></returns>
        public Task<DbNonRecord> UpdateTaskAsync(IEnumerable<DbUpdateTaskData<M>> taskList, bool enableSqlTransaction = false)
        {
            throw new NotImplementedException("ClickHouse Not Supported");

            //DbNonRecord result = new DbNonRecord();

            //if (null == taskList || !taskList.Any())
            //    return result;

            //string dbString = this._dbConnectionStringHandler.GetConnection();
            //if (string.IsNullOrEmpty(dbString))
            //    throw new Exception("dbString is null");

            //#region 定义全局参数

            //Type modelT = typeof(M);
            //string tableName = this._dbMappingHandler.GetDbTableName(modelT);
            //if (!string.IsNullOrEmpty(suffix))
            //    tableName = $"{tableName}{suffix}";
            //DbColumnAttribute[] columns = this._dbMappingHandler.GetDbColumnCollection(modelT, d => !d.IsDbGenerated);

            //#endregion

            //#region 开始循环解析任务

            //List<DbUpdateTaskSqlData> sqlDataList = new List<DbUpdateTaskSqlData>();

            //foreach (var task in taskList)
            //{
            //    #region 跳过循环特殊条件

            //    if (null == task.WhereExp || null == task.UpdatePropertys)
            //        continue;

            //    #endregion

            //    #region 条件解析

            //    Expression where_func_lambdaExp = null;
            //    if (task.WhereExp is LambdaExpression)
            //    {
            //        //在方法参数上直接写条件
            //        where_func_lambdaExp = task.WhereExp;
            //    }
            //    else if (task.WhereExp is MemberExpression)
            //    {
            //        //通过条件组合的模式
            //        object lambdaObject = ExpressionCalculater.GetValue(task.WhereExp);
            //        where_func_lambdaExp = lambdaObject as Expression;
            //    }
            //    else
            //    {
            //        result.AppendError("尚未实现直接解析" + task.WhereExp.NodeType.ToString() + "的特例");
            //        return result;
            //    }

            //    //解析Where条件
            //    ClickHouseWhereScriptResult whereResult = ClickHouseWhereScriptHandler.ExecuteResolver(where_func_lambdaExp, this._dbMappingHandler, false);
            //    if (!whereResult.IsAvailable())
            //    {
            //        result.CopyStatus(whereResult);
            //        return result;
            //    }

            //    #endregion

            //    #region 更新字段解析

            //    ClickHouseUpdateScriptResult updatePropertyResult = null;
            //    if (task.UpdatePropertys is LambdaExpression && task.UpdatePropertys.Body.NodeType == ExpressionType.MemberInit)
            //    {
            //        updatePropertyResult = ClickHouseUpdateScriptHandler.ExecuteResolver(task.UpdatePropertys, this._dbMappingHandler);
            //        if (!updatePropertyResult.IsAvailable())
            //        {
            //            result.CopyStatus(updatePropertyResult);
            //            return result;
            //        }
            //    }
            //    else
            //    {
            //        result.AppendError("updatePropertys表达式格式异常,表达式格式必须是MemberInit,例如：d => new News() { Content = d.Content + \":已变更\" }");
            //        return result;
            //    }

            //    #endregion

            //    #region 开始拼装Sql语句

            //    //获取所有的数据源列
            //    DbColumnAttribute[] colums = this._dbMappingHandler.GetDbColumnCollection(modelT);

            //    List<DbParameter> parameters = new List<DbParameter>();
            //    DbParameter cur_parameter = null;
            //    StringBuilder sqlBuilder = new StringBuilder("update ");
            //    sqlBuilder.Append("[");
            //    sqlBuilder.Append(tableName);
            //    sqlBuilder.Append("]");
            //    sqlBuilder.Append(" set ");
            //    foreach (var item in updatePropertyResult.FieldMembers)
            //    {
            //        //自增长的自动跳过
            //        if (colums.Any(d => d.PropertyNameMapping == item.PropertyItem.Name && d.IsDbGenerated))
            //            continue;

            //        string cur_field = colums.First(d => d.PropertyNameMapping == item.PropertyItem.Name).DbColumnName;

            //        sqlBuilder.Append(" ");
            //        sqlBuilder.Append("[");
            //        sqlBuilder.Append(cur_field);
            //        sqlBuilder.Append("]");
            //        sqlBuilder.Append("=");
            //        sqlBuilder.Append(item.UpdateTextFragment);
            //        sqlBuilder.Append(",");

            //        foreach (var pitem in item.Parameter)
            //        {
            //            cur_parameter = new ClickHouseDbParameter(pitem.Name, pitem.Value);
            //            parameters.Add(cur_parameter);
            //        }
            //    }
            //    sqlBuilder.Replace(",", " ", sqlBuilder.Length - 1, 1);
            //    if (whereResult != null)
            //    {
            //        sqlBuilder.Append("where ");
            //        sqlBuilder.Append(whereResult.TextScript);
            //        foreach (var item in whereResult.Parameters)
            //        {
            //            cur_parameter = new ClickHouseDbParameter(item.Name, item.Value);
            //            parameters.Add(cur_parameter);
            //        }
            //    }
            //    sqlBuilder.Append(";");

            //    #endregion

            //    #region 填充SqlDataList

            //    sqlDataList.Add(new DbUpdateTaskSqlData()
            //    {
            //        SqlText = sqlBuilder.ToString(),
            //        SqlParameters = parameters.ToArray()
            //    });

            //    #endregion
            //}

            //#endregion

            //#region 执行Sql语句

            //using (ClickHouseConnection connection = new ClickHouseConnection(dbString))
            //{
            //    using (SqlCommand command = new SqlCommand())
            //    {
            //        command.Connection = connection;

            //        // 尝试打开数据库链接
            //        try
            //        {
            //            await connection.OpenAsync();
            //        }
            //        catch (Exception ex)
            //        {
            //            result.AppendException(ex);

            //            await command.DisposeAsync();
            //            await connection.CloseAsync();
            //            await connection.DisposeAsync();

            //            return result;
            //        }

            //        //判断是否需要开启事务
            //        if (enableSqlTransaction)
            //        {
            //            var tx = (SqlTransaction)await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            //            command.Transaction = tx;
            //        }

            //        //开始循环执行Sql
            //        foreach (var sql in sqlDataList)
            //        {
            //            command.CommandText = sql.SqlText;
            //            command.Parameters.Clear();
            //            command.Parameters.AddRange(sql.SqlParameters);

            //            try
            //            {
            //                result.AffectedRow += await command.ExecuteNonQueryAsync();
            //            }
            //            catch (Exception ex)
            //            {
            //                result.AppendError("sql语句执行异常," + command.CommandText);
            //                result.AppendException(ex);

            //                if (enableSqlTransaction && null != command.Transaction)
            //                    await command.Transaction.RollbackAsync();

            //                await command.DisposeAsync();
            //                await connection.CloseAsync();
            //                await connection.DisposeAsync();

            //                return result;
            //            }
            //        }

            //        if (enableSqlTransaction && null != command.Transaction)
            //            await command.Transaction.CommitAsync();
            //    }
            //}

            //#endregion

            //return result;
        }

        /// <summary>
        /// 删除操作
        /// </summary>
        /// <param name="deleteExp">删除条件</param>
        /// <returns></returns>
        public Task<DbNonRecord> DeleteAsync(Expression<Func<M, bool>> deleteExp)
        {
            throw new NotImplementedException("ClickHouse Not Supported");

            //DbNonRecord result = new DbNonRecord();

            //string dbString = this._dbConnectionStringHandler.GetConnection();
            //if (string.IsNullOrEmpty(dbString))
            //    throw new Exception("dbString is null");

            //Type modelT = typeof(M);
            //ClickHouseWhereScriptResult whereResult;
            //if (deleteExp != null)
            //{
            //    #region 解析条件语句

            //    Expression where_func_lambdaExp;
            //    if (deleteExp is LambdaExpression)
            //    {
            //        //在方法参数上直接写条件
            //        where_func_lambdaExp = deleteExp;
            //    }
            //    else if (deleteExp is MemberExpression)
            //    {
            //        //通过条件组合的模式
            //        object lambdaObject = ExpressionCalculater.GetValue(deleteExp);
            //        where_func_lambdaExp = lambdaObject as Expression;
            //    }
            //    else
            //    {
            //        result.AppendError("尚未实现直接解析" + deleteExp.NodeType.ToString() + "的特例");
            //        return result;
            //    }

            //    //执行where解析
            //    whereResult = ClickHouseWhereScriptHandler.ExecuteResolver(where_func_lambdaExp, this._dbMappingHandler, false);
            //    if (!whereResult.IsAvailable())
            //    {
            //        result.CopyStatus(whereResult);
            //        return result;
            //    }

            //    #endregion

            //    #region 拼接Sql语句

            //    string tableName = this._dbMappingHandler.GetDbTableName(modelT);
            //    if (!string.IsNullOrEmpty(suffix))
            //        tableName = $"{tableName}{suffix}";

            //    List<DbParameter> parameters = new List<DbParameter>();
            //    DbParameter cur_parameter;
            //    StringBuilder sqlBuilder = new StringBuilder("delete from ");
            //    sqlBuilder.Append("[");
            //    sqlBuilder.Append(tableName);
            //    sqlBuilder.Append("]");
            //    sqlBuilder.Append(" where ");
            //    sqlBuilder.Append(whereResult.TextScript);
            //    foreach (var item in whereResult.Parameters)
            //    {
            //        cur_parameter = new ClickHouseDbParameter(item.Name, item.Value);
            //        parameters.Add(cur_parameter);
            //    }
            //    sqlBuilder.Append(";");
            //    //初始化Debug
            //    result.DebugInit(sqlBuilder, ClickHouseGrammarRule.C_ParamChar, parameters.ToArray());

            //    #endregion

            //    #region 开始执行Sql语句

            //    using (DbConnection connection = new ClickHouseConnection(dbString))
            //    {
            //        using (DbCommand command = new SqlCommand())
            //        {
            //            command.Connection = connection;
            //            command.CommandText = sqlBuilder.ToString();
            //            foreach (DbParameter item in parameters)
            //                command.Parameters.Add(item);

            //            // 尝试打开数据库链接
            //            try
            //            {
            //                await connection.OpenAsync();
            //            }
            //            catch (Exception ex)
            //            {
            //                result.AppendException(ex);

            //                await command.DisposeAsync();
            //                await connection.CloseAsync();
            //                await connection.DisposeAsync();

            //                return result;
            //            }

            //            // 执行数据查询
            //            try
            //            {
            //                result.AffectedRow = await command.ExecuteNonQueryAsync();
            //            }
            //            catch (Exception ex)
            //            {
            //                result.AppendError("sql语句执行异常," + command.CommandText);
            //                result.AppendException(ex);

            //                await command.DisposeAsync();
            //                await connection.CloseAsync();
            //                await connection.DisposeAsync();

            //                return result;
            //            }
            //        }
            //    }

            //    #endregion

            //    return result;
            //}
            //else
            //{
            //    result.AppendError("不允许传入null条件进行删除，此行为属于非法行为！");
            //    return result;
            //}
        }

        #endregion
    }
}
