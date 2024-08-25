﻿using ClickHouse.Client.ADO;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;

namespace AtomicCore.Integration.ClickHouseDbProviderUnitTest
{
    #region T4Config

    public class T4Config
    {
        public const string global_DbName = "default";
        public const string global_namespace = "AtomicCore.Integration.ClickHouseDbProviderUnitTest";
        public const string global_dbPrefix = "ClickHouse";
        public static string global_ConnStr = $"Host=127.0.0.1;Port=8123;Username={global_DbName};Password=123456;Database=default";

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

        #region SqlInvoke

        private static DataTable SqlInvoke(string connectionString, string commandText)
        {
            DataTable dt = new DataTable();

            using (ClickHouseConnection connection = new ClickHouseConnection(connectionString))
            {
                connection.Open();
                using (ClickHouseCommand command = connection.CreateCommand())
                {
                    command.CommandText = commandText;

                    using (DbDataReader reader = command.ExecuteReader())
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
                return ClickHouseDbTypeMap.MapCsharpTypeForClickHouse(ColumnType);
            }
        }

        /// <summary>
        /// 数据库类型对应的C#类型默认值 # ReadOnly
        /// </summary>
        public string CSharpDefVal
        {
            get
            {
                return ClickHouseDbTypeMap.MapCsharpDefValForClickHouse(ColumnType);
            }
        }

        /// <summary>
        /// 公共类型 # ReadOnly
        /// </summary>
        public string CommonType
        {
            get
            {
                return ClickHouseDbTypeMap.MapCommonTypeForClickHouse(ColumnType);
            }
        }
    }

    #endregion

    #region ClickHouseDbTypeMap

    public class ClickHouseDbTypeMap
    {
        public static string MapCsharpTypeForClickHouse(string dbtype)
        {
            if (string.IsNullOrEmpty(dbtype)) return "object";
            dbtype = dbtype.ToLower();
            string csharpType;

            switch (dbtype)
            {
                case "int8": csharpType = "sbyte"; break;
                case "int16": csharpType = "short"; break;
                case "int32": csharpType = "int"; break;
                case "int64": csharpType = "long"; break;
                case "uint8": csharpType = "byte"; break;
                case "uint16": csharpType = "ushort"; break;
                case "uint32": csharpType = "uint"; break;
                case "uint64": csharpType = "ulong"; break;
                case "float32": csharpType = "float"; break;
                case "float64": csharpType = "double"; break;
                case "decimal": csharpType = "decimal"; break;
                case "string": csharpType = "string"; break;
                case "fixedstring": csharpType = "string"; break;
                case "date": csharpType = "DateTime"; break;
                case "datetime": csharpType = "DateTime"; break;
                case "datetime64": csharpType = "DateTime"; break;
                case "uuid": csharpType = "Guid"; break;
                case "bool": csharpType = "bool"; break;
                case "boolean": csharpType = "bool"; break;
                case "array": csharpType = "Array"; break;
                case "nullable(int8)": csharpType = "sbyte?"; break;
                case "nullable(int16)": csharpType = "short?"; break;
                case "nullable(int32)": csharpType = "int?"; break;
                case "nullable(int64)": csharpType = "long?"; break;
                case "nullable(uint8)": csharpType = "byte?"; break;
                case "nullable(uint16)": csharpType = "ushort?"; break;
                case "nullable(uint32)": csharpType = "uint?"; break;
                case "nullable(uint64)": csharpType = "ulong?"; break;
                case "nullable(float32)": csharpType = "float?"; break;
                case "nullable(float64)": csharpType = "double?"; break;
                case "nullable(decimal)": csharpType = "decimal?"; break;
                case "nullable(string)": csharpType = "string"; break;
                case "nullable(date)": csharpType = "DateTime?"; break;
                case "nullable(datetime)": csharpType = "DateTime?"; break;
                case "nullable(datetime64)": csharpType = "DateTime?"; break;
                case "nullable(uuid)": csharpType = "Guid?"; break;
                case "nullable(bool)": csharpType = "bool?"; break;
                case "nullable(boolean)": csharpType = "bool?"; break;
                default: csharpType = "object"; break;
            }

            return csharpType;
        }

        public static string MapCsharpDefValForClickHouse(string dbtype)
        {
            if (string.IsNullOrEmpty(dbtype)) return "null";
            dbtype = dbtype.ToLower();

            string defVal = "null";
            switch (dbtype)
            {
                case "int8": defVal = "0"; break;
                case "int16": defVal = "0"; break;
                case "int32": defVal = "0"; break;
                case "int64": defVal = "0"; break;
                case "uint8": defVal = "0"; break;
                case "uint16": defVal = "0"; break;
                case "uint32": defVal = "0"; break;
                case "uint64": defVal = "0"; break;
                case "float32": defVal = "0f"; break;
                case "float64": defVal = "0d"; break;
                case "decimal": defVal = "0m"; break;
                case "string": defVal = "string.Empty"; break;
                case "fixedstring": defVal = "string.Empty"; break;
                case "date": defVal = "DateTime.Parse(\"1900-01-01\")"; break;
                case "datetime": defVal = "DateTime.Parse(\"1900-01-01\")"; break;
                case "datetime64": defVal = "DateTime.Parse(\"1900-01-01\")"; break;
                case "uuid": defVal = "Guid.Empty"; break;
                case "bool": defVal = "false"; break;
                case "boolean": defVal = "false"; break;
                case "array": defVal = "Array.Empty<object>()"; break;
                case "nullable(int8)": defVal = "null"; break;
                case "nullable(int16)": defVal = "null"; break;
                case "nullable(int32)": defVal = "null"; break;
                case "nullable(int64)": defVal = "null"; break;
                case "nullable(uint8)": defVal = "null"; break;
                case "nullable(uint16)": defVal = "null"; break;
                case "nullable(uint32)": defVal = "null"; break;
                case "nullable(uint64)": defVal = "null"; break;
                case "nullable(float32)": defVal = "null"; break;
                case "nullable(float64)": defVal = "null"; break;
                case "nullable(decimal)": defVal = "null"; break;
                case "nullable(string)": defVal = "null"; break;
                case "nullable(date)": defVal = "null"; break;
                case "nullable(datetime)": defVal = "null"; break;
                case "nullable(datetime64)": defVal = "null"; break;
                case "nullable(uuid)": defVal = "null"; break;
                case "nullable(bool)": defVal = "null"; break;
                case "nullable(boolean)": defVal = "null"; break;
                default: defVal = "null"; break;
            }
            return defVal;
        }

        public static string MapCommonTypeForClickHouse(string dbtype)
        {
            string commonType = string.Empty;
            switch (dbtype.ToLower())
            {
                case "int8":
                    commonType = "Int8";
                    break;

                case "int16":
                    commonType = "Int16";
                    break;

                case "int32":
                    commonType = "Int32";
                    break;

                case "int64":
                    commonType = "Int64";
                    break;

                case "uint8":
                    commonType = "UInt8";
                    break;

                case "uint16":
                    commonType = "UInt16";
                    break;

                case "uint32":
                    commonType = "UInt32";
                    break;

                case "uint64":
                    commonType = "UInt64";
                    break;

                case "float32":
                    commonType = "Float32";
                    break;

                case "float64":
                    commonType = "Float64";
                    break;

                case "decimal":
                    commonType = "Decimal";
                    break;

                case "string":
                    commonType = "String";
                    break;

                case "fixedstring":
                    commonType = "FixedString";
                    break;

                case "date":
                    commonType = "Date";
                    break;

                case "datetime":
                    commonType = "DateTime";
                    break;

                case "datetime64":
                    commonType = "DateTime64";
                    break;

                case "uuid":
                    commonType = "UUID";
                    break;

                case "bool":
                    commonType = "Boolean";
                    break;

                case "boolean":
                    commonType = "Boolean";
                    break;

                case "array":
                    commonType = "Array";
                    break;

                case "nullable":
                    commonType = "Nullable";
                    break;

                case "tuple":
                    commonType = "Tuple";
                    break;

                case "map":
                    commonType = "Map";
                    break;

                case "enum8":
                    commonType = "Enum8";
                    break;

                case "enum16":
                    commonType = "Enum16";
                    break;

                case "lowcardinality":
                    commonType = "LowCardinality";
                    break;

                case "nothing":
                    commonType = "Nothing";
                    break;

                case "aggregatefunction":
                    commonType = "AggregateFunction";
                    break;

                case "simpleaggregatefunction":
                    commonType = "SimpleAggregateFunction";
                    break;

                case "interval":
                    commonType = "Interval";
                    break;

                case "uuid4":
                    commonType = "UUID";
                    break;

                default:
                    commonType = "Unknown";
                    break;
            }
            return commonType;
        }
    }

    #endregion

    #region T4FileManager

    public class T4FileManager
    {
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
            classBuilder.Replace("{#global_dbPrefix#}", T4Config.global_dbPrefix);
            classBuilder.Replace("{#DbRepositoryPropertyTemplate#}", PropListBuilder.ToString());

            //开始在指定位置进行创建或修改文件
            string io_savePath = string.Format("{0}Biz{1}DbRepository.cs", io_dataBaseDirPath, T4Config.global_dbPrefix);
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
