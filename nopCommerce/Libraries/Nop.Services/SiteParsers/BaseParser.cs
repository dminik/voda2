namespace Nop.Services.SiteParsers
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Text.RegularExpressions;
	using System.Threading;
	using System.Web;

	using Nop.Core.Domain.YandexMarket;
	using Nop.Services.FileParsers;
	using Nop.Services.Logging;
	using Nop.Services.YandexMarket;

	using OpenQA.Selenium;
	using OpenQA.Selenium.IE;
	using OpenQA.Selenium.Remote;
	using OpenQA.Selenium.Support.UI;

	public abstract class BaseParser
	{
		public void Init(string catalogName, int parserCategoryId, int parseNotMoreThen, string productsPageUrl, ILogger logger, IYandexMarketProductService yandexMarketProductService)
		{
			this.mImageFolderPathForProductList = catalogName;			
			this.ParseNotMoreThen = parseNotMoreThen;
			this.mLogger = logger;
			ParserCategoryId = parserCategoryId;
			ProductsPageUrl = productsPageUrl;			
			_yandexMarketProductService = yandexMarketProductService;

			
			ProductsArtikulsInPiceList = GetProductsArtikulsInPiceList();

			// Удаляем фантомные продукты из базы, если они появились в прайсе
			DeleteFromDbAppearedInPriceListFantoms(ParserCategoryId, ProductsArtikulsInPiceList);

			ExistedProductUrlList = _yandexMarketProductService.GetByCategory(parserCategoryId, withFantoms: true).Select(s => s.Url).ToList();
		}

		

		private IYandexMarketProductService _yandexMarketProductService;
		private int ParseNotMoreThen { get; set; }
		private int ParserCategoryId { get; set; }
		private string ProductsPageUrl { get; set; }
		private List<string> ExistedProductUrlList { get; set; }
		private List<string> ProductsArtikulsInPiceList { get; set; }

		private int ParsedProductsAmount = 1;
		
		// Папка для картинок
		private readonly string mImageFolderPathBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProductsCatalog");
		private string mImageFolderPathForProductList;

		protected ILogger mLogger;
		protected IWebDriver mDriver;

		protected virtual string CssSelectorForProductLinkInProductList { get{throw new Exception("Not implemented");}}
		protected virtual string CssSelectorForNextLinkInProductList { get { throw new Exception("Not implemented"); } }
		protected virtual string CssSelectorForProductNameInProductPage { get { throw new Exception("Not implemented"); } }
		protected virtual string CssSelectorForProductPictureInProductPage { get { throw new Exception("Not implemented"); } }
		protected virtual string CssSelectorForProductPicturesInProductPage { get { return ""; } }
		protected virtual string CssSelectorForProductSpecsRowsInProductPage { get { throw new Exception("Not implemented"); } }
		protected virtual string CssSelectorForProductSpecKeyInProductPage { get { throw new Exception("Not implemented"); } }
		protected virtual string CssSelectorForProductSpecValInProductPage { get { throw new Exception("Not implemented"); } }
		protected virtual string CssSelectorForProductArticulInProductPage { get { return ""; } }
		protected virtual string CssSelectorForProductFullDescriptionInProductPage { get { return ""; } }
		

		protected virtual string GetLinkToSpecsTabOfProduct(string productLink)
		{
			return productLink;
		}

		protected virtual YandexMarketProductRecord ProductPostProcessing(YandexMarketProductRecord product)
		{
			return product;
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
				//Thread.Sleep(3000);
								
				bool isNextPage = false;				
				int pageLinksCounter = 1;

				do
				{
					// Найти все ссылки на товары			
					var linksFromCurrentPage =
						Enumerable.ToList<string>(
							this.mDriver.FindElements(By.CssSelector(CssSelectorForProductLinkInProductList)).Select(s => s.GetAttribute("href")));
					
					this.mLogger.Debug("Have page " + pageLinksCounter++ + " with links");

					var nextPageUrl = "";
					try
					{
						//Ищем ссылку на следущую страницу, если она есть
						nextPageUrl = this.mDriver.FindElement(By.CssSelector(CssSelectorForNextLinkInProductList)).GetAttribute("href");
						nextPageUrl = nextPageUrl.Replace("pg", "/pg").Replace("//pg", "/pg");

						isNextPage = true;						
					}
					catch (NoSuchElementException)
					{
						isNextPage = false;
					}

					// ===== Парсим все товары с страницы
					resultProductList.AddRange(GetProductsByLinks(linksFromCurrentPage));

					if (this.ParseNotMoreThen <= resultProductList.Count) break;


					if (isNextPage) // Жммем на следущую страницу, если она есть
					{ 																	
						this.mDriver.Navigate().GoToUrl(nextPageUrl);
						//Thread.Sleep(3000);						
					}
				}
				while (isNextPage);				
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

		private IEnumerable<YandexMarketProductRecord> GetProductsByLinks(IEnumerable<string> productLinks)
		{
			var resultProductList = new List<YandexMarketProductRecord>();			
			
			int existedProductCounter = 0;
			
			foreach (var currentProductLink in productLinks)
			{
				// существующий в базе продукт игнорируем
				if (this.ExistedProductUrlList.Contains(currentProductLink))
				{
					existedProductCounter++;
					continue;
				}

				if (existedProductCounter != 0)
				{
					this.mLogger.Debug("Was skipped existed products: " + existedProductCounter);
					existedProductCounter = 0;
				}

				this.mLogger.Debug("Proceeding product " + ParsedProductsAmount);

				var product = this.CreateProduct(currentProductLink);

				// несуществующий в прайсе созданный фантом дальше игнорируем
				if (product == null)									
					continue;				

				resultProductList.Add(product);

				ParsedProductsAmount++;

				if (ParsedProductsAmount > this.ParseNotMoreThen)
				{
					this.mLogger.Debug("Stop parsing because was parsed  " + this.ParseNotMoreThen);
					break;
				}

				//Thread.Sleep(2 * 1000);
			} // end for products

			return resultProductList;
		}

		private YandexMarketProductRecord CreateProduct(string productLink)
		{
			var product = new YandexMarketProductRecord();
			product.Url = productLink;

			// Переходим на страницу спецификации товара
			var pageSpecsUrl = GetLinkToSpecsTabOfProduct(productLink);
			this.mDriver.Navigate().GoToUrl(pageSpecsUrl);
			//Thread.Sleep(3000);

			this.mLogger.Debug("Have specs page " + pageSpecsUrl);

			product.YandexMarketCategoryRecordId = ParserCategoryId;

			// Artikul
			if (CssSelectorForProductArticulInProductPage != string.Empty)
				product.Articul = this.mDriver.FindElement(By.CssSelector(CssSelectorForProductArticulInProductPage)).Text.Replace("Код: ", "");
			
			// Если продукта в прайсе нет, то создаем полупустой продукт с url и артикулом, что бы не заходить в него следующий раз
			product.IsNotInPriceList = !ProductsArtikulsInPiceList.Contains(product.Articul);
						
			if (!product.IsNotInPriceList)
			{				
				// Найти имя товара		
				product.Name = this.mDriver.FindElement(By.CssSelector(CssSelectorForProductNameInProductPage)).Text;

				// Скачиваем картинки
				this.SaveImages(product);

				// Найти спецификации товара	
				this.GetSpecs(product);
		
				// Переходим на страницу описания товара			
				this.mDriver.Navigate().GoToUrl(productLink);
				//Thread.Sleep(3000);

				this.mLogger.Debug("Have main product page " + productLink);

				if (CssSelectorForProductFullDescriptionInProductPage != string.Empty)
				{				
					product.FullDescription = GetInnerHtml(this.mDriver.FindElement(By.CssSelector(CssSelectorForProductFullDescriptionInProductPage)));				
				}

				product = ProductPostProcessing(product);				
			}
			else
			{					
				this.mLogger.Debug("Create NotInPriceList fantom product");
			}

			mLogger.Debug("Saving product...");
			_yandexMarketProductService.Insert(product);
			mLogger.Debug("Saving product Done.");

			if(!product.IsNotInPriceList)
				return product;
			else			
				return null;			
		}

		private void GetSpecs(YandexMarketProductRecord product)
		{
			var specificationElements = this.mDriver.FindElements(By.CssSelector(this.CssSelectorForProductSpecsRowsInProductPage));

			this.mLogger.Debug("Have product " + product.Name + ". Start getting " + specificationElements.Count + " specs...");

			foreach (var currentSpecificationElement in specificationElements)
			{
				// Key
				IWebElement currentKeyElement;
				try
				{
					currentKeyElement = currentSpecificationElement.FindElement(
						By.CssSelector(this.CssSelectorForProductSpecKeyInProductPage));
					while (currentKeyElement.Text == "")
					{
						var x = ((OpenQA.Selenium.Remote.RemoteWebElement)(currentKeyElement)).LocationOnScreenOnceScrolledIntoView.X;
						if (x == 9999998)
						{
							continue;
						}
						//Thread.Sleep(1000);
						currentKeyElement =
							currentSpecificationElement.FindElement(By.CssSelector(this.CssSelectorForProductSpecKeyInProductPage));
					}
				}
				catch (NoSuchElementException)
				{
					this.mLogger.Debug("not key");
					continue;
				}

				// Value				
				var currentValueElement =
					currentSpecificationElement.FindElement(By.CssSelector(this.CssSelectorForProductSpecValInProductPage));
				var currentValueHtml = this.GetInnerHtml(currentValueElement);

				product.Specifications.Add(new YandexMarketSpecRecord(currentKeyElement.Text.Trim(), currentValueHtml.Trim()));
			}

			this.mLogger.Debug("Finished getting specs.");
		}

		private void SaveImages(YandexMarketProductRecord product)
		{
			try
			{			
				// Get main image
				var imageUrl = this.mDriver.FindElement(By.CssSelector(this.CssSelectorForProductPictureInProductPage)).GetAttribute("src");
				//Thread.Sleep(3000);
				// do it twice because of some bag
				imageUrl = this.mDriver.FindElement(By.CssSelector(this.CssSelectorForProductPictureInProductPage)).GetAttribute("src");

				if (imageUrl.Contains(".jpg") || imageUrl.Contains(".gif"))
					product.ImageUrl_1 = this.SaveImage(imageUrl, product.Name);
				
				// Get other images
				if (CssSelectorForProductPicturesInProductPage != string.Empty)
				{
					//driver.manage().timeouts().implicitlyWait(0, TimeUnit.MILLISECONDS) 
					mDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(1));
					var imagesLinks = this.mDriver.FindElements(By.CssSelector(this.CssSelectorForProductPicturesInProductPage));
					int imageCounter = 2;
					foreach (var curImagesLink in imagesLinks)
					{
						var curImageUrl = curImagesLink.GetAttribute("href");
						if (curImageUrl.Contains(".jpg") || curImageUrl.Contains(".gif"))
						{
							if (product.ImageUrl_1 != "") 
								product.ImageUrl_1 += ";";

							product.ImageUrl_1 += this.SaveImage(curImageUrl, product.Name + "_" + imageCounter);
							imageCounter++;
						}
					}
				}
			}
			catch
			{
				this.mLogger.Debug("Error while images saving.");
				throw;
			}
		}
		
		private string SaveImage(string remoteFileUrl, string productName)
		{
			string fileName = productName.Replace('/', '-').Replace('\\', '-').Replace('*', '-').Replace('?', '-').Replace('"', '-').Replace('\'', '-').Replace('<', '-').Replace('>', '-').Replace('|', '-').Replace(' ', '_').Replace('+', '_');
			fileName += Path.GetExtension(remoteFileUrl);

			var folderPath = Path.Combine(this.mImageFolderPathBase, this.mImageFolderPathForProductList);
			var imageFolderPath = this.MakeFolder(folderPath);

			var fullFileName = Path.Combine(imageFolderPath, fileName);

			var webClient = new WebClient();

			//this.mLogger.Debug("Image will be saved to " + fullFileName + "...");
			webClient.DownloadFile(remoteFileUrl, fullFileName);
			//this.mLogger.Debug("Image saved");

			return Path.Combine(this.mImageFolderPathForProductList, fileName);
		}

		private string GetInnerHtml(IWebElement element)
		{
			var remoteWebDriver = (RemoteWebElement)element;
			var javaScriptExecutor = (IJavaScriptExecutor)remoteWebDriver.WrappedDriver;
			var innerHtml = javaScriptExecutor.ExecuteScript("return arguments[0].innerHTML;", element).ToString();

			return innerHtml;
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

		public static BaseParser Create(string catalogName, int parserCategoryId, int parseNotMoreThen, string productsPageUrl, ILogger logger, IYandexMarketProductService _yandexMarketProductService)
		{
			BaseParser parser = null;

			if(productsPageUrl.Contains("yugcontract.ua"))
				parser = new UgContractParser();
			else if (productsPageUrl.Contains("market.yandex"))
				parser = new YandexMarketParser();
			else
				throw new Exception("Can't define parser type for url=" + productsPageUrl);

			parser.Init(catalogName, parserCategoryId, parseNotMoreThen, productsPageUrl, logger,  _yandexMarketProductService);

			return parser;
		}

		protected string GetValByRegExp(string inputString, string pattern)
		{
			Match m = Regex.Match(inputString, pattern,
								RegexOptions.IgnoreCase | RegexOptions.Compiled,
								TimeSpan.FromSeconds(1));

			if (m.Success)			
				return m.Groups[1].ToString();							
			else			
				return "";			
		}

		private static List<string> GetProductsArtikulsInPiceList()
		{
			string filePath = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProductsCatalog"), "shop_f5.txt");
			List<string> errors;
			var productsArtikulsInPiceList = FileParserVendor.ParseFile(filePath, out errors).Select(x => x.Articul).ToList();
			return productsArtikulsInPiceList;
		}

		private void DeleteFromDbAppearedInPriceListFantoms(int parserCategoryId, List<string> productsArtikulsInPiceList)
		{
			var appearedInPriceListFantoms =
				this._yandexMarketProductService.GetByCategory(parserCategoryId)
										   .Where(x => x.IsNotInPriceList && productsArtikulsInPiceList.Contains(x.Articul));

			appearedInPriceListFantoms.ToList().ForEach(this._yandexMarketProductService.Delete);
		}
	}
}
