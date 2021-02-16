// Copyright (C) 2019 Topsoft (https://topsoft.by)

using System;
using System.Data;

namespace T2.CLS.StorageService.Model
{
	internal sealed class PartitionArchiveJob
	{
		#region Ctors

		public PartitionArchiveJob(string id, string systemId, string systemName, TableSchema tableSchema, string partition, string partitionName, DateTime date, string partitionSource, string partitionDestination)
		{
			Id = id;
			SystemId = systemId;
			SystemName = systemName;
			TableSchema = tableSchema;
			Partition = partition;
			PartitionName = partitionName;
			Date = date;
			PartitionSource = partitionSource;
			PartitionDestination = partitionDestination;
		}

		#endregion

		#region Properties

		public DateTime Date { get; }

		public string Id { get; }

		public string PartitionDestination { get; }

		public string PartitionSource { get; }

		public string SystemId { get; }

		public string SystemName { get; }

		public TableSchema TableSchema { get; }

		public string Partition { get; }

		public string PartitionName { get; }

		#endregion

		#region  Methods

		public static PartitionArchiveJob Load(DataRow dataRow)
		{
			var partitionArchiveJob = new PartitionArchiveJob(
				dataRow.Field<string>("Id"),
				dataRow.Field<string>("SystemId"),
				dataRow.Field<string>("SystemName"),
				TableSchema.Deserialize(dataRow.Field<string>("TableSchema")),
				dataRow.Field<string>("Partition"),
				dataRow.Field<string>("PartitionName"),
				dataRow.Field<DateTime>("Date"),
				dataRow.Field<string>("PartitionSource"),
				dataRow.Field<string>("PartitionDestination"));

			return partitionArchiveJob;
		}

		#endregion
	}
}