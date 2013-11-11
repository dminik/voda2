namespace Nop.Admin.Controllers
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Mvc;

	using Nop.Services.Catalog;
	using Nop.Services.Logging;
	using Nop.Services.SiteParsers;
	using Nop.Web.Framework.Controllers;


	[AdminAuthorize]
	public class VendorFileParserController : Controller
	{
		private readonly IProductService _productService;

		private readonly ILogger _logger;
		private readonly IF5PriceParserService _f5PriceParserService;

		public VendorFileParserController(IProductService productService, ILogger logger, IF5PriceParserService f5PriceParserService)
		{
			_productService = productService;
			_logger = logger;
			this._f5PriceParserService = f5PriceParserService;
		}

		public ActionResult VendorFileParser()
		{			
			return View();
		}
		
		[HttpPost]
		public ActionResult ParseAndShow()
		{			
			var list = _f5PriceParserService.ParseAndShow(true);
			return Json(list);
		}

		[HttpPost]
		public ActionResult ApplyImport()
		{
			var msg = _f5PriceParserService.SetVendorPrices(true);
			return Content(msg);			
		}
	}
}
