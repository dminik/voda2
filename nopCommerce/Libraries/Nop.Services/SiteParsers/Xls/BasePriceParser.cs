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
	using Nop.Core.Caching;
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
		protected readonly ICacheManager _cacheManager;
		protected IProductService _productService;
		protected ILogger mLogger; 
		private readonly CookieContainer _cookieCont = new CookieContainer();		
		private const string PRICE_LIST_BY_TYPE_KEY = "Nop.prycelist.type-{0}";

		public BasePriceParser(IProductService productService, ILogger logger, ICacheManager cacheManager)
		{
			_cacheManager = cacheManager;
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

			string key = string.Format(PRICE_LIST_BY_TYPE_KEY, this.GetType().Name);

			if (isUpdateCacheFromInternet)
			{
				if(DownloadNewPriceListToCache())
					_cacheManager.Remove(key);
			}

			var result = _cacheManager.Get(key, -1, () => { 
				
				this.mLogger.Debug("Start " + this.GetType().Name + ".GetPriceListFromFileCache...");

				try
				{
					this.ResultList = XlsProvider.LoadFromFile<T>(this.PriceFullFileName, GetObjectFromReader, SheetNameFirst);

					this.PostProcessing();
				}
				catch (Exception ex)
				{
					this.mLogger.Error(ex.Message, ex);
					this.mLogger.Error("Not success GetPriceListFromCache.");
					throw;
				}

				this.mLogger.Debug("End  " + this.GetType().Name + ".GetPriceListFromFileCache.");

				var distinctedResult = this.ResultList.GroupBy(x => x.Articul).Select(grp => grp.First());
				return distinctedResult;
			
			});

			return result;
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

		private bool DownloadNewPriceListToCache()
		{
			var result = false;
			this.mLogger.Debug("Start " + this.GetType().Name + ".DownloadNewPriceListToCache...");

			
			

			try
			{				
				// Ходим по страницам и логинимся
				GetAutherisationPage();
				var response =  PostAutherisation();
				AfterAutherisation(response);
				DownloadFile();

				string key = string.Format(PRICE_LIST_BY_TYPE_KEY, this.GetType().Name);
				_cacheManager.Remove(key);

				result = true;
			}
			catch (Exception ex)
			{
				this.mLogger.Error(ex.Message, ex);
				this.mLogger.Error("Not success DownloadNewPriceListToCache for " + this.GetType().Name);
				result = false;
			}

			this.mLogger.Debug("End " + this.GetType().Name + ". DownloadNewPriceListToCache.");

			return result;
		}

		private void DownloadFile()
		{
			// Скачиваем файл
			HttpWebResponse myHttpWebResponse = ParserHelper.HttpQuery(
				this.UrlDownload,
				true,
				//ispost
				"",
				//params                               
				"application/x-www-form-urlencoded", _cookieCont, UrlBase);

			var stream = myHttpWebResponse.GetResponseStream();
			XlsProvider.SaveStreamToFile(this.PriceFullFileName, stream);
		}

		private HttpWebResponse GetAutherisationPage()
		{
			HttpWebResponse myHttpWebResponse = ParserHelper.HttpQuery(
					UrlAuthorizationGet,
					false,
					"",
					"", _cookieCont, UrlBase);

			ParserHelper.CheckResponseCorrect(myHttpWebResponse, "", HttpStatusCode.OK);
			
			return myHttpWebResponse;
		}

		private HttpWebResponse PostAutherisation()
		{			
			HttpWebResponse myHttpWebResponse = ParserHelper.HttpQuery(
				UrlAuthorizationPost,
				true,
				UrlAuthorizationPostParams,
				"application/x-www-form-urlencoded", _cookieCont, UrlBase);

			ParserHelper.CheckResponseCorrect(myHttpWebResponse, "", HttpStatusCode.Found);
			return myHttpWebResponse;
		}
		
		
		
	}
}
