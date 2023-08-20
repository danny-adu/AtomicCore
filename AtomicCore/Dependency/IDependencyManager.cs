using System;
using System.Collections.ObjectModel;

namespace AtomicCore.Dependency
{
    /// <summary>
    /// 依赖注入管理接口
    /// </summary>
    public interface IDependencyManager : IDependencyResolver
    {
        /// <summary>
        /// 当前指定不解析的类型只读集合
        /// </summary>
        ReadOnlyCollection<Type> NotResolveTypes { get; }

        /// <summary>
        /// 追加指定不解析的类型
        /// </summary>
        /// <param name="types"></param>
        void AddNotResolveTypes(params Type[] types);

        /// <summary>
        /// 移除已指定的不解析的类型
        /// </summary>
        /// <param name="types"></param>
        void DelNotResolveTypes(params Type[] types);

        /// <summary>
        /// 依赖引擎初始化(再次初始化等于重新构造)
        /// </summary>
        void Initialize();
    }
}
