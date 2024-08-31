using AtomicCore.DbProvider;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace AtomicCore.Integration.ClickHouseDbProvider
{
    /// <summary>
    /// ClickHouse Db Helper
    /// </summary>
    internal static class ClickHouseDbHelper
    {
        #region Public Methods

        /// <summary>
        /// 根据数据库格式类型获取SqlDbType类型
        /// </summary>
        /// <param name="dbtypeName"></param>
        /// <returns></returns>
        internal static System.Data.DbType GetDbtype(string dbtypeName)
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
        /// <typeparam name="M">IDbModel</typeparam>
        /// <param name="reader">数据源</param>
        /// <param name="dbModelT">当前dbmodel类型</param>
        /// <param name="dbMappingHandler">IDbMappingHandler</param>
        /// <param name="selectFields">需要被指定填充的字段</param>
        /// <returns></returns>
        internal static M AutoFillModel<M>(DbDataReader reader, Type dbModelT, IDbMappingHandler dbMappingHandler, IEnumerable<ClickHouseSelectField> selectFields)
            where M : IDbModel, new()
        {
            if (null == dbMappingHandler)
                throw new ArgumentNullException(nameof(dbMappingHandler));

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
