namespace Nop.Admin.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Mvc;

	using Nop.Admin.Models.YandexMarket;
	using Nop.Core;
	using Nop.Core.Domain.Catalog;
	using Nop.Core.Domain.YandexMarket;
	using Nop.Services.Catalog;
	using Nop.Services.Logging;
	using Nop.Services.Seo;
	using Nop.Services.SiteParsers;
	using Nop.Services.SiteParsers.Categories;
	using Nop.Services.YandexMarket;
	using Nop.Web.Framework;
	using Nop.Web.Framework.Controllers;

	using Telerik.Web.Mvc;

	[AdminAuthorize]
	public class YandexMarketParserCategoryController : Controller
	{
		private static bool mIsStopParsing = false;
		private readonly IYandexMarketCategoryService _yandexMarketCategoryService;
		private readonly ICategoryService _shopCategoryService;
		private readonly IProductService _productService;
		private readonly IStoreContext _storeContext;
		private readonly ILogger _logger;
		private readonly IUrlRecordService _urlRecordService;

		 public YandexMarketParserCategoryController(
			IYandexMarketCategoryService yandexMarketCategoryService, 
			ICategoryService shopCategoryService,
			IProductService productService,
			IStoreContext storeContext,
			ILogger logger,
			 IUrlRecordService urlRecordService)
		{
			this._yandexMarketCategoryService = yandexMarketCategoryService;
			_shopCategoryService = shopCategoryService;
			this._productService = productService;
			_storeContext = storeContext;
			_logger = logger;
			this._urlRecordService = urlRecordService;
		}

		 public ActionResult YandexMarketParserCategory()
		 {			 
			 return View();
		 }

		[HttpPost]
		public ActionResult CategoryParseAndShow(string urlCategoryForParsing)
		{
			mIsStopParsing = false;

			_logger.Debug("--- CategoryParse START...");
			
			var parser = BaseParserCategories.Create(urlCategoryForParsing, _logger, _yandexMarketCategoryService);
			var newCategory = parser.Parse(ref mIsStopParsing);
			
			_logger.Debug("+++ CategoryParse  DONE.");

			return Json(newCategory);
		}

		private Category InsertShopCategoryHierarhy(YandexMarketCategoryRecord yaCategory)
		{
			var oldShopCategory = this._shopCategoryService.GetAllCategoriesByParentCategoryId(yaCategory.ParentId).SingleOrDefault(x => x.Name == yaCategory.Name);
			

			if (oldShopCategory != null) 
				return oldShopCategory;
			
			var newShopCategory = new Category()
				{
					Name = yaCategory.Name,
					ParentCategoryId = yaCategory.ParentId,
					Published = true,
					CategoryTemplateId = 1,
					PageSize = 4,
					CreatedOnUtc = DateTime.UtcNow,
					UpdatedOnUtc = DateTime.UtcNow,
					PageSizeOptions = "8, 4, 20, 50",
					PriceRanges =
						"-10;10-30;30-70;70-100;100-150;150-200; 200-300;300-500;500-800;800-1000;1000-1500;1500-2000;2000-3000;3000-5000;5000-8000;8000-12000;12000-15000;15000-;",
				};

			_shopCategoryService.InsertCategory(newShopCategory);
			yaCategory.ShopCategoryId = newShopCategory.Id;
			
			var seName = newShopCategory.ValidateSeName("", newShopCategory.Name, true);
			_urlRecordService.SaveSlug(newShopCategory, seName, 0);			

			foreach (var currentCategory in yaCategory.Children)
			{
				currentCategory.ParentId = newShopCategory.Id;
				InsertShopCategoryHierarhy(currentCategory);
			}

			return newShopCategory;
		}

		private YandexMarketCategoryRecord InsertParserCategoryHierarhy(YandexMarketCategoryRecord yaCategory)
		{
			var oldYaCategory = this._yandexMarketCategoryService.GetAll()
				.SingleOrDefault(x => x.Name == yaCategory.Name && x.ParentId == yaCategory.ParentId);

			if (oldYaCategory != null)
				return oldYaCategory;
			

			// Создаем только листья, категории-контейнеры для других категорий нам не нужны для парсенья
			if (yaCategory.Children.Count == 0)
				_yandexMarketCategoryService.Insert(yaCategory);

			foreach (var currentYaCategory in yaCategory.Children)
			{
				currentYaCategory.ParentId = yaCategory.Id;
				InsertParserCategoryHierarhy(currentYaCategory);
			}

			return yaCategory;
		}

		[HttpPost]
		public ActionResult CategoryParseAndApply(string urlCategoryForParsing)
		{
			mIsStopParsing = false;

			_logger.Debug("--- CategoryParse START...");

			var parser = BaseParserCategories.Create(urlCategoryForParsing, _logger, _yandexMarketCategoryService);
			var newYaCategory = parser.Parse(ref mIsStopParsing);

			newYaCategory.ParentId = 0;
			var y = InsertShopCategoryHierarhy(newYaCategory);
			var x = InsertParserCategoryHierarhy(newYaCategory);
			
			_logger.Debug("+++ CategoryParse  DONE.");

			return Json(newYaCategory);
		}

		[HttpPost]
		public ActionResult ParseStop()
		{
			mIsStopParsing = true;
			return Content("Success");
		}
	}
}
