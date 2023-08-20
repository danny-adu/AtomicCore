using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using AtomicCore.DbProvider;
using Autofac;
using Autofac.Core.Lifetime;
using Microsoft.Extensions.DependencyModel;

namespace AtomicCore.Dependency
{
    /// <summary>
    /// 依赖注入管理者
    /// </summary>
    public class DependencyManager : IDependencyManager, IDependencyLifetimeScope, IDisposable
    {
        #region Variable

        /// <summary>
        /// 引用LIB为Package类型的名称
        /// </summary>
        private const string C_LIBTYPE_PACKAGE = "package";
        /// <summary>
        /// 引用LIB为referenceassembly类型的名称
        /// </summary>
        private const string C_LIBTYPE_REFERENCEASSEMBLY = "referenceassembly";
        /// <summary>
        /// 引用LIB为project类型的名称
        /// </summary>
        private const string C_LIBTYPE_PROJECT = "project";


        /// <summary>
        /// IOC当前容器接口实例
        /// </summary>
        private Autofac.IContainer _container = null;

        /// <summary>
        /// IOC的lifetimeScope接口实例
        /// </summary>
        private IDependencyLifetimeScope _lifetimeScop = null;

        /// <summary>
        /// 当前只读的不解析的类型集合
        /// </summary>
        private List<Type> _notResolveTypes = new List<Type>();

        #endregion

        #region IDependencyManager

        /// <summary>
        /// 当前指定不解析的类型只读集合
        /// </summary>
        public ReadOnlyCollection<Type> NotResolveTypes
        {
            get { return this._notResolveTypes.AsReadOnly(); }
        }

        /// <summary>
        /// 追加指定不解析的类型
        /// </summary>
        /// <param name="types"></param>
        public void AddNotResolveTypes(params Type[] types)
        {
            if (null == types || types.Length <= 0)
                return;

            foreach (var type in types)
            {
                if (this._notResolveTypes.Contains(type))
                    continue;

                this._notResolveTypes.Add(type);
            }
        }

        /// <summary>
        /// 移除已指定的不解析的类型
        /// </summary>
        /// <param name="types"></param>
        public void DelNotResolveTypes(params Type[] types)
        {
            if (null == types || types.Length <= 0)
                return;

            foreach (var type in types)
            {
                if (!this._notResolveTypes.Contains(type))
                    continue;

                this._notResolveTypes.Remove(type);
            }
        }

        /// <summary>
        /// 依赖引擎初始化(再次初始化等于重新构造)
        /// </summary>
        public void Initialize()
        {
            #region 0.所有程序集和程序集下类型

            //获取默认的依赖引用
            DependencyContext deps = DependencyContext.Default;
            //排除所有的系统程序集、Nuget下载包
            CompilationLibrary[] libs = deps.CompileLibraries.Where(lib =>
                !lib.Serviceable &&
                lib.Type != C_LIBTYPE_PACKAGE &&
                lib.Type != C_LIBTYPE_REFERENCEASSEMBLY
            ).ToArray();

            //开始遍历装载程序集中的类型
            List<string> libNameList = new List<string>();
            List<Type> listAllType = new List<Type>();
            foreach (var lib in libs)
            {
                if (libNameList.Exists(d => d.Equals(lib.Name, StringComparison.OrdinalIgnoreCase)))
                    continue;

                try
                {
                    var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));
                    listAllType.AddRange(assembly.GetTypes().Where(type => null != type));
                    libNameList.Add(lib.Name);
                }
                catch { }
            }

            //包含自身框架命名开头的包或项
            CompilationLibrary[] selfLibs = deps.CompileLibraries.Where(lib =>
                lib.Name.StartsWith("AtomicCore.", StringComparison.OrdinalIgnoreCase) &&
                !libNameList.Exists(d => d.Equals(lib.Name, StringComparison.OrdinalIgnoreCase))
            ).ToArray();
            foreach (var lib in selfLibs)
            {
                if (libNameList.Exists(d => d.Equals(lib.Name, StringComparison.OrdinalIgnoreCase)))
                    continue;

                try
                {
                    var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));
                    listAllType.AddRange(assembly.GetTypes().Where(type => null != type));
                    libNameList.Add(lib.Name);
                }
                catch { }
            }

            //过滤相同的类型数据
            listAllType = listAllType.Distinct(new TypeClassEqualityComparer()).ToList();

            #endregion

            #region 1.初始化固定系统级注册

            ContainerBuilder builder = new ContainerBuilder();
            builder.Register<IDbMappingHandler>(c => new DbMappingHandler()).SingleInstance();//DB模型映射
            this.CryptographyRegistration(builder);//加密&解密

            #endregion

            #region 2.IDependencyRegisterHook 外部挂钩注册

            Type[] hookTypes = this.FindClassesOfType<IDependencyRegisterHook>(listAllType, true);
            if (null != hookTypes && hookTypes.Length > 0)
            {
                List<IDependencyRegisterHook> hookList = new List<IDependencyRegisterHook>();
                foreach (var hookT in hookTypes)
                    hookList.Add(Activator.CreateInstance(hookT) as IDependencyRegisterHook);

                hookList = hookList.AsQueryable().OrderBy(d => d.Priority).ToList();
                hookList.ForEach(o => o.Register(builder, listAllType));
            }

            #endregion

            #region 3.挂载自身接口与实现的注册(单例)

            builder.RegisterInstance(this).As(typeof(IDependencyManager), typeof(IDependencyResolver)).SingleInstance();

            #endregion

            #region 99.IDependencyLifetimeScope引擎接口注册

            this._container = builder.Build();

            Type[] lifetimeScopeTypes = this.FindClassesOfType<IDependencyLifetimeScope>(listAllType, true);
            int lifetimeScopeClassTotal = null == lifetimeScopeTypes ? 0 : lifetimeScopeTypes.Count(d => d.FullName != this.GetType().FullName);
            if (lifetimeScopeClassTotal <= 0)
                this._lifetimeScop = this;
            else
            {
                if (lifetimeScopeClassTotal > 1)
                    throw new Exception("IDependencyLifetimeScope接口的实现只用在全局实现一次即可,切勿多次实现！");

                this._lifetimeScop = Activator.CreateInstance(lifetimeScopeTypes.First()) as IDependencyLifetimeScope;
            }

            #endregion

            #region 100.将注册的IDependencyRebuildDelegate接口实现进行纳入执行

            Type[] rebuildDelegateTypes = this.FindClassesOfType<IDependencyInitComplete>(listAllType, true);
            if (null != rebuildDelegateTypes && rebuildDelegateTypes.Length > 0)
            {
                List<IDependencyInitComplete> taskList = new List<IDependencyInitComplete>();
                foreach (var taskT in rebuildDelegateTypes)
                    taskList.Add(Activator.CreateInstance(taskT) as IDependencyInitComplete);
                foreach (var taskOpt in taskList.AsQueryable().OrderBy(d => d.Priority))
                    taskOpt.OnCompleted(this._container);
            }

            #endregion
        }

        #endregion

        #region IDependencyResolver

        /// <summary>
        /// 是否可被解析出来
        /// </summary>
        /// <param name="type"></param>
        /// <param name="regName"></param>
        /// <returns></returns>
        public bool IsRegistered(Type type, string regName = null)
        {
            Autofac.ILifetimeScope scope = this._lifetimeScop.GetScope(this._container);

            if (string.IsNullOrEmpty(regName))
                return scope.IsRegistered(type);
            else
                return scope.IsRegisteredWithName(regName, type);
        }

        /// <summary>
        /// 是否可被解析出来
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="regName"></param>
        /// <returns></returns>
        public bool IsRegistered<T>(string regName = null)
        {
            Autofac.ILifetimeScope scope = this._lifetimeScop.GetScope(this._container);

            if (string.IsNullOrEmpty(regName))
                return scope.IsRegistered<T>();
            else
                return scope.IsRegisteredWithName<T>(regName);
        }

        /// <summary>
        /// 根据参数类型进行依赖解析
        /// </summary>
        /// <param name="type"></param>
        /// <param name="regName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object Resolve(Type type, string regName = null, params KeyValuePair<string, object>[] parameters)
        {
            Autofac.ILifetimeScope scope = this._lifetimeScop.GetScope(this._container);

            if (null != parameters && parameters.Length > 0)
            {
                List<Autofac.Core.Parameter> paramList = new List<Autofac.Core.Parameter>();
                foreach (var item in parameters)
                {
                    paramList.Add(new NamedParameter(item.Key, item.Value));
                }

                if (string.IsNullOrEmpty(regName))
                    return scope.Resolve(type, paramList);
                else
                    return scope.ResolveNamed(regName, type, paramList);
            }
            else
            {
                if (string.IsNullOrEmpty(regName))
                    return scope.Resolve(type);
                else
                    return scope.ResolveNamed(regName, type);
            }
        }

        /// <summary>
        /// 根据泛型T进行依赖解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="regName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T Resolve<T>(string regName = null, params KeyValuePair<string, object>[] parameters)
             where T : class
        {
            Autofac.ILifetimeScope scope = this._lifetimeScop.GetScope(this._container);

            if (null != parameters && parameters.Length > 0)
            {
                List<Autofac.Core.Parameter> paramList = new List<Autofac.Core.Parameter>();
                foreach (var item in parameters)
                {
                    paramList.Add(new NamedParameter(item.Key, item.Value));
                }

                if (string.IsNullOrEmpty(regName))
                    return scope.Resolve<T>(paramList);
                else
                    return scope.ResolveNamed<T>(regName, paramList);
            }
            else
            {
                if (string.IsNullOrEmpty(regName))
                    return scope.Resolve<T>();
                else
                    return scope.ResolveNamed<T>(regName);
            }
        }

        /// <summary>
        /// 根据泛型T进行依赖解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T[] ResolveAll<T>(params KeyValuePair<string, object>[] parameters)
        {
            Autofac.ILifetimeScope scope = this._lifetimeScop.GetScope(this._container);

            if (null != parameters && parameters.Length > 0)
            {
                List<Autofac.Core.Parameter> paramList = new List<Autofac.Core.Parameter>();
                foreach (var item in parameters)
                {
                    paramList.Add(new NamedParameter(item.Key, item.Value));
                }

                return scope.Resolve<IEnumerable<T>>(paramList).ToArray();
            }
            else
            {
                return scope.Resolve<IEnumerable<T>>().ToArray();
            }
        }

        /// <summary>
        /// 尝试解析指定的类型
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool TryResolve(Type serviceType, out object instance)
        {
            Autofac.ILifetimeScope scope = this._lifetimeScop.GetScope(this._container);
            return scope.TryResolve(serviceType, out instance);
        }

        /// <summary>
        /// 解析未注册的类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object ResolveUnregistered(Type type)
        {
            Autofac.ILifetimeScope scope = this._lifetimeScop.GetScope(this._container);
            var constructors = type.GetConstructors();
            foreach (var constructor in constructors)
            {
                try
                {
                    var parameters = constructor.GetParameters();
                    var parameterInstances = new List<object>();
                    foreach (var parameter in parameters)
                    {
                        var service = scope.Resolve(parameter.ParameterType);
                        if (service == null)
                        {
                            throw new Exception(string.Format("未知的参数类型{0}",
                                parameter.ParameterType));
                        }
                        parameterInstances.Add(service);
                    }
                    return Activator.CreateInstance(type, parameterInstances.ToArray());
                }
                catch
                {
                    continue;
                }
            }
            throw new Exception("未发现满足依赖解析的构造函数");
        }

        #endregion

        #region IDependencyLifetimeScope

        /// <summary>
        /// 获取生命周期实例
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        ILifetimeScope IDependencyLifetimeScope.GetScope(Autofac.IContainer container)
        {
            return this._container.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag);
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// 实例析构资源释放
        /// </summary>
        public void Dispose()
        {
            if (null != this._container)
            {
                this._container.Dispose();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 加密/解密接口注册
        /// </summary>
        /// <param name="builder"></param>
        private void CryptographyRegistration(ContainerBuilder builder)
        {
            builder.RegisterType<AesSymmetricAlgorithm>()
                .As(typeof(IEncryptAlgorithm), typeof(IDecryptAlgorithm), typeof(IDesSymmetricAlgorithm))
                .Named<IEncryptAlgorithm>(CryptoMethods.AES)
                .Named<IDecryptAlgorithm>(CryptoMethods.AES)
                .Named<IDesSymmetricAlgorithm>(CryptoMethods.AES).InstancePerDependency();

            builder.RegisterType<DesSymmetricAlgorithm>()
                .As(typeof(IEncryptAlgorithm), typeof(IDecryptAlgorithm), typeof(IDesSymmetricAlgorithm))
                .Named<IEncryptAlgorithm>(CryptoMethods.DES)
                .Named<IDecryptAlgorithm>(CryptoMethods.DES)
                .Named<IDesSymmetricAlgorithm>(CryptoMethods.DES).InstancePerDependency();

            builder.RegisterType<CBCPKCS5SymmetricAlgorithm>()
                .As(typeof(IEncryptAlgorithm), typeof(IDecryptAlgorithm), typeof(IDesSymmetricAlgorithm))
                .Named<IEncryptAlgorithm>(CryptoMethods.CBCPKCS5)
                .Named<IDecryptAlgorithm>(CryptoMethods.CBCPKCS5)
                .Named<IDesSymmetricAlgorithm>(CryptoMethods.CBCPKCS5).InstancePerDependency();
        }

        /// <summary>
        /// 从指定的类型集合中筛选出匹配的类型
        /// </summary>
        /// <typeparam name="I">指定接口类型</typeparam>
        /// <param name="scanTypeList">指定的类型集合</param>
        /// <param name="onlyConcreteClasses">True:不包含抽象类型,False:包含抽象类型</param>
        /// <returns></returns>
        private Type[] FindClassesOfType<I>(List<Type> scanTypeList, bool onlyConcreteClasses = true)
        {
            if (null == scanTypeList || scanTypeList.Count <= 0)
                return null;

            Type assignTypeFrom = typeof(I);
            Type[] findTypes = scanTypeList.Where(t => typeof(I).IsAssignableFrom(t) || (assignTypeFrom.IsGenericTypeDefinition && DoesTypeImplementOpenGeneric(t, assignTypeFrom))).ToArray();
            if (null == findTypes || findTypes.Length <= 0)
                return null;

            if (onlyConcreteClasses)
                return findTypes.Where(t => t.IsClass && !t.IsAbstract).ToArray();
            else
                return findTypes;
        }

        /// <summary>
        /// 是否表示可以用来构造其他泛型类型的泛型类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="openGeneric"></param>
        /// <returns></returns>
        private bool DoesTypeImplementOpenGeneric(Type type, Type openGeneric)
        {
            try
            {
                var genericTypeDefinition = openGeneric.GetGenericTypeDefinition();
                foreach (var implementedInterface in type.FindInterfaces((objType, objCriteria) => true, null))
                {
                    if (!implementedInterface.IsGenericType)
                        continue;

                    var isMatch = genericTypeDefinition.IsAssignableFrom(implementedInterface.GetGenericTypeDefinition());
                    return isMatch;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Private Classies

        /// <summary>
        /// Type类型比较接口实现
        /// </summary>
        private class TypeClassEqualityComparer : IEqualityComparer<Type>
        {
            /// <summary>
            /// 比较是否相等
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public bool Equals(Type x, Type y)
            {
                return x.FullName.Equals(y.FullName, StringComparison.OrdinalIgnoreCase);
            }

            /// <summary>
            /// 返回哈希CODE
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int GetHashCode(Type obj)
            {
                return obj.GetHashCode();
            }
        }

        #endregion
    }
}
