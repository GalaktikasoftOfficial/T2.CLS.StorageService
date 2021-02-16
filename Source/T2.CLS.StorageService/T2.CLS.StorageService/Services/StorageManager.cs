// Copyright (C) 2019 Topsoft (https://topsoft.by)

using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using T2.CLS.StorageService.ClickHouse;
using T2.CLS.StorageService.Dto.ClickHouse;
using T2.CLS.StorageService.Interfaces;
using T2.CLS.StorageService.Model;
using T2.CLS.StorageService.Utils;
using T2.CLS.StorageService.Extensions;

namespace T2.CLS.StorageService.Services
{
	internal sealed partial class StorageManager : IStorageManager
	{
		#region Static Fields and Constants

		private const int FailThreshold = 10;

		private static readonly string OpenConnectionFailId = Guid.NewGuid().ToString();
		private static readonly string InitStorageFailId = Guid.NewGuid().ToString();
		private static readonly string HandlePartitionsFailId = Guid.NewGuid().ToString();
		private static readonly DateTime NotExpireDate = new DateTime(2100, 1, 1);

		#endregion

		#region Fields

		private readonly IConfigurationManager _configurationManager;
		private readonly Dictionary<string, int> _failCount = new Dictionary<string, int>();
		private readonly ILogger<StorageManager> _logger;
		private bool _configurationDirty = true;
		private IClickHouseHttpClient _clickHouseHttpClient;

		#endregion

		#region Ctors

		public StorageManager(IConfigurationManager configurationManager, IClickHouseHttpClient clickHouseHttpClient, ILogger<StorageManager> logger)
		{
			_configurationManager = configurationManager;
			_logger = logger;

			_clickHouseHttpClient = clickHouseHttpClient;

			_configurationManager.StorageConfigurationChanged += OnStorageConfigurationChanged;
			_configurationManager.LogConfigurationChanged += OnLogConfigurationChanged;
		}

		#endregion

		#region  Methods

		private bool CheckDataBase()
		{
			
			_logger.SML00001_Debug_Start_Create_ODBC_Connection();

			var configuration = _configurationManager.StorageConfiguration;
			
			try
			{
				_clickHouseHttpClient.ExecuteScalar($"SELECT count() FROM databases WHERE name = '{configuration.DataBase}'", "system", out int count);
				
				if (count == 0)
				{
					_clickHouseHttpClient.Execute($"CREATE DATABASE IF NOT EXISTS {configuration.DataBase}", "system");
				}

				return true;
			}
			catch (Exception e)
			{
                _logger.SML00009_Error_Can_not_open_connection_ConnectionString_configurationConnectionString(configuration?.HttpUrl, e);
				return false;
			}
		}

		private void CreateLogSystemEntry(LogSystemConfiguration systemConfiguration)
		{
			try
			{
                _logger.SML00005_Debug_Create_Log_SystemEntry();

				var CommandText =
					$"INSERT INTO t2_systems (Id, Name) VALUES ('{systemConfiguration.Id}', '{systemConfiguration.Name}')";

				_logger.SML00006_Trace_SQL_commandText(CommandText);

				_clickHouseHttpClient.Execute(CommandText);

				_logger.SML00007_Information_Log_system_entry_created_SystemName_systemConfigurationName_SystemId_systemConfigurationId(systemConfiguration.Name, systemConfiguration.Id);

			}
			catch (Exception e)
			{
                _logger.SML00008_Error_LogSystem_entry_creation_failed_SystemName_systemConfigurationName_SystemId_systemConfigurationId(systemConfiguration.Name, systemConfiguration.Id, e);

				throw;
			}
		}

		private void CreateLogTable(LogSystemConfiguration systemConfiguration, TableSchema tableSchema, bool isArchive, DateTime expireDate)
		{
			CreateLogTable(systemConfiguration.Id, systemConfiguration.Name, tableSchema, isArchive, expireDate);
		}

		private void CreateLogTable(string logSystemId, string logSystemName, TableSchema tableSchema, bool isArchive, DateTime expireDate)
		{
			try
			{
				CreateTable(tableSchema);

				CreateLogTableEntry(logSystemId, logSystemName, tableSchema.Name, isArchive, expireDate);
			}
			catch (Exception e)
			{
                _logger.SML00010_Error_LogTable_creation_failed_SystemName_logSystemName_TableSchema_n_tableSchema(logSystemName, tableSchema.Serialize(), e);
				throw;
			}
		}

		private void CreateLogTableEntry(string systemId, string systemName, string tableName, bool isArchive, DateTime expireDate)
		{
			try
			{
                _logger.SML00014_Debug_Create_Log_Table_Entry();

				var commandText = $"INSERT INTO t2_tables (Id, SystemId, SystemName, TableName, IsArchive, CreateDate, ExpireDate) VALUES ('{Guid.NewGuid():D}', '{systemId}', '{systemName}', '{tableName}', {(isArchive ? 1 : 0)}, toDateTime(now()), toDateTime('{FormatClickHouseDateTime(expireDate)}'))";

				_logger.SML00011_Trace_SQL_sqlQuery(commandText);

				_clickHouseHttpClient.Execute(commandText);

				_logger.SML00012_Information_Log_table_entry_created_SystemName_systemName_TableName_tableName(
					systemName, tableName);
			}
			catch (Exception e)
			{
                _logger.SML00013_Error_LogTable_entry_creation_failed_SystemName_systemName_TableName_tableName(systemName, tableName, e);
				throw;
			}
		}

		[SuppressMessage("ReSharper", "RedundantStringInterpolation")]
		private void CreateTable(TableSchema tableSchema)
		{
			
			_logger.SML00016_Debug_Create_Table();

			var dateField = tableSchema.DateTimeField;

			static string FieldDef(FieldSchema fieldSchema) => $"{fieldSchema.Name} {TypeMapping.ToClickHouseTypeName(fieldSchema.Type)}";

			var commandText = $"CREATE TABLE IF NOT EXISTS {tableSchema.Name} ({string.Join(",", tableSchema.Fields.Select(FieldDef))})" +
			                  $" Engine = MergeTree()" +
			                  $" PARTITION BY toYYYYMMDD({dateField.Name})" +
			                  $" ORDER BY ({string.Join(',', tableSchema.SortFields.Select(f => f.Name))})";
							  
			_logger.SML00015_Trace_SQL_commandText(commandText);

			try
			{
				_clickHouseHttpClient.Execute(commandText);

                _logger.SML00017_Information_Table_tableName_created(tableSchema.Name);
			}
			catch (Exception e)
			{
                _logger.SML00018_Error_Table_tableName_creation_failed(tableSchema.Name, e);
				throw;
			}
		}

		private void DropTable(string tableName)
		{
			_logger.SML00019_Debug_Drop_Table();
			
			var commandText = $"DROP TABLE IF EXISTS {tableName}";

			_clickHouseHttpClient.Execute(commandText);

            _logger.SML00020_Trace_SQL_commandText(commandText);

		}

		private bool EnsureLogSystem(LogSystemConfiguration systemConfiguration)
		{
			
			_logger.SML00021_Debug_Ensure_Log_System();
			try
			{
				var command = $"SELECT Id FROM t2_systems WHERE Name = '{systemConfiguration.Name}'";
				_clickHouseHttpClient.ExecuteScalar(command, string.Empty, out string id);


				_logger.SML00022_Trace_SQL_commandText(command);

				systemConfiguration.Id = id;

				if (string.IsNullOrEmpty(systemConfiguration.Id))
				{
					systemConfiguration.Id = Guid.NewGuid().ToString();

					CreateLogSystemEntry(systemConfiguration);
				}

                _logger.SML00023_Debug_Found_System_Configuration_Id_systemConfigurationId(systemConfiguration.Id);

				var tableSchema = systemConfiguration.AsTableSchema();
				var existingTable = FindTable(tableSchema.Name);
				
				if (existingTable != null)
				{
					if (tableSchema.IsEqualTo(existingTable))
						return true;

					if (IsLogTable(tableSchema.Name) == false)
						throw new Exception($"Table '{tableSchema.Name}' is already exists and does not belong to log tables.");

					UpdateLogTable(systemConfiguration, existingTable, tableSchema);
				}
				else
					CreateLogTable(systemConfiguration, tableSchema, false, NotExpireDate);

				return true;
			}
			catch (Exception e)
			{
                _logger.SML00024_Error_LogSystem_configuration_failed_SystemName_systemConfigurationName(systemConfiguration.Name, e);
				return false;
			}
		}

		private TableSchema FindTable(string tableName)
		{
            _logger.SML00025_Debug_Find_Table();

			var commandText = $"SELECT name, type, is_in_partition_key, is_in_sorting_key FROM columns WHERE table='{tableName}' AND database = '{_configurationManager.StorageConfiguration.DataBase}'";

			_logger.SML00026_Trace_SQL_commandText(commandText);

			var result = _clickHouseHttpClient.ExcuteRequest<SystemColumnsDto>(commandText, "system");
			
		
			if (!result.Any())
				return null;

			var fields = new List<FieldSchema>();
			var sortFields = new List<FieldSchema>();
			FieldSchema dateTimeField = null;

			foreach (var systemColumnsDto in result)
			{
			
				var name = systemColumnsDto.name;
				var type = TypeMapping.FromClickHouseType(systemColumnsDto.type);
				var fieldSchema = new FieldSchema(name, type);

				if (systemColumnsDto.is_in_partition_key == 1)
					dateTimeField = fieldSchema;

				if (systemColumnsDto.is_in_sorting_key == 1)
					sortFields.Add(fieldSchema);

				fields.Add(fieldSchema);
			}
			
            _logger.SML00027_Information_Existing_table_tableName_found(tableName);
			return new TableSchema(tableName, fields, dateTimeField, sortFields);
		}

		private static string FormatClickHouseDate(DateTime dateTime)
		{
			// Date format example: 2016-06-15
			return dateTime.ToString("yyyy-MM-dd");
		}

		private static string FormatClickHouseDateTime(DateTime dateTime)
		{
			// DateTime format example: 2016-06-15 23:00:00
			return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
		}

		private static string GenerateUniqueTableName(string tableName)
		{
			return $"{tableName}_{Guid.NewGuid():N}";
		}

		private string GetDataPath()
		{
			return _configurationManager.StorageConfiguration.DataPath;
		}

		private IEnumerable<TableSchema> GetLogTables()
		{
			
			_logger.SML00028_Debug_Get_Log_Tables();

			var tables = _clickHouseHttpClient.ExcuteRequest<TableDto>("SELECT TableName FROM t2_tables");

			foreach (var tableDto in tables)
			{
				var tableName = tableDto.TableName;
				var table = FindTable(tableName);

				if (table != null)
					yield return table;
			}
		}

		private IEnumerable<TableSchema> GetLogTablesBySystemName(string systemName)
		{
            _logger.SML00030_Debug_Get_Log_Tables_By_System_Name();
			
			var commandText = $"SELECT TableName FROM t2_tables WHERE SystemName='{systemName}'";

            _logger.SML00031_Trace_SQL_commandText(commandText);

			var tables = _clickHouseHttpClient.ExcuteRequest<TableDto>(commandText);

			foreach (var tableDto in tables)
			{
				var tableName = tableDto.TableName;
				var table = FindTable(tableName);

				if (table != null)
					yield return table;
			}
		}

		private string GetTableDataPath(string table)
		{
			return Path.Combine(GetDataPath(), _configurationManager.StorageConfiguration.DataBase, table);
		}

		private bool InitStorage()
		{
            _logger.SML00032_Debug_InitStorage();

			try
			{
				// t2_systems
				{
					var commandText = "CREATE TABLE IF NOT EXISTS t2_systems(Id String, Name String) Engine = MergeTree() ORDER BY Id PRIMARY KEY Id";
					
					_logger.SML00033_Trace_SQL_commandText(commandText);

					_clickHouseHttpClient.Execute(commandText);
				}

				// t2_tables
				{
					var commandText = "CREATE TABLE IF NOT EXISTS t2_tables(Id String, SystemId String, SystemName String, TableName String, IsArchive UInt8, CreateDate DateTime, ExpireDate DateTime) Engine = MergeTree() ORDER BY Id PRIMARY KEY Id";
					
					_logger.SML00034_Trace_SQL_commandText(commandText);

					_clickHouseHttpClient.Execute(commandText);
				}

				// t2_archive_jobs
				{
					var commandText = "CREATE TABLE IF NOT EXISTS t2_archive_jobs(Id String, SystemId String, SystemName String, TableName String, TableSchema String, Partition String, PartitionName String, Date Date, PartitionSource String, PartitionDestination String) Engine = MergeTree() ORDER BY Id PRIMARY KEY Id";
                        
					_logger.SML00035_Trace_SQL_commandText(commandText);

					_clickHouseHttpClient.Execute(commandText);
				}

				// t2_archives
				{
					var commandText = "CREATE TABLE IF NOT EXISTS t2_archives(Id String, SystemId String, SystemName String, TableSchema String, Partition String, PartitionName String, Date Date, Path String) Engine = MergeTree() ORDER BY Id PRIMARY KEY Id";
                   
					_logger.SML00036_Trace_SQL_commandText(commandText);

					_clickHouseHttpClient.Execute(commandText);
				}
			}
			catch (Exception e)
			{
                _logger.SML00037_Error_Error_Init_storage(e);

				return false;
			}

			return true;
		}

		private bool IsLogTable(string tableName)
		{
            _logger.SML00038_Debug_Is_Log_Table();

			var commandText = $"SELECT count() FROM t2_tables WHERE TableName='{tableName}'";

            _logger.SML00039_Trace_SQL_commandText(commandText);

			_clickHouseHttpClient.ExecuteScalar(commandText, string.Empty, out int count);
			var result = count > 0;

			_logger.SML00040_Debug_TableName_is_log_table_isLog(tableName, result);

			return result;
		}

		private void OnLogConfigurationChanged(object sender, EventArgs e)
		{
			_configurationDirty = true;
		}

		private void OnStorageConfigurationChanged(object sender, EventArgs e)
		{
			_configurationDirty = true;
		}

		private void RenameLogTable(LogSystemConfiguration systemConfiguration, string oldTableName, string newTableName)
		{
            _logger.SML00041_Debug_Rename_Log_Table();
			try
			{
				RenameTable(oldTableName, newTableName);

				
				var commandText = $"ALTER TABLE t2_tables UPDATE TableName = '{newTableName}' WHERE SystemName = '{systemConfiguration.Name}' AND TableName = '{oldTableName}'";
				
                _logger.SML00042_Trace_SQL_commandText(commandText);

				_clickHouseHttpClient.Execute(commandText);

                _logger.SML00043_Information_LogTable_entry_updated_Old_TableName_oldTableName_new_TableName_newTableName(oldTableName, newTableName);
			}
			catch (Exception e)
			{
				_logger.SML00044_Error_Renaming_LogTable_failed_Old_TableName_oldTableName_new_TableName_newTableName(oldTableName, newTableName, e);
				throw;
			}
		}

		private void RenameTable(string oldTableName, string newTableName)
		{
            _logger.SML00045_Debug_Rename_Table();
			try
			{
				var commandText = $"RENAME TABLE {oldTableName} TO {newTableName}";

                _logger.SML00046_Trace_SQL_commandText(commandText);

				_clickHouseHttpClient.Execute(commandText);

                _logger.SML00047_Information_Table_oldTableName_renamed_to_newTableName(oldTableName, newTableName);
			}
			catch (Exception e)
			{
                _logger.SML00048_Error_Renaming_table_failed_Old_TableName_oldTableName_new_TableName_newTableName(oldTableName, newTableName, e);
				throw;
			}
		}

		private void UpdateLogTable(LogSystemConfiguration systemConfiguration, TableSchema oldSchema, TableSchema newSchema)
		{
            _logger.SML00049_Debug_Update_Log_Table();
			try
			{
				var existingTable = GetLogTablesBySystemName(systemConfiguration.Name).FirstOrDefault(t => t.IsEqualTo(newSchema));

				if (existingTable != null)
				{
					RenameLogTable(systemConfiguration, oldSchema.Name, GenerateUniqueTableName(oldSchema.Name));
					RenameLogTable(systemConfiguration, existingTable.Name, newSchema.Name);

					return;
				}

				RenameLogTable(systemConfiguration, oldSchema.Name, GenerateUniqueTableName(oldSchema.Name));
				CreateLogTable(systemConfiguration, newSchema, false, NotExpireDate);
			}
			catch (Exception e)
			{
				_logger.SML00050_Error_Updating_LogTable_failed_SystemName_systemConfigurationName(systemConfiguration.Name, e);

				throw;
			}
		}

		private void VerifyPermissibleFail(string failId, int permissibleFailCount, Func<string> errorFactory)
		{
			if (_failCount.TryGetValue(failId, out var failCount) == false)
				_failCount[failId] = 1;
			else
				_failCount[failId] = ++failCount;

			if (failCount > permissibleFailCount)
				throw new Exception(errorFactory());
		}

		#endregion

		#region Interface Implementations

		#region IStorageManager

		public async Task<bool> HandleStorage(CancellationToken stoppingToken)
		{
			if (_configurationDirty == false)
			{
				try
				{
					/*var handlePartitions = await HandlePartitions(stoppingToken);

					if (handlePartitions == false)
						VerifyPermissibleFail(HandlePartitionsFailId, FailThreshold, () => "Handle partitions fails exceeded permissible threshold. Shutting down.");
					*/
					
					return true;
				}
				catch (TaskCanceledException)
				{
					return true;
				}
			}

			try
			{
				if (!CheckDataBase())
				{
					VerifyPermissibleFail(OpenConnectionFailId, FailThreshold, () => "Open connection fails exceeded permissible threshold. Shutting down.");

					return true;
				}

				if (!InitStorage())
				{
					VerifyPermissibleFail(InitStorageFailId, FailThreshold, () => "Init storage fails exceeded permissible threshold. Shutting down.");

					return true;
				}

				var logConfiguration = _configurationManager.LogConfiguration;

				if (logConfiguration == null)
					return true;

				foreach (var systemConfiguration in logConfiguration.SystemConfigurations)
				{
					var result = EnsureLogSystem(systemConfiguration);

					if (result)
						continue;

					VerifyPermissibleFail(systemConfiguration.Name, FailThreshold, () => $"LogSystem '{systemConfiguration.Name}' configuration fails exceeded permissible threshold. Shutting down.");
				}

				return true;
			}
			catch (Exception e)
			{
				_logger.LogError(e, "HandleStorage");

				return false;
			}
			finally
			{
				_configurationDirty = false;
			}
		}

		#endregion

		#endregion
	}
}