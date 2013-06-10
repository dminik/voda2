namespace Nop.Plugin.Misc.YandexMarketParser.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Text;
	using System.Threading;
	using System.Web.Mvc;

	using Nop.Core.Domain.Directory;
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

		public YandexMarketParserController(IYandexMarketCategoryService yandexMarketCategoryService)
		{
			_yandexMarketCategoryService = yandexMarketCategoryService;
		}


		[AdminAuthorize]
		[ChildActionOnly]
		public ActionResult Configure()
		{
			var model = new YandexMarketParserModel();
		  
			model.HelloWord = "Ne beebe";

			return View("Nop.Plugin.Misc.YandexMarketParser.Views.YandexMarketParser.Configure", model);
		}

		
		[HttpPost]
		[AdminAuthorize]
		[ChildActionOnly]
		public ActionResult Configure(YandexMarketParserModel model)
		{
			if (!model.IsTest)
			{
				if (!ModelState.IsValid) return Configure();

				var parser = new Parser("Atoll", model.ParseNotMoreThen);
				model = parser.Parse();
			}
			else
			{
				model = new YandexMarketParserModel()
				{
					IsTest = true,
					CatalogName = "testCatalogName",
					ProductList =
						new List<Product>()
							{
								new Product()
									{
										Title = "Product 1",
										ImageUrl_1 = "url1",
										Specifications = new Dictionary<string, string>() { { "key1", "value1" }, { "key2", "value2" }, }
									},
								new Product()
									{
										Title = "Product 2",
										ImageUrl_1 = "url2",
										Specifications = new Dictionary<string, string>() { { "key1", "value1" }, { "key2", "value2" }, }
									}
							}
				};
			}

			return View("Nop.Plugin.Misc.YandexMarketParser.Views.YandexMarketParser.Configure", model);
		}

		[HttpPost, GridAction(EnableCustomBinding = true)]
		public ActionResult CategoryList(GridCommand command)
		{			
			var records = _yandexMarketCategoryService.GetAll(command.Page - 1, command.PageSize);
			var categorysModel = records
				.Select(x =>
				{
					var m = new YandexMarketCategoryModel()
					{
						Id = x.Id,
						Name = x.Name,
					};
					
					return m;
				})
				.ToList();
			var model = new GridModel<YandexMarketCategoryModel>
			{
				Data = categorysModel,
				Total = records.TotalCount
			};

			return new JsonResult
			{
				Data = model
			};
		}

		[GridAction(EnableCustomBinding = true)]
		public ActionResult CategoryUpdate(YandexMarketCategoryModel model, GridCommand command)
		{			
			var category = _yandexMarketCategoryService.GetById(model.Id);
			category.Name = model.Name;
			
			_yandexMarketCategoryService.Update(category);

			return CategoryList(command);
		}

		[GridAction(EnableCustomBinding = true)]
		public ActionResult CategoryDelete(int id, GridCommand command)
		{		
			var category = _yandexMarketCategoryService.GetById(id);
			if (category != null)
				_yandexMarketCategoryService.Delete(category);

			return CategoryList(command);
		}		
	}
}
