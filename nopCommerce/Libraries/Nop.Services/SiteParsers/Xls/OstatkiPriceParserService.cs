namespace Nop.Services.SiteParsers
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;

	using Nop.Core.Caching;
	using Nop.Core.Domain.Security;
	using Nop.Core.IO;
	using Nop.Core.Infrastructure;
	using Nop.Services.Catalog;
	using Nop.Services.Logging;
	using Nop.Services.SiteParsers.Xls;
	using Nop.Services.Tasks;

	public class OstatkiPriceParserService : BasePriceParser<ProductLineVendor>, IOstatkiPriceParserService
	{
		public OstatkiPriceParserService(IProductService productService, ILogger logger, ICacheManager cacheManager)
			: base(productService, logger, cacheManager)
		{
			
		}

		protected override string XlsFileName { get { return "Ostatki.xls"; } }

		protected override string UrlBase { get { return "http://yugpartner.com.ua"; } }
		protected override string UrlAuthorizationGet { get { return UrlBase + "/auth/"; } }
		protected override string UrlAuthorizationPostParams
		{
			get
			{
				var securitySettings = EngineContext.Current.Resolve<SecuritySettings>();

				return "login=" + securitySettings.F5SiteLogin
					+ "&password=" + securitySettings.F5SitePassword;
			}
		}
		
		protected override string UrlDownload 
		{ 
			get
			{				
				var yesterday = DateTime.UtcNow.AddDays(-1).ToString("dd.MM.yyyy");
				return UrlBase + "/report-f5/trade-balances/date" + yesterday + "/suppliers4,95/excel/";
			} 
		}
		
		protected override void PostProcessing()
		{
			if (ResultList.Count() < 700)
				throw new Exception("Hm... " + this.GetType().Name + ".Price list less then 700 lines. Count=" + ResultList.Count());
		}

		protected override ProductLineVendor GetObjectFromReader(IDataReader reader)
		{
			var newProductLineVendor = new ProductLineVendor();
			
			try
			{
				var artikul = XlsProvider.GetFieldValueFromReader<string>(reader, "Код товара");

				int intArticil;
				if (!int.TryParse(artikul, out intArticil)) // articul должен быть 7 символов
					return null;

				newProductLineVendor.Articul = artikul;
				newProductLineVendor.Name = XlsProvider.GetFieldValueFromReader<string>(reader, "Товар");

				//newProductLineVendor.PriceRaschet = GetInt(XlsProvider.GetFieldValueFromReader<string>(reader, "Расч# цена"));
				//newProductLineVendor.Price = GetInt(XlsProvider.GetFieldValueFromReader<string>(reader, "Цена продажи"));
				//newProductLineVendor.PriceBase = GetInt(XlsProvider.GetFieldValueFromReader<string>(reader, "Базовая цена"));
				//newProductLineVendor.PriceDiff = GetInt(XlsProvider.GetFieldValueFromReader<string>(reader, "Цена продажи – Базовая цена"));

			}
			catch
			{
				// errors.Add(worksheet.Cells[currentRow, GetColumnIndex(properties, "Код товара")].Value as string);
				return null;
			}
			return newProductLineVendor;
		}

		public OstatkiParserModel ParseAndShow(bool isUpdateCacheFromInternet)
		{
			var newSpecsOnly = _Parse(isUpdateCacheFromInternet);
			return newSpecsOnly;
		}

		/// <summary>
		/// Set existing in Boyarka
		/// </summary>
		/// <param name="isUpdateCacheFromInternet"></param>
		/// <returns></returns>
		public string SetExistingInBoyarka(bool isUpdateCacheFromInternet)
		{
			var priceList = _Parse(isUpdateCacheFromInternet);

			string notFoundArticules = ""; 
			string foundArticules = ""; 
			int successCounter = 0;

			/*
			                     Товара нет нигде               Товар есть у поставщика               Товар в Боярке
			 цена           0 (не показываем его на сайте)       есть                                       есть
			 предзаказ              да                           да                                          нет
		     доставка сейчас                     -               нет                                          да
			 
			*/



			var productVariantList = _productService.GetProductVariants().ToList();
			foreach (var currentProductVariant in productVariantList)
			{
				var curProductLine = priceList.ProductLineList.SingleOrDefault(x => x.Articul == currentProductVariant.Sku);
				var isProductInPriceList = curProductLine == null;
			
				if (isProductInPriceList)
				{
					notFoundArticules += currentProductVariant.Sku + ", ";
					continue;
				}
			
				//if (currentProductVariant.StockQuantity != curProductLine.Amount)
				//{
				//	currentProductVariant.StockQuantity = curProductLine.Amount;
				//	isNeedUpdate = true;
				//}

				//if (currentProductVariant.Price != curProductLine.Price)
				//{
				//	currentProductVariant.Price = curProductLine.Price;
				//	isNeedUpdate = true;
				//}

				if (currentProductVariant.AvailableForPreOrder != false)
				{
					currentProductVariant.AvailableForPreOrder = false;					
				}

				//if (isNeedUpdate)
				//	_productService.UpdateProductVariant(currentProductVariant);

				successCounter++;
				foundArticules += curProductLine.Articul + ", ";
			}

			if (productVariantList.Count > 0) // Вызываем сохранение всего контекста (всех вариантов)
				_productService.UpdateProductVariant(productVariantList[0]);

			var msg = "";

			if (notFoundArticules == "")
				msg = "Success for all!";
			else
			{
				msg = "Success for " + successCounter + " from " + priceList.ProductLineList.Count();
			}

			mLogger.Debug(msg);
			return msg;
		}
	
		private OstatkiParserModel _Parse(bool isUpdateCacheFromInternet)
		{			
			List<string> errors;

			var list = GetPriceListFromCache(out errors, isUpdateCacheFromInternet);

			var resultModel = new OstatkiParserModel { ProductLineList = list, ErrorList = errors };

			return resultModel;			
		}
	}
}
