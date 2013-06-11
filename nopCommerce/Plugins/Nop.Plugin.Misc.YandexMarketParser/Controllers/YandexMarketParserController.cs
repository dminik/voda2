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

		public YandexMarketParserController(IYandexMarketCategoryService yandexMarketCategoryService)
		{
			_yandexMarketCategoryService = yandexMarketCategoryService;
		}


		[AdminAuthorize]
		[ChildActionOnly]
		public ActionResult Configure()
		{

			var r = RouteTable.Routes.Where(x => x is Route).Select(s => (s as Route).Url).Where(g => g.Contains("YandexMarketParser")).ToList();		

			var model = new YandexMarketParserModel();
		  
			model.HelloWord = "Ne beebe";

			//var rr =  Url.RouteUrl("Plugin.Misc.YandexMarketParser.Category.List");

			var categories = _yandexMarketCategoryService.GetAll();
			foreach (var c in categories)
				model.AvailableCategories.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });





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

				var categoryName = _yandexMarketCategoryService.GetById(model.CategoryId).Name;
				var parser = new Parser(categoryName, model.ParseNotMoreThen);
				model.ProductList = parser.Parse();				
			}
			else
			{
				model = new YandexMarketParserModel()
				{
					IsTest = true,
					CategoryId = model.CategoryId,
					ProductList =
						new List<ProductRecord>()
							{
								new ProductRecord()
									{
										Title = "Product 1",
										ImageUrl_1 = "url1",
										Specifications = new Dictionary<string, string>() { { "key1", "value1" }, { "key2", "value2" }, }
									},
								new ProductRecord()
									{
										Title = "Product 2",
										ImageUrl_1 = "url2",
										Specifications = new Dictionary<string, string>() { { "key1", "value1" }, { "key2", "value2" }, }
									}
							}
				};
			}

			var categories = _yandexMarketCategoryService.GetAll();
			foreach (var c in categories)
				model.AvailableCategories.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });

			return View("Nop.Plugin.Misc.YandexMarketParser.Views.YandexMarketParser.Configure", model);
		}

		
		
	}
}
