using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AtomicCore.DbProvider;
using ClickHouse.Client.ADO;
using ClickHouse.Client.Copy;

namespace AtomicCore.Integration.ClickHouseDbProvider
{
    /// <summary>
    /// ClickHouse的数据仓储驱动类
    /// </summary>
    /// <typeparam name="M"></typeparam>
    public class ClickHouseDbProvider<M> : IDbProvider<M>, IDbConnectionString, IDbConnectionString<M>
        where M : IDbModel, new()
    {
        #region Constructors

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private readonly string _dbConnQuery = null;

        /// <summary>
        /// 数据库链接字符串 
        /// </summary>
        private readonly IDbConnectionString _dbConnectionStringHandler = null;

        /// <summary>
        /// 数据库字段映射处理接口
        /// </summary>
        private readonly IDbMappingHandler _dbMappingHandler = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbConnQuery">数据库链接字符串</param>
        /// <param name="dbMappingHandler">数据库字段映射处理接口</param>
        public ClickHouseDbProvider(string dbConnQuery, IDbMappingHandler dbMappingHandler)
        {
            if (string.IsNullOrEmpty(dbConnQuery))
                throw new ArgumentNullException("dbConnQuery");
            if (null == dbMappingHandler)
                throw new ArgumentNullException("dbMappingHandler");

            this._dbConnQuery = dbConnQuery;
            this._dbConnectionStringHandler = this;
            this._dbMappingHandler = dbMappingHandler;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbConnString">数据库连接字符串接口</param>
        /// <param name="dbMappingHandler">数据库字段映射处理接口</param>
        public ClickHouseDbProvider(IDbConnectionString dbConnString, IDbMappingHandler dbMappingHandler)
        {
            if (null == dbConnString)
                throw new ArgumentNullException("dbConnString");
            if (null == dbMappingHandler)
                throw new ArgumentNullException("dbMappingHandler");

            this._dbConnectionStringHandler = dbConnString;
            this._dbMappingHandler = dbMappingHandler;
        }

        #endregion

        #region IDBRepository<M>

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public DbSingleRecord<M> Insert(M model, string suffix = null)
        {
            DbSingleRecord<M> result = new DbSingleRecord<M>();

            string dbString = this._dbConnectionStringHandler.GetConnection();
            if (string.IsNullOrEmpty(dbString))
                throw new Exception("dbString is null");
            Type modelT = typeof(M);

            if (null == model)
            {
                result.AppendError("插入数据时候的Model为空");
                return result;
            }

            // 获取字段映射
            DbColumnAttribute[] columns = this._dbMappingHandler.GetDbColumnCollection(modelT);
            if (null == columns || columns.Length <= 0)
            {
                result.AppendError(string.Format("{0}类型无字段映射", modelT.FullName));
                return result;
            }

            // 找出需要设置的字段
            DbColumnAttribute[] setFields = columns.Where(d => !d.IsDbGenerated).ToArray();
            if (setFields.Length <= 0)
            {
                result.AppendError("插入的表仅有自增长列或没有指定任何列");
                return result;
            }

            #region 拼接Sql语句

            string tableName = this._dbMappingHandler.GetDbTableName(modelT);
            if (!string.IsNullOrEmpty(suffix))
                tableName = $"{tableName}{suffix}";

            StringBuilder sqlBuilder = new StringBuilder("insert into ");
            sqlBuilder.Append(ClickHouseGrammarRule.GenerateTableWrapped(tableName));
            sqlBuilder.Append(" (");

            foreach (var field in setFields.Select(d => $"{ClickHouseGrammarRule.GenerateFieldWrapped(d.DbColumnName)}"))
                sqlBuilder.Append(field);

            sqlBuilder.Replace(",", ")", sqlBuilder.Length - 1, 1);
            sqlBuilder.Append(" values ");
            sqlBuilder.Append("(");
            foreach (var item in setFields)
            {
                string parameterName = string.Format("{0}", item.DbColumnName);
                PropertyInfo p_info = this._dbMappingHandler.GetPropertySingle(modelT, item.DbColumnName);
                object param_val = p_info.GetValue(model, null);
                param_val = ClickHouseGrammarRule.FormatPropertValue(param_val, p_info);
                var dbType = this.GetDbtype(item.DbType);
                var param_str = ClickHouseGrammarRule.GetSqlTextByDbType(param_val, dbType);

                sqlBuilder.Append(param_str);
                sqlBuilder.Append(",");
            }
            sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            sqlBuilder.Append(");");

            //初始化Debug
            result.DebugInit(sqlBuilder, ClickHouseGrammarRule.C_ParamChar, null);

            #endregion

            #region 执行Sql语句

            using (var connection = new ClickHouseConnection(dbString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.Connection = connection;
                    command.CommandText = sqlBuilder.ToString();

                    //尝试打开数据库连结
                    if (this.TryOpenDbConnection(connection, ref result))
                    {
                        try
                        {
                            //  由于其列式存储的特点，通常不返回受影响的行数，即使插入成功也是如此
                            _ = command.ExecuteNonQuery();
                            result.Record = model;
                        }
                        catch (Exception ex)
                        {
                            result.Record = default;
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
        /// 批露插入数据(返回的集合若存在自增长主键,均未赋值)
        /// </summary>
        /// <param name="modelList"></param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public DbCollectionRecord<M> InsertBatch(IEnumerable<M> modelList, string suffix = null)
        {
            DbCollectionRecord<M> result = new DbCollectionRecord<M>();

            if (null == modelList || !modelList.Any())
                return result;

            //获取Db数据库链接字符串
            string dbString = this._dbConnectionStringHandler.GetConnection();
            if (string.IsNullOrEmpty(dbString))
                throw new Exception("dbString is null");

            //获取当前模型的类型和DB类型
            Type modelT = typeof(M);
            DbColumnAttribute[] columns = this._dbMappingHandler.GetDbColumnCollection(modelT);

            //构造内存数据表
            DataTable dt = new DataTable();
            dt.Columns.AddRange(this._dbMappingHandler.GetPropertyCollection(modelT).Select(s => new DataColumn(this._dbMappingHandler.GetDbColumnSingle(modelT, s.Name).DbColumnName, s.PropertyType)).ToArray());

            //开始向虚拟内存表中进行映射
            foreach (var item in modelList)
            {
                DataRow r = dt.NewRow();

                foreach (var col in columns)
                    r[col.DbColumnName] = this._dbMappingHandler.GetPropertySingle(modelT, col.DbColumnName).GetValue(item, null);

                dt.Rows.Add(r);
            }

            // 获取当前表或试图名
            string tableName = this._dbMappingHandler.GetDbTableName(modelT);
            if (!string.IsNullOrEmpty(suffix))
                tableName = $"{tableName}{suffix}";

            // 设置列名
            var col_names = this._dbMappingHandler.GetPropertyCollection(modelT).Select(s => s.Name).ToArray();

            //开始执行
            using (ClickHouseConnection connection = new ClickHouseConnection(dbString))
            {
                using (var bulkCopy = new ClickHouseBulkCopy(connection))
                {
                    // 由于.NET Standard 2.1无法使用init进行初始化复制, 所以需要进行反射赋值
                    bulkCopy.ReflectionSet(tableName, col_names);

                    // 设置批量处理数量
                    bulkCopy.BatchSize = dt.Rows.Count;

                    //尝试打开数据库连结
                    if (this.TryOpenDbConnection(connection, ref result))
                    {
                        // 调用 InitAsync 方法来初始化列名和元数据
                        bulkCopy.InitAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                        bulkCopy.WriteToServerAsync(dt, System.Threading.CancellationToken.None)
                            .ConfigureAwait(false).GetAwaiter().GetResult();

                        result.Record = new List<M>(modelList);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 更新操作（局部更新）
        /// </summary>
        /// <param name="whereExp">需要被更新的条件</param>
        /// <param name="updatePropertys">需要被替换或更新的属性</param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public DbNonRecord Update(Expression<Func<M, bool>> whereExp, Expression<Func<M, M>> updatePropertys, string suffix = null)
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
            if (!string.IsNullOrEmpty(suffix))
                tableName = $"{tableName}{suffix}";

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
                //foreach (var item in whereResult.Parameters)
                //{
                //    cur_parameter = new ClickHouseDbParameter(item.Name, item.Value);
                //    parameters.Add(cur_parameter);
                //}
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
                    if (this.TryOpenDbConnection(connection, ref result))
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
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public DbNonRecord Update(Expression<Func<M, bool>> whereExp, M model, string suffix = null)
        {
            throw new NotImplementedException();

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
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public DbNonRecord UpdateTask(IEnumerable<DbUpdateTaskData<M>> taskList, bool enableSqlTransaction = false, string suffix = null)
        {
            throw new NotImplementedException();

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
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public DbNonRecord Delete(Expression<Func<M, bool>> deleteExp, string suffix = null)
        {
            throw new NotImplementedException();

            //DbNonRecord result = new DbNonRecord();

            //string dbString = this._dbConnectionStringHandler.GetConnection();
            //if (string.IsNullOrEmpty(dbString))
            //    throw new Exception("dbString is null");

            //Type modelT = typeof(M);
            //if (deleteExp != null)
            //{
            //    Expression where_func_lambdaExp;

            //    #region 解析条件语句

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
            //    ClickHouseWhereScriptResult whereResult = ClickHouseWhereScriptHandler.ExecuteResolver(where_func_lambdaExp, this._dbMappingHandler, false);
            //    if (!whereResult.IsAvailable())
            //    {
            //        result.CopyStatus(whereResult);
            //        return result;
            //    }

            //    #endregion

            //    #region 拼接Sql语句

            //    // 获取当前表或试图名
            //    string tableName = this._dbMappingHandler.GetDbTableName(modelT);
            //    if (!string.IsNullOrEmpty(suffix))
            //        tableName = $"{tableName}{suffix}";

            //    List<DbParameter> parameters = new List<DbParameter>();
            //    StringBuilder sqlBuilder = new StringBuilder("delete from ");
            //    sqlBuilder.Append("[");
            //    sqlBuilder.Append(tableName);
            //    sqlBuilder.Append("]");
            //    sqlBuilder.Append(" where ");
            //    sqlBuilder.Append(whereResult.TextScript);
            //    foreach (var item in whereResult.Parameters)
            //    {
            //        DbParameter cur_parameter = new ClickHouseDbParameter(item.Name, item.Value);
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
            //            {
            //                command.Parameters.Add(item);
            //            }
            //            //尝试打开数据库连结
            //            if (this.TryOpenDbConnection(connection, ref result))
            //            {
            //                try
            //                {
            //                    result.AffectedRow = command.ExecuteNonQuery();
            //                }
            //                catch (Exception ex)
            //                {
            //                    result.AppendError("sql语句执行异常," + command.CommandText);
            //                    result.AppendException(ex);

            //                    command.Dispose();
            //                    connection.Close();
            //                    connection.Dispose();

            //                    return result;
            //                }
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

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="exp">查询表达式</param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public DbSingleRecord<M> Fetch(Expression<Func<IDbFetchQueryable<M>, IDbFetchQueryable<M>>> exp, string suffix = null)
        {
            DbSingleRecord<M> result = new DbSingleRecord<M>();

            string dbString = this._dbConnectionStringHandler.GetConnection();
            if (string.IsNullOrEmpty(dbString))
                throw new Exception("dbString is null");

            Type modelT = typeof(M);
            DbColumnAttribute[] columns = this._dbMappingHandler.GetDbColumnCollection(modelT);

            ClickHouseSentenceResult resolveResult = null;
            List<DbParameter> parameters = new List<DbParameter>();

            #region 解析表达式条件

            if (exp != null)
            {
                resolveResult = ClickHouseSentenceHandler.ExecuteResolver(exp, this._dbMappingHandler);
                if (!resolveResult.IsAvailable())
                {
                    result.CopyStatus(resolveResult);
                    return result;
                }
            }

            #endregion

            #region 拼接SQL语句

            // 获取当前表或试图名
            string tableName = this._dbMappingHandler.GetDbTableName(modelT);
            if (!string.IsNullOrEmpty(suffix))
                tableName = $"{tableName}{suffix}";

            StringBuilder sqlBuilder = new StringBuilder("select ");
            if (resolveResult == null)
            {
                string all_cols = string.Join(',', columns.Select(s => $"{ClickHouseGrammarRule.GenerateFieldWrapped(s.DbColumnName)}"));

                sqlBuilder.Append(all_cols);
                sqlBuilder.Append(" from ");
                sqlBuilder.Append(ClickHouseGrammarRule.GenerateTableWrapped(tableName));
            }
            else
            {
                #region 指定需要查询的字段

                if (resolveResult.SqlSelectFields == null || resolveResult.SqlSelectFields.Count() <= 0)
                {
                    //如果没有设置要查询的字段，默认查询所有
                    foreach (var item in columns)
                        resolveResult.SetSelectField(new ClickHouseSelectField
                        {
                            DBFieldAsName = item.DbColumnName,
                            DBSelectFragment = item.DbColumnName,
                            IsModelProperty = true
                        });
                }

                string all_cols = string.Join(',', resolveResult.SqlSelectFields.Select(s => $"{ClickHouseGrammarRule.GenerateFieldWrapped(s.DBSelectFragment)}"));

                sqlBuilder.Append(all_cols);
                sqlBuilder.Append(" from ");
                sqlBuilder.Append(ClickHouseGrammarRule.GenerateTableWrapped(tableName));

                #endregion

                #region 指定Where条件

                if (!string.IsNullOrEmpty(resolveResult.SqlWhereConditionText))
                {
                    sqlBuilder.Append(" where ");
                    sqlBuilder.Append(resolveResult.SqlWhereConditionText);
                }

                #endregion

                #region 指定Order条件

                if (!string.IsNullOrEmpty(resolveResult.SqlOrderConditionText))
                {
                    sqlBuilder.Append(" order by ");
                    sqlBuilder.Append(resolveResult.SqlOrderConditionText);
                }

                #endregion
            }
            sqlBuilder.Append(" limit 1;");

            //初始化Debug
            result.DebugInit(sqlBuilder, ClickHouseGrammarRule.C_ParamChar, parameters.ToArray());

            #endregion

            #region 执行Sql语句

            using (DbConnection connection = new ClickHouseConnection(dbString))
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.Connection = connection;
                    command.CommandText = sqlBuilder.ToString();
                    if (parameters.Count > 0)
                        foreach (var item in parameters)
                            command.Parameters.Add(item);

                    //尝试打开数据库连结
                    if (this.TryOpenDbConnection(connection, ref result))
                    {
                        //尝试执行SQL语句
                        DbDataReader reader = this.TryExecuteReader(command, ref result);
                        if (reader != null && reader.HasRows && reader.Read())
                        {
                            result.Record = this.AutoFillModel(reader, modelT, resolveResult.SqlSelectFields);
                            //释放资源，关闭连结
                            this.DisposeReader(reader);
                        }
                    }
                }
            }

            #endregion

            return result;
        }

        /// <summary>
        /// 获取集合
        /// </summary>
        /// <param name="exp">查询表达式</param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public DbCollectionRecord<M> FetchList(Expression<Func<IDbFetchListQueryable<M>, IDbFetchListQueryable<M>>> exp, string suffix = null)
        {
            throw new NotImplementedException();

            //DbCollectionRecord<M> result = new DbCollectionRecord<M>();

            //string dbString = this._dbConnectionStringHandler.GetConnection();
            //if (string.IsNullOrEmpty(dbString))
            //    throw new Exception("dbString is null");

            //Type modelT = typeof(M);
            //DbColumnAttribute[] columns = this._dbMappingHandler.GetDbColumnCollection(modelT);
            //if (null == columns || columns.Length <= 0)
            //{
            //    result.AppendError(string.Format("类型{0}未映射对上任何字段", modelT.FullName));
            //    return result;
            //}

            //ClickHouseSentenceResult resolveResult = null;
            //List<DbParameter> parameters = new List<DbParameter>();
            //DbParameter cur_parameter = null;
            //int currentPage = 0;
            //int pageSize = 0;

            //#region 解析表达式条件

            //if (exp != null)
            //{
            //    resolveResult = ClickHouseSentenceHandler.ExecuteResolver(exp, this._dbMappingHandler);
            //    if (!resolveResult.IsAvailable())
            //    {
            //        result.CopyStatus(resolveResult);
            //        return result;
            //    }
            //}

            //#endregion

            //#region 拼接SQL语句

            //// 获取当前表或试图名
            //string tableName = this._dbMappingHandler.GetDbTableName(modelT);
            //if (!string.IsNullOrEmpty(suffix))
            //    tableName = $"{tableName}{suffix}";

            //bool count_from_list = false;
            //StringBuilder countBuilder = new StringBuilder();
            //StringBuilder queryBuilder = new StringBuilder("select ");

            //if (resolveResult == null)
            //{
            //    #region 初始化查询分析中间对象

            //    // 初始化保证该不为空（下面查询出数据后封装model会用）
            //    resolveResult = ClickHouseSentenceResult.Create();

            //    // 设置分页参数
            //    currentPage = ClickHouseSentenceResult.DEFAULT_CURRENTPAGE;
            //    pageSize = ClickHouseSentenceResult.DEFAULT_PAGESIZE;
            //    resolveResult.SetPageCondition(currentPage, pageSize);

            //    //设置检索的全字段，默认查询所有
            //    foreach (var item in columns)
            //        resolveResult.SetSelectField(new ClickHouseSelectField
            //        {
            //            DBFieldAsName = item.DbColumnName,
            //            DBSelectFragment = item.DbColumnName,
            //            IsModelProperty = true
            //        });

            //    #endregion

            //    #region 强制设置数据结果集统计从列表中加载

            //    // 指示数据集从list加载
            //    count_from_list = true;

            //    #endregion

            //    #region 拼接构造查询语句(是否需要加载top关键字)

            //    // 已经是默认的分页索引和页码
            //    queryBuilder.Append(" top ");
            //    queryBuilder.Append(pageSize);
            //    queryBuilder.Append(" * from ");
            //    queryBuilder.Append("[");
            //    queryBuilder.Append(tableName);
            //    queryBuilder.Append("]");
            //    queryBuilder.Append(";");

            //    #endregion
            //}
            //else
            //{
            //    #region 读取分页参数

            //    // 参数读取
            //    currentPage = resolveResult.SqlPagerCondition.Key;
            //    pageSize = resolveResult.SqlPagerCondition.Value;

            //    // 检查分页参数的非法与合理性
            //    if (currentPage < 1)
            //        currentPage = ClickHouseSentenceResult.DEFAULT_CURRENTPAGE;
            //    if (pageSize < 1)
            //        pageSize = ClickHouseSentenceResult.DEFAULT_PAGESIZE;

            //    #endregion

            //    #region 特殊情况处理（若currpage=1并且pageSize=int.MaxValue,那么就是查询所有的数据,所以可设置不启动分页）

            //    bool enablePaging = true;
            //    if (1 == currentPage && int.MaxValue == pageSize)
            //        enablePaging = false;

            //    #endregion

            //    #region 启用分页则需要根据条件查询总数量(拼接构造统计语句)

            //    if (enablePaging)
            //    {
            //        countBuilder.Append("select count(1) from ");
            //        countBuilder.Append("[");
            //        countBuilder.Append(tableName);
            //        countBuilder.Append("] ");
            //        if (!string.IsNullOrEmpty(resolveResult.SqlWhereConditionText))
            //        {
            //            countBuilder.Append(" where ");
            //            countBuilder.Append(resolveResult.SqlWhereConditionText);
            //        }
            //        countBuilder.Append(";");
            //    }
            //    else
            //        count_from_list = true; // 指示数据集从list加载

            //    #endregion

            //    #region 拼接构造查询语句

            //    //第一页起始数据
            //    if (currentPage == 1)
            //    {
            //        #region 设置头N条数据

            //        if (pageSize < int.MaxValue)
            //        {
            //            queryBuilder.Append(" top ");
            //            queryBuilder.Append(pageSize);
            //            queryBuilder.Append(" ");
            //        }

            //        #endregion

            //        #region 指定需要查询的字段

            //        if (resolveResult.SqlSelectFields == null || resolveResult.SqlSelectFields.Count() <= 0)
            //        {
            //            //如果没有设置要查询的字段，默认查询所有
            //            foreach (var item in columns)
            //                resolveResult.SetSelectField(new ClickHouseSelectField
            //                {
            //                    DBFieldAsName = item.DbColumnName,
            //                    DBSelectFragment = item.DbColumnName,
            //                    IsModelProperty = true
            //                });
            //        }
            //        foreach (var item in resolveResult.SqlSelectFields)
            //        {
            //            if (item.IsModelProperty)
            //            {
            //                queryBuilder.Append("[");
            //                queryBuilder.Append(item.DBSelectFragment);
            //                queryBuilder.Append("]");
            //                queryBuilder.Append(" as ");
            //                queryBuilder.Append("[");
            //                queryBuilder.Append(item.DBFieldAsName);
            //                queryBuilder.Append("]");
            //                queryBuilder.Append(",");
            //            }
            //        }
            //        queryBuilder.Replace(",", "", queryBuilder.Length - 1, 1);
            //        queryBuilder.Append(" from ");
            //        queryBuilder.Append("[");
            //        queryBuilder.Append(tableName);
            //        queryBuilder.Append("] ");

            //        #endregion

            //        #region 指定Where条件

            //        if (!string.IsNullOrEmpty(resolveResult.SqlWhereConditionText))
            //        {
            //            queryBuilder.Append(" where ");
            //            queryBuilder.Append(resolveResult.SqlWhereConditionText);
            //        }
            //        if (resolveResult.SqlQuerylParameters != null && resolveResult.SqlQuerylParameters.Count() > 0)
            //        {
            //            foreach (var item in resolveResult.SqlQuerylParameters)
            //            {
            //                cur_parameter = new ClickHouseDbParameter(item.Name, item.Value);
            //                parameters.Add(cur_parameter);
            //            }
            //        }

            //        #endregion

            //        #region 指定Order条件

            //        if (!string.IsNullOrEmpty(resolveResult.SqlOrderConditionText))
            //        {
            //            queryBuilder.Append(" order by ");
            //            queryBuilder.Append(resolveResult.SqlOrderConditionText);
            //        }

            //        #endregion
            //    }
            //    //第N页数据
            //    else
            //    {
            //        // 准备开始拼接开窗函数
            //        queryBuilder.Append(" * from (");
            //        queryBuilder.Append("select row_number() over (order by ");

            //        #region 指定排序

            //        if (string.IsNullOrEmpty(resolveResult.SqlOrderConditionText))
            //        {
            //            //设置主键倒序排序
            //            IEnumerable<string> pks = columns.Where(d => d.IsDbPrimaryKey).Select(d => d.DbColumnName);
            //            foreach (var item in pks)
            //            {
            //                queryBuilder.Append("[");
            //                queryBuilder.Append(item);
            //                queryBuilder.Append("]");
            //                queryBuilder.Append(" desc,");
            //            }
            //            queryBuilder.Replace(",", "", queryBuilder.Length - 1, 1);
            //        }
            //        else
            //            queryBuilder.Append(resolveResult.SqlOrderConditionText);

            //        queryBuilder.Append(") as [RowId],");

            //        #endregion

            //        #region 指定查询的字段

            //        if (resolveResult.SqlSelectFields == null || resolveResult.SqlSelectFields.Count() <= 0)
            //        {
            //            //如果没有设置要查询的字段，默认查询所有
            //            foreach (var item in columns)
            //                resolveResult.SetSelectField(new ClickHouseSelectField
            //                {
            //                    DBFieldAsName = item.DbColumnName,
            //                    DBSelectFragment = item.DbColumnName,
            //                    IsModelProperty = true
            //                });
            //        }
            //        foreach (var item in resolveResult.SqlSelectFields)
            //        {
            //            if (item.IsModelProperty)
            //            {
            //                queryBuilder.Append("[");
            //                queryBuilder.Append(item.DBSelectFragment);
            //                queryBuilder.Append("]");
            //                queryBuilder.Append(" as ");
            //                queryBuilder.Append("[");
            //                queryBuilder.Append(item.DBFieldAsName);
            //                queryBuilder.Append("]");
            //                queryBuilder.Append(",");
            //            }
            //        }
            //        queryBuilder.Replace(",", "", queryBuilder.Length - 1, 1);

            //        #endregion

            //        #region 指定查询的表

            //        queryBuilder.Append(" from ");
            //        queryBuilder.Append("[");
            //        queryBuilder.Append(tableName);
            //        queryBuilder.Append("]");

            //        #endregion

            //        #region 指定Where条件

            //        if (!string.IsNullOrEmpty(resolveResult.SqlWhereConditionText))
            //        {
            //            queryBuilder.Append(" where ");
            //            queryBuilder.Append(resolveResult.SqlWhereConditionText);
            //        }
            //        if (resolveResult.SqlQuerylParameters != null && resolveResult.SqlQuerylParameters.Count() > 0)
            //        {
            //            foreach (var item in resolveResult.SqlQuerylParameters)
            //            {
            //                cur_parameter = new ClickHouseDbParameter(item.Name, item.Value);
            //                parameters.Add(cur_parameter);
            //            }
            //        }

            //        #endregion

            //        queryBuilder.Append(") [T1] ");
            //        queryBuilder.Append(" where [RowId]>=");
            //        queryBuilder.Append(((currentPage - 1) * pageSize + 1));
            //        queryBuilder.Append(" and ");
            //        queryBuilder.Append(" [RowId]<= ");
            //        queryBuilder.Append((currentPage * pageSize));
            //        queryBuilder.Append(" order by [RowId] asc");
            //    }
            //    queryBuilder.Append(";");

            //    #endregion
            //}

            ////初始化Debug
            //var debugBuilder = new StringBuilder();
            //if (countBuilder.Length > 0)
            //    debugBuilder.Append(countBuilder);
            //if (queryBuilder.Length > 0)
            //    debugBuilder.Append(queryBuilder);
            //result.DebugInit(debugBuilder, ClickHouseGrammarRule.C_ParamChar, parameters.ToArray());

            //#endregion

            //#region 执行Sql语句

            //using (DbConnection connection = new ClickHouseConnection(dbString))
            //{
            //    using (DbCommand command = new SqlCommand())
            //    {
            //        command.Connection = connection;
            //        if (parameters.Count > 0)
            //            foreach (var item in parameters)
            //                command.Parameters.Add(item);


            //        //尝试打开数据库链接
            //        if (this.TryOpenDbConnection(connection, ref result))
            //        {
            //            //尝试执行语句返回第一行第一列
            //            if (countBuilder.Length > 0)
            //            {
            //                command.CommandText = countBuilder.ToString();
            //                result.TotalCount = Convert.ToInt32(command.ExecuteScalar());

            //                count_from_list = false;
            //            }
            //            else
            //                count_from_list = true;

            //            //如果存在符合条件的数据则进行二次查询，否则跳出
            //            if (count_from_list || result.TotalCount > 0)
            //            {
            //                result.CurrentPage = currentPage;
            //                result.PageSize = pageSize;

            //                //尝试执行语句返回DataReader
            //                command.CommandText = queryBuilder.ToString();
            //                DbDataReader reader = this.TryExecuteReader(command, ref result);
            //                if (reader != null && reader.HasRows)
            //                {
            //                    result.Record = new List<M>();
            //                    M entity = default;
            //                    while (reader.Read())
            //                    {
            //                        entity = this.AutoFillModel(reader, modelT, resolveResult.SqlSelectFields);
            //                        result.Record.Add(entity);
            //                    }
            //                    //释放资源，关闭连结
            //                    this.DisposeReader(reader);
            //                }

            //                // 查询符合条件的数据赋值
            //                if (count_from_list)
            //                    result.TotalCount = null == result.Record ? 0 : result.Record.Count;
            //            }
            //        }
            //    }
            //}

            //#endregion

            //return result;
        }

        /// <summary>
        /// 执行计算 Count, SUM，MAX,MIN等
        /// </summary>
        /// <param name="exp">查询表达式</param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public DbCalculateRecord Calculate(Expression<Func<IDbCalculateQueryable<M>, IDbCalculateQueryable<M>>> exp, string suffix = null)
        {
            throw new NotImplementedException();

            //DbCalculateRecord result = new DbCalculateRecord();

            //string dbString = this._dbConnectionStringHandler.GetConnection();
            //if (string.IsNullOrEmpty(dbString))
            //    throw new Exception("dbString is null");

            //Type modelT = typeof(M);
            //ClickHouseSentenceResult resolveResult = null;
            //List<DbParameter> parameters = new List<DbParameter>();

            //#region 解析表达式条件

            //if (exp != null)
            //{
            //    resolveResult = ClickHouseSentenceHandler.ExecuteResolver(exp, this._dbMappingHandler);
            //    if (!resolveResult.IsAvailable())
            //    {
            //        result.CopyStatus(resolveResult);
            //        return result;
            //    }
            //}
            //else
            //{
            //    result.AppendError("表达式exp不允许为空");
            //    return result;
            //}

            //#endregion

            //// 获取当前表或试图名
            //string tableName = this._dbMappingHandler.GetDbTableName(modelT);
            //if (!string.IsNullOrEmpty(suffix))
            //    tableName = $"{tableName}{suffix}";

            //StringBuilder sqlBuilder = new StringBuilder("select ");
            //if (resolveResult.SqlSelectFields != null && resolveResult.SqlSelectFields.Count() > 0)
            //{
            //    #region 拼接Sql语句

            //    foreach (var item in resolveResult.SqlSelectFields.OrderBy(d => d.IsModelProperty).OrderBy(d => d.DBFieldAsName))
            //    {
            //        sqlBuilder.Append(item.DBSelectFragment);
            //        sqlBuilder.Append(" as ");
            //        sqlBuilder.Append("[");
            //        sqlBuilder.Append(item.DBFieldAsName);
            //        sqlBuilder.Append("],");
            //    }
            //    sqlBuilder.Replace(",", "", sqlBuilder.Length - 1, 1);
            //    sqlBuilder.Append(" from ");
            //    sqlBuilder.Append("[");
            //    sqlBuilder.Append(tableName);
            //    sqlBuilder.Append("] ");
            //    if (!string.IsNullOrEmpty(resolveResult.SqlWhereConditionText))
            //    {
            //        sqlBuilder.Append(" where ");
            //        sqlBuilder.Append(resolveResult.SqlWhereConditionText);
            //    }
            //    if (resolveResult.SqlQuerylParameters != null && resolveResult.SqlQuerylParameters.Count() > 0)
            //        foreach (var item in resolveResult.SqlQuerylParameters)
            //            parameters.Add(new ClickHouseDbParameter(item.Name, item.Value));

            //    if (!string.IsNullOrEmpty(resolveResult.SqlGroupConditionBuilder))
            //    {
            //        sqlBuilder.Append(" group by ");
            //        sqlBuilder.Append(resolveResult.SqlGroupConditionBuilder);
            //    }
            //    if (!string.IsNullOrEmpty(resolveResult.SqlOrderConditionText))
            //    {
            //        sqlBuilder.Append(" order by ");
            //        sqlBuilder.Append(resolveResult.SqlOrderConditionText);
            //    }
            //    sqlBuilder.Append(";");

            //    //初始化Debug
            //    result.DebugInit(sqlBuilder, ClickHouseGrammarRule.C_ParamChar, parameters.ToArray());

            //    #endregion

            //    #region 执行Sql语句

            //    using (DbConnection connection = new ClickHouseConnection(dbString))
            //    {
            //        using (DbCommand command = new SqlCommand())
            //        {
            //            command.Connection = connection;
            //            command.CommandText = sqlBuilder.ToString();
            //            if (parameters.Count > 0)
            //                foreach (var item in parameters)
            //                    command.Parameters.Add(item);

            //            //尝试打开数据库链接
            //            if (this.TryOpenDbConnection(connection, ref result))
            //            {
            //                //尝试执行语句返回DataReader
            //                DbDataReader reader = this.TryExecuteReader(command, ref result);
            //                if (reader != null && reader.HasRows)
            //                {
            //                    List<DbRowRecord> rowDataList = new List<DbRowRecord>();//设置所有的行数据容器
            //                    DbRowRecord rowItem = null;//设置行数据对象
            //                    DbColumnRecord columnItem = null;//列数据对象
            //                    while (reader.Read())
            //                    {
            //                        rowItem = new DbRowRecord();

            //                        //开始遍历所有的列数据
            //                        foreach (var item in resolveResult.SqlSelectFields)
            //                        {
            //                            object objVal = reader[item.DBFieldAsName];
            //                            if (objVal != null && objVal != DBNull.Value)
            //                            {
            //                                columnItem = new DbColumnRecord
            //                                {
            //                                    Name = item.DBFieldAsName,
            //                                    Value = objVal
            //                                };

            //                                //在行数据对象中装载列数据
            //                                rowItem.Add(columnItem);
            //                            }
            //                        }
            //                        rowDataList.Add(rowItem);
            //                    }
            //                    result.Record = rowDataList;

            //                    //释放资源，关闭连结
            //                    this.DisposeReader(reader);
            //                }
            //            }
            //        }
            //    }

            //    #endregion

            //    return result;
            //}
            //else
            //{
            //    #region 必须至少指定一个运算模式，例如:Count,Sum,Max,Min等

            //    result.AppendError("必须至少指定一个运算模式，例如:Count,Sum,Max,Min等");
            //    return result;

            //    #endregion
            //}
        }

        #endregion

        #region IDbAsyncProvider<M>

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public async Task<DbSingleRecord<M>> InsertAsync(M model, string suffix = null)
        {
            DbSingleRecord<M> result = new DbSingleRecord<M>();

            string dbString = this._dbConnectionStringHandler.GetConnection();
            if (string.IsNullOrEmpty(dbString))
                throw new Exception("dbString is null");
            Type modelT = typeof(M);

            if (null == model)
            {
                result.AppendError("插入数据时候的Model为空");
                return result;
            }

            // 获取字段映射
            DbColumnAttribute[] columns = this._dbMappingHandler.GetDbColumnCollection(modelT);
            if (null == columns || columns.Length <= 0)
            {
                result.AppendError(string.Format("{0}类型无字段映射", modelT.FullName));
                return result;
            }

            // 找出需要设置的字段
            DbColumnAttribute[] setFields = columns.Where(d => !d.IsDbGenerated).ToArray();
            if (setFields.Length <= 0)
            {
                result.AppendError("插入的表仅有自增长列或没有指定任何列");
                return result;
            }

            #region 拼接Sql语句

            string tableName = this._dbMappingHandler.GetDbTableName(modelT);
            if (!string.IsNullOrEmpty(suffix))
                tableName = $"{tableName}{suffix}";

            StringBuilder sqlBuilder = new StringBuilder("insert into ");
            sqlBuilder.Append(ClickHouseGrammarRule.GenerateTableWrapped(tableName));
            sqlBuilder.Append(" (");

            foreach (var field in setFields.Select(d => $"{ClickHouseGrammarRule.GenerateFieldWrapped(d.DbColumnName)}"))
                sqlBuilder.Append(field);

            sqlBuilder.Replace(",", ")", sqlBuilder.Length - 1, 1);
            sqlBuilder.Append(" values ");
            sqlBuilder.Append("(");
            foreach (var item in setFields)
            {
                string parameterName = string.Format("{0}", item.DbColumnName);
                PropertyInfo p_info = this._dbMappingHandler.GetPropertySingle(modelT, item.DbColumnName);
                object param_val = p_info.GetValue(model, null);
                param_val = ClickHouseGrammarRule.FormatPropertValue(param_val, p_info);
                var dbType = this.GetDbtype(item.DbType);
                var param_str = ClickHouseGrammarRule.GetSqlTextByDbType(param_val, dbType);

                sqlBuilder.Append(param_str);
                sqlBuilder.Append(",");
            }
            sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            sqlBuilder.Append(");");

            //初始化Debug
            result.DebugInit(sqlBuilder, ClickHouseGrammarRule.C_ParamChar, null);

            #endregion

            #region 执行Sql语句

            using (DbConnection connection = new ClickHouseConnection(dbString))
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.Connection = connection;
                    command.CommandText = sqlBuilder.ToString();

                    // 尝试打开数据库链接
                    try
                    {
                        await connection.OpenAsync();
                    }
                    catch (Exception ex)
                    {
                        result.AppendException(ex);

                        await command.DisposeAsync();
                        await connection.CloseAsync();
                        await connection.DisposeAsync();

                        return result;
                    }

                    try
                    {
                        _ = await command.ExecuteNonQueryAsync();
                        result.Record = model;
                    }
                    catch (Exception ex)
                    {
                        result.AppendException(ex);

                        await command.DisposeAsync();
                        await connection.CloseAsync();
                        await connection.DisposeAsync();

                        return result;
                    }
                }
            }

            #endregion

            return result;
        }

        /// <summary>
        /// 批露插入数据异步处理(返回的集合若存在自增长主键,均未赋值)
        /// </summary>
        /// <param name="modelList"></param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public async Task<DbCollectionRecord<M>> InsertBatchAsync(IEnumerable<M> modelList, string suffix = null)
        {
            DbCollectionRecord<M> result = new DbCollectionRecord<M>();

            if (null == modelList || !modelList.Any())
                return result;

            //获取Db数据库链接字符串
            string dbString = this._dbConnectionStringHandler.GetConnection();
            if (string.IsNullOrEmpty(dbString))
                throw new Exception("dbString is null");

            //获取当前模型的类型和DB类型
            Type modelT = typeof(M);
            DbColumnAttribute[] columns = this._dbMappingHandler.GetDbColumnCollection(modelT);

            //构造内存数据表
            DataTable dt = new DataTable();
            dt.Columns.AddRange(this._dbMappingHandler.GetPropertyCollection(modelT).Select(s => new DataColumn(this._dbMappingHandler.GetDbColumnSingle(modelT, s.Name).DbColumnName, s.PropertyType)).ToArray());

            //开始向虚拟内存表中进行映射
            foreach (var item in modelList)
            {
                DataRow r = dt.NewRow();

                foreach (var col in columns)
                    r[col.DbColumnName] = this._dbMappingHandler.GetPropertySingle(modelT, col.DbColumnName).GetValue(item, null);

                dt.Rows.Add(r);
            }

            // 获取当前表或试图名
            string tableName = this._dbMappingHandler.GetDbTableName(modelT);
            if (!string.IsNullOrEmpty(suffix))
                tableName = $"{tableName}{suffix}";

            // 设置列名
            var col_names = this._dbMappingHandler.GetPropertyCollection(modelT).Select(s => s.Name).ToArray();

            //开始执行
            using (ClickHouseConnection connection = new ClickHouseConnection(dbString))
            {
                using (var bulkCopy = new ClickHouseBulkCopy(connection))
                {
                    // 由于.NET Standard 2.1无法使用init进行初始化复制, 所以需要进行反射赋值
                    bulkCopy.ReflectionSet(tableName, col_names);

                    // 设置批量处理数量
                    bulkCopy.BatchSize = dt.Rows.Count;

                    // 尝试打开数据库链接
                    try
                    {
                        await connection.OpenAsync();
                    }
                    catch (Exception ex)
                    {
                        result.AppendException(ex);

                        bulkCopy.Dispose();
                        await connection.CloseAsync();
                        await connection.DisposeAsync();

                        return result;
                    }

                    try
                    {
                        // 调用 InitAsync 方法来初始化列名和元数据
                        await bulkCopy.InitAsync();
                        await bulkCopy.WriteToServerAsync(dt, System.Threading.CancellationToken.None);

                        result.Record = new List<M>(modelList);
                    }
                    catch (Exception ex)
                    {
                        result.AppendException(ex);

                        bulkCopy.Dispose();
                        await connection.CloseAsync();
                        await connection.DisposeAsync();

                        return result;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 更新操作（局部更新）
        /// </summary>
        /// <param name="whereExp">需要被更新的条件</param>
        /// <param name="updatePropertys">需要被替换或更新的属性</param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public Task<DbNonRecord> UpdateAsync(Expression<Func<M, bool>> whereExp, Expression<Func<M, M>> updatePropertys, string suffix = null)
        {
            throw new NotImplementedException();

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
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public Task<DbNonRecord> UpdateAsync(Expression<Func<M, bool>> whereExp, M model, string suffix = null)
        {
            throw new NotImplementedException();

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
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public Task<DbNonRecord> UpdateTaskAsync(IEnumerable<DbUpdateTaskData<M>> taskList, bool enableSqlTransaction = false, string suffix = null)
        {
            throw new NotImplementedException();

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
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public Task<DbNonRecord> DeleteAsync(Expression<Func<M, bool>> deleteExp, string suffix = null)
        {
            throw new NotImplementedException();

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

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="exp">查询表达式</param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public Task<DbSingleRecord<M>> FetchAsync(Expression<Func<IDbFetchQueryable<M>, IDbFetchQueryable<M>>> exp, string suffix = null)
        {
            throw new NotImplementedException();

            //DbSingleRecord<M> result = new DbSingleRecord<M>();

            //string dbString = this._dbConnectionStringHandler.GetConnection();
            //if (string.IsNullOrEmpty(dbString))
            //    throw new Exception("dbString is null");

            //Type modelT = typeof(M);
            //ClickHouseSentenceResult resolveResult = null;
            //List<DbParameter> parameters = new List<DbParameter>();

            //#region 解析表达式条件

            //if (exp != null)
            //{
            //    resolveResult = ClickHouseSentenceHandler.ExecuteResolver(exp, this._dbMappingHandler);
            //    if (!resolveResult.IsAvailable())
            //    {
            //        result.CopyStatus(resolveResult);
            //        return result;
            //    }
            //}

            //#endregion

            //#region 拼接SQL语句

            //string tableName = this._dbMappingHandler.GetDbTableName(modelT);
            //if (!string.IsNullOrEmpty(suffix))
            //    tableName = $"{tableName}{suffix}";

            //StringBuilder sqlBuilder = new StringBuilder("select top 1 ");
            //if (resolveResult == null)
            //{
            //    sqlBuilder.Append(" * from ");
            //    sqlBuilder.Append("[");
            //    sqlBuilder.Append(tableName);
            //    sqlBuilder.Append("]");
            //}
            //else
            //{
            //    #region 指定需要查询的字段

            //    if (resolveResult.SqlSelectFields == null || resolveResult.SqlSelectFields.Count() <= 0)
            //    {
            //        //如果没有设置要查询的字段，默认查询所有
            //        DbColumnAttribute[] fields = this._dbMappingHandler.GetDbColumnCollection(modelT);

            //        foreach (var item in fields)
            //            resolveResult.SetSelectField(new ClickHouseSelectField
            //            {
            //                DBFieldAsName = item.DbColumnName,
            //                DBSelectFragment = item.DbColumnName,
            //                IsModelProperty = true
            //            });
            //    }
            //    foreach (var item in resolveResult.SqlSelectFields)
            //    {
            //        if (item.IsModelProperty)
            //        {
            //            sqlBuilder.Append("[");
            //            sqlBuilder.Append(item.DBSelectFragment);
            //            sqlBuilder.Append("]");
            //            sqlBuilder.Append(" as ");
            //            sqlBuilder.Append("[");
            //            sqlBuilder.Append(item.DBFieldAsName);
            //            sqlBuilder.Append("]");
            //            sqlBuilder.Append(",");
            //        }
            //    }
            //    sqlBuilder.Replace(",", "", sqlBuilder.Length - 1, 1);
            //    sqlBuilder.Append(" from ");
            //    sqlBuilder.Append("[");
            //    sqlBuilder.Append(tableName);
            //    sqlBuilder.Append("] ");

            //    #endregion

            //    #region 指定Where条件

            //    if (!string.IsNullOrEmpty(resolveResult.SqlWhereConditionText))
            //    {
            //        sqlBuilder.Append(" where ");
            //        sqlBuilder.Append(resolveResult.SqlWhereConditionText);
            //    }

            //    //装载参数
            //    if (resolveResult.SqlQuerylParameters != null && resolveResult.SqlQuerylParameters.Count() > 0)
            //        foreach (var item in resolveResult.SqlQuerylParameters)
            //            parameters.Add(new ClickHouseDbParameter(item.Name, item.Value));

            //    #endregion

            //    #region 指定Order条件

            //    if (!string.IsNullOrEmpty(resolveResult.SqlOrderConditionText))
            //    {
            //        sqlBuilder.Append(" order by ");
            //        sqlBuilder.Append(resolveResult.SqlOrderConditionText);
            //    }

            //    #endregion
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
            //        if (parameters.Count > 0)
            //            foreach (var item in parameters)
            //                command.Parameters.Add(item);

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

            //        //尝试执行SQL语句
            //        DbDataReader reader = null;
            //        try
            //        {
            //            reader = await command.ExecuteReaderAsync();
            //        }
            //        catch (Exception ex)
            //        {
            //            result.AppendError("sql语句执行错误，" + command.CommandText);
            //            result.AppendException(ex);

            //            await command.DisposeAsync();
            //            await connection.CloseAsync();
            //            await connection.DisposeAsync();

            //            return result;
            //        }
            //        if (reader != null && reader.HasRows && await reader.ReadAsync())
            //        {
            //            result.Record = this.AutoFillModel(reader, modelT, resolveResult.SqlSelectFields);

            //            //释放资源，关闭连结
            //            await reader.CloseAsync();
            //            await reader.DisposeAsync();
            //        }
            //    }
            //}

            //#endregion

            //return result;
        }

        /// <summary>
        /// 获取集合
        /// </summary>
        /// <param name="exp">查询表达式</param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public Task<DbCollectionRecord<M>> FetchListAsync(Expression<Func<IDbFetchListQueryable<M>, IDbFetchListQueryable<M>>> exp, string suffix = null)
        {
            throw new NotImplementedException();

            //DbCollectionRecord<M> result = new DbCollectionRecord<M>();

            //string dbString = this._dbConnectionStringHandler.GetConnection();
            //if (string.IsNullOrEmpty(dbString))
            //    throw new Exception("dbString is null");

            //Type modelT = typeof(M);
            //DbColumnAttribute[] columns = this._dbMappingHandler.GetDbColumnCollection(modelT);
            //if (null == columns || columns.Length <= 0)
            //{
            //    result.AppendError(string.Format("类型{0}未映射对上任何字段", modelT.FullName));
            //    return result;
            //}

            //ClickHouseSentenceResult resolveResult = null;
            //List<DbParameter> parameters = new List<DbParameter>();
            //int currentPage = 0;
            //int pageSize = 0;

            //#region 解析表达式条件

            //if (exp != null)
            //{
            //    resolveResult = ClickHouseSentenceHandler.ExecuteResolver(exp, this._dbMappingHandler);
            //    if (!resolveResult.IsAvailable())
            //    {
            //        result.CopyStatus(resolveResult);
            //        return result;
            //    }
            //}

            //#endregion

            //#region 拼接SQL语句

            //string tableName = this._dbMappingHandler.GetDbTableName(modelT);
            //if (!string.IsNullOrEmpty(suffix))
            //    tableName = $"{tableName}{suffix}";

            //bool count_from_list = false;
            //StringBuilder countBuilder = new StringBuilder();
            //StringBuilder queryBuilder = new StringBuilder("select ");

            //if (resolveResult == null)
            //{
            //    #region 初始化查询分析中间对象

            //    // 初始化保证该不为空（下面查询出数据后封装model会用）
            //    resolveResult = ClickHouseSentenceResult.Create();

            //    // 设置分页参数
            //    currentPage = ClickHouseSentenceResult.DEFAULT_CURRENTPAGE;
            //    pageSize = ClickHouseSentenceResult.DEFAULT_PAGESIZE;
            //    resolveResult.SetPageCondition(currentPage, pageSize);

            //    //设置检索的全字段，默认查询所有
            //    foreach (var item in columns)
            //        resolveResult.SetSelectField(new ClickHouseSelectField
            //        {
            //            DBFieldAsName = item.DbColumnName,
            //            DBSelectFragment = item.DbColumnName,
            //            IsModelProperty = true
            //        });

            //    #endregion

            //    #region 强制设置数据结果集统计从列表中加载

            //    // 指示数据集从list加载
            //    count_from_list = true;

            //    #endregion

            //    #region 拼接构造查询语句(是否需要加载top关键字)

            //    // 已经是默认的分页索引和页码
            //    queryBuilder.Append(" top ");
            //    queryBuilder.Append(pageSize);
            //    queryBuilder.Append(" * from ");
            //    queryBuilder.Append("[");
            //    queryBuilder.Append(tableName);
            //    queryBuilder.Append("]");
            //    queryBuilder.Append(";");

            //    #endregion
            //}
            //else
            //{
            //    #region 读取分页参数

            //    // 参数读取
            //    currentPage = resolveResult.SqlPagerCondition.Key;
            //    pageSize = resolveResult.SqlPagerCondition.Value;

            //    // 检查分页参数的非法与合理性
            //    if (currentPage < 1)
            //        currentPage = ClickHouseSentenceResult.DEFAULT_CURRENTPAGE;
            //    if (pageSize < 1)
            //        pageSize = ClickHouseSentenceResult.DEFAULT_PAGESIZE;

            //    #endregion

            //    #region 特殊情况处理（若currpage=1并且pageSize=int.MaxValue,那么就是查询所有的数据,所以可设置不启动分页）

            //    bool enablePaging = true;
            //    if (1 == currentPage && int.MaxValue == pageSize)
            //        enablePaging = false;

            //    #endregion

            //    #region 启用分页则需要根据条件查询总数量(拼接构造统计语句)

            //    if (enablePaging)
            //    {
            //        countBuilder.Append("select count(1) from ");
            //        countBuilder.Append("[");
            //        countBuilder.Append(tableName);
            //        countBuilder.Append("] ");
            //        if (!string.IsNullOrEmpty(resolveResult.SqlWhereConditionText))
            //        {
            //            countBuilder.Append(" where ");
            //            countBuilder.Append(resolveResult.SqlWhereConditionText);
            //        }
            //        countBuilder.Append(";");
            //    }
            //    else
            //        count_from_list = true; // 指示数据集从list加载

            //    #endregion

            //    #region 拼接构造查询语句

            //    //第一页起始数据
            //    if (currentPage == 1)
            //    {
            //        #region 设置头N条数据

            //        if (pageSize < int.MaxValue)
            //        {
            //            queryBuilder.Append(" top ");
            //            queryBuilder.Append(pageSize);
            //            queryBuilder.Append(" ");
            //        }

            //        #endregion

            //        #region 指定需要查询的字段

            //        if (resolveResult.SqlSelectFields == null || resolveResult.SqlSelectFields.Count() <= 0)
            //        {
            //            //如果没有设置要查询的字段，默认查询所有
            //            foreach (var item in columns)
            //                resolveResult.SetSelectField(new ClickHouseSelectField
            //                {
            //                    DBFieldAsName = item.DbColumnName,
            //                    DBSelectFragment = item.DbColumnName,
            //                    IsModelProperty = true
            //                });
            //        }
            //        foreach (var item in resolveResult.SqlSelectFields)
            //        {
            //            if (item.IsModelProperty)
            //            {
            //                queryBuilder.Append("[");
            //                queryBuilder.Append(item.DBSelectFragment);
            //                queryBuilder.Append("]");
            //                queryBuilder.Append(" as ");
            //                queryBuilder.Append("[");
            //                queryBuilder.Append(item.DBFieldAsName);
            //                queryBuilder.Append("]");
            //                queryBuilder.Append(",");
            //            }
            //        }
            //        queryBuilder.Replace(",", "", queryBuilder.Length - 1, 1);
            //        queryBuilder.Append(" from ");
            //        queryBuilder.Append("[");
            //        queryBuilder.Append(tableName);
            //        queryBuilder.Append("] ");

            //        #endregion

            //        #region 指定Where条件

            //        if (!string.IsNullOrEmpty(resolveResult.SqlWhereConditionText))
            //        {
            //            queryBuilder.Append(" where ");
            //            queryBuilder.Append(resolveResult.SqlWhereConditionText);
            //        }
            //        if (resolveResult.SqlQuerylParameters != null && resolveResult.SqlQuerylParameters.Count() > 0)
            //            foreach (var item in resolveResult.SqlQuerylParameters)
            //                parameters.Add(new ClickHouseDbParameter(item.Name, item.Value));

            //        #endregion

            //        #region 指定Order条件

            //        if (!string.IsNullOrEmpty(resolveResult.SqlOrderConditionText))
            //        {
            //            queryBuilder.Append(" order by ");
            //            queryBuilder.Append(resolveResult.SqlOrderConditionText);
            //        }

            //        #endregion
            //    }
            //    //第N页数据
            //    else
            //    {
            //        // 准备开始拼接开窗函数
            //        queryBuilder.Append(" * from (");
            //        queryBuilder.Append("select row_number() over (order by ");

            //        #region 指定排序

            //        if (string.IsNullOrEmpty(resolveResult.SqlOrderConditionText))
            //        {
            //            //设置主键倒序排序
            //            IEnumerable<string> pks = columns.Where(d => d.IsDbPrimaryKey).Select(d => d.DbColumnName);
            //            foreach (var item in pks)
            //            {
            //                queryBuilder.Append("[");
            //                queryBuilder.Append(item);
            //                queryBuilder.Append("]");
            //                queryBuilder.Append(" desc,");
            //            }
            //            queryBuilder.Replace(",", string.Empty, queryBuilder.Length - 1, 1);
            //        }
            //        else
            //            queryBuilder.Append(resolveResult.SqlOrderConditionText);

            //        queryBuilder.Append(") as [RowId],");

            //        #endregion

            //        #region 指定查询的字段

            //        if (resolveResult.SqlSelectFields == null || resolveResult.SqlSelectFields.Count() <= 0)
            //        {
            //            //如果没有设置要查询的字段，默认查询所有
            //            foreach (var item in columns)
            //                resolveResult.SetSelectField(new ClickHouseSelectField
            //                {
            //                    DBFieldAsName = item.DbColumnName,
            //                    DBSelectFragment = item.DbColumnName,
            //                    IsModelProperty = true
            //                });
            //        }
            //        foreach (var item in resolveResult.SqlSelectFields)
            //        {
            //            if (item.IsModelProperty)
            //            {
            //                queryBuilder.Append("[");
            //                queryBuilder.Append(item.DBSelectFragment);
            //                queryBuilder.Append("]");
            //                queryBuilder.Append(" as ");
            //                queryBuilder.Append("[");
            //                queryBuilder.Append(item.DBFieldAsName);
            //                queryBuilder.Append("]");
            //                queryBuilder.Append(",");
            //            }
            //        }
            //        queryBuilder.Replace(",", "", queryBuilder.Length - 1, 1);

            //        #endregion

            //        #region 指定查询的表

            //        queryBuilder.Append(" from ");
            //        queryBuilder.Append("[");
            //        queryBuilder.Append(tableName);
            //        queryBuilder.Append("]");

            //        #endregion

            //        #region 指定Where条件

            //        if (!string.IsNullOrEmpty(resolveResult.SqlWhereConditionText))
            //        {
            //            queryBuilder.Append(" where ");
            //            queryBuilder.Append(resolveResult.SqlWhereConditionText);
            //        }
            //        if (resolveResult.SqlQuerylParameters != null && resolveResult.SqlQuerylParameters.Count() > 0)
            //            foreach (var item in resolveResult.SqlQuerylParameters)
            //                parameters.Add(new ClickHouseDbParameter(item.Name, item.Value));

            //        #endregion

            //        queryBuilder.Append(") [T1] ");
            //        queryBuilder.Append(" where [RowId]>=");
            //        queryBuilder.Append(((currentPage - 1) * pageSize + 1));
            //        queryBuilder.Append(" and ");
            //        queryBuilder.Append(" [RowId]<= ");
            //        queryBuilder.Append((currentPage * pageSize));
            //        queryBuilder.Append(" order by [RowId] asc");
            //    }
            //    queryBuilder.Append(";");

            //    #endregion
            //}

            ////初始化Debug
            //var debugBuilder = new StringBuilder();
            //if (countBuilder.Length > 0)
            //    debugBuilder.Append(countBuilder);
            //if (queryBuilder.Length > 0)
            //    debugBuilder.Append(queryBuilder);
            //result.DebugInit(debugBuilder, ClickHouseGrammarRule.C_ParamChar, parameters.ToArray());

            //#endregion

            //#region 执行Sql语句

            //using (DbConnection connection = new ClickHouseConnection(dbString))
            //{
            //    using (DbCommand command = new SqlCommand())
            //    {
            //        command.Connection = connection;
            //        if (parameters.Count > 0)
            //            foreach (var item in parameters)
            //                command.Parameters.Add(item);

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

            //        // 判断是否需要执行统计语句
            //        if (countBuilder.Length > 0)
            //        {
            //            //尝试执行语句返回第一行第一列
            //            command.CommandText = countBuilder.ToString();
            //            result.TotalCount = Convert.ToInt32(await command.ExecuteScalarAsync());

            //            count_from_list = false;
            //        }
            //        else
            //            count_from_list = true;

            //        //如果存在符合条件的数据则进行二次查询，否则跳出
            //        if (count_from_list || result.TotalCount > 0)
            //        {
            //            result.CurrentPage = currentPage;
            //            result.PageSize = pageSize;

            //            //尝试执行语句返回DataReader
            //            command.CommandText = queryBuilder.ToString();

            //            DbDataReader reader = null;
            //            try
            //            {
            //                reader = await command.ExecuteReaderAsync();
            //            }
            //            catch (Exception ex)
            //            {
            //                result.AppendError("sql语句执行错误，" + command.CommandText);
            //                result.AppendException(ex);

            //                await command.DisposeAsync();
            //                await connection.CloseAsync();
            //                await connection.DisposeAsync();

            //                return result;
            //            }
            //            if (reader != null && reader.HasRows)
            //            {
            //                result.Record = new List<M>();
            //                M entity = default;
            //                while (await reader.ReadAsync())
            //                {
            //                    entity = this.AutoFillModel(reader, modelT, resolveResult.SqlSelectFields);
            //                    result.Record.Add(entity);
            //                }

            //                //释放资源，关闭连结
            //                await reader.CloseAsync();
            //                await reader.DisposeAsync();
            //            }

            //            // 查询符合条件的数据赋值
            //            if (count_from_list)
            //                result.TotalCount = null == result.Record ? 0 : result.Record.Count;
            //        }
            //    }
            //}

            //#endregion

            //return result;
        }

        /// <summary>
        /// 执行计算 Count, SUM，MAX,MIN等
        /// </summary>
        /// <param name="exp">查询表达式</param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public Task<DbCalculateRecord> CalculateAsync(Expression<Func<IDbCalculateQueryable<M>, IDbCalculateQueryable<M>>> exp, string suffix = null)
        {
            throw new NotImplementedException();

            //DbCalculateRecord result = new DbCalculateRecord();

            //string dbString = this._dbConnectionStringHandler.GetConnection();
            //if (string.IsNullOrEmpty(dbString))
            //    throw new Exception("dbString is null");

            //Type modelT = typeof(M);
            //ClickHouseSentenceResult resolveResult = null;
            //List<DbParameter> parameters = new List<DbParameter>();

            //#region 解析表达式条件

            //if (exp != null)
            //{
            //    resolveResult = ClickHouseSentenceHandler.ExecuteResolver(exp, this._dbMappingHandler);
            //    if (!resolveResult.IsAvailable())
            //    {
            //        result.CopyStatus(resolveResult);
            //        return result;
            //    }
            //}
            //else
            //{
            //    result.AppendError("表达式exp不允许为空");
            //    return result;
            //}

            //#endregion

            //StringBuilder sqlBuilder = new StringBuilder("select ");
            //if (resolveResult.SqlSelectFields != null && resolveResult.SqlSelectFields.Count() > 0)
            //{
            //    #region 拼接Sql语句

            //    string tableName = this._dbMappingHandler.GetDbTableName(modelT);
            //    if (!string.IsNullOrEmpty(suffix))
            //        tableName = $"{tableName}{suffix}";

            //    foreach (var item in resolveResult.SqlSelectFields.OrderBy(d => d.IsModelProperty).OrderBy(d => d.DBFieldAsName))
            //    {
            //        sqlBuilder.Append(item.DBSelectFragment);
            //        sqlBuilder.Append(" as ");
            //        sqlBuilder.Append("[");
            //        sqlBuilder.Append(item.DBFieldAsName);
            //        sqlBuilder.Append("],");
            //    }
            //    sqlBuilder.Replace(",", "", sqlBuilder.Length - 1, 1);
            //    sqlBuilder.Append(" from ");
            //    sqlBuilder.Append("[");
            //    sqlBuilder.Append(tableName);
            //    sqlBuilder.Append("] ");
            //    if (!string.IsNullOrEmpty(resolveResult.SqlWhereConditionText))
            //    {
            //        sqlBuilder.Append(" where ");
            //        sqlBuilder.Append(resolveResult.SqlWhereConditionText);
            //    }
            //    if (resolveResult.SqlQuerylParameters != null && resolveResult.SqlQuerylParameters.Count() > 0)
            //        foreach (var item in resolveResult.SqlQuerylParameters)
            //            parameters.Add(new ClickHouseDbParameter(item.Name, item.Value));

            //    if (!string.IsNullOrEmpty(resolveResult.SqlGroupConditionBuilder))
            //    {
            //        sqlBuilder.Append(" group by ");
            //        sqlBuilder.Append(resolveResult.SqlGroupConditionBuilder);
            //    }
            //    if (!string.IsNullOrEmpty(resolveResult.SqlOrderConditionText))
            //    {
            //        sqlBuilder.Append(" order by ");
            //        sqlBuilder.Append(resolveResult.SqlOrderConditionText);
            //    }
            //    sqlBuilder.Append(";");

            //    //初始化Debug
            //    result.DebugInit(sqlBuilder, ClickHouseGrammarRule.C_ParamChar, parameters.ToArray());

            //    #endregion

            //    #region 执行Sql语句

            //    using (DbConnection connection = new ClickHouseConnection(dbString))
            //    {
            //        using (DbCommand command = new SqlCommand())
            //        {
            //            command.Connection = connection;
            //            command.CommandText = sqlBuilder.ToString();
            //            if (parameters.Count > 0)
            //                foreach (var item in parameters)
            //                    command.Parameters.Add(item);

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

            //            //尝试执行语句返回DataReader
            //            DbDataReader reader = null;
            //            try
            //            {
            //                reader = await command.ExecuteReaderAsync();
            //            }
            //            catch (Exception ex)
            //            {
            //                result.AppendError("sql语句执行错误，" + command.CommandText);
            //                result.AppendException(ex);

            //                await command.DisposeAsync();
            //                await connection.CloseAsync();
            //                await connection.DisposeAsync();

            //                return result;
            //            }
            //            if (reader != null && reader.HasRows)
            //            {
            //                List<DbRowRecord> rowDataList = new List<DbRowRecord>();//设置所有的行数据容器
            //                DbRowRecord rowItem = null;//设置行数据对象
            //                DbColumnRecord columnItem = null;//列数据对象
            //                while (await reader.ReadAsync())
            //                {
            //                    rowItem = new DbRowRecord();

            //                    //开始遍历所有的列数据
            //                    foreach (var item in resolveResult.SqlSelectFields)
            //                    {
            //                        object objVal = reader[item.DBFieldAsName];
            //                        if (objVal != null && objVal != DBNull.Value)
            //                        {
            //                            columnItem = new DbColumnRecord();
            //                            columnItem.Name = item.DBFieldAsName;
            //                            columnItem.Value = objVal;

            //                            //在行数据对象中装载列数据
            //                            rowItem.Add(columnItem);
            //                        }
            //                    }
            //                    rowDataList.Add(rowItem);
            //                }

            //                result.Record = rowDataList;

            //                //释放资源，关闭连结
            //                await reader.CloseAsync();
            //                await reader.DisposeAsync();
            //            }
            //        }
            //    }

            //    #endregion

            //    return result;
            //}
            //else
            //{
            //    #region 必须至少指定一个运算模式，例如:Count,Sum,Max,Min等

            //    result.AppendError("必须至少指定一个运算模式，例如:Count,Sum,Max,Min等");
            //    return result;

            //    #endregion
            //}
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 尝试打开数据库链接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool TryOpenDbConnection<T>(DbConnection connection, ref T result)
            where T : ResultBase
        {
            bool isOpen;
            try
            {
                connection.Open();
                isOpen = true;
            }
            catch (Exception ex)
            {
                isOpen = false;
                result.AppendError("数据库无法打开!");
                result.AppendException(ex);
            }
            return isOpen;
        }

        /// <summary>
        /// 尝试执行DBDataReader,可能返回为null值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private DbDataReader TryExecuteReader<T>(DbCommand command, ref T result)
            where T : ResultBase
        {
            DbDataReader reader;
            try
            {
                reader = command.ExecuteReader();
            }
            catch (Exception ex)
            {
                reader = null;
                result.AppendError("sql语句执行错误，" + command.CommandText);
                result.AppendException(ex);
            }
            return reader;
        }

        /// <summary>
        /// 执行关闭并且释放资源
        /// </summary>
        /// <param name="reader"></param>
        public void DisposeReader(DbDataReader reader)
        {
            if (null == reader)
                return;

            //释放资源，关闭连结
            using (reader as IDisposable) { }
        }

        /// <summary>
        /// Model实体自动填充(请在調用该方法前进行reader.Read()判断)
        /// </summary>
        /// <param name="reader">数据源</param>
        /// <param name="dbModelT">当前dbmodel类型</param>
        /// <param name="selectFields">需要被指定填充的字段</param>
        /// <returns></returns>
        private M AutoFillModel(DbDataReader reader, Type dbModelT, IEnumerable<ClickHouseSelectField> selectFields)
        {
            bool isCreateInstance = false;
            M model = default;

            DbColumnAttribute[] columns = this._dbMappingHandler.GetDbColumnCollection(dbModelT);
            if (null == columns || columns.Length <= 0)
                return model;

            if (null == selectFields || selectFields.Count() <= 0)
                return model;

            //开始循环填充Model中指定要被填充的属性值
            foreach (var item in selectFields)
            {
                if (!columns.Any(d => d.DbColumnName == item.DBFieldAsName))
                    continue;
                if (reader.GetOrdinal(item.DBFieldAsName) < 0)
                    continue;
                object fieldValue = reader[item.DBFieldAsName];
                if (DBNull.Value == fieldValue)
                    continue;

                if (!isCreateInstance)
                {
                    model = new M();
                    isCreateInstance = true;
                }

                DbColumnAttribute cur_column = columns.First(d => d.DbColumnName == item.DBFieldAsName);
                if (null == cur_column)
                    continue;

                PropertyInfo p = dbModelT.GetProperty(cur_column.PropertyNameMapping);
                if (null == p)
                    continue;

                fieldValue = fieldValue.GetType() == typeof(Guid) ? Guid.Parse(fieldValue.ToString()) : Convert.ChangeType(fieldValue, p.PropertyType, default(IFormatProvider));
                p.SetValue(model, fieldValue, null);
            }

            return model;
        }

        /// <summary>
        /// 根据数据库格式类型获取SqlDbType类型
        /// </summary>
        /// <param name="dbtypeName"></param>
        /// <returns></returns>
        private System.Data.DbType GetDbtype(string dbtypeName)
        {
            var dbType = System.Data.DbType.String; // 默认设置为String
            bool isFind = true;

            switch (dbtypeName.ToLower().Trim())
            {
                case "int8":
                case "tinyint":
                    dbType = System.Data.DbType.SByte;
                    break;
                case "int16":
                case "smallint":
                    dbType = System.Data.DbType.Int16;
                    break;
                case "int32":
                case "int":
                    dbType = System.Data.DbType.Int32;
                    break;
                case "int64":
                case "bigint":
                    dbType = System.Data.DbType.Int64;
                    break;
                case "uint8":
                    dbType = System.Data.DbType.Byte;
                    break;
                case "uint16":
                    dbType = System.Data.DbType.UInt16;
                    break;
                case "uint32":
                    dbType = System.Data.DbType.UInt32;
                    break;
                case "uint64":
                    dbType = System.Data.DbType.UInt64;
                    break;
                case "float32":
                case "float":
                    dbType = System.Data.DbType.Single;
                    break;
                case "float64":
                case "double":
                    dbType = System.Data.DbType.Double;
                    break;
                case "decimal":
                    dbType = System.Data.DbType.Decimal;
                    break;
                case "string":
                case "fixedstring":
                case "varchar":
                case "char":
                    dbType = System.Data.DbType.String;
                    break;
                case "date":
                    dbType = System.Data.DbType.Date;
                    break;
                case "datetime":
                case "datetime64":
                    dbType = System.Data.DbType.DateTime;
                    break;
                case "uuid":
                    dbType = System.Data.DbType.Guid;
                    break;
                case "bool":
                case "boolean":
                    dbType = System.Data.DbType.Boolean;
                    break;
                case "array":
                    dbType = System.Data.DbType.Object; // Array 类型通常用 Object 表示
                    break;
                default:
                    isFind = false;
                    break;
            }

            if (!isFind)
                throw new Exception("未找到类型 " + dbtypeName);

            return dbType;
        }

        #endregion

        #region IDbConnectionString

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <returns></returns>
        string IDbConnectionString.GetConnection()
        {
            return this._dbConnQuery;
        }

        #endregion
    }
}
