namespace Nop.Services.SiteParsers
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Threading;
	using System.Web;

	using Nop.Core.Domain.YandexMarket;
	using Nop.Services.Logging;

	using OpenQA.Selenium;
	using OpenQA.Selenium.IE;
	using OpenQA.Selenium.Remote;
	using OpenQA.Selenium.Support.UI;

	public abstract class BaseParser
	{
		public void Init(string catalogName, int parserCategoryId, int parseNotMoreThen, string productsPageUrl, ILogger logger)
		{
			this.mImageFolderPathForProductList = catalogName;			
			this.ParseNotMoreThen = parseNotMoreThen;
			this.mLogger = logger;
			ParserCategoryId = parserCategoryId;
			ProductsPageUrl = productsPageUrl;
		}

		private int ParseNotMoreThen { get; set; }
		private int ParserCategoryId { get; set; }
		private string ProductsPageUrl { get; set; }
		
		// Папка для картинок
		private readonly string mImageFolderPathBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProductsCatalog");
		private string mImageFolderPathForProductList;

		protected ILogger mLogger;
		protected IWebDriver mDriver;

		protected virtual string CssSelectorForProductLinkInProductList { get{throw new Exception("Not implemented");}}
		protected virtual string CssSelectorForNextLinkInProductList { get { throw new Exception("Not implemented"); } }
		protected virtual string CssSelectorForProductNameInProductPage { get { throw new Exception("Not implemented"); } }
		protected virtual string CssSelectorForProductPictureInProductPage { get { throw new Exception("Not implemented"); } }
		protected virtual string CssSelectorForProductSpecsRowsInProductPage { get { throw new Exception("Not implemented"); } }
		protected virtual string CssSelectorForProductSpecKeyInProductPage { get { throw new Exception("Not implemented"); } }
		protected virtual string CssSelectorForProductSpecValInProductPage { get { throw new Exception("Not implemented"); } }
		protected virtual string CssSelectorForProductArticulInProductPage { get { return ""; } }
		protected virtual string CssSelectorForProductFullDescriptionInProductPage { get { return ""; } }
		

		protected virtual string GetLinkToSpecsTabOfProduct(string productLink)
		{
			return productLink;
		}
		
		public List<YandexMarketProductRecord> Parse()
		{
			this.mLogger.Debug("Start Parsing...");

			var resultProductList = new List<YandexMarketProductRecord>();

			try
			{
				mDriver = new InternetExplorerDriver();
				mDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(60));

				// mDriver = new FirefoxDriver();
				//var binary = new FirefoxBinary("c:\\Program Files (x86)\\Mozilla Firefox\\firefox.exe");
				//var profile = new FirefoxProfile();
				//profile.Port = 7056;  
				//mDriver = new FirefoxDriver(profile);
				
				// Ссылка на список товаров
				this.mDriver.Navigate().GoToUrl(ProductsPageUrl);
				Thread.Sleep(3000);

				this.mLogger.Debug("Have page 1 with liks");

				const int delayInSeconds = 4;
				bool isNextPage = false;
				var productLinks = new List<string>();
				int pageLinksCounter = 1;

				do
				{
					// Найти все ссылки на товары			
					var linksFromCurrentPage =
						Enumerable.ToList<string>(
							this.mDriver.FindElements(By.CssSelector(CssSelectorForProductLinkInProductList)).Select(s => s.GetAttribute("href")));
					productLinks.AddRange(linksFromCurrentPage);

					if (this.ParseNotMoreThen <= 10) break;

					this.mLogger.Debug("Have page " + pageLinksCounter + " with links");

					try
					{
						// Жммем на следущую страницу, если она есть
						var nextPage = this.mDriver.FindElement(By.CssSelector(CssSelectorForNextLinkInProductList));

						nextPage.Click();
						isNextPage = true;
						pageLinksCounter++;
					}
					catch (NoSuchElementException)
					{
						isNextPage = false;
					}
				}
				while (isNextPage);

				this.mLogger.Debug("Have " + productLinks.Count + " links on the page");

				int productCounter = 1;
				foreach (var currentProductLink in productLinks)
				{
					this.mLogger.Debug("Proceeding product " + productCounter);

					var product = this.CreateProduct(currentProductLink);

					resultProductList.Add(product);

					productCounter++;

					if (productCounter > this.ParseNotMoreThen) break;

					Thread.Sleep(delayInSeconds * 1000);
				}
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
		
		private YandexMarketProductRecord CreateProduct(string productLink)
		{
			var product = new YandexMarketProductRecord();
			
			// Переходим на страницу спецификации товара
			var pageSpecsUrl = GetLinkToSpecsTabOfProduct(productLink);
			this.mDriver.Navigate().GoToUrl(pageSpecsUrl);
			Thread.Sleep(3000);

			this.mLogger.Debug("Have specs page " + pageSpecsUrl);
			
			product.YandexMarketCategoryRecordId = ParserCategoryId;

			// Найти имя товара		
			product.Name = this.mDriver.FindElement(By.CssSelector(CssSelectorForProductNameInProductPage)).Text;

			// Скачиваем картинку
			var imageUrl = this.mDriver.FindElement(By.CssSelector(CssSelectorForProductPictureInProductPage)).GetAttribute("src");
			Thread.Sleep(3000);
			// do it twice because of some bag
			imageUrl = this.mDriver.FindElement(By.CssSelector(CssSelectorForProductPictureInProductPage)).GetAttribute("src");
			product.ImageUrl_1 = this.SaveImage(imageUrl, product.Name);

			// Найти спецификации товара	table.b-properties tr   
			var specificationElements = this.mDriver.FindElements(By.CssSelector(CssSelectorForProductSpecsRowsInProductPage));

			this.mLogger.Debug("Have product " + product.Name + ". Start getting " + specificationElements.Count + " specs...");

			foreach (var currentSpecificationElement in specificationElements)
			{
				// Key
				IWebElement currentKeyElement;
				try
				{					
					currentKeyElement = currentSpecificationElement.FindElement(By.CssSelector(CssSelectorForProductSpecKeyInProductPage));
					while (currentKeyElement.Text == "")
					{
						var x = ((OpenQA.Selenium.Remote.RemoteWebElement)(currentKeyElement)).LocationOnScreenOnceScrolledIntoView.X;
						if(x==9999998)
							continue;
						Thread.Sleep(1000);
						currentKeyElement = currentSpecificationElement.FindElement(By.CssSelector(CssSelectorForProductSpecKeyInProductPage));
					}

				}
				catch (NoSuchElementException)
				{
					this.mLogger.Debug("not key");
					continue;
				}

				// Value				
				var currentValueElement = currentSpecificationElement.FindElement(By.CssSelector(CssSelectorForProductSpecValInProductPage));


				
				//WebDriverWait wait = new WebDriverWait(mDriver, 10);
				//IWebElement element = wait.Until(ExpectedConditions.ElementExists(By.CssSelector(""), ));

				product.Specifications.Add(new YandexMarketSpecRecord(currentKeyElement.Text, currentValueElement.Text));				
			}

			this.mLogger.Debug("Finished getting specs.");

			if (CssSelectorForProductArticulInProductPage != string.Empty)
				product.Articul = this.mDriver.FindElement(By.CssSelector(CssSelectorForProductArticulInProductPage)).Text.Replace("Код: ", "");

			// Переходим на страницу описания товара			
			this.mDriver.Navigate().GoToUrl(productLink);
			Thread.Sleep(3000);

			this.mLogger.Debug("Have main product page " + productLink);

			if (CssSelectorForProductFullDescriptionInProductPage != string.Empty)
			{
				product.FullDescription =
					this.mDriver.FindElement(By.CssSelector(CssSelectorForProductFullDescriptionInProductPage)).Text;

				product.FullDescription =
					GetInnerHtml(this.mDriver.FindElement(By.CssSelector(CssSelectorForProductFullDescriptionInProductPage)));

				const string toErase1 = "Если вы заметили некорректные данные в описании товара, выделите ошибку и нажмите";
				const string toErase2 = "Ctrl+Enter";
				const string toErase3 = ", чтобы сообщить нам об этом.";
				product.FullDescription = product.FullDescription.Replace(toErase1, "").Replace(toErase2, "").Replace(toErase3, "");
			}

			return product;
		}

		private string GetInnerHtml(IWebElement element)
		{
			var remoteWebDriver = (RemoteWebElement)element;
			var javaScriptExecutor = (IJavaScriptExecutor)remoteWebDriver.WrappedDriver;
			var innerHtml = javaScriptExecutor.ExecuteScript("return arguments[0].innerHTML;", element).ToString();

			return innerHtml;
		}

		private string SaveImage(string remoteFileUrl, string productName)
		{
			string fileName = productName.Replace('/', '-').Replace('\\', '-').Replace('*', '-').Replace('?', '-').Replace('"', '-').Replace('\'', '-').Replace('<', '-').Replace('>', '-').Replace('|', '-').Replace(' ', '_');
			fileName += Path.GetExtension(remoteFileUrl);

			var folderPath = Path.Combine(this.mImageFolderPathBase, this.mImageFolderPathForProductList);
			var imageFolderPath = this.MakeFolder(folderPath);

			var fullFileName = Path.Combine(imageFolderPath, fileName);

			var webClient = new WebClient();

			this.mLogger.Debug("Image will be saved to " + fullFileName + "...");
			webClient.DownloadFile(remoteFileUrl, fullFileName);
			this.mLogger.Debug("Image saved");

			return Path.Combine(this.mImageFolderPathForProductList, fileName);
		}

		private string MakeFolder(string path)
		{
			var newFolder = new System.IO.DirectoryInfo(path);
			if (!newFolder.Exists)
			{
				newFolder.Create();				
			}

			return path;
		}

		public static BaseParser Create(string catalogName, int parserCategoryId, int parseNotMoreThen, string productsPageUrl, ILogger logger)
		{
			BaseParser parser = null;

			if(productsPageUrl.Contains("yugcontract.ua"))
				parser = new UgContractParser();
			else if (productsPageUrl.Contains("market.yandex"))
				parser = new YandexMarketParser();
			else
				throw new Exception("Can't define parser type for url=" + productsPageUrl);

			parser.Init(catalogName, parserCategoryId, parseNotMoreThen, productsPageUrl, logger);

			return parser;
		}
	}
}
