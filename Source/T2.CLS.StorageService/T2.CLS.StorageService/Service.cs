// Copyright (C) 2019 Topsoft (https://topsoft.by)

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using T2.CLS.StorageService.Interfaces;

namespace T2.CLS.StorageService
{
	internal sealed class Service : BackgroundService
	{
		#region Fields

		private readonly ILogger<Service> _logger;
		private readonly IStorageManager _storageManager;

		#endregion

		#region Ctors

		public Service(IStorageManager storageManager, ILogger<Service> logger)
		{
			_storageManager = storageManager;
			_logger = logger;
		}

		#endregion

		#region  Methods

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("Service started.");

			while (stoppingToken.IsCancellationRequested == false)
			{
				if (await _storageManager.HandleStorage(stoppingToken) == false)
					break;

				try
				{
					await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
				}
				catch (TaskCanceledException)
				{
				}
			}

			_logger.LogInformation("Service finished.");
		}

		#endregion
	}
}