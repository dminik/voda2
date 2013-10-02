namespace Nop.Services.SiteParsers
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Threading;
	using System.Web;

	using Nop.Core;
	using Nop.Core.Domain.YandexMarket;
	using Nop.Services.FileParsers;
	using Nop.Services.Logging;
	using Nop.Services.YandexMarket;

	using OfficeOpenXml;

	using OpenQA.Selenium;
	using OpenQA.Selenium.IE;
	using OpenQA.Selenium.Remote;
	using OpenQA.Selenium.Support.UI;

	
	public class F5Parser
	{
		private CookieContainer cookieCont = new CookieContainer();
		string strHtmlLast = "";

		

		public void Init(ILogger logger)
		{
			mLogger = logger;
		}

		
		// Папка для картинок
		private static string p_FileName = "f5_pricesss.xls";
		private readonly string mF5PriceFullFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProductsCatalog", p_FileName);
		
		protected ILogger mLogger;
		
		protected virtual string CssSelectorForProductLinkInProductList { get{throw new Exception("Not implemented");}}
		
		
		public void Parse(ILogger logger)
		{
			mLogger = logger;
		
			this.mLogger.Debug("Start Parsing...");

			HttpWebResponse myHttpWebResponse = null;

			try
			{

				myHttpWebResponse = GetAutherisationPage();
				myHttpWebResponse = PostAutherisation();





				// Скачиваем файл
				myHttpWebResponse = HTTP_Query(
												   "http://shop.f5.ua/20861/client/price/export/",
												   true,  //ispost
												   "",     //params                               
												   "application/x-www-form-urlencoded");     //contentType     												
			}
			catch (Exception ex)
			{
				this.mLogger.Debug(ex.Message, ex);
				throw;
			}
			finally
			{				
				
			}

			this.mLogger.Debug("End Parsing.");

			var stream = myHttpWebResponse.GetResponseStream();
			SaveExcelFile(mF5PriceFullFileName, stream);

		}

		//Страничка логинилка (получаем страничку для вводим пароль)
		private HttpWebResponse GetAutherisationPage()
		{
			#region ===== 1 Первый запрос основной странички логина

			HttpWebResponse myHttpWebResponse = HTTP_Query(
					"http://shop.f5.ua",
					false,
					"",
					"");

			CheckResponseCorrect(myHttpWebResponse, "", HttpStatusCode.OK);

			#endregion
			
			return myHttpWebResponse;
		}

		//Залогиниваемся (получаем Welcome)
		private HttpWebResponse PostAutherisation()
		{
			//1 Первый запрос основной странички логина
			HttpWebResponse myHttpWebResponse = HTTP_Query(
				"http://shop.f5.ua",
				true,
				"login=oleynic&password=18072008y&go=%D0%92%D1%85%D0%BE%D0%B4",
				"application/x-www-form-urlencoded");

			CheckResponseCorrect(myHttpWebResponse, "", HttpStatusCode.Found);
			return myHttpWebResponse;
		}


		private void CheckResponseCorrect(HttpWebResponse p_myHttpWebResponse, string p_originalStrInContent, HttpStatusCode status)
		{
			strHtmlLast = GetHTMLtext(p_myHttpWebResponse);

			if (p_myHttpWebResponse.StatusCode != status)
				throw new Exception("Url: " + p_myHttpWebResponse.ResponseUri
					+ "; expected http status is " + status.ToString()
					+ "; current http status is " + p_myHttpWebResponse.StatusCode.ToString());

			if (p_originalStrInContent != "")
				if (!strHtmlLast.Contains(p_originalStrInContent))
					throw new Exception("Url: " + p_myHttpWebResponse.ResponseUri + "; Downloaded page content do not have string like: " + p_originalStrInContent);
		}

		private string GetHTMLtext(HttpWebResponse p_HttpWebResponse)
		{
			try
			{
				Encoding enc = null;
				switch (p_HttpWebResponse.CharacterSet)
				{
					case "utf-8":
						enc = Encoding.UTF8;
						break;
					case "1251":
						enc = Encoding.GetEncoding(1251);
						break;

					default:
						enc = Encoding.UTF8;
						break;
				}

				StreamReader myStreamReader = new StreamReader(p_HttpWebResponse.GetResponseStream(), enc);
				strHtmlLast = myStreamReader.ReadToEnd();
				return strHtmlLast;
			}
			catch
			{
				return "";
			}
		}

		private HttpWebResponse HTTP_Query(string p_uri, bool p_isPost, string p_post_params, string p_contentType)
		{
			ServicePointManager.Expect100Continue = false;
			ServicePointManager.SetTcpKeepAlive(true, 6000000, 100000);

			HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(p_uri);
			

			myHttpWebRequest.Accept = "application/x-ms-application, image/jpeg, application/xaml+xml, image/gif, image/pjpeg, application/x-ms-xbap, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*"; // p_accept;// "application/x-ms-application, image/jpeg, application/xaml+xml, image/gif, image/pjpeg, application/x-ms-xbap, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
			myHttpWebRequest.Headers.Add("Accept-Language", "ru-RU");
			myHttpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; MRSPUTNIK 2, 3, 0, 289; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET CLR 1.1.4322; .NET4.0C; .NET4.0E; InfoPath.2)";
			myHttpWebRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
			//myHttpWebRequest.Host = "business.moneygram.com";


			if (p_contentType != "") myHttpWebRequest.ContentType = p_contentType;

			if (cookieCont.Count > 0)
				myHttpWebRequest.CookieContainer = cookieCont;//вписываем куки

			myHttpWebRequest.Timeout = 180000;//3 min
			myHttpWebRequest.AllowAutoRedirect = false;// ставим False, чтобы при получении кода 302 не делать автоматический редирект
			if (p_isPost)
			{
				myHttpWebRequest.Method = "POST";

				//Пишем параметры формы в запрос
				byte[] ByteArr = System.Text.Encoding.GetEncoding(1251).GetBytes(p_post_params);
				myHttpWebRequest.ContentLength = ByteArr.Length;
				myHttpWebRequest.GetRequestStream().Write(ByteArr, 0, ByteArr.Length);
				myHttpWebRequest.GetRequestStream().Close();
			}

			myHttpWebRequest.KeepAlive = true;

			//progressBar.SetProcessProgress(requestID, "Info - Перед GetResponse;");
			HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
			//progressBar.SetProcessProgress(requestID, "Info - После GetResponse;");

			//запоминаем полученные куки
			if (!String.IsNullOrEmpty(myHttpWebResponse.Headers["Set-Cookie"]))
				cookieCont.SetCookies(new Uri("http://shop.f5.ua"), myHttpWebResponse.Headers["Set-Cookie"]);

			return myHttpWebResponse;
		}

		public IEnumerable<ProductLineVendor> ReadXlsx(out List<string> errors)
		{
			errors = new List<string>();

			var resultProductLineVendors = this.LoadFromFile(mF5PriceFullFileName);


			return resultProductLineVendors;






			//FileInfo file = new FileInfo(mF5PriceFullFileName);

			//var fileStream = new FileStream(mF5PriceFullFileName, FileMode.Open, FileAccess.Read);

			//// ok, we can run the real code of the sample now
			//using (var xlPackage = new ExcelPackage(fileStream))
			//{
			//	// get the first worksheet in the workbook
			//	var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
			//	if (worksheet == null)
			//		throw new NopException("No worksheet found");

			//	//the columns
			//	var properties = new string[]
			//	{
			//		"Код товара",
			//		"Название товара",                   
			//		"Расч. цена",                   
			//		"Фикс. цена",                   
			//		"Акция",                   
			//		"Цена продажи",                   
			//		"Базовая цена",                   
			//		"Цена продажи – Базовая цена",                   					          
			//	};

				

			//	int currentRow = 1;
			//	while (true)
			//	{
			//		#region Is last string in file
					
			//		bool allColumnsAreEmpty = true;

			//		for (var currentColumn = 1; currentColumn <= properties.Length; currentColumn++)
			//		{
			//			if (worksheet.Cells[currentRow, currentColumn].Value != null
			//				&& !String.IsNullOrEmpty(worksheet.Cells[currentRow, currentColumn].Value.ToString()))
			//			{
			//				allColumnsAreEmpty = false;
			//				break;
			//			}
			//		}

			//		if (allColumnsAreEmpty)
			//			break;

			//		#endregion

			//		var artikul = worksheet.Cells[currentRow, GetColumnIndex(properties, "Код товара")].Value as string;
					
			//		if (artikul.Length != 7) // articul должен быть 7 символов
			//			continue;

			//		var newProductLineVendor = new ProductLineVendor();

			//		try
			//		{
			//			newProductLineVendor.Articul = artikul;
			//			newProductLineVendor.Name = worksheet.Cells[currentRow, GetColumnIndex(properties, "Название товара")].Value as string;

			//			newProductLineVendor.PriceRaschet = GetInt(worksheet.Cells[currentRow, GetColumnIndex(properties, "Расч. цена")].Value as string);
			//			newProductLineVendor.Price = GetInt(worksheet.Cells[currentRow, GetColumnIndex(properties, "Цена продажи")].Value as string);
			//			newProductLineVendor.PriceBase = GetInt(worksheet.Cells[currentRow, GetColumnIndex(properties, "Базовая цена")].Value as string);
			//			newProductLineVendor.PriceDiff = GetInt(worksheet.Cells[currentRow, GetColumnIndex(properties, "Цена продажи – Базовая цена")].Value as string);
			//		}
			//		catch (Exception ex)
			//		{
			//			errors.Add(worksheet.Cells[currentRow, GetColumnIndex(properties, "Код товара")].Value as string);
			//			continue;
			//		}

			//		resultProductLineVendors.Add(newProductLineVendor);


			//		//next product
			//		currentRow++;
			//	}// end while
			//}// end using

			//return resultProductLineVendors;
		}

		protected virtual int GetColumnIndex(string[] properties, string columnName)
		{
			if (properties == null)
				throw new ArgumentNullException("properties");

			if (columnName == null)
				throw new ArgumentNullException("columnName");

			for (int i = 0; i < properties.Length; i++)
				if (properties[i].Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
					return i + 1; //excel indexes start from 1
			return 0;
		}

		private static int GetInt(string str)
		{
			if (str == "") return 0;

			var partStr = str.Split('.')[0];
			var result = int.Parse(partStr);
			return result;
		}

		private void SaveExcelFile(string p_FileName, Stream stream)
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



		private const string dataSheetName = "Sheet1";


		public IEnumerable<ProductLineVendor> LoadFromFile(string fullFileName)
		{
			string conString = FormatXlsConnectionString(fullFileName);
			IEnumerable<ProductLineVendor> result = null;
			
			using (var con = new System.Data.OleDb.OleDbConnection(conString))
			{
				using (var cmd = new System.Data.OleDb.OleDbCommand())
				{
					cmd.Connection = con;
					cmd.CommandType = CommandType.TableDirect;
					cmd.CommandText = FormatXlsTableName(dataSheetName);

					con.Open();
					using (var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
					{
						if (reader != null)
							result = GetDailyActivityFileLineDetailsListFromReader(reader);
					}
				}
			}
			return result;
		}

		protected static ProductLineVendor GetDailyActivityFileLineDetailsFromReader(IDataReader reader)
		{
			var newProductLineVendor = new ProductLineVendor();
			
			try
			{

				var artikul = (string)GetFieldValueFromReader(reader, "Код товара", typeof(string));

				if (artikul.Length != 7) // articul должен быть 7 символов
					return null;

				newProductLineVendor.Articul = artikul;
				newProductLineVendor.Name = (string)GetFieldValueFromReader(reader, "Название товара", typeof(string));
				
				newProductLineVendor.PriceRaschet = GetInt((string)GetFieldValueFromReader(reader, "Расч# цена", typeof(string)));				
				newProductLineVendor.Price = GetInt((string)GetFieldValueFromReader(reader, "Цена продажи", typeof(string)));				
				newProductLineVendor.PriceBase = GetInt((string)GetFieldValueFromReader(reader, "Базовая цена", typeof(string)));				
				newProductLineVendor.PriceDiff = GetInt((string)GetFieldValueFromReader(reader, "Цена продажи – Базовая цена", typeof(string)));

			}
			catch (Exception ex)
			{
				// errors.Add(worksheet.Cells[currentRow, GetColumnIndex(properties, "Код товара")].Value as string);
				return null;
			}
			return newProductLineVendor;
		}

		protected static IEnumerable<ProductLineVendor> GetDailyActivityFileLineDetailsListFromReader(IDataReader reader)
		{
			var result = new List<ProductLineVendor>();
			while (reader.Read())
			{
				var s = GetDailyActivityFileLineDetailsFromReader(reader);

				if(s != null)
					result.Add(s);
			}
			return result;
		}

		public static string FormatXlsConnectionString(string fullFileName)
		{
			// "HDR=Yes;" indicates that the first row contains columnnames, not data. 
			// "HDR=No;" indicates the opposite.
			// "IMEX=1;" tells the driver to always read "intermixed" (numbers, dates, strings etc) data columns as text. 
			//    Note that this option might affect excel sheet write access negative.
			return string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"{0}\";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"",
				fullFileName);
		}

		public static string FormatXlsTableName(string dataSheetName)
		{
			return string.Format("{0}$", dataSheetName);
		}

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

		#endregion Converters		

		#region GetFieldValueFromReader

		//Ищем значение в текущей строке
		protected static int? IsExistFieldValue(IDataRecord rec, string fieldValue)
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

		protected static int? GetFieldIndex(IDataRecord rec, string fieldName)
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

		protected static object GetFieldValueFromReader(IDataReader reader, string fieldName, Type resultType,
			bool isRequired, object defaultValue)
		{
			
			
			
			object result = null;

			int? fieldIndex = GetFieldIndex(reader, fieldName);




			if (fieldIndex.HasValue)
			{
				result = CodeValue(reader[fieldIndex.Value]);
				if (result == null)
				{
					if (isRequired)
						throw new Exception(string.Format("Не задано значение обязательного поля \"{0}\"", fieldName));
					else
						result = defaultValue;
				}
				string convertErrMessage = null;
				if (!TryConvertValue(result, resultType, out result, out convertErrMessage))
					throw new Exception(string.Format("Ошибка преобразования значения \"{0}\" поля \"{1}\" в тип \"{2}\": {3}",
						result, fieldName, resultType, convertErrMessage));
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

		protected static object GetFieldValueFromReader(IDataReader reader, string fieldName, Type resultType,
			bool isRequired)
		{
			return GetFieldValueFromReader(reader, fieldName, resultType, isRequired, null);
		}

		protected static object GetFieldValueFromReader(IDataReader reader, string fieldName, Type resultType)
		{
			return GetFieldValueFromReader(reader, fieldName, resultType, false, null);
		}

		#endregion GetFieldValueFromReader

		#region Converters

		public static object DbValue(object codeValue)
		{
			return (codeValue == null ? DBNull.Value : codeValue);
		}

		public static object CodeValue(object dbValue)
		{
			return (dbValue == DBNull.Value ? null : dbValue);
		}

		protected static bool TryConvertValue(object value, Type resultType,
			out object result, out string errorMessage)
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
					if (value == null || ((value is string) && ((string)value) == string.Empty))
						return true;
					else
						resultType = Nullable.GetUnderlyingType(resultType);
				}
				result = Convert.ChangeType(value, resultType); // TO-DO: заюзать еще IFormatProvider provider
				return true;
			}
			catch (InvalidCastException)
			{
				errorMessage = string.Format("значение {0} не может быть преобразовано в {1}", value, resultType);
				return false;
			}
		}

		#endregion Converters
	}
}
