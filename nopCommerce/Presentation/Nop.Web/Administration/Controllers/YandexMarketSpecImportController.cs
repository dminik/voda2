namespace Nop.Admin.Controllers
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Helpers;
	using System.Web.Mvc;

	using Nop.Admin.Models.YandexMarket;
	using Nop.Core.Domain.Catalog;
	using Nop.Core.Domain.YandexMarket;
	using Nop.Core.Infrastructure;
	using Nop.Services.Catalog;
	using Nop.Services.Logging;
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
		private readonly ILogger _logger;

		public YandexMarketSpecImportController(
			IYandexMarketSpecService yandexMarketSpecService, 
			IYandexMarketCategoryService yandexMarketCategoryService,
			ISpecificationAttributeService specificationAttributeService,
			ILogger logger)
		{
			_yandexMarketSpecService = yandexMarketSpecService;
			_yandexMarketCategoryService = yandexMarketCategoryService;
			_specificationAttributeService = specificationAttributeService;
			_logger = logger;
		}



		public ActionResult YandexMarketSpecImport()
		{
			var model = new YandexMarketSpecImportModel();
			model.AvailableCategories = _yandexMarketCategoryService.GetCategoriesForDDL();
			model.ParserCategoryId = 0;

			return View(model);
		}
		
		[HttpPost]
		public ActionResult GetNewSpecs()
		{
			var activeParserCategoriesIdList = _yandexMarketCategoryService.GetActive().Select(x => x.Id);
			var newSpecsOnly = _GetNewSpecs(activeParserCategoriesIdList);

			foreach (var specificationAttribute in newSpecsOnly)
			{
				specificationAttribute.Order();
			}
			
			
			return Json(newSpecsOnly);
		}

		[HttpPost]
		public ActionResult GetAllSpecs()
		{
			var activeParserCategoriesIdList = _yandexMarketCategoryService.GetActive().Select(x => x.Id);
			//var newSpecsOnly = _GetNewSpecs(activeParserCategoriesIdList);

			var mapper = new YandexMarketSpecMapper(_yandexMarketSpecService, EngineContext.Current.Resolve<ISpecificationAttributeService>());

			var allSpecs = mapper.GetSumOfYandexSpecsAndShopSpecs(activeParserCategoriesIdList);

			bool any = false;
			var resultNewSpecsOnly = new List<SpecificationAttribute>();

			foreach (SpecificationAttribute y in allSpecs)
			{
				if (y.SpecificationAttributeOptions.Any(r => r.DisplayOrder == 777))
				{
					resultNewSpecsOnly.Add(y);
				}
			}
			

			foreach (var specificationAttribute in resultNewSpecsOnly)
			{
				specificationAttribute.Order();
			}

			return Json(resultNewSpecsOnly);
		}

		[HttpPost]
		public ActionResult ApplyImport()
		{
			_logger.Debug("--- ApplyImport START...");
			var activeParserCategoriesIdList = _yandexMarketCategoryService.GetActive().Select(x => x.Id);
			var newSpecsOnly = _GetNewSpecs(activeParserCategoriesIdList);

			_logger.Debug("Will be imported specs: " + newSpecsOnly.Count);

			var importedCounter = 0;
			foreach (var curSpecAttr in newSpecsOnly)
			{
				if (curSpecAttr.Id != 0)
				{
					SpecificationAttribute curSpecAttrFromDb = _specificationAttributeService.GetSpecificationAttributeById(curSpecAttr.Id);

					foreach (var curSpecAttrOpt in curSpecAttr.SpecificationAttributeOptions)
					{
						curSpecAttrOpt.SpecificationAttributeId = curSpecAttrFromDb.Id;
						if (!curSpecAttrFromDb.SpecificationAttributeOptions.Any(x => x.Name == curSpecAttrOpt.Name))
						{
							curSpecAttrOpt.DisplayOrder = 0; // сбрасываем с 777
							_specificationAttributeService.InsertSpecificationAttributeOption(curSpecAttrOpt);
						}
					}
				}
				else
				{
					_specificationAttributeService.InsertSpecificationAttribute(curSpecAttr);					
				}
				importedCounter++;

				if(importedCounter%5 == 0) // через каждые 5 записей выводить в лог сообщение
					_logger.Debug("Imported specs: " + importedCounter + "...");
			}

			_logger.Debug("--- ApplyImport End.");

			return Content("Success!");
		}



		private List<SpecificationAttribute> _GetNewSpecs(IEnumerable<int> parserCategoryIdList)
		{
			var mapper = new YandexMarketSpecMapper(_yandexMarketSpecService, EngineContext.Current.Resolve<ISpecificationAttributeService>());

			var allSpecs = mapper.GetSumOfYandexSpecsAndShopSpecs(parserCategoryIdList);
			var newSpecsOnly = mapper.GetNewYandexSpecsOnly(allSpecs);

			return newSpecsOnly;
		}
	}
}
