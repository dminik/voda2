using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;

namespace Nop.Core.IO
{
	using System.Data;

	public static class XlsProvider  
	{
		private const string DataSheetName = "Sheet1";

		private delegate IEnumerable<T> GetListFromReader<out T>(IDataReader reader);
		public delegate T GetObjectFromReader<out T>(IDataReader reader);

		public static void SaveStreamToFile(string p_FileName, Stream stream)
		{
			BinaryReader reader = null;
			FileStream fileStream = null;
			BinaryWriter writer = null;

			try
			{
				reader = new BinaryReader(stream);

				fileStream = new FileStream(p_FileName, FileMode.Create, FileAccess.Write);
				writer = new BinaryWriter(fileStream);
				byte[] buffer = new byte[4096];
				int ret;
				int byteTotal = 0;

				while ((ret = reader.Read(buffer, 0, buffer.Length)) > 0)
				{
					writer.Write(buffer, 0, ret);
					byteTotal += ret;
				}

			}
			finally
			{
				if (reader != null) reader.Close();
				if (writer != null)
				{
					writer.Flush();
					writer.Close();
				}


				if (fileStream != null)
				{
					fileStream.Close();
				}
			}
		}

		public static IEnumerable<T> LoadFromFile<T>(string fullFileName, GetObjectFromReader<T> getObjectFromReaderFunc)
		{
			string conString = FormatXlsConnectionString(fullFileName);
			IEnumerable<T> result = null;

			using (var con = new System.Data.OleDb.OleDbConnection(conString))
			{
				using (var cmd = new System.Data.OleDb.OleDbCommand())
				{
					cmd.Connection = con;
					cmd.CommandType = CommandType.TableDirect;
					cmd.CommandText = FormatXlsTableName(DataSheetName);

					con.Open();
					using (var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
					{
						if (reader != null)
							result = GetDailyActivityFileLineDetailsListFromReader<T>(reader, getObjectFromReaderFunc);
					}
				}
			}
			return result;
		}

		private static IEnumerable<T> GetDailyActivityFileLineDetailsListFromReader<T>(IDataReader reader, GetObjectFromReader<T> getObjectFromReaderFunc)
		{
			var result = new List<T>();
			while (reader.Read())
			{
				var s = getObjectFromReaderFunc(reader);

				if (s != null)
					result.Add(s);
			}
			return result;
		}

		#region Converters

	

		#endregion Converters

		#region GetFieldValueFromReader

		//Ищем значение в текущей строке
		private static int? IsExistFieldValue(IDataRecord rec, string fieldValue)
		{
			if (rec != null && !string.IsNullOrEmpty(fieldValue))
			{
				for (int i = 0; i < rec.FieldCount; i++)
				{
					if (string.Compare(rec[i].ToString(), fieldValue, true) == 0)
						return i;
				}
			}

			return null;
		}

		private static int? GetFieldIndex(IDataRecord rec, string fieldName)
		{
			if (rec != null && !string.IsNullOrEmpty(fieldName))
			{
				for (int i = 0; i < rec.FieldCount; i++)
				{
					if (string.Compare(rec.GetName(i), fieldName, true) == 0)
						return i;
				}
			}
			return null;
		}

		public static T GetFieldValueFromReader<T>(IDataReader reader, string fieldName, bool isRequired, T defaultValue) 
			where T : class 
		{
			T result;

			int? fieldIndex = GetFieldIndex(reader, fieldName);

			if (fieldIndex.HasValue)
			{
				var resultObj = CodeValue(reader[fieldIndex.Value]);
				if (resultObj == null)
				{
					if (isRequired)
						throw new Exception(string.Format("Не задано значение обязательного поля \"{0}\"", fieldName));
					else
						result = defaultValue;
				}
				string convertErrMessage = null;

				var resultType = typeof(T);

				if (!TryConvertValue<T>(resultObj, resultType, out result, out convertErrMessage))
					throw new Exception(string.Format("Ошибка преобразования значения \"{0}\" поля \"{1}\" в тип \"{2}\": {3}",
						resultObj, fieldName, resultType, convertErrMessage));
			}
			else
			{
				if (isRequired && defaultValue == null)
					throw new Exception(string.Format("Поле \"{0}\" не найдено", fieldName));
				else
					result = defaultValue;
			}
			return result;
		}

		public static T GetFieldValueFromReader<T>(IDataReader reader, string fieldName, bool isRequired)
			where T : class 
		{
			return GetFieldValueFromReader<T>(reader, fieldName, isRequired, null);
		}

		public static T GetFieldValueFromReader<T>(IDataReader reader, string fieldName)
			where T : class 
		{
			return GetFieldValueFromReader<T>(reader, fieldName, false, null);
		}

		#endregion GetFieldValueFromReader

		#region Converters

		private static string IdToStr(object id)
		{
			string result = null;
			if (id != null)
			{
				if (id is string)
					result = Convert.ToString(id);
				else if (id is double?)
					result = Convert.ToString(Math.Truncate((id as double?).Value));
			}
			return result;
		}

		private static int? IdToInt(object id)
		{
			int? result = null;
			if (id != null)
			{
				if (id is string)
					result = Convert.ToInt32(id);
				else if (id is double?)
					result = Convert.ToInt32(Math.Truncate((id as double?).Value));
			}
			return result;
		}


		private static decimal? AmountToDecimal(object amount)
		{
			decimal? result = null;
			if (amount != null)
			{
				if (amount is string)
					result = Convert.ToDecimal(amount);
				else if (amount is double?)
					result = Convert.ToDecimal((amount as double?).Value);
			}
			return result;
		}

		private static object DbValue(object codeValue)
		{
			return (codeValue == null ? DBNull.Value : codeValue);
		}

		private static object CodeValue(object dbValue)
		{
			return (dbValue == DBNull.Value ? null : dbValue);
		}

		private static bool TryConvertValue<T>(object value, Type resultType, out T result, out string errorMessage)
			where T : class 
		{

			result = null;
			errorMessage = null;
			bool isNullableType = (Nullable.GetUnderlyingType(resultType) != null);
			if (value == null && resultType.IsValueType && !isNullableType)
			{
				errorMessage = string.Format("значение типа {0} не может быть пустым (NULL)", resultType);
				return false;
			}
			try
			{
				if (isNullableType)
				{
					if (value == null || ((value is string) && (value.ToString()) == string.Empty))
						return true;
					else
						resultType = Nullable.GetUnderlyingType(resultType);
				}
				result = (T) Convert.ChangeType(value, resultType); // TO-DO: заюзать еще IFormatProvider provider
				return true;
			}
			catch (InvalidCastException)
			{
				errorMessage = string.Format("значение {0} не может быть преобразовано в {1}", value, resultType);
				return false;
			}
		}

		#endregion Converters

		#region Helpers 

		private static string FormatXlsConnectionString(string fullFileName)
		{
			// "HDR=Yes;" indicates that the first row contains columnnames, not data. 
			// "HDR=No;" indicates the opposite.
			// "IMEX=1;" tells the driver to always read "intermixed" (numbers, dates, strings etc) data columns as text. 
			//    Note that this option might affect excel sheet write access negative.
			return string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"{0}\";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"",
				fullFileName);
		}

		private static string FormatXlsTableName(string dataSheetName)
		{
			return string.Format("{0}$", dataSheetName);
		}
		#endregion Helpers
	}
}