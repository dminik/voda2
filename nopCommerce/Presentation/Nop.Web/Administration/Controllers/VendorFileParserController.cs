namespace Nop.Admin.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Web.Helpers;
	using System.Web.Mvc;

	using Nop.Admin.Models.FileParser;	
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
	public class VendorFileParserController : Controller
	{
		private readonly IProductService _productService;

		public VendorFileParserController(IProductService productService)
		{
			_productService = productService;			
		}

		public ActionResult VendorFileParser()
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

			_productService.ClearProductVariantsPrice(); // reset  all

			// set from from priceList
			foreach (var curProductLine in newSpecsOnly.ProductLineList) 
			{
				var currentProductVariant = _productService.GetProductVariantBySku(curProductLine.Articul);
				if (currentProductVariant == null)
				{
					notFoundArticules += curProductLine.Articul + ", "; // it is very new products, not exist in our shop db
					continue;
				}

				var isNeedUpdate = false;

				if (currentProductVariant.StockQuantity != 0)
				{
					currentProductVariant.StockQuantity = 0; 
					isNeedUpdate = true;
				}

				if (currentProductVariant.Price != curProductLine.PriceRaschet)
				{
					currentProductVariant.Price = curProductLine.PriceRaschet;
					isNeedUpdate = true;
				}

				if (currentProductVariant.AvailableForPreOrder != true)
				{
					currentProductVariant.AvailableForPreOrder = true;
					isNeedUpdate = true;
				}
				
				if (isNeedUpdate)
					_productService.UpdateProductVariant(currentProductVariant);

				successCounter++;
				foundArticules += curProductLine.Articul + ", ";
			}

			if (notFoundArticules == "")
				return Content("Success for all!");
			else 
			{
				return Content("Success for " + successCounter + " from " + newSpecsOnly.ProductLineList.Count() 
					+ ". Not Found Articules in shop but they exist in file Vendor:" + notFoundArticules
					+ ". Success Articules" + foundArticules);
			}
		}



		private VendorFileParserModel _Parse()
		{
			string filePath = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProductsCatalog"), "shop_f5.txt");
			var resultModel = new VendorFileParserModel(); 
			List<string> errors;
			resultModel.ProductLineList = FileParserVendor.ParseFile(filePath, out errors);
			resultModel.ErrorList = errors;

			return resultModel;
;
		}
	}
}
