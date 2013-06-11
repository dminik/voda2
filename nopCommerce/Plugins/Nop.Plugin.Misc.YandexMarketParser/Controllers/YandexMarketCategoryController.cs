namespace Nop.Plugin.Misc.YandexMarketParser.Controllers
{
	using System.Linq;
	using System.Web.Mvc;

	using Nop.Plugin.Misc.YandexMarketParser.Domain;
	using Nop.Plugin.Misc.YandexMarketParser.Models;
	using Nop.Plugin.Misc.YandexMarketParser.Services;
	using Nop.Web.Framework.Controllers;

	using Telerik.Web.Mvc;

	[AdminAuthorize]
	public class YandexMarketCategoryController : Controller
	{
		private readonly IYandexMarketCategoryService _yandexMarketCategoryService;

		public YandexMarketCategoryController(IYandexMarketCategoryService yandexMarketCategoryService)
		{
			_yandexMarketCategoryService = yandexMarketCategoryService;
		}

		//[HttpPost, GridAction(EnableCustomBinding = true)]
		//public ActionResult List(GridCommand command)
		//{
		//	var records = _yandexMarketCategoryService.GetAll(command.Page - 1, command.PageSize);
		//	var categorysModel = records
		//		.Select(x =>
		//		{
		//			var m = new YandexMarketCategoryModel()
		//			{
		//				Id = x.Id,
		//				Name = x.Name,
		//			};

		//			return m;
		//		})
		//		.ToList();
		//	var model = new GridModel<YandexMarketCategoryModel>
		//	{
		//		Data = categorysModel,
		//		Total = records.TotalCount
		//	};

		//	return new JsonResult
		//	{
		//		Data = model
		//	};
		//}

		//[GridAction(EnableCustomBinding = true)]
		//public ActionResult Update(YandexMarketCategoryModel model, GridCommand command)
		//{
		//	var category = _yandexMarketCategoryService.GetById(model.Id);
		//	category.Name = model.Name;

		//	_yandexMarketCategoryService.Update(category);

		//	return List(command);
		//}

		//[GridAction(EnableCustomBinding = true)]
		//public ActionResult Delete(int id, GridCommand command)
		//{
		//	var category = _yandexMarketCategoryService.GetById(id);
		//	if (category != null)
		//		_yandexMarketCategoryService.Delete(category);

		//	return List(command);
		//}

		[HttpPost]
		public ActionResult Add(YandexMarketParserModel model)
		{
			var newItem = new YandexMarketCategoryRecord()
			{
				Name = model.AddCategoryName
			};
			_yandexMarketCategoryService.Insert(newItem);

			return Json(new { Result = true });
		}
	}
}
