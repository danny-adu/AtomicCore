using System;
using AtomicCore.DbProvider;

namespace AtomicCore.Repository.ClickHouse
{
    /// <summary>
    /// 用户基础信息表
    /// </summary>
	[DbDatabase(Name = "default")]
    [DbTable(Name = "Member_UserBasics")]
	public class Member_UserBasics : IDbModel
	{
		        /// <summary>
        /// 
        /// </summary>
		[DbColumn(DbColumnName = "UserID", DbType = "Int32", IsDbPrimaryKey = true, IsDbGenerated =  false)]
        public int UserID { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
		[DbColumn(DbColumnName = "UserName", DbType = "String", IsDbPrimaryKey = false, IsDbGenerated =  false)]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
		[DbColumn(DbColumnName = "UserAge", DbType = "Int32", IsDbPrimaryKey = false, IsDbGenerated =  false)]
        public int UserAge { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
		[DbColumn(DbColumnName = "UserIsBlock", DbType = "Bool", IsDbPrimaryKey = false, IsDbGenerated =  false)]
        public bool UserIsBlock { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
		[DbColumn(DbColumnName = "UserCreateAt", DbType = "DateTime", IsDbPrimaryKey = false, IsDbGenerated =  false)]
        public DateTime UserCreateAt { get; set; } = DateTime.Parse("1900-01-01");


	}
}