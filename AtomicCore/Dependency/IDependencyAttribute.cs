using Autofac.Builder;

namespace AtomicCore.Dependency
{
    /// <summary>
    /// DI-IOC标签接口定义
    /// </summary>
    public interface IDependencyAttribute
    {
        /// <summary>
        /// 标签执行优先级
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// DI依赖注册构造追加
        /// </summary>
        /// <typeparam name="TLimit"></typeparam>
        /// <typeparam name="TActivatorData"></typeparam>
        /// <typeparam name="TRegistrationStyle"></typeparam>
        /// <param name="builder">Iocbuilder</param>
        void RegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle>(IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> builder);
    }
}
