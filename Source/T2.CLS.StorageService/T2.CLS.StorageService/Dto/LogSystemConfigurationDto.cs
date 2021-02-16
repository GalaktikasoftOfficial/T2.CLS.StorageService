// Copyright (C) 2019 Topsoft (https://topsoft.by)

using System;
using System.Collections.Generic;

namespace T2.CLS.StorageService.Dto
{
	public sealed class LogSystemConfigurationDto
	{
		#region Properties

		public List<LogFieldDto> Fields { get; set; } = new List<LogFieldDto>();

		public string Name { get; set; }

		public string DateTimeField { get; set; }

		public List<string> SortFields { get; set; } = new List<string>();

		public TimeSpan PartitionLifetime { get; set; }

		#endregion
	}
}