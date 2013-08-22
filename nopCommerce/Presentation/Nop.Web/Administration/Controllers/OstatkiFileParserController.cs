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
		private readonly IProductService _productService;

		public OstatkiFileParserController(
			IYandexMarketSpecService yandexMarketSpecService, 
			IYandexMarketCategoryService yandexMarketCategoryService,
			ISpecificationAttributeService specificationAttributeService,
			IProductService productService)
		{
			_productService = productService;
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

		[HttpPost]
		public ActionResult ApplyImport()
		{
			var newSpecsOnly = _Parse();
			string notFoundArticules = "";
			string foundArticules = "";
			int successCounter = 0;

			foreach (var curProductLine in newSpecsOnly.ProductLineList)
			{
				var currentProductVariant = _productService.GetProductVariantBySku(curProductLine.Articul);
				if (currentProductVariant == null)
				{
					notFoundArticules += curProductLine.Articul + ", ";
					continue;
				}

				currentProductVariant.StockQuantity = curProductLine.Amount;
				currentProductVariant.Price = curProductLine.Price;
				_productService.UpdateProductVariant(currentProductVariant);
				successCounter++;
				foundArticules += curProductLine.Articul + ", ";
			}

			if (notFoundArticules == "")
				return Content("Success for all!");
			else 
			{
				return Content("Success for " + successCounter + " from " + newSpecsOnly.ProductLineList.Count() 
					+ ". Not Found Articules in shop but they exist in file Ostatki:" + notFoundArticules
					+ ". Success Articules" + foundArticules);
			}
		}



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
