﻿namespace Nop.Admin.Controllers
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Mvc;

	using Nop.Admin.Models.FileParser;
	using Nop.Services.Catalog;
	using Nop.Services.Logging;
	using Nop.Services.SiteParsers;
	using Nop.Web.Framework.Controllers;


	[AdminAuthorize]
	public class VendorFileParserController : Controller
	{
		private readonly IProductService _productService;

		private readonly ILogger _logger;

		public VendorFileParserController(IProductService productService, ILogger logger)
		{
			_productService = productService;
			_logger = logger;
		}

		public ActionResult VendorFileParser()
		{			
			return View();
		}
		
		[HttpPost]
		public ActionResult ParseAndShow()
		{
			var parser = new F5PriceParser();
			List<string> errors;

			var list = parser.GetPriceListFromCache(_logger, out errors, true);

			var newSpecsOnly = new VendorFileParserModel { ProductLineList = list, ErrorList = errors };
			
			return Json(newSpecsOnly);
		}

		[HttpPost]
		public ActionResult ApplyImport()
		{
			var parser = new F5PriceParser();			
			List<string> errors;

			var list = parser.GetPriceListFromCache(_logger, out errors, true);


			var newSpecsOnly = new VendorFileParserModel { ProductLineList = list, ErrorList = errors };	
			string foundArticules = "";
			int successCounter = 0;

			// Получить все варианты
			// Если вариант есть в прайсе, то выставляем цену и преордер
			// Иначе сбрасываем
			// Апдейдим сразу все
			
			var productVariantList = _productService.GetProductVariants().ToList();
			foreach (var currentProductVariant in productVariantList)
			{
				var curProductLine = newSpecsOnly.ProductLineList.SingleOrDefault(x => x.Articul == currentProductVariant.Sku);

				if (curProductLine == null)
				{
					if (currentProductVariant.Price != 0)
						currentProductVariant.Price = 0;	

					continue;
				}

				var price = curProductLine.Price > 5 ? curProductLine.Price : 3; // товара дешевле 3 гривен быть не должно
				if (currentProductVariant.Price != price)
				{					
					currentProductVariant.Price = price;
				}

				if (currentProductVariant.AvailableForPreOrder != true)
					currentProductVariant.AvailableForPreOrder = true;
						
				successCounter++;
				foundArticules += curProductLine.Articul + ", ";													
			}

			if (productVariantList.Count > 0) // Вызываем сохранение всего контекста (всех вариантов)
				_productService.UpdateProductVariant(productVariantList[0]);


			return Content("Success for " + successCounter + " from " + newSpecsOnly.ProductLineList.Count() 					
				+ ". Success Articules" + foundArticules);			
		}
	}
}
