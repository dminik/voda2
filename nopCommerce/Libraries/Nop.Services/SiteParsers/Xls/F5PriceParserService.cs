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
	using Nop.Core.Domain.Security;
	using Nop.Core.Domain.YandexMarket;
	using Nop.Core.IO;
	using Nop.Core.Infrastructure;
	using Nop.Services.Catalog;
	using Nop.Services.FileParsers;
	using Nop.Services.Logging;
	using Nop.Services.YandexMarket;

	using OfficeOpenXml;

	using OpenQA.Selenium;
	using OpenQA.Selenium.IE;
	using OpenQA.Selenium.Remote;
	using OpenQA.Selenium.Support.UI;


	public class F5PriceParserService : BasePriceParser<ProductLineVendor>, IF5PriceParserService
	{	
		public F5PriceParserService(IProductService productService, ILogger logger)
			: base(productService, logger)
		{
			
		}

		protected override string XlsFileName { get { return "f5_price.xls"; } }

		protected override string UrlBase { get { return "http://shop.f5.ua"; } }
		protected override string UrlAuthorization { get { return UrlBase; } }
		protected override string UrlAuthorizationPostParams
		{
			get
			{
				var securitySettings = EngineContext.Current.Resolve<SecuritySettings>();
				
				return "login=" + securitySettings.F5SiteLogin 
					+ "&password=" + securitySettings.F5SitePassword 
					+ "&go=%D0%92%D1%85%D0%BE%D0%B4";
			}
		}
		protected override string UrlDownload { get { return UrlBase + "/20861/client/price/export/"; } }

		protected override void PostProcessing()
		{
			if (ResultList.Count() < 9000)
				throw new Exception("Hm..." + this.GetType().Name + ". Price list less then 9000 lines. Count=" + ResultList.Count());
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
			catch 
			{
				// errors.Add(worksheet.Cells[currentRow, GetColumnIndex(properties, "Код товара")].Value as string);
				return null;
			}
			return newProductLineVendor;
		}

		public VendorParserModel ParseAndShow(bool isUpdateCacheFromInternet)
		{			
			List<string> errors;

			var list = GetPriceListFromCache(out errors, isUpdateCacheFromInternet);

			var newSpecsOnly = new VendorParserModel { ProductLineList = list, ErrorList = errors };

			return newSpecsOnly;
		}

		public string SetVendorPrices(bool isUpdateCacheFromInternet)
		{			
			List<string> errors;

			var list = GetPriceListFromCache(out errors, isUpdateCacheFromInternet);


			var newSpecsOnly = new VendorParserModel { ProductLineList = list, ErrorList = errors };
			string foundArticules = "";
			int successCounter = 0;

			// Получить все варианты
			// Если вариант есть в прайсе, то выставляем цену и преордер
			// Иначе сбрасываем
			// Апдейдим сразу все

			var productVariantList = _productService.GetProductVariants().ToList();
			foreach (var currentProductVariant in productVariantList)
			{
				var curProductLine = newSpecsOnly.ProductLineList.SingleOrDefault(x => x.Articul == currentProductVariant.Sku);

				if (curProductLine == null)
				{
					if (currentProductVariant.Price != 0)
						currentProductVariant.Price = 0;

					continue;
				}

				var price = curProductLine.Price > 5 ? curProductLine.Price : 3; // товара дешевле 3 гривен быть не должно
				if (currentProductVariant.Price != price)
				{
					currentProductVariant.Price = price;
				}

				if (currentProductVariant.AvailableForPreOrder != true)
					currentProductVariant.AvailableForPreOrder = true;

				successCounter++;
				foundArticules += curProductLine.Articul + ", ";
			}

			if (productVariantList.Count > 0) // Вызываем сохранение всего контекста (всех вариантов)
				_productService.UpdateProductVariant(productVariantList[0]);

			var msg = "Success for " + successCounter + " from " + newSpecsOnly.ProductLineList.Count();

			mLogger.Debug(msg);

			return msg;
		}
	}
}
