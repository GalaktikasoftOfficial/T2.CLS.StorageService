// Copyright (C) 2019 Topsoft (https://topsoft.by)

using System.Collections.Generic;

namespace T2.CLS.StorageService.Dto
{
	public sealed class TableSchemaDto
	{
		#region Properties

		public string DateTimeField { get; set; }

		public List<FieldSchemaDto> Fields { get; set; } = new List<FieldSchemaDto>();

		public string Name { get; set; }

		public List<string> SortFields { get; set; } = new List<string>();

		#endregion
	}
}