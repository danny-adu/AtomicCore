using System;
using AtomicCore.DbProvider;

namespace AtomicCore.Integration.MssqlDbProviderUnitTest
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
		[DbColumn(DbColumnName = "ID", DbType = "int", IsDbPrimaryKey = true, IsDbGenerated =  true)]
        public int ID { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
		[DbColumn(DbColumnName = "Name", DbType = "nvarchar", IsDbPrimaryKey = false, IsDbGenerated =  false)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
		[DbColumn(DbColumnName = "Sex", DbType = "int", IsDbPrimaryKey = false, IsDbGenerated =  false)]
        public int Sex { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
		[DbColumn(DbColumnName = "QQ", DbType = "varchar", IsDbPrimaryKey = false, IsDbGenerated =  false)]
        public string QQ { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
		[DbColumn(DbColumnName = "IsDel", DbType = "int", IsDbPrimaryKey = false, IsDbGenerated =  false)]
        public int IsDel { get; set; } = 0;


	}
}