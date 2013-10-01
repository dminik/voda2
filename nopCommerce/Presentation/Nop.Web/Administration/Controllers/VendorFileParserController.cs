﻿namespace Nop.Admin.Controllers
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
