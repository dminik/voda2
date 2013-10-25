namespace Nop.Admin.Controllers
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Mvc;

	using Nop.Admin.Models.YandexMarket;
	using Nop.Core;
	using Nop.Core.Domain.YandexMarket;
	using Nop.Services.Catalog;
	using Nop.Services.Logging;
	using Nop.Services.SiteParsers;
	using Nop.Services.SiteParsers.Categories;
	using Nop.Services.YandexMarket;
	using Nop.Web.Framework;
	using Nop.Web.Framework.Controllers;

	using Telerik.Web.Mvc;

	[AdminAuthorize]
	public class YandexMarketParserCategoryController : Controller
	{
		private static bool mIsStopParsing = false;
		private readonly IYandexMarketCategoryService _yandexMarketCategoryService;
		private readonly ICategoryService _shopCategoryService;
		private readonly IProductService _productService;
		 private readonly IStoreContext _storeContext;
		 private readonly ILogger _logger;

		 public YandexMarketParserCategoryController(
			IYandexMarketCategoryService yandexMarketCategoryService, 
			ICategoryService shopCategoryService,
			IProductService productService,
			IStoreContext storeContext,
			ILogger logger)
		{
			this._yandexMarketCategoryService = yandexMarketCategoryService;
			_shopCategoryService = shopCategoryService;
			this._productService = productService;
			_storeContext = storeContext;
			_logger = logger;
		}

		 public ActionResult YandexMarketParserCategory()
		 {			 
			 return View();
		 }

		[HttpPost]
		public ActionResult CategoryParseAndShow(string urlCategoryForParsing)
		{
			mIsStopParsing = false;

			_logger.Debug("--- CategoryParse START...");
			
			var parser = BaseParserCategories.Create(urlCategoryForParsing, _logger, _yandexMarketCategoryService);
			var newCategoryList = parser.Parse(ref mIsStopParsing);
			
			_logger.Debug("+++ CategoryParse  DONE.");

			return Json(newCategoryList);
		}

		[HttpPost]
		public ActionResult ParseStop()
		{
			mIsStopParsing = true;
			return Content("Success");
		}
	}
}
