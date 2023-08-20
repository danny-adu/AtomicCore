using System;
using System.Reflection;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Event Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EventAttribute : Attribute
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        public EventAttribute(string name) : this(name, false)
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isAnonymous"></param>
        public EventAttribute(string name, bool isAnonymous)
        {
            this.Name = name;
            this.IsAnonymous = isAnonymous;
        }

        #endregion

        #region Propertys

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// IsAnonymous
        /// </summary>
        public bool IsAnonymous { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// GetAttribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static EventAttribute GetAttribute<T>()
        {
            var type = typeof(T);
            return GetAttribute(type);
        }

        /// <summary>
        /// GetAttribute
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static EventAttribute GetAttribute(Type type)
        {
            return type.GetTypeInfo().GetCustomAttribute<EventAttribute>(true);
        }

        /// <summary>
        /// GetAttribute
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static EventAttribute GetAttribute(object instance)
        {
            var type = instance.GetType();
            return GetAttribute(type);
        }

        /// <summary>
        /// IsEventType
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsEventType<T>()
        {
            return GetAttribute<T>() != null;
        }

        /// <summary>
        /// IsEventType
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsEventType(Type type)
        {
            return GetAttribute(type) != null;
        }

        /// <summary>
        /// IsEventType
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsEventType(object type)
        {
            return GetAttribute(type) != null;
        }

        #endregion
    }
}