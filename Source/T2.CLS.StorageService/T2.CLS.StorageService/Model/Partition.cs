// Copyright (C) 2019 Topsoft (https://topsoft.by)

using System;
using System.Data;

namespace T2.CLS.StorageService.Model
{
	internal sealed class PartitionInfo
	{
		#region Properties

		public string Database { get; private set; }

		public string Id { get; private set; }

		public bool IsActive { get; private set; }

		public DateTime MaxTime { get; private set; }

		public DateTime MinTime { get; private set; }

		public string Name { get; private set; }

		public string Partition { get; private set; }

		public string Path { get; set; }

		public string Table { get; private set; }

		#endregion

		#region  Methods

		public static PartitionInfo Load(DataRow dataRow)
		{
			return new PartitionInfo
			{
				Database = dataRow.Field<string>("database"),
				Id = dataRow.Field<string>("partition_id"),
				MaxTime = dataRow.Field<DateTime>("max_time"),
				MinTime = dataRow.Field<DateTime>("min_time"),
				Partition = dataRow.Field<string>("partition"),
				Path = dataRow.Field<string>("path"),
				Table = dataRow.Field<string>("table"),
				IsActive = dataRow.Field<byte>("active") == 1,
				Name = dataRow.Field<string>("name")
			};
		}

		#endregion
	}
}