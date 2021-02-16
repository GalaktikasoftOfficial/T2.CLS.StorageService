// Copyright (C) 2019 Topsoft (https://topsoft.by)

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;

namespace T2.CLS.StorageService.Model
{
	internal sealed class LogConfiguration
	{
		#region Fields

		private readonly List<LogSystemConfiguration> _logConfigurations = new List<LogSystemConfiguration>();
		private readonly IReadOnlyCollection<LogSystemConfiguration> _readOnlyCollection;
		private readonly Dictionary<string, LogSystemConfiguration> _systemConfigurationsDictionary = new Dictionary<string, LogSystemConfiguration>();

		#endregion

		#region Ctors

		public LogConfiguration(string configurationRoot, ILogger<LogConfiguration> logger)
		{
			_readOnlyCollection = _logConfigurations.AsReadOnly();

			foreach (var logConfigPath in Directory.EnumerateFiles(configurationRoot, "*.json"))
			{
				try
				{
					var systemConfiguration = LogSystemConfiguration.ReadFromFile(logConfigPath);

					_systemConfigurationsDictionary.Add(systemConfiguration.Name, systemConfiguration);
					_logConfigurations.Add(systemConfiguration);
				}
				catch (Exception e)
				{
					logger.LogError(e, $"Can not read log configuration file '{logConfigPath}'");
				}
			}
		}

		#endregion

		#region Properties

		public IEnumerable<LogSystemConfiguration> SystemConfigurations => _readOnlyCollection;

		#endregion

		#region  Methods

		private LogSystemConfiguration FindSystemConfiguration(string name)
		{
			return _systemConfigurationsDictionary.TryGetValue(name, out var systemConfiguration) ? systemConfiguration : null;
		}

		public bool IsEqualTo(LogConfiguration logConfiguration)
		{
			if (logConfiguration == null)
				return false;

			if (_readOnlyCollection.Count != logConfiguration._readOnlyCollection.Count)
				return false;

			foreach (var systemConfiguration in SystemConfigurations)
			{
				var otherSystemConfiguration = logConfiguration.FindSystemConfiguration(systemConfiguration.Name);

				if (systemConfiguration.IsEqualTo(otherSystemConfiguration) == false)
					return false;
			}

			return true;
		}

		#endregion
	}
}