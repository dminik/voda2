namespace Nop.Admin.Controllers
{
	using System;
	using System.Linq;
	using System.Web.Mvc;

	using Nop.Core.Infrastructure;
	using Nop.Services.Catalog;
	using Nop.Services.Logging;
	using Nop.Services.SiteParsers;
	using Nop.Services.SiteParsers.Xls;
	using Nop.Services.Tasks;
	using Nop.Web.Framework.Controllers;


	[AdminAuthorize]
	public class OstatkiFileParserController : Controller
	{
		private readonly IProductService _productService;
		private readonly ILogger _logger;
		private readonly IOstatkiPriceParserService _ostatkiPriceParserService;
		private readonly IF5PriceParserService _f5PriceParserService;
		private readonly IYugCatalogPriceParserService _yugCatalogPriceParserService;

		public OstatkiFileParserController(IProductService productService, ILogger logger, 
			IOstatkiPriceParserService ugContractPriceParserService,
			IF5PriceParserService f5PriceParserService,
			 IYugCatalogPriceParserService yugCatalogPriceParserService)
		{
			_productService = productService;
			_logger = logger;
			_ostatkiPriceParserService = ugContractPriceParserService;
			_f5PriceParserService = f5PriceParserService;
			_yugCatalogPriceParserService = yugCatalogPriceParserService;
		}

		public ActionResult OstatkiFileParser()
		{			
			return View();
		}
		
		[HttpPost]
		public ActionResult ParseAndShow()
		{
			var list = _ostatkiPriceParserService.ParseAndShow(true);
			return Json(list);
		}

		[HttpPost]
		public ActionResult ParseAndShowyUgCatalog()
		{
			var list = _yugCatalogPriceParserService.ParseAndShow(true);
			return Json(list);
		}

		[HttpPost]
		public ActionResult ApplyImportAll()
		{			
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
				}
			}
			else
				isNeedUpdate = true;


			string notFoundArticules = "";
			string foundArticules = "";
			int successCounter = 0;

			/*
			 * Порядок проставки флагов и цен
			 * 1. Сбрасываем все цены и флаги в 0
			 * 2. Если Товар есть у поставщика или в Боярке - ставим цену
			 * 2. Если товар есть у поставщика, то ставим предзаказ "да"
			 * 2. Если товар есть в Боярке, то ставим предзаказ "нет" и Доставка сейчас "Да"
			 * 
			 * 
			                     Товара нет нигде               Товар есть у поставщика               Товар в Боярке  
			 цена           0 (не показываем его на сайте)       100                                         100		
			 предзаказ              да                           да                                          нет			
		     доставка сейчас        нет                          нет                                          да
			 
			*/

			var productVariantList = _productService.GetProductVariants().ToList();

			var f5PriceList = _f5PriceParserService.ParseAndShow(isNeedUpdate);
			var ostatkiPriceList = _ostatkiPriceParserService.ParseAndShow(isNeedUpdate);
			var ugCatalogPriceList = _yugCatalogPriceParserService.ParseAndShow(isNeedUpdate);

			foreach (var currentProductVariant in productVariantList)
			{
				var curF5ProductLine = f5PriceList.ProductLineList.SingleOrDefault(x => x.Articul == currentProductVariant.Sku);
				var isProductInF5PriceList = curF5ProductLine != null;

				if (!isProductInF5PriceList)
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
				foundArticules += curF5ProductLine.Articul + ", ";
			}

			if (productVariantList.Count > 0) // Вызываем сохранение всего контекста (всех вариантов)
				_productService.UpdateProductVariant(productVariantList[0]);



			return Content("Success!");
		}

		[HttpPost]
		public ActionResult ApplyImport()
		{
			var msg = _ostatkiPriceParserService.SetExistingInBoyarka(true);
			return Content(msg);			
		}		
	}
}
