using AtomicCore;
using AtomicCore.DbProvider;
using System;

namespace AtomicCore.Integration.ClickHouseDbProviderUnitTest
{
    /// <summary>
    /// Db数据仓储
    /// </summary>
    public static class BizClickHouseDbRepository
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
        static BizClickHouseDbRepository()
        {
            if (null == AtomicKernel.Dependency)
                throw new Exception("请先初始调用‘AtomicCore.AtomicKernel.Initialize()’");

            s_decrypt = AtomicKernel.Dependency.Resolve<IDecryptAlgorithm>(CryptoMethods.DES);
        }

        #endregion

		#region Table And View

		        /// <summary>
        /// 用户基础信息表
        /// </summary>
		public static IDbProvider<Member_UserBasics> Member_UserBasics { get { return GetRepository<Member_UserBasics>(); } }



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

        #endregion
	}
}