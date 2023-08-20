using System;
using System.Collections.Generic;
using Autofac;

namespace AtomicCore.Dependency
{
    /// <summary>
    /// 外部应用IOC注册挂钩接口
    /// </summary>
    public interface IDependencyRegisterHook
    {
        /// <summary>
        /// 排序优先级
        /// </summary>
        int Priority
        {
            get;
        }

        /// <summary>
        /// 依赖注册
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="findTypes"></param>
        void Register(ContainerBuilder builder, List<Type> findTypes);
    }
}
