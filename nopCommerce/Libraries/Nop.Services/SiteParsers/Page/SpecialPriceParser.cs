namespace Nop.Services.SiteParsers.Page
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Net;
	using System.Threading;

	using Nop.Core.Domain.Security;
	using Nop.Core.Infrastructure;
	using Nop.Services.Logging;

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
				this.mDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(260));
				
#if DEBUG
				resultSpecialPriceList = this.GetParserSpecialPriceList();
#else 
				resultSpecialPriceList = new List<SpecialPrice>();
#endif
			}
			catch (Exception ex)
			{
				this.mLogger.Error(ex.Message, ex);
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
				this.mLogger.Debug("1");
				// ’одим по страницам и логинимс€				
				PostAutherisation();
				this.mLogger.Debug("2");
				resultSpecialPriceList = FindDataOnHtmlPage();
				this.mLogger.Debug("22");
			}
			catch (Exception ex)
			{
				this.mLogger.Error(ex.Message, ex);
				this.mLogger.Error("Not success DownloadNewSpecialPriceListToCache for " + this.GetType().Name);
				throw;
			}

		
			this.mLogger.Debug("End " + this.GetType().Name + ". DownloadNewSpecialPriceListToCache.");



			return resultSpecialPriceList;
		}

		private List<SpecialPrice> FindDataOnHtmlPage()
		{			
			var resultSpecialPriceList = new List<SpecialPrice>();

			// —сылка рекомендованные цены

			Thread.Sleep(70000);
			this.mDriver.Navigate().GoToUrl(this.UrlSpecialPriceForParsing);
			this.mLogger.Debug("25");
		
			
			this.mLogger.Debug("3");
			Thread.Sleep(70000);

			var source =  ParserHelper.GetPageSource(mDriver, 20);

			if(source.Length != 0)
				this.mLogger.Debug(source);

			ReadOnlyCollection<IWebElement> allTables;
			this.mLogger.Debug("35");
			try
			{								
				allTables = ParserHelper.FindElements(mDriver, ".MsoNormalTable", 20);


				Thread.Sleep(70000);
				this.mLogger.Debug("4");
			}
			catch (Exception ex)
			{
				this.mLogger.Debug(source);
				throw;
			}
			

			var index = 0;
			foreach (var curTbl in allTables)
			{
				index++;
				this.mLogger.Debug("index=" + index);
				var dataList = ExtractDataFromTbl(curTbl);
				resultSpecialPriceList.AddRange(dataList);
			}

			this.mLogger.Debug(this.mDriver.PageSource);
			this.mLogger.Debug("5 resultSpecialPriceList.Count=" + resultSpecialPriceList.Count);

			var x = this.mDriver.PageSource;
			this.mLogger.Debug(x);

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

			this.mLogger.Debug("44");
			this.mDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(260));
			mDriver.Navigate().GoToUrl(UrlAuthorizationGet);

			var source = ParserHelper.GetPageSource(mDriver, 20);

			if (source.Length != 0)
				this.mLogger.Debug(source);

			try
			{
				this.mLogger.Debug("45");
				var txtLogin = mDriver.FindElement(By.Name("login"));
				this.mLogger.Debug("46");
				txtLogin.SendKeys(securitySettings.F5SiteLogin);
				this.mLogger.Debug("47");
				this.mDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(260));
				var txtPass = mDriver.FindElement(By.Name("password"));
				this.mLogger.Debug("48");
				this.mDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(260));
				txtPass.SendKeys(securitySettings.F5SitePassword);
				this.mLogger.Debug("49");

				
				txtPass.Submit();

				Thread.Sleep(5000);
				
			}
			catch (Exception ex)
			{
				this.mLogger.Debug("499");
				this.mLogger.Debug("51 mDriver=" + source);
				throw;
			}

			this.mLogger.Debug("50");
		}
	}
}
