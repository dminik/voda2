namespace Nop.Services.SiteParsers
{
	using System;
	using System.Collections.ObjectModel;
	using System.IO;
	using System.Net;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Threading;

	using Nop.Core.Infrastructure;
	using Nop.Services.Logging;

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

		public static ReadOnlyCollection<IWebElement> FindElements(IWebDriver driver, string cssSelector, int tryTimes = 3)
		{
			var time = 0;
			var logger = EngineContext.Current.Resolve<ILogger>();

			do
			{
				time++;
				try
				{
					var elements = driver.FindElements(By.CssSelector(cssSelector));
					
					var count = elements.Count;
					logger.Debug("FindElements=" + count); 

					return elements;
				}
				catch (Exception ex)
				{
					if (!ex.Message.Contains("elements timed out") || time == tryTimes)
						throw;

					Thread.Sleep(10000);
				}

			}
			while (time < tryTimes);

			throw new Exception("Error in my FindElements");
		}

		public static string GetPageSource(IWebDriver driver, int tryTimes = 6)
		{
			var time = 0;
			do
			{
				time++;
				try
				{
					var source = driver.PageSource;

					var logger = EngineContext.Current.Resolve<ILogger>();
					if (source.Length != 0)
						logger.Debug(source);

					return source;
				}
				catch (Exception ex)
				{
					if (!ex.Message.Contains("source timed out") || time == tryTimes)
						throw;

					Thread.Sleep(10000);
				}

			}
			while (time < tryTimes);

			throw new Exception("Error in my GetPageSource");
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

		public static void CheckResponseCorrect(HttpWebResponse myHttpWebResponse, string originalStrInContent, HttpStatusCode status)
		{
			var strHtmlLast = GetHtmlText(myHttpWebResponse);

			if (myHttpWebResponse.StatusCode != status)
				throw new Exception("Url: " + myHttpWebResponse.ResponseUri
					+ "; expected http status is " + status.ToString()
					+ "; current http status is " + myHttpWebResponse.StatusCode.ToString());

			if (originalStrInContent != "")
				if (!strHtmlLast.Contains(originalStrInContent))
					throw new Exception("Url: " + myHttpWebResponse.ResponseUri + "; Downloaded page content do not have string like: " + originalStrInContent);
		}

		public static string GetHtmlText(HttpWebResponse pHttpWebResponse)
		{
			try
			{
				Encoding enc = null;
				switch (pHttpWebResponse.CharacterSet)
				{
					case "utf-8":
						enc = Encoding.UTF8;
						break;
					case "1251":
						enc = Encoding.GetEncoding(1251);
						break;

					default:
						enc = Encoding.UTF8;
						break;
				}

				StreamReader myStreamReader = new StreamReader(pHttpWebResponse.GetResponseStream(), enc);
				var strHtmlLast = myStreamReader.ReadToEnd();
				return strHtmlLast;
			}
			catch
			{
				return "";
			}
		}

		public static HttpWebResponse HttpQuery(string uri, bool isPost, string postParams, string contentType, CookieContainer cookieCont, string urlBase)
		{
			ServicePointManager.Expect100Continue = false;
			ServicePointManager.SetTcpKeepAlive(true, 6000000, 100000);

			HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(uri);


			myHttpWebRequest.Accept = "application/x-ms-application, image/jpeg, application/xaml+xml, image/gif, image/pjpeg, application/x-ms-xbap, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*"; // accept;// "application/x-ms-application, image/jpeg, application/xaml+xml, image/gif, image/pjpeg, application/x-ms-xbap, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
			myHttpWebRequest.Headers.Add("Accept-Language", "ru-RU");
			myHttpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; MRSPUTNIK 2, 3, 0, 289; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET CLR 1.1.4322; .NET4.0C; .NET4.0E; InfoPath.2)";
			myHttpWebRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
			//myHttpWebRequest.Host = "business.moneygram.com";


			if (contentType != "") myHttpWebRequest.ContentType = contentType;

			if (cookieCont.Count > 0)
				myHttpWebRequest.CookieContainer = cookieCont;//вписываем куки

			myHttpWebRequest.Timeout = 180000;//3 min
			myHttpWebRequest.AllowAutoRedirect = false;// ставим False, чтобы при получении кода 302 не делать автоматический редирект
			if (isPost)
			{
				myHttpWebRequest.Method = "POST";

				//Пишем параметры формы в запрос
				byte[] ByteArr = System.Text.Encoding.GetEncoding(1251).GetBytes(postParams);
				myHttpWebRequest.ContentLength = ByteArr.Length;
				myHttpWebRequest.GetRequestStream().Write(ByteArr, 0, ByteArr.Length);
				myHttpWebRequest.GetRequestStream().Close();
			}

			myHttpWebRequest.KeepAlive = true;

			HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

			//запоминаем полученные куки
			if (!String.IsNullOrEmpty(myHttpWebResponse.Headers["Set-Cookie"]))
				cookieCont.SetCookies(new Uri(urlBase), myHttpWebResponse.Headers["Set-Cookie"]);

			return myHttpWebResponse;
		}

		public static int GetInt(string str)
		{
			if (str == "") return 0;

			try
			{
				var partStr = str.Split('.', ',')[0];
				var result = int.Parse(partStr);
				return result;
			}
			catch
			{
				var partStr = str.Split(',')[0];
				var result = int.Parse(partStr);
				return result;
			}
		}

		public static decimal GetDecimal(string str)
		{
			if (str == "") return 0;

			try
			{
				var result = decimal.Parse(str);
				return result;
			}
			catch
			{
				var partStr = str.Replace(',', '.');
				var result = decimal.Parse(partStr);
				return result;
			}
		}
	}
}
