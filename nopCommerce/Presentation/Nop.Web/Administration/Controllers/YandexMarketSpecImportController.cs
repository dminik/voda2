namespace Nop.Admin.Controllers
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Mvc;

	using Nop.Admin.Models.YandexMarket;
	using Nop.Core.Domain.Catalog;
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
		private readonly IYandexMarketSpecService _yandexMarketSpecService;
		private readonly ISpecificationAttributeService _specificationAttributeService;
		private readonly IYandexMarketCategoryService _yandexMarketCategoryService;

		public YandexMarketSpecImportController(
			IYandexMarketSpecService yandexMarketSpecService, 
			IYandexMarketCategoryService yandexMarketCategoryService,
			ISpecificationAttributeService specificationAttributeService)
		{
			_yandexMarketSpecService = yandexMarketSpecService;
			_yandexMarketCategoryService = yandexMarketCategoryService;
			_specificationAttributeService = specificationAttributeService;
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
			var newSpecsOnly = _GetNewSpecs(categoryId);
			return Json(newSpecsOnly);
		}

		[HttpPost]
		public ActionResult ApplyImport(int categoryId)
		{
			var newSpecsOnly = _GetNewSpecs(categoryId);

			foreach (var curSpecAttr in newSpecsOnly)
			{
				SpecificationAttribute curSpecAttrFromDb;
				if (curSpecAttr.Id != 0)
				{
					curSpecAttrFromDb = _specificationAttributeService.GetSpecificationAttributeById(curSpecAttr.Id);

					foreach (var curSpecAttrOpt in curSpecAttr.SpecificationAttributeOptions)
					{
						curSpecAttrOpt.SpecificationAttributeId = curSpecAttrFromDb.Id;
						_specificationAttributeService.InsertSpecificationAttributeOption(curSpecAttrOpt);
						break;
					}
				}
				else
				{
					_specificationAttributeService.InsertSpecificationAttribute(curSpecAttr);					
				}

				
				
			}
			
			return new EmptyResult();
		}



		private List<SpecificationAttribute> _GetNewSpecs(int categoryId)
		{
			var mapper = new YandexMarketSpecMapper(_yandexMarketSpecService, EngineContext.Current.Resolve<ISpecificationAttributeService>());

			var allSpecs = mapper.GetSumOfYandexSpecsAndShopSpecs(categoryId);
			var newSpecsOnly = mapper.GetNewYandexSpecsOnly(allSpecs);

			return newSpecsOnly;
		}
	}
}
