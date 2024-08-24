using System;
using System.Collections.Generic;
using Autofac;
using AtomicCore.DbProvider;
using AtomicCore.Dependency;

namespace AtomicCore.Integration.ClickHouseDbProvider
{
    /// <summary>
    /// IOC 注册挂钩
    /// </summary>
    public sealed class ClickHouseRegisterHook : IDependencyRegisterHook
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
            builder.RegisterGeneric(typeof(ClickHouseDbProvider<>)).Named(DatabaseType.Mssql2008, typeof(IDbProvider<>)).InstancePerDependency();
            //builder.RegisterType<ClickHouseDbProcedurer>().Named(DatabaseType.Mssql2008, typeof(IDbProcedurer)).InstancePerDependency();
        }
    }
}
