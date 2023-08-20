using System.Collections.Generic;
using System.Data;

namespace AtomicCore.DbProvider
{
    /// <summary>
    /// 存储过程输入参数抽象类
    /// </summary>
    public abstract class DbExecuteInputBase
    {
        #region Constructors

        /// <summary>
        /// 构造函数
        /// </summary>
        public DbExecuteInputBase() { }

        #endregion

        #region Propertys

        private string _commandText = null;
        private CommandType _commandType = CommandType.Text;
        private bool _hasReturnRecords = false;

        /// <summary>
        /// 针对数据源运行的文本命令
        /// </summary>
        public string CommandText
        {
            get { return _commandText; }
            set { _commandText = value; }
        }

        /// <summary>
        /// 指示或指定如何解释 CommandText 属性
        /// </summary>
        public CommandType CommandType
        {
            get { return _commandType; }
            set { _commandType = value; }
        }

        /// <summary>
        /// 是否有返回的记录,默认:false，即无返回记录
        /// </summary>
        public bool HasReturnRecords
        {
            get { return _hasReturnRecords; }
            set { _hasReturnRecords = value; }
        }


        #endregion

        #region Methods

        /// <summary>
        /// 根据参数名称获取参数值
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public abstract object GetParamValue(string paramName);

        /// <summary>
        /// 返回当前输入的参数装箱集合
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<object> GetParameterCollection();

        #endregion
    }
}
