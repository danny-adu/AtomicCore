using System;
using AtomicCore.DbProvider;

namespace AtomicCore.Integration.MssqlDbProviderUnitTest
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
		[DbColumn(DbColumnName = "ID", DbType = "int", IsDbPrimaryKey = true, IsDbGenerated =  false)]
        public int ID { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
		[DbColumn(DbColumnName = "Reamark", DbType = "nvarchar", IsDbPrimaryKey = false, IsDbGenerated =  false)]
        public string Reamark { get; set; } = string.Empty;


	}
}