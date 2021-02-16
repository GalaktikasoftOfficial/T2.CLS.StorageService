using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using T2.CLS.StorageService.Dto.ClickHouse;

namespace T2.CLS.StorageService.Interfaces
{
	internal interface IClickHouseHttpClient
	{

		/// <summary>
		/// Исполнение запроса без ожидаемого результата 
		/// </summary>
		/// <param name="query">Запрос.</param>
		/// <param name="database">База данных по умолчани</param>
		void Execute(string query, string dataBase);

		/// <summary>
		/// Исполнение запроса без ожидаемого результата. База данных по умолчанию из конфигурации
		/// </summary>
		/// <param name="query">Запрос</param>
		void Execute(string query);

		/// <summary>
		// Исполнение запроса и парсинг из JSON в объекты
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="query"></param>
		/// <param name="database"></param>
		/// <returns></returns>
		IEnumerable<T> ExcuteRequest<T>(string query, string dataBase) where T: BaseObject ;

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="query"></param>
		/// <returns></returns>
		IEnumerable<T> ExcuteRequest<T>(string query) where T : BaseObject;

		void ExecuteScalar(string query, string dataBase, out int result);
		void ExecuteScalar(string query, string dataBase, out string result);

	}
}
