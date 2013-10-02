namespace Nop.Services.SiteParsers
{
	using System;
	using System.Collections;
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
	using Nop.Core.IO;
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
		protected ILogger mLogger; 
		private readonly CookieContainer _cookieCont = new CookieContainer();
		string _strHtmlLast = "";
				
		private readonly string _f5PriceFullFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProductsCatalog", "f5_pricesss.xls");
		


		public IEnumerable<ProductLineVendor> GetPriceListFromCache(ILogger logger, out List<string> errors, bool isUpdateCacheFromInternet)
		{
			errors = new List<string>();
			IEnumerable<ProductLineVendor> resultProductLineVendors;

			if(isUpdateCacheFromInternet)
				DownloadNewPriceListToCache(logger);

			this.mLogger.Debug("Start  GetPriceListFromCache...");

			try
			{
				resultProductLineVendors = XlsProvider.LoadFromFile<ProductLineVendor>(this._f5PriceFullFileName, GetProductLineVendorFromReader);

				if (resultProductLineVendors.Count() < 9000)
					throw new Exception("Hm... Price list less then 9000 lines. Count=" + resultProductLineVendors.Count());
			}
			catch (Exception ex)
			{
				this.mLogger.Debug(ex.Message, ex);
				this.mLogger.Debug("Not success GetPriceListFromCache.");
				throw;
			}

			this.mLogger.Debug("End  GetPriceListFromCache.");

			return resultProductLineVendors;
		}

		private void DownloadNewPriceListToCache(ILogger logger)
		{
			mLogger = logger;

			this.mLogger.Debug("Start  DownloadNewPriceListToCache...");

			HttpWebResponse myHttpWebResponse = null;

			try
			{
				// Ходим по страницам и логинимся
				myHttpWebResponse = GetAutherisationPage();
				myHttpWebResponse = PostAutherisation();

				// Скачиваем файл
				myHttpWebResponse = this.HttpQuery("http://shop.f5.ua/20861/client/price/export/",
												   true,  //ispost
												   "",     //params                               
												   "application/x-www-form-urlencoded");     //contentType     	

				var stream = myHttpWebResponse.GetResponseStream();
				XlsProvider.SaveStreamToFile(this._f5PriceFullFileName, stream);
			}
			catch (Exception ex)
			{
				this.mLogger.Debug(ex.Message, ex);
				this.mLogger.Debug("Not success DownloadNewPriceListToCache.");
				throw;
			}

			this.mLogger.Debug("End  DownloadNewPriceListToCache.");
		}

		private HttpWebResponse GetAutherisationPage()
		{
			HttpWebResponse myHttpWebResponse = this.HttpQuery(
					"http://shop.f5.ua",
					false,
					"",
					"");

			CheckResponseCorrect(myHttpWebResponse, "", HttpStatusCode.OK);
			
			return myHttpWebResponse;
		}

		private HttpWebResponse PostAutherisation()
		{			
			HttpWebResponse myHttpWebResponse = this.HttpQuery(
				"http://shop.f5.ua",
				true,
				"login=oleynic&password=18072008y&go=%D0%92%D1%85%D0%BE%D0%B4",
				"application/x-www-form-urlencoded");

			CheckResponseCorrect(myHttpWebResponse, "", HttpStatusCode.Found);
			return myHttpWebResponse;
		}

		private void CheckResponseCorrect(HttpWebResponse myHttpWebResponse, string originalStrInContent, HttpStatusCode status)
		{
			this._strHtmlLast = this.GetHtmlText(myHttpWebResponse);

			if (myHttpWebResponse.StatusCode != status)
				throw new Exception("Url: " + myHttpWebResponse.ResponseUri
					+ "; expected http status is " + status.ToString()
					+ "; current http status is " + myHttpWebResponse.StatusCode.ToString());

			if (originalStrInContent != "")
				if (!this._strHtmlLast.Contains(originalStrInContent))
					throw new Exception("Url: " + myHttpWebResponse.ResponseUri + "; Downloaded page content do not have string like: " + originalStrInContent);
		}

		private string GetHtmlText(HttpWebResponse pHttpWebResponse)
		{
			try
			{
				Encoding enc = null;
				switch (pHttpWebResponse.CharacterSet)
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

				StreamReader myStreamReader = new StreamReader(pHttpWebResponse.GetResponseStream(), enc);
				this._strHtmlLast = myStreamReader.ReadToEnd();
				return this._strHtmlLast;
			}
			catch
			{
				return "";
			}
		}

		private HttpWebResponse HttpQuery(string uri, bool isPost, string postParams, string contentType)
		{
			ServicePointManager.Expect100Continue = false;
			ServicePointManager.SetTcpKeepAlive(true, 6000000, 100000);

			HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(uri);
			

			myHttpWebRequest.Accept = "application/x-ms-application, image/jpeg, application/xaml+xml, image/gif, image/pjpeg, application/x-ms-xbap, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*"; // accept;// "application/x-ms-application, image/jpeg, application/xaml+xml, image/gif, image/pjpeg, application/x-ms-xbap, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
			myHttpWebRequest.Headers.Add("Accept-Language", "ru-RU");
			myHttpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; MRSPUTNIK 2, 3, 0, 289; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET CLR 1.1.4322; .NET4.0C; .NET4.0E; InfoPath.2)";
			myHttpWebRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
			//myHttpWebRequest.Host = "business.moneygram.com";


			if (contentType != "") myHttpWebRequest.ContentType = contentType;

			if (this._cookieCont.Count > 0)
				myHttpWebRequest.CookieContainer = this._cookieCont;//вписываем куки

			myHttpWebRequest.Timeout = 180000;//3 min
			myHttpWebRequest.AllowAutoRedirect = false;// ставим False, чтобы при получении кода 302 не делать автоматический редирект
			if (isPost)
			{
				myHttpWebRequest.Method = "POST";

				//Пишем параметры формы в запрос
				byte[] ByteArr = System.Text.Encoding.GetEncoding(1251).GetBytes(postParams);
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
				this._cookieCont.SetCookies(new Uri("http://shop.f5.ua"), myHttpWebResponse.Headers["Set-Cookie"]);

			return myHttpWebResponse;
		}
	
		private static int GetInt(string str)
		{
			if (str == "") return 0;

			var partStr = str.Split('.')[0];
			var result = int.Parse(partStr);
			return result;
		}
				
		private static ProductLineVendor GetProductLineVendorFromReader(IDataReader reader)
		{
			var newProductLineVendor = new ProductLineVendor();
			
			try
			{
				var artikul = XlsProvider.GetFieldValueFromReader<string>(reader, "Код товара");

				if (artikul.Length != 7) // articul должен быть 7 символов
					return null;

				newProductLineVendor.Articul = artikul;
				newProductLineVendor.Name = XlsProvider.GetFieldValueFromReader<string>(reader, "Название товара");

				newProductLineVendor.PriceRaschet = GetInt(XlsProvider.GetFieldValueFromReader<string>(reader, "Расч# цена"));
				newProductLineVendor.Price = GetInt(XlsProvider.GetFieldValueFromReader<string>(reader, "Цена продажи"));
				newProductLineVendor.PriceBase = GetInt(XlsProvider.GetFieldValueFromReader<string>(reader, "Базовая цена"));
				newProductLineVendor.PriceDiff = GetInt(XlsProvider.GetFieldValueFromReader<string>(reader, "Цена продажи – Базовая цена"));

			}
			catch (Exception ex)
			{
				// errors.Add(worksheet.Cells[currentRow, GetColumnIndex(properties, "Код товара")].Value as string);
				return null;
			}
			return newProductLineVendor;
		}
	}
}
