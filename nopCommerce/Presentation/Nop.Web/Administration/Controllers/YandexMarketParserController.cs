﻿namespace Nop.Admin.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Transactions;
	using System.Web.Mvc;

	using Nop.Admin.Models.YandexMarket;
	using Nop.Core.Domain.Logging;
	using Nop.Core.Domain.YandexMarket;
	using Nop.Services.Logging;
	using Nop.Services.SiteParsers;
	using Nop.Services.YandexMarket;
	using Nop.Web.Framework.Controllers;

	[AdminAuthorize]
	public class YandexMarketParserController : Controller
	{
		private readonly IYandexMarketCategoryService _yandexMarketCategoryService;
		private readonly IYandexMarketProductService _yandexMarketProductService;
		private readonly ILogger _logger;

		public YandexMarketParserController(
			IYandexMarketCategoryService yandexMarketCategoryService,
			IYandexMarketProductService yandexMarketProductService,
			ILogger logger)
		{
			_yandexMarketCategoryService = yandexMarketCategoryService;
			_yandexMarketProductService = yandexMarketProductService;
			_logger = logger;
		}


		//[AdminAuthorize]
		//[ChildActionOnly]
		public ActionResult Configure()
		{
			var model = new YandexMarketParserModel();
			model.AvailableCategories = GetCategoriesForDDL();

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
				productList = CreateTestProductList(model.CategoryId);
			}
			else
			{
				var categoryName = _yandexMarketCategoryService.GetById(model.CategoryId).Name;
				var parser = new Parser(categoryName, model.CategoryId, model.ParseNotMoreThen, _logger);
				productList = parser.Parse();
			}

			using (var tsTransScope = new TransactionScope())
			{
				_logger.Debug("Deleting old products...");
				_yandexMarketProductService.DeleteByCategory(model.CategoryId);

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
								"Product 1",
								"url1",		
								categoryId,
									new List<YandexMarketSpecRecord>()
										{
											new YandexMarketSpecRecord("key1", "value1"),
											new YandexMarketSpecRecord("key2", "value2")
										}
							),
							new YandexMarketProductRecord(
								"Product 2",
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

		private List<SelectListItem> GetCategoriesForDDL()
		{
			var result = new List<SelectListItem>();
			result.Add(new SelectListItem() { Text = "---", Value = "0" });

			var categories = _yandexMarketCategoryService.GetAll();
			List<SelectListItem> ddlList = categories.Select(c => new SelectListItem() { Text = c.Name, Value = c.Id.ToString() }).ToList();
			result.AddRange(ddlList);

			return result;
		}

	}
}
