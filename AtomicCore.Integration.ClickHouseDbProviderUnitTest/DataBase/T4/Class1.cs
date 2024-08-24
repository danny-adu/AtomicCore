using ClickHouse.Client.ADO;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace AtomicCore.Integration.ClickHouseDbProviderUnitTest
{
    #region T4Config

    public class T4Config
    {
        public const string global_DbName = "default";
        public const string global_namespace = "AtomicCore.Integration.ClickHouseDbProviderUnitTest";
        public static string global_ConnStr = string.Format("Host=127.0.0.1;Port=8123;Username=default;Password=123456;Database=default", global_DbName);

        public const string global_modelTemplate = "ModelTemplate.txt";
        public const string global_viewTemplate = "ViewTemplate.txt";
        public const string global_propertyTemplate = "PropertyTemplate.txt";
        public const string global_dbRepositoryTemplate = "DbRepositoryTemplate.txt";
        public const string global_dbRepositoryPropertyTemplate = "DbRepositoryPropertyTemplate.txt";
        public const string global_dbProceduresTemplate = "DbProceduresTemplate.txt";
        public const string global_dbProceduresFuncTemplate = "DbProceduresFuncTemplate.txt";

        public const string global_modelSaveDir = "Model";
        public const string global_viewSaveDir = "View";
    }

    #endregion

    #region DbHelper

    public static class DbHelper
    {
        #region Variables

        private const string c_reg_suffix = "_[0-9]+$";

        #endregion

        #region GetDbTables

        public static List<DbTable> GetDbTables(string connectionString, string database)
        {
            string sql = @$"
                SELECT 
                        name, 
                        engine, 
                        partition_key
                        sorting_key, 
                        primary_key, 
                        comment
                FROM 
                        system.tables WHERE database = '{database}' ORDER BY `name` ASC
            ";

            DataTable dt = SqlInvoke(connectionString, sql);
            var tb_list = dt.Rows.Cast<DataRow>().Select(row => new DbTable
            {
                TableName = row.Field<string>("name"),
                TableDesc = string.IsNullOrEmpty(row.Field<string>("comment")) ? row.Field<string>("name") : row.Field<string>("comment"),
                TableEngine = row.Field<string>("engine"),
                HasPrimaryKey = !string.IsNullOrEmpty(row.Field<string>("primary_key")),
                PrimaryKeyName = row.Field<string>("primary_key") ?? string.Empty
            }).ToList();

            return tb_list.Where(d => !Regex.IsMatch(d.TableName, c_reg_suffix, RegexOptions.IgnoreCase)).ToList();
        }

        #endregion

        #region GetDbViews

        public static List<DbView> GetDbViews(string connectionString, string database)
        {
            string sql = $@"             
                SELECT 
                        name, 
                        engine, 
                        partition_key
                        sorting_key, 
                        primary_key, 
                        comment
                FROM 
                        system.tables
                WHERE 
                        database = '{database}' AND (engine = 'View' OR engine = 'MaterializedView')
                ORDER BY name;
            ";

            DataTable dt = SqlInvoke(connectionString, sql);
            var view_list = dt.Rows.Cast<DataRow>().Select(row => new DbView
            {
                ViewName = row.Field<string>("name"),
                ViewDesc = string.IsNullOrEmpty(row.Field<string>("comment")) ? row.Field<string>("name") : row.Field<string>("comment"),
                ViewEngine = row.Field<string>("engine"),
                HasPrimaryKey = !string.IsNullOrEmpty(row.Field<string>("primary_key")),
                PrimaryKeyName = row.Field<string>("primary_key") ?? string.Empty
            }).ToList();

            return view_list.Where(d => !Regex.IsMatch(d.ViewName, c_reg_suffix, RegexOptions.IgnoreCase)).ToList();
        }

        #endregion

        #region GetDbColumns

        public static List<DbColumn> GetDbColumns(string connectionString, string database, string table_or_view)
        {
            string sql = $@"
                SELECT 
                        position,
                        name,
                        type,
                        default_kind,
                        default_expression,
                        data_compressed_bytes,
                        data_uncompressed_bytes,
                        marks_bytes,
                        comment,
                        is_in_partition_key,
                        is_in_sorting_key,
                        is_in_primary_key,
                        is_in_sampling_key,
                        type LIKE 'Nullable%' AS is_nullable,
                        compression_codec, 
                        character_octet_length, 
                        numeric_precision, 
                        numeric_precision_radix, 
                        numeric_scale, 
                        datetime_precision
                FROM 
                        system.columns
                WHERE 
                        database = '{database}' AND table = '{table_or_view}'
                ORDER BY position;
            ";

            DataTable dt = SqlInvoke(connectionString, sql);
            return dt.Rows.Cast<DataRow>().Select(row => new DbColumn()
            {
                ColumnID = (int)row.Field<ulong>("position"),  // Assuming ordinal_position as ColumnID
                ColumnName = row.Field<string>("name"),
                ColumnType = row.Field<string>("type"),
                IsPrimaryKey = row.Field<byte>("is_in_primary_key") == 1, 
                IsIdentity = false, // ClickHouse doesn't have auto-increment, so this is generally false
                IsNullable = row.Field<byte>("is_nullable") == 1,
                Remark = row.Field<string>("comment") ?? string.Empty
            }).ToList();
        }

        #endregion

        #region GetDbProc

        /// <summary>
        /// 返回获取指定数据库下的存储过程列表
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="database"></param>
        /// <returns></returns>
        public static List<DbProc> GetProcList(string connectionString, string database)
        {
            #region 获取所有存储过程名称

            string procNameListSql = string.Format(
                @"select * from {0}.dbo.sysobjects where OBJECTPROPERTY(id, N'IsProcedure') = 1 and name not in ('sp_alterdiagram','sp_creatediagram','sp_dropdiagram','sp_helpdiagramdefinition','sp_helpdiagrams','sp_renamediagram','sp_upgraddiagrams') order by name;",
                database);
            DataTable dt = SqlInvoke(connectionString, procNameListSql);
            List<string> procNameList = dt.Rows.Cast<DataRow>().Select(row => row.Field<string>("name")).ToList();
            if (null == procNameList || !procNameList.Any())
                return new List<DbProc>();

            #endregion

            #region 开始循环所有的存储过程

            DbProc eachProc = null;
            List<DbProc> procList = new List<DbProc>();

            StringBuilder paramSqlBuilder = null;
            foreach (var procName in procNameList.Where(d => !Regex.IsMatch(d, c_reg_suffix, RegexOptions.IgnoreCase)))
            {
                paramSqlBuilder = new StringBuilder(string.Format(@"
                    select 
                        REPLACE(name,'@','') as paramName,
		                (select top 1 name from sys.types where user_type_id = xtype) as paramType,
		                CAST(length as int) as paramLength,
		                CAST(xprec as int) as paramPrecision,
		                CAST(xscale as int) as paramScale,
                        CAST(isoutparam as bit) as paramOutput 
                    from {0}.dbo.syscolumns where ID in (SELECT id FROM {0}.dbo.sysobjects WHERE OBJECTPROPERTY(id, N'IsProcedure') = 1 and id = object_id(N'[dbo].[{1}]'))",
                database,
                procName));

                DataTable paramDT = SqlInvoke(connectionString, paramSqlBuilder.ToString());

                //////List<DataRow> drlist = paramDT.Rows.Cast<DataRow>().ToList();
                //////if (drlist.Any())
                //////{
                //////    var name = drlist[0].Field<string>("paramName");
                //////    var type = drlist[0].Field<string>("paramType");
                //////    var lenth = drlist[0].Field<int>("paramLength");
                //////    var precision = drlist[0].Field<int>("paramPrecision");
                //////    var scale = drlist[0].Field<int>("paramScale");
                //////    var output = drlist[0].Field<bool>("paramOutput");
                //////}

                List<DbProcParameter> procParamList = paramDT.Rows.Cast<DataRow>().Select(row => new DbProcParameter()
                {
                    ParamName = row.Field<string>("paramName"),
                    ParamDbType = row.Field<string>("paramType"),
                    ParamLength = row.Field<int>("paramLength"),
                    ParamPrecision = row.Field<int>("paramPrecision"),
                    ParamScale = row.Field<int>("paramScale"),
                    ParamIsOutput = row.Field<bool>("paramOutput")
                }).ToList();

                eachProc = new DbProc()
                {
                    ProcName = procName,
                    ParameterList = procParamList ?? new List<DbProcParameter>()
                };
                procList.Add(eachProc);
            }

            #endregion

            return procList;
        }

        #endregion

        #region SqlInvoke

        private static DataTable SqlInvoke(string connectionString, string commandText, params DbParameter[] parms)
        {
            var dt = new DataTable();

            using (var connection = new ClickHouseConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = commandText;
                    if (null != parms && parms.Length > 0)
                        command.Parameters.AddRange(parms);

                    using (var reader = command.ExecuteReader())
                    {
                        // 创建列
                        for (int i = 0; i < reader.FieldCount; i++)
                            dt.Columns.Add(reader.GetName(i), reader.GetFieldType(i));

                        // 填充数据
                        while (reader.Read())
                        {
                            DataRow row = dt.NewRow();
                            for (int i = 0; i < reader.FieldCount; i++)
                                row[i] = reader.IsDBNull(i) ? DBNull.Value : reader.GetValue(i);

                            dt.Rows.Add(row);
                        }
                    }
                }
            }

            return dt;
        }

        #endregion
    }

    #endregion

    #region DbTable

    /// <summary>
    /// 表结构
    /// </summary>
    public sealed class DbTable
    {
        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 表明说明
        /// </summary>
        public string TableDesc { get; set; }

        /// <summary>
        /// 表的Engine
        /// </summary>
        public string TableEngine { get; set; }

        /// <summary>
        /// 是否含有主键
        /// </summary>
        public bool HasPrimaryKey { get; set; }

        /// <summary>
        /// 主键名称
        /// </summary>
        public string PrimaryKeyName { get; set; }
    }

    /// <summary>
    /// View结构
    /// </summary>
    public sealed class DbView
    {
        /// <summary>
        /// View名称
        /// </summary>
        public string ViewName { get; set; }

        /// <summary>
        /// View说明
        /// </summary>
        public string ViewDesc { get; set; }

        /// <summary>
        /// View-Engine
        /// </summary>
        public string ViewEngine { get; set; }

        /// <summary>
        /// 是否含有主键
        /// </summary>
        public bool HasPrimaryKey { get; set; }

        /// <summary>
        /// 主键名称
        /// </summary>
        public string PrimaryKeyName { get; set; }
    }

    #endregion

    #region DbColumn

    /// <summary>
    /// 表字段结构
    /// </summary>
    public sealed class DbColumn
    {
        /// <summary>
        /// 字段ID
        /// </summary>
        public int ColumnID { get; set; }

        /// <summary>
        /// 字段名称
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        public string ColumnType { get; set; }

        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// 是否自增列
        /// </summary>
        public bool IsIdentity { get; set; }

        /// <summary>
        /// 是否允许空
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 数据库类型对应的C#类型 # ReadOnly
        /// </summary>
        public string CSharpType
        {
            get
            {
                return SqlServerDbTypeMap.MapCsharpType(ColumnType);
            }
        }

        /// <summary>
        /// 数据库类型对应的C#类型默认值 # ReadOnly
        /// </summary>
        public string CSharpDefVal
        {
            get
            {
                return SqlServerDbTypeMap.MapCsharpDefVal(ColumnType);
            }
        }

        /// <summary>
        /// 公共类型 # ReadOnly
        /// </summary>
        public string CommonType
        {
            get
            {
                return SqlServerDbTypeMap.MapCommonType(ColumnType);
            }
        }
    }

    #endregion

    #region DbProc

    /// <summary>
    /// Db存储过程
    /// </summary>
    public sealed class DbProc
    {
        /// <summary>
        /// 存储过程名称
        /// </summary>
        public string ProcName { get; set; }

        /// <summary>
        /// 参数列表
        /// </summary>
        public List<DbProcParameter> ParameterList { get; set; }
    }

    /// <summary>
    /// Db存储过程参数
    /// </summary>
    public sealed class DbProcParameter
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string ParamName { get; set; }

        /// <summary>
        /// 参数数据库类型
        /// </summary>
        public string ParamDbType { get; set; }

        /// <summary>
        /// 参数长度
        /// </summary>
        public int ParamLength { get; set; }

        /// <summary>
        /// 参数的整数部分位数
        /// </summary>
        public int ParamPrecision { get; set; }

        /// <summary>
        /// 参数的小数部分精度
        /// </summary>
        public int ParamScale { get; set; }

        /// <summary>
        /// 参数对应映射的C#类型
        /// </summary>
        public string ParamCSharpType
        {
            get
            {
                return SqlServerDbTypeMap.MapCsharpType(ParamDbType);
            }
        }

        /// <summary>
        /// 参数是否是返回参数类型
        /// </summary>
        public bool ParamIsOutput { get; set; }

        /// <summary>
        /// 是否是数值类型
        /// </summary>
        public bool IsDecimalNumber
        {
            get
            {
                switch (ParamDbType.ToLower())
                {
                    case "tinyint":
                    case "smallint":
                    case "int":
                    case "money":
                    case "float":
                    case "decimal":
                    case "smallmoney":
                    case "bigint":
                        return true;
                }

                return false;
            }
        }
    }

    #endregion

    #region SqlServerDbTypeMap

    public class SqlServerDbTypeMap
    {
        public static string MapCsharpType(string dbtype)
        {
            if (string.IsNullOrEmpty(dbtype)) return dbtype;
            dbtype = dbtype.ToLower();
            string csharpType = "object";
            switch (dbtype)
            {
                case "bigint": csharpType = "long"; break;
                case "binary": csharpType = "byte[]"; break;
                case "bit": csharpType = "bool"; break;
                case "char": csharpType = "string"; break;
                case "date": csharpType = "DateTime"; break;
                case "datetime": csharpType = "DateTime"; break;
                case "datetime2": csharpType = "DateTime"; break;
                case "datetimeoffset": csharpType = "DateTimeOffset"; break;
                case "decimal": csharpType = "decimal"; break;
                case "float": csharpType = "double"; break;
                case "image": csharpType = "byte[]"; break;
                case "int": csharpType = "int"; break;
                case "money": csharpType = "decimal"; break;
                case "nchar": csharpType = "string"; break;
                case "ntext": csharpType = "string"; break;
                case "numeric": csharpType = "decimal"; break;
                case "nvarchar": csharpType = "string"; break;
                case "real": csharpType = "Single"; break;
                case "smalldatetime": csharpType = "DateTime"; break;
                case "smallint": csharpType = "short"; break;
                case "smallmoney": csharpType = "decimal"; break;
                case "sql_variant": csharpType = "object"; break;
                case "sysname": csharpType = "object"; break;
                case "text": csharpType = "string"; break;
                case "time": csharpType = "TimeSpan"; break;
                case "timestamp": csharpType = "byte[]"; break;
                case "tinyint": csharpType = "byte"; break;
                case "uniqueidentifier": csharpType = "Guid"; break;
                case "varbinary": csharpType = "byte[]"; break;
                case "varchar": csharpType = "string"; break;
                case "xml": csharpType = "string"; break;
                default: csharpType = "object"; break;
            }
            return csharpType;
        }

        public static string MapCsharpDefVal(string dbtype)
        {
            if (string.IsNullOrEmpty(dbtype)) return "null";
            dbtype = dbtype.ToLower();

            string defVal = "null";
            switch (dbtype)
            {
                case "bigint": defVal = "0"; break;
                case "binary": defVal = "null"; break;
                case "bit": defVal = "false"; break;
                case "char": defVal = "string.Empty"; break;
                case "date": defVal = "DateTime.Parse(\"1900-01-01\")"; break;
                case "datetime": defVal = "DateTime.Parse(\"1900-01-01\")"; break;
                case "datetime2": defVal = "DateTime.Parse(\"1900-01-01\")"; break;
                case "datetimeoffset": defVal = "DateTime.Parse(\"1900-01-01\")"; break;
                case "decimal": defVal = "0"; break;
                case "float": defVal = "0"; break;
                case "image": defVal = "null"; break;
                case "int": defVal = "0"; break;
                case "money": defVal = "0"; break;
                case "nchar": defVal = "string.Empty"; break;
                case "ntext": defVal = "string.Empty"; break;
                case "numeric": defVal = "0"; break;
                case "nvarchar": defVal = "string.Empty"; break;
                case "real": defVal = "0"; break;
                case "smalldatetime": defVal = "DateTime.Parse(\"1900-01-01\")"; break;
                case "smallint": defVal = "0"; break;
                case "smallmoney": defVal = "0"; break;
                case "sql_variant": defVal = "null"; break;
                case "sysname": defVal = "null;"; break;
                case "text": defVal = "string.Empty"; break;
                case "time": defVal = "DateTime.Parse(\"1900-01-01\")"; break;
                case "timestamp": defVal = "null"; break;
                case "tinyint": defVal = "0"; break;
                case "uniqueidentifier": defVal = "Guid.Empty"; break;
                case "varbinary": defVal = "null"; break;
                case "varchar": defVal = "string.Empty"; break;
                case "xml": defVal = "string.Empty"; break;
                default: defVal = "string.Empty"; break;
            }
            return defVal;
        }

        public static string MapCommonType(string dbtype)
        {
            string commonType = string.Empty;
            switch (dbtype.ToLower())
            {
                case "bigint":
                    commonType = "BigInt";
                    break;

                case "binary":
                    commonType = "Binary";
                    break;

                case "bit":
                    commonType = "Bit";
                    break;

                case "char":
                    commonType = "Char";
                    break;

                case "date":
                    commonType = "Date";
                    break;

                case "datetime":
                    commonType = "DateTime";
                    break;

                case "datetime2":
                    commonType = "DateTime2";
                    break;

                case "datetimeoffset":
                    commonType = "DateTimeOffset";
                    break;

                case "decimal":
                    commonType = "Decimal";
                    break;

                case "float":
                    commonType = "Float";
                    break;

                case "image":
                    commonType = "Image";
                    break;

                case "int":
                    commonType = "Int";
                    break;

                case "money":
                    commonType = "Money";
                    break;

                case "nchar":
                    commonType = "NChar";
                    break;

                case "ntext":
                    commonType = "NText";
                    break;

                case "numeric":
                    commonType = "Decimal";
                    break;

                case "nvarchar":
                    commonType = "NVarChar";
                    break;

                case "real":
                    commonType = "Real";
                    break;

                case "smalldatetime":
                    commonType = "SmallDateTime";
                    break;

                case "smallint":
                    commonType = "SmallInt";
                    break;

                case "smallmoney":
                    commonType = "SmallMoney";
                    break;

                case "sql_variant":
                    commonType = typeof(object).ToString();
                    break;

                case "sysname":
                    commonType = typeof(object).ToString();
                    break;

                case "text":
                    commonType = "Text";
                    break;

                case "time":
                    commonType = "Time";
                    break;

                case "timestamp":
                    commonType = "Timestamp";
                    break;

                case "tinyint":
                    commonType = "TinyInt";
                    break;

                case "uniqueidentifier":
                    commonType = "UniqueIdentifier";
                    break;

                case "varbinary":
                    commonType = "VarBinary";
                    break;

                case "varchar":
                    commonType = "VarChar";
                    break;

                case "xml":
                    commonType = "Xml";
                    break;

                default:
                    commonType = typeof(object).ToString();
                    break;
            }
            return commonType;
        }
    }

    #endregion

    #region T4FileManager

    public class T4FileManager
    {
        private const string def_schema = "dbo";

        /// <summary>
        /// 执行生成
        /// </summary>
        /// <param name="t4_host_templateFile"></param>
        public static void GenerateORMEntity(string t4_host_templateFile)
        {
            string baseDirName = "DataBase";
            int baseCuteLen = baseDirName.Length + 1;

            //Business程序集里的DataBase文件夹根路径,注意：结果带杠
            string io_dataBaseDirPath = t4_host_templateFile.Remove(t4_host_templateFile.IndexOf(baseDirName) + baseCuteLen);
            //t4模版的根路径,注意：结果带杠
            string io_t4TempBasePath = t4_host_templateFile.Remove(t4_host_templateFile.LastIndexOf('\\') + 1);

            List<DbTable> tableList = DbHelper.GetDbTables(T4Config.global_ConnStr, T4Config.global_DbName);//获取表列表
            List<DbView> viewList = DbHelper.GetDbViews(T4Config.global_ConnStr, T4Config.global_DbName);//获取视图列表

            //开始生成Model + View
            CreateModelFiles(tableList, io_t4TempBasePath, io_dataBaseDirPath);
            CreateViewFiles(viewList, io_t4TempBasePath, io_dataBaseDirPath);

            //创建仓储管理类
            CreateDbRepository(tableList, viewList, io_t4TempBasePath, io_dataBaseDirPath);

            //执行生成存储过程
            List<DbProc> procList = DbHelper.GetProcList(T4Config.global_ConnStr, T4Config.global_DbName);//获取存储过程列表
            CreateDbProcedures(procList, io_t4TempBasePath, io_dataBaseDirPath);
        }

        /// <summary>
        /// 生成Model
        /// </summary>
        /// <param name="tableList">表列表</param>
        /// <param name="io_t4TempBasePath">t4模版的IO路径,结尾带'/'</param>
        /// <param name="io_dataBaseDirPath">Business程序集里的DataBase文件夹根路径,结尾带'/',生成新的文件的时候使用</param>
        private static void CreateModelFiles(List<DbTable> tableList, string io_t4TempBasePath, string io_dataBaseDirPath)
        {
            //基础验证
            if (null == tableList || !tableList.Any() || string.IsNullOrEmpty(io_t4TempBasePath))
                return;

            //初始化模型存储文件夹
            string io_saveDirPath = string.Format("{0}{1}", io_dataBaseDirPath, T4Config.global_modelSaveDir);
            if (!System.IO.Directory.Exists(io_saveDirPath))
                System.IO.Directory.CreateDirectory(io_saveDirPath);

            //拼接Model + Property模版IO路径
            string io_modelPath = string.Format("{0}{1}", io_t4TempBasePath, T4Config.global_modelTemplate);
            string io_propPath = string.Format("{0}{1}", io_t4TempBasePath, T4Config.global_propertyTemplate);
            if (!System.IO.File.Exists(io_modelPath))
                return;
            if (!System.IO.File.Exists(io_propPath))
                return;

            //加载Model + Property模版流
            string tmp_modelContent = string.Empty;
            string tmp_propContent = string.Empty;
            using (StreamReader sr_modelTemp = new StreamReader(io_modelPath, Encoding.UTF8))
            {
                tmp_modelContent = sr_modelTemp.ReadToEnd();
            }
            using (StreamReader sr_propTemp = new StreamReader(io_propPath, Encoding.UTF8))
            {
                tmp_propContent = sr_propTemp.ReadToEnd();
            }


            //开始循环所有表数据
            StringBuilder tableBuilder = null;
            foreach (var tb_item in tableList)
            {
                //获取该表的所有字段,若该表无字段,则跳出本轮循环
                List<DbColumn> colList = DbHelper.GetDbColumns(T4Config.global_ConnStr, T4Config.global_DbName, tb_item.TableName);
                if (null == colList || !colList.Any())
                    continue;

                // 指定当前主键
                if (tb_item.HasPrimaryKey)
                {
                    var pk = colList.FirstOrDefault(d => d.ColumnName == tb_item.PrimaryKeyName);
                    if (null != pk)
                        pk.IsPrimaryKey = true;
                }

                //循环字段开始优先构造属性部分
                StringBuilder propBuilder = new StringBuilder();
                StringBuilder piBuilder = null;
                foreach (var col_item in colList)
                {
                    piBuilder = new StringBuilder(tmp_propContent);
                    piBuilder.Replace("{#propertyName#}", col_item.ColumnName);
                    piBuilder.Replace("{#propertyColumnType#}", col_item.ColumnType);
                    piBuilder.Replace("{#propertyCSharpType#}", col_item.CSharpType);
                    piBuilder.Replace("{#propertyIsPrimaryKey#}", col_item.IsPrimaryKey.ToString().ToLower());
                    piBuilder.Replace("{#propertyIsIdentity#}", col_item.IsIdentity.ToString().ToLower());
                    piBuilder.Replace("{#propertyRemark#}", col_item.Remark);
                    piBuilder.Replace("{#propertyDefVal#}", col_item.CSharpDefVal);

                    propBuilder.AppendLine(piBuilder.ToString());
                }
                if (propBuilder.Length <= 0)
                    propBuilder.Append("该表无字段");


                //初始化表模版数据
                tableBuilder = new StringBuilder(tmp_modelContent);
                tableBuilder.Replace("{#global_namespace#}", T4Config.global_namespace);
                tableBuilder.Replace("{#global_DbName#}", T4Config.global_DbName);
                tableBuilder.Replace("{#tableName#}", tb_item.TableName);
                tableBuilder.Replace("{#tableDesc#}", tb_item.TableDesc);
                tableBuilder.Replace("{#PropertyTemplate#}", propBuilder.ToString());

                //在指定位置保存文件
                string io_savePath = string.Format("{0}{1}/{2}.cs", io_dataBaseDirPath, T4Config.global_modelSaveDir, tb_item.TableName);
                byte[] io_saveByte = Encoding.UTF8.GetBytes(tableBuilder.ToString());
                using (FileStream fs = new FileStream(io_savePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    fs.SetLength(0);
                    fs.Write(io_saveByte, 0, io_saveByte.Length);
                }
            }
        }

        /// <summary>
        /// 生成View
        /// </summary>
        /// <param name="viewList">视图列表</param>
        /// <param name="io_t4TempBasePath">t4模版的IO路径,结尾带'/'</param>
        /// <param name="io_dataBaseDirPath">Business程序集里的DataBase文件夹根路径,结尾带'/',生成新的文件的时候使用</param>
        private static void CreateViewFiles(List<DbView> viewList, string io_t4TempBasePath, string io_dataBaseDirPath)
        {
            //基础验证
            if (null == viewList || !viewList.Any() || string.IsNullOrEmpty(io_t4TempBasePath))
                return;

            //初始化模型存储文件夹
            string io_saveDirPath = string.Format("{0}{1}", io_dataBaseDirPath, T4Config.global_viewSaveDir);
            if (!System.IO.Directory.Exists(io_saveDirPath))
                System.IO.Directory.CreateDirectory(io_saveDirPath);

            //拼接Model + Property模版IO路径
            string io_modelPath = string.Format("{0}{1}", io_t4TempBasePath, T4Config.global_modelTemplate);
            string io_propPath = string.Format("{0}{1}", io_t4TempBasePath, T4Config.global_propertyTemplate);
            if (!System.IO.File.Exists(io_modelPath))
                return;
            if (!System.IO.File.Exists(io_propPath))
                return;

            //加载Model + Property模版流
            string tmp_modelContent = string.Empty;
            string tmp_propContent = string.Empty;
            using (StreamReader sr_modelTemp = new StreamReader(io_modelPath, Encoding.UTF8))
            {
                tmp_modelContent = sr_modelTemp.ReadToEnd();
            }
            using (StreamReader sr_propTemp = new StreamReader(io_propPath, Encoding.UTF8))
            {
                tmp_propContent = sr_propTemp.ReadToEnd();
            }


            //开始循环所有表数据
            StringBuilder tableBuilder = null;
            foreach (var tb_item in viewList)
            {
                //获取该表的所有字段,若该表无字段,则跳出本轮循环
                List<DbColumn> colList = DbHelper.GetDbColumns(T4Config.global_ConnStr, T4Config.global_DbName, tb_item.ViewName);
                if (null == colList || !colList.Any())
                    continue;

                // 指定当前主键
                if (tb_item.HasPrimaryKey)
                {
                    var pk = colList.FirstOrDefault(d => d.ColumnName == tb_item.PrimaryKeyName);
                    if (null != pk)
                        pk.IsPrimaryKey = true;
                }

                //循环字段开始优先构造属性部分
                StringBuilder propBuilder = new StringBuilder();
                StringBuilder piBuilder = null;
                foreach (var col_item in colList)
                {
                    piBuilder = new StringBuilder(tmp_propContent);
                    piBuilder.Replace("{#propertyName#}", col_item.ColumnName);
                    piBuilder.Replace("{#propertyColumnType#}", col_item.ColumnType);
                    piBuilder.Replace("{#propertyCSharpType#}", col_item.CSharpType);
                    piBuilder.Replace("{#propertyIsPrimaryKey#}", col_item.IsPrimaryKey.ToString().ToLower());
                    piBuilder.Replace("{#propertyIsIdentity#}", col_item.IsIdentity.ToString().ToLower());
                    piBuilder.Replace("{#propertyRemark#}", string.IsNullOrEmpty(col_item.Remark) ? col_item.ColumnName : col_item.Remark);
                    piBuilder.Replace("{#propertyDefVal#}", col_item.CSharpDefVal);

                    propBuilder.AppendLine(piBuilder.ToString());
                }
                if (propBuilder.Length <= 0)
                    propBuilder.Append("该表无字段");


                //初始化表模版数据
                tableBuilder = new StringBuilder(tmp_modelContent);
                tableBuilder.Replace("{#global_namespace#}", T4Config.global_namespace);
                tableBuilder.Replace("{#global_DbName#}", T4Config.global_DbName);
                tableBuilder.Replace("{#tableName#}", tb_item.ViewName);
                tableBuilder.Replace("{#tableDesc#}", string.IsNullOrEmpty(tb_item.ViewDesc) ? tb_item.ViewName : tb_item.ViewDesc);
                tableBuilder.Replace("{#PropertyTemplate#}", propBuilder.ToString());

                //在指定位置保存文件
                string io_savePath = string.Format("{0}{1}/{2}.cs", io_dataBaseDirPath, T4Config.global_viewSaveDir, tb_item.ViewName);
                byte[] io_saveByte = Encoding.UTF8.GetBytes(tableBuilder.ToString());
                using (FileStream fs = new FileStream(io_savePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    fs.SetLength(0);
                    fs.Write(io_saveByte, 0, io_saveByte.Length);
                }
            }
        }

        /// <summary>
        /// 生成DbRepository的仓储管理类(生成 BizDbRepository.cs 类)
        /// </summary>
        /// <param name="tableViewList"></param>
        /// <param name="io_t4TempBasePath">t4模版的IO路径,结尾带'/'</param>
        /// <param name="io_dataBaseDirPath">Business程序集里的DataBase文件夹根路径,结尾带'/',生成新的文件的时候使用</param>
        private static void CreateDbRepository(List<DbTable> tableList, List<DbView> viewList, string io_t4TempBasePath, string io_dataBaseDirPath)
        {
            //基础验证
            if (null == tableList || !tableList.Any() || string.IsNullOrEmpty(io_t4TempBasePath))
                return;

            //拼接模版IO路径
            string io_dbReposTempPath = string.Format("{0}{1}", io_t4TempBasePath, T4Config.global_dbRepositoryTemplate);
            string io_dbReposPropTempPath = string.Format("{0}{1}", io_t4TempBasePath, T4Config.global_dbRepositoryPropertyTemplate);
            if (!System.IO.File.Exists(io_dbReposTempPath))
                return;
            if (!System.IO.File.Exists(io_dbReposPropTempPath))
                return;

            //加载模版流
            string tmp_reposContent = string.Empty;
            string tmp_propContent = string.Empty;
            using (StreamReader sr_modelTemp = new StreamReader(io_dbReposTempPath, Encoding.UTF8))
            {
                tmp_reposContent = sr_modelTemp.ReadToEnd();
            }
            using (StreamReader sr_propTemp = new StreamReader(io_dbReposPropTempPath, Encoding.UTF8))
            {
                tmp_propContent = sr_propTemp.ReadToEnd();
            }

            //开始循环所有表和视图开始构造DB数据访问静态属性
            var PropListBuilder = new StringBuilder();
            StringBuilder eachPropBuilder;

            if (null != tableList)
            {
                foreach (var item in tableList)
                {
                    eachPropBuilder = new StringBuilder(tmp_propContent);
                    eachPropBuilder.Replace("{#tableName#}", item.TableName);
                    eachPropBuilder.Replace("{#tableDesc#}", string.IsNullOrEmpty(item.TableDesc) ? item.TableName : item.TableDesc);

                    PropListBuilder.AppendLine(eachPropBuilder.ToString());
                }
            }

            // 开始循环所有的视图
            if (null != viewList)
            {
                foreach (var item in viewList)
                {
                    eachPropBuilder = new StringBuilder(tmp_propContent);
                    eachPropBuilder.Replace("{#tableName#}", item.ViewName);
                    eachPropBuilder.Replace("{#tableDesc#}", string.IsNullOrEmpty(item.ViewDesc) ? item.ViewName : item.ViewDesc);

                    PropListBuilder.AppendLine(eachPropBuilder.ToString());
                }
            }

            //开始初始化类
            StringBuilder classBuilder = new StringBuilder(tmp_reposContent);
            classBuilder.Replace("{#global_namespace#}", T4Config.global_namespace);
            classBuilder.Replace("{#DbRepositoryPropertyTemplate#}", PropListBuilder.ToString());

            //开始在指定位置进行创建或修改文件
            string io_savePath = string.Format("{0}BizDbRepository.cs", io_dataBaseDirPath);
            byte[] io_saveByte = Encoding.UTF8.GetBytes(classBuilder.ToString());
            using (FileStream fs = new FileStream(io_savePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                fs.SetLength(0);
                fs.Write(io_saveByte, 0, io_saveByte.Length);
            }
        }

        /// <summary>
        /// 生成BizDbProcedures.cs的存储过程调用类
        /// </summary>
        /// <param name="tableViewList">存储过程信息列表</param>
        /// <param name="io_t4TempBasePath">t4模版的IO路径,结尾带'/'</param>
        /// <param name="io_dataBaseDirPath">Business程序集里的DataBase文件夹根路径,结尾带'/',生成新的文件的时候使用</param>
        private static void CreateDbProcedures(List<DbProc> tableViewList, string io_t4TempBasePath, string io_dataBaseDirPath)
        {
            //基础验证
            if (null == tableViewList || !tableViewList.Any() || string.IsNullOrEmpty(io_t4TempBasePath))
                return;

            //拼接模版IO路径
            string io_dbProcTempPath = string.Format("{0}{1}", io_t4TempBasePath, T4Config.global_dbProceduresTemplate);
            string io_dbProcFuncTempPath = string.Format("{0}{1}", io_t4TempBasePath, T4Config.global_dbProceduresFuncTemplate);
            if (!System.IO.File.Exists(io_dbProcTempPath))
                return;
            if (!System.IO.File.Exists(io_dbProcFuncTempPath))
                return;

            //加载模版流
            string tmp_procTempContent = string.Empty;
            string tmp_procFuncContent = string.Empty;
            using (StreamReader sr_procTemp = new StreamReader(io_dbProcTempPath, Encoding.UTF8))
            {
                tmp_procTempContent = sr_procTemp.ReadToEnd();
            }
            using (StreamReader sr_procFunc = new StreamReader(io_dbProcFuncTempPath, Encoding.UTF8))
            {
                tmp_procFuncContent = sr_procFunc.ReadToEnd();
            }

            //定义用于接收所有存储过程的容器
            StringBuilder procFuncBuilder = new StringBuilder();

            //开始循环所有的存储过程
            StringBuilder eachProcFuncBuilder = null;
            foreach (var procItem in tableViewList)
            {
                eachProcFuncBuilder = new StringBuilder(tmp_procFuncContent);
                eachProcFuncBuilder.Replace("{#procName#}", procItem.ProcName);

                //判断函数输入参数个数
                if (null == procItem.ParameterList || !procItem.ParameterList.Any())
                {
                    eachProcFuncBuilder.Replace("{#procParams#}", string.Empty);
                    eachProcFuncBuilder.Replace("{#procAsyncParams#}", string.Empty);
                    eachProcFuncBuilder.Replace("{#procParamInputs#}", string.Empty);
                    eachProcFuncBuilder.Replace("{#procParamOutputs#}", string.Empty);
                    eachProcFuncBuilder.Replace("{#procAsyncParamOutputs#}", string.Empty);
                }
                else
                {
                    StringBuilder procFuncArgsBuilder = new StringBuilder();
                    StringBuilder procFuncAsyncArgsBuilder = new StringBuilder();
                    StringBuilder procFuncInputBuilder = new StringBuilder();
                    StringBuilder procFuncOutputBuilder = new StringBuilder();
                    StringBuilder procFuncOutputAsyncBuilder = new StringBuilder();

                    var hasOutparams = procItem.ParameterList.Any(d => d.ParamIsOutput);
                    if (hasOutparams)
                        procFuncOutputAsyncBuilder.AppendLine("// Note that there are return out parameters");

                    foreach (var funParam in procItem.ParameterList)
                    {
                        //拼接同步函数的Args
                        procFuncArgsBuilder.Append(string.Format(
                            "{0}{1} {2},",
                            funParam.ParamIsOutput ? "ref " : string.Empty,
                            funParam.ParamCSharpType,
                            funParam.ParamName)
                        );

                        //拼接异步函数的Args
                        if (!funParam.ParamIsOutput)
                            // 拼接同步函数参数
                            procFuncAsyncArgsBuilder.Append(string.Format(
                                "{0} {1},",
                                funParam.ParamCSharpType,
                                funParam.ParamName)
                            );

                        //拼接Input
                        if (funParam.ParamIsOutput)
                        {
                            if (funParam.IsDecimalNumber)
                                procFuncInputBuilder.AppendLine(string.Format("input.AddParameter(\"{0}\", 0, {1}, {2},MssqlParameterDirection.InputOutput);",
                                    funParam.ParamName,
                                    funParam.ParamPrecision,
                                    funParam.ParamScale
                                ));
                            else
                                procFuncInputBuilder.AppendLine(string.Format("input.AddParameter(\"{0}\", {1}, {2}, MssqlParameterDirection.InputOutput);",
                                    funParam.ParamName,
                                    SqlServerDbTypeMap.MapCsharpDefVal(funParam.ParamDbType),
                                    (funParam.ParamLength > 0 ? funParam.ParamLength : 4000)
                                ));
                        }
                        else
                            procFuncInputBuilder.AppendLine(string.Format("input.AddParameter(\"{0}\", {0});", funParam.ParamName));

                        //拼接output
                        if (funParam.ParamIsOutput)
                        {
                            procFuncOutputBuilder.AppendLine(string.Format(
                                "{0} = ({1})input.GetParamValue(\"{0}\");",
                                funParam.ParamName,
                                funParam.ParamCSharpType)
                            );

                            procFuncOutputAsyncBuilder.AppendLine(string.Format("// outparam : {0}", funParam.ParamName));
                        }
                    }

                    eachProcFuncBuilder.Replace("{#procParams#}", procFuncArgsBuilder.ToString());
                    eachProcFuncBuilder.Replace("{#procAsyncParams#}", procFuncAsyncArgsBuilder.ToString());
                    eachProcFuncBuilder.Replace("{#procParamInputs#}", procFuncInputBuilder.ToString());
                    eachProcFuncBuilder.Replace("{#procParamOutputs#}", procFuncOutputBuilder.ToString());
                    eachProcFuncBuilder.Replace("{#procAsyncParamOutputs#}", procFuncOutputAsyncBuilder.ToString());
                }

                procFuncBuilder.AppendLine(eachProcFuncBuilder.ToString());
            }

            //开始处理存储过程调用类模版相关数据
            StringBuilder classBuilder = new StringBuilder(tmp_procTempContent);
            classBuilder.Replace("{#global_namespace#}", T4Config.global_namespace);
            classBuilder.Replace("{#global_DbName#}", T4Config.global_DbName);
            classBuilder.Replace("{#procFunctions#}", procFuncBuilder.ToString());

            //开始在指定位置进行创建或修改文件
            string io_savePath = string.Format("{0}BizDbProcedures.cs", io_dataBaseDirPath);
            byte[] io_saveByte = Encoding.UTF8.GetBytes(classBuilder.ToString());
            using (FileStream fs = new FileStream(io_savePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                fs.SetLength(0);
                fs.Write(io_saveByte, 0, io_saveByte.Length);
            }
        }
    }

    #endregion
}
