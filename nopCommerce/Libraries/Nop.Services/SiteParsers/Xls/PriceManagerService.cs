namespace Nop.Services.SiteParsers.Xls
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;

	using Nop.Core.Domain.Catalog;
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

		private IEnumerable<Category> GetUsedCategories(IEnumerable<ProductLine> products, IEnumerable<ProductVariant> allProductVariantList)
		{			
			var skuList = products.ToList().Select(x => x.Articul).ToList();

			var usedCategories = allProductVariantList
				.Where(x =>  skuList.Contains(x.Sku))
				.Select(pv => (pv.Product.ProductCategories.ToList()[0]).Category)
				.Distinct();

			return usedCategories;
		}

		public string ApplyImportAll(bool isForceDownloadingNewData)
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

			if(isForceDownloadingNewData)
				isNeedUpdate = true;

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

			var boyarkaCategoriesIds = GetUsedCategories(ostatkiPriceList.ProductLineList, productVariantList).Select(x => x.Id).ToList();

			var totalProducts = productVariantList.Count;
			var productsToSell = 0;
			var productsToSellInBoyarka = 0;


			this._logger.Debug("     Updating products...");

			foreach (var currentProductVariant in productVariantList)
			{
				var productInF5Price = f5PriceList.ProductLineList.SingleOrDefault(x => x.Articul == currentProductVariant.Sku);
				var isProductInF5Price = productInF5Price != null;

				if (!isProductInF5Price)
				{
					// 1. Сбрасываем все цены и флаги в 0	
					if (currentProductVariant.Price != 0 || currentProductVariant.Published != false)
					{
						currentProductVariant.Price = 0;
						currentProductVariant.Published = false;
						currentProductVariant.AdminComment = "";
					}
					continue;
				}

				var productInBoyarka = ostatkiPriceList.ProductLineList.SingleOrDefault(x => x.Articul == currentProductVariant.Sku);
				var isProductInBoyarkaPrice = productInBoyarka != null;

				var productInVendor = ugCatalogPriceList.ProductLineList.SingleOrDefault(x => x.Articul == currentProductVariant.Sku);
				var isProductInVendorPrice = productInVendor != null;
				

				//  2. Если Товар есть у поставщика или в Боярке - ставим цену				
				if (isProductInVendorPrice || isProductInBoyarkaPrice)
				{
					decimal appliedTax;
					string priceCalcInfo;

					var productCategoryId = currentProductVariant.Product.ProductCategories.ToList()[0].CategoryId;

					var price = CalculatePrice(
						productCategoryId, 
						productInVendor, isProductInVendorPrice,
						productInF5Price, 
						productInBoyarka, isProductInBoyarkaPrice,
						boyarkaCategoriesIds,
						out appliedTax,
						out priceCalcInfo);

					if (currentProductVariant.Price != price || currentProductVariant.Published != true || currentProductVariant.AdminComment != priceCalcInfo)
					{
						currentProductVariant.Price = price;
						currentProductVariant.Published = true;
						currentProductVariant.AdminComment = priceCalcInfo;
					}
					productsToSell++;
				}
				else
				{
					// 1. Сбрасываем все цены и флаги в 0	
					if (currentProductVariant.Price != 0 || currentProductVariant.Published != false)
					{
						currentProductVariant.Price = 0;
						currentProductVariant.Published = false;
						currentProductVariant.AdminComment = "";
					}
					continue;
				}

				// 3. Если товар есть у поставщика, то ставим предзаказ "да"
				if (isProductInVendorPrice)
				{
					if(currentProductVariant.AvailableForPreOrder != true)
						currentProductVariant.AvailableForPreOrder = true;
				}

				// 4. Если товар есть в Боярке, то ставим предзаказ "нет" и Доставка сейчас "Да"
				if (isProductInBoyarkaPrice)
				{
					if (currentProductVariant.AvailableForPreOrder != false)
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

		private decimal CalculatePrice(int productCategoryId, 
						ProductLine productInVendor, bool isProductInVendorPrice,
						ProductLineVendor productInF5Price,
						ProductLine productInBoyarka, bool isProductInBoyarkaPrice, 
						IEnumerable<int> boyarkaCategoriesIds,
						out decimal appliedTaxPercent,
						out string priceCalcInfo)
		{
			decimal returnPrice = 0;
			

			decimal onePercentPrice = productInF5Price.PriceBase / 100;
			decimal f5Percent = productInF5Price.PriceDiff / onePercentPrice;
			f5Percent = decimal.Round(f5Percent, 2, MidpointRounding.AwayFromZero);
			const decimal MyDiscontMnozhitelForCategoriesInBoyarka = 0.66m; // скидываем от цены в Наташином магазине (множитель на Наташину наценку)
			const decimal MyDiscontMnozhitelForCategoriesInBoyarkaForNotExistProducts = 0.45m; // скидываем от цены в Наташином магазине (множитель на Наташину наценку)
			const decimal MyPercentsForCategoriesNotInBoyarka = 5; // мой процент наценки для товаров, которыми Боярка не занимается
			const decimal MyNatashasFeePercent = 2;// процент базовой стоимости, который отчисляется Наташе
			//const decimal MyMinFee = 15;//
			const int MinPrice = 5;

			var myBasePrice = productInF5Price.PriceBase;
			
			myBasePrice = myBasePrice > MinPrice ? myBasePrice : MinPrice; // товара дешевле 5 гривен быть не должно

			appliedTaxPercent = 0;

			bool isboyarkaCategory = true;

			// Товар есть в Боярке?
			if (isProductInBoyarkaPrice)
			{
				appliedTaxPercent = f5Percent * MyDiscontMnozhitelForCategoriesInBoyarka;				
			}
			else if(isProductInVendorPrice) // Товара нет в Бояке, но есть в Киеве
			{				
				isboyarkaCategory = boyarkaCategoriesIds.Contains(productCategoryId);

				// Хотя товара нет, но Товарная категория Боярская (ноуты)
				if (isboyarkaCategory)
				{
					appliedTaxPercent = f5Percent * MyDiscontMnozhitelForCategoriesInBoyarkaForNotExistProducts;										
				}
				else // Товарная категория в Боярке не продается (холодильники)
				{
					appliedTaxPercent = MyPercentsForCategoriesNotInBoyarka;
				}
			}
			else
				throw new Exception("my Strange situation");

			// Вычисляем цену c учетом наценки
			var totalFee = myBasePrice * (appliedTaxPercent / 100);
			returnPrice = myBasePrice + totalFee;


			var natashasFee = productInF5Price.PriceBase * (MyNatashasFeePercent / 100); // это 2 процента от конечной стоимости 
			var myFee = totalFee - natashasFee;


			//if (myFee < MyMinFee)// если моя прибыль меньше порога, то увеличиваем цену и пересчитваем общую наценку appliedTax
			//{
			//	var needToAdd = MyMinFee - myFee;
			//	returnPrice += needToAdd;
			//	// decimal f5Percent = productInF5Price.PriceDiff / onePercentPrice;
			//	appliedTaxPercent = (returnPrice - productInF5Price.PriceBase) / onePercentPrice;
			//}


			priceCalcInfo =  "Есть в Боярке = " + isProductInBoyarkaPrice + "<br/>"				
				+ "Категория продается в Боярке = " + isboyarkaCategory + "<br/>"
				+ "Закупочная цена = " + productInF5Price.PriceBase.ToString("#.## грн") + "<br/>"
				+ "Моя базовая цена (для оччччень дешевых товаров до ~5 грн) = " + myBasePrice.ToString("#.## грн") + "<br/>"
				+ "Процент Наташе = " + MyNatashasFeePercent.ToString("#.##") + "%<br/>"
				+ "Отчисления Наташе = " + natashasFee.ToString("#.## грн") + "<br/>"
				+ "Витринная цена F5 = " + productInF5Price.PriceRaschet.ToString("#.## грн") + "<br/>"				
				+ "Процент выставленный на витрине F5= " + f5Percent.ToString("#.##") + "%<br/><br/>"
				+ "Мой множитель на Процент выставленный на витрине F5 для присутсвующих товаров = " + MyDiscontMnozhitelForCategoriesInBoyarka.ToString("#.##") + "<br/><br/>"
				+ "Мой множитель на Процент выставленный на витрине F5 для отсутсвующих сейчас товаров = " + MyDiscontMnozhitelForCategoriesInBoyarkaForNotExistProducts.ToString("#.##") + "<br/><br/>"
				+ "Процент для непродающихся в Боярке вообще категорий товаров = " + MyPercentsForCategoriesNotInBoyarka.ToString("#.##") + "%<br/><br/>"
				+ "Итог: Вычесленный процент для этого товара = " + appliedTaxPercent.ToString("#.##") + "%<br/>"
				+ "Общая прибыль = " + totalFee.ToString("#.## грн") + "<br/>"
				+ "Моя прибыль = " + myFee.ToString("#.## грн") + "<br/>";

			return returnPrice;
		}
	}
}
