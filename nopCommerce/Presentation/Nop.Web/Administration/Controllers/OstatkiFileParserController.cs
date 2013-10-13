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
		private readonly ILogger _logger;
		private readonly IOstatkiPriceParserService _ostatkiPriceParserService;
		private readonly IPriceManagerService _priceManagerService;
		private readonly IYugCatalogPriceParserService _yugCatalogPriceParserService;

		public OstatkiFileParserController(IProductService productService, ILogger logger, 
			IOstatkiPriceParserService ugContractPriceParserService,
			IPriceManagerService priceManagerService,
			 IYugCatalogPriceParserService yugCatalogPriceParserService)
		{			
			_logger = logger;
			_ostatkiPriceParserService = ugContractPriceParserService;		
			_yugCatalogPriceParserService = yugCatalogPriceParserService;
			_priceManagerService = priceManagerService;
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
			var msgResult = _priceManagerService.ApplyImportAll();
			return Content(msgResult);
		}

		[HttpPost]
		public ActionResult ApplyImport()
		{
			var msg = _ostatkiPriceParserService.SetExistingInBoyarka(true);
			return Content(msg);			
		}		
	}
}
