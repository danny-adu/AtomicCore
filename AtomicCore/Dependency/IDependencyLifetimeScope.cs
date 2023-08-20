namespace AtomicCore.Dependency
{
    /// <summary>
    /// Dependency解析的实例接口
    /// </summary>
    public interface IDependencyLifetimeScope
    {
        /// <summary>
        /// 获取依赖解析使用的生命周期范围实例
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        Autofac.ILifetimeScope GetScope(Autofac.IContainer container);
    }
}
