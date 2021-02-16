using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using T2.CLS.StorageService.Dto;
using T2.CLS.StorageService.Dto.ClickHouse;
using T2.CLS.StorageService.Interfaces;
using T2.CLS.StorageService.Model;

namespace T2.CLS.StorageService.ClickHouse
{
	internal class ClickHouseHttpClient : IClickHouseHttpClient
	{
		#region Fields

		private ILogger<ClickHouseHttpClient> _logger;
		private ILoggerFactory                _loggerFactory;
		private HttpClient _httpClient;
		private IConfigurationManager _configurationManager;

		#endregion

		#region ctr

		public ClickHouseHttpClient(IConfigurationManager configurationManager, ILoggerFactory loggerFactory)
		{
			_loggerFactory = loggerFactory;
			_logger = _loggerFactory.CreateLogger<ClickHouseHttpClient>();
			_configurationManager = configurationManager;
		}

		#endregion

		#region Properties

		internal HttpClient HttpClient
		{
			get
			{
				if (_httpClient == null)
				{
					_httpClient = new HttpClient();
					_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
						Convert.ToBase64String(Encoding.UTF8.GetBytes(
							$"{_configurationManager.StorageConfiguration.User}:{_configurationManager.StorageConfiguration.Password}")));
				}

				return _httpClient;
			}
		}
		
		#endregion

		#region IClickHouseHttpClient


		public void Execute(string query) 
		{
			Execute(query, string.Empty);
		}

		public void Execute(string query, string dataBase)
		{
			try
			{
				var db = !string.IsNullOrEmpty(dataBase) ? dataBase : !string.IsNullOrEmpty(_configurationManager.StorageConfiguration.DataBase) ? _configurationManager.StorageConfiguration.DataBase  : "default";

				var originalUri = new Uri(_configurationManager.StorageConfiguration.HttpUrl);
				var uri = new Uri(originalUri, $"?database={db}");
				
				var result = HttpClient.PostAsync(uri, new StringContent(query));

				if (result.Result.IsSuccessStatusCode)
				{
					return;
				}

				throw result.Exception;
			}
			catch (Exception e)
			{
				_logger.LogError(e, "ExecuteError");
				throw e;
			}
		}

		public IEnumerable<T> ExcuteRequest<T>(string query, string dataBase) where T : BaseObject
		{
			IEnumerable<T> result = null;

			try
			{
				var db = !string.IsNullOrEmpty(dataBase) ? dataBase : !string.IsNullOrEmpty(_configurationManager.StorageConfiguration.DataBase) ? _configurationManager.StorageConfiguration.DataBase : "default";

				var originalUri = new Uri(_configurationManager.StorageConfiguration.HttpUrl);
				var uri = new Uri(originalUri, $"?database={db}");

				var httpResult = HttpClient.PostAsync(uri, new StringContent(query + " FORMAT JSON"));

				if (httpResult.Result.IsSuccessStatusCode)
				{
					var contentResult = httpResult.Result.Content.ReadAsStringAsync().Result;
					var outputData = JsonConvert.DeserializeObject<OutputDataJson<T>>(contentResult);
					result = outputData?.data ?? new List<T>();
					return result;
				}

				throw httpResult.Exception;
			}
			catch (Exception e)
			{
				_logger.LogError(e, "ExecuteScalar");
				throw e;
			}

		}

		public IEnumerable<T> ExcuteRequest<T>(string query) where T : BaseObject
		{
			return ExcuteRequest<T>(query, string.Empty);
		}

		public void ExecuteScalar(string query, string dataBase, out int result)
		{
			result = default(int);

			try
			{
				var db = !string.IsNullOrEmpty(dataBase) ? dataBase : !string.IsNullOrEmpty(_configurationManager.StorageConfiguration.DataBase) ? _configurationManager.StorageConfiguration.DataBase : "default";

				var originalUri = new Uri(_configurationManager.StorageConfiguration.HttpUrl);
				var uri = new Uri(originalUri, $"?database={db}");

				var httpResult = HttpClient.PostAsync(uri, new StringContent(query));

				if (httpResult.Result.IsSuccessStatusCode)
				{
					var contentResult = httpResult.Result.Content.ReadAsStringAsync().Result;
					result = Convert.ToInt32(contentResult);
					return;
				}

				throw httpResult.Exception;
			}
			catch (Exception e)
			{
				_logger.LogError(e, "ExecuteScalar");
				throw e;
			}
		}

		public void ExecuteScalar(string query, string dataBase, out string result)
		{
			result = default(string);

			try
			{
				var db = !string.IsNullOrEmpty(dataBase) ? dataBase : !string.IsNullOrEmpty(_configurationManager.StorageConfiguration.DataBase) ? _configurationManager.StorageConfiguration.DataBase : "default";

				var originalUri = new Uri(_configurationManager.StorageConfiguration.HttpUrl);
				var uri = new Uri(originalUri, $"?database={db}");

				var httpResult = HttpClient.PostAsync(uri, new StringContent(query));

				if (httpResult.Result.IsSuccessStatusCode)
				{
					var contentResult = httpResult.Result.Content.ReadAsStringAsync().Result;
					result = contentResult;
					return;
				}

				throw httpResult.Exception;
			}
			catch (Exception e)
			{
				_logger.LogError(e, "ExecuteScalar");
				throw e;
			}
		}


		#endregion
	}
}
