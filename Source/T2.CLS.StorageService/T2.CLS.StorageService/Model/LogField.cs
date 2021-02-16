// Copyright (C) 2019 Topsoft (https://topsoft.by)

using System;

namespace T2.CLS.StorageService.Model
{
	internal sealed class LogField
	{
		#region Ctors

		public LogField(string name, Type type)
		{
			Name = name;
			Type = type;
		}

		#endregion

		#region Properties

		public string Name { get; }

		public Type Type { get; }

		#endregion

		#region  Methods

		public FieldSchema AsFieldSchema()
		{
			return new FieldSchema(Name, Type);
		}

		public bool IsEqualTo(LogField field)
		{
			return field != null && string.Equals(Name, field.Name, StringComparison.Ordinal) && Type == field.Type;
		}

		#endregion
	}
}