﻿namespace Nop.Admin.Controllers
{
	using System.Linq;
	using System.Web.Mvc;

	using Nop.Admin.Models.YandexMarket;
	using Nop.Core.Domain.YandexMarket;
	using Nop.Services.YandexMarket;
	using Nop.Web.Framework.Controllers;

	using Telerik.Web.Mvc;

	[AdminAuthorize]
	public class YandexMarketCategoryController : Controller
	{
		private readonly IYandexMarketCategoryService _yandexMarketCategoryService;

		public YandexMarketCategoryController(IYandexMarketCategoryService yandexMarketCategoryService)
		{
			this._yandexMarketCategoryService = yandexMarketCategoryService;
		}





		[HttpPost, GridAction(EnableCustomBinding = true)]
		public ActionResult ListCategory(GridCommand command)
		{
			var records = this._yandexMarketCategoryService.GetAll(command.Page - 1, command.PageSize);
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
		public ActionResult UpdateCategory(YandexMarketCategoryModel model, GridCommand command)
		{
			var category = this._yandexMarketCategoryService.GetById(model.Id);
			category.Name = model.Name;

			this._yandexMarketCategoryService.Update(category);

			return this.ListCategory(command);
		}

		[GridAction(EnableCustomBinding = true)]
		public ActionResult DeleteCategory(int id, GridCommand command)
		{
			var category = this._yandexMarketCategoryService.GetById(id);
			if (category != null)
				this._yandexMarketCategoryService.Delete(category);

			return this.ListCategory(command);
		}


		[HttpPost]
		public ActionResult Add(YandexMarketParserModel model)
		{
			var newItem = new YandexMarketCategoryRecord()
			{
				Name = model.AddCategoryName
			};
			this._yandexMarketCategoryService.Insert(newItem);

			return this.Json(new { Result = true });
		}
	}
}