using System;
using AtomicCore.DbProvider;

namespace AtomicCore.Integration.MysqlDbProviderUnitTest
{
    /// <summary>
    /// Topic_QQS_Ext
    /// </summary>
	[DbDatabase(Name = "DLYS_MNKS")]
    [DbTable(Name = "Topic_QQS_Ext")]
	public class Topic_QQS_Ext : IDbModel
	{
		        /// <summary>
        /// 
        /// </summary>
		[DbColumn(DbColumnName = "qq", DbType = "varchar", IsDbPrimaryKey = true, IsDbGenerated =  false)]
        public string qq { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
		[DbColumn(DbColumnName = "name", DbType = "varchar", IsDbPrimaryKey = false, IsDbGenerated =  false)]
        public string name { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
		[DbColumn(DbColumnName = "sex", DbType = "int", IsDbPrimaryKey = false, IsDbGenerated =  false)]
        public int sex { get; set; } = 0;


	}
}