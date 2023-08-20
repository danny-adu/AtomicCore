using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AtomicCore.Integration.MysqlDbProvider
{
    /// <summary>
    /// MySql语法规则定义类
    /// </summary>
    internal static class MysqlGrammarRule
    {
        /// <summary>
        /// SqlServer参数前缀 ?
        /// </summary>
        public const char C_ParamChar = '?';

        /// <summary>
        /// 表格包裹格式(Sql语法中使用``号将表名进行包裹，防止关键字命名冲突)
        /// </summary>
        public const string C_TableWrappedFormat = "`{0}`";

        /// <summary>
        /// 字段包裹格式(Sql语法中使用``号将字段名进行包裹，防止关键字命名冲突)
        /// </summary>
        public const string C_FieldWrappedFormat = "`{0}`";

        /// <summary>
        /// 返回Guid完全不重复的ID标识
        /// </summary>
        /// <returns></returns>
        public static string GetUniqueIdentifier()
        {
            return Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// 生成SQL查询的参数名称
        /// </summary>
        /// <param name="pName">指定的名称</param>
        /// <returns></returns>
        public static string GenerateParamName(string pName)
        {
            if (string.IsNullOrEmpty(pName))
                throw new Exception("name is null");
            return string.Format("{0}{1}", C_ParamChar, pName);
        }

        /// <summary>
        /// 生成SQL查询的表包裹文本
        /// </summary>
        /// <param name="tbName"></param>
        /// <returns></returns>
        public static string GenerateTableWrapped(string tbName)
        {
            if (string.IsNullOrEmpty(tbName))
                throw new Exception("tbName is null");
            return string.Format(C_TableWrappedFormat, tbName);
        }

        /// <summary>
        /// 生成SQL查询的字段包裹文本
        /// </summary>
        /// <param name="dbFieldName">字段名称</param>
        /// <param name="tablePreName">字段表前缀</param>
        /// <returns></returns>
        public static string GenerateFieldWrapped(string dbFieldName, string tablePreName = null)
        {
            if (string.IsNullOrEmpty(dbFieldName))
                throw new Exception("fName is null");

            if (string.IsNullOrEmpty(tablePreName))
            {
                if (Regex.IsMatch(dbFieldName, @"(?:(?:Count)|(?:Sum)|(?:Max)|(?:Min))\([^\(\)]+\)", RegexOptions.IgnoreCase))
                    return dbFieldName;
                else
                    return string.Format(C_FieldWrappedFormat, dbFieldName);
            }
            else
                return string.Join(".", GenerateTableWrapped(tablePreName), GenerateFieldWrapped(dbFieldName, null));
        }

        /// <summary>
        /// 根据数据类型返回在Sql中值表达式
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetSqlText(object value)
        {
            if (null == value)
                throw new ArgumentNullException("value is null");

            string sqlText = null;
            if (value is string)
            {
                sqlText = string.Format("'{0}'", value);
            }
            else if (value is Guid)
            {
                sqlText = string.Format("'{0}'", value);
            }
            else if (value is DateTime)
            {
                sqlText = string.Format("'{0}'", value);
            }
            else
            {
                sqlText = value.ToString();
            }
            return sqlText;
        }

        /// <summary>
        /// 根据数据类型返回在Sql中值表达式
        /// </summary>
        /// <param name="value"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static string GetSqlTextByDbType(object value, DbType dbType)
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
    }
}
