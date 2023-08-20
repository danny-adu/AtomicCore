using System;
using System.Collections.Generic;
using AtomicCore.DbProvider;
using AtomicCore.Dependency;
using Autofac;

namespace AtomicCore.Integration.MysqlDbProvider
{
    /// <summary>
    /// IOC 注册挂钩
    /// </summary>
    public sealed class MysqlRegisterHook : IDependencyRegisterHook
    {
        private const int C_Priority = 1;

        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority
        {
            get { return C_Priority; }
        }

        /// <summary>
        /// 注册函数
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="findTypes"></param>
        public void Register(ContainerBuilder builder, List<Type> findTypes)
        {
            builder.RegisterGeneric(typeof(MysqlDbProvider<>)).Named(DatabaseType.Mysql, typeof(IDbProvider<>)).InstancePerDependency();
            builder.RegisterType<MysqlDbProcedurer>().Named(DatabaseType.Mysql, typeof(IDbProcedurer)).InstancePerDependency();
        }
    }
}
