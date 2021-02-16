// Copyright (C) 2019 Topsoft (https://topsoft.by)

using System;
using System.Data;
using Newtonsoft.Json;

namespace T2.CLS.StorageService.Dto
{
	public sealed class ArchiveInfoDto
	{
		#region Properties

		public DateTime Date { get; set; }

		public string Id { get; set; }

		public string SystemId { get; set; }

		public string Path { get; set; }

		public string SystemName { get; set; }

		public TableSchemaDto TableSchema { get; set; }

		public string Partition { get; set; }

		public string PartitionName { get; set; }

		#endregion

		public static ArchiveInfoDto Load(DataRow dataTableRow)
		{
			var archiveInfo = new ArchiveInfoDto
			{
				Id = dataTableRow.Field<string>("Id"),
				Date = dataTableRow.Field<DateTime>("Date"),
				SystemId = dataTableRow.Field<string>("SystemId"),
				SystemName = dataTableRow.Field<string>("SystemName"),
				Partition = dataTableRow.Field<string>("Partition"),
				PartitionName = dataTableRow.Field<string>("PartitionName"),
				TableSchema = JsonConvert.DeserializeObject<TableSchemaDto>(dataTableRow.Field<string>("TableSchema")),
				Path = dataTableRow.Field<string>("Path")
			};

			return archiveInfo;
		}
	}
}