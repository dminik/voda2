namespace Nop.Services.SiteParsers
{
	using System;
	using System.Text.RegularExpressions;
	using System.Threading;


	using OpenQA.Selenium;
	using OpenQA.Selenium.Remote;


	public static class ParserHelper
	{
		public static IWebElement FindElement(IWebDriver driver, string cssSelector, int tryTimes = 3)
		{
			var time = 0;
			do
			{
				time++;
				try
				{
					return driver.FindElement(By.CssSelector(cssSelector));
				}
				catch (Exception ex)
				{
					if(!ex.Message.Contains("element timed out") || time == tryTimes)
						throw;

					Thread.Sleep(3000);
				}
			
			}
			while (time < tryTimes);
			
			throw new Exception("Error in my FindElement");
		}

		public static string GetInnerHtml(IWebElement element)
		{
			var remoteWebDriver = (RemoteWebElement)element;
			var javaScriptExecutor = (IJavaScriptExecutor)remoteWebDriver.WrappedDriver;
			var innerHtml = javaScriptExecutor.ExecuteScript("return arguments[0].innerHTML;", element).ToString();

			return innerHtml;
		}

		public static string GetValByRegExp(string inputString, string pattern)
		{
			Match m = Regex.Match(inputString, pattern,
								RegexOptions.IgnoreCase | RegexOptions.Compiled,
								TimeSpan.FromSeconds(1));

			if (m.Success)			
				return m.Groups[1].ToString();							
			else			
				return "";			
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
