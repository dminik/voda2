namespace Nop.Plugin.Misc.YandexMarketParser.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Text;
	using System.Threading;
	using System.Web.Mvc;
	using System.Web.Routing;

	using Nop.Core;
	using Nop.Core.Domain.Directory;
	using Nop.Plugin.Misc.YandexMarketParser.Domain;
	using Nop.Plugin.Misc.YandexMarketParser.Models;
	using Nop.Plugin.Misc.YandexMarketParser.Services;
	using Nop.Services.Configuration;
	using Nop.Services.Directory;
	using Nop.Services.Localization;
	using Nop.Services.Security;
	using Nop.Services.Shipping;
	using Nop.Web.Framework.Controllers;

	using Telerik.Web.Mvc;

	[AdminAuthorize]
	public class YandexMarketParserController : Controller
	{
		private readonly IYandexMarketCategoryService _yandexMarketCategoryService;
		private readonly IYandexMarketProductService _yandexMarketProductService;

		public YandexMarketParserController(
			IYandexMarketCategoryService yandexMarketCategoryService,
			IYandexMarketProductService yandexMarketProductService)
		{
			_yandexMarketCategoryService = yandexMarketCategoryService;
			_yandexMarketProductService = yandexMarketProductService;
		}


		[AdminAuthorize]
		[ChildActionOnly]
		public ActionResult Configure()
		{
			var model = new YandexMarketParserModel();
			model.AvailableCategories = GetCategoriesForDDL();

			return View("Nop.Plugin.Misc.YandexMarketParser.Views.YandexMarketParser.Configure", model);
		}

		[HttpPost]
		[AdminAuthorize]
		[ChildActionOnly]
		public ActionResult Configure(YandexMarketParserModel model)
		{
			if (!ModelState.IsValid)
				return Configure();

			if (model.IsTest)
			{				
				model.ProductList = CreateTestProductList(model.CategoryId);
				_yandexMarketProductService.InsertList(model.ProductList);
			}
			else
			{
				var categoryName = _yandexMarketCategoryService.GetById(model.CategoryId).Name;
				var parser = new Parser(categoryName, model.ParseNotMoreThen);
				model.ProductList = parser.Parse();
			}

			return View("Nop.Plugin.Misc.YandexMarketParser.Views.YandexMarketParser.Configure", model);
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

			var categories = this._yandexMarketCategoryService.GetAll();
			List<SelectListItem> ddlList = categories.Select(c => new SelectListItem() { Text = c.Name, Value = c.Id.ToString() }).ToList();
			result.AddRange(ddlList);

			return result;
		}

	}
}
