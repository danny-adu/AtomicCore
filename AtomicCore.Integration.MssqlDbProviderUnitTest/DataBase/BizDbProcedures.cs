using AtomicCore;
using AtomicCore.DbProvider;
using AtomicCore.Integration.MssqlDbProvider;
using System;
using System.Data;
using System.Threading.Tasks;

namespace AtomicCore.Integration.MssqlDbProviderUnitTest
{
    /// <summary>
    /// Db数据库存储过程调用入口
    /// </summary>
    public static class BizDbProcedures
	{
        #region Db Conn

        /// <summary>
        /// 参数名称
        /// </summary>
        private const string conn_paramName = "dbConnString";

        /// <summary>
        /// 数据库链接字符串 
        /// </summary>
        private static readonly string s_realConnString = string.Empty;

        #endregion

        #region Constructors

        /// <summary>
        /// 构造函数
        /// </summary>
        static BizDbProcedures()
        {
            IDecryptAlgorithm s_decrypt = AtomicKernel.Dependency.Resolve<IDecryptAlgorithm>(CryptoMethods.DES);
            if (null == s_decrypt)
                throw new NotImplementedException("IDecryptAlgorithm Instance is null");

            ConnectionStringJsonSettings conn_conf = AtomicCore.ConfigurationJsonManager.ConnectionStrings["DLYS_MNKS"];
            if (null == conn_conf || string.IsNullOrEmpty(conn_conf.ConnectionString))
                throw new Exception("miss db connection string");

            if (s_decrypt.IsCiphertext(conn_conf.ConnectionString))
                s_realConnString = s_decrypt.Decrypt(conn_conf.ConnectionString);
            else
                s_realConnString = conn_conf.ConnectionString;
        }

        #endregion

		#region Proc Functions

				public static DbProcedureRecord SP_GetFirstPY(string STR,ref string OUT_PY, bool hasReturns = true)
		{
            var input = Mssql2008DbExecuteInput.Create();
            input.CommandText = "SP_GetFirstPY";
            input.CommandType = CommandType.StoredProcedure;
            input.HasReturnRecords = hasReturns;//有无返回值

			input.AddParameter("STR", STR);
input.AddParameter("OUT_PY", string.Empty, 4000, MssqlParameterDirection.InputOutput);


			IDbProcedurer dal = AtomicKernel.Dependency.Resolve<IDbProcedurer>(DatabaseType.Mssql2008, new System.Collections.Generic.KeyValuePair<string, object>(conn_paramName, s_realConnString));
            var result = dal.Execute(input);

			OUT_PY = (string)input.GetParamValue("OUT_PY");


            return result;
		}

		public static async Task<DbProcedureRecord> SP_GetFirstPYAsync(string STR, bool hasReturns = true)
		{
            var input = Mssql2008DbExecuteInput.Create();
            input.CommandText = "SP_GetFirstPY";
            input.CommandType = CommandType.StoredProcedure;
            input.HasReturnRecords = hasReturns;//有无返回值

			input.AddParameter("STR", STR);
input.AddParameter("OUT_PY", string.Empty, 4000, MssqlParameterDirection.InputOutput);


			IDbProcedurer dal = AtomicKernel.Dependency.Resolve<IDbProcedurer>(DatabaseType.Mssql2008, new System.Collections.Generic.KeyValuePair<string, object>(conn_paramName, s_realConnString));
            var result = await dal.ExecuteAsync(input);

			// Note that there are return out parameters
// outparam : OUT_PY


            return result;
		}



		#endregion
	}
}