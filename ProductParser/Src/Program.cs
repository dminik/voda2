using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductParser
{
	using OpenQA.Selenium;
	using OpenQA.Selenium.Firefox;

	// Requires reference to WebDriver.Support.dll
	// using OpenQA.Selenium.Support.UI;
	using OpenQA.Selenium.Support.UI;

	class Program
	{	
		static readonly IWebDriver Driver = new FirefoxDriver();

		static void Main(string[] args)
		{			
			// Ссылка на список товаров
			Driver.Navigate().GoToUrl("http://market.yandex.ua/guru.xml?CMD=-RR=0,0,0,0-PF=1801946~EQ~sel~12561075-VIS=70-CAT_ID=975896-EXC=1-PG=10&hid=90582");

			bool isNextPage = false;

			var productLinks = new List<string>();

			do
			{
				// Найти все ссылки на товары			
				var linksFromCurrentPage = Driver.FindElements(By.CssSelector("a.b-offers__name")).Select(s => s.GetAttribute("href")).ToList();
				productLinks.AddRange(linksFromCurrentPage);

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

			int counter = 0;
			foreach (var currentProductLink in productLinks)
			{
				var product = ProcessProductLink(currentProductLink);

				productList.Add(product);

				if(counter++ == 10)
					break;
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
	}
}
