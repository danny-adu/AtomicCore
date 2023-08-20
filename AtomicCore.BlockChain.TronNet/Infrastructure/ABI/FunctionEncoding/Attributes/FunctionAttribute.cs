using System;
using System.Reflection;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Function Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class FunctionAttribute : Attribute
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="returnType"></param>
        public FunctionAttribute(string name, string returnType)
        {
            this.Name = name;
            this.ReturnType = returnType;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        public FunctionAttribute(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dtoReturnType"></param>
        public FunctionAttribute(string name, Type dtoReturnType)
        {
            this.DTOReturnType = dtoReturnType;
            this.Name = name;
        }

        #endregion

        #region Propertys

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// DTOReturnType
        /// </summary>
        public Type DTOReturnType { get; private set; }

        /// <summary>
        /// ReturnType
        /// </summary>
        public string ReturnType { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// GetAttribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static FunctionAttribute GetAttribute<T>()
        {
            var type = typeof(T);
            return GetAttribute(type);
        }

        /// <summary>
        /// GetAttribute
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static FunctionAttribute GetAttribute(Type type)
        {
            return type.GetTypeInfo().GetCustomAttribute<FunctionAttribute>(true);
        }

        /// <summary>
        /// GetAttribute
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static FunctionAttribute GetAttribute(object instance)
        {
            var type = instance.GetType();
            return GetAttribute(type);
        }

        /// <summary>
        /// IsFunctionType
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsFunctionType<T>()
        {
            return GetAttribute<T>() != null;
        }

        /// <summary>
        /// IsFunctionType
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsFunctionType(Type type)
        {
            return GetAttribute(type) != null;
        }

        /// <summary>
        /// IsFunctionType
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsFunctionType(object type)
        {
            return GetAttribute(type) != null;
        }

        #endregion
    }
}