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
				mDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(15));

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
			// Переходим на страницу спецификации товара
			var pageUrl = GetLinkToSpecsTabOfProduct(productLink);
			this.mDriver.Navigate().GoToUrl(pageUrl);
			Thread.Sleep(3000);

			this.mLogger.Debug("Have page " + pageUrl);
			var product = new YandexMarketProductRecord();
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
				}
				catch (NoSuchElementException)
				{
					this.mLogger.Debug("not key");
					continue;
				}

				// Value				
				var currentValueElement = currentSpecificationElement.FindElement(By.CssSelector(CssSelectorForProductSpecValInProductPage));
				
				product.Specifications.Add(new YandexMarketSpecRecord(currentKeyElement.Text, currentValueElement.Text));				
			}

			this.mLogger.Debug("Finished getting specs.");

			return product;
		}

		private string SaveImage(string remoteFileUrl, string productName)
		{
			string fileName = productName.Replace('/', '-').Replace('\\', '-').Replace('*', '-').Replace('?', '-').Replace('"', '-').Replace('\'', '-').Replace('<', '-').Replace('>', '-').Replace('|', '-');
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
	}
}
