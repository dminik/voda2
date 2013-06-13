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
	public class YandexMarketProductController : Controller
	{
		private readonly IYandexMarketProductService _yandexMarketProductService;

		public YandexMarketProductController(IYandexMarketProductService yandexMarketProductService)
		{
			_yandexMarketProductService = yandexMarketProductService;
		}





		[HttpPost, GridAction(EnableCustomBinding = true)]
		public ActionResult ListProduct(int categoryId, GridCommand command)
		{
			var records = _yandexMarketProductService.GetByCategory(categoryId, command.Page - 1, command.PageSize);
			var productsModel = records
				.Select(x =>
					{
						var m = new YandexMarketProductModel(
							x.Name,
							x.ImageUrl_1,
							x.YandexMarketCategoryRecordId,
							x.Specifications.Select(s => new YandexMarketSpecModel(s.Key, s.Value)).ToList(),
							x.Id);

					return m;
				})
				.ToList();

			var model = new GridModel<YandexMarketProductModel>
			{
				Data = productsModel,
				Total = records.TotalCount
			};

			return new JsonResult
			{
				Data = model
			};
		}

		[GridAction(EnableCustomBinding = true)]
		public ActionResult UpdateProduct(YandexMarketProductRecord model, GridCommand command)
		{
			var product = _yandexMarketProductService.GetById(model.Id);
			product.Name = model.Name;

			_yandexMarketProductService.Update(product);

			return ListProduct(model.YandexMarketCategoryRecordId, command);
		}

		[GridAction(EnableCustomBinding = true)]
		public ActionResult DeleteProduct(int id, GridCommand command)
		{
			var product = _yandexMarketProductService.GetById(id);
			int categoryId = product.YandexMarketCategoryRecordId;

			if (product != null)
				_yandexMarketProductService.Delete(product);

			return ListProduct(categoryId, command);
		}
	}
}
