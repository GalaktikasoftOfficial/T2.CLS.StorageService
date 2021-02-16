// Copyright (C) 2019 Topsoft (https://topsoft.by)

using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using T2.CLS.StorageService.ClickHouse;
using T2.CLS.StorageService.Interfaces;
using T2.CLS.StorageService.Services;
using T2.CLS.StorageService.Utils;

namespace T2.CLS.StorageService
{
	public static class Program
	{
		#region  Methods

		private static IHostBuilder CreateHostBuilder(string[] args)
		{
			return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.ConfigureKestrel(serverOptions =>
					{
						serverOptions.ListenAnyIP(5000);
					});
					webBuilder.UseStartup<Startup>();
				})
				.ConfigureServices((hostContext, services) =>
				{
					services.AddLogging();
					services.AddHostedService<Service>();

					services.AddSingleton<IConfigurationManager, ConfigurationManager>();
					services.AddSingleton<IStorageManager, StorageManager>();
					services.AddSingleton<IClickHouseHttpClient, ClickHouseHttpClient>();
					services.AddSingleton(p => p.GetService<IConfigurationManager>().StorageConfiguration);
				})
				.ConfigureAppConfiguration((hostContext, configApp) =>
				{
					configApp.AddJsonFile(Path.Combine(IOUtils.GetAppBaseDirectory(), "appsettings.json") , false);
					configApp.AddCommandLine(args);
				})
				.ConfigureLogging((hostContext, configLogging) =>
				{
					configLogging.AddConsole();
					configLogging.AddDebug();
				});
		}

		public static void Main(string[] args)
		{
#if DEBUG
			WaitDebugger().Wait();
#endif

			CreateHostBuilder(args).Build().Run();
		}

		private static async Task WaitDebugger()
		{
			while (true)
			{
				await Task.Delay(100);

				if (Debugger.IsAttached)
					break;
			}
		}

		#endregion
	}
}