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
    /// ClickHouse MergeTree Engine
    /// </summary>
    public class ClickHouseMergeTreeEngine<M> : ClickHouseTableEngineBase, IClickHouseTableEngine<M>
        where M : IDbModel, new()
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbConnString"></param>
        /// <param name="dbMappingHandler"></param>
        public ClickHouseMergeTreeEngine(IDbConnectionString dbConnString, IDbMappingHandler dbMappingHandler)
            : base(dbConnString, dbMappingHandler)
        {

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 更新操作（局部更新）
        /// </summary>
        /// <param name="whereExp">需要被更新的条件</param>
        /// <param name="updatePropertys">需要被替换或更新的属性</param>
        /// <returns></returns>
        public async Task<DbNonRecord> UpdateAsync(Expression<Func<M, bool>> whereExp, Expression<Func<M, M>> updatePropertys)
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

                    // 执行查询语句
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
        /// <returns></returns>
        public Task<DbNonRecord> UpdateAsync(Expression<Func<M, bool>> whereExp, M model)
        {
            throw new NotImplementedException("ClickHouse Not Supported");
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
        }

        /// <summary>
        /// 删除操作
        /// </summary>
        /// <param name="deleteExp">删除条件</param>
        /// <returns></returns>
        public async Task<DbNonRecord> DeleteAsync(Expression<Func<M, bool>> deleteExp)
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

                    // 执行数据查询
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

        #endregion
    }
}
