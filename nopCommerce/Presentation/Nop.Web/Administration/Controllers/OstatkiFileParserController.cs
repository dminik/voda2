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
	public class OstatkiFileParserController : Controller
	{
		private readonly IProductService _productService;

		public OstatkiFileParserController(IProductService productService)
		{
			_productService = productService;			
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

			/*
			                     Товара нет нигде               Товар есть у поставщика               Товар в Боярке
			 цена           0 (не показываем его на сайте)       есть                                       есть
			 предзаказ              да                           да                                          нет
		     доставка сейчас                     -               нет                                          да
			 * 
			 * 
			 * Подправить:
				Количество не отслеживаем ок
				Отображение в 2 местах ок
				При импорте, парсинге в двух файлах ок
				Слово Заказать ок
				Фильтр наличия - подправить хранимку

			*/






			foreach (var curProductLine in newSpecsOnly.ProductLineList)
			{
				var currentProductVariant = _productService.GetProductVariantBySku(curProductLine.Articul);
				if (currentProductVariant == null)
				{
					notFoundArticules += curProductLine.Articul + ", ";
					continue;
				}

				var isNeedUpdate = false;

				//if (currentProductVariant.StockQuantity != curProductLine.Amount)
				//{
				//	currentProductVariant.StockQuantity = curProductLine.Amount;
				//	isNeedUpdate = true;
				//}

				//if (currentProductVariant.Price != curProductLine.Price)
				//{
				//	currentProductVariant.Price = curProductLine.Price;
				//	isNeedUpdate = true;
				//}

				if (currentProductVariant.AvailableForPreOrder != false)
				{
					currentProductVariant.AvailableForPreOrder = false;
					isNeedUpdate = true;
				}
								
				if(isNeedUpdate)
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
			resultModel.ProductLineList = Nop.Services.FileParsers.FileParserOstatki.ParseOstatkiFile(filePath, out errors);
			resultModel.ErrorList = errors;

			return resultModel;
;
		}
	}
}
