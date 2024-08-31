using AtomicCore.DbProvider;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtomicCore.Integration.ClickHouseDbProvider
{
    /// <summary>
    /// ClickHouse Table Engine Base 
    /// </summary>
    public abstract class ClickHouseTableEngineBase
    {
        #region Variables

        /// <summary>
        /// 数据库链接字符串 
        /// </summary>
        protected readonly IDbConnectionString _dbConnectionStringHandler = null;

        /// <summary>
        /// 数据库字段映射处理接口
        /// </summary>
        protected readonly IDbMappingHandler _dbMappingHandler = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbConnString"></param>
        /// <param name="dbMappingHandler"></param>
        public ClickHouseTableEngineBase(IDbConnectionString dbConnString, IDbMappingHandler dbMappingHandler)
        {
            if (null == dbConnString)
                throw new ArgumentNullException("dbConnString");
            if (null == dbMappingHandler)
                throw new ArgumentNullException("dbMappingHandler");

            this._dbConnectionStringHandler = dbConnString;
            this._dbMappingHandler = dbMappingHandler;
        }

        #endregion

        #region Protected Methods

        


        #endregion
    }
}
