namespace Nop.Admin.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Web.Helpers;
	using System.Web.Mvc;

	using Nop.Admin.Models.OstatkiFileParser;
	using Nop.Admin.Models.YandexMarket;
	using Nop.Core.Domain.Catalog;
	using Nop.Core.Domain.YandexMarket;
	using Nop.Core.Infrastructure;
	using Nop.Services.Catalog;
	using Nop.Services.FileParsers;
	using Nop.Services.SiteParsers;
	using Nop.Services.YandexMarket;
	using Nop.Web.Framework.Controllers;

	using Telerik.Web.Mvc;

	[AdminAuthorize]
	public class OstatkiFileParserController : Controller
	{
		private readonly IYandexMarketSpecService _yandexMarketSpecService;
		private readonly ISpecificationAttributeService _specificationAttributeService;
		private readonly IYandexMarketCategoryService _yandexMarketCategoryService;

		public OstatkiFileParserController(
			IYandexMarketSpecService yandexMarketSpecService, 
			IYandexMarketCategoryService yandexMarketCategoryService,
			ISpecificationAttributeService specificationAttributeService)
		{
			_yandexMarketSpecService = yandexMarketSpecService;
			_yandexMarketCategoryService = yandexMarketCategoryService;
			_specificationAttributeService = specificationAttributeService;
		}



		public ActionResult OstatkiFileParser()
		{			
			return View();
		}
		
		[HttpPost]
		public ActionResult ParseAndShow()
		{
			var newSpecsOnly = _Parse();
			return Json(newSpecsOnly);
		}

		//[HttpPost]
		//public ActionResult ApplyImport(int parserCategoryId)
		//{
		//	var newSpecsOnly = _GetNewSpecs(parserCategoryId);

		//	foreach (var curSpecAttr in newSpecsOnly)
		//	{
		//		SpecificationAttribute curSpecAttrFromDb;
		//		if (curSpecAttr.Id != 0)
		//		{
		//			curSpecAttrFromDb = _specificationAttributeService.GetSpecificationAttributeById(curSpecAttr.Id);

		//			foreach (var curSpecAttrOpt in curSpecAttr.SpecificationAttributeOptions)
		//			{
		//				curSpecAttrOpt.SpecificationAttributeId = curSpecAttrFromDb.Id;
		//				_specificationAttributeService.InsertSpecificationAttributeOption(curSpecAttrOpt);
		//				break;
		//			}
		//		}
		//		else
		//		{
		//			_specificationAttributeService.InsertSpecificationAttribute(curSpecAttr);					
		//		}
		//	}

		//	return Content("Success!");
		//}



		private OstatkiFileParserModel _Parse()
		{
			string filePath = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProductsCatalog"), "Остатки.txt");
			var resultModel = new OstatkiFileParserModel(); 
			List<string> errors;
			resultModel.ProductLineList = Nop.Services.FileParsers.OstatkiFileParser.ParseOstatkiFile(filePath, out errors);
			resultModel.ErrorList = errors;

			return resultModel;
;
		}
	}
}
