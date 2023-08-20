using System;
using System.Text;
using System.Data.Common;
using System.Data;

namespace AtomicCore.DbProvider
{
    /// <summary>
    /// 返回DB查询结果的公共抽象类
    /// </summary>
    public abstract class DbRecordBase : ResultBase, IDbDebug
    {
        #region Constructors

        /// <summary>
        /// 构造函数
        /// </summary>
        public DbRecordBase()
            : base()
        {

        }

        #endregion

        #region IDBDebug

        private string _paramChar = string.Empty;
        private StringBuilder _debugSqlText = null;
        private DbParameter[] _debugSqlParameters = null;

        /// <summary>
        /// 原始SQL语句（直接复制到查询分析器中可以被执行的）
        /// </summary>
        public string OriginalSqlText
        {
            get
            {
                if (this._debugSqlText != null)
                {
                    if (null != this._debugSqlParameters)
                        foreach (var param in this._debugSqlParameters)
                            this.ReplaceParameter(param);
                        
                    
                    return this._debugSqlText.ToString();
                }

                return null;
            }
        }

        /// <summary>
        /// 获取需要被调试的Sql语句
        /// </summary>
        /// <returns></returns>
        public string DebugSqlText
        {
            get
            {
                if (this._debugSqlText != null)
                    return this._debugSqlText.ToString();
                
                return null;
            }
        }

        /// <summary>
        /// 获取需要被调试的Sql参数
        /// </summary>
        /// <returns></returns>
        public DbParameter[] DebugSqlParameters
        {
            get { return this._debugSqlParameters; }
        }

        /// <summary>
        /// 初始化Sql查询的Debug条件
        /// </summary>
        /// <param name="sqlBuilder">sql语句拼接对象</param>
        /// <param name="paramChar">sql的参数前缀符,例如:Mssql-@,Mysql-?</param>
        /// <param name="sqlParameters">参数集合</param>
        public void DebugInit(StringBuilder sqlBuilder, char paramChar, params DbParameter[] sqlParameters)
        {
            this._debugSqlText = sqlBuilder;
            this._paramChar = paramChar.ToString();
            this._debugSqlParameters = sqlParameters;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 替换指定的参数,将Sql语句清理的可被执行
        /// </summary>
        /// <param name="parameter"></param>
        private void ReplaceParameter(DbParameter parameter)
        {
            this._debugSqlText.Replace(string.Format("{0}{1}", this._paramChar, parameter.ParameterName), this.GetSqlValue(parameter.Value, parameter.DbType));
        }

        /// <summary>
        /// 根据数据类型返回在Sql中值表达式
        /// </summary>
        /// <param name="value"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        private string GetSqlValue(object value, DbType dbType)
        {
            string sqlVal = string.Empty;
            switch (dbType)
            {
                case DbType.AnsiString:
                    sqlVal = string.Format("'{0}'", value);
                    break;
                case DbType.Binary:
                    sqlVal = string.Format("'{0}'", value);
                    break;
                case DbType.Byte:
                    sqlVal = value.ToString();
                    break;
                case DbType.Boolean:
                    sqlVal = Convert.ToBoolean(value) ? "1" : "0";
                    break;
                case DbType.Currency:
                    sqlVal = value.ToString();
                    break;
                case DbType.Date:
                    sqlVal = string.Format("'{0}'", value);
                    break;
                case DbType.DateTime:
                    sqlVal = string.Format("'{0}'", value);
                    break;
                case DbType.Decimal:
                    sqlVal = value.ToString();
                    break;
                case DbType.Double:
                    sqlVal = value.ToString();
                    break;
                case DbType.Guid:
                    sqlVal = string.Format("'{0}'", value);
                    break;
                case DbType.Int16:
                    sqlVal = value.ToString();
                    break;
                case DbType.Int32:
                    sqlVal = value.ToString();
                    break;
                case DbType.Int64:
                    sqlVal = value.ToString();
                    break;
                case DbType.Object:
                    sqlVal = string.Format("'{0}'", value);
                    break;
                case DbType.SByte:
                    sqlVal = value.ToString();
                    break;
                case DbType.Single:
                    sqlVal = value.ToString();
                    break;
                case DbType.String:
                    sqlVal = string.Format("'{0}'", value);
                    break;
                case DbType.Time:
                    sqlVal = string.Format("'{0}'", value);
                    break;
                case DbType.UInt16:
                    sqlVal = value.ToString();
                    break;
                case DbType.UInt32:
                    sqlVal = value.ToString();
                    break;
                case DbType.UInt64:
                    sqlVal = value.ToString();
                    break;
                case DbType.VarNumeric:
                    sqlVal = value.ToString();
                    break;
                case DbType.AnsiStringFixedLength:
                    sqlVal = string.Format("'{0}'", value);
                    break;
                case DbType.StringFixedLength:
                    sqlVal = string.Format("'{0}'", value);
                    break;
                case DbType.Xml:
                    sqlVal = string.Format("'{0}'", value);
                    break;
                case DbType.DateTime2:
                    sqlVal = string.Format("'{0}'", value);
                    break;
                case DbType.DateTimeOffset:
                    sqlVal = value.ToString();
                    break;
            }
            return sqlVal;
        }

        #endregion
    }
}
