// Copyright (C) 2019 Topsoft (https://topsoft.by)

using System;
using Microsoft.Extensions.Configuration;
using T2.CLS.StorageService.Dto;

namespace T2.CLS.StorageService.Model
{
	internal sealed class StorageConfiguration
	{
		#region Ctors

		public StorageConfiguration(IConfigurationSection storageConfigPath)
		{
			var storageConfigurationDto = new StorageConfigurationDto();

			storageConfigPath.Bind(storageConfigurationDto);

			HttpUrl = storageConfigurationDto.HttpUrl;
			User = storageConfigurationDto.User;
			Password = storageConfigurationDto.Password;
			DataBase = storageConfigurationDto.DataBase;
			DataPath = storageConfigurationDto.DataPath;
			ArchivePath = storageConfigurationDto.ArchivePath;
		}

		#endregion

		#region Properties

		public string ArchivePath { get; }


		public string HttpUrl { get; }

		public string User { get; }

		public string Password { get; }

		public string DataBase { get; }

		public string DataPath { get; }

		#endregion

		#region  Methods

		public bool IsEqualTo(StorageConfiguration storageConfiguration)
		{
			if (storageConfiguration == null)
				return false;

			return string.Equals(HttpUrl, storageConfiguration.HttpUrl, StringComparison.Ordinal)
					&& string.Equals(DataBase, storageConfiguration.DataBase, StringComparison.Ordinal)
					&& string.Equals(User, storageConfiguration.User, StringComparison.Ordinal)
					&& string.Equals(Password, storageConfiguration.Password, StringComparison.Ordinal);
		}

		#endregion
	}
}