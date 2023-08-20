using System;
using System.Collections.Generic;

namespace AtomicCore.Dependency
{
    /// <summary>
    /// Ioc 接口依赖注入解析接口定义
    /// </summary>
    public interface IDependencyResolver
    {
        /// <summary>
        /// 服务是否可被解析出来
        /// </summary>
        /// <param name="type">注册类型</param>
        /// <param name="regName">注册别名</param>
        /// <returns></returns>
        bool IsRegistered(Type type, string regName = null);

        /// <summary>
        /// 服务是否可被解析出来
        /// </summary>
        /// <typeparam name="T">注册类型</typeparam>
        /// <param name="regName">注册别名</param>
        /// <returns></returns>
        bool IsRegistered<T>(string regName = null);

        /// <summary>
        ///  根据参数类型进行依赖解析
        /// </summary>
        /// <param name="type">需要被解析的类型</param>
        /// <param name="regName">别名</param>
        /// <param name="parameters">解析构造的时候需要带入的参数</param>
        /// <returns></returns>
        object Resolve(Type type, string regName = null, params KeyValuePair<string, object>[] parameters);

        /// <summary>
        /// 根据泛型T进行依赖解析
        /// </summary>
        /// <typeparam name="T">需要被解析的类型</typeparam>
        /// <param name="regName">别名</param>
        /// <param name="parameters">解析构造的时候需要带入的参数</param>
        /// <returns></returns>
        T Resolve<T>(string regName = null, params KeyValuePair<string, object>[] parameters) where T : class;

        /// <summary>
        /// 根据泛型T进行依赖解析
        /// </summary>
        /// <typeparam name="T">需要被解析的类型</typeparam>
        /// <param name="parameters">解析构造的时候需要带入的参数</param>
        /// <returns></returns>
        T[] ResolveAll<T>(params KeyValuePair<string, object>[] parameters);

        /// <summary>
        /// 尝试解析指定的类型
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        bool TryResolve(Type serviceType, out object instance);

        /// <summary>
        /// 解析未注册的类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        object ResolveUnregistered(Type type);
    }
}
