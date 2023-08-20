using System;
using AtomicCore.DbProvider;

namespace AtomicCore.Integration.MysqlDbProviderUnitTest
{
    /// <summary>
    /// Topic_QQS
    /// </summary>
	[DbDatabase(Name = "DLYS_MNKS")]
    [DbTable(Name = "Topic_QQS")]
	public class Topic_QQS : IDbModel
	{
		        /// <summary>
        /// 
        /// </summary>
		[DbColumn(DbColumnName = "id", DbType = "int", IsDbPrimaryKey = true, IsDbGenerated =  true)]
        public int id { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
		[DbColumn(DbColumnName = "qq", DbType = "varchar", IsDbPrimaryKey = false, IsDbGenerated =  false)]
        public string qq { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
		[DbColumn(DbColumnName = "text", DbType = "varchar", IsDbPrimaryKey = false, IsDbGenerated =  false)]
        public string text { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
		[DbColumn(DbColumnName = "isdel", DbType = "bit", IsDbPrimaryKey = false, IsDbGenerated =  false)]
        public bool isdel { get; set; } = false;


	}
}