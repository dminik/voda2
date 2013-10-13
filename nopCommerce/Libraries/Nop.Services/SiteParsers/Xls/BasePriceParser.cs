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
	using Nop.Services.Catalog;
	using Nop.Services.Logging;
	using Nop.Services.SiteParsers.Xls;
	using Nop.Services.YandexMarket;

	using OfficeOpenXml;

	using OpenQA.Selenium;
	using OpenQA.Selenium.IE;
	using OpenQA.Selenium.Remote;
	using OpenQA.Selenium.Support.UI;

	public class BasePriceParser<T> where T : ProductLine 
	{
		protected IProductService _productService;
		protected ILogger mLogger; 
		private readonly CookieContainer _cookieCont = new CookieContainer();
		string _strHtmlLast = "";

		public BasePriceParser(IProductService productService, ILogger logger)
		{
			mLogger = logger;
			_productService = productService;
		}

		protected IEnumerable<T> ResultList { get; set; }
		
		protected virtual string UrlBase { get { throw new Exception("Not implemented"); } }
		protected virtual string UrlAuthorizationGet { get { throw new Exception("Not implemented"); } }
		protected virtual string UrlAuthorizationPost { get { return UrlAuthorizationGet; } }
		protected virtual string UrlAuthorizationPostParams { get { throw new Exception("Not implemented"); } }
		protected virtual string UrlDownload { get { throw new Exception("Not implemented"); } }
		protected virtual string SheetNameFirst { get { return ""; } }


		protected virtual string XlsFileName { get { throw new Exception("Not implemented"); } }
		protected virtual string PriceFullFileName
		{
			get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProductsCatalog", XlsFileName); }
		}

		protected IEnumerable<T> GetPriceListFromCache(out List<string> errors, bool isUpdateCacheFromInternet)
		{
			errors = new List<string>();
			
			if (isUpdateCacheFromInternet)
				DownloadNewPriceListToCache();

			this.mLogger.Debug("Start " + this.GetType().Name + ".GetPriceListFromCache...");

			try
			{
				this.ResultList = XlsProvider.LoadFromFile<T>(this.PriceFullFileName, GetObjectFromReader, SheetNameFirst);

				this.PostProcessing();
			}
			catch (Exception ex)
			{
				this.mLogger.Debug(ex.Message, ex);
				this.mLogger.Debug("Not success GetPriceListFromCache.");
				throw;
			}

			this.mLogger.Debug("End  " + this.GetType().Name + ".GetPriceListFromCache.");

			var distinctedResult = this.ResultList.GroupBy(x => x.Articul).Select(grp => grp.First());
			return distinctedResult;
		}

		

		protected virtual T GetObjectFromReader(IDataReader reader)
		{
			throw new Exception("Not implemented");
		}

		protected virtual HttpWebResponse AfterAutherisation(HttpWebResponse httpWebResponse)
		{
			return httpWebResponse;
		}

		protected virtual void PostProcessing()
		{
		}

		private void DownloadNewPriceListToCache()
		{
			this.mLogger.Debug("Start " + this.GetType().Name + ".DownloadNewPriceListToCache...");

			try
			{				
				// ����� �� ��������� � ���������
				GetAutherisationPage();
				var response =  PostAutherisation();
				AfterAutherisation(response);
				DownloadFile();
			}
			catch (Exception ex)
			{
				this.mLogger.Debug(ex.Message, ex);
				this.mLogger.Debug("Not success DownloadNewPriceListToCache for " + this.GetType().Name);
				throw;
			}

			this.mLogger.Debug("End " + this.GetType().Name + ". DownloadNewPriceListToCache.");
		}

		private void DownloadFile()
		{
			// ��������� ����
			HttpWebResponse myHttpWebResponse = this.HttpQuery(
				this.UrlDownload,
				true,
				//ispost
				"",
				//params                               
				"application/x-www-form-urlencoded");

			var stream = myHttpWebResponse.GetResponseStream();
			XlsProvider.SaveStreamToFile(this.PriceFullFileName, stream);
		}

		private HttpWebResponse GetAutherisationPage()
		{
			HttpWebResponse myHttpWebResponse = this.HttpQuery(
					UrlAuthorizationGet,
					false,
					"",
					"");

			CheckResponseCorrect(myHttpWebResponse, "", HttpStatusCode.OK);
			
			return myHttpWebResponse;
		}

		private HttpWebResponse PostAutherisation()
		{			
			HttpWebResponse myHttpWebResponse = this.HttpQuery(
				UrlAuthorizationPost,
				true,
				UrlAuthorizationPostParams,
				"application/x-www-form-urlencoded");

			CheckResponseCorrect(myHttpWebResponse, "", HttpStatusCode.Found);
			return myHttpWebResponse;
		}
		
		protected void CheckResponseCorrect(HttpWebResponse myHttpWebResponse, string originalStrInContent, HttpStatusCode status)
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

		protected HttpWebResponse HttpQuery(string uri, bool isPost, string postParams, string contentType)
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
				myHttpWebRequest.CookieContainer = this._cookieCont;//��������� ����

			myHttpWebRequest.Timeout = 180000;//3 min
			myHttpWebRequest.AllowAutoRedirect = false;// ������ False, ����� ��� ��������� ���� 302 �� ������ �������������� ��������
			if (isPost)
			{
				myHttpWebRequest.Method = "POST";

				//����� ��������� ����� � ������
				byte[] ByteArr = System.Text.Encoding.GetEncoding(1251).GetBytes(postParams);
				myHttpWebRequest.ContentLength = ByteArr.Length;
				myHttpWebRequest.GetRequestStream().Write(ByteArr, 0, ByteArr.Length);
				myHttpWebRequest.GetRequestStream().Close();
			}

			myHttpWebRequest.KeepAlive = true;
			
			HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

			//���������� ���������� ����
			if (!String.IsNullOrEmpty(myHttpWebResponse.Headers["Set-Cookie"]))
				this._cookieCont.SetCookies(new Uri(UrlBase), myHttpWebResponse.Headers["Set-Cookie"]);

			return myHttpWebResponse;
		}
	
		protected static int GetInt(string str)
		{
			if (str == "") return 0;

			try
			{
				var partStr = str.Split('.', ',')[0];
				var result = int.Parse(partStr);
				return result;
			}
			catch
			{
				var partStr = str.Split(',')[0];
				var result = int.Parse(partStr);
				return result;
			}			
		}
				
		
	}
}