// Copyright (C) 2019 Topsoft (https://topsoft.by)

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using T2.CLS.StorageService.Dto;
using T2.CLS.StorageService.Interfaces;
using T2.CLS.StorageService.Model;
using T2.CLS.StorageService.Utils;

namespace T2.CLS.StorageService.Services
{
	internal sealed partial class StorageManager
	{
		#region Static Fields and Constants

		private const int MaxArchiveThreadsCount = 4;
		private static readonly TimeSpan PartitionMinimumLifetime = TimeSpan.FromDays(1);

#if DEBUG
		private static readonly TimeSpan PartitionHandleTimeout = TimeSpan.FromSeconds(10);
#else
		private static readonly TimeSpan PartitionHandleTimeout = TimeSpan.FromHours(1);
#endif

		#endregion

		#region Fields

		private DateTime _partitionHandleLastTime = DateTime.MinValue;
		private bool _partitionHandling;

		#endregion

		#region  Methods

		/*
		private ArchivePartitionResult ArchivePartition(PartitionArchiveJob partitionArchiveJob)
		{
			try
			{
				_logger.LogInformation($"Archive job '{partitionArchiveJob.Id}' has started.");

				if (Directory.Exists(partitionArchiveJob.PartitionSource) == false)
				{
					_logger.LogWarning($"Partition directory '{partitionArchiveJob.PartitionSource}' for archive job '{partitionArchiveJob.Id}' was not found.\nArchive job will be removed.");

					return ArchivePartitionResult.PartitionNotExist;
				}

				if (Directory.Exists(partitionArchiveJob.PartitionDestination))
				{
					_logger.LogWarning($"Partition target directory '{partitionArchiveJob.PartitionDestination}' is not empty.\nDeleting content.");

					Directory.Delete(partitionArchiveJob.PartitionDestination, true);
				}

				_logger.LogInformation($"Moving partition data from '{partitionArchiveJob.PartitionSource}' to '{partitionArchiveJob.PartitionDestination}'.");

				IOUtils.DirectoryCopy(partitionArchiveJob.PartitionSource, partitionArchiveJob.PartitionDestination, true);

				var archiveInfo = new ArchiveInfoDto
				{
					Id = partitionArchiveJob.Id,
					Date = DateTime.Now,
					SystemName = partitionArchiveJob.SystemName,
					TableSchema = partitionArchiveJob.TableSchema.AsDto()
				};

				var archiveInfoPath = Path.Combine(partitionArchiveJob.PartitionDestination, "t2_archive_info");

				File.WriteAllText(archiveInfoPath, JsonConvert.SerializeObject(archiveInfo, Formatting.Indented));

				_logger.LogInformation($"Archive info created at '{archiveInfoPath}'.");

				Directory.Delete(partitionArchiveJob.PartitionSource, true);

				_logger.LogInformation($"Detached partition data at '{partitionArchiveJob.PartitionSource}' deleted.");
				_logger.LogInformation($"Archive job '{partitionArchiveJob.Id}' has successfully finished.");

				return ArchivePartitionResult.Completed;
			}
			catch (Exception e)
			{
				_logger.LogError(e, $"Archive job '{partitionArchiveJob.Id}' failed.");

				return ArchivePartitionResult.Failed;
			}
		}

		private void AttachPartitionAsync(string tableName, string partName)
		{
			_clickHouseHttpClient.Execute($"ALTER TABLE {tableName} ATTACH PART '{partName}'");
		}

		private void CreateArchiveEntries(List<PartitionArchiveJob> partitionArchiveJobs)
		{
			if (partitionArchiveJobs.Count == 0)
				return;

			var values = string.Join(',', partitionArchiveJobs.Select(j => $"('{j.Id}', '{j.SystemId}', '{j.SystemName}', '{j.TableSchema.Serialize()}', '{j.Partition}', '{j.PartitionName}', '{FormatClickHouseDate(j.Date)}', '{j.PartitionDestination}')"));

			var commandText = $"INSERT INTO t2_archives (Id, SystemId, SystemName, TableSchema, Partition, PartitionName, Date, Path) VALUES {values}";

			_clickHouseHttpClient.Execute(commandText);

			_logger.LogInformation($"Archive entries created: {string.Join("\n\t", partitionArchiveJobs.Select(j => $"Id={j.Id}, SystemId={j.SystemId}, SystemName={j.SystemName}, TableName={j.TableSchema.Name}, Partition={j.Partition}, Date={FormatClickHouseDate(j.Date)}"))}");
		}

		private static PartitionArchiveJob CreateArchivePartitionJob(OutdatedPartition outdatedPartition, StorageConfiguration storageConfiguration)
		{
			var archiveJobId = Guid.NewGuid().ToString();
			var date = outdatedPartition.Partition.MaxTime;
			var destination = Path.Combine(storageConfiguration.ArchivePath, outdatedPartition.SystemConfiguration.Name, date.ToString("dd-MM-yyyy"), archiveJobId).Replace("\\", "/");

			return new PartitionArchiveJob(archiveJobId,
				outdatedPartition.SystemConfiguration.Id,
				outdatedPartition.SystemConfiguration.Name,
				outdatedPartition.TableSchema,
				outdatedPartition.Partition.Partition,
				outdatedPartition.Partition.Name,
				date,
				outdatedPartition.Partition.Path,
				destination);
		}

		private void CreateArchivePartitionJobEntries(List<PartitionArchiveJob> partitionArchiveJobs)
		{
			if (partitionArchiveJobs.Count == 0)
				return;

			var values = string.Join(',', partitionArchiveJobs.Select(j => $"('{j.Id}', '{j.SystemId}', '{j.SystemName}', '{j.TableSchema.Name}', '{j.TableSchema.Serialize()}', '{j.Partition}', '{j.PartitionName}', '{FormatClickHouseDate(j.Date)}', '{j.PartitionSource}', '{j.PartitionDestination}')"));

			var commandText = $"INSERT INTO t2_archive_jobs (Id, SystemId, SystemName, TableName, TableSchema, Partition, PartitionName, Date, PartitionSource, PartitionDestination) VALUES {values}";

			_clickHouseHttpClient.Execute(commandText);

			_logger.LogInformation($"ArchiveJob entries created: {string.Join("\n\t", partitionArchiveJobs.Select(j => $"Id={j.Id}, SystemName={j.SystemName}, TableName={j.TableSchema.Name}, Date={FormatClickHouseDate(j.Date)}"))}");
		}

		private async Task<bool> DetachPartitionAsync(PartitionInfo partitionInfo)
		{
			try
			{
				_logger.LogInformation($"Detaching partition '{partitionInfo.Partition}'...");

				var commandText = $"ALTER TABLE {partitionInfo.Table} DETACH PARTITION {partitionInfo.Partition}";

				new Task(() => _clickHouseHttpClient.Execute(commandText)).Start();

				var checkPartitionDetachedQuery = $"SELECT count() FROM parts WHERE name = '{partitionInfo.Name}' AND table = '{partitionInfo.Table}'";

				while (true)
				{
					_clickHouseHttpClient.ExecuteScalar(checkPartitionDetachedQuery, "system", out int count);
					if (count == 0)
						break;

					await Task.Delay(10);
				}

				_logger.LogInformation($"Partition '{partitionInfo.Partition}' has been detached.");

				return true;
			}
			catch (Exception e)
			{
				_logger.LogError(e, $"Detaching partition '{partitionInfo.Partition}' failed.");

				return false;
			}
		}


		private IEnumerable<string> EnumerateDetachedPartitions(string table)
		{
			var detachedPartitionsPath = GetDetachedPartitionsPath(table);

			if (Directory.Exists(detachedPartitionsPath) == false)
				yield break;

			foreach (var directory in Directory.EnumerateDirectories(detachedPartitionsPath))
			{
				if (File.Exists(Path.Combine(directory, "partition.dat")))
					yield return Path.GetFileName(directory);
			}
		}

		
		
		private static ArchiveInfoDto GetArchiveById(string archiveId)
		{
			using var getArchiveCommand = connection.CreateCommand();

			getArchiveCommand.CommandText = $"SELECT * FROM t2_archives WHERE Id = '{archiveId}'";

			var dataTable = new DataTable();
			var adapter = new OdbcDataAdapter(getArchiveCommand);

			adapter.Fill(dataTable);

			return dataTable.Rows.Count != 1 ? null : ArchiveInfoDto.Load(dataTable.Rows[0]);
		}
		

		private IEnumerable<PartitionArchiveJob> GetArchiveJobs(OdbcConnection connection)
		{
			using var command = connection.CreateCommand();

			command.CommandText = "SELECT * FROM t2_archive_jobs";

			var dataAdapter = new OdbcDataAdapter(command);
			var dataTable = new DataTable();

			dataAdapter.Fill(dataTable);

			foreach (DataRow dataTableRow in dataTable.Rows)
				yield return PartitionArchiveJob.Load(dataTableRow);
		}

		private string GetDetachedPartitionsPath(string table, OdbcConnection connection)
		{
			return Path.Combine(GetTableDataPath(table, connection), "detached");
		}

		private List<OutdatedPartition> GetOutdatedPartitions(OdbcConnection connection)
		{
			var outdatedPartitions = new List<OutdatedPartition>();
			var logConfiguration = _configurationManager.LogConfiguration;
			var now = DateTime.Now;

			foreach (var systemConfiguration in logConfiguration.SystemConfigurations)
			{
				var logTables = GetLogTablesBySystemName(systemConfiguration.Name, connection).ToList();
				var partitionLifetime = systemConfiguration.PartitionLifetime;

				if (partitionLifetime < PartitionMinimumLifetime)
					partitionLifetime = PartitionMinimumLifetime;

				outdatedPartitions.AddRange(from logTable in logTables
					from partitionInfo in GetPartitionsByTable(logTable.Name, connection).ToList()
					where now - partitionInfo.MaxTime > partitionLifetime
					select new OutdatedPartition(partitionInfo, systemConfiguration, logTable));
			}

			return outdatedPartitions;
		}

		private static IEnumerable<PartitionInfo> GetPartitionsByTable(string table, OdbcConnection connection)
		{
			return QueryPartitions($"SELECT * FROM system.parts WHERE table='{table}'", connection);
		}

		private void HandleArchiveJobs(OdbcConnection connection)
		{
			var archiveJobs = GetArchiveJobs(connection).ToList();

			if (archiveJobs.Count == 0)
				return;

			var completedJobs = new ConcurrentBag<PartitionArchiveJob>();
			var dummyJobs = new ConcurrentBag<PartitionArchiveJob>();
			var options = new ParallelOptions {MaxDegreeOfParallelism = MaxArchiveThreadsCount};

			Parallel.ForEach(archiveJobs, options, job =>
			{
				switch (ArchivePartition(job))
				{
					case ArchivePartitionResult.Completed:
						completedJobs.Add(job);

						break;
					case ArchivePartitionResult.PartitionNotExist:
						dummyJobs.Add(job);

						break;
				}
			});

			if (completedJobs.Count > 0)
			{
				var completedJobsList = completedJobs.ToList();

				RemoveArchiveJobs(completedJobsList, connection);
				CreateArchiveEntries(completedJobsList, connection);
			}

			if (dummyJobs.Count > 0)
				RemoveArchiveJobs(dummyJobs.ToList(), connection);
		}

		private async Task<bool> HandlePartitions(CancellationToken stoppingToken)
		{
			if (_partitionHandling)
				return true;

			if (DateTime.Now - _partitionHandleLastTime < PartitionHandleTimeout)
				return true;

			_partitionHandleLastTime = DateTime.Now;

			try
			{
				await Task.Run(HandlePartitionsTask, stoppingToken);
			}
			catch (Exception e)
			{
				_logger.LogError(e, "HandlePartitions");

				return false;
			}

			return true;

			//var detachedPartitions = EnumerateDetachedPartitions("ESBLog", connection).ToList();

			//foreach (var detachedPartition in detachedPartitions)
			//{
			//	AttachPartitionAsync(detachedPartition, "ESBLog", connection).Wait();
			//}

			//var partitions = GetPartitionsByTable(systemConfiguration.Name, connection).ToList();

			//foreach (var partitionInfo in partitions) 
			//	DetachPartitionAsync(partitionInfo, connection).Wait();
		}

		private async void HandlePartitionsTask()
		{
			try
			{
				_partitionHandling = true;

				_logger.LogInformation("Handle partitions task started.");

				await using var connection = CreateConnection();

				if (connection == null)
					return;

				var storageConfiguration = _configurationManager.StorageConfiguration;
				var outdatedPartitions = GetOutdatedPartitions(connection);
				var archiveJobs = new List<PartitionArchiveJob>();

				foreach (var outdatedPartition in outdatedPartitions)
				{
					var detachedPartitionPath = Path.Combine(GetTableDataPath(outdatedPartition.TableSchema.Name, connection), "detached", outdatedPartition.Partition.Name).Replace("\\", "/");

					outdatedPartition.Partition.Path = detachedPartitionPath;

					archiveJobs.Add(CreateArchivePartitionJob(outdatedPartition, storageConfiguration));
				}

				foreach (var outdatedPartition in outdatedPartitions)
					await DetachPartitionAsync(outdatedPartition.Partition, connection);

				CreateArchivePartitionJobEntries(archiveJobs, connection);

				HandleArchiveJobs(connection);
			}
			finally
			{
				_logger.LogInformation("Handle partitions task finished.");

				_partitionHandling = false;
			}
		}

		private static IEnumerable<PartitionInfo> QueryPartitions(string query, OdbcConnection connection)
		{
			using var command = connection.CreateCommand();

			command.CommandText = query;

			var dataAdapter = new OdbcDataAdapter(command);
			var dataTable = new DataTable();

			dataAdapter.Fill(dataTable);

			foreach (DataRow dataTableRow in dataTable.Rows)
				yield return PartitionInfo.Load(dataTableRow);
		}

		private void RemoveArchiveJobs(List<PartitionArchiveJob> partitionArchiveJobs, OdbcConnection connection)
		{
			if (partitionArchiveJobs.Count == 0)
				return;

			using var command = connection.CreateCommand();

			command.CommandText = $"ALTER TABLE default.t2_archive_jobs DELETE WHERE Id in({string.Join(", ", partitionArchiveJobs.Select(j => $"'{j.Id}'"))})";

			command.ExecuteNonQuery();

			_logger.LogInformation($"ArchiveJob entries removed: {string.Join("\n\t", partitionArchiveJobs.Select(j => $"Id={j.Id}, SystemId={j.SystemId}, SystemName={j.SystemName}, TableName={j.TableSchema.Name}, Date={FormatClickHouseDate(j.Date)}"))}");
		}

		#endregion

		#region Interface Implementations

		#region IStorageManager

		public async Task<List<ArchiveInfoDto>> GetArchives(string systemName)
		{
			await using var connection = CreateConnection();
			await using var command = connection.CreateCommand();

			command.CommandText = $"SELECT * FROM t2_archives WHERE SystemName = '{systemName}'";

			var dataAdapter = new OdbcDataAdapter(command);
			var dataTable = new DataTable();

			dataAdapter.Fill(dataTable);

			return dataTable.Rows.Cast<DataRow>().Select(ArchiveInfoDto.Load).ToList();
		}

		public async Task<bool> RestoreArchive(string id, TimeSpan lifeTime)
		{
			await using var connection = CreateConnection();

			var archiveInfo = GetArchiveById(id, connection);
			var archiveTableSchema = TableSchema.FromDto(archiveInfo.TableSchema);
			var archiveId = Guid.Parse(archiveInfo.Id);

			archiveTableSchema.Name = $"{archiveTableSchema.Name}_{archiveId:N}";

			var detachedPartitionsPath = GetDetachedPartitionsPath(archiveTableSchema.Name, connection);
			var partitionPath = Path.Combine(detachedPartitionsPath, archiveInfo.PartitionName);

			if (Directory.Exists(partitionPath))
				return false;

			CreateLogTable(archiveInfo.SystemId, archiveInfo.SystemName, archiveTableSchema, true, DateTime.Now + lifeTime,connection);

			IOUtils.DirectoryCopy(archiveInfo.Path, partitionPath, true);

			AttachPartitionAsync(archiveTableSchema.Name, archiveInfo.PartitionName, connection);

			return true;
		}

		#endregion

		#endregion

		#region  Nested Types

		private enum ArchivePartitionResult
		{
			PartitionNotExist,
			Completed,
			Failed
		}

		private struct OutdatedPartition
		{
			public OutdatedPartition(PartitionInfo partition, LogSystemConfiguration systemConfiguration, TableSchema tableSchema)
			{
				Partition = partition;
				SystemConfiguration = systemConfiguration;
				TableSchema = tableSchema;
			}

			public readonly PartitionInfo Partition;
			public readonly LogSystemConfiguration SystemConfiguration;
			public readonly TableSchema TableSchema;
		}*/

		#endregion
	}
}