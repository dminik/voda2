namespace Nop.Services.SiteParsers.Categories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;
	using System.Threading;

	using Newtonsoft.Json;

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
		

		class XXX
		{
		//	"id":"181",
		//"level":0,
		//"pid":"0",
		//"expanded":false,
		//"have_children":true,
		//"name":"\u041a\u043e\u043c\u043f\u044c\u044e\u0442\u0435\u0440\u043d\u0430\u044f \u0442\u0435\u0445\u043d\u0438\u043a\u0430",
		//"active":false,
		//"is_first":false,
		//"show":true,
		//"url":"kompyuternaya-tehnika-portal"

			public string id;
			public int level;
			public string pid;
			public bool expanded;
			public bool have_children;
			public string name;
			public bool active;
			public bool is_first;
			public bool show;
			public string url;

		}

		private YandexMarketCategoryRecord ProcessCategory(XXX category, XXX[] categoriesArray)
		{
			var newCategory = new YandexMarketCategoryRecord()
			{
				IsActive = true,
				Name = category.name,
				ParentId = int.Parse(category.pid),
				Url = category.url,
				Id = int.Parse(category.id),
				Children = new List<YandexMarketCategoryRecord>(),
			};

			if (category.have_children)
			{
				var children = categoriesArray.Where(x => x.pid == category.id);
				foreach (var child in children)
				{
					var newChild = ProcessCategory(child, categoriesArray);
					newCategory.Children.Add(newChild);
				}
			}
				

			return newCategory;
		}

		public List<YandexMarketCategoryRecord> Parse(ref bool isStopProducsImport)
		{
			this.mLogger.Debug("Start Parsing Category ...");

			var resultCategoriesHierarchy = new List<YandexMarketCategoryRecord>();

			try
			{
				this.mDriver = new InternetExplorerDriver();
				this.mDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(60));

				// Ссылка на список товаров
				this.mDriver.Navigate().GoToUrl(this.UrlCategoryForParsing);
				//Thread.Sleep(3000);

				
					// Найти все ссылки на товары	
				//CssSelectorForProductLinkInProductList = ".cat-menu .level1 a.have-children em";
				//var allcollapsedMenus = mDriver.FindElements(By.CssSelector(CssSelectorForProductLinkInProductList)));

				string source = mDriver.PageSource;
				var indexStart = source.IndexOf("var CATEGORIES = [", System.StringComparison.Ordinal) + "var CATEGORIES = [".Length - 1;
				var indexEnd = source.IndexOf("}];", indexStart, System.StringComparison.Ordinal);

				var categoriesText = source.Substring(indexStart, indexEnd - indexStart + 2);

				var categoriesArray = JsonConvert.DeserializeObject<XXX[]>(categoriesText);

				
				// всем елементам меню верхнего уровня
				foreach (var currentCategory in categoriesArray.Where(x => x.level == 0))
				{
					var category = ProcessCategory(currentCategory, categoriesArray);

					resultCategoriesHierarchy.Add(category);
				}

				//foreach (var allcollapsedMenu in allcollapsedMenus)
				//{
				//	allcollapsedMenu.Click();
				//}

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

			return resultCategoriesHierarchy;
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
