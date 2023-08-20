using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using MySql.Data.MySqlClient;
using AtomicCore.DbProvider;
using System.Data.Common;
using System.Threading.Tasks;

namespace AtomicCore.Integration.MysqlDbProvider
{
    /// <summary>
    /// Mysql版本的或以后的版本的数据仓储驱动类
    /// </summary>
    /// <typeparam name="M"></typeparam>
    public class MysqlDbProvider<M> : IDbProvider<M>, IDbConnectionString, IDbConnectionString<M>
        where M : IDbModel, new()
    {
        #region Variable

        /// <summary>
        /// 批量插入批次总数据
        /// </summary>
        private const int c_batchTotal = 500;

        #endregion

        #region Constructors

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private string _dbConnQuery = null;
        /// <summary>
        /// 数据库链接字符串 
        /// </summary>
        private IDbConnectionString _dbConnectionStringHandler = null;
        /// <summary>
        /// 数据库字段映射处理接口
        /// </summary>
        private IDbMappingHandler _dbMappingHandler = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbConnQuery">数据库链接字符串</param>
        /// <param name="dbMappingHandler">数据库字段映射处理接口</param>
        public MysqlDbProvider(string dbConnQuery, IDbMappingHandler dbMappingHandler)
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
        public MysqlDbProvider(IDbConnectionString dbConnString, IDbMappingHandler dbMappingHandler)
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

            if (model != null)
            {
                #region 判断主键数量

                DbColumnAttribute[] columns = this._dbMappingHandler.GetDbColumnCollection(modelT);
                if (null == columns || columns.Length <= 0)
                {
                    result.AppendError(string.Format("{0}类型无字段映射", modelT.FullName));
                    return result;
                }

                DbColumnAttribute[] setPrimaryKeys = columns.Where(d => d.IsDbGenerated).ToArray();
                if (setPrimaryKeys.Count() > 1)
                {
                    result.AppendError("暂不允许使用双自增长键！请最多设置一列为自增长主键！");
                    return result;
                }

                #endregion

                //需要设置参数插入的字段
                DbColumnAttribute[] setFields = columns.Where(d => !d.IsDbGenerated).ToArray();
                if (setFields.Count() > 0)
                {
                    List<MySqlParameter> parameters = new List<MySqlParameter>();

                    #region 拼接Sql语句

                    string tableName = this._dbMappingHandler.GetDbTableName(modelT);
                    if (!string.IsNullOrEmpty(suffix))
                        tableName = $"{tableName}{suffix}";

                    StringBuilder sqlBuilder = new StringBuilder("insert into ");
                    sqlBuilder.Append(MysqlGrammarRule.GenerateTableWrapped(tableName));
                    sqlBuilder.Append(" (");
                    foreach (var item in setFields.Select(d => d.DbColumnName))
                    {
                        sqlBuilder.AppendFormat("{0},", MysqlGrammarRule.GenerateFieldWrapped(item));
                    }
                    sqlBuilder.Replace(",", ")", sqlBuilder.Length - 1, 1);
                    sqlBuilder.Append(" values ");
                    sqlBuilder.Append("(");
                    foreach (var item in setFields)
                    {
                        string parameterName = string.Format("{0}", item.DbColumnName);
                        PropertyInfo p_info = this._dbMappingHandler.GetPropertySingle(modelT, item.DbColumnName);
                        object parameterValue = p_info.GetValue(model, null);

                        MySqlDbType dbType = this.GetDbtype(item.DbType);

                        MySqlParameter paremter = new MySqlParameter(MysqlGrammarRule.GenerateParamName(parameterName), dbType);
                        paremter.Value = parameterValue;
                        parameters.Add(paremter);

                        sqlBuilder.Append(MysqlGrammarRule.GenerateParamName(parameterName));
                        sqlBuilder.Append(",");
                    }
                    sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
                    sqlBuilder.Append(");");
                    sqlBuilder.Append("select @@identity;");
                    //初始化Debug
                    result.DebugInit(sqlBuilder, MysqlGrammarRule.C_ParamChar, parameters.ToArray());

                    #endregion

                    #region 执行Sql语句

                    using (MySqlConnection connection = new MySqlConnection(dbString))
                    {
                        using (MySqlCommand command = new MySqlCommand())
                        {
                            command.Connection = connection;
                            command.CommandText = sqlBuilder.ToString();
                            foreach (MySqlParameter item in parameters)
                            {
                                command.Parameters.Add(item);
                            }
                            //尝试打开数据库连结
                            if (this.TryOpenDbConnection(connection, ref result))
                            {
                                if (setPrimaryKeys != null && setPrimaryKeys.Count() > 0)
                                {
                                    try
                                    {
                                        object dbVal = command.ExecuteScalar();
                                        PropertyInfo pinfo = this._dbMappingHandler.GetPropertySingle(modelT, setPrimaryKeys.First().DbColumnName);
                                        if (pinfo != null && dbVal != DBNull.Value)
                                        {
                                            dbVal = Convert.ChangeType(dbVal, pinfo.PropertyType);
                                            pinfo.SetValue(model, dbVal, null);
                                        }
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
                                else
                                {
                                    try
                                    {
                                        int affectedRow = command.ExecuteNonQuery();
                                        if (affectedRow > 0)
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
                    }

                    #endregion

                    return result;
                }
                else
                {
                    result.AppendError("插入的表仅有自增长列或没有指定任何列");
                    return result;
                }
            }
            else
            {
                result.AppendError("插入数据时候的Model为空");
                return result;
            }
        }

        /// <summary>
        /// 批露插入数据
        /// </summary>
        /// <param name="modelList"></param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public DbCollectionRecord<M> InsertBatch(IEnumerable<M> modelList, string suffix = null)
        {
            DbCollectionRecord<M> result = new DbCollectionRecord<M>();
            result.Record = new List<M>();

            if (null == modelList || !modelList.Any())
            {
                result.AppendError("无数据需要进行批露插入");
                return result;
            }

            string dbString = this._dbConnectionStringHandler.GetConnection();
            if (string.IsNullOrEmpty(dbString))
                throw new Exception("dbString is null");

            Type modelT = typeof(M);
            string tableName = this._dbMappingHandler.GetDbTableName(modelT);
            if (!string.IsNullOrEmpty(suffix))
                tableName = $"{tableName}{suffix}";

            #region 判断主键数量

            DbColumnAttribute[] columns = this._dbMappingHandler.GetDbColumnCollection(modelT);
            if (null == columns || columns.Length <= 0)
            {
                result.AppendError(string.Format("{0}类型无字段映射", modelT.FullName));
                return result;
            }

            DbColumnAttribute[] setPrimaryKeys = columns.Where(d => d.IsDbGenerated).ToArray();
            if (setPrimaryKeys.Count() > 1)
            {
                result.AppendError("暂不允许使用双自增长键！请最多设置一列为自增长主键！");
                return result;
            }

            #endregion

            #region 获取所有非自增长列元数据

            DbColumnAttribute[] setFields = columns.Where(d => !d.IsDbGenerated).ToArray();
            if (!setFields.Any())
            {
                result.AppendError("插入的表仅有自增长列或没有指定任何列");
                return result;
            }

            #endregion

            #region 循环构建待插入数据脚本（循环拼接SQL语句）

            StringBuilder sqlBuilder = null;
            List<MySqlParameter> parameters = null;
            MySqlParameter paremter = null;
            Dictionary<M, KeyValuePair<StringBuilder, List<MySqlParameter>>> sqlList = new Dictionary<M, KeyValuePair<StringBuilder, List<MySqlParameter>>>();

            foreach (var model in modelList)
            {
                parameters = new List<MySqlParameter>();

                #region 拼接Sql语句

                sqlBuilder = new StringBuilder("insert into ");
                sqlBuilder.Append(MysqlGrammarRule.GenerateTableWrapped(tableName));
                sqlBuilder.Append(" (");
                foreach (var item in setFields.Select(d => d.DbColumnName))
                {
                    sqlBuilder.AppendFormat("{0},", MysqlGrammarRule.GenerateFieldWrapped(item));
                }
                sqlBuilder.Replace(",", ")", sqlBuilder.Length - 1, 1);
                sqlBuilder.Append(" values ");
                sqlBuilder.Append("(");
                foreach (var item in setFields)
                {
                    string parameterName = string.Format("{0}", item.DbColumnName);
                    PropertyInfo p_info = this._dbMappingHandler.GetPropertySingle(modelT, item.DbColumnName);
                    object parameterValue = p_info.GetValue(model, null);

                    MySqlDbType dbType = this.GetDbtype(item.DbType);

                    paremter = new MySqlParameter(MysqlGrammarRule.GenerateParamName(parameterName), dbType);
                    paremter.Value = parameterValue;
                    parameters.Add(paremter);

                    sqlBuilder.Append(MysqlGrammarRule.GenerateParamName(parameterName));
                    sqlBuilder.Append(",");
                }
                sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
                sqlBuilder.Append(");");
                sqlBuilder.Append("select @@identity;");

                #endregion

                sqlList.Add(model, new KeyValuePair<StringBuilder, List<MySqlParameter>>(sqlBuilder, parameters));
            }

            #endregion

            #region 执行Sql语句

            int sqlcount = 0;
            using (MySqlConnection connection = new MySqlConnection(dbString))
            {
                //尝试打开数据库连结
                if (!this.TryOpenDbConnection<DbCollectionRecord<M>>(connection, ref result))
                    return result;

                using (MySqlCommand command = new MySqlCommand())
                {
                    //开始事务
                    MySqlTransaction tx = connection.BeginTransaction();

                    //初始化赋值Command
                    command.Connection = connection;
                    command.Transaction = tx;

                    //开始循环事务插入
                    foreach (var sqlNode in sqlList)
                    {
                        command.Parameters.Clear();
                        command.CommandText = sqlNode.Value.Key.ToString();
                        foreach (MySqlParameter item in sqlNode.Value.Value)
                            command.Parameters.Add(item);

                        if (null != setPrimaryKeys && setPrimaryKeys.Any())
                        {
                            try
                            {
                                object dbVal = command.ExecuteScalar();
                                PropertyInfo pinfo = this._dbMappingHandler.GetPropertySingle(modelT, setPrimaryKeys.First().DbColumnName);
                                if (pinfo != null && dbVal != DBNull.Value)
                                {
                                    dbVal = Convert.ChangeType(dbVal, pinfo.PropertyType);
                                    pinfo.SetValue(sqlNode.Key, dbVal, null);
                                }
                                result.Record.Add(sqlNode.Key);
                            }
                            catch (Exception ex)
                            {
                                tx.Rollback();
                                result.Record.Clear();
                                result.AppendException(ex);

                                command.Dispose();
                                connection.Close();
                                connection.Dispose();

                                return result;
                            }
                        }
                        else
                        {
                            try
                            {
                                int affectedRow = command.ExecuteNonQuery();
                                if (affectedRow > 0)
                                    result.Record.Add(sqlNode.Key);
                            }
                            catch (Exception ex)
                            {
                                tx.Rollback();
                                result.Record.Clear();
                                result.AppendException(ex);

                                command.Dispose();
                                connection.Close();
                                connection.Dispose();

                                return result;
                            }
                        }

                        //已执行的SQL++
                        sqlcount++;

                        //按批次进行事务提交
                        if (sqlcount > 0 && (sqlcount % c_batchTotal == 0 || sqlcount == sqlList.Count))
                        {
                            tx.Commit();

                            if (sqlcount != sqlList.Count)
                                tx = connection.BeginTransaction();
                        }
                    }
                }
            }

            #endregion

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
            MysqlWhereScriptResult whereResult = null;
            MysqlUpdateScriptResult updatePropertyResult = null;

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
                whereResult = MysqlWhereScriptHandler.ExecuteResolver(where_func_lambdaExp, this._dbMappingHandler, false);
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
                    updatePropertyResult = MysqlUpdateScriptHandler.ExecuteResolver(updatePropertys, this._dbMappingHandler);
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

            string tableName = this._dbMappingHandler.GetDbTableName(modelT);
            if (!string.IsNullOrEmpty(suffix))
                tableName = $"{tableName}{suffix}";

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            MySqlParameter cur_parameter = null;
            StringBuilder sqlBuilder = new StringBuilder("update ");
            sqlBuilder.Append(MysqlGrammarRule.GenerateTableWrapped(tableName));
            sqlBuilder.Append(" set ");
            foreach (var item in updatePropertyResult.FieldMembers)
            {
                //自增长的自动跳过
                if (colums.Any(d => d.PropertyNameMapping == item.PropertyItem.Name && d.IsDbGenerated))
                {
                    continue;
                }

                string cur_field = colums.First(d => d.PropertyNameMapping == item.PropertyItem.Name).DbColumnName;

                sqlBuilder.Append(" ");
                sqlBuilder.Append(MysqlGrammarRule.GenerateFieldWrapped(cur_field));
                sqlBuilder.Append("=");
                sqlBuilder.Append(item.UpdateTextFragment);
                sqlBuilder.Append(",");

                foreach (var pitem in item.Parameter)
                {
                    cur_parameter = new MySqlParameter(pitem.Name, pitem.Value);
                    parameters.Add(cur_parameter);
                }
            }
            sqlBuilder.Replace(",", " ", sqlBuilder.Length - 1, 1);
            if (whereResult != null)
            {
                sqlBuilder.Append("where ");
                sqlBuilder.Append(whereResult.TextScript);
                foreach (var item in whereResult.Parameters)
                {
                    cur_parameter = new MySqlParameter(item.Name, item.Value);
                    parameters.Add(cur_parameter);
                }
            }
            sqlBuilder.Append(";");
            //初始化Debug
            result.DebugInit(sqlBuilder, MysqlGrammarRule.C_ParamChar, parameters.ToArray());

            #endregion

            #region 执行Sql语句

            using (MySqlConnection connection = new MySqlConnection(dbString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = sqlBuilder.ToString();
                    foreach (MySqlParameter item in parameters)
                        command.Parameters.Add(item);

                    //尝试打开数据库连结
                    if (this.TryOpenDbConnection<DbNonRecord>(connection, ref result))
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
            DbNonRecord result = new DbNonRecord();

            string dbString = this._dbConnectionStringHandler.GetConnection();
            if (string.IsNullOrEmpty(dbString))
                throw new Exception("dbString is null");

            Type modelT = typeof(M);
            MysqlWhereScriptResult whereResult = null;

            #region 验证需要被修改的实体是否为null

            if (model == null)
            {
                result.AppendError("修改数据时候的Model为空");
                return result;
            }

            #endregion

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

                //执行where解析
                whereResult = MysqlWhereScriptHandler.ExecuteResolver(where_func_lambdaExp, this._dbMappingHandler, false);
                if (!whereResult.IsAvailable())
                {
                    result.CopyStatus(whereResult);
                    return result;
                }
            }

            #endregion

            #region 开始拼接Sql语句

            DbColumnAttribute[] columns = this._dbMappingHandler.GetDbColumnCollection(modelT, d => !d.IsDbGenerated);

            string tableName = this._dbMappingHandler.GetDbTableName(modelT);
            if (!string.IsNullOrEmpty(suffix))
                tableName = $"{tableName}{suffix}";

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            MySqlParameter cur_parameter = null;
            StringBuilder sqlBuilder = new StringBuilder("update ");
            sqlBuilder.Append(MysqlGrammarRule.GenerateTableWrapped(tableName));
            sqlBuilder.Append(" set ");
            foreach (var item in columns)
            {
                PropertyInfo p = modelT.GetProperty(item.PropertyNameMapping);
                if (p != null)
                {
                    string parameterName = string.Format("set_{0}", item.DbColumnName);
                    object parameterVal = p.GetValue(model, null);

                    sqlBuilder.Append(MysqlGrammarRule.GenerateFieldWrapped(item.DbColumnName));
                    sqlBuilder.Append("=");
                    sqlBuilder.Append(MysqlGrammarRule.GenerateParamName(parameterName));
                    sqlBuilder.Append(",");

                    cur_parameter = new MySqlParameter(MysqlGrammarRule.GenerateParamName(parameterName), parameterVal);
                    parameters.Add(cur_parameter);
                }
            }
            sqlBuilder.Replace(",", " ", sqlBuilder.Length - 1, 1);
            if (whereResult != null)
            {
                sqlBuilder.Append("where ");
                sqlBuilder.Append(whereResult.TextScript);
                foreach (var item in whereResult.Parameters)
                {
                    cur_parameter = new MySqlParameter(item.Name, item.Value);
                    parameters.Add(cur_parameter);
                }
            }
            sqlBuilder.Append(";");
            //初始化Debug
            result.DebugInit(sqlBuilder, MysqlGrammarRule.C_ParamChar, parameters.ToArray());

            #endregion

            #region 执行Sql语句

            using (MySqlConnection connection = new MySqlConnection(dbString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = sqlBuilder.ToString();
                    foreach (MySqlParameter item in parameters)
                        command.Parameters.Add(item);

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
        /// 批量更新操作
        /// </summary>
        /// <param name="modelList"></param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public DbNonRecord UpdateBatch(IEnumerable<M> modelList, string suffix = null)
        {
            DbNonRecord result = new DbNonRecord();

            if (null == modelList || !modelList.Any())
                return result;

            string dbString = this._dbConnectionStringHandler.GetConnection();
            if (string.IsNullOrEmpty(dbString))
                throw new Exception("dbString is null");

            #region 临时变量

            Type modelT = typeof(M);
            string origTableName = this._dbMappingHandler.GetDbTableName(modelT);
            if (!string.IsNullOrEmpty(suffix))
                origTableName = $"{origTableName}{suffix}";
            DbColumnAttribute[] columns = this._dbMappingHandler.GetDbColumnCollection(modelT);

            #endregion

            #region 开始拼接Sql语句

            StringBuilder sqlBuilder = null;
            List<DbParameter> sqlParameter = null;
            List<DbUpdateTaskSqlData> sqlDataList = new List<DbUpdateTaskSqlData>();

            foreach (var model in modelList)
            {
                //构造循环builder
                sqlBuilder = new StringBuilder();
                sqlParameter = new List<DbParameter>();

                //拼接SQL + 填充参数化参数部分
                sqlBuilder.AppendFormat("update {0} set ", MysqlGrammarRule.GenerateTableWrapped(origTableName));
                foreach (var item in columns.Where(d => !d.IsDbGenerated))
                {
                    PropertyInfo p = modelT.GetProperty(item.PropertyNameMapping);
                    if (null == p)
                        continue;

                    string each_paramName = string.Format("set_{0}", item.DbColumnName);
                    object each_paramVal = p.GetValue(model, null);

                    sqlBuilder.AppendFormat(
                        "{0}={1},",
                        MysqlGrammarRule.GenerateFieldWrapped(item.DbColumnName),
                        MysqlGrammarRule.GenerateParamName(each_paramName)
                    );
                    sqlParameter.Add(new MySqlParameter(MysqlGrammarRule.GenerateParamName(each_paramName), each_paramVal));
                }
                sqlBuilder.Replace(",", string.Empty, sqlBuilder.Length - 1, 1);
                sqlBuilder.Append(" where ");
                if (!columns.Any(d => d.IsDbPrimaryKey))
                {
                    result.AppendError(string.Format("表{0}尚未设置主键", origTableName));
                    return result;
                }
                else
                {
                    foreach (var item in columns.Where(d => d.IsDbPrimaryKey))
                    {
                        PropertyInfo p = modelT.GetProperty(item.PropertyNameMapping);
                        if (null == p)
                            continue;

                        object each_pkv = p.GetValue(model, null);

                        sqlBuilder.AppendFormat("{0}={1} and ", item.DbColumnName, each_pkv);
                    }
                    sqlBuilder.Remove(sqlBuilder.Length - 4, 4);
                }
                sqlBuilder.Append(";");

                sqlDataList.Add(new DbUpdateTaskSqlData()
                {
                    SqlText = sqlBuilder.ToString(),
                    SqlParameters = sqlParameter.ToArray()
                });
            }

            #endregion

            #region 执行Sql语句

            using (MySqlConnection connection = new MySqlConnection(dbString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    //设置command连接字符串
                    command.Connection = connection;

                    //尝试打开数据库
                    if (this.TryOpenDbConnection(connection, ref result))
                    {
                        //开始循环执行Sql
                        foreach (var sql in sqlDataList)
                        {
                            command.CommandText = sql.SqlText;
                            command.Parameters.Clear();
                            command.Parameters.AddRange(sql.SqlParameters);

                            try
                            {
                                result.AffectedRow += command.ExecuteNonQuery();
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
            }

            #endregion

            return result;
        }

        /// <summary>
        /// 批量更新任务
        /// </summary>
        /// <param name="taskList"></param>
        /// <param name="enableSqlTransaction"></param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public DbNonRecord UpdateTask(IEnumerable<DbUpdateTaskData<M>> taskList, bool enableSqlTransaction = false, string suffix = null)
        {
            DbNonRecord result = new DbNonRecord();

            if (null == taskList || !taskList.Any())
                return result;

            string dbString = this._dbConnectionStringHandler.GetConnection();
            if (string.IsNullOrEmpty(dbString))
                throw new Exception("dbString is null");

            #region 定义全局参数

            Type modelT = typeof(M);
            string origTableName = this._dbMappingHandler.GetDbTableName(modelT);
            if (!string.IsNullOrEmpty(suffix))
                origTableName = $"{origTableName}{suffix}";
            DbColumnAttribute[] columns = this._dbMappingHandler.GetDbColumnCollection(modelT, d => !d.IsDbGenerated);

            #endregion

            #region 开始循环解析任务

            List<DbUpdateTaskSqlData> sqlDataList = new List<DbUpdateTaskSqlData>();

            foreach (var task in taskList)
            {
                #region 跳过循环特殊条件

                if (null == task.WhereExp || null == task.UpdatePropertys)
                    continue;

                #endregion

                #region 条件解析

                Expression where_func_lambdaExp = null;
                if (task.WhereExp is LambdaExpression)
                {
                    //在方法参数上直接写条件
                    where_func_lambdaExp = task.WhereExp;
                }
                else if (task.WhereExp is MemberExpression)
                {
                    //通过条件组合的模式
                    object lambdaObject = ExpressionCalculater.GetValue(task.WhereExp);
                    where_func_lambdaExp = lambdaObject as Expression;
                }
                else
                {
                    result.AppendError("尚未实现直接解析" + task.WhereExp.NodeType.ToString() + "的特例");
                    return result;
                }

                //解析Where条件
                MysqlWhereScriptResult whereResult = MysqlWhereScriptHandler.ExecuteResolver(where_func_lambdaExp, this._dbMappingHandler, false);
                if (!whereResult.IsAvailable())
                {
                    result.CopyStatus(whereResult);
                    return result;
                }

                #endregion

                #region 更新字段解析

                MysqlUpdateScriptResult updatePropertyResult = null;
                if (task.UpdatePropertys is LambdaExpression && task.UpdatePropertys.Body.NodeType == ExpressionType.MemberInit)
                {
                    updatePropertyResult = MysqlUpdateScriptHandler.ExecuteResolver(task.UpdatePropertys, this._dbMappingHandler);
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

                #endregion

                #region 开始拼装Sql语句

                //获取所有的数据源列
                DbColumnAttribute[] colums = this._dbMappingHandler.GetDbColumnCollection(modelT);

                List<DbParameter> parameters = new List<DbParameter>();
                DbParameter cur_parameter = null;
                StringBuilder sqlBuilder = new StringBuilder();
                sqlBuilder.AppendFormat("update {0} set ", MysqlGrammarRule.GenerateTableWrapped(origTableName));
                foreach (var item in updatePropertyResult.FieldMembers)
                {
                    //自增长的自动跳过
                    if (colums.Any(d => d.PropertyNameMapping == item.PropertyItem.Name && d.IsDbGenerated))
                        continue;

                    string cur_field = colums.First(d => d.PropertyNameMapping == item.PropertyItem.Name).DbColumnName;

                    sqlBuilder.AppendFormat(
                        "{0}={1},",
                        MysqlGrammarRule.GenerateFieldWrapped(cur_field),
                        item.UpdateTextFragment
                    );

                    foreach (var pitem in item.Parameter)
                    {
                        cur_parameter = new MySqlParameter(pitem.Name, pitem.Value);
                        parameters.Add(cur_parameter);
                    }
                }
                sqlBuilder.Replace(",", string.Empty, sqlBuilder.Length - 1, 1);
                if (whereResult != null)
                {
                    sqlBuilder.Append(" where ");
                    sqlBuilder.Append(whereResult.TextScript);
                    foreach (var item in whereResult.Parameters)
                    {
                        cur_parameter = new MySqlParameter(item.Name, item.Value);
                        parameters.Add(cur_parameter);
                    }
                }
                sqlBuilder.Append(";");

                #endregion

                #region 填充SqlDataList

                sqlDataList.Add(new DbUpdateTaskSqlData()
                {
                    SqlText = sqlBuilder.ToString(),
                    SqlParameters = parameters.ToArray()
                });

                #endregion
            }

            #endregion

            #region 执行Sql语句

            using (MySqlConnection connection = new MySqlConnection(dbString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;

                    //尝试打开数据库连结
                    if (this.TryOpenDbConnection(connection, ref result))
                    {
                        //判断是否需要开启事务
                        if (enableSqlTransaction)
                            command.Transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);

                        //开始循环执行Sql
                        foreach (var sql in sqlDataList)
                        {
                            command.CommandText = sql.SqlText;
                            command.Parameters.Clear();
                            command.Parameters.AddRange(sql.SqlParameters);

                            try
                            {
                                result.AffectedRow += command.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                result.AppendError("sql语句执行异常," + command.CommandText);
                                result.AppendException(ex);

                                if (enableSqlTransaction && null != command.Transaction)
                                    command.Transaction.Rollback();

                                command.Dispose();
                                connection.Close();
                                connection.Dispose();

                                return result;
                            }
                        }

                        if (enableSqlTransaction && null != command.Transaction)
                            command.Transaction.Commit();
                    }
                }
            }

            #endregion

            return result;
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

            string dbString = this._dbConnectionStringHandler.GetConnection();
            if (string.IsNullOrEmpty(dbString))
                throw new Exception("dbString is null");

            Type modelT = typeof(M);
            MysqlWhereScriptResult whereResult;
            if (deleteExp != null)
            {
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
                whereResult = MysqlWhereScriptHandler.ExecuteResolver(where_func_lambdaExp, this._dbMappingHandler, false);
                if (!whereResult.IsAvailable())
                {
                    result.CopyStatus(whereResult);
                    return result;
                }

                #endregion

                #region 拼接Sql语句

                string tableName = this._dbMappingHandler.GetDbTableName(modelT);
                if (!string.IsNullOrEmpty(suffix))
                    tableName = $"{tableName}{suffix}";

                List<MySqlParameter> parameters = new List<MySqlParameter>();

                StringBuilder sqlBuilder = new StringBuilder("delete from ");
                sqlBuilder.Append(MysqlGrammarRule.GenerateTableWrapped(tableName));
                sqlBuilder.Append(" where ");
                sqlBuilder.Append(whereResult.TextScript);
                foreach (var item in whereResult.Parameters)
                    parameters.Add(new MySqlParameter(item.Name, item.Value));

                sqlBuilder.Append(";");

                //初始化Debug
                result.DebugInit(sqlBuilder, MysqlGrammarRule.C_ParamChar, parameters.ToArray());

                #endregion

                #region 开始执行Sql语句

                using (MySqlConnection connection = new MySqlConnection(dbString))
                {
                    using (MySqlCommand command = new MySqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = sqlBuilder.ToString();
                        foreach (MySqlParameter item in parameters)
                        {
                            command.Parameters.Add(item);
                        }
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
            else
            {
                result.AppendError("不允许传入null条件进行删除，此行为属于非法行为！");
                return result;
            }
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
            MysqlSentenceResult resolveResult = null;
            List<MySqlParameter> parameters = new List<MySqlParameter>();

            #region 解析表达式条件

            if (exp != null)
            {
                resolveResult = MysqlSentenceHandler.ExecuteResolver(exp, this._dbMappingHandler);
                if (!resolveResult.IsAvailable())
                {
                    result.CopyStatus(resolveResult);
                    return result;
                }
            }

            #endregion

            #region 拼接SQL语句

            string tableName = this._dbMappingHandler.GetDbTableName(modelT);
            if (!string.IsNullOrEmpty(suffix))
                tableName = $"{tableName}{suffix}";

            StringBuilder sqlBuilder = new StringBuilder();
            if (resolveResult == null)
            {
                sqlBuilder.Append("select * from ");
                sqlBuilder.Append(MysqlGrammarRule.GenerateTableWrapped(tableName));
                sqlBuilder.Append(" limit 1,1");
            }
            else
            {
                #region 指定需要查询的字段

                sqlBuilder.Append("select ");
                if (resolveResult.SqlSelectFields == null || resolveResult.SqlSelectFields.Count() <= 0)
                {
                    //如果没有设置要查询的字段，默认查询所有
                    DbColumnAttribute[] fields = this._dbMappingHandler.GetDbColumnCollection(modelT);

                    MysqlSelectField fieldItem = null;
                    foreach (var item in fields)
                    {
                        fieldItem = new MysqlSelectField();
                        fieldItem.DBFieldAsName = item.DbColumnName;
                        fieldItem.DBSelectFragment = item.DbColumnName;
                        fieldItem.IsModelProperty = true;
                        resolveResult.SetSelectField(fieldItem);
                    }
                }
                foreach (var item in resolveResult.SqlSelectFields)
                {
                    if (item.IsModelProperty)
                    {
                        sqlBuilder.Append(MysqlGrammarRule.GenerateFieldWrapped(item.DBSelectFragment));
                        sqlBuilder.Append(" as ");
                        sqlBuilder.Append(MysqlGrammarRule.GenerateFieldWrapped(item.DBFieldAsName));
                        sqlBuilder.Append(",");
                    }
                }
                sqlBuilder.Replace(",", string.Empty, sqlBuilder.Length - 1, 1);
                sqlBuilder.Append(" from ");
                sqlBuilder.Append(MysqlGrammarRule.GenerateTableWrapped(tableName));

                #endregion

                #region 指定Where条件

                if (!string.IsNullOrEmpty(resolveResult.SqlWhereConditionText))
                {
                    sqlBuilder.Append(" where ");
                    sqlBuilder.Append(resolveResult.SqlWhereConditionText);
                }
                //装载参数
                if (resolveResult.SqlQuerylParameters != null && resolveResult.SqlQuerylParameters.Count() > 0)
                    foreach (var item in resolveResult.SqlQuerylParameters)
                        parameters.Add(new MySqlParameter(item.Name, item.Value));

                #endregion

                #region 指定Order条件

                if (!string.IsNullOrEmpty(resolveResult.SqlOrderConditionText))
                {
                    sqlBuilder.Append(" order by ");
                    sqlBuilder.Append(resolveResult.SqlOrderConditionText);
                }

                #endregion
            }
            sqlBuilder.Append(";");

            //初始化Debug
            result.DebugInit(sqlBuilder, MysqlGrammarRule.C_ParamChar, parameters.ToArray());

            #endregion

            #region 执行Sql语句

            using (MySqlConnection connection = new MySqlConnection(dbString))
            {
                using (MySqlCommand command = new MySqlCommand())
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
                        MySqlDataReader reader = this.TryExecuteReader(command, ref result);
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

            MysqlSentenceResult resolveResult = null;
            List<MySqlParameter> parameters = new List<MySqlParameter>();

            #region 解析表达式条件

            if (exp != null)
            {
                resolveResult = MysqlSentenceHandler.ExecuteResolver(exp, this._dbMappingHandler);
                if (!resolveResult.IsAvailable())
                {
                    result.CopyStatus(resolveResult);
                    return result;
                }
            }

            #endregion

            #region 拼接SQL语句

            string tableName = this._dbMappingHandler.GetDbTableName(modelT);
            if (!string.IsNullOrEmpty(suffix))
                tableName = $"{tableName}{suffix}";

            bool count_from_list = false;
            StringBuilder countBuilder = new StringBuilder();
            StringBuilder queryBuilder = new StringBuilder();

            int currentPage;
            int pageSize;
            if (resolveResult == null)
            {
                #region 初始化查询分析中间对象

                // 初始化保证该不为空（下面查询出数据后封装model会用）
                resolveResult = MysqlSentenceResult.Create();

                // 设置分页参数
                currentPage = MysqlSentenceResult.DEFAULT_CURRENTPAGE;
                pageSize = MysqlSentenceResult.DEFAULT_PAGESIZE;
                resolveResult.SetPageCondition(currentPage, pageSize);

                //设置检索的全字段，默认查询所有
                foreach (var item in columns)
                    resolveResult.SetSelectField(new MysqlSelectField
                    {
                        DBFieldAsName = item.DbColumnName,
                        DBSelectFragment = item.DbColumnName,
                        IsModelProperty = true
                    });

                #endregion

                #region 强制设置数据结果集统计从列表中加载

                count_from_list = true;

                #endregion

                #region 拼接构造查询语句

                queryBuilder.Append("select * from ");
                queryBuilder.Append(MysqlGrammarRule.GenerateTableWrapped(tableName));
                queryBuilder.AppendFormat(" limit 0,{0}", pageSize);
                queryBuilder.Append(";");

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
                    currentPage = MysqlSentenceResult.DEFAULT_CURRENTPAGE;
                if (pageSize < 1)
                    pageSize = MysqlSentenceResult.DEFAULT_PAGESIZE;

                #endregion

                #region 特殊情况处理（若currpage=1并且pageSize=int.MaxValue,那么就是查询所有的数据,所以可设置不启动分页）

                var enablePaging = true;
                if (1 == currentPage && int.MaxValue == pageSize)
                    enablePaging = false;

                #endregion

                #region 启用分页则需要根据条件查询总数量(拼接构造统计语句)

                if (enablePaging)
                {
                    countBuilder.Append("select count(1) from ");
                    countBuilder.Append(MysqlGrammarRule.GenerateTableWrapped(tableName));
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

                #region Select XX From

                queryBuilder.Append("select ");
                if (resolveResult.SqlSelectFields == null || resolveResult.SqlSelectFields.Count() <= 0)
                {
                    //如果没有设置要查询的字段，默认查询所有
                    foreach (var item in columns)
                        resolveResult.SetSelectField(new MysqlSelectField
                        {
                            DBFieldAsName = item.DbColumnName,
                            DBSelectFragment = item.DbColumnName,
                            IsModelProperty = true
                        });
                }
                foreach (var item in resolveResult.SqlSelectFields)
                {
                    if (item.IsModelProperty)
                    {
                        queryBuilder.Append(MysqlGrammarRule.GenerateFieldWrapped(item.DBSelectFragment));
                        queryBuilder.Append(" as ");
                        queryBuilder.Append(MysqlGrammarRule.GenerateFieldWrapped(item.DBFieldAsName));
                        queryBuilder.Append(",");
                    }
                }
                queryBuilder.Replace(",", string.Empty, queryBuilder.Length - 1, 1);
                queryBuilder.Append(" from ");
                queryBuilder.Append(MysqlGrammarRule.GenerateTableWrapped(tableName));

                #endregion

                #region 指定Where条件

                if (!string.IsNullOrEmpty(resolveResult.SqlWhereConditionText))
                {
                    queryBuilder.Append(" where ");
                    queryBuilder.Append(resolveResult.SqlWhereConditionText);
                }
                if (resolveResult.SqlQuerylParameters != null && resolveResult.SqlQuerylParameters.Count() > 0)
                    foreach (var item in resolveResult.SqlQuerylParameters)
                        parameters.Add(new MySqlParameter(item.Name, item.Value));

                #endregion

                #region 指定Order条件

                if (!string.IsNullOrEmpty(resolveResult.SqlOrderConditionText))
                {
                    queryBuilder.Append(" order by ");
                    queryBuilder.Append(resolveResult.SqlOrderConditionText);
                }

                #endregion

                #region 指定分页数

                queryBuilder.AppendFormat(" limit {0},{1}", (pageSize * (currentPage - 1)), pageSize);
                queryBuilder.Append(";");

                #endregion

                #endregion
            }

            //初始化Debug
            var debugBuilder = new StringBuilder();
            if (countBuilder.Length > 0)
                debugBuilder.Append(countBuilder);
            if (queryBuilder.Length > 0)
                debugBuilder.Append(queryBuilder);
            result.DebugInit(debugBuilder, MysqlGrammarRule.C_ParamChar, parameters.ToArray());

            #endregion

            #region 执行Sql语句

            using (MySqlConnection connection = new MySqlConnection(dbString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    if (parameters.Count > 0)
                        foreach (var item in parameters)
                            command.Parameters.Add(item);


                    //尝试打开数据库链接
                    if (this.TryOpenDbConnection(connection, ref result))
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
                            MySqlDataReader reader = this.TryExecuteReader(command, ref result);
                            if (reader != null && reader.HasRows)
                            {
                                result.Record = new List<M>();
                                M entity = default;
                                while (reader.Read())
                                {
                                    entity = this.AutoFillModel(reader, modelT, resolveResult.SqlSelectFields);
                                    result.Record.Add(entity);
                                }
                                //释放资源，关闭连结
                                this.DisposeReader(reader);
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
            MysqlSentenceResult resolveResult = null;
            List<MySqlParameter> parameters = new List<MySqlParameter>();

            #region 解析表达式条件

            if (exp != null)
            {
                resolveResult = MysqlSentenceHandler.ExecuteResolver(exp, this._dbMappingHandler);
                if (!resolveResult.IsAvailable())
                {
                    result.CopyStatus(resolveResult);
                    return result;
                }
            }
            else
            {
                result.AppendError("表达式exp不允许为空");
                return result;
            }

            #endregion

            StringBuilder sqlBuilder = new StringBuilder("select ");
            if (resolveResult.SqlSelectFields != null && resolveResult.SqlSelectFields.Count() > 0)
            {
                #region 拼接Sql语句

                string tableName = this._dbMappingHandler.GetDbTableName(modelT);
                if (!string.IsNullOrEmpty(suffix))
                    tableName = $"{tableName}{suffix}";

                foreach (var item in resolveResult.SqlSelectFields.OrderBy(d => d.IsModelProperty).OrderBy(d => d.DBFieldAsName))
                {
                    sqlBuilder.Append(MysqlGrammarRule.GenerateFieldWrapped(item.DBSelectFragment));
                    sqlBuilder.Append(" as ");
                    sqlBuilder.Append(MysqlGrammarRule.GenerateFieldWrapped(item.DBFieldAsName));
                    sqlBuilder.Append(",");
                }
                sqlBuilder.Replace(",", "", sqlBuilder.Length - 1, 1);
                sqlBuilder.Append(" from ");
                sqlBuilder.Append(MysqlGrammarRule.GenerateTableWrapped(tableName));

                if (!string.IsNullOrEmpty(resolveResult.SqlWhereConditionText))
                {
                    sqlBuilder.Append(" where ");
                    sqlBuilder.Append(resolveResult.SqlWhereConditionText);
                }
                if (resolveResult.SqlQuerylParameters != null && resolveResult.SqlQuerylParameters.Count() > 0)
                    foreach (var item in resolveResult.SqlQuerylParameters)
                        parameters.Add(new MySqlParameter(item.Name, item.Value));

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
                result.DebugInit(sqlBuilder, MysqlGrammarRule.C_ParamChar, parameters.ToArray());

                #endregion

                #region 执行Sql语句

                using (MySqlConnection connection = new MySqlConnection(dbString))
                {
                    using (MySqlCommand command = new MySqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = sqlBuilder.ToString();
                        if (parameters.Count > 0)
                            foreach (var item in parameters)
                                command.Parameters.Add(item);


                        //尝试打开数据库链接
                        if (this.TryOpenDbConnection(connection, ref result))
                        {
                            //尝试执行语句返回DataReader
                            MySqlDataReader reader = this.TryExecuteReader(command, ref result);
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
                                this.DisposeReader(reader);
                            }
                        }
                    }
                }

                #endregion

                return result;
            }
            else
            {
                #region 必须至少指定一个运算模式，例如:Count,Sum,Max,Min等

                result.AppendError("必须至少指定一个运算模式，例如:Count,Sum,Max,Min等");
                return result;

                #endregion
            }
        }

        #endregion

        #region IDBAsynceRepository<M>

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

            if (model != null)
            {
                #region 判断主键数量

                DbColumnAttribute[] columns = this._dbMappingHandler.GetDbColumnCollection(modelT);
                if (null == columns || columns.Length <= 0)
                {
                    result.AppendError(string.Format("{0}类型无字段映射", modelT.FullName));
                    return result;
                }

                DbColumnAttribute[] setPrimaryKeys = columns.Where(d => d.IsDbGenerated).ToArray();
                if (setPrimaryKeys.Count() > 1)
                {
                    result.AppendError("暂不允许使用双自增长键！请最多设置一列为自增长主键！");
                    return result;
                }

                #endregion

                //需要设置参数插入的字段
                DbColumnAttribute[] setFields = columns.Where(d => !d.IsDbGenerated).ToArray();
                if (setFields.Count() > 0)
                {
                    List<MySqlParameter> parameters = new List<MySqlParameter>();

                    #region 拼接Sql语句

                    string tableName = this._dbMappingHandler.GetDbTableName(modelT);
                    if (!string.IsNullOrEmpty(suffix))
                        tableName = $"{tableName}{suffix}";

                    StringBuilder sqlBuilder = new StringBuilder("insert into ");
                    sqlBuilder.Append(MysqlGrammarRule.GenerateTableWrapped(tableName));
                    sqlBuilder.Append(" (");
                    foreach (var item in setFields.Select(d => d.DbColumnName))
                    {
                        sqlBuilder.AppendFormat("{0},", MysqlGrammarRule.GenerateFieldWrapped(item));
                    }
                    sqlBuilder.Replace(",", ")", sqlBuilder.Length - 1, 1);
                    sqlBuilder.Append(" values ");
                    sqlBuilder.Append("(");
                    foreach (var item in setFields)
                    {
                        string parameterName = string.Format("{0}", item.DbColumnName);
                        PropertyInfo p_info = this._dbMappingHandler.GetPropertySingle(modelT, item.DbColumnName);
                        object parameterValue = p_info.GetValue(model, null);

                        MySqlDbType dbType = this.GetDbtype(item.DbType);

                        MySqlParameter paremter = new MySqlParameter(MysqlGrammarRule.GenerateParamName(parameterName), dbType);
                        paremter.Value = parameterValue;
                        parameters.Add(paremter);

                        sqlBuilder.Append(MysqlGrammarRule.GenerateParamName(parameterName));
                        sqlBuilder.Append(",");
                    }
                    sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
                    sqlBuilder.Append(");");
                    sqlBuilder.Append("select @@identity;");
                    //初始化Debug
                    result.DebugInit(sqlBuilder, MysqlGrammarRule.C_ParamChar, parameters.ToArray());

                    #endregion

                    #region 执行Sql语句

                    using (MySqlConnection connection = new MySqlConnection(dbString))
                    {
                        using (MySqlCommand command = new MySqlCommand())
                        {
                            command.Connection = connection;
                            command.CommandText = sqlBuilder.ToString();
                            foreach (MySqlParameter item in parameters)
                                command.Parameters.Add(item);

                            //尝试打开数据库连结
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

                            if (setPrimaryKeys != null && setPrimaryKeys.Count() > 0)
                            {
                                try
                                {
                                    object dbVal = await command.ExecuteScalarAsync();
                                    PropertyInfo pinfo = this._dbMappingHandler.GetPropertySingle(modelT, setPrimaryKeys.First().DbColumnName);
                                    if (pinfo != null && dbVal != DBNull.Value)
                                    {
                                        dbVal = Convert.ChangeType(dbVal, pinfo.PropertyType);
                                        pinfo.SetValue(model, dbVal, null);
                                    }
                                    result.Record = model;
                                }
                                catch (Exception ex)
                                {
                                    result.Record = default;
                                    result.AppendException(ex);

                                    await command.DisposeAsync();
                                    await connection.CloseAsync();
                                    await connection.DisposeAsync();

                                    return result;
                                }
                            }
                            else
                            {
                                try
                                {
                                    int affectedRow = await command.ExecuteNonQueryAsync();
                                    if (affectedRow > 0)
                                        result.Record = model;
                                }
                                catch (Exception ex)
                                {
                                    result.Record = default;
                                    result.AppendException(ex);

                                    await command.DisposeAsync();
                                    await connection.CloseAsync();
                                    await connection.DisposeAsync();

                                    return result;
                                }
                            }
                        }
                    }

                    #endregion

                    return result;
                }
                else
                {
                    result.AppendError("插入的表仅有自增长列或没有指定任何列");
                    return result;
                }
            }
            else
            {
                result.AppendError("插入数据时候的Model为空");
                return result;
            }
        }

        /// <summary>
        /// 批露插入数据
        /// </summary>
        /// <param name="modelList"></param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public async Task<DbCollectionRecord<M>> InsertBatchAsync(IEnumerable<M> modelList, string suffix = null)
        {
            DbCollectionRecord<M> result = new DbCollectionRecord<M>();
            result.Record = new List<M>();

            if (null == modelList || !modelList.Any())
            {
                result.AppendError("无数据需要进行批露插入");
                return result;
            }

            string dbString = this._dbConnectionStringHandler.GetConnection();
            if (string.IsNullOrEmpty(dbString))
                throw new Exception("dbString is null");

            Type modelT = typeof(M);
            string tableName = this._dbMappingHandler.GetDbTableName(modelT);
            if (!string.IsNullOrEmpty(suffix))
                tableName = $"{tableName}{suffix}";

            #region 判断主键数量

            DbColumnAttribute[] columns = this._dbMappingHandler.GetDbColumnCollection(modelT);
            if (null == columns || columns.Length <= 0)
            {
                result.AppendError(string.Format("{0}类型无字段映射", modelT.FullName));
                return result;
            }

            DbColumnAttribute[] setPrimaryKeys = columns.Where(d => d.IsDbGenerated).ToArray();
            if (setPrimaryKeys.Count() > 1)
            {
                result.AppendError("暂不允许使用双自增长键！请最多设置一列为自增长主键！");
                return result;
            }

            #endregion

            #region 获取所有非自增长列元数据

            DbColumnAttribute[] setFields = columns.Where(d => !d.IsDbGenerated).ToArray();
            if (!setFields.Any())
            {
                result.AppendError("插入的表仅有自增长列或没有指定任何列");
                return result;
            }

            #endregion

            #region 循环构建待插入数据脚本（循环拼接SQL语句）

            StringBuilder sqlBuilder = null;
            List<MySqlParameter> parameters = null;
            MySqlParameter paremter = null;
            Dictionary<M, KeyValuePair<StringBuilder, List<MySqlParameter>>> sqlList = new Dictionary<M, KeyValuePair<StringBuilder, List<MySqlParameter>>>();

            foreach (var model in modelList)
            {
                parameters = new List<MySqlParameter>();

                #region 拼接Sql语句

                sqlBuilder = new StringBuilder("insert into ");
                sqlBuilder.Append(MysqlGrammarRule.GenerateTableWrapped(tableName));
                sqlBuilder.Append(" (");
                foreach (var item in setFields.Select(d => d.DbColumnName))
                {
                    sqlBuilder.AppendFormat("{0},", MysqlGrammarRule.GenerateFieldWrapped(item));
                }
                sqlBuilder.Replace(",", ")", sqlBuilder.Length - 1, 1);
                sqlBuilder.Append(" values ");
                sqlBuilder.Append("(");
                foreach (var item in setFields)
                {
                    string parameterName = string.Format("{0}", item.DbColumnName);
                    PropertyInfo p_info = this._dbMappingHandler.GetPropertySingle(modelT, item.DbColumnName);
                    object parameterValue = p_info.GetValue(model, null);

                    MySqlDbType dbType = this.GetDbtype(item.DbType);

                    paremter = new MySqlParameter(MysqlGrammarRule.GenerateParamName(parameterName), dbType);
                    paremter.Value = parameterValue;
                    parameters.Add(paremter);

                    sqlBuilder.Append(MysqlGrammarRule.GenerateParamName(parameterName));
                    sqlBuilder.Append(",");
                }
                sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
                sqlBuilder.Append(");");
                sqlBuilder.Append("select @@identity;");

                #endregion

                sqlList.Add(model, new KeyValuePair<StringBuilder, List<MySqlParameter>>(sqlBuilder, parameters));
            }

            #endregion

            #region 执行Sql语句

            int sqlcount = 0;
            using (MySqlConnection connection = new MySqlConnection(dbString))
            {
                //尝试打开数据库连结
                try
                {
                    await connection.OpenAsync();
                }
                catch (Exception ex)
                {
                    result.AppendException(ex);

                    await connection.CloseAsync();
                    await connection.DisposeAsync();

                    return result;
                }

                using (MySqlCommand command = new MySqlCommand())
                {
                    //开始事务
                    MySqlTransaction tx = await connection.BeginTransactionAsync();

                    //初始化赋值Command
                    command.Connection = connection;
                    command.Transaction = tx;

                    //开始循环事务插入
                    foreach (var sqlNode in sqlList)
                    {
                        command.Parameters.Clear();
                        command.CommandText = sqlNode.Value.Key.ToString();
                        foreach (MySqlParameter item in sqlNode.Value.Value)
                            command.Parameters.Add(item);

                        if (null != setPrimaryKeys && setPrimaryKeys.Any())
                        {
                            try
                            {
                                object dbVal = await command.ExecuteScalarAsync();
                                PropertyInfo pinfo = this._dbMappingHandler.GetPropertySingle(modelT, setPrimaryKeys.First().DbColumnName);
                                if (pinfo != null && dbVal != DBNull.Value)
                                {
                                    dbVal = Convert.ChangeType(dbVal, pinfo.PropertyType);
                                    pinfo.SetValue(sqlNode.Key, dbVal, null);
                                }
                                result.Record.Add(sqlNode.Key);
                            }
                            catch (Exception ex)
                            {
                                await tx.RollbackAsync();
                                result.Record.Clear();
                                result.AppendException(ex);

                                await command.DisposeAsync();
                                await connection.CloseAsync();
                                await connection.DisposeAsync();

                                return result;
                            }
                        }
                        else
                        {
                            try
                            {
                                int affectedRow = await command.ExecuteNonQueryAsync();
                                if (affectedRow > 0)
                                    result.Record.Add(sqlNode.Key);
                            }
                            catch (Exception ex)
                            {
                                await tx.RollbackAsync();
                                result.Record.Clear();
                                result.AppendException(ex);

                                await command.DisposeAsync();
                                await connection.CloseAsync();
                                await connection.DisposeAsync();

                                return result;
                            }
                        }

                        //已执行的SQL++
                        sqlcount++;

                        //按批次进行事务提交
                        if (sqlcount > 0 && (sqlcount % c_batchTotal == 0 || sqlcount == sqlList.Count))
                        {
                            await tx.CommitAsync();

                            if (sqlcount != sqlList.Count)
                                tx = await connection.BeginTransactionAsync();
                        }
                    }
                }
            }

            #endregion

            return result;
        }

        /// <summary>
        /// 更新操作（局部更新）
        /// </summary>
        /// <param name="whereExp">需要被更新的条件</param>
        /// <param name="updatePropertys">需要被替换或更新的属性</param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public async Task<DbNonRecord> UpdateAsync(Expression<Func<M, bool>> whereExp, Expression<Func<M, M>> updatePropertys, string suffix = null)
        {
            DbNonRecord result = new DbNonRecord();

            string dbString = this._dbConnectionStringHandler.GetConnection();
            if (string.IsNullOrEmpty(dbString))
                throw new Exception("dbString is null");

            Type modelT = typeof(M);
            MysqlWhereScriptResult whereResult = null;
            MysqlUpdateScriptResult updatePropertyResult = null;

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
                whereResult = MysqlWhereScriptHandler.ExecuteResolver(where_func_lambdaExp, this._dbMappingHandler, false);
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
                    updatePropertyResult = MysqlUpdateScriptHandler.ExecuteResolver(updatePropertys, this._dbMappingHandler);
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

            string tableName = this._dbMappingHandler.GetDbTableName(modelT);
            if (!string.IsNullOrEmpty(suffix))
                tableName = $"{tableName}{suffix}";

            List<MySqlParameter> parameters = new List<MySqlParameter>();

            StringBuilder sqlBuilder = new StringBuilder("update ");
            sqlBuilder.Append(MysqlGrammarRule.GenerateTableWrapped(tableName));
            sqlBuilder.Append(" set ");
            foreach (var item in updatePropertyResult.FieldMembers)
            {
                //自增长的自动跳过
                if (colums.Any(d => d.PropertyNameMapping == item.PropertyItem.Name && d.IsDbGenerated))
                    continue;

                string cur_field = colums.First(d => d.PropertyNameMapping == item.PropertyItem.Name).DbColumnName;

                sqlBuilder.Append(" ");
                sqlBuilder.Append(MysqlGrammarRule.GenerateFieldWrapped(cur_field));
                sqlBuilder.Append("=");
                sqlBuilder.Append(item.UpdateTextFragment);
                sqlBuilder.Append(",");

                foreach (var pitem in item.Parameter)
                    parameters.Add(new MySqlParameter(pitem.Name, pitem.Value));
            }
            sqlBuilder.Replace(",", " ", sqlBuilder.Length - 1, 1);
            if (whereResult != null)
            {
                sqlBuilder.Append("where ");
                sqlBuilder.Append(whereResult.TextScript);
                foreach (var item in whereResult.Parameters)
                    parameters.Add(new MySqlParameter(item.Name, item.Value));
            }
            sqlBuilder.Append(";");

            //初始化Debug
            result.DebugInit(sqlBuilder, MysqlGrammarRule.C_ParamChar, parameters.ToArray());

            #endregion

            #region 执行Sql语句

            using (MySqlConnection connection = new MySqlConnection(dbString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = sqlBuilder.ToString();
                    foreach (MySqlParameter item in parameters)
                        command.Parameters.Add(item);

                    //尝试打开数据库连结
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
                        result.AffectedRow = await command.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        result.AppendError("sql语句执行异常," + command.CommandText);
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
        /// 更新操作（整体更新）
        /// </summary>
        /// <param name="whereExp">需要被更新的条件</param>
        /// <param name="model">需要被整体替换的实体</param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public async Task<DbNonRecord> UpdateAsync(Expression<Func<M, bool>> whereExp, M model, string suffix = null)
        {
            DbNonRecord result = new DbNonRecord();

            string dbString = this._dbConnectionStringHandler.GetConnection();
            if (string.IsNullOrEmpty(dbString))
                throw new Exception("dbString is null");

            Type modelT = typeof(M);
            MysqlWhereScriptResult whereResult = null;

            #region 验证需要被修改的实体是否为null

            if (model == null)
            {
                result.AppendError("修改数据时候的Model为空");
                return result;
            }

            #endregion

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

                //执行where解析
                whereResult = MysqlWhereScriptHandler.ExecuteResolver(where_func_lambdaExp, this._dbMappingHandler, false);
                if (!whereResult.IsAvailable())
                {
                    result.CopyStatus(whereResult);
                    return result;
                }
            }

            #endregion

            #region 开始拼接Sql语句

            DbColumnAttribute[] columns = this._dbMappingHandler.GetDbColumnCollection(modelT, d => !d.IsDbGenerated);

            string tableName = this._dbMappingHandler.GetDbTableName(modelT);
            if (!string.IsNullOrEmpty(suffix))
                tableName = $"{tableName}{suffix}";

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            MySqlParameter cur_parameter = null;
            StringBuilder sqlBuilder = new StringBuilder("update ");
            sqlBuilder.Append(MysqlGrammarRule.GenerateTableWrapped(tableName));
            sqlBuilder.Append(" set ");
            foreach (var item in columns)
            {
                PropertyInfo p = modelT.GetProperty(item.PropertyNameMapping);
                if (p != null)
                {
                    string parameterName = string.Format("set_{0}", item.DbColumnName);
                    object parameterVal = p.GetValue(model, null);

                    sqlBuilder.Append(MysqlGrammarRule.GenerateFieldWrapped(item.DbColumnName));
                    sqlBuilder.Append("=");
                    sqlBuilder.Append(MysqlGrammarRule.GenerateParamName(parameterName));
                    sqlBuilder.Append(",");

                    cur_parameter = new MySqlParameter(MysqlGrammarRule.GenerateParamName(parameterName), parameterVal);
                    parameters.Add(cur_parameter);
                }
            }
            sqlBuilder.Replace(",", " ", sqlBuilder.Length - 1, 1);
            if (whereResult != null)
            {
                sqlBuilder.Append("where ");
                sqlBuilder.Append(whereResult.TextScript);
                foreach (var item in whereResult.Parameters)
                    parameters.Add(new MySqlParameter(item.Name, item.Value));
            }
            sqlBuilder.Append(";");

            //初始化Debug
            result.DebugInit(sqlBuilder, MysqlGrammarRule.C_ParamChar, parameters.ToArray());

            #endregion

            #region 执行Sql语句

            using (MySqlConnection connection = new MySqlConnection(dbString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = sqlBuilder.ToString();
                    foreach (MySqlParameter item in parameters)
                        command.Parameters.Add(item);

                    //尝试打开数据库连结
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
                        result.AffectedRow = await command.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        result.AppendError("sql语句执行异常," + command.CommandText);
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
        /// 批量更新操作
        /// </summary>
        /// <param name="modelList"></param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public async Task<DbNonRecord> UpdateBatchAsync(IEnumerable<M> modelList, string suffix = null)
        {
            DbNonRecord result = new DbNonRecord();

            if (null == modelList || !modelList.Any())
                return result;

            string dbString = this._dbConnectionStringHandler.GetConnection();
            if (string.IsNullOrEmpty(dbString))
                throw new Exception("dbString is null");

            #region 临时变量

            Type modelT = typeof(M);
            string origTableName = this._dbMappingHandler.GetDbTableName(modelT);
            if (!string.IsNullOrEmpty(suffix))
                origTableName = $"{origTableName}{suffix}";
            DbColumnAttribute[] columns = this._dbMappingHandler.GetDbColumnCollection(modelT);

            #endregion

            #region 开始拼接Sql语句

            StringBuilder sqlBuilder = null;
            List<DbParameter> sqlParameter = null;
            List<DbUpdateTaskSqlData> sqlDataList = new List<DbUpdateTaskSqlData>();

            foreach (var model in modelList)
            {
                //构造循环builder
                sqlBuilder = new StringBuilder();
                sqlParameter = new List<DbParameter>();

                //拼接SQL + 填充参数化参数部分
                sqlBuilder.AppendFormat("update {0} set ", MysqlGrammarRule.GenerateTableWrapped(origTableName));
                foreach (var item in columns.Where(d => !d.IsDbGenerated))
                {
                    PropertyInfo p = modelT.GetProperty(item.PropertyNameMapping);
                    if (null == p)
                        continue;

                    string each_paramName = string.Format("set_{0}", item.DbColumnName);
                    object each_paramVal = p.GetValue(model, null);

                    sqlBuilder.AppendFormat(
                        "{0}={1},",
                        MysqlGrammarRule.GenerateFieldWrapped(item.DbColumnName),
                        MysqlGrammarRule.GenerateParamName(each_paramName)
                    );
                    sqlParameter.Add(new MySqlParameter(MysqlGrammarRule.GenerateParamName(each_paramName), each_paramVal));
                }
                sqlBuilder.Replace(",", string.Empty, sqlBuilder.Length - 1, 1);
                sqlBuilder.Append(" where ");
                if (!columns.Any(d => d.IsDbPrimaryKey))
                {
                    result.AppendError(string.Format("表{0}尚未设置主键", origTableName));
                    return result;
                }
                else
                {
                    foreach (var item in columns.Where(d => d.IsDbPrimaryKey))
                    {
                        PropertyInfo p = modelT.GetProperty(item.PropertyNameMapping);
                        if (null == p)
                            continue;

                        object each_pkv = p.GetValue(model, null);

                        sqlBuilder.AppendFormat("{0}={1} and ", item.DbColumnName, each_pkv);
                    }
                    sqlBuilder.Remove(sqlBuilder.Length - 4, 4);
                }
                sqlBuilder.Append(";");

                sqlDataList.Add(new DbUpdateTaskSqlData()
                {
                    SqlText = sqlBuilder.ToString(),
                    SqlParameters = sqlParameter.ToArray()
                });
            }

            #endregion

            #region 执行Sql语句

            using (MySqlConnection connection = new MySqlConnection(dbString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    //设置command连接字符串
                    command.Connection = connection;

                    //尝试打开数据库连结
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

                    //开始循环执行Sql
                    foreach (var sql in sqlDataList)
                    {
                        command.CommandText = sql.SqlText;
                        command.Parameters.Clear();
                        command.Parameters.AddRange(sql.SqlParameters);

                        try
                        {
                            result.AffectedRow += await command.ExecuteNonQueryAsync();
                        }
                        catch (Exception ex)
                        {
                            result.AppendError("sql语句执行异常," + command.CommandText);
                            result.AppendException(ex);

                            await command.DisposeAsync();
                            await connection.CloseAsync();
                            await connection.DisposeAsync();

                            return result;
                        }
                    }
                }
            }

            #endregion

            return result;
        }

        /// <summary>
        /// 批量更新任务
        /// </summary>
        /// <param name="taskList"></param>
        /// <param name="enableSqlTransaction"></param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public async Task<DbNonRecord> UpdateTaskAsync(IEnumerable<DbUpdateTaskData<M>> taskList, bool enableSqlTransaction = false, string suffix = null)
        {
            DbNonRecord result = new DbNonRecord();

            if (null == taskList || !taskList.Any())
                return result;

            string dbString = this._dbConnectionStringHandler.GetConnection();
            if (string.IsNullOrEmpty(dbString))
                throw new Exception("dbString is null");

            #region 定义全局参数

            Type modelT = typeof(M);
            string origTableName = this._dbMappingHandler.GetDbTableName(modelT);
            if (!string.IsNullOrEmpty(suffix))
                origTableName = $"{origTableName}{suffix}";
            DbColumnAttribute[] columns = this._dbMappingHandler.GetDbColumnCollection(modelT, d => !d.IsDbGenerated);

            #endregion

            #region 开始循环解析任务

            List<DbUpdateTaskSqlData> sqlDataList = new List<DbUpdateTaskSqlData>();

            foreach (var task in taskList)
            {
                #region 跳过循环特殊条件

                if (null == task.WhereExp || null == task.UpdatePropertys)
                    continue;

                #endregion

                #region 条件解析

                Expression where_func_lambdaExp = null;
                if (task.WhereExp is LambdaExpression)
                {
                    //在方法参数上直接写条件
                    where_func_lambdaExp = task.WhereExp;
                }
                else if (task.WhereExp is MemberExpression)
                {
                    //通过条件组合的模式
                    object lambdaObject = ExpressionCalculater.GetValue(task.WhereExp);
                    where_func_lambdaExp = lambdaObject as Expression;
                }
                else
                {
                    result.AppendError("尚未实现直接解析" + task.WhereExp.NodeType.ToString() + "的特例");
                    return result;
                }

                //解析Where条件
                MysqlWhereScriptResult whereResult = MysqlWhereScriptHandler.ExecuteResolver(where_func_lambdaExp, this._dbMappingHandler, false);
                if (!whereResult.IsAvailable())
                {
                    result.CopyStatus(whereResult);
                    return result;
                }

                #endregion

                #region 更新字段解析

                MysqlUpdateScriptResult updatePropertyResult = null;
                if (task.UpdatePropertys is LambdaExpression && task.UpdatePropertys.Body.NodeType == ExpressionType.MemberInit)
                {
                    updatePropertyResult = MysqlUpdateScriptHandler.ExecuteResolver(task.UpdatePropertys, this._dbMappingHandler);
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

                #endregion

                #region 开始拼装Sql语句

                //获取所有的数据源列
                DbColumnAttribute[] colums = this._dbMappingHandler.GetDbColumnCollection(modelT);

                List<DbParameter> parameters = new List<DbParameter>();

                StringBuilder sqlBuilder = new StringBuilder();
                sqlBuilder.AppendFormat("update {0} set ", MysqlGrammarRule.GenerateTableWrapped(origTableName));
                foreach (var item in updatePropertyResult.FieldMembers)
                {
                    //自增长的自动跳过
                    if (colums.Any(d => d.PropertyNameMapping == item.PropertyItem.Name && d.IsDbGenerated))
                        continue;

                    string cur_field = colums.First(d => d.PropertyNameMapping == item.PropertyItem.Name).DbColumnName;

                    sqlBuilder.AppendFormat(
                        "{0}={1},",
                        MysqlGrammarRule.GenerateFieldWrapped(cur_field),
                        item.UpdateTextFragment
                    );

                    foreach (var pitem in item.Parameter)
                        parameters.Add(new MySqlParameter(pitem.Name, pitem.Value));
                }
                sqlBuilder.Replace(",", string.Empty, sqlBuilder.Length - 1, 1);
                if (whereResult != null)
                {
                    sqlBuilder.Append(" where ");
                    sqlBuilder.Append(whereResult.TextScript);
                    foreach (var item in whereResult.Parameters)
                        parameters.Add(new MySqlParameter(item.Name, item.Value));
                }
                sqlBuilder.Append(";");

                #endregion

                #region 填充SqlDataList

                sqlDataList.Add(new DbUpdateTaskSqlData()
                {
                    SqlText = sqlBuilder.ToString(),
                    SqlParameters = parameters.ToArray()
                });

                #endregion
            }

            #endregion

            #region 执行Sql语句

            using (MySqlConnection connection = new MySqlConnection(dbString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;

                    //尝试打开数据库连结
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

                    //判断是否需要开启事务
                    if (enableSqlTransaction)
                        command.Transaction = await connection.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

                    //开始循环执行Sql
                    foreach (var sql in sqlDataList)
                    {
                        command.CommandText = sql.SqlText;
                        command.Parameters.Clear();
                        command.Parameters.AddRange(sql.SqlParameters);

                        try
                        {
                            result.AffectedRow += await command.ExecuteNonQueryAsync();
                        }
                        catch (Exception ex)
                        {
                            result.AppendError("sql语句执行异常," + command.CommandText);
                            result.AppendException(ex);

                            if (enableSqlTransaction && null != command.Transaction)
                                await command.Transaction.RollbackAsync();

                            await command.DisposeAsync();
                            await connection.CloseAsync();
                            await connection.DisposeAsync();

                            return result;
                        }
                    }

                    if (enableSqlTransaction && null != command.Transaction)
                        await command.Transaction.CommitAsync();
                }
            }

            #endregion

            return result;
        }

        /// <summary>
        /// 删除操作
        /// </summary>
        /// <param name="deleteExp">删除条件</param>
        /// <param name="suffix">分表后缀,无分表不传该字段</param>
        /// <returns></returns>
        public async Task<DbNonRecord> DeleteAsync(Expression<Func<M, bool>> deleteExp, string suffix = null)
        {
            DbNonRecord result = new DbNonRecord();

            string dbString = this._dbConnectionStringHandler.GetConnection();
            if (string.IsNullOrEmpty(dbString))
                throw new Exception("dbString is null");

            Type modelT = typeof(M);
            MysqlWhereScriptResult whereResult;
            if (deleteExp != null)
            {
                #region 解析条件语句

                Expression where_func_lambdaExp = null;
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
                whereResult = MysqlWhereScriptHandler.ExecuteResolver(where_func_lambdaExp, this._dbMappingHandler, false);
                if (!whereResult.IsAvailable())
                {
                    result.CopyStatus(whereResult);
                    return result;
                }

                #endregion

                #region 拼接Sql语句

                string tableName = this._dbMappingHandler.GetDbTableName(modelT);
                if (!string.IsNullOrEmpty(suffix))
                    tableName = $"{tableName}{suffix}";

                List<MySqlParameter> parameters = new List<MySqlParameter>();

                StringBuilder sqlBuilder = new StringBuilder("delete from ");
                sqlBuilder.Append(MysqlGrammarRule.GenerateTableWrapped(tableName));
                sqlBuilder.Append(" where ");
                sqlBuilder.Append(whereResult.TextScript);
                foreach (var item in whereResult.Parameters)
                    parameters.Add(new MySqlParameter(item.Name, item.Value));
                sqlBuilder.Append(";");

                //初始化Debug
                result.DebugInit(sqlBuilder, MysqlGrammarRule.C_ParamChar, parameters.ToArray());

                #endregion

                #region 开始执行Sql语句

                using (MySqlConnection connection = new MySqlConnection(dbString))
                {
                    using (MySqlCommand command = new MySqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = sqlBuilder.ToString();
                        foreach (MySqlParameter item in parameters)
                            command.Parameters.Add(item);

                        //尝试打开数据库连结
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
                            result.AffectedRow = await command.ExecuteNonQueryAsync();
                        }
                        catch (Exception ex)
                        {
                            result.AppendError("sql语句执行异常," + command.CommandText);
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
            else
            {
                result.AppendError("不允许传入null条件进行删除，此行为属于非法行为！");
                return result;
            }
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
            MysqlSentenceResult resolveResult = null;
            List<MySqlParameter> parameters = new List<MySqlParameter>();

            #region 解析表达式条件

            if (exp != null)
            {
                resolveResult = MysqlSentenceHandler.ExecuteResolver(exp, this._dbMappingHandler);
                if (!resolveResult.IsAvailable())
                {
                    result.CopyStatus(resolveResult);
                    return result;
                }
            }

            #endregion

            #region 拼接SQL语句

            string tableName = this._dbMappingHandler.GetDbTableName(modelT);
            if (!string.IsNullOrEmpty(suffix))
                tableName = $"{tableName}{suffix}";

            StringBuilder sqlBuilder = new StringBuilder();
            if (resolveResult == null)
            {
                sqlBuilder.Append("select * from ");
                sqlBuilder.Append(MysqlGrammarRule.GenerateTableWrapped(tableName));
                sqlBuilder.Append(" limit 1,1");
            }
            else
            {
                #region 指定需要查询的字段

                sqlBuilder.Append("select ");
                if (resolveResult.SqlSelectFields == null || resolveResult.SqlSelectFields.Count() <= 0)
                {
                    //如果没有设置要查询的字段，默认查询所有
                    DbColumnAttribute[] fields = this._dbMappingHandler.GetDbColumnCollection(modelT);

                    MysqlSelectField fieldItem = null;
                    foreach (var item in fields)
                    {
                        fieldItem = new MysqlSelectField();
                        fieldItem.DBFieldAsName = item.DbColumnName;
                        fieldItem.DBSelectFragment = item.DbColumnName;
                        fieldItem.IsModelProperty = true;
                        resolveResult.SetSelectField(fieldItem);
                    }
                }
                foreach (var item in resolveResult.SqlSelectFields)
                {
                    if (item.IsModelProperty)
                    {
                        sqlBuilder.Append(MysqlGrammarRule.GenerateFieldWrapped(item.DBSelectFragment));
                        sqlBuilder.Append(" as ");
                        sqlBuilder.Append(MysqlGrammarRule.GenerateFieldWrapped(item.DBFieldAsName));
                        sqlBuilder.Append(",");
                    }
                }
                sqlBuilder.Replace(",", "", sqlBuilder.Length - 1, 1);
                sqlBuilder.Append(" from ");
                sqlBuilder.Append(MysqlGrammarRule.GenerateTableWrapped(tableName));

                #endregion

                #region 指定Where条件

                if (!string.IsNullOrEmpty(resolveResult.SqlWhereConditionText))
                {
                    sqlBuilder.Append(" where ");
                    sqlBuilder.Append(resolveResult.SqlWhereConditionText);
                }

                //装载参数
                if (resolveResult.SqlQuerylParameters != null && resolveResult.SqlQuerylParameters.Count() > 0)
                    foreach (var item in resolveResult.SqlQuerylParameters)
                        parameters.Add(new MySqlParameter(item.Name, item.Value));

                #endregion

                #region 指定Order条件

                if (!string.IsNullOrEmpty(resolveResult.SqlOrderConditionText))
                {
                    sqlBuilder.Append(" order by ");
                    sqlBuilder.Append(resolveResult.SqlOrderConditionText);
                }

                #endregion
            }
            sqlBuilder.Append(";");

            //初始化Debug
            result.DebugInit(sqlBuilder, MysqlGrammarRule.C_ParamChar, parameters.ToArray());

            #endregion

            #region 执行Sql语句

            using (MySqlConnection connection = new MySqlConnection(dbString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = sqlBuilder.ToString();
                    if (parameters.Count > 0)
                        foreach (var item in parameters)
                            command.Parameters.Add(item);

                    //尝试打开数据库连结
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
                    DbDataReader reader;
                    try
                    {
                        reader = await command.ExecuteReaderAsync();
                    }
                    catch (Exception ex)
                    {
                        reader = null;
                        result.AppendError("sql语句执行错误，" + command.CommandText);
                        result.AppendException(ex);

                        await reader.CloseAsync();
                        await reader.DisposeAsync();
                        await command.DisposeAsync();
                        await connection.CloseAsync();
                        await connection.DisposeAsync();

                        return result;
                    }
                    if (reader != null && reader.HasRows && await reader.ReadAsync())
                    {
                        result.Record = this.AutoFillModel(reader, modelT, resolveResult.SqlSelectFields);

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

            MysqlSentenceResult resolveResult = null;
            List<MySqlParameter> parameters = new List<MySqlParameter>();

            #region 解析表达式条件

            if (exp != null)
            {
                resolveResult = MysqlSentenceHandler.ExecuteResolver(exp, this._dbMappingHandler);
                if (!resolveResult.IsAvailable())
                {
                    result.CopyStatus(resolveResult);
                    return result;
                }
            }

            #endregion

            #region 拼接SQL语句

            string tableName = this._dbMappingHandler.GetDbTableName(modelT);
            if (!string.IsNullOrEmpty(suffix))
                tableName = $"{tableName}{suffix}";

            bool count_from_list = false;
            StringBuilder countBuilder = new StringBuilder();
            StringBuilder queryBuilder = new StringBuilder();

            int currentPage;
            int pageSize;
            if (resolveResult == null)
            {
                #region 初始化查询分析中间对象

                // 初始化保证该不为空（下面查询出数据后封装model会用）
                resolveResult = MysqlSentenceResult.Create();

                // 设置分页参数
                currentPage = MysqlSentenceResult.DEFAULT_CURRENTPAGE;
                pageSize = MysqlSentenceResult.DEFAULT_PAGESIZE;
                resolveResult.SetPageCondition(currentPage, pageSize);

                //设置检索的全字段，默认查询所有
                foreach (var item in columns)
                    resolveResult.SetSelectField(new MysqlSelectField
                    {
                        DBFieldAsName = item.DbColumnName,
                        DBSelectFragment = item.DbColumnName,
                        IsModelProperty = true
                    });

                #endregion

                #region 强制设置数据结果集统计从列表中加载

                count_from_list = true;

                #endregion

                #region 拼接构造查询语句

                queryBuilder.Append("select * from ");
                queryBuilder.Append(MysqlGrammarRule.GenerateTableWrapped(tableName));
                queryBuilder.AppendFormat(" limit 0,{0}", pageSize);
                queryBuilder.Append(";");

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
                    currentPage = MysqlSentenceResult.DEFAULT_CURRENTPAGE;
                if (pageSize < 1)
                    pageSize = MysqlSentenceResult.DEFAULT_PAGESIZE;

                #endregion

                #region 特殊情况处理（若currpage=1并且pageSize=int.MaxValue,那么就是查询所有的数据,所以可设置不启动分页）

                var enablePaging = true;
                if (1 == currentPage && int.MaxValue == pageSize)
                    enablePaging = false;

                #endregion

                #region 启用分页则需要根据条件查询总数量(拼接构造统计语句)

                if (enablePaging)
                {
                    countBuilder.Append("select count(1) from ");
                    countBuilder.Append(MysqlGrammarRule.GenerateTableWrapped(tableName));
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

                #region Select XX From

                queryBuilder.Append("select ");
                if (resolveResult.SqlSelectFields == null || resolveResult.SqlSelectFields.Count() <= 0)
                {
                    //如果没有设置要查询的字段，默认查询所有
                    foreach (var item in columns)
                        resolveResult.SetSelectField(new MysqlSelectField
                        {
                            DBFieldAsName = item.DbColumnName,
                            DBSelectFragment = item.DbColumnName,
                            IsModelProperty = true
                        });
                }
                foreach (var item in resolveResult.SqlSelectFields)
                {
                    if (item.IsModelProperty)
                    {
                        queryBuilder.Append(MysqlGrammarRule.GenerateFieldWrapped(item.DBSelectFragment));
                        queryBuilder.Append(" as ");
                        queryBuilder.Append(MysqlGrammarRule.GenerateFieldWrapped(item.DBFieldAsName));
                        queryBuilder.Append(",");
                    }
                }
                queryBuilder.Replace(",", string.Empty, queryBuilder.Length - 1, 1);
                queryBuilder.Append(" from ");
                queryBuilder.Append(MysqlGrammarRule.GenerateTableWrapped(tableName));

                #endregion

                #region 指定Where条件

                if (!string.IsNullOrEmpty(resolveResult.SqlWhereConditionText))
                {
                    queryBuilder.Append(" where ");
                    queryBuilder.Append(resolveResult.SqlWhereConditionText);
                }
                if (resolveResult.SqlQuerylParameters != null && resolveResult.SqlQuerylParameters.Count() > 0)
                    foreach (var item in resolveResult.SqlQuerylParameters)
                        parameters.Add(new MySqlParameter(item.Name, item.Value));

                #endregion

                #region 指定Order条件

                if (!string.IsNullOrEmpty(resolveResult.SqlOrderConditionText))
                {
                    queryBuilder.Append(" order by ");
                    queryBuilder.Append(resolveResult.SqlOrderConditionText);
                }

                #endregion

                #region 指定分页数

                queryBuilder.AppendFormat(" limit {0},{1}", (pageSize * (currentPage - 1)), pageSize);
                queryBuilder.Append(";");

                #endregion

                #endregion
            }

            //初始化Debug
            var debugBuilder = new StringBuilder();
            if (countBuilder.Length > 0)
                debugBuilder.Append(countBuilder);
            if (queryBuilder.Length > 0)
                debugBuilder.Append(queryBuilder);
            result.DebugInit(debugBuilder, MysqlGrammarRule.C_ParamChar, parameters.ToArray());

            #endregion

            #region 执行Sql语句

            using (MySqlConnection connection = new MySqlConnection(dbString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    if (parameters.Count > 0)
                        foreach (var item in parameters)
                            command.Parameters.Add(item);

                    //尝试打开数据库连结
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

                        DbDataReader reader;
                        try
                        {
                            reader = await command.ExecuteReaderAsync();
                        }
                        catch (Exception ex)
                        {
                            reader = null;
                            result.AppendError("sql语句执行错误，" + command.CommandText);
                            result.AppendException(ex);

                            await reader.CloseAsync();
                            await reader.DisposeAsync();
                            await command.DisposeAsync();
                            await connection.CloseAsync();
                            await connection.DisposeAsync();

                            return result;
                        }
                        if (reader != null && reader.HasRows)
                        {
                            result.Record = new List<M>();
                            while (await reader.ReadAsync())
                            {
                                M entity = this.AutoFillModel(reader, modelT, resolveResult.SqlSelectFields);
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
            MysqlSentenceResult resolveResult = null;
            List<MySqlParameter> parameters = new List<MySqlParameter>();

            #region 解析表达式条件

            if (exp != null)
            {
                resolveResult = MysqlSentenceHandler.ExecuteResolver(exp, this._dbMappingHandler);
                if (!resolveResult.IsAvailable())
                {
                    result.CopyStatus(resolveResult);
                    return result;
                }
            }
            else
            {
                result.AppendError("表达式exp不允许为空");
                return result;
            }

            #endregion

            StringBuilder sqlBuilder = new StringBuilder("select ");
            if (resolveResult.SqlSelectFields != null && resolveResult.SqlSelectFields.Count() > 0)
            {
                #region 拼接Sql语句

                string tableName = this._dbMappingHandler.GetDbTableName(modelT);
                if (!string.IsNullOrEmpty(suffix))
                    tableName = $"{tableName}{suffix}";

                foreach (var item in resolveResult.SqlSelectFields.OrderBy(d => d.IsModelProperty).OrderBy(d => d.DBFieldAsName))
                {
                    sqlBuilder.Append(MysqlGrammarRule.GenerateFieldWrapped(item.DBSelectFragment));
                    sqlBuilder.Append(" as ");
                    sqlBuilder.Append(MysqlGrammarRule.GenerateFieldWrapped(item.DBFieldAsName));
                    sqlBuilder.Append(",");
                }
                sqlBuilder.Replace(",", string.Empty, sqlBuilder.Length - 1, 1);
                sqlBuilder.Append(" from ");
                sqlBuilder.Append(MysqlGrammarRule.GenerateTableWrapped(tableName));

                if (!string.IsNullOrEmpty(resolveResult.SqlWhereConditionText))
                {
                    sqlBuilder.Append(" where ");
                    sqlBuilder.Append(resolveResult.SqlWhereConditionText);
                }
                if (resolveResult.SqlQuerylParameters != null && resolveResult.SqlQuerylParameters.Count() > 0)
                    foreach (var item in resolveResult.SqlQuerylParameters)
                        parameters.Add(new MySqlParameter(item.Name, item.Value));

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
                result.DebugInit(sqlBuilder, MysqlGrammarRule.C_ParamChar, parameters.ToArray());

                #endregion

                #region 执行Sql语句

                using (MySqlConnection connection = new MySqlConnection(dbString))
                {
                    using (MySqlCommand command = new MySqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = sqlBuilder.ToString();
                        if (parameters.Count > 0)
                            foreach (var item in parameters)
                                command.Parameters.Add(item);

                        //尝试打开数据库连结
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
                        DbDataReader reader;
                        try
                        {
                            reader = await command.ExecuteReaderAsync();
                        }
                        catch (Exception ex)
                        {
                            reader = null;
                            result.AppendError("sql语句执行错误，" + command.CommandText);
                            result.AppendException(ex);

                            await reader.CloseAsync();
                            await reader.DisposeAsync();
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
                            await reader.CloseAsync();
                            await reader.DisposeAsync();
                        }
                    }
                }

                #endregion

                return result;
            }
            else
            {
                #region 必须至少指定一个运算模式，例如:Count,Sum,Max,Min等

                result.AppendError("必须至少指定一个运算模式，例如:Count,Sum,Max,Min等");
                return result;

                #endregion
            }
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
        private bool TryOpenDbConnection<T>(MySqlConnection connection, ref T result)
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
        /// 尝试执行MySqlDataReader,可能返回为null值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private MySqlDataReader TryExecuteReader<T>(MySqlCommand command, ref T result)
            where T : ResultBase
        {
            MySqlDataReader reader;
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
        private void DisposeReader(DbDataReader reader)
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
        private M AutoFillModel(DbDataReader reader, Type dbModelT, IEnumerable<MysqlSelectField> selectFields)
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
        private MySqlDbType GetDbtype(string dbtypeName)
        {
            MySqlDbType dbType = MySqlDbType.VarChar;
            bool isFind = true;

            switch (dbtypeName.ToLower().Trim())
            {
                case "blob":
                    dbType = MySqlDbType.Blob;
                    break;
                case "decimal":
                    dbType = MySqlDbType.Decimal;
                    break;
                case "bit":
                    dbType = MySqlDbType.Bit;
                    break;
                case "longblob":
                    dbType = MySqlDbType.LongBlob;
                    break;
                case "varstring":
                    dbType = MySqlDbType.VarString;
                    break;
                case "varbinary":
                    dbType = MySqlDbType.VarBinary;
                    break;
                case "float":
                    dbType = MySqlDbType.Float;
                    break;
                case "date":
                    dbType = MySqlDbType.DateTime;
                    break;
                case "geometry":
                    dbType = MySqlDbType.Geometry;
                    break;
                case "numeric":
                    dbType = MySqlDbType.DateTime;
                    break;
                case "year":
                    dbType = MySqlDbType.Year;
                    break;
                case "tinytext":
                    dbType = MySqlDbType.TinyText;
                    break;
                case "string":
                    dbType = MySqlDbType.String;
                    break;
                case "enum":
                    dbType = MySqlDbType.Enum;
                    break;
                case "datetime":
                    dbType = MySqlDbType.DateTime;
                    break;
                case "timestamp":
                    dbType = MySqlDbType.Timestamp;
                    break;
                case "time":
                    dbType = MySqlDbType.Time;
                    break;
                case "tinyint":
                    dbType = MySqlDbType.Int16;
                    break;
                case "int":
                    dbType = MySqlDbType.Int32;
                    break;
                case "double":
                    dbType = MySqlDbType.Double;
                    break;
                case "bool":
                    dbType = MySqlDbType.Bit;
                    break;
                case "tinyblob":
                    dbType = MySqlDbType.TinyBlob;
                    break;
                case "smallint":
                    dbType = MySqlDbType.Int32;
                    break;
                case "longtext":
                    dbType = MySqlDbType.LongText;
                    break;
                case "binary":
                    dbType = MySqlDbType.Binary;
                    break;
                case "char":
                    dbType = MySqlDbType.VarChar;
                    break;
                case "varchar":
                    dbType = MySqlDbType.VarChar;
                    break;
                case "nvarchar":
                    dbType = MySqlDbType.VarChar;
                    break;
                case "text":
                    dbType = MySqlDbType.Text;
                    break;
                default:
                    isFind = false;
                    break;
            }

            if (!isFind)
            {
                throw new Exception("未找到类型" + dbtypeName);
            }

            return dbType;
        }

        #endregion

        #region IDbConnectionString

        /// <summary>
        /// 获取数据库连接字符串
        /// </summary>
        /// <returns></returns>
        string IDbConnectionString.GetConnection()
        {
            return this._dbConnQuery;
        }

        #endregion
    }
}
