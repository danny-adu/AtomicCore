﻿using System;
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
                var dbType = ClickHouseDbHelper.GetDbtype(item.DbType);
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
                    if (ClickHouseDbHelper.TryOpenDbConnection(connection, ref result))
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
                    if (ClickHouseDbHelper.TryOpenDbConnection(connection, ref result))
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
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public DbNonRecord Update(Expression<Func<M, bool>> whereExp, M model, string suffix = null)
        {
            throw new NotImplementedException("ClickHouse Not Supported");
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
            throw new NotImplementedException("ClickHouse Not Supported");
        }

        /// <summary>
        /// 删除操作
        /// </summary>
        /// <param name="deleteExp">删除条件</param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public DbNonRecord Delete(Expression<Func<M, bool>> deleteExp, string suffix = null)
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
            if (!string.IsNullOrEmpty(suffix))
                tableName = $"{tableName}{suffix}";

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
                        //尝试执行SQL语句
                        DbDataReader reader = ClickHouseDbHelper.TryExecuteReader(command, ref result);
                        if (reader != null && reader.HasRows && reader.Read())
                        {
                            result.Record = ClickHouseDbHelper.AutoFillModel<M>(reader, modelT, _dbMappingHandler, resolveResult.SqlSelectFields);
                            //释放资源，关闭连结
                            ClickHouseDbHelper.DisposeReader(reader);
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
            DbCollectionRecord<M> result = new DbCollectionRecord<M>();

            string dbString = this._dbConnectionStringHandler.GetConnection();
            if (string.IsNullOrEmpty(dbString))
                throw new Exception("dbString is null");

            Type modelT = typeof(M);
            DbColumnAttribute[] columns = this._dbMappingHandler.GetDbColumnCollection(modelT);
            if (null == columns || columns.Length <= 0)
            {
                result.AppendError(string.Format("类型{0}未映射对上任何字段", modelT.FullName));
                return result;
            }

            int currentPage = 0;
            int pageSize = 0;
            ClickHouseSentenceResult resolveResult = null;

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

            // 参数定义
            bool count_from_list = false;
            StringBuilder countBuilder = new StringBuilder();
            StringBuilder queryBuilder = new StringBuilder("select ");

            if (resolveResult == null)
            {
                #region 初始化查询分析中间对象

                // 初始化保证该不为空（下面查询出数据后封装model会用）
                resolveResult = ClickHouseSentenceResult.Create();

                // 设置分页参数
                currentPage = ClickHouseSentenceResult.DEFAULT_CURRENTPAGE;
                pageSize = ClickHouseSentenceResult.DEFAULT_PAGESIZE;
                resolveResult.SetPageCondition(currentPage, pageSize);

                //设置检索的全字段，默认查询所有
                foreach (var item in columns)
                    resolveResult.SetSelectField(new ClickHouseSelectField
                    {
                        DBFieldAsName = item.DbColumnName,
                        DBSelectFragment = item.DbColumnName,
                        IsModelProperty = true
                    });

                #endregion

                #region 强制设置数据结果集统计从列表中加载

                // 指示数据集从list加载
                count_from_list = true;

                #endregion

                #region 拼接构造查询语句

                // 查询出所有的字段
                string all_cols = string.Join(',', columns.Select(s => $"{ClickHouseGrammarRule.GenerateFieldWrapped(s.DbColumnName)}"));

                // 已经是默认的分页索引和页码
                queryBuilder.Append(all_cols);
                queryBuilder.Append(" from ");
                queryBuilder.Append(ClickHouseGrammarRule.GenerateTableWrapped(tableName));
                queryBuilder.Append($" limit {pageSize}");

                #endregion
            }
            else
            {
                #region 读取分页参数

                // 参数读取
                currentPage = resolveResult.SqlPagerCondition.Key;
                pageSize = resolveResult.SqlPagerCondition.Value;

                // 检查分页参数的非法与合理性
                if (currentPage < 1)
                    currentPage = ClickHouseSentenceResult.DEFAULT_CURRENTPAGE;
                if (pageSize < 1)
                    pageSize = ClickHouseSentenceResult.DEFAULT_PAGESIZE;

                #endregion

                #region 特殊情况处理（若currpage=1并且pageSize=int.MaxValue,那么就是查询所有的数据,所以可设置不启动分页）

                bool enablePaging = true;
                if (1 == currentPage && int.MaxValue == pageSize)
                    enablePaging = false;

                #endregion

                #region 启用分页则需要根据条件查询总数量(拼接构造统计语句)

                if (enablePaging)
                {
                    countBuilder.Append($"select count(1) from {ClickHouseGrammarRule.GenerateTableWrapped(tableName)}");
                    if (!string.IsNullOrEmpty(resolveResult.SqlWhereConditionText))
                    {
                        countBuilder.Append(" where ");
                        countBuilder.Append(resolveResult.SqlWhereConditionText);
                    }
                    countBuilder.Append(";");
                }
                else
                    count_from_list = true; // 指示数据集从list加载

                #endregion

                #region 拼接构造查询语句

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

                queryBuilder.Append(all_cols);

                #endregion

                #region 指定搜索表

                queryBuilder.Append(" from ");
                queryBuilder.Append(ClickHouseGrammarRule.GenerateTableWrapped(tableName));

                #endregion

                #region 指定Where条件

                if (!string.IsNullOrEmpty(resolveResult.SqlWhereConditionText))
                {
                    queryBuilder.Append(" where ");
                    queryBuilder.Append(resolveResult.SqlWhereConditionText);
                }

                #endregion

                #region 指定Order条件

                if (!string.IsNullOrEmpty(resolveResult.SqlOrderConditionText))
                {
                    queryBuilder.Append(" order by ");
                    queryBuilder.Append(resolveResult.SqlOrderConditionText);
                }

                #endregion

                #region 分页条件

                queryBuilder.Append($" limit {pageSize}");

                int offset = (currentPage - 1) * pageSize;
                if (offset > 0)
                    queryBuilder.Append($" offset {offset}");

                #endregion

                #endregion
            }
            queryBuilder.Append(";");

            //初始化Debug
            var debugBuilder = new StringBuilder();
            if (countBuilder.Length > 0)
                debugBuilder.Append(countBuilder);
            if (queryBuilder.Length > 0)
                debugBuilder.Append(queryBuilder);
            result.DebugInit(debugBuilder, ClickHouseGrammarRule.C_ParamChar);

            #endregion

            #region 执行Sql语句

            using (DbConnection connection = new ClickHouseConnection(dbString))
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.Connection = connection;

                    //尝试打开数据库链接
                    if (ClickHouseDbHelper.TryOpenDbConnection(connection, ref result))
                    {
                        //尝试执行语句返回第一行第一列
                        if (countBuilder.Length > 0)
                        {
                            command.CommandText = countBuilder.ToString();
                            result.TotalCount = Convert.ToInt32(command.ExecuteScalar());

                            count_from_list = false;
                        }
                        else
                            count_from_list = true;

                        //如果存在符合条件的数据则进行二次查询，否则跳出
                        if (count_from_list || result.TotalCount > 0)
                        {
                            result.CurrentPage = currentPage;
                            result.PageSize = pageSize;

                            //尝试执行语句返回DataReader
                            command.CommandText = queryBuilder.ToString();
                            DbDataReader reader = ClickHouseDbHelper.TryExecuteReader(command, ref result);
                            if (reader != null && reader.HasRows)
                            {
                                result.Record = new List<M>();
                                M entity = default;
                                while (reader.Read())
                                {
                                    entity = ClickHouseDbHelper.AutoFillModel<M>(reader, modelT, _dbMappingHandler, resolveResult.SqlSelectFields);
                                    result.Record.Add(entity);
                                }
                                //释放资源，关闭连结
                                ClickHouseDbHelper.DisposeReader(reader);
                            }

                            // 查询符合条件的数据赋值
                            if (count_from_list)
                                result.TotalCount = null == result.Record ? 0 : result.Record.Count;
                        }
                    }
                }
            }

            #endregion

            return result;
        }

        /// <summary>
        /// 执行计算 Count, SUM，MAX,MIN等
        /// </summary>
        /// <param name="exp">查询表达式</param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public DbCalculateRecord Calculate(Expression<Func<IDbCalculateQueryable<M>, IDbCalculateQueryable<M>>> exp, string suffix = null)
        {
            DbCalculateRecord result = new DbCalculateRecord();

            string dbString = this._dbConnectionStringHandler.GetConnection();
            if (string.IsNullOrEmpty(dbString))
                throw new Exception("dbString is null");

            Type modelT = typeof(M);
            string tableName = this._dbMappingHandler.GetDbTableName(modelT);
            if (!string.IsNullOrEmpty(suffix))
                tableName = $"{tableName}{suffix}";

            #region 解析表达式条件

            if (null == exp)
            {
                result.AppendError("表达式exp不允许为空");
                return result;
            }

            var resolveResult = ClickHouseSentenceHandler.ExecuteResolver(exp, this._dbMappingHandler);
            if (!resolveResult.IsAvailable())
            {
                result.CopyStatus(resolveResult);
                return result;
            }
            if (null == resolveResult.SqlSelectFields || resolveResult.SqlSelectFields.Count() <= 0)
            {
                result.AppendError("必须至少指定一个运算模式，例如:Count,Sum,Max,Min等");
                return result;
            }

            #endregion

            #region 拼接Sql语句

            StringBuilder sqlBuilder = new StringBuilder("select ");

            foreach (var item in resolveResult.SqlSelectFields.OrderBy(d => d.IsModelProperty).OrderBy(d => d.DBFieldAsName).OrderByDescending(d => d.IsModelProperty))
            {
                if (item.IsModelProperty)
                    // 说明是表字段
                    sqlBuilder.Append($"{ClickHouseGrammarRule.GenerateFieldWrapped(item.DBSelectFragment)} as {item.DBFieldAsName},");
                else
                    // 说明是运算函数等
                    sqlBuilder.Append($"{item.DBSelectFragment} as {item.DBFieldAsName},");
            }

            sqlBuilder.Replace(",", "", sqlBuilder.Length - 1, 1);
            sqlBuilder.Append(" from ");
            sqlBuilder.Append(ClickHouseGrammarRule.GenerateTableWrapped(tableName));

            if (!string.IsNullOrEmpty(resolveResult.SqlWhereConditionText))
            {
                sqlBuilder.Append(" where ");
                sqlBuilder.Append(resolveResult.SqlWhereConditionText);
            }

            if (!string.IsNullOrEmpty(resolveResult.SqlGroupConditionBuilder))
            {
                sqlBuilder.Append(" group by ");
                sqlBuilder.Append(resolveResult.SqlGroupConditionBuilder);
            }
            if (!string.IsNullOrEmpty(resolveResult.SqlOrderConditionText))
            {
                sqlBuilder.Append(" order by ");
                sqlBuilder.Append(resolveResult.SqlOrderConditionText);
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

                    //尝试打开数据库链接
                    if (ClickHouseDbHelper.TryOpenDbConnection(connection, ref result))
                    {
                        //尝试执行语句返回DataReader
                        DbDataReader reader = ClickHouseDbHelper.TryExecuteReader(command, ref result);
                        if (reader != null && reader.HasRows)
                        {
                            List<DbRowRecord> rowDataList = new List<DbRowRecord>();//设置所有的行数据容器
                            DbRowRecord rowItem = null;//设置行数据对象
                            DbColumnRecord columnItem = null;//列数据对象
                            while (reader.Read())
                            {
                                rowItem = new DbRowRecord();

                                //开始遍历所有的列数据
                                foreach (var item in resolveResult.SqlSelectFields)
                                {
                                    object objVal = reader[item.DBFieldAsName];
                                    if (objVal != null && objVal != DBNull.Value)
                                    {
                                        columnItem = new DbColumnRecord
                                        {
                                            Name = item.DBFieldAsName,
                                            Value = objVal
                                        };

                                        //在行数据对象中装载列数据
                                        rowItem.Add(columnItem);
                                    }
                                }
                                rowDataList.Add(rowItem);
                            }
                            result.Record = rowDataList;

                            //释放资源，关闭连结
                            ClickHouseDbHelper.DisposeReader(reader);
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
                var dbType = ClickHouseDbHelper.GetDbtype(item.DbType);
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
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public Task<DbNonRecord> UpdateAsync(Expression<Func<M, bool>> whereExp, M model, string suffix = null)
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
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public Task<DbNonRecord> UpdateTaskAsync(IEnumerable<DbUpdateTaskData<M>> taskList, bool enableSqlTransaction = false, string suffix = null)
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
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public Task<DbNonRecord> DeleteAsync(Expression<Func<M, bool>> deleteExp, string suffix = null)
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

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="exp">查询表达式</param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public async Task<DbSingleRecord<M>> FetchAsync(Expression<Func<IDbFetchQueryable<M>, IDbFetchQueryable<M>>> exp, string suffix = null)
        {
            DbSingleRecord<M> result = new DbSingleRecord<M>();

            string dbString = this._dbConnectionStringHandler.GetConnection();
            if (string.IsNullOrEmpty(dbString))
                throw new Exception("dbString is null");

            Type modelT = typeof(M);
            DbColumnAttribute[] columns = this._dbMappingHandler.GetDbColumnCollection(modelT);

            ClickHouseSentenceResult resolveResult = null;

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
            result.DebugInit(sqlBuilder, ClickHouseGrammarRule.C_ParamChar);

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

                    //尝试执行SQL语句
                    DbDataReader reader = null;
                    try
                    {
                        reader = await command.ExecuteReaderAsync();
                    }
                    catch (Exception ex)
                    {
                        result.AppendError("sql语句执行错误，" + command.CommandText);
                        result.AppendException(ex);

                        await command.DisposeAsync();
                        await connection.CloseAsync();
                        await connection.DisposeAsync();

                        return result;
                    }
                    if (reader != null && reader.HasRows && await reader.ReadAsync())
                    {
                        result.Record = ClickHouseDbHelper.AutoFillModel<M>(reader, modelT, _dbMappingHandler, resolveResult.SqlSelectFields);

                        //释放资源，关闭连结
                        await reader.CloseAsync();
                        await reader.DisposeAsync();
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
        public async Task<DbCollectionRecord<M>> FetchListAsync(Expression<Func<IDbFetchListQueryable<M>, IDbFetchListQueryable<M>>> exp, string suffix = null)
        {
            DbCollectionRecord<M> result = new DbCollectionRecord<M>();

            string dbString = this._dbConnectionStringHandler.GetConnection();
            if (string.IsNullOrEmpty(dbString))
                throw new Exception("dbString is null");

            Type modelT = typeof(M);
            DbColumnAttribute[] columns = this._dbMappingHandler.GetDbColumnCollection(modelT);
            if (null == columns || columns.Length <= 0)
            {
                result.AppendError(string.Format("类型{0}未映射对上任何字段", modelT.FullName));
                return result;
            }

            int currentPage = 0;
            int pageSize = 0;
            ClickHouseSentenceResult resolveResult = null;

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

            // 参数定义
            bool count_from_list = false;
            StringBuilder countBuilder = new StringBuilder();
            StringBuilder queryBuilder = new StringBuilder("select ");

            if (resolveResult == null)
            {
                #region 初始化查询分析中间对象

                // 初始化保证该不为空（下面查询出数据后封装model会用）
                resolveResult = ClickHouseSentenceResult.Create();

                // 设置分页参数
                currentPage = ClickHouseSentenceResult.DEFAULT_CURRENTPAGE;
                pageSize = ClickHouseSentenceResult.DEFAULT_PAGESIZE;
                resolveResult.SetPageCondition(currentPage, pageSize);

                //设置检索的全字段，默认查询所有
                foreach (var item in columns)
                    resolveResult.SetSelectField(new ClickHouseSelectField
                    {
                        DBFieldAsName = item.DbColumnName,
                        DBSelectFragment = item.DbColumnName,
                        IsModelProperty = true
                    });

                #endregion

                #region 强制设置数据结果集统计从列表中加载

                // 指示数据集从list加载
                count_from_list = true;

                #endregion

                #region 拼接构造查询语句

                // 查询出所有的字段
                string all_cols = string.Join(',', columns.Select(s => $"{ClickHouseGrammarRule.GenerateFieldWrapped(s.DbColumnName)}"));

                // 已经是默认的分页索引和页码
                queryBuilder.Append(all_cols);
                queryBuilder.Append(" from ");
                queryBuilder.Append(ClickHouseGrammarRule.GenerateTableWrapped(tableName));
                queryBuilder.Append($" limit {pageSize}");

                #endregion
            }
            else
            {
                #region 读取分页参数

                // 参数读取
                currentPage = resolveResult.SqlPagerCondition.Key;
                pageSize = resolveResult.SqlPagerCondition.Value;

                // 检查分页参数的非法与合理性
                if (currentPage < 1)
                    currentPage = ClickHouseSentenceResult.DEFAULT_CURRENTPAGE;
                if (pageSize < 1)
                    pageSize = ClickHouseSentenceResult.DEFAULT_PAGESIZE;

                #endregion

                #region 特殊情况处理（若currpage=1并且pageSize=int.MaxValue,那么就是查询所有的数据,所以可设置不启动分页）

                bool enablePaging = true;
                if (1 == currentPage && int.MaxValue == pageSize)
                    enablePaging = false;

                #endregion

                #region 启用分页则需要根据条件查询总数量(拼接构造统计语句)

                if (enablePaging)
                {
                    countBuilder.Append($"select count(1) from {ClickHouseGrammarRule.GenerateTableWrapped(tableName)}");
                    if (!string.IsNullOrEmpty(resolveResult.SqlWhereConditionText))
                    {
                        countBuilder.Append(" where ");
                        countBuilder.Append(resolveResult.SqlWhereConditionText);
                    }
                    countBuilder.Append(";");
                }
                else
                    count_from_list = true; // 指示数据集从list加载

                #endregion

                #region 拼接构造查询语句

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

                queryBuilder.Append(all_cols);

                #endregion

                #region 指定搜索表

                queryBuilder.Append(" from ");
                queryBuilder.Append(ClickHouseGrammarRule.GenerateTableWrapped(tableName));

                #endregion

                #region 指定Where条件

                if (!string.IsNullOrEmpty(resolveResult.SqlWhereConditionText))
                {
                    queryBuilder.Append(" where ");
                    queryBuilder.Append(resolveResult.SqlWhereConditionText);
                }

                #endregion

                #region 指定Order条件

                if (!string.IsNullOrEmpty(resolveResult.SqlOrderConditionText))
                {
                    queryBuilder.Append(" order by ");
                    queryBuilder.Append(resolveResult.SqlOrderConditionText);
                }

                #endregion

                #region 分页条件

                queryBuilder.Append($" limit {pageSize}");

                int offset = (currentPage - 1) * pageSize;
                if (offset > 0)
                    queryBuilder.Append($" offset {offset}");

                #endregion

                #endregion
            }
            queryBuilder.Append(";");

            //初始化Debug
            var debugBuilder = new StringBuilder();
            if (countBuilder.Length > 0)
                debugBuilder.Append(countBuilder);
            if (queryBuilder.Length > 0)
                debugBuilder.Append(queryBuilder);
            result.DebugInit(debugBuilder, ClickHouseGrammarRule.C_ParamChar);

            #endregion

            #region 执行Sql语句

            using (DbConnection connection = new ClickHouseConnection(dbString))
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.Connection = connection;

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

                    // 判断是否需要执行统计语句
                    if (countBuilder.Length > 0)
                    {
                        //尝试执行语句返回第一行第一列
                        command.CommandText = countBuilder.ToString();
                        result.TotalCount = Convert.ToInt32(await command.ExecuteScalarAsync());

                        count_from_list = false;
                    }
                    else
                        count_from_list = true;

                    //如果存在符合条件的数据则进行二次查询，否则跳出
                    if (count_from_list || result.TotalCount > 0)
                    {
                        result.CurrentPage = currentPage;
                        result.PageSize = pageSize;

                        //尝试执行语句返回DataReader
                        command.CommandText = queryBuilder.ToString();

                        DbDataReader reader = null;
                        try
                        {
                            reader = await command.ExecuteReaderAsync();
                        }
                        catch (Exception ex)
                        {
                            result.AppendError("sql语句执行错误，" + command.CommandText);
                            result.AppendException(ex);

                            await command.DisposeAsync();
                            await connection.CloseAsync();
                            await connection.DisposeAsync();

                            return result;
                        }
                        if (reader != null && reader.HasRows)
                        {
                            result.Record = new List<M>();
                            M entity = default;
                            while (await reader.ReadAsync())
                            {
                                entity = ClickHouseDbHelper.AutoFillModel<M>(reader, modelT, _dbMappingHandler, resolveResult.SqlSelectFields);
                                result.Record.Add(entity);
                            }

                            //释放资源，关闭连结
                            await reader.CloseAsync();
                            await reader.DisposeAsync();
                        }

                        // 查询符合条件的数据赋值
                        if (count_from_list)
                            result.TotalCount = null == result.Record ? 0 : result.Record.Count;
                    }
                }
            }

            #endregion

            return result;
        }

        /// <summary>
        /// 执行计算 Count, SUM，MAX,MIN等
        /// </summary>
        /// <param name="exp">查询表达式</param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public async Task<DbCalculateRecord> CalculateAsync(Expression<Func<IDbCalculateQueryable<M>, IDbCalculateQueryable<M>>> exp, string suffix = null)
        {
            DbCalculateRecord result = new DbCalculateRecord();

            string dbString = this._dbConnectionStringHandler.GetConnection();
            if (string.IsNullOrEmpty(dbString))
                throw new Exception("dbString is null");

            Type modelT = typeof(M);
            string tableName = this._dbMappingHandler.GetDbTableName(modelT);
            if (!string.IsNullOrEmpty(suffix))
                tableName = $"{tableName}{suffix}";

            #region 解析表达式条件

            if (null == exp)
            {
                result.AppendError("表达式exp不允许为空");
                return result;
            }

            var resolveResult = ClickHouseSentenceHandler.ExecuteResolver(exp, this._dbMappingHandler);
            if (!resolveResult.IsAvailable())
            {
                result.CopyStatus(resolveResult);
                return result;
            }
            if (null == resolveResult.SqlSelectFields || resolveResult.SqlSelectFields.Count() <= 0)
            {
                result.AppendError("必须至少指定一个运算模式，例如:Count,Sum,Max,Min等");
                return result;
            }

            #endregion

            #region 拼接Sql语句

            StringBuilder sqlBuilder = new StringBuilder("select ");

            foreach (var item in resolveResult.SqlSelectFields.OrderBy(d => d.IsModelProperty).OrderBy(d => d.DBFieldAsName).OrderByDescending(d => d.IsModelProperty))
            {
                if (item.IsModelProperty)
                    // 说明是表字段
                    sqlBuilder.Append($"{ClickHouseGrammarRule.GenerateFieldWrapped(item.DBSelectFragment)} as {item.DBFieldAsName},");
                else
                    // 说明是运算函数等
                    sqlBuilder.Append($"{item.DBSelectFragment} as {item.DBFieldAsName},");
            }

            sqlBuilder.Replace(",", "", sqlBuilder.Length - 1, 1);
            sqlBuilder.Append(" from ");
            sqlBuilder.Append(ClickHouseGrammarRule.GenerateTableWrapped(tableName));

            if (!string.IsNullOrEmpty(resolveResult.SqlWhereConditionText))
            {
                sqlBuilder.Append(" where ");
                sqlBuilder.Append(resolveResult.SqlWhereConditionText);
            }

            if (!string.IsNullOrEmpty(resolveResult.SqlGroupConditionBuilder))
            {
                sqlBuilder.Append(" group by ");
                sqlBuilder.Append(resolveResult.SqlGroupConditionBuilder);
            }
            if (!string.IsNullOrEmpty(resolveResult.SqlOrderConditionText))
            {
                sqlBuilder.Append(" order by ");
                sqlBuilder.Append(resolveResult.SqlOrderConditionText);
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

                    //尝试执行语句返回DataReader
                    DbDataReader reader = null;
                    try
                    {
                        reader = await command.ExecuteReaderAsync();
                    }
                    catch (Exception ex)
                    {
                        result.AppendError("sql语句执行错误，" + command.CommandText);
                        result.AppendException(ex);

                        await command.DisposeAsync();
                        await connection.CloseAsync();
                        await connection.DisposeAsync();

                        return result;
                    }
                    if (reader != null && reader.HasRows)
                    {
                        List<DbRowRecord> rowDataList = new List<DbRowRecord>();//设置所有的行数据容器
                        DbRowRecord rowItem = null;//设置行数据对象
                        DbColumnRecord columnItem = null;//列数据对象
                        while (await reader.ReadAsync())
                        {
                            rowItem = new DbRowRecord();

                            //开始遍历所有的列数据
                            foreach (var item in resolveResult.SqlSelectFields)
                            {
                                object objVal = reader[item.DBFieldAsName];
                                if (objVal != null && objVal != DBNull.Value)
                                {
                                    columnItem = new DbColumnRecord();
                                    columnItem.Name = item.DBFieldAsName;
                                    columnItem.Value = objVal;

                                    //在行数据对象中装载列数据
                                    rowItem.Add(columnItem);
                                }
                            }
                            rowDataList.Add(rowItem);
                        }

                        result.Record = rowDataList;

                        //释放资源，关闭连结
                        await reader.CloseAsync();
                        await reader.DisposeAsync();
                    }
                }
            }

            #endregion

            return result;
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
