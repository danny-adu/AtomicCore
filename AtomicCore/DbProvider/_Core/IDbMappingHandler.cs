using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AtomicCore.DbProvider
{
    /// <summary>
    /// DB仓储映射处理接口
    /// </summary>
    public interface IDbMappingHandler
    {
        /// <summary>
        /// 根据IDBModel模型全名获取对应的存储容器中的数据库名
        /// </summary>
        /// <param name="modelType">IDBModel模型类型</param>
        /// <returns></returns>
        string GetDatabaseName(Type modelType);

        /// <summary>
        /// 根据IDBModel模型全名获取对应存储实例的表名
        /// </summary>
        /// <param name="modelType">IDBModel模型类型</param>
        /// <returns></returns>
        string GetDbTableName(Type modelType);

        /// <summary>
        /// 返回符合条件的数据列属性对象【DBModel=>Database】
        /// </summary>
        /// <param name="modelType">IDBModel模型类型</param>
        /// <param name="propertyName">DBModel的属性反射名称</param>
        /// <returns></returns>
        DbColumnAttribute GetDbColumnSingle(Type modelType, string propertyName);

        /// <summary>
        /// 返回符合条件的数据列属性集合【DBModel=>Database】
        /// </summary>
        /// <param name="modelType">IDBModel模型类型</param>
        /// <param name="exp">筛选条件,默认null将返回所有的列集合</param>
        /// <returns></returns>
        DbColumnAttribute[] GetDbColumnCollection(Type modelType, Expression<Func<DbColumnAttribute, bool>> exp = null);

        /// <summary>
        /// 返回符合条件的IDBModel实例的属性对象
        /// </summary>
        /// <param name="modelType">IDBModel模型类型</param>
        /// <param name="dbColumnName">数据源列名</param>
        /// <returns></returns>
        PropertyInfo GetPropertySingle(Type modelType, string dbColumnName);

        /// <summary>
        /// 返回符合条件的IDBModel实例的属性集合
        /// </summary>
        /// <param name="modelType">IDBModel模型类型</param>
        /// <param name="exp">筛选条件,默认null将返回所有的属性集合</param>
        /// <returns></returns>
        PropertyInfo[] GetPropertyCollection(Type modelType, Expression<Func<DbColumnAttribute, bool>> exp = null);
    }
}
