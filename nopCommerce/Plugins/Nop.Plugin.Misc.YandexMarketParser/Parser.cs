﻿namespace Nop.Plugin.Misc.YandexMarketParser
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.IO;
	using System.Net;
	using System.Reflection;
	using System.Threading;

	using Nop.Plugin.Misc.YandexMarketParser.Models;

	using OpenQA.Selenium;
	using OpenQA.Selenium.Firefox;
	using OpenQA.Selenium.IE;

	// Requires reference to WebDriver.Support.dll
	// using OpenQA.Selenium.Support.UI;

	public class Parser
	{
		public Parser(string catalogName)
		{
			imageFolderPathForProductList = catalogName;
			imageFolderPathBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProductsCatalog");			 
		}

		// Папка для картинок
		string imageFolderPathBase;
		string imageFolderPathForProductList;


		IWebDriver mDriver;

		public YandexMarketParserModel Parse()
		{



			mDriver = new InternetExplorerDriver();

			// mDriver = new FirefoxDriver();
			//var binary = new FirefoxBinary("c:\\Program Files (x86)\\Mozilla Firefox\\firefox.exe");
			//var profile = new FirefoxProfile();
			//profile.Port = 7056;  
			//mDriver = new FirefoxDriver(profile);

			Thread.Sleep(5000);
			// Ссылка на список товаров
			mDriver.Navigate().GoToUrl("http://market.yandex.ua/guru.xml?CMD=-RR=0,0,0,0-PF=1801946~EQ~sel~12561075-VIS=70-CAT_ID=975896-EXC=1-PG=10&hid=90582");
			Thread.Sleep(5000);
			const bool useFirstThreeProducts = true;
			const int delayInSeconds = 6;


			bool isNextPage = false;

			var productLinks = new List<string>();

			do
			{
				// Найти все ссылки на товары			
				var linksFromCurrentPage = mDriver.FindElements(By.CssSelector("a.b-offers__name")).Select(s => s.GetAttribute("href")).ToList();
				productLinks.AddRange(linksFromCurrentPage);

				if (useFirstThreeProducts)
					break;

				try
				{
					// Жммем на следущую страницу, если она есть
					var nextPage = mDriver.FindElement(By.CssSelector("a.b-pager__next"));

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
			mDriver.Quit();

			var resultModel = new YandexMarketParserModel()
				{
					CatalogName = imageFolderPathForProductList,
					ProductList = productList
				};

			return resultModel;
		}

		private Product ProcessProductLink(string productLink)
		{
			// Переходим на страницу спецификации товара
			mDriver.Navigate().GoToUrl(productLink.Replace("model.xml", "model-spec.xml"));
			Thread.Sleep(3000);

			var product = new Product();

			// Найти имя товара		
			product.Title = mDriver.FindElement(By.CssSelector("h1.b-page-title")).Text;

			// Скачиваем картинку
			var imageUrl = mDriver.FindElement(By.CssSelector("div.b-model-microcard__img img")).GetAttribute("src");
			Thread.Sleep(3000);
			imageUrl = mDriver.FindElement(By.CssSelector("div.b-model-microcard__img img")).GetAttribute("src");
			product.ImageUrl_1 = SaveImage(imageUrl, product.Title);

			// Найти спецификации товара	table.b-properties tr
			var specificationElements = mDriver.FindElements(By.CssSelector("table.b-properties tr"));

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

			return product;
		}

		private string SaveImage(string remoteFileUrl, string productName)
		{
			string fileName = productName.Replace('/', '-').Replace('\\', '-').Replace('*', '-').Replace('?', '-').Replace('"', '-').Replace('\'', '-').Replace('<', '-').Replace('>', '-').Replace('|', '-');
			fileName += Path.GetExtension(remoteFileUrl);

			var folderPath = Path.Combine(imageFolderPathBase, imageFolderPathForProductList);
			var imageFolderPath = MakeFolder(folderPath);

			var fullFileName = Path.Combine(imageFolderPath, fileName);

			var webClient = new WebClient();
			webClient.DownloadFile(remoteFileUrl, fullFileName);

			return Path.Combine(imageFolderPathForProductList, fileName);
		}

		private string MakeFolder(string path)
		{
			var newFolder = new System.IO.DirectoryInfo(path);
			if (!newFolder.Exists)
			{
				try
				{
					newFolder.Create();
				}
				catch (IOException ex)
				{
					string s = Directory.GetCurrentDirectory();
					var s2 = Assembly.GetExecutingAssembly().FullName;
					
					throw;

				}
				catch (Exception ex)
				{
					string s = Directory.GetCurrentDirectory();
					var s2 = Assembly.GetExecutingAssembly().FullName;
					throw;

				}
				
			}

			return path;
		}
	}
}