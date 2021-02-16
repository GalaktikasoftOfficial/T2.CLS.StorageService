using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace T2.CLS.TestConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			HttpClient client = new HttpClient();
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
				Convert.ToBase64String(Encoding.UTF8.GetBytes(
					$"default:topsoft")));
			var db = "system";

			var originalUri = new Uri("http://192.168.208.3:8123/");
			var uri = new Uri(originalUri, $"?database={db}");
			
			
			var result = client.PostAsync(uri, new StringContent("CREATE DATABASE IF NOT EXISTS TESTDB"));
			if (result.Result.IsSuccessStatusCode)
			{
				Console.WriteLine(result.Result.Content.ReadAsStringAsync().Result);
			}
			else
			{
				Console.WriteLine(result.Exception);
			}
		}
	}
}
