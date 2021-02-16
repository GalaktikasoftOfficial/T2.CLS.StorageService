// Copyright (C) 2019 Topsoft (https://topsoft.by)

using System;
using System.Diagnostics;
using System.IO;

namespace T2.CLS.StorageService.Utils
{
	internal static class IOUtils
	{
		#region  Methods

		public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
		{
			// Get the subdirectories for the specified directory.
			var dir = new DirectoryInfo(sourceDirName);

			if (!dir.Exists)
			{
				throw new DirectoryNotFoundException(
					"Source directory does not exist or could not be found: "
					+ sourceDirName);
			}

			var dirs = dir.GetDirectories();

			// If the destination directory doesn't exist, create it.
			if (!Directory.Exists(destDirName))
			{
				Directory.CreateDirectory(destDirName);
			}

			// Get the files in the directory and copy them to the new location.
			var files = dir.GetFiles();

			foreach (var file in files)
			{
				var tempPath = Path.Combine(destDirName, file.Name);

				file.CopyTo(tempPath, false);
			}

			// If copying subdirectories, copy them and their contents to new location.
			if (copySubDirs)
			{
				foreach (var subDir in dirs)
				{
					var temppath = Path.Combine(destDirName, subDir.Name);

					DirectoryCopy(subDir.FullName, temppath, copySubDirs);
				}
			}
		}

		public static string GetAppBaseDirectory()
		{
			//return Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
			return AppContext.BaseDirectory;
		}

		#endregion
	}
}