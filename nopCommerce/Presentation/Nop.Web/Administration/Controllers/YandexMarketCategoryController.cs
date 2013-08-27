namespace Nop.Admin.Controllers
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Mvc;

	using Nop.Admin.Models.YandexMarket;
	using Nop.Core.Domain.YandexMarket;
	using Nop.Services.Catalog;
	using Nop.Services.YandexMarket;
	using Nop.Web.Framework.Controllers;

	using Telerik.Web.Mvc;

	[AdminAuthorize]
	public class YandexMarketCategoryController : Controller
	{
		private readonly IYandexMarketCategoryService _yandexMarketCategoryService;
		private readonly ICategoryService _shopCategoryService;

		public YandexMarketCategoryController(
			IYandexMarketCategoryService yandexMarketCategoryService, 
			ICategoryService shopCategoryService)
		{
			this._yandexMarketCategoryService = yandexMarketCategoryService;
			_shopCategoryService = shopCategoryService;
		}





		[HttpPost, GridAction(EnableCustomBinding = true)]
		public ActionResult ListCategory(GridCommand command)
		{
			var availableShopCategories =
				_shopCategoryService.GetAllCategories();//.Select(x => new Dictionary<int, string>( x.Id, x.Name)).ToList();
  

			var records =_yandexMarketCategoryService.GetAll(command.Page - 1, command.PageSize);
			var categorysModel = records
				.Select(x =>
					{
						var shopCategory = availableShopCategories.SingleOrDefault(s => s.Id == x.ShopCategoryId);
						var shopCategoryName = "";
						if (shopCategory != null)
						{
							shopCategoryName = shopCategory.Name;
						}
						else
						{
							shopCategoryName = "---";
							x.ShopCategoryId = 0;
						}
						
						var m = new YandexMarketCategoryModel()
						{
							Id = x.Id,
							Name = x.Name,
							Url = x.Url,
							ShopCategoryId = x.ShopCategoryId,
							ShopCategoryName = shopCategoryName,
							IsActive = x.IsActive,
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
		public ActionResult UpdateCategory(YandexMarketCategoryModel model, GridCommand command)
		{
			var category =_yandexMarketCategoryService.GetById(model.Id);
			category.Name = model.Name;
			category.Url = model.Url;
			category.IsActive = model.IsActive;
			
			_yandexMarketCategoryService.Update(category);

			return ListCategory(command);
		}

		[GridAction(EnableCustomBinding = true)]
		public ActionResult DeleteCategory(int id, GridCommand command)
		{
			var category =_yandexMarketCategoryService.GetById(id);
			if (category != null)
				this._yandexMarketCategoryService.Delete(category);

			return ListCategory(command);
		}


		[HttpPost]
		public ActionResult Add(YandexMarketParserModel model)
		{
			var newItem = new YandexMarketCategoryRecord()
			{
				Name = model.AddParserCategoryName,
				Url = model.AddParserCategoryUrl,
				ShopCategoryId = model.AddShopCategoryId,
				IsActive = model.AddIsActive,
			};
			this._yandexMarketCategoryService.Insert(newItem);

			return Json(new { Result = true });
		}
	}
}
