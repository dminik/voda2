namespace Nop.Services.SiteParsers
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Threading;
	using System.Web;

	using Nop.Core;
	using Nop.Core.Domain.YandexMarket;
	using Nop.Core.IO;
	using Nop.Services.FileParsers;
	using Nop.Services.Logging;
	using Nop.Services.YandexMarket;

	using OfficeOpenXml;

	using OpenQA.Selenium;
	using OpenQA.Selenium.IE;
	using OpenQA.Selenium.Remote;
	using OpenQA.Selenium.Support.UI;


	public class F5PriceParser : BasePriceParser<ProductLineVendor>
	{		
		protected override string XlsFileName { get { return "f5_price.xls"; } }

		protected override string UrlBase { get { return "http://shop.f5.ua"; } }
		protected override string UrlAuthorization { get { return UrlBase; } }
		protected override string UrlAuthorizationPostParams { get { return "login=oleynic&password=18072008y&go=%D0%92%D1%85%D0%BE%D0%B4"; } }
		protected override string UrlDownload { get { return UrlBase + "/20861/client/price/export/"; } }

		protected override void PostProcessing()
		{
			if (ResultList.Count() < 9000)
				throw new Exception("Hm... Price list less then 9000 lines. Count=" + ResultList.Count());
		}


		protected override ProductLineVendor GetObjectFromReader(IDataReader reader)
		{
			var newProductLineVendor = new ProductLineVendor();
			
			try
			{
				var artikul = XlsProvider.GetFieldValueFromReader<string>(reader, "Код товара");

				if (artikul.Length != 7) // articul должен быть 7 символов
					return null;

				newProductLineVendor.Articul = artikul;
				newProductLineVendor.Name = XlsProvider.GetFieldValueFromReader<string>(reader, "Название товара");

				newProductLineVendor.PriceRaschet = GetInt(XlsProvider.GetFieldValueFromReader<string>(reader, "Расч# цена"));
				newProductLineVendor.Price = GetInt(XlsProvider.GetFieldValueFromReader<string>(reader, "Цена продажи"));
				newProductLineVendor.PriceBase = GetInt(XlsProvider.GetFieldValueFromReader<string>(reader, "Базовая цена"));
				newProductLineVendor.PriceDiff = GetInt(XlsProvider.GetFieldValueFromReader<string>(reader, "Цена продажи – Базовая цена"));

			}
			catch (Exception ex)
			{
				// errors.Add(worksheet.Cells[currentRow, GetColumnIndex(properties, "Код товара")].Value as string);
				return null;
			}
			return newProductLineVendor;
		}
	}
}
