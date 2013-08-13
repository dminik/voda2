namespace Nop.Admin.Controllers
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
			var records =_yandexMarketCategoryService.GetAll(command.Page - 1, command.PageSize);
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
			var category =_yandexMarketCategoryService.GetById(model.Id);
			category.Name = model.Name;

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
				Name = model.AddParserCategoryName
			};
			this._yandexMarketCategoryService.Insert(newItem);

			return Json(new { Result = true });
		}
	}
}
