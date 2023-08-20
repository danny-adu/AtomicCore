namespace AtomicCore.Dependency
{
    /// <summary>
    /// Dependency的Init初始化所有的类型注册完毕后执行的事件接口
    /// </summary>
    public interface IDependencyInitComplete
    {
        /// <summary>
        /// 标签执行优先级
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// 容器都注册完毕后的执行
        /// </summary>
        /// <param name="container"></param>
        void OnCompleted(Autofac.IContainer container);
    }
}
