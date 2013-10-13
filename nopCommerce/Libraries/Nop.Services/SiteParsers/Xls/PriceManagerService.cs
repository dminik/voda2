namespace Nop.Services.SiteParsers.Xls
{
	using System;
	using System.Linq;

	using Nop.Core.Infrastructure;
	using Nop.Services.Catalog;
	using Nop.Services.Logging;
	using Nop.Services.Tasks;

	public class PriceManagerService : IPriceManagerService
	{
		private readonly IProductService _productService;
		private readonly ILogger _logger;
		private readonly IOstatkiPriceParserService _ostatkiPriceParserService;
		private readonly IF5PriceParserService _f5PriceParserService;
		private readonly IYugCatalogPriceParserService _yugCatalogPriceParserService;

		public PriceManagerService(IProductService productService, 
			ILogger logger, 
			IOstatkiPriceParserService ugContractPriceParserService,
			IF5PriceParserService f5PriceParserService,
			 IYugCatalogPriceParserService yugCatalogPriceParserService)			
		{
			this._productService = productService;
			this._logger = logger;
			this._ostatkiPriceParserService = ugContractPriceParserService;
			this._f5PriceParserService = f5PriceParserService;
			this._yugCatalogPriceParserService = yugCatalogPriceParserService;
		}
		
		public string ApplyImportAll()
		{
			this._logger.Debug("+++++ ApplyPriceImportAll start");
			var name = typeof(ParsePricesTask).FullName + ", Nop.Services";
			var scheduleTaskService = EngineContext.Current.Resolve<IScheduleTaskService>();
			var scheduleTask = scheduleTaskService.GetTaskByType(name);

			var isNeedUpdate = false;

			// Update prices if it need
			if (scheduleTask.LastSuccessUtc != null)
			{
				if (scheduleTask.LastSuccessUtc.Value.Date < DateTime.UtcNow.Date)
				{
					isNeedUpdate = true;
					this._logger.Debug("      Need daily update");
				}
			}
			else
			{
				this._logger.Debug("      Need first time update");
				isNeedUpdate = true;
			}

			/*
			 * Порядок проставки флагов и цен
			 * 1. Сбрасываем все цены и флаги в 0
			 * 2. Если Товар есть у поставщика или в Боярке - ставим цену
			 * 3. Если товар есть у поставщика, то ставим предзаказ "да"
			 * 4. Если товар есть в Боярке, то ставим предзаказ "нет" и Доставка сейчас "Да"
			 * 
			 * 
			                     Товара нет нигде               Товар есть у поставщика               Товар в Боярке  
			 цена           0 (не показываем его на сайте)       100                                         100		
			 предзаказ              да                           да                                          нет			
		     доставка сейчас        нет                          нет                                          да
			 
			*/

			var productVariantList = this._productService.GetProductVariants().ToList();

			var f5PriceList = this._f5PriceParserService.ParseAndShow(isNeedUpdate);
			var ostatkiPriceList = this._ostatkiPriceParserService.ParseAndShow(isNeedUpdate);
			var ugCatalogPriceList = this._yugCatalogPriceParserService.ParseAndShow(isNeedUpdate);


			var totalProducts = productVariantList.Count;
			var productsToSell = 0;
			var productsToSellInBoyarka = 0;

			foreach (var currentProductVariant in productVariantList)
			{
				var productInVendor = ugCatalogPriceList.ProductLineList.SingleOrDefault(x => x.Articul == currentProductVariant.Sku);
				var isProductInVendor = productInVendor != null;

				var productInBoyarka = ostatkiPriceList.ProductLineList.SingleOrDefault(x => x.Articul == currentProductVariant.Sku);
				var isProductInBoyarka = productInBoyarka != null;

				var productInF5Price = f5PriceList.ProductLineList.SingleOrDefault(x => x.Articul == currentProductVariant.Sku);
				var isProductInF5Price = productInF5Price != null;

				//  2. Если Товар есть у поставщика или в Боярке - ставим цену				
				if ((isProductInVendor || isProductInBoyarka) && isProductInF5Price)
				{
					var price = productInF5Price.Price > 5 ? productInF5Price.Price : 3; // товара дешевле 3 гривен быть не должно					
					currentProductVariant.Price = price;
					productsToSell++;
				}
				else
				{
					// 1. Сбрасываем все цены и флаги в 0					
					currentProductVariant.Price = 0;
					continue;
				}

				// 3. Если товар есть у поставщика, то ставим предзаказ "да"
				if (isProductInVendor)
					currentProductVariant.AvailableForPreOrder = true;

				// 4. Если товар есть в Боярке, то ставим предзаказ "нет" и Доставка сейчас "Да"
				if (isProductInBoyarka)
				{
					currentProductVariant.AvailableForPreOrder = false;
					productsToSellInBoyarka++;
				}

			}// END FOR

			if (productVariantList.Count > 0) // Вызываем сохранение всего контекста (всех вариантов)
				this._productService.UpdateProductVariant(productVariantList[0]);

			var msgResult =	  "      totalProducts: " + totalProducts 
							+ ", productsToSell: " + productsToSell
			                + ", productsToSellInBoyarka: " + productsToSellInBoyarka;

			this._logger.Debug(msgResult);

			this._logger.Debug("+++++ ApplyPriceImportAll end");

			return msgResult;
		}
	}
}
