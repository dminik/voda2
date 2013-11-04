namespace Nop.Services.SiteParsers.Page
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;

	using Nop.Core.Domain.Security;
	using Nop.Core.Domain.YandexMarket;
	using Nop.Core.Infrastructure;
	using Nop.Services.Logging;
	using Nop.Services.SiteParsers.Categories;
	using Nop.Services.YandexMarket;

	using OpenQA.Selenium;
	using OpenQA.Selenium.IE;

	public class SpecialPrice
	{
		public string ProductName { get; set; }
		public decimal ProductPrice { get; set; }

	}

	public class SpecialPriceParser
	{
		public SpecialPriceParser()
		{						
			this.mLogger = EngineContext.Current.Resolve<ILogger>();	
		}

		private readonly CookieContainer _cookieCont = new CookieContainer();

		protected virtual string UrlSpecialPriceForParsing 
		{ 
			get
			{
				return "http://yugpartner.com.ua/b2b/recommended-retail-prices";
			}
		}

		protected virtual string UrlBase { get { return "http://yugpartner.com.ua"; } }
		protected virtual string UrlAuthorizationGet { get { return UrlBase + "/auth/"; } }
		protected virtual string UrlAuthorizationPost { get { return UrlAuthorizationGet; } }
		protected virtual string UrlAuthorizationPostParams
		{
			get
			{
				var securitySettings = EngineContext.Current.Resolve<SecuritySettings>();

				return "login=" + securitySettings.F5SiteLogin
					+ "&password=" + securitySettings.F5SitePassword;
			}
		}

		protected ILogger mLogger;
		protected IWebDriver mDriver;
		
		public IEnumerable<SpecialPrice> Parse()
		{
			this.mLogger.Debug("Start HTML Parsing SpecialPrice ...");

			IEnumerable<SpecialPrice> resultSpecialPriceList = null;

			try
			{
				this.mDriver = new InternetExplorerDriver();
				this.mDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(60));
				
				resultSpecialPriceList = this.GetParserSpecialPriceList();
			}
			catch (Exception ex)
			{
				this.mLogger.Debug(ex.Message, ex);
				throw;
			}
			finally
			{				
				this.mDriver.Quit();
			}

			this.mLogger.Debug("End HTML Parsing.");

			return resultSpecialPriceList;
		}
		
		private IEnumerable<SpecialPrice> GetParserSpecialPriceList()
		{
			var resultSpecialPriceList = new List<SpecialPrice>();

			this.mLogger.Debug("Start " + this.GetType().Name + ".DownloadNewSpecialPriceListToCache...");

			try
			{
				// ’одим по страницам и логинимс€				
				PostAutherisation();

				resultSpecialPriceList = FindDataOnHtmlPage();
			}
			catch (Exception ex)
			{
				this.mLogger.Debug(ex.Message, ex);
				this.mLogger.Debug("Not success DownloadNewSpecialPriceListToCache for " + this.GetType().Name);
			}

			this.mLogger.Debug("End " + this.GetType().Name + ". DownloadNewSpecialPriceListToCache.");



			return resultSpecialPriceList;
		}

		private List<SpecialPrice> FindDataOnHtmlPage()
		{			
			var resultSpecialPriceList = new List<SpecialPrice>();

			// —сылка рекомендованные цены
			this.mDriver.Navigate().GoToUrl(this.UrlSpecialPriceForParsing);

			var allTables = mDriver.FindElements(By.CssSelector(".MsoNormalTable"));

			foreach (var curTbl in allTables)
			{
				var dataList = ExtractDataFromTbl(curTbl);
				resultSpecialPriceList.AddRange(dataList);
			}

			return resultSpecialPriceList;
		}

		private IEnumerable<SpecialPrice> ExtractDataFromTbl(IWebElement curTbl)
		{
			var resultSpecialPriceList = new List<SpecialPrice>();

			var lines = curTbl.FindElements(By.CssSelector("tr"));

			if(lines.Count != 2)
				throw new Exception("tbl do not have two lines.");

			var lineHeader = lines[0];
			var lineData = lines[1];

			var lineHeaderColumns = lineHeader.FindElements(By.CssSelector("td"));
			var lineDataColumns = lineData.FindElements(By.CssSelector("td"));

			for (var i = 0; i<lineDataColumns.Count; i++)
			{
				var curLineHeaderColumnValue = lineHeaderColumns[i].Text;
				var curLineDataColumnValue = lineDataColumns[i].Text.Replace("грн", "").Replace(".", "").Trim();
				var dPrice = ParserHelper.GetDecimal(curLineDataColumnValue);

				resultSpecialPriceList.Add(new SpecialPrice() { ProductName = curLineHeaderColumnValue, ProductPrice = dPrice });
			}

			return resultSpecialPriceList;
		}
		
		private void PostAutherisation()
		{
			var securitySettings = EngineContext.Current.Resolve<SecuritySettings>();

			mDriver.Navigate().GoToUrl(UrlAuthorizationGet);
			
			var txtLogin = mDriver.FindElement(By.Name("login"));
			txtLogin.SendKeys(securitySettings.F5SiteLogin);

			var txtPass = mDriver.FindElement(By.Name("password"));
			txtPass.SendKeys(securitySettings.F5SitePassword);
			
			txtPass.Submit();
		}
	}
}
