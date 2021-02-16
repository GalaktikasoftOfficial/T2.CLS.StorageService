using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace T2.CLS.StorageService.Dto.ClickHouse
{
	public class OutputDataJson<T> where T: BaseObject
	{
		public IEnumerable<OutputDataMeta> meta { get; set; }
		public IEnumerable<T> data { get; set; }
	}

	public class OutputDataMeta
	{
		public string name { get; set; }
		public string type { get; set; }
	}
}
