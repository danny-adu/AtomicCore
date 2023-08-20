using System;
using System.Collections.Generic;
using System.Reflection;

namespace AtomicCore.DbProvider
{
    /// <summary>
    /// SqlServer数据映射缓存类
    /// </summary>
    internal sealed class DbMappingCache
    {
        #region Variable

        /// <summary>
        /// 成员反射搜索条件
        /// </summary>
        private static readonly BindingFlags s_propertyBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.GetProperty;

        /// <summary>
        /// 模型Cache,Key:modelType's FullName,Value:model mapping db descriptor
        /// </summary>
        private static Dictionary<string, DbCacheDescriptor> s_modelCache = new Dictionary<string, DbCacheDescriptor>();

        #endregion

        #region Public Methods

        /// <summary>
        /// 获取数据实例信息
        /// </summary>
        /// <param name="modelType">模型类型</param>
        /// <returns></returns>
        public static DbDatabaseAttribute GetDBDatabase(Type modelType)
        {
            if (null == modelType)
            {
                throw new ArgumentNullException("The Method 'GetDBInstance' Of Parameter 'modelType' is null");
            }

            if (!s_modelCache.ContainsKey(modelType.FullName))
            {
                object operatekey = GetKey(modelType);
                lock (operatekey)
                {
                    if (!s_modelCache.ContainsKey(modelType.FullName))
                    {
                        DbCacheDescriptor register = RegisterCache(modelType);
                        s_modelCache.Add(modelType.FullName, register);
                    }
                }
            }

            DbCacheDescriptor dbTableDes = s_modelCache[modelType.FullName];
            return dbTableDes.DbDatabase;
        }

        /// <summary>
        /// 获取表信息
        /// </summary>
        /// <param name="modelType">模型类型</param>
        /// <returns></returns>
        public static DbTableAttribute GetTable(Type modelType)
        {
            if (null == modelType)
            {
                throw new ArgumentNullException("The Method 'GetTable' Of Parameter 'modelType' is null");
            }

            if (!s_modelCache.ContainsKey(modelType.FullName))
            {
                object operatekey = GetKey(modelType);
                lock (operatekey)
                {
                    if (!s_modelCache.ContainsKey(modelType.FullName))
                    {
                        DbCacheDescriptor register = RegisterCache(modelType);
                        s_modelCache.Add(modelType.FullName, register);
                    }
                }
            }

            DbCacheDescriptor dbTableDes = s_modelCache[modelType.FullName];
            return dbTableDes.DbTable;
        }

        /// <summary>
        /// 获取列信息
        /// </summary>
        /// <param name="modelType">模型类型</param>
        /// <returns></returns>
        public static Dictionary<PropertyInfo, DbColumnAttribute> GetColumns(Type modelType)
        {
            if (null == modelType)
            {
                throw new ArgumentNullException("The Method 'GetColumn' Of Parameter 'modelType' is null");
            }

            if (!s_modelCache.ContainsKey(modelType.FullName))
            {
                object operatekey = GetKey(modelType);
                lock (operatekey)
                {
                    if (!s_modelCache.ContainsKey(modelType.FullName))
                    {
                        DbCacheDescriptor register = RegisterCache(modelType);
                        s_modelCache.Add(modelType.FullName, register);
                    }
                }
            }

            DbCacheDescriptor dbTableDes = s_modelCache[modelType.FullName];
            return dbTableDes.DbColumns;
        }

        /// <summary>
        /// 注册缓存
        /// </summary>
        /// <param name="modelType"></param>
        private static DbCacheDescriptor RegisterCache(Type modelType)
        {
            DbCacheDescriptor des = new DbCacheDescriptor();

            object[] databaseAttrs = modelType.GetCustomAttributes(typeof(DbDatabaseAttribute), false);
            if (null != databaseAttrs && databaseAttrs.Length > 0)
            {
                des.DbDatabase = databaseAttrs[0] as DbDatabaseAttribute;
            }
            else
            {
                throw new Exception("无法注册DB模型,缺少DBDatabaseAttribute");
            }

            object[] tableAttrs = modelType.GetCustomAttributes(typeof(DbTableAttribute), false);
            if (null != tableAttrs && tableAttrs.Length > 0)
            {
                des.DbTable = tableAttrs[0] as DbTableAttribute;
            }
            else
            {
                throw new Exception("无法注册DB模型,缺少DBTableAttribute");
            }

            des.DbColumns = new Dictionary<PropertyInfo, DbColumnAttribute>();
            PropertyInfo[] pis = modelType.GetProperties(s_propertyBindingFlags);
            foreach (PropertyInfo pi in pis)
            {
                object[] columnAttrs = pi.GetCustomAttributes(typeof(DbColumnAttribute), false);
                if (null != columnAttrs && columnAttrs.Length > 0)
                {
                    DbColumnAttribute column = columnAttrs[0] as DbColumnAttribute;
                    column.PropertyNameMapping = pi.Name;
                    des.DbColumns.Add(pi, column);
                }
            }

            return des;
        }

        #endregion

        #region Cache Key Manages(Private)

        /// <summary>
        /// 初始化KEY集合的起始KEY
        /// </summary>
        private static object s_objectKeysInit = new object();
        /// <summary>
        /// KEY集合,保证每一种类型的操作会有控制在单线程中
        /// </summary>
        private static Dictionary<string, object> s_objectKeyCollection = new Dictionary<string, object>();

        /// <summary>
        /// 获取指定模型的操作KEY实例
        /// </summary>
        /// <param name="modelType"></param>
        /// <returns></returns>
        private static object GetKey(Type modelType)
        {
            string keyName = modelType.FullName;
            if (!s_objectKeyCollection.ContainsKey(keyName))
            {
                lock (s_objectKeysInit)
                {
                    if (!s_objectKeyCollection.ContainsKey(keyName))
                    {
                        s_objectKeyCollection.Add(keyName, new object());
                    }
                }
            }
            return s_objectKeyCollection[keyName];
        }

        #endregion
    }
}
