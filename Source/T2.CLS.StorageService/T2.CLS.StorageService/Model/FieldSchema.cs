// Copyright (C) 2019 Topsoft (https://topsoft.by)

using System;
using T2.CLS.StorageService.Dto;
using T2.CLS.StorageService.Utils;

namespace T2.CLS.StorageService.Model
{
	internal sealed class FieldSchema
	{
		#region Ctors

		public FieldSchema(string name, Type type)
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

		public FieldSchemaDto AsDto()
		{
			return new FieldSchemaDto
			{
				Name = Name,
				TypeName = TypeMapping.ToClickHouseTypeName(Type)
			};
		}

		public static FieldSchema FromDto(FieldSchemaDto fieldSchemaDto)
		{
			return new FieldSchema(fieldSchemaDto.Name, TypeMapping.FromTypeName(fieldSchemaDto.TypeName));
		}

		public bool IsEqualTo(FieldSchema field)
		{
			return field != null && string.Equals(Name, field.Name, StringComparison.Ordinal) && Type == field.Type;
		}

		#endregion
	}
}