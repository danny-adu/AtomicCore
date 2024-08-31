using Autofac;

namespace AtomicCore.Integration.ClickHouseDbProvider
{
    /// <summary>
    /// ClickHouse Table Engine ReginsterHook
    /// </summary>
    public static class ClickHouseTableEngineRegisterHook
    {
        /// <summary>
        /// 注册方法
        /// </summary>
        /// <param name="builder"></param>
        public static void Register(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(ClickHouseMergeTreeEngine<>)).Named(ClickHouseTableEngineDef.MergeTree, typeof(IClickHouseTableEngine<>)).InstancePerDependency();
        }
    }
}
