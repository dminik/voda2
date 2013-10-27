namespace Nop.Admin.Controllers
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Mvc;

	using Nop.Admin.Models.YandexMarket;
	using Nop.Core;
	using Nop.Core.Domain.YandexMarket;
	using Nop.Services.Catalog;
	using Nop.Services.YandexMarket;
	using Nop.Web.Framework;
	using Nop.Web.Framework.Controllers;

	using Telerik.Web.Mvc;

	[AdminAuthorize]
	public class YandexMarketCategoryController : Controller
	{
		private readonly IYandexMarketCategoryService _yandexMarketCategoryService;
		private readonly IYandexMarketProductService _yandexMarketProductService;
		private readonly ICategoryService _shopCategoryService;
		private readonly IProductService _productService;
		 private readonly IStoreContext _storeContext;

		public YandexMarketCategoryController(
			IYandexMarketProductService yandexMarketProductService,
			IYandexMarketCategoryService yandexMarketCategoryService, 
			ICategoryService shopCategoryService,
			IProductService productService,
			IStoreContext storeContext)
		{
			_yandexMarketProductService = yandexMarketProductService;
			this._yandexMarketCategoryService = yandexMarketCategoryService;
			_shopCategoryService = shopCategoryService;
			this._productService = productService;
			_storeContext = storeContext;
		}





		[HttpPost]
		[GridAction(EnableCustomBinding = true)]
		public ActionResult ListCategory(bool isWithProductCountInCategories, GridCommand command)
		{			
			var availableShopCategories =
				_shopCategoryService.GetAllCategories();//.Select(x => new Dictionary<int, string>( x.Id, x.Name)).ToList();
  

			var yaCategories =_yandexMarketCategoryService.GetAll(command.Page - 1, command.PageSize);

			var categorysModel = yaCategories
				.Select(currentYaCategory =>
					{
						var shopCategory = availableShopCategories.SingleOrDefault(s => s.Id == currentYaCategory.ShopCategoryId);
						var shopCategoryName = "";
						var numberOfProducts = 0;
						var notImportedRecords = 0;

						if (shopCategory != null)
						{
							shopCategoryName = shopCategory.Name;

							if (isWithProductCountInCategories)
							{
								numberOfProducts =
									this._productService.SearchProducts(
										categoryIds: new List<int>() { shopCategory.Id },
										priceMin: 1,
										storeId: _storeContext.CurrentStore.Id,
										pageSize: 1).TotalCount;

								// посчитать сколько не импортировано еще
								notImportedRecords = _yandexMarketProductService.GetByCategory(
																								categoryId: currentYaCategory.Id, 
																								isNotImportedOnly: true, 
																								pageIndex: command.Page - 1, 
																								pageSize: 1).TotalCount;								
							}
						}
						else
						{
							shopCategoryName = "---";
							currentYaCategory.ShopCategoryId = 0;
						}
						
						var m = new YandexMarketCategoryModel()
						{
							Id = currentYaCategory.Id,
							Name = currentYaCategory.Name,
							Url = currentYaCategory.Url,
							ShopCategoryId = currentYaCategory.ShopCategoryId,
							ShopCategoryName = shopCategoryName,
							IsActive = currentYaCategory.IsActive,
							AlreadyImportedProducts = numberOfProducts,
							NotImportedProducts = notImportedRecords,
						};

					return m;
				})
				.ForCommand(command)
				.ToList();

			var model = new GridModel<YandexMarketCategoryModel>
			{
				Data = categorysModel,
				Total = yaCategories.TotalCount
			};

			return new JsonResult
			{
				Data = model
			};
		}

		[GridAction(EnableCustomBinding = true)]
		public ActionResult UpdateCategory(YandexMarketCategoryModel model, GridCommand command)
		{
			UpdateCategory(model);
			return ListCategory(isWithProductCountInCategories: false, command: command);
		}

		private void UpdateCategory(YandexMarketCategoryModel model)
		{
			var category = _yandexMarketCategoryService.GetById(model.Id);
			category.Name = model.Name;
			category.Url = model.Url;
			category.IsActive = model.IsActive;

			_yandexMarketCategoryService.Update(category);			
		}

		public ActionResult UpdateCategories(
			[Bind(Prefix = "inserted")]IEnumerable<YandexMarketCategoryModel> insertedCategorys,
			[Bind(Prefix = "updated")]IEnumerable<YandexMarketCategoryModel> updatedCategorys,
			[Bind(Prefix = "deleted")]IEnumerable<YandexMarketCategoryModel> deletedCategorys,
			GridCommand command)
		{
			if (insertedCategorys != null)
			{
				foreach (var category in insertedCategorys)
				{
					// perform insert
				}
			}

			if (updatedCategorys != null)
			{
				foreach (var category in updatedCategorys)
				{
					//perform update
					UpdateCategory(category);
				}
			}

			if (deletedCategorys != null)
			{
				foreach (var category in deletedCategorys)
				{
					//perform update
				}
			}

			return ListCategory(isWithProductCountInCategories: false, command: command);
		}

		[GridAction(EnableCustomBinding = true)]
		public ActionResult DeleteCategory(int id, GridCommand command)
		{
			var category =_yandexMarketCategoryService.GetById(id);
			if (category != null)
				this._yandexMarketCategoryService.Delete(category);

			return ListCategory(isWithProductCountInCategories: false, command: command);
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

		[HttpPost]
		public ActionResult SetActiveAllParserCategoties(bool isActive)
		{
			_yandexMarketCategoryService.SetActiveAllParserCategoties(isActive);
			return Json(new { Result = true });
		}
	}
}
