// Copyright (C) 2019 Topsoft (https://topsoft.by)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using T2.CLS.StorageService.Dto;
using T2.CLS.StorageService.Utils;

namespace T2.CLS.StorageService.Model
{
	internal sealed class LogSystemConfiguration
	{
		#region Fields

		private readonly List<LogField> _fields = new List<LogField>();
		private readonly Dictionary<string, LogField> _fieldsDictionary = new Dictionary<string, LogField>();
		private readonly List<LogField> _orderByFields = new List<LogField>();

		#endregion

		#region Ctors

		private LogSystemConfiguration()
		{
			Fields = new ReadOnlyCollection<LogField>(_fields);
			OrderByFields = new ReadOnlyCollection<LogField>(_orderByFields);
		}

		private LogSystemConfiguration(LogSystemConfigurationDto dto) : this()
		{
			Name = dto.Name;
			PartitionLifetime = dto.PartitionLifetime;

			foreach (var logFieldDto in dto.Fields)
			{
				var logField = new LogField(logFieldDto.Name, TypeMapping.FromTypeName(logFieldDto.TypeName));

				if (string.Equals(logField.Name, dto.DateTimeField, StringComparison.OrdinalIgnoreCase))
					DateTimeField = logField;

				if (dto.SortFields.Contains(logField.Name, StringComparer.OrdinalIgnoreCase))
					_orderByFields.Add(logField);

				_fieldsDictionary.Add(logField.Name, logField);
				_fields.Add(logField);
			}
		}

		#endregion

		#region Properties

		public LogField DateTimeField { get; }

		public ReadOnlyCollection<LogField> Fields { get; }

		public string Id { get; set; }

		public string Name { get; }

		public ReadOnlyCollection<LogField> OrderByFields { get; }

		public LogField PartitionByField { get; }

		public TimeSpan PartitionLifetime { get; }

		#endregion

		#region  Methods

		public TableSchema AsTableSchema()
		{
			var fields = Fields.Select(f => f.AsFieldSchema()).ToList();
			var dateTimeField = fields.FirstOrDefault(f => f.Name == DateTimeField.Name);
			var sortFieldsHashSet = new HashSet<string>(OrderByFields.Select(f => f.Name));
			var sortFields = fields.Where(f => sortFieldsHashSet.Contains(f.Name)).ToList();

			return new TableSchema(Name, fields, dateTimeField, sortFields);
		}

		private LogField FindField(string name)
		{
			return _fieldsDictionary.TryGetValue(name, out var field) ? field : null;
		}

		public bool IsEqualTo(LogSystemConfiguration systemConfiguration)
		{
			if (systemConfiguration == null)
				return false;

			if (string.Equals(Name, systemConfiguration.Name, StringComparison.Ordinal) == false)
				return false;

			if (_fields.Count != systemConfiguration._fields.Count)
				return false;

			foreach (var field in _fields)
			{
				var otherField = systemConfiguration.FindField(field.Name);

				if (field.IsEqualTo(otherField) == false)
					return false;
			}

			return true;
		}

		public static LogSystemConfiguration ReadFromFile(string logConfigPath)
		{
			using var fileStream = new FileStream(logConfigPath, FileMode.Open, FileAccess.Read);
			using var streamReader = new StreamReader(fileStream);
			var content = streamReader.ReadToEnd();
			var logConfigDto = JsonConvert.DeserializeObject<LogSystemConfigurationDto>(content);

			return new LogSystemConfiguration(logConfigDto);
		}

		#endregion
	}
}