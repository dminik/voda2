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

	// Requires reference to WebDriver.Support.dll
	// using OpenQA.Selenium.Support.UI;

	public class Parser
	{
		public Parser(string catalogName, int categoryId, int parseNotMoreThen, string productsPageUrl, ILogger logger)
		{
			this.imageFolderPathForProductList = catalogName;
			string filePath = Path.Combine(HttpRuntime.AppDomainAppPath, "content\\files\\exportimport",  "StaticFileName");
			this.imageFolderPathBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProductsCatalog");
			this.ParseNotMoreThen = parseNotMoreThen;
			_logger = logger;
			CategoryId = categoryId;
			ProductsPageUrl = productsPageUrl;
		}

		int ParseNotMoreThen { get; set; }
		int CategoryId { get; set; }
		string ProductsPageUrl { get; set; }
		
		// Папка для картинок
		string imageFolderPathBase;
		string imageFolderPathForProductList;

		private ILogger _logger;

		IWebDriver mDriver;

		public List<YandexMarketProductRecord> Parse()
		{
			_logger.Debug("Start Parsing...");

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

				_logger.Debug("Have page 1 with liks");

				const int delayInSeconds = 4;

				bool isNextPage = false;

				var productLinks = new List<string>();

				int pageLinksCounter = 1;

				do
				{
					// Найти все ссылки на товары			
					var linksFromCurrentPage =
						Enumerable.ToList<string>(
							this.mDriver.FindElements(By.CssSelector("a.b-offers__name")).Select(s => s.GetAttribute("href")));
					productLinks.AddRange(linksFromCurrentPage);

					if (this.ParseNotMoreThen <= 10) break;

					_logger.Debug("Have page " + pageLinksCounter + " with links");

					try
					{
						// Жммем на следущую страницу, если она есть
						var nextPage = this.mDriver.FindElement(By.CssSelector("a.b-pager__next"));

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

				_logger.Debug("Have " + productLinks.Count + " liks");

				int productCounter = 1;
				foreach (var currentProductLink in productLinks)
				{
					_logger.Debug("Proceeding product " + productCounter);

					var product = this.CreateProduct(currentProductLink);

					resultProductList.Add(product);

					productCounter++;

					if (productCounter > this.ParseNotMoreThen) break;

					Thread.Sleep(delayInSeconds * 1000);
				}
			}
			catch (Exception ex)
			{
				_logger.Debug(ex.Message, ex);
				throw;
			}
			finally
			{				
				this.mDriver.Quit();
			}

			_logger.Debug("End Parsing.");

			return resultProductList;
		}

		private YandexMarketProductRecord CreateProduct(string productLink)
		{
			// Переходим на страницу спецификации товара
			var pageUrl = productLink.Replace("model.xml", "model-spec.xml");
			this.mDriver.Navigate().GoToUrl(pageUrl);
			Thread.Sleep(3000);

			_logger.Debug("Have page " + pageUrl);
			var product = new YandexMarketProductRecord();
			product.YandexMarketCategoryRecordId = CategoryId;

			// Найти имя товара		
			product.Name = this.mDriver.FindElement(By.CssSelector("h1.b-page-title")).Text;

			// Скачиваем картинку
			var imageUrl = this.mDriver.FindElement(By.CssSelector("div.b-model-microcard__img img")).GetAttribute("src");
			Thread.Sleep(3000);
			imageUrl = this.mDriver.FindElement(By.CssSelector("div.b-model-microcard__img img")).GetAttribute("src");
			product.ImageUrl_1 = this.SaveImage(imageUrl, product.Name);

			// Найти спецификации товара	table.b-properties tr   
			var specificationElements = this.mDriver.FindElements(By.CssSelector("table.b-properties tr:has(th.b-properties__label)"));

			_logger.Debug("Have product " + product.Name + ". Start getting " + specificationElements.Count + " specs...");

			foreach (var currentSpecificationElement in specificationElements)
			{
				// Key
				IWebElement currentKeyElement;
				try
				{					
					currentKeyElement = currentSpecificationElement.FindElement(By.CssSelector(".b-properties__label span"));					
				}
				catch (NoSuchElementException)
				{
					_logger.Debug("not key");
					continue;
				}

				// Value				
				var currentValueElement = currentSpecificationElement.FindElement(By.CssSelector(".b-properties__value"));
				
				product.Specifications.Add(new YandexMarketSpecRecord(currentKeyElement.Text, currentValueElement.Text));				
			}

			_logger.Debug("Finished getting specs.");

			return product;
		}

		private string SaveImage(string remoteFileUrl, string productName)
		{
			string fileName = productName.Replace('/', '-').Replace('\\', '-').Replace('*', '-').Replace('?', '-').Replace('"', '-').Replace('\'', '-').Replace('<', '-').Replace('>', '-').Replace('|', '-');
			fileName += Path.GetExtension(remoteFileUrl);

			var folderPath = Path.Combine(this.imageFolderPathBase, this.imageFolderPathForProductList);
			var imageFolderPath = this.MakeFolder(folderPath);

			var fullFileName = Path.Combine(imageFolderPath, fileName);

			var webClient = new WebClient();

			_logger.Debug("Image will be saved to " + fullFileName + "...");
			webClient.DownloadFile(remoteFileUrl, fullFileName);

			_logger.Debug("Image saved");

			return Path.Combine(this.imageFolderPathForProductList, fileName);
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
