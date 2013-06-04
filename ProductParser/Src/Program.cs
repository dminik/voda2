using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductParser
{
	using System.IO;
	using System.Net;
	using System.Reflection;
	using System.Threading;

	using OpenQA.Selenium;
	using OpenQA.Selenium.Firefox;

	// Requires reference to WebDriver.Support.dll
	// using OpenQA.Selenium.Support.UI;
	using OpenQA.Selenium.Support.UI;

	class Program
	{

		// Папка для картинок
		static string imageFolderPathBase = "..\\..\\ProductsCatalog\\";
		static string imageFolderPathForProductList = "Atoll\\";


		static readonly IWebDriver Driver = new FirefoxDriver();

		static void Main(string[] args)
		{
			// Ссылка на список товаров
			Driver.Navigate().GoToUrl("http://market.yandex.ua/guru.xml?CMD=-RR=0,0,0,0-PF=1801946~EQ~sel~12561075-VIS=70-CAT_ID=975896-EXC=1-PG=10&hid=90582");
			const bool useFirstThreeProducts = true;
			const int delayInSeconds = 6;


			bool isNextPage = false;

			var productLinks = new List<string>();

			do
			{
				// Найти все ссылки на товары			
				var linksFromCurrentPage = Driver.FindElements(By.CssSelector("a.b-offers__name")).Select(s => s.GetAttribute("href")).ToList();
				productLinks.AddRange(linksFromCurrentPage);

				if (useFirstThreeProducts)
					break;

				try
				{
					// Жммем на следущую страницу, если она есть
					var nextPage = Driver.FindElement(By.CssSelector("a.b-pager__next"));

					nextPage.Click();
					isNextPage = true;
				}
				catch (NoSuchElementException)
				{
					isNextPage = false;
				}
			}
			while (isNextPage);



			var productList = new List<Product>();

			int counter = 1;
			foreach (var currentProductLink in productLinks)
			{
				var product = ProcessProductLink(currentProductLink);

				productList.Add(product);

				counter++;

				if (useFirstThreeProducts && counter == 3)
					break;

				Thread.Sleep(delayInSeconds * 1000);
			}

			//Close the browser
			Driver.Quit();

			System.Console.WriteLine("Press any key to exit...");
			System.Console.ReadKey();

		}

		private static Product ProcessProductLink(string productLink)
		{
			// Переходим на страницу спецификации товара
			Driver.Navigate().GoToUrl(productLink.Replace("model.xml", "model-spec.xml"));

			var product = new Product();

			// Найти имя товара		
			product.Title = Driver.FindElement(By.CssSelector("h1.b-page-title")).Text;

			// Скачиваем картинку
			var imageUrl = Driver.FindElement(By.CssSelector("div.b-model-microcard__img img")).GetAttribute("src");
			product.ImageUrl_1 = SaveImage(imageUrl, product.Title);

			// Найти спецификации товара	table.b-properties tr
			var specificationElements = Driver.FindElements(By.CssSelector("table.b-properties tr"));

			foreach (var currentSpecificationElement in specificationElements)
			{
				// Key
				IWebElement currentKeyElement;
				try
				{
					currentKeyElement = currentSpecificationElement.FindElement(By.CssSelector("th.b-properties__label span"));
				}
				catch (NoSuchElementException)
				{
					continue;
				}

				// Value
				var currentValueElement = currentSpecificationElement.FindElement(By.CssSelector("td.b-properties__value"));

				product.Specifications.Add(currentKeyElement.Text, currentValueElement.Text);
			}


			Console.WriteLine(product.ToString() + Environment.NewLine);

			return product;
		}

		private static string SaveImage(string remoteFileUrl, string productName)
		{
			string fileName = productName.Replace('/', '-').Replace('\\', '-').Replace('*', '-').Replace('?', '-').Replace('"', '-').Replace('\'', '-').Replace('<', '-').Replace('>', '-').Replace('|', '-');
			fileName += Path.GetExtension(remoteFileUrl);

			var folderPath = Path.Combine(imageFolderPathBase, imageFolderPathForProductList);
			var imageFolderPath = MakeFolder(folderPath);
			
			var fullFileName = Path.Combine(imageFolderPath, fileName) + Path.GetExtension(remoteFileUrl);

			var webClient = new WebClient();
			webClient.DownloadFile(remoteFileUrl, fullFileName);

			return Path.Combine(imageFolderPathForProductList, fileName);
		}

		public static string MakeFolder(string path)
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
