namespace Nop.Admin.Controllers
{
	using System.Linq;
	using System.Web.Mvc;

	using Nop.Admin.Models.YandexMarket;
	using Nop.Core.Domain.YandexMarket;
	using Nop.Core.Infrastructure;
	using Nop.Services.Catalog;
	using Nop.Services.SiteParsers;
	using Nop.Services.YandexMarket;
	using Nop.Web.Framework.Controllers;

	using Telerik.Web.Mvc;

	[AdminAuthorize]
	public class YandexMarketSpecImportController : Controller
	{
		private readonly IYandexMarketCategoryService _yandexMarketCategoryService;
		private readonly IYandexMarketSpecService _yandexMarketSpecService;

		public YandexMarketSpecImportController(
			IYandexMarketSpecService yandexMarketSpecService, 
			IYandexMarketCategoryService yandexMarketCategoryService)
		{
			_yandexMarketSpecService = yandexMarketSpecService;
			_yandexMarketCategoryService = yandexMarketCategoryService;
		}



		public ActionResult YandexMarketSpecImport()
		{
			var model = new YandexMarketSpecImportModel();
			model.AvailableCategories = _yandexMarketCategoryService.GetCategoriesForDDL();
			model.CategoryId = 0;

			return View(model);
		}
		
		[HttpPost]
		public ActionResult GetNewSpecs(int categoryId)
		{
			var mapper = new YandexMarketSpecMapper(_yandexMarketSpecService, EngineContext.Current.Resolve<ISpecificationAttributeService>());

			var allSpecs = mapper.GetSumOfYandexSpecsAndShopSpecs(categoryId);
			var newSpecsOnly = mapper.GetNewYandexSpecsOnly(allSpecs);

			return Json(newSpecsOnly);
		}
	}
}
