using System;
using System.Reflection;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Function Output Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class FunctionOutputAttribute : Attribute
    {
        #region Public Methods

        /// <summary>
        /// GetAttribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static FunctionOutputAttribute GetAttribute<T>()
        {
            var type = typeof(T);
            return type.GetTypeInfo().GetCustomAttribute<FunctionOutputAttribute>(true);
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

        #endregion
    }
}