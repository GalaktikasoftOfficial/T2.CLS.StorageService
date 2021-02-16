// Copyright (C) 2019 Topsoft (https://topsoft.by)

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using T2.CLS.StorageService.Dto;
using T2.CLS.StorageService.Interfaces;

namespace T2.CLS.StorageService.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class ArchiveController : ControllerBase
	{
		#region Fields

		private readonly ILogger<ArchiveController> _logger;

		private readonly IStorageManager _storageManager;

		#endregion

		#region Ctors

		public ArchiveController(IStorageManager storageManager, ILogger<ArchiveController> logger)
		{
			_storageManager = storageManager;
			_logger = logger;
		}

		#endregion

		#region  Methods

		/*[HttpGet]
		public async Task<IEnumerable<ArchiveInfoDto>> Get(string systemName)
		{
			return await _storageManager.GetArchives(systemName);
		}

		[HttpGet("Restore")]
		public async Task<bool> Restore(string id, TimeSpan lifeTime)
		{
			return await _storageManager.RestoreArchive(id, lifeTime);
		}*/

		#endregion
	}
}