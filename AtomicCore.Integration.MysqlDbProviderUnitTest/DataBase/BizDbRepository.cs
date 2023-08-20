using AtomicCore;
using AtomicCore.DbProvider;
using System.Linq;

namespace AtomicCore.Integration.MysqlDbProviderUnitTest
{
    /// <summary>
    /// Db数据仓储
    /// </summary>
    public static class BizDbRepository
	{
		#region Variable

        /// <summary>
        /// 解密接口实例
        /// </summary>
        private static readonly IDecryptAlgorithm s_decrypt = null;

        #endregion

        #region Constructors

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static BizDbRepository()
        {
            s_decrypt = AtomicKernel.Dependency.Resolve<IDecryptAlgorithm>(CryptoMethods.DES);
        }

        #endregion

		#region Table And View

		        /// <summary>
        /// Topic_QQS
        /// </summary>
		public static IDbProvider<Topic_QQS> Topic_QQS { get { return GetRepository<Topic_QQS>(); } }

        /// <summary>
        /// Topic_QQS_Ext
        /// </summary>
		public static IDbProvider<Topic_QQS_Ext> Topic_QQS_Ext { get { return GetRepository<Topic_QQS_Ext>(); } }



		#endregion

		#region Public Methods

        /// <summary>
        /// 获取数据仓储
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <returns></returns>
        public static IDbProvider<M> GetRepository<M>()
             where M : IDbModel, new()
        {
            return AtomicKernel.GetDbProvider<M>(s_decrypt);
        }

        /// <summary>
        /// 抛出Db操作返回的错误结果集异常
        /// </summary>
        /// <param name="result"></param>
        public static void ThrowDbResultUnAvailable(ResultBase result)
        {
            if (result.Exceptions.Any())
            {
                BizLog4Provider.DBLogger.Error("db执行异常", result.Exceptions.First());
                return;
            }
            else if (result.Errors.Any())
            {
                BizLog4Provider.DBLogger.Error(result.Errors.First());
                return;
            }
            else
                return;
        }

        #endregion
	}
}