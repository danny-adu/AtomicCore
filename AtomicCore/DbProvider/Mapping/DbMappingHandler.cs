using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AtomicCore.DbProvider
{
    /// <summary>
    /// SqlServer仓储映射实现类
    /// </summary>
    public sealed class DbMappingHandler : IDbMappingHandler
    {
        #region IDBRepositoryMapping

        /// <summary>
        /// 获取SqlServer数据库的实例名称
        /// </summary>
        /// <param name="modelType"></param>
        /// <returns></returns>
        string IDbMappingHandler.GetDatabaseName(Type modelType)
        {
            return DbMappingCache.GetDBDatabase(modelType).Name;
        }

        /// <summary>
        /// 获取SqlServer数据库的表名称
        /// </summary>
        /// <param name="modelType"></param>
        /// <returns></returns>
        string IDbMappingHandler.GetDbTableName(Type modelType)
        {
            return DbMappingCache.GetTable(modelType).Name;
        }

        /// <summary>
        /// 获取列名信息
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        DbColumnAttribute IDbMappingHandler.GetDbColumnSingle(Type modelType, string propertyName)
        {
            Dictionary<PropertyInfo, DbColumnAttribute> columnMappings = DbMappingCache.GetColumns(modelType);
            if (null == columnMappings)
            {
                return null;
            }
            return columnMappings.FirstOrDefault(d => d.Key.Name == propertyName).Value;
        }

        /// <summary>
        /// 获取符合条件的列集合信息
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        DbColumnAttribute[] IDbMappingHandler.GetDbColumnCollection(Type modelType, Expression<Func<DbColumnAttribute, bool>> exp)
        {
            Dictionary<PropertyInfo, DbColumnAttribute> columnMappings = DbMappingCache.GetColumns(modelType);
            if (null == columnMappings)
            {
                return null;
            }

            if (null == exp)
            {
                return columnMappings.Values.ToArray();
            }
            else
            {
                return columnMappings.Values.Where(exp.Compile()).ToArray();
            }
        }

        /// <summary>
        /// 获取符合条件的模型属性对象
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="dbColumnName"></param>
        /// <returns></returns>
        PropertyInfo IDbMappingHandler.GetPropertySingle(Type modelType, string dbColumnName)
        {
            Dictionary<PropertyInfo, DbColumnAttribute> columnMappings = DbMappingCache.GetColumns(modelType);
            if (null == columnMappings)
            {
                return null;
            }

            return columnMappings.FirstOrDefault(d => d.Value.DbColumnName == dbColumnName).Key;
        }

        /// <summary>
        /// 获取符合条件的模型属性集合
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        PropertyInfo[] IDbMappingHandler.GetPropertyCollection(Type modelType, Expression<Func<DbColumnAttribute, bool>> exp)
        {
            Dictionary<PropertyInfo, DbColumnAttribute> columnMappings = DbMappingCache.GetColumns(modelType);
            if (null == columnMappings)
            {
                return null;
            }

            if (null == exp)
            {
                return columnMappings.Keys.ToArray();
            }
            else
            {
                List<PropertyInfo> pis = new List<PropertyInfo>();
                foreach (var item in columnMappings.Values.Where(exp.Compile()))
                {
                    pis.Add(columnMappings.FirstOrDefault(d => d.Value.DbColumnName == item.DbColumnName).Key);
                }

                return pis.ToArray();
            }
        }

        #endregion
    }
}
