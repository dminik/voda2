namespace Nop.Admin.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Transactions;
	using System.Web.Mvc;

	using Nop.Admin.Models.YandexMarket;
	using Nop.Core.Domain.Catalog;
	using Nop.Core.Domain.Logging;
	using Nop.Core.Domain.YandexMarket;
	using Nop.Services.Catalog;
	using Nop.Services.Logging;
	using Nop.Services.SiteParsers;
	using Nop.Services.YandexMarket;
	using Nop.Web.Framework;
	using Nop.Web.Framework.Controllers;

	[AdminAuthorize]
	public class YandexMarketParserController : Controller
	{
		private readonly IYandexMarketCategoryService _yandexMarketCategoryService;
		private readonly ICategoryService _shopCategoryService;
		private readonly IYandexMarketProductService _yandexMarketProductService;
		private readonly ILogger _logger;

		public YandexMarketParserController(
			IYandexMarketCategoryService yandexMarketCategoryService,
			IYandexMarketProductService yandexMarketProductService,
			ICategoryService shopCategoryService,
			ILogger logger)
		{
			_yandexMarketCategoryService = yandexMarketCategoryService;
			_yandexMarketProductService = yandexMarketProductService;
			_shopCategoryService = shopCategoryService;
			_logger = logger;
		}


		//[AdminAuthorize]
		//[ChildActionOnly]
		public ActionResult Configure()
		{
			var model = new YandexMarketParserModel();
			model.AvailableParserCategories = _yandexMarketCategoryService.GetCategoriesForDDL();
			model.AvailableShopCategories =
				_shopCategoryService.GetAllCategories()
				                    .Select(x => new SelectListItem() { Selected = false, Text = x.Name, Value = x.Id.ToString() }).ToList();
  
			return View(model);
		}


		//[AdminAuthorize]
		//[ChildActionOnly]
		[HttpPost]
		public ActionResult Parse(YandexMarketParserModel model)
		{
			_logger.Debug("---  PARSE START...");

			if (!this.ModelState.IsValid)
				throw new Exception("ModelState.IsNOTValid");

			var productList = new List<YandexMarketProductRecord>();


			if (model.IsTest)
			{
				productList = CreateTestProductList(model.ParserCategoryId);
			}
			else
			{
				var categoryName = _yandexMarketCategoryService.GetById(model.ParserCategoryId).Name;
				var parser = BaseParser.Create(categoryName, model.ParserCategoryId, model.ParseNotMoreThen, model.ProductsPageUrl, _logger);				
				productList = parser.Parse();
			}

			using (var tsTransScope = new TransactionScope())
			{
				_logger.Debug("Deleting old products...");
				_yandexMarketProductService.DeleteByCategory(model.ParserCategoryId);

				_logger.Debug("Saving new " + productList.Count + " products...");
				_yandexMarketProductService.InsertList(productList);

				_logger.Debug("+++ PARSE DONE.");

				tsTransScope.Complete();
				return Json(new { Result = true });
			}
		}
		
		private List<YandexMarketProductRecord> CreateTestProductList(int categoryId)
		{
			return new List<YandexMarketProductRecord>()
					{
						new YandexMarketProductRecord(
							"11111",	
							"Product 1",
								"Описание 1", 
								"url1",		
								categoryId,
									new List<YandexMarketSpecRecord>()
										{
											new YandexMarketSpecRecord("key1", "value1"),
											new YandexMarketSpecRecord("key2", "value2")
										}
							),
							new YandexMarketProductRecord(
								"22222",
								"Product 2",
								"Описание 2",
								"url1",		
								categoryId,
									new List<YandexMarketSpecRecord>()
										{
											new YandexMarketSpecRecord("key1", "value1"),
											new YandexMarketSpecRecord("key2", "value2")
										}
							)
					};
		}
	}
}
