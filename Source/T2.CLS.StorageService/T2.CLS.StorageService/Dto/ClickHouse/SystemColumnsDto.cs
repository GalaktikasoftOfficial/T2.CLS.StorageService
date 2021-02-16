using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace T2.CLS.StorageService.Dto.ClickHouse
{
	public class SystemColumnsDto : BaseObject
	{
		public string name { get; set; }
		public string type { get; set; }
		public byte is_in_partition_key { get; set; }
		public byte is_in_sorting_key { get; set; }
	}
}
