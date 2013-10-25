namespace Nop.Services.SiteParsers.Categories
{
	using System;
	using System.Collections.Generic;
	using Nop.Core.Domain.YandexMarket;
	using Nop.Services.Logging;
	using Nop.Services.YandexMarket;
	using OpenQA.Selenium;
	using OpenQA.Selenium.IE;


	public abstract class BaseParserCategories
	{		
		public void Init(string urlCategoryForParsing, ILogger logger, IYandexMarketCategoryService yandexMarketCategoryService)
		{			
			this.mLogger = logger;
			
			this.UrlCategoryForParsing = urlCategoryForParsing;			
			this._yandexMarketCategoryService = yandexMarketCategoryService;
		}

		private IYandexMarketCategoryService _yandexMarketCategoryService;		
		protected string UrlCategoryForParsing { get; set; }
		
		protected ILogger mLogger;
		protected IWebDriver mDriver;

		// protected virtual string CssSelectorForProductLinkInProductList { get{throw new Exception("Not implemented");}}				
		// protected virtual string CssSelectorForProductArticulInProductPage { get { return ""; } }

		protected virtual YandexMarketCategoryRecord GetParserCategory()
		{
			throw new Exception("Not implemented");
		}
		

		public YandexMarketCategoryRecord Parse(ref bool isStopProducsImport)
		{
			this.mLogger.Debug("Start Parsing Category ...");

			YandexMarketCategoryRecord resultCategory = null;

			try
			{
				this.mDriver = new InternetExplorerDriver();
				this.mDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(60));

				// —сылка на список товаров
				this.mDriver.Navigate().GoToUrl(this.UrlCategoryForParsing);

				resultCategory = GetParserCategory();
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

			this.mLogger.Debug("End Parsing.");

			return resultCategory;
		}
		
		public static BaseParserCategories Create(string productsPageUrl, ILogger logger, IYandexMarketCategoryService _yandexMarketCategoryService)
		{
			BaseParserCategories parser = null;

			if(productsPageUrl.Contains("yugcontract.ua"))
				parser = new UgContractParserCategory();
			//else if (productsPageUrl.Contains("market.yandex"))
			//	parser = new YandexMarketParser();
			else
				throw new Exception("Can't define parser type for url=" + productsPageUrl);

			parser.Init(productsPageUrl, logger,  _yandexMarketCategoryService);

			return parser;
		}

		private List<YandexMarketCategoryRecord> GetTestData()
		{
			var resultProductList = new List<YandexMarketCategoryRecord>();

			var level_0_1 = new YandexMarketCategoryRecord()
			{
				Id = 1,
				IsActive = true,
				Name = "level_0_1",
				ParentId = 0,
				ShopCategoryId = 0,
				Url = "someUrl",
				Children = new List<YandexMarketCategoryRecord>()
			};
			resultProductList.Add(level_0_1);

			var level_1_1 = new YandexMarketCategoryRecord()
			{
				Id = 2,
				IsActive = true,
				Name = "level_1_1",
				ParentId = 1,
				ShopCategoryId = 0,
				Url = "someUrl",
				Children = new List<YandexMarketCategoryRecord>()
			};



			var level_0_2 = new YandexMarketCategoryRecord()
			{
				Id = 3,
				IsActive = true,
				Name = "level_0_2",
				ParentId = 0,
				ShopCategoryId = 0,
				Url = "someUrl",
				Children = new List<YandexMarketCategoryRecord>()
			};
			resultProductList.Add(level_0_2);


			var level_1_2 = new YandexMarketCategoryRecord()
			{
				Id = 4,
				IsActive = true,
				Name = "level_1_2",
				ParentId = 3,
				ShopCategoryId = 0,
				Url = "someUrl",
				Children = new List<YandexMarketCategoryRecord>()
			};



			level_0_2.Children.Add(level_1_2);
			level_0_1.Children.Add(level_1_1);

			return resultProductList;
		}
	}
}
