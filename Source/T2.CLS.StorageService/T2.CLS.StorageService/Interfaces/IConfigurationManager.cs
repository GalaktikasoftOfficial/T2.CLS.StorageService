// Copyright (C) 2019 Topsoft (https://topsoft.by)

using System;
using T2.CLS.StorageService.Model;

namespace T2.CLS.StorageService.Interfaces
{
	internal interface IConfigurationManager
	{
		#region Fields

		event EventHandler LogConfigurationChanged;

		event EventHandler StorageConfigurationChanged;

		#endregion

		#region Properties

		LogConfiguration LogConfiguration { get; }

		StorageConfiguration StorageConfiguration { get; }

		#endregion
	}
}