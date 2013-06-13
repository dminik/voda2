﻿namespace Nop.Core.SiteParsers
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Threading;
	using System.Web;

	using Nop.Core.Domain.YandexMarket;

	using OpenQA.Selenium;
	using OpenQA.Selenium.IE;

	// Requires reference to WebDriver.Support.dll
	// using OpenQA.Selenium.Support.UI;

	public class Parser
	{		
		public Parser(string catalogName, int parseNotMoreThen)
		{
			this.imageFolderPathForProductList = catalogName;
			string filePath = Path.Combine(HttpRuntime.AppDomainAppPath, "content\\files\\exportimport",  "StaticFileName");
			this.imageFolderPathBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProductsCatalog");
			this.ParseNotMoreThen = parseNotMoreThen;
		}

		int ParseNotMoreThen { get; set; }

		// Папка для картинок
		string imageFolderPathBase;
		string imageFolderPathForProductList;


		IWebDriver mDriver;

		public List<YandexMarketProductRecord> Parse()
		{
			var resultProductList = new List<YandexMarketProductRecord>();

			try
			{
				this.mDriver = new InternetExplorerDriver();

				// mDriver = new FirefoxDriver();
				//var binary = new FirefoxBinary("c:\\Program Files (x86)\\Mozilla Firefox\\firefox.exe");
				//var profile = new FirefoxProfile();
				//profile.Port = 7056;  
				//mDriver = new FirefoxDriver(profile);

				Thread.Sleep(5000);
				// Ссылка на список товаров
				this.mDriver.Navigate().GoToUrl("http://market.yandex.ua/guru.xml?CMD=-RR=0,0,0,0-PF=1801946~EQ~sel~12561075-VIS=70-CAT_ID=975896-EXC=1-PG=10&hid=90582");
				Thread.Sleep(5000);
				
				const int delayInSeconds = 6;


				bool isNextPage = false;

				var productLinks = new List<string>();

				do
				{
					// Найти все ссылки на товары			
					var linksFromCurrentPage = this.mDriver.FindElements(By.CssSelector("a.b-offers__name")).Select(s => s.GetAttribute("href")).ToList();
					productLinks.AddRange(linksFromCurrentPage);

					if (this.ParseNotMoreThen <= 10)
						break;

					try
					{
						// Жммем на следущую страницу, если она есть
						var nextPage = this.mDriver.FindElement(By.CssSelector("a.b-pager__next"));

						nextPage.Click();
						isNextPage = true;
					}
					catch (NoSuchElementException)
					{
						isNextPage = false;
					}
				}
				while (isNextPage);
				
				int counter = 1;
				foreach (var currentProductLink in productLinks)
				{
					var product = this.ProcessProductLink(currentProductLink);

					resultProductList.Add(product);

					counter++;

					if (counter > this.ParseNotMoreThen)
						break;

					Thread.Sleep(delayInSeconds * 1000);
				}				
			}
			finally
			{				
				this.mDriver.Quit();
			}

			return resultProductList;
		}

		private YandexMarketProductRecord ProcessProductLink(string productLink)
		{
			// Переходим на страницу спецификации товара
			this.mDriver.Navigate().GoToUrl(productLink.Replace("model.xml", "model-spec.xml"));
			Thread.Sleep(3000);

			var product = new YandexMarketProductRecord();

			// Найти имя товара		
			product.Name = this.mDriver.FindElement(By.CssSelector("h1.b-page-title")).Text;

			// Скачиваем картинку
			var imageUrl = this.mDriver.FindElement(By.CssSelector("div.b-model-microcard__img img")).GetAttribute("src");
			Thread.Sleep(3000);
			imageUrl = this.mDriver.FindElement(By.CssSelector("div.b-model-microcard__img img")).GetAttribute("src");
			product.ImageUrl_1 = this.SaveImage(imageUrl, product.Name);

			// Найти спецификации товара	table.b-properties tr
			var specificationElements = this.mDriver.FindElements(By.CssSelector("table.b-properties tr"));

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

				product.Specifications.Add(new YandexMarketSpecRecord(currentKeyElement.Text, currentValueElement.Text));
			}

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
			webClient.DownloadFile(remoteFileUrl, fullFileName);

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
