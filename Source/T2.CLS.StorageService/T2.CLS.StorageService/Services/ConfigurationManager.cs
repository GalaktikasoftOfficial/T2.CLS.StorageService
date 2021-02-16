// Copyright (C) 2019 Topsoft (https://topsoft.by)

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using T2.CLS.StorageService.Interfaces;
using T2.CLS.StorageService.Model;
using T2.CLS.StorageService.Utils;

namespace T2.CLS.StorageService.Services
{
	internal sealed class ConfigurationManager : IConfigurationManager
	{
		#region Static Fields and Constants

		private static readonly string ConfigurationRoot = Path.Combine(IOUtils.GetAppBaseDirectory(), "SystemConfig");

		#endregion

		#region Fields

		// ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
		private readonly FileSystemWatcher _fileSystemWatcher;
		private readonly ILoggerFactory _loggerFactory;
		private bool _isReloading;
		private LogConfiguration _logConfiguration;
		private StorageConfiguration _storageConfiguration;

		#endregion

		#region Ctors

		public ConfigurationManager(IConfiguration configuration, ILoggerFactory loggerFactory)
		{
			_loggerFactory = loggerFactory;

			_fileSystemWatcher = new FileSystemWatcher
			{
				Path = ConfigurationRoot,
				NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size,
				IncludeSubdirectories = true
			};

			_fileSystemWatcher.Changed += OnChanged;
			_fileSystemWatcher.Created += OnChanged;
			_fileSystemWatcher.Deleted += OnChanged;
			_fileSystemWatcher.Renamed += OnRenamed;

			_fileSystemWatcher.EnableRaisingEvents = true;

			var storageSection = configuration.GetSection("Storage");

			StorageConfiguration = new StorageConfiguration(storageSection);

			ReloadConfiguration();
		}

		#endregion

		#region  Methods

		private void OnChanged(object sender, FileSystemEventArgs e)
		{
			ReloadConfiguration(true);
		}

		private void OnRenamed(object sender, RenamedEventArgs e)
		{
			ReloadConfiguration(true);
		}

		private async void ReloadConfiguration(bool delay = false)
		{
			if (_isReloading)
				return;

			try
			{
				_isReloading = true;

				// Wait some time IO from other processes to complete
				if (delay)
					await Task.Delay(100);

				if (Directory.Exists(ConfigurationRoot) == false)
					return;

				LogConfiguration = new LogConfiguration(ConfigurationRoot, _loggerFactory.CreateLogger<LogConfiguration>());
			}
			finally
			{
				_isReloading = false;
			}
		}

		#endregion

		#region Interface Implementations

		#region IConfigurationManager

		public LogConfiguration LogConfiguration
		{
			get => _logConfiguration;
			private set
			{
				if (_logConfiguration?.IsEqualTo(value) == true)
					return;

				_logConfiguration = value;

				LogConfigurationChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		public event EventHandler LogConfigurationChanged;

		public event EventHandler StorageConfigurationChanged;

		public StorageConfiguration StorageConfiguration
		{
			get => _storageConfiguration;
			private set
			{
				if (_storageConfiguration?.IsEqualTo(value) == true)
					return;

				_storageConfiguration = value;

				StorageConfigurationChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		#endregion

		#endregion
	}
}