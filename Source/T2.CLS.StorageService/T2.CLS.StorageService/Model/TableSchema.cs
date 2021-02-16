// Copyright (C) 2019 Topsoft (https://topsoft.by)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using T2.CLS.StorageService.Dto;

namespace T2.CLS.StorageService.Model
{
	internal sealed class TableSchema
	{
		#region Ctors

		public TableSchema(string name, IEnumerable<FieldSchema> fields, FieldSchema dateTimeField, IEnumerable<FieldSchema> sortFields)
		{
			Name = name;
			Fields = new ReadOnlyCollection<FieldSchema>(fields.ToList());
			SortFields = new ReadOnlyCollection<FieldSchema>(sortFields.ToList());
			DateTimeField = dateTimeField;

			Validate();
		}

		private TableSchema(TableSchemaDto dto)
		{
			var fieldsList = dto.Fields.Select(FieldSchema.FromDto).ToList();
			var dateTimeField = fieldsList.FirstOrDefault(f => string.Equals(f.Name, dto.DateTimeField, StringComparison.OrdinalIgnoreCase));
			var sortFields = fieldsList.Where(f => dto.SortFields.Contains(f.Name, StringComparer.OrdinalIgnoreCase));

			Name = dto.Name;
			Fields = new ReadOnlyCollection<FieldSchema>(fieldsList);
			SortFields = new ReadOnlyCollection<FieldSchema>(sortFields.ToList());
			DateTimeField = dateTimeField;

			Validate();
		}

		#endregion

		#region Properties

		public FieldSchema DateTimeField { get; }

		public ReadOnlyCollection<FieldSchema> Fields { get; }

		public string Name { get; set; }

		public ReadOnlyCollection<FieldSchema> SortFields { get; }

		#endregion

		#region  Methods

		public TableSchemaDto AsDto()
		{
			var tableSchemaDto = new TableSchemaDto
			{
				Name = Name
			};

			foreach (var fieldSchema in Fields)
				tableSchemaDto.Fields.Add(fieldSchema.AsDto());

			return tableSchemaDto;
		}

		public static TableSchema Deserialize(string str)
		{
			return FromDto(JsonConvert.DeserializeObject<TableSchemaDto>(str));
		}

		public FieldSchema FindField(string name)
		{
			return Fields.FirstOrDefault(f => string.Equals(f.Name, name, StringComparison.Ordinal));
		}

		public static TableSchema FromDto(TableSchemaDto tableSchemaDto)
		{
			return new TableSchema(tableSchemaDto);
		}

		public bool IsEqualTo(TableSchema table)
		{
			if (table == null)
				return false;

			if (Fields.Count != table.Fields.Count)
				return false;

			foreach (var field in Fields)
			{
				var otherField = table.FindField(field.Name);

				if (field.IsEqualTo(otherField) == false)
					return false;
			}

			return true;
		}

		public string Serialize()
		{
			return JsonConvert.SerializeObject(AsDto());
		}

		private void Validate()
		{
			var nameHashSet = new HashSet<string>(StringComparer.Ordinal);

			foreach (var fieldSchema in Fields)
				if (nameHashSet.Add(fieldSchema.Name) == false)
					throw new Exception("Duplicate field name");

			if (DateTimeField == null)
				throw new Exception("DateTimeField is not specified");

			if (SortFields.Count == 0)
				throw new Exception("SortFields are not specified");

		}

		#endregion
	}
}