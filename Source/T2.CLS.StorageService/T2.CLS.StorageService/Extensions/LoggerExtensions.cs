using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using T2.CLS.StorageService.Properties;

namespace T2.CLS.StorageService.Extensions
{
    public static class LoggerExtensions
    {
		private static readonly Action<ILogger, Exception> _SML00001_Debug_Start_Create_ODBC_Connection;
		private static readonly Action<ILogger, Exception> _SML00002_Debug_Open_ODBC_Connection;
		private static readonly Action<ILogger, Exception> _SML00003_Debug_Create_new_Data_Base;
		private static readonly Action<ILogger, Exception> _SML00004_Debug_DataBase_changed;
		private static readonly Action<ILogger, Exception> _SML00005_Debug_Create_Log_SystemEntry;
		private static readonly Action<ILogger, object, Exception> _SML00006_Trace_SQL_commandText;
		private static readonly Action<ILogger, object, object, Exception> _SML00007_Information_Log_system_entry_created_SystemName_systemConfigurationName_SystemId_systemConfigurationId;
		private static readonly Action<ILogger, object, object, Exception> _SML00008_Error_LogSystem_entry_creation_failed_SystemName_systemConfigurationName_SystemId_systemConfigurationId;
		private static readonly Action<ILogger, object, Exception> _SML00009_Error_Can_not_open_connection_ConnectionString_configurationConnectionString;
		private static readonly Action<ILogger, object, object, Exception> _SML00010_Error_LogTable_creation_failed_SystemName_logSystemName_TableSchema_n_tableSchema;
		private static readonly Action<ILogger, object, Exception> _SML00011_Trace_SQL_sqlQuery;
		private static readonly Action<ILogger, object, object, Exception> _SML00012_Information_Log_table_entry_created_SystemName_systemName_TableName_tableName;
		private static readonly Action<ILogger, object, object, Exception> _SML00013_Error_LogTable_entry_creation_failed_SystemName_systemName_TableName_tableName;
		private static readonly Action<ILogger, Exception> _SML00014_Debug_Create_Log_Table_Entry;
		private static readonly Action<ILogger, object, Exception> _SML00015_Trace_SQL_commandText;
		private static readonly Action<ILogger, Exception> _SML00016_Debug_Create_Table;
		private static readonly Action<ILogger, object, Exception> _SML00017_Information_Table_tableName_created;
		private static readonly Action<ILogger, object, Exception> _SML00018_Error_Table_tableName_creation_failed;
		private static readonly Action<ILogger, Exception> _SML00019_Debug_Drop_Table;
		private static readonly Action<ILogger, object, Exception> _SML00020_Trace_SQL_commandText;
		private static readonly Action<ILogger, Exception> _SML00021_Debug_Ensure_Log_System;
		private static readonly Action<ILogger, object, Exception> _SML00022_Trace_SQL_commandText;
		private static readonly Action<ILogger, object, Exception> _SML00023_Debug_Found_System_Configuration_Id_systemConfigurationId;
		private static readonly Action<ILogger, object, Exception> _SML00024_Error_LogSystem_configuration_failed_SystemName_systemConfigurationName;
		private static readonly Action<ILogger, Exception> _SML00025_Debug_Find_Table;
		private static readonly Action<ILogger, object, Exception> _SML00026_Trace_SQL_commandText;
		private static readonly Action<ILogger, object, Exception> _SML00027_Information_Existing_table_tableName_found;
		private static readonly Action<ILogger, Exception> _SML00028_Debug_Get_Log_Tables;
		private static readonly Action<ILogger, object, Exception> _SML00029_Trace_SQL_commandText;
		private static readonly Action<ILogger, Exception> _SML00030_Debug_Get_Log_Tables_By_System_Name;
		private static readonly Action<ILogger, object, Exception> _SML00031_Trace_SQL_commandText;
		private static readonly Action<ILogger, Exception> _SML00032_Debug_InitStorage;
		private static readonly Action<ILogger, object, Exception> _SML00033_Trace_SQL_commandText;
		private static readonly Action<ILogger, object, Exception> _SML00034_Trace_SQL_commandText;
		private static readonly Action<ILogger, object, Exception> _SML00035_Trace_SQL_commandText;
		private static readonly Action<ILogger, object, Exception> _SML00036_Trace_SQL_commandText;
		private static readonly Action<ILogger, Exception> _SML00037_Error_Error_Init_storage;
		private static readonly Action<ILogger, Exception> _SML00038_Debug_Is_Log_Table;
		private static readonly Action<ILogger, object, Exception> _SML00039_Trace_SQL_commandText;
		private static readonly Action<ILogger, object, object, Exception> _SML00040_Debug_TableName_is_log_table_isLog;
		private static readonly Action<ILogger, Exception> _SML00041_Debug_Rename_Log_Table;
		private static readonly Action<ILogger, object, Exception> _SML00042_Trace_SQL_commandText;
		private static readonly Action<ILogger, object, object, Exception> _SML00043_Information_LogTable_entry_updated_Old_TableName_oldTableName_new_TableName_newTableName;
		private static readonly Action<ILogger, object, object, Exception> _SML00044_Error_Renaming_LogTable_failed_Old_TableName_oldTableName_new_TableName_newTableName;
		private static readonly Action<ILogger, Exception> _SML00045_Debug_Rename_Table;
		private static readonly Action<ILogger, object, Exception> _SML00046_Trace_SQL_commandText;
		private static readonly Action<ILogger, object, object, Exception> _SML00047_Information_Table_oldTableName_renamed_to_newTableName;
		private static readonly Action<ILogger, object, object, Exception> _SML00048_Error_Renaming_table_failed_Old_TableName_oldTableName_new_TableName_newTableName;
		private static readonly Action<ILogger, Exception> _SML00049_Debug_Update_Log_Table;
		private static readonly Action<ILogger, object, Exception> _SML00050_Error_Updating_LogTable_failed_SystemName_systemConfigurationName;


        static LoggerExtensions()
        {
            _SML00001_Debug_Start_Create_ODBC_Connection = LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(1, nameof(Logs.SML00001)),
                Logs.SML00001);
            _SML00002_Debug_Open_ODBC_Connection = LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(2, nameof(Logs.SML00002)),
                Logs.SML00002);
            _SML00003_Debug_Create_new_Data_Base = LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(3, nameof(Logs.SML00003)),
                Logs.SML00003);
            _SML00004_Debug_DataBase_changed = LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(4, nameof(Logs.SML00004)),
                Logs.SML00004);
            _SML00005_Debug_Create_Log_SystemEntry = LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(5, nameof(Logs.SML00005)),
                Logs.SML00005);
            _SML00006_Trace_SQL_commandText = LoggerMessage.Define<object>(
                LogLevel.Trace,
                new EventId(6, nameof(Logs.SML00006)),
                Logs.SML00006);
            _SML00007_Information_Log_system_entry_created_SystemName_systemConfigurationName_SystemId_systemConfigurationId = LoggerMessage.Define<object, object>(
                LogLevel.Information,
                new EventId(7, nameof(Logs.SML00007)),
                Logs.SML00007);
            _SML00008_Error_LogSystem_entry_creation_failed_SystemName_systemConfigurationName_SystemId_systemConfigurationId = LoggerMessage.Define<object, object>(
                LogLevel.Error,
                new EventId(8, nameof(Logs.SML00008)),
                Logs.SML00008);
            _SML00009_Error_Can_not_open_connection_ConnectionString_configurationConnectionString = LoggerMessage.Define<object>(
                LogLevel.Error,
                new EventId(9, nameof(Logs.SML00009)),
                Logs.SML00009);
            _SML00010_Error_LogTable_creation_failed_SystemName_logSystemName_TableSchema_n_tableSchema = LoggerMessage.Define<object, object>(
                LogLevel.Error,
                new EventId(10, nameof(Logs.SML00010)),
                Logs.SML00010);
            _SML00011_Trace_SQL_sqlQuery = LoggerMessage.Define<object>(
                LogLevel.Trace,
                new EventId(11, nameof(Logs.SML00011)),
                Logs.SML00011);
            _SML00012_Information_Log_table_entry_created_SystemName_systemName_TableName_tableName = LoggerMessage.Define<object, object>(
                LogLevel.Information,
                new EventId(12, nameof(Logs.SML00012)),
                Logs.SML00012);
            _SML00013_Error_LogTable_entry_creation_failed_SystemName_systemName_TableName_tableName = LoggerMessage.Define<object, object>(
                LogLevel.Error,
                new EventId(13, nameof(Logs.SML00013)),
                Logs.SML00013);
            _SML00014_Debug_Create_Log_Table_Entry = LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(14, nameof(Logs.SML00014)),
                Logs.SML00014);
            _SML00015_Trace_SQL_commandText = LoggerMessage.Define<object>(
                LogLevel.Trace,
                new EventId(15, nameof(Logs.SML00015)),
                Logs.SML00015);
            _SML00016_Debug_Create_Table = LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(16, nameof(Logs.SML00016)),
                Logs.SML00016);
            _SML00017_Information_Table_tableName_created = LoggerMessage.Define<object>(
                LogLevel.Information,
                new EventId(17, nameof(Logs.SML00017)),
                Logs.SML00017);
            _SML00018_Error_Table_tableName_creation_failed = LoggerMessage.Define<object>(
                LogLevel.Error,
                new EventId(18, nameof(Logs.SML00018)),
                Logs.SML00018);
            _SML00019_Debug_Drop_Table = LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(19, nameof(Logs.SML00019)),
                Logs.SML00019);
            _SML00020_Trace_SQL_commandText = LoggerMessage.Define<object>(
                LogLevel.Trace,
                new EventId(20, nameof(Logs.SML00020)),
                Logs.SML00020);
            _SML00021_Debug_Ensure_Log_System = LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(21, nameof(Logs.SML00021)),
                Logs.SML00021);
            _SML00022_Trace_SQL_commandText = LoggerMessage.Define<object>(
                LogLevel.Trace,
                new EventId(22, nameof(Logs.SML00022)),
                Logs.SML00022);
            _SML00023_Debug_Found_System_Configuration_Id_systemConfigurationId = LoggerMessage.Define<object>(
                LogLevel.Debug,
                new EventId(23, nameof(Logs.SML00023)),
                Logs.SML00023);
            _SML00024_Error_LogSystem_configuration_failed_SystemName_systemConfigurationName = LoggerMessage.Define<object>(
                LogLevel.Error,
                new EventId(24, nameof(Logs.SML00024)),
                Logs.SML00024);
            _SML00025_Debug_Find_Table = LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(25, nameof(Logs.SML00025)),
                Logs.SML00025);
            _SML00026_Trace_SQL_commandText = LoggerMessage.Define<object>(
                LogLevel.Trace,
                new EventId(26, nameof(Logs.SML00026)),
                Logs.SML00026);
            _SML00027_Information_Existing_table_tableName_found = LoggerMessage.Define<object>(
                LogLevel.Information,
                new EventId(27, nameof(Logs.SML00027)),
                Logs.SML00027);
            _SML00028_Debug_Get_Log_Tables = LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(28, nameof(Logs.SML00028)),
                Logs.SML00028);
            _SML00029_Trace_SQL_commandText = LoggerMessage.Define<object>(
                LogLevel.Trace,
                new EventId(29, nameof(Logs.SML00029)),
                Logs.SML00029);
            _SML00030_Debug_Get_Log_Tables_By_System_Name = LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(30, nameof(Logs.SML00030)),
                Logs.SML00030);
            _SML00031_Trace_SQL_commandText = LoggerMessage.Define<object>(
                LogLevel.Trace,
                new EventId(31, nameof(Logs.SML00031)),
                Logs.SML00031);
            _SML00032_Debug_InitStorage = LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(32, nameof(Logs.SML00032)),
                Logs.SML00032);
            _SML00033_Trace_SQL_commandText = LoggerMessage.Define<object>(
                LogLevel.Trace,
                new EventId(33, nameof(Logs.SML00033)),
                Logs.SML00033);
            _SML00034_Trace_SQL_commandText = LoggerMessage.Define<object>(
                LogLevel.Trace,
                new EventId(34, nameof(Logs.SML00034)),
                Logs.SML00034);
            _SML00035_Trace_SQL_commandText = LoggerMessage.Define<object>(
                LogLevel.Trace,
                new EventId(35, nameof(Logs.SML00035)),
                Logs.SML00035);
            _SML00036_Trace_SQL_commandText = LoggerMessage.Define<object>(
                LogLevel.Trace,
                new EventId(36, nameof(Logs.SML00036)),
                Logs.SML00036);
            _SML00037_Error_Error_Init_storage = LoggerMessage.Define(
                LogLevel.Error,
                new EventId(37, nameof(Logs.SML00037)),
                Logs.SML00037);
            _SML00038_Debug_Is_Log_Table = LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(38, nameof(Logs.SML00038)),
                Logs.SML00038);
            _SML00039_Trace_SQL_commandText = LoggerMessage.Define<object>(
                LogLevel.Trace,
                new EventId(39, nameof(Logs.SML00039)),
                Logs.SML00039);
            _SML00040_Debug_TableName_is_log_table_isLog = LoggerMessage.Define<object, object>(
                LogLevel.Debug,
                new EventId(40, nameof(Logs.SML00040)),
                Logs.SML00040);
            _SML00041_Debug_Rename_Log_Table = LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(41, nameof(Logs.SML00041)),
                Logs.SML00041);
            _SML00042_Trace_SQL_commandText = LoggerMessage.Define<object>(
                LogLevel.Trace,
                new EventId(42, nameof(Logs.SML00042)),
                Logs.SML00042);
            _SML00043_Information_LogTable_entry_updated_Old_TableName_oldTableName_new_TableName_newTableName = LoggerMessage.Define<object, object>(
                LogLevel.Information,
                new EventId(43, nameof(Logs.SML00043)),
                Logs.SML00043);
            _SML00044_Error_Renaming_LogTable_failed_Old_TableName_oldTableName_new_TableName_newTableName = LoggerMessage.Define<object, object>(
                LogLevel.Error,
                new EventId(44, nameof(Logs.SML00044)),
                Logs.SML00044);
            _SML00045_Debug_Rename_Table = LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(45, nameof(Logs.SML00045)),
                Logs.SML00045);
            _SML00046_Trace_SQL_commandText = LoggerMessage.Define<object>(
                LogLevel.Trace,
                new EventId(46, nameof(Logs.SML00046)),
                Logs.SML00046);
            _SML00047_Information_Table_oldTableName_renamed_to_newTableName = LoggerMessage.Define<object, object>(
                LogLevel.Information,
                new EventId(47, nameof(Logs.SML00047)),
                Logs.SML00047);
            _SML00048_Error_Renaming_table_failed_Old_TableName_oldTableName_new_TableName_newTableName = LoggerMessage.Define<object, object>(
                LogLevel.Error,
                new EventId(48, nameof(Logs.SML00048)),
                Logs.SML00048);
            _SML00049_Debug_Update_Log_Table = LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(49, nameof(Logs.SML00049)),
                Logs.SML00049);
            _SML00050_Error_Updating_LogTable_failed_SystemName_systemConfigurationName = LoggerMessage.Define<object>(
                LogLevel.Error,
                new EventId(50, nameof(Logs.SML00050)),
                Logs.SML00050);

        }
        
        /// <summary>
        /// Start Create ODBC Connection
        /// </summary>
        public static void SML00001_Debug_Start_Create_ODBC_Connection(this ILogger logger)
        {
            _SML00001_Debug_Start_Create_ODBC_Connection(logger, null);
        }

        /// <summary>
        /// Open ODBC Connection
        /// </summary>
        public static void SML00002_Debug_Open_ODBC_Connection(this ILogger logger)
        {
            _SML00002_Debug_Open_ODBC_Connection(logger, null);
        }

        /// <summary>
        /// Create new Data Base
        /// </summary>
        public static void SML00003_Debug_Create_new_Data_Base(this ILogger logger)
        {
            _SML00003_Debug_Create_new_Data_Base(logger, null);
        }

        /// <summary>
        /// DataBase changed
        /// </summary>
        public static void SML00004_Debug_DataBase_changed(this ILogger logger)
        {
            _SML00004_Debug_DataBase_changed(logger, null);
        }

        /// <summary>
        /// Create Log SystemEntry
        /// </summary>
        public static void SML00005_Debug_Create_Log_SystemEntry(this ILogger logger)
        {
            _SML00005_Debug_Create_Log_SystemEntry(logger, null);
        }

        /// <summary>
        /// SQL>> {commandText}
        /// </summary>
        public static void SML00006_Trace_SQL_commandText(this ILogger logger, object commandText)
        {
            _SML00006_Trace_SQL_commandText(logger, commandText, null);
        }

        /// <summary>
        /// Log system entry created. SystemName='{systemConfigurationName}', SystemId='{systemConfigurationId}'.
        /// </summary>
        public static void SML00007_Information_Log_system_entry_created_SystemName_systemConfigurationName_SystemId_systemConfigurationId(this ILogger logger, object systemConfigurationName, object systemConfigurationId)
        {
            _SML00007_Information_Log_system_entry_created_SystemName_systemConfigurationName_SystemId_systemConfigurationId(logger, systemConfigurationName, systemConfigurationId, null);
        }

        /// <summary>
        /// LogSystem entry creation failed. SystemName='{systemConfigurationName}', SystemId='{systemConfigurationId}'
        /// </summary>
        public static void SML00008_Error_LogSystem_entry_creation_failed_SystemName_systemConfigurationName_SystemId_systemConfigurationId(this ILogger logger, object systemConfigurationName, object systemConfigurationId, Exception ex)
        {
            _SML00008_Error_LogSystem_entry_creation_failed_SystemName_systemConfigurationName_SystemId_systemConfigurationId(logger, systemConfigurationName, systemConfigurationId, ex);
        }

        /// <summary>
        /// Can not open connection. ConnectionString='{configurationConnectionString}'.
        /// </summary>
        public static void SML00009_Error_Can_not_open_connection_ConnectionString_configurationConnectionString(this ILogger logger, object configurationConnectionString, Exception ex)
        {
            _SML00009_Error_Can_not_open_connection_ConnectionString_configurationConnectionString(logger, configurationConnectionString, ex);
        }

        /// <summary>
        /// LogTable creation failed. SystemName='{logSystemName}', TableSchema:\n{tableSchema}
        /// </summary>
        public static void SML00010_Error_LogTable_creation_failed_SystemName_logSystemName_TableSchema_n_tableSchema(this ILogger logger, object logSystemName, object tableSchema, Exception ex)
        {
            _SML00010_Error_LogTable_creation_failed_SystemName_logSystemName_TableSchema_n_tableSchema(logger, logSystemName, tableSchema, ex);
        }

        /// <summary>
        /// SQL>> {sqlQuery}
        /// </summary>
        public static void SML00011_Trace_SQL_sqlQuery(this ILogger logger, object sqlQuery)
        {
            _SML00011_Trace_SQL_sqlQuery(logger, sqlQuery, null);
        }

        /// <summary>
        /// Log table entry created. SystemName='{systemName}', TableName='{tableName}'.
        /// </summary>
        public static void SML00012_Information_Log_table_entry_created_SystemName_systemName_TableName_tableName(this ILogger logger, object systemName, object tableName)
        {
            _SML00012_Information_Log_table_entry_created_SystemName_systemName_TableName_tableName(logger, systemName, tableName, null);
        }

        /// <summary>
        /// LogTable entry creation failed. SystemName='{systemName}', TableName='{tableName}'
        /// </summary>
        public static void SML00013_Error_LogTable_entry_creation_failed_SystemName_systemName_TableName_tableName(this ILogger logger, object systemName, object tableName, Exception ex)
        {
            _SML00013_Error_LogTable_entry_creation_failed_SystemName_systemName_TableName_tableName(logger, systemName, tableName, ex);
        }

        /// <summary>
        /// Create Log Table Entry 
        /// </summary>
        public static void SML00014_Debug_Create_Log_Table_Entry(this ILogger logger)
        {
            _SML00014_Debug_Create_Log_Table_Entry(logger, null);
        }

        /// <summary>
        /// SQL>> {commandText}
        /// </summary>
        public static void SML00015_Trace_SQL_commandText(this ILogger logger, object commandText)
        {
            _SML00015_Trace_SQL_commandText(logger, commandText, null);
        }

        /// <summary>
        /// Create Table
        /// </summary>
        public static void SML00016_Debug_Create_Table(this ILogger logger)
        {
            _SML00016_Debug_Create_Table(logger, null);
        }

        /// <summary>
        /// Table {tableName} created.
        /// </summary>
        public static void SML00017_Information_Table_tableName_created(this ILogger logger, object tableName)
        {
            _SML00017_Information_Table_tableName_created(logger, tableName, null);
        }

        /// <summary>
        /// Table {tableName} creation failed.
        /// </summary>
        public static void SML00018_Error_Table_tableName_creation_failed(this ILogger logger, object tableName, Exception ex)
        {
            _SML00018_Error_Table_tableName_creation_failed(logger, tableName, ex);
        }

        /// <summary>
        /// Drop Table
        /// </summary>
        public static void SML00019_Debug_Drop_Table(this ILogger logger)
        {
            _SML00019_Debug_Drop_Table(logger, null);
        }

        /// <summary>
        /// SQL>> {commandText}
        /// </summary>
        public static void SML00020_Trace_SQL_commandText(this ILogger logger, object commandText)
        {
            _SML00020_Trace_SQL_commandText(logger, commandText, null);
        }

        /// <summary>
        /// Ensure Log System
        /// </summary>
        public static void SML00021_Debug_Ensure_Log_System(this ILogger logger)
        {
            _SML00021_Debug_Ensure_Log_System(logger, null);
        }

        /// <summary>
        /// SQL>> {commandText}
        /// </summary>
        public static void SML00022_Trace_SQL_commandText(this ILogger logger, object commandText)
        {
            _SML00022_Trace_SQL_commandText(logger, commandText, null);
        }

        /// <summary>
        /// Found System Configuration. Id: {systemConfigurationId}
        /// </summary>
        public static void SML00023_Debug_Found_System_Configuration_Id_systemConfigurationId(this ILogger logger, object systemConfigurationId)
        {
            _SML00023_Debug_Found_System_Configuration_Id_systemConfigurationId(logger, systemConfigurationId, null);
        }

        /// <summary>
        /// LogSystem configuration failed. SystemName='{systemConfigurationName}'.
        /// </summary>
        public static void SML00024_Error_LogSystem_configuration_failed_SystemName_systemConfigurationName(this ILogger logger, object systemConfigurationName, Exception ex)
        {
            _SML00024_Error_LogSystem_configuration_failed_SystemName_systemConfigurationName(logger, systemConfigurationName, ex);
        }

        /// <summary>
        /// Find Table
        /// </summary>
        public static void SML00025_Debug_Find_Table(this ILogger logger)
        {
            _SML00025_Debug_Find_Table(logger, null);
        }

        /// <summary>
        /// SQL>> {commandText}
        /// </summary>
        public static void SML00026_Trace_SQL_commandText(this ILogger logger, object commandText)
        {
            _SML00026_Trace_SQL_commandText(logger, commandText, null);
        }

        /// <summary>
        /// Existing table '{tableName}' found.
        /// </summary>
        public static void SML00027_Information_Existing_table_tableName_found(this ILogger logger, object tableName)
        {
            _SML00027_Information_Existing_table_tableName_found(logger, tableName, null);
        }

        /// <summary>
        /// Get Log Tables
        /// </summary>
        public static void SML00028_Debug_Get_Log_Tables(this ILogger logger)
        {
            _SML00028_Debug_Get_Log_Tables(logger, null);
        }

        /// <summary>
        /// SQL>> {commandText}
        /// </summary>
        public static void SML00029_Trace_SQL_commandText(this ILogger logger, object commandText)
        {
            _SML00029_Trace_SQL_commandText(logger, commandText, null);
        }

        /// <summary>
        /// Get Log Tables By System Name
        /// </summary>
        public static void SML00030_Debug_Get_Log_Tables_By_System_Name(this ILogger logger)
        {
            _SML00030_Debug_Get_Log_Tables_By_System_Name(logger, null);
        }

        /// <summary>
        /// SQL>> {commandText}
        /// </summary>
        public static void SML00031_Trace_SQL_commandText(this ILogger logger, object commandText)
        {
            _SML00031_Trace_SQL_commandText(logger, commandText, null);
        }

        /// <summary>
        /// InitStorage
        /// </summary>
        public static void SML00032_Debug_InitStorage(this ILogger logger)
        {
            _SML00032_Debug_InitStorage(logger, null);
        }

        /// <summary>
        /// SQL>> {commandText}
        /// </summary>
        public static void SML00033_Trace_SQL_commandText(this ILogger logger, object commandText)
        {
            _SML00033_Trace_SQL_commandText(logger, commandText, null);
        }

        /// <summary>
        /// SQL>> {commandText}
        /// </summary>
        public static void SML00034_Trace_SQL_commandText(this ILogger logger, object commandText)
        {
            _SML00034_Trace_SQL_commandText(logger, commandText, null);
        }

        /// <summary>
        /// SQL>> {commandText}
        /// </summary>
        public static void SML00035_Trace_SQL_commandText(this ILogger logger, object commandText)
        {
            _SML00035_Trace_SQL_commandText(logger, commandText, null);
        }

        /// <summary>
        /// SQL>> {commandText}
        /// </summary>
        public static void SML00036_Trace_SQL_commandText(this ILogger logger, object commandText)
        {
            _SML00036_Trace_SQL_commandText(logger, commandText, null);
        }

        /// <summary>
        /// Error Init storage
        /// </summary>
        public static void SML00037_Error_Error_Init_storage(this ILogger logger, Exception ex)
        {
            _SML00037_Error_Error_Init_storage(logger, ex);
        }

        /// <summary>
        /// Is Log Table
        /// </summary>
        public static void SML00038_Debug_Is_Log_Table(this ILogger logger)
        {
            _SML00038_Debug_Is_Log_Table(logger, null);
        }

        /// <summary>
        /// SQL>> {commandText}
        /// </summary>
        public static void SML00039_Trace_SQL_commandText(this ILogger logger, object commandText)
        {
            _SML00039_Trace_SQL_commandText(logger, commandText, null);
        }

        /// <summary>
        /// '{TableName}' is log table: {isLog}
        /// </summary>
        public static void SML00040_Debug_TableName_is_log_table_isLog(this ILogger logger, object TableName, object isLog)
        {
            _SML00040_Debug_TableName_is_log_table_isLog(logger, TableName, isLog, null);
        }

        /// <summary>
        /// Rename Log Table
        /// </summary>
        public static void SML00041_Debug_Rename_Log_Table(this ILogger logger)
        {
            _SML00041_Debug_Rename_Log_Table(logger, null);
        }

        /// <summary>
        /// SQL>> {commandText}
        /// </summary>
        public static void SML00042_Trace_SQL_commandText(this ILogger logger, object commandText)
        {
            _SML00042_Trace_SQL_commandText(logger, commandText, null);
        }

        /// <summary>
        /// LogTable entry updated. Old TableName='{oldTableName}', new TableName='{newTableName}'.
        /// </summary>
        public static void SML00043_Information_LogTable_entry_updated_Old_TableName_oldTableName_new_TableName_newTableName(this ILogger logger, object oldTableName, object newTableName)
        {
            _SML00043_Information_LogTable_entry_updated_Old_TableName_oldTableName_new_TableName_newTableName(logger, oldTableName, newTableName, null);
        }

        /// <summary>
        /// Renaming LogTable failed. Old TableName='{oldTableName}', new TableName='{newTableName}'.
        /// </summary>
        public static void SML00044_Error_Renaming_LogTable_failed_Old_TableName_oldTableName_new_TableName_newTableName(this ILogger logger, object oldTableName, object newTableName, Exception ex)
        {
            _SML00044_Error_Renaming_LogTable_failed_Old_TableName_oldTableName_new_TableName_newTableName(logger, oldTableName, newTableName, ex);
        }

        /// <summary>
        /// Rename Table
        /// </summary>
        public static void SML00045_Debug_Rename_Table(this ILogger logger)
        {
            _SML00045_Debug_Rename_Table(logger, null);
        }

        /// <summary>
        /// SQL>> {commandText}
        /// </summary>
        public static void SML00046_Trace_SQL_commandText(this ILogger logger, object commandText)
        {
            _SML00046_Trace_SQL_commandText(logger, commandText, null);
        }

        /// <summary>
        /// Table '{oldTableName}' renamed to '{newTableName}'.
        /// </summary>
        public static void SML00047_Information_Table_oldTableName_renamed_to_newTableName(this ILogger logger, object oldTableName, object newTableName)
        {
            _SML00047_Information_Table_oldTableName_renamed_to_newTableName(logger, oldTableName, newTableName, null);
        }

        /// <summary>
        /// Renaming table failed. Old TableName='{oldTableName}', new TableName='{newTableName}'.
        /// </summary>
        public static void SML00048_Error_Renaming_table_failed_Old_TableName_oldTableName_new_TableName_newTableName(this ILogger logger, object oldTableName, object newTableName, Exception ex)
        {
            _SML00048_Error_Renaming_table_failed_Old_TableName_oldTableName_new_TableName_newTableName(logger, oldTableName, newTableName, ex);
        }

        /// <summary>
        /// Update Log Table
        /// </summary>
        public static void SML00049_Debug_Update_Log_Table(this ILogger logger)
        {
            _SML00049_Debug_Update_Log_Table(logger, null);
        }

        /// <summary>
        /// Updating LogTable failed. SystemName='{systemConfigurationName}'.
        /// </summary>
        public static void SML00050_Error_Updating_LogTable_failed_SystemName_systemConfigurationName(this ILogger logger, object systemConfigurationName, Exception ex)
        {
            _SML00050_Error_Updating_LogTable_failed_SystemName_systemConfigurationName(logger, systemConfigurationName, ex);
        }


    }
}