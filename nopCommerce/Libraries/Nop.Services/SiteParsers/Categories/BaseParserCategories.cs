namespace Nop.Services.SiteParsers.Categories
{
	using System;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;
	using System.Threading;

	using Nop.Core.Domain.YandexMarket;
	using Nop.Services.Logging;
	using Nop.Services.YandexMarket;

	using OpenQA.Selenium;
	using OpenQA.Selenium.IE;
	using OpenQA.Selenium.Remote;

	class ExistedProduct
	{
		public string Url { get; set; }
		public bool IsFantom { get; set; }
	}

	public abstract class BaseParserCategories
	{
		public void Init(string urlCategoryForParsing, ILogger logger, IYandexMarketCategoryService yandexMarketCategoryService)
		{			
			this.mLogger = logger;
			
			this.UrlCategoryForParsing = urlCategoryForParsing;			
			this._yandexMarketCategoryService = yandexMarketCategoryService;
		}

		private IYandexMarketCategoryService _yandexMarketCategoryService;		
		private string UrlCategoryForParsing { get; set; }
		
		protected ILogger mLogger;
		protected IWebDriver mDriver;

		protected virtual string CssSelectorForProductLinkInProductList { get{throw new Exception("Not implemented");}}				
		protected virtual string CssSelectorForProductArticulInProductPage { get { return ""; } }
		
		protected virtual string GetLinkToSpecsTabOfProduct(string productLink)
		{
			return productLink;
		}
		
		public List<YandexMarketCategoryRecord> Parse(ref bool isStopProducsImport)
		{
			this.mLogger.Debug("Start Parsing Category ...");

			var resultProductList = new List<YandexMarketCategoryRecord>();

			try
			{
				this.mDriver = new InternetExplorerDriver();
				this.mDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(60));

				// Ссылка на список товаров
				this.mDriver.Navigate().GoToUrl(this.UrlCategoryForParsing);
				//Thread.Sleep(3000);
								
				//bool isNextPage = false;				
				//int pageLinksCounter = 1;

				//do
				//{
				//	if (isStopProducsImport)
				//		throw new Exception("Stopped by user.");

				//	// Найти все ссылки на товары			
				//	var linksFromCurrentPage =
				//		Enumerable.ToList<string>(
				//			this.mDriver.FindElements(By.CssSelector(CssSelectorForProductLinkInProductList)).Select(s => s.GetAttribute("href")));
					
				//	this.mLogger.Debug("Have page " + pageLinksCounter++ + " with links");

				//	var nextPageUrl = "";
				//	try
				//	{
				//		//Ищем ссылку на следущую страницу, если она есть
				//		if (!this.mDriver.PageSource.Contains(NextLinkInProductListName)) 
				//			throw new Exception();

				//		nextPageUrl = this.mDriver.FindElement(By.CssSelector(CssSelectorForNextLinkInProductList)).GetAttribute("href");
				//		nextPageUrl = nextPageUrl.Replace("pg", "/pg").Replace("//pg", "/pg");

				//		isNextPage = true;						
				//	}
				//	catch (Exception)
				//	{
				//		isNextPage = false;
				//	}

				//	// ===== Парсим все товары с страницы
				//	resultProductList.AddRange(GetProductsByLinks(linksFromCurrentPage));

				//	if (this.ParseNotMoreThen <= resultProductList.Count) break;


				//	if (isNextPage) // Жммем на следущую страницу, если она есть
				//	{ 																	
				//		this.mDriver.Navigate().GoToUrl(nextPageUrl);
				//		//Thread.Sleep(3000);						
				//	}
				//}
				//while (isNextPage);				
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

			return resultProductList;
		}

		

		//private YandexMarketCategoryRecord CreateProduct(string productLink)
		//{			
		//	var product = new YandexMarketCategoryRecord();
		//	product.Url = productLink;

		//	// Переходим на страницу спецификации товара
		//	var pageSpecsUrl = this.GetLinkToSpecsTabOfProduct(productLink);
		//	this.mDriver.Navigate().GoToUrl(pageSpecsUrl);
		//	//Thread.Sleep(3000);

		//	this.mLogger.Debug("Have specs page " + pageSpecsUrl);

		//	product.YandexMarketCategoryRecordId = this.ParserCategoryId;

		//	// Artikul
		//	if (this.CssSelectorForProductArticulInProductPage != string.Empty)
		//		product.Articul = this.FindElement(this.CssSelectorForProductArticulInProductPage).Text.Replace("Код: ", "");

		//		if (CssSelectorForProductFullDescriptionInProductPage != string.Empty)
		//		{
		//			try
		//			{
		//				product.FullDescription =
		//					this.GetInnerHtml(this.mDriver.FindElement(By.CssSelector(CssSelectorForProductFullDescriptionInProductPage)));
		//			}
		//			catch (Exception ex)
		//			{
		//				product.FullDescription = "Нет описания";

		//				if (!ex.Message.Contains("Unable to find element")) 
		//					throw;
		//			}
		//		}

		//		product = ProductPostProcessing(product);				
		//	}
			

		//	this.mLogger.Debug("Saving product...");
		//	this._yandexMarketCategoryService.Insert(product);
		//	this.mLogger.Debug("Saving product Done.");

		//	if(!product.IsNotInPriceList)
		//		return product;
		//	else			
		//		return null;			
		//}

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
	}
}
