// Copyright (C) 2019 Topsoft (https://topsoft.by)

using System;
using System.Collections.Generic;
using System.Linq;

namespace T2.CLS.StorageService.Utils
{
	internal static class TypeMapping
	{
		#region Static Fields and Constants

		private static readonly Dictionary<string, Type> DotNetTypesDictionary = new Dictionary<string, Type>
		{
			{"DateTime", typeof(DateTime)},
			{"String", typeof(string)},
			{"Byte", typeof(byte)},
			{"Int16", typeof(short)},
			{"Int32", typeof(int)},
			{"Int64", typeof(long)},
			{"UInt16", typeof(ushort)},
			{"UInt32", typeof(uint)},
			{"UInt64", typeof(ulong)},
		};

		private static readonly Dictionary<Type, string> ClickHouseTypeNamesDictionary;

		private static readonly Dictionary<string, Type> ClickHouseTypesDictionary = new Dictionary<string, Type>
		{
			{"DateTime", typeof(DateTime)},
			{"String", typeof(string)},
			{"Int8", typeof(sbyte)},
			{"Int16", typeof(short)},
			{"Int32", typeof(int)},
			{"Int64", typeof(long)},
			{"UInt8", typeof(byte)},
			{"UInt16", typeof(ushort)},
			{"UInt32", typeof(uint)},
			{"UInt64", typeof(ulong)},
		};

		#endregion

		#region Ctors

		static TypeMapping()
		{
			ClickHouseTypeNamesDictionary = ClickHouseTypesDictionary.ToDictionary(kv => kv.Value, kv => kv.Key);
		}

		#endregion

		#region  Methods

		public static Type FromClickHouseType(string typeName)
		{
			return ClickHouseTypesDictionary.TryGetValue(typeName, out var type) ? type : null;
		}

		public static Type FromTypeName(string typeName)
		{
			return DotNetTypesDictionary.TryGetValue(typeName, out var type) ? type : null;
		}

		public static string ToClickHouseTypeName(Type type)
		{
			return ClickHouseTypeNamesDictionary.TryGetValue(type, out var typeName) ? typeName : null;
		}

		#endregion
	}
}