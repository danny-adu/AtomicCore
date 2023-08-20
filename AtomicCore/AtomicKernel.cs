using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using AtomicCore.DbProvider;
using AtomicCore.Dependency;

namespace AtomicCore
{
    /// <summary>
    /// Atomic内核調用入口
    /// </summary>
    public static class AtomicKernel
    {
        #region Variable

        /// <summary>
        /// 数据库缓存操作KEY
        /// </summary>
        private static readonly object s_dbProviderKey = new object();

        /// <summary>
        /// 数据库操作实例缓存
        /// </summary>
        private static readonly Dictionary<Type, object> s_dbProviderCache = new Dictionary<Type, object>();

        /// <summary>
        /// 允许的数据库类型
        /// </summary>
        private static readonly string[] s_allowDbType = new string[] 
        { 
            DatabaseType.Mssql2008, 
            DatabaseType.Mysql, 
            DatabaseType.SQLite 
        };

        #endregion

        #region Propertys

        /// <summary>
        /// 依赖注入解析者
        /// </summary>
        public static IDependencyResolver Dependency { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// 获取数据驱动接口实例
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="decryptAlgorithm">解密算法</param>
        /// <returns></returns>
        public static IDbProvider<M> GetDbProvider<M>(IDecryptAlgorithm decryptAlgorithm = null)
            where M : IDbModel, new()
        {
            IDbProvider<M> instance = null;

            Type MT = typeof(M);
            if (!s_dbProviderCache.ContainsKey(MT))
            {
                lock (s_dbProviderKey)
                {
                    if (!s_dbProviderCache.ContainsKey(MT))
                    {
                        //解析数据模型映射接口实例
                        IDbMappingHandler dbMapper = Dependency.Resolve<IDbMappingHandler>();
                        if (null == dbMapper)
                            throw new Exception("未能解析出IDbMappingHandler接口实例");

                        //获取数据库链接
                        string dbName = dbMapper.GetDatabaseName(typeof(M));
                        ConnectionStringJsonSettings connectionSetting = ConfigurationJsonManager.ConnectionStrings[dbName];
                        if (null == connectionSetting)
                            throw new Exception("connection 节点中未配置" + dbName);

                        if (string.IsNullOrEmpty(connectionSetting.ProviderName))
                            throw new Exception("ProviderName is null,检查检点是否配置了providerName属性");

                        if (!s_allowDbType.Any(d => d.Equals(connectionSetting.ProviderName)))
                            throw new Exception("ProviderName is illegal");


                        //执行解密
                        string connStr = null;
                        if (null == decryptAlgorithm || !decryptAlgorithm.IsCiphertext(connectionSetting.ConnectionString))
                            connStr = connectionSetting.ConnectionString;
                        else
                        {
                            try
                            {
                                connStr = decryptAlgorithm.Decrypt(connectionSetting.ConnectionString);
                            }
                            catch
                            {
                                connStr = connectionSetting.ConnectionString;
                            }
                        }

                        //开始构造DAL（使用构造函数一）
                        Dictionary<string, object> paramDic = new Dictionary<string, object>
                        {
                            { "dbConnQuery", connStr },
                            { "dbMappingHandler", dbMapper }
                        };

                        instance = Dependency.Resolve<IDbProvider<M>>(connectionSetting.ProviderName, paramDic.ToArray());

                        s_dbProviderCache.Add(MT, instance);
                    }
                    else
                        instance = s_dbProviderCache[MT] as IDbProvider<M>;
                }
            }
            else
                instance = s_dbProviderCache[MT] as IDbProvider<M>;

            return instance;
        }

        /// <summary>
        /// 获取数据驱动接口实例
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="dbType">数据库类型 DatabaseType</param>
        /// <param name="conn">数据库连结接口实例</param>
        /// <param name="decryptAlgorithm">解密算法</param>
        /// <returns></returns>
        public static IDbProvider<M> GetDbProvider<M>(string dbType, IDbConnectionString conn, IDecryptAlgorithm decryptAlgorithm = null)
            where M : IDbModel, new()
        {
            IDbProvider<M> instance = null;

            Type MT = typeof(M);
            if (!s_dbProviderCache.ContainsKey(MT))
            {
                lock (s_dbProviderKey)
                {
                    if (!s_dbProviderCache.ContainsKey(MT))
                    {
                        if (!Dependency.IsRegistered<IDbProvider<M>>(dbType))
                            throw new Exception("dbType is illegal");

                        if (null == conn)
                            throw new ArgumentNullException("conn");


                        //解析数据模型映射接口实例
                        IDbMappingHandler dbMapper = Dependency.Resolve<IDbMappingHandler>();
                        if (null == dbMapper)
                            throw new Exception("未能解析出IDbMappingHandler接口实例");

                        //执行解密
                        string connStr = null;
                        if (null == decryptAlgorithm)
                            connStr = conn.GetConnection();
                        else
                            connStr = decryptAlgorithm.Decrypt(conn.GetConnection());

                        //开始构造DAL（使用构造函数一）
                        Dictionary<string, object> paramDic = new Dictionary<string, object>
                        {
                            { "dbConnQuery", connStr },
                            { "dbMappingHandler", dbMapper }
                        };

                        instance = Dependency.Resolve<IDbProvider<M>>(dbType, paramDic.ToArray());

                        s_dbProviderCache.Add(MT, instance);
                    }
                    else
                    {
                        instance = s_dbProviderCache[MT] as IDbProvider<M>;
                    }
                }
            }
            else
            {
                instance = s_dbProviderCache[MT] as IDbProvider<M>;
            }

            return instance;
        }

        /// <summary>
        /// 初始化函数
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Initialize()
        {
            //Ioc Init
            DependencyManager dependency = new DependencyManager();
            dependency.Initialize();
            Dependency = dependency;
        }

        #endregion
    }
}
