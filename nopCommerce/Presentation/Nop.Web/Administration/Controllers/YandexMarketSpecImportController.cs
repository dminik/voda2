namespace Nop.Admin.Controllers
{
	using System;
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
		private static bool mIsStopProducsImport = false;
		private readonly IYandexMarketSpecService _yandexMarketSpecService;
		private readonly ISpecificationAttributeService2 _specificationAttributeService;
		private readonly IYandexMarketCategoryService _yandexMarketCategoryService;
		private readonly ILogger _logger;

		public YandexMarketSpecImportController(
			IYandexMarketSpecService yandexMarketSpecService, 
			IYandexMarketCategoryService yandexMarketCategoryService,
			ISpecificationAttributeService2 specificationAttributeService,
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

		/// <summary>
		/// All new hilighted specs with old specs
		/// </summary>		
		[HttpPost]
		public ActionResult GetAllSpecs()
		{
			var activeParserCategoriesIdList = _yandexMarketCategoryService.GetActive().Select(x => x.Id);
			//var newSpecsOnly = _GetNewSpecs(activeParserCategoriesIdList);

			var mapper = new YandexMarketSpecMapper(_yandexMarketSpecService, EngineContext.Current.Resolve<ISpecificationAttributeService>());

			var allSpecs = mapper.GetSumOfYandexSpecsAndShopSpecs(activeParserCategoriesIdList);
			
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
		public ActionResult ApplyImportStop()
		{
			mIsStopProducsImport = true;
			return Content("Success");
		}
		
		[HttpPost]
		public ActionResult ApplyImport()
		{
			mIsStopProducsImport = false;

			_logger.Debug("--- ApplyImport START...");
			var activeParserCategoriesIdList = _yandexMarketCategoryService.GetActive().Select(x => x.Id);
			var newSpecsOnly = _GetNewSpecs(activeParserCategoriesIdList);

			_logger.Debug("Will be imported specs: " + newSpecsOnly.Count);

			var importedCounter = 0;


			foreach (var specAttrOpt in newSpecsOnly.SelectMany(x => x.SpecificationAttributeOptions).ToList())
			{
				CheckStopAction();

				specAttrOpt.DisplayOrder = 0;// сбрасываем с 777
			}
			

			foreach (var curSpecAttr in newSpecsOnly)
			{
				CheckStopAction();

				if (curSpecAttr.Id != 0)
				{
					SpecificationAttribute curSpecAttrFromDb = _specificationAttributeService.GetSpecificationAttributeById(curSpecAttr.Id);

					var specOptionsToInsert = new List<SpecificationAttributeOption>();

					foreach (var curSpecAttrOpt in curSpecAttr.SpecificationAttributeOptions)
					{
						CheckStopAction();

						curSpecAttrOpt.SpecificationAttributeId = curSpecAttrFromDb.Id;
						if (curSpecAttrFromDb.SpecificationAttributeOptions.All(x => x.Name != curSpecAttrOpt.Name))
						{
							specOptionsToInsert.Add(curSpecAttrOpt);							
						}
					}
					_specificationAttributeService.InsertSpecificationAttributeOptionList(specOptionsToInsert);// save all at once
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

		private void CheckStopAction()
		{
			if (mIsStopProducsImport)
			{
				var msg = "Stopped by user.";
				_logger.Debug(msg);
				throw new Exception(msg);
			}
		}
	}
}
