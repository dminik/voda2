namespace Nop.Admin.Controllers
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Mvc;

	using Nop.Admin.Models.YandexMarket;
	using Nop.Core;
	using Nop.Core.Domain.YandexMarket;
	using Nop.Services.Catalog;
	using Nop.Services.YandexMarket;
	using Nop.Web.Framework;
	using Nop.Web.Framework.Controllers;

	using Telerik.Web.Mvc;

	[AdminAuthorize]
	public class YandexMarketParserCategoryController : Controller
	{
		private readonly IYandexMarketCategoryService _yandexMarketCategoryService;
		private readonly ICategoryService _shopCategoryService;
		private readonly IProductService _productService;
		 private readonly IStoreContext _storeContext;

		 public YandexMarketParserCategoryController(
			IYandexMarketCategoryService yandexMarketCategoryService, 
			ICategoryService shopCategoryService,
			IProductService productService,
			IStoreContext storeContext)
		{
			this._yandexMarketCategoryService = yandexMarketCategoryService;
			_shopCategoryService = shopCategoryService;
			this._productService = productService;
			_storeContext = storeContext;
		}

		 public ActionResult YandexMarketParserCategory()
		 {			 
			 return View();
		 }

		[HttpPost]
		public ActionResult CategoryParseAndShow(string urlCategoryForParsing)
		{
			//_yandexMarketCategoryService.SetActiveAllParserCategoties(isActive);
			return Json(new { Hehe="NeHehe" });
		}
	}
}
