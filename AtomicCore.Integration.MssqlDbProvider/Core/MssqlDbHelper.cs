using AtomicCore.DbProvider;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace AtomicCore.Integration.MssqlDbProvider
{
    /// <summary>
    /// Sql DbHelper
    /// </summary>
    internal static class MssqlDbHelper
    {
        #region Public Methods

        /// <summary>
        /// 根据数据库格式类型获取SqlDbType类型
        /// </summary>
        /// <param name="dbtypeName"></param>
        /// <returns></returns>
        internal static System.Data.SqlDbType GetDbtype(string dbtypeName)
        {
            System.Data.SqlDbType dbType = System.Data.SqlDbType.VarChar;
            bool isFind = true;

            switch (dbtypeName.ToLower().Trim())
            {

                case "bigint":
                    dbType = System.Data.SqlDbType.BigInt;
                    break;
                case "binary":
                    dbType = System.Data.SqlDbType.Binary;
                    break;
                case "bool":
                    dbType = System.Data.SqlDbType.Bit;
                    break;
                case "bit":
                    dbType = System.Data.SqlDbType.Bit;
                    break;
                case "char":
                    dbType = System.Data.SqlDbType.Char;
                    break;
                case "date":
                    dbType = System.Data.SqlDbType.Date;
                    break;
                case "datetime":
                    dbType = System.Data.SqlDbType.DateTime;
                    break;
                case "datetime2":
                    dbType = System.Data.SqlDbType.DateTime2;
                    break;
                case "datetimeoffset":
                    dbType = System.Data.SqlDbType.DateTimeOffset;
                    break;
                case "decimal":
                    dbType = System.Data.SqlDbType.Decimal;
                    break;
                case "numeric":
                    dbType = System.Data.SqlDbType.Decimal;
                    break;
                case "float":
                    dbType = System.Data.SqlDbType.Float;
                    break;
                case "image":
                    dbType = System.Data.SqlDbType.Image;
                    break;
                case "int":
                    dbType = System.Data.SqlDbType.Int;
                    break;
                case "money":
                    dbType = System.Data.SqlDbType.Money;
                    break;
                case "nchar":
                    dbType = System.Data.SqlDbType.NChar;
                    break;
                case "ntext":
                    dbType = System.Data.SqlDbType.NText;
                    break;
                case "nvarchar":
                    dbType = System.Data.SqlDbType.NVarChar;
                    break;
                case "real":
                    dbType = System.Data.SqlDbType.Real;
                    break;
                case "smalldatetime":
                    dbType = System.Data.SqlDbType.SmallDateTime;
                    break;
                case "smallint":
                    dbType = System.Data.SqlDbType.SmallInt;
                    break;
                case "smallmoney":
                    dbType = System.Data.SqlDbType.SmallMoney;
                    break;
                case "structured":
                    dbType = System.Data.SqlDbType.Structured;
                    break;
                case "text":
                    dbType = System.Data.SqlDbType.Text;
                    break;
                case "time":
                    dbType = System.Data.SqlDbType.Time;
                    break;
                case "timestamp":
                    dbType = System.Data.SqlDbType.Timestamp;
                    break;
                case "tinyint":
                    dbType = System.Data.SqlDbType.TinyInt;
                    break;
                case "udt":
                    dbType = System.Data.SqlDbType.Udt;
                    break;
                case "uniqueidentifier":
                    dbType = System.Data.SqlDbType.UniqueIdentifier;
                    break;
                case "varbinary":
                    dbType = System.Data.SqlDbType.VarBinary;
                    break;
                case "varchar":
                    dbType = System.Data.SqlDbType.VarChar;
                    break;
                case "string":
                    dbType = System.Data.SqlDbType.VarChar;
                    break;
                case "variant":
                    dbType = System.Data.SqlDbType.Variant;
                    break;
                case "xml":
                    dbType = System.Data.SqlDbType.Xml;
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

        /// <summary>
        /// 尝试打开数据库链接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        internal static bool TryOpenDbConnection<T>(DbConnection connection, ref T result)
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
        internal static DbDataReader TryExecuteReader<T>(DbCommand command, ref T result)
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
        internal static void DisposeReader(DbDataReader reader)
        {
            if (null == reader)
                return;

            //释放资源，关闭连结
            using (reader as IDisposable) { }
        }

        /// <summary>
        /// Model实体自动填充(请在調用该方法前进行reader.Read()判断)
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="reader">数据源</param>
        /// <param name="dbModelT">当前dbmodel类型</param>
        /// <param name="dbMappingHandler">dbMappingHandler</param>
        /// <param name="selectFields">需要被指定填充的字段</param>
        /// <returns></returns>
        internal static M AutoFillModel<M>(DbDataReader reader, Type dbModelT, IDbMappingHandler dbMappingHandler, IEnumerable<MssqlSelectField> selectFields)
            where M : IDbModel, new()
        {
            bool isCreateInstance = false;
            M model = default;

            DbColumnAttribute[] columns = dbMappingHandler.GetDbColumnCollection(dbModelT);
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

        #endregion
    }
}
