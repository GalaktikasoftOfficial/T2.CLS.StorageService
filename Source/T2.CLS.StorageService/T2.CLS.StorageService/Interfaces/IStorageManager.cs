// Copyright (C) 2019 Topsoft (https://topsoft.by)

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using T2.CLS.StorageService.Dto;

namespace T2.CLS.StorageService.Interfaces
{
	public interface IStorageManager
	{
		#region  Methods
	/*
		Task<List<ArchiveInfoDto>> GetArchives(string systemName);
		*/

		Task<bool> HandleStorage(CancellationToken stoppingToken);
/*
		Task<bool> RestoreArchive(string id, TimeSpan lifeTime);
		*/

		#endregion
	}
}