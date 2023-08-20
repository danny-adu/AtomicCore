using System;

namespace AtomicCore.BlockChain.TronNet
{
    /// <summary>
    /// Parameter Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ParameterAttribute : Attribute
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="order"></param>
        public ParameterAttribute(string type, string name = null, int order = 1)
        {
            Parameter = new Parameter(type, name, order);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="order"></param>
        /// <param name="indexed"></param>
        public ParameterAttribute(string type, string name, int order, bool indexed = false) : this(type, name, order)
        {
            Parameter.Indexed = indexed;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="order"></param>
        public ParameterAttribute(string type, int order) : this(type, null, order)
        {

        }

        #endregion

        #region Propertys

        /// <summary>
        /// Parameter
        /// </summary>
        public Parameter Parameter { get; }

        /// <summary>
        /// Order
        /// </summary>
        public int Order => Parameter.Order;

        /// <summary>
        /// Name
        /// </summary>
        public string Name => Parameter.Name;

        /// <summary>
        /// Type
        /// </summary>
        public string Type => Parameter.Type;

        #endregion
    }
}