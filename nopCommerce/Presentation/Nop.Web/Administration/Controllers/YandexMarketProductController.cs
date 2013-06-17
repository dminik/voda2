namespace Nop.Admin.Controllers
{
	using System;
	using System.Linq;
	using System.Web.Mvc;

	using Nop.Admin.Models.Catalog;
	using Nop.Admin.Models.YandexMarket;
	using Nop.Core.Domain.Catalog;
	using Nop.Core.Domain.YandexMarket;
	using Nop.Services.Catalog;
	using Nop.Services.Security;
	using Nop.Services.Seo;
	using Nop.Services.YandexMarket;
	using Nop.Web.Framework.Controllers;

	using Telerik.Web.Mvc;

	[AdminAuthorize]
	public class YandexMarketProductController : Controller
	{
		private readonly IYandexMarketProductService _yandexMarketProductService;
		private readonly IProductService _productService;
		private readonly IUrlRecordService _urlRecordService;
		private readonly ICategoryService _categoryService;

		public YandexMarketProductController(IYandexMarketProductService yandexMarketProductService,
			IProductService productService,
			IUrlRecordService urlRecordService,
			ICategoryService categoryService)
		{
			_yandexMarketProductService = yandexMarketProductService;
			this._productService = productService;
			this._urlRecordService = urlRecordService;
			this._categoryService = categoryService;
		}

		#region Grid actions

		[HttpPost]
		[GridAction(EnableCustomBinding = true)]
		public ActionResult ListProduct(int categoryId, GridCommand command)
		{
			var records = _yandexMarketProductService.GetByCategory(categoryId, command.Page - 1, command.PageSize);
			var productsModel = records.Select(
				x =>
				{
					var m = new YandexMarketProductModel(
						x.Name,
						x.ImageUrl_1,
						x.YandexMarketCategoryRecordId,
						x.Specifications.Select(s => new YandexMarketSpecModel(s.Key, s.Value)).ToList(),
						x.Id);

					return m;
				}).ToList();

			var model = new GridModel<YandexMarketProductModel> { Data = productsModel, Total = records.TotalCount };

			return new JsonResult { Data = model };
		}

		[GridAction(EnableCustomBinding = true)]
		public ActionResult UpdateProduct(YandexMarketProductModel model, GridCommand command)
		{
			var product = _yandexMarketProductService.GetById(model.Id);
			product.Name = model.Name;

			this._yandexMarketProductService.Update(product);

			return ListProduct(product.YandexMarketCategoryRecordId, command);
		}

		[GridAction(EnableCustomBinding = true)]
		public ActionResult DeleteProduct(int id, GridCommand command)
		{
			var product = _yandexMarketProductService.GetById(id);
			int categoryId = product.YandexMarketCategoryRecordId;

			if (product != null) this._yandexMarketProductService.Delete(product);

			return ListProduct(categoryId, command);
		}

		#endregion

		[HttpPost]
		public ActionResult ImportProductList(int categoryId)
		{
			var records = _yandexMarketProductService.GetByCategory(categoryId);

			foreach (var curYaProduct in records)
			{
				ImportYaProduct(curYaProduct);
			}

			return Content("Success!");
		}

		private void ImportYaProduct(YandexMarketProductRecord yaProduct)
		{

			//product				
			var product = new Product
				{
					CreatedOnUtc = DateTime.UtcNow,
					UpdatedOnUtc = DateTime.UtcNow,
					Name = yaProduct.Name,
					Published = true,
				};

			_productService.InsertProduct(product);

			//search engine name
			var seName = product.ValidateSeName(product.Name, product.Name, true);
			_urlRecordService.SaveSlug(product, seName, 0);

			//variant
			var variant = new ProductVariant()
			{
				ProductId = product.Id,
				Published = true,
				DisplayOrder = 1,
				CreatedOnUtc = DateTime.UtcNow,
				UpdatedOnUtc = DateTime.UtcNow,
			};

			_productService.InsertProductVariant(variant);

			// Category
			const int categoryId = 19;
			var productCategory = new ProductCategory()
			{
				ProductId = product.Id,
				CategoryId = categoryId,
				IsFeaturedProduct = false,
				DisplayOrder = 1
			};
			_categoryService.InsertProductCategory(productCategory);
		}
	}
}
