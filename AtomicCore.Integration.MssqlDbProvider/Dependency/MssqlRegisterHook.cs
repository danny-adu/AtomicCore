using System;
using System.Collections.Generic;
using Autofac;
using AtomicCore.DbProvider;
using AtomicCore.Dependency;

namespace AtomicCore.Integration.MssqlDbProvider
{
    /// <summary>
    /// IOC 注册挂钩
    /// </summary>
    public sealed class MssqlRegisterHook : IDependencyRegisterHook
    {
        private const int C_Priority = 1;

        /// <summary>
        /// 注册排序
        /// </summary>
        public int Priority
        {
            get { return C_Priority; }
        }

        /// <summary>
        /// 注册方法
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="findTypes"></param>
        public void Register(ContainerBuilder builder, List<Type> findTypes)
        {
            builder.RegisterGeneric(typeof(Mssql2008DbProvider<>)).Named(DatabaseType.Mssql2008, typeof(IDbProvider<>)).InstancePerDependency();
            builder.RegisterType<Mssql2008DbProcedurer>().Named(DatabaseType.Mssql2008, typeof(IDbProcedurer)).InstancePerDependency();
        }
    }
}
