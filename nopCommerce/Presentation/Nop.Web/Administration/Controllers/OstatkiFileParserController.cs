namespace Nop.Admin.Controllers
{
	using System;
	using System.Linq;
	using System.Web.Mvc;

	using Nop.Admin.Models.YandexMarket;
	using Nop.Core.Infrastructure;
	using Nop.Services.Catalog;
	using Nop.Services.Logging;
	using Nop.Services.SiteParsers;
	using Nop.Services.SiteParsers.Xls;
	using Nop.Services.Tasks;
	using Nop.Services.YandexMarket;
	using Nop.Web.Framework.Controllers;


	[AdminAuthorize]
	public class OstatkiFileParserController : Controller
	{		
		private readonly ILogger _logger;
		private readonly IOstatkiPriceParserService _ostatkiPriceParserService;
		private readonly ISpecialPriceService _specialPriceService;
		private readonly IPriceManagerService _priceManagerService;
		private readonly IYugCatalogPriceParserService _yugCatalogPriceParserService;
		private readonly IProductService _productService;

		public OstatkiFileParserController(
			IProductService productService, 
			ILogger logger, 
			IOstatkiPriceParserService ugContractPriceParserService,
			IPriceManagerService priceManagerService,
			IYugCatalogPriceParserService yugCatalogPriceParserService,
			ISpecialPriceService specialPriceService)
		{
			_productService = productService;
			_logger = logger;
			_ostatkiPriceParserService = ugContractPriceParserService;		
			_yugCatalogPriceParserService = yugCatalogPriceParserService;
			_priceManagerService = priceManagerService;
			_specialPriceService = specialPriceService;
		}

		public ActionResult OstatkiFileParser()
		{
			return View(new OstatkiFileParserModel());
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
		public ActionResult ShowProductsFromOstatkiWhichNotExistsInShop()
		{
			var listBoyarka = _ostatkiPriceParserService.ParseAndShow(false).ProductLineList.ToList();
			var listBoyarkaSku = listBoyarka.Select(x => x.Articul).ToList();




			// var productsVariantsSku = _productService.SearchProducts().SelectMany(x => x.ProductVariants).Select(s => s.Sku);

			var productsVariantsSku = _productService.SearchProductVariants(0, 0, 0, "", false, 0, 999999).Select(s => s.Sku).ToList();

			var orfinedBoyarkaSku = listBoyarkaSku.Where(s => !productsVariantsSku.Contains(s)).ToList();


			var notFoundProducts = listBoyarka.Where(s => orfinedBoyarkaSku.Contains(s.Articul)).Select(x => x.Articul + " " + x.Name);

			return Json(notFoundProducts);
		}
		
		[HttpPost]
		public ActionResult ApplyImport()
		{
			var msg = _ostatkiPriceParserService.SetExistingInBoyarka(true);
			return Content(msg);			
		}

		[HttpPost]
		public ActionResult ApplyImportAll(OstatkiFileParserModel model)
		{
			var msgResult = _priceManagerService.ApplyImportAll(model.IsForceDownloadingNewData);
			return Content(msgResult);
		}

		[HttpPost]
		public ActionResult ParseAndShowSpecialPrice()
		{
			var list = _specialPriceService.GetAll(true);
			return Json(list);
		}

	}
}
