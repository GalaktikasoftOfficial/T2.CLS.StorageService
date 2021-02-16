// Copyright (C) 2019 Topsoft (https://topsoft.by)

namespace T2.CLS.StorageService.Dto
{
	public sealed class StorageConfigurationDto
	{
		#region Properties
		
		public string HttpUrl { get; set; }

		public string User { get; set; }

		public string Password { get; set; }

		public string DataBase { get; set; }

		public string ArchivePath { get; set; }
		
		public string DataPath { get; set; }

		#endregion
	}
}