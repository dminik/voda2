namespace Nop.Admin.Controllers
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Web.Mvc;
	using System.Xml.Linq;

	using Nop.Admin.Models.Catalog;
	using Nop.Admin.Models.YandexMarket;
	using Nop.Core;
	using Nop.Core.Domain.Catalog;
	using Nop.Core.Domain.Seo;
	using Nop.Core.Domain.YandexMarket;
	using Nop.Services.Catalog;
	using Nop.Services.Logging;
	using Nop.Services.Media;
	using Nop.Services.Security;
	using Nop.Services.Seo;
	using Nop.Services.SiteParsers;
	using Nop.Services.SiteParsers.Products;
	using Nop.Services.SiteParsers.Xls;
	using Nop.Services.YandexMarket;
	using Nop.Web.Framework.Controllers;

	using Telerik.Web.Mvc;

	[AdminAuthorize]
	public class YandexMarketProductController : Controller
	{
		private static bool mIsStopProducsImport = false;

		private readonly IYandexMarketProductService _yandexMarketProductService;
		private readonly IProductService _productService;
		private readonly IUrlRecordService _urlRecordService;
		private readonly ICategoryService _categoryService;		
		private readonly IPictureService _pictureService;
		private readonly ISpecificationAttributeService2 _specificationAttributeService;
		private readonly IYandexMarketCategoryService _yandexMarketCategoryService;
		private readonly ILogger _logger;
		private readonly SeoSettings _seoSettings;
		private readonly IPriceManagerService _priceManagerService;

		public YandexMarketProductController(IYandexMarketProductService yandexMarketProductService,
			IProductService productService,
			IUrlRecordService urlRecordService,
			ICategoryService categoryService,			
			IPictureService pictureService,
			ISpecificationAttributeService2 specificationAttributeService,
			IYandexMarketCategoryService yandexMarketCategoryService,
			ILogger logger,
			SeoSettings seoSettings,
			IPriceManagerService priceManagerService)
		{
			_yandexMarketProductService = yandexMarketProductService;
			_productService = productService;
			_urlRecordService = urlRecordService;
			_categoryService = categoryService;			
			_pictureService = pictureService;
			_specificationAttributeService = specificationAttributeService;
			_yandexMarketCategoryService = yandexMarketCategoryService;
			_logger = logger;
			_seoSettings = seoSettings;
			_priceManagerService = priceManagerService;
		}

		#region Grid actions

		[HttpPost]
		public ActionResult ImportProductListStop()
		{
			mIsStopProducsImport = true;
			return Content("Success");
		}

		[HttpPost]
		[GridAction(EnableCustomBinding = true)]
		public ActionResult ListProduct(bool isNotImportedOnly, int parserCategoryId, GridCommand command)
		{			
			var records = _yandexMarketProductService.GetByCategory(parserCategoryId, isNotImportedOnly, command.Page - 1, command.PageSize);
			
			var productsModel = records.Select(
				x =>
				{
					var formated = new FormatterUgContract().Format(x);
					
					var m = new YandexMarketProductModel(
						formated.Articul,
						formated.Name,
						formated.FullDescription,
						formated.ImageUrl_1,
						formated.Url,
						formated.YandexMarketCategoryRecordId,
						formated.Specifications.Select(s => new YandexMarketSpecModel(s.Key, s.Value)).ToList(),
						formated.Id);

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

			return ListProduct(false, product.YandexMarketCategoryRecordId, command);
		}

		[GridAction(EnableCustomBinding = true)]
		public ActionResult DeleteProduct(int id, GridCommand command)
		{
			var product = _yandexMarketProductService.GetById(id);
			int parserCategoryId = product.YandexMarketCategoryRecordId;

			if (product != null) this._yandexMarketProductService.Delete(product);

			return ListProduct(false, parserCategoryId, command);
		}

		#endregion

		[HttpPost]
		public ActionResult ImportProductList()
		{
			mIsStopProducsImport = false;

			_logger.Debug("--- ImportProductList START...");

			var productsToImportTotalCount = _yandexMarketProductService.GetByCategory(categoryId: 0, isNotImportedOnly: true, isActiveOnly: true).TotalCount;
			var parserCategories = _yandexMarketCategoryService.GetActive();
			_logger.Debug("--- Will be import categories: " + parserCategories.Count + ". Products: " + productsToImportTotalCount);

			
			var totalImportedCounter = 0;

			var allSpecAttrList = _specificationAttributeService.GetSpecificationAttributes();

			int currentCategoryIndex = 1;
			// цикл по активным категориям						
			foreach (var currentParserCategory in parserCategories)
			{				
				var records = _yandexMarketProductService.GetByCategory(currentParserCategory.Id, isNotImportedOnly: true);

				var categoryProductCounter = 1;

				_logger.Debug("--- Start Category (" + currentCategoryIndex + " from " + parserCategories.Count + ") " + currentParserCategory.Name + ". Will be imported products: " + records.TotalCount);
				foreach (var curYaProduct in records)
				{
					CheckStopAction();
					
					var isImported = ImportYaProduct(curYaProduct, currentParserCategory.ShopCategoryId, allSpecAttrList);


					if (totalImportedCounter % 10 == 0) // через каждые 5 записей выводить в лог сообщение
						_logger.Debug("... imported products in current category: " + categoryProductCounter + ", in general: " + totalImportedCounter + " from " + productsToImportTotalCount + "...");

					if (isImported)
					{
						categoryProductCounter++;
						totalImportedCounter++;
					}
				}

				_logger.Debug("--- End Category imported " + currentParserCategory.Name + ". In general: " + totalImportedCounter + " from " + productsToImportTotalCount);

				currentCategoryIndex++;
			}

			_priceManagerService.ApplyImportAll();
			_logger.Debug("--- ImportProductList for End.");

			return Content("Success");
		}

		private bool ImportYaProduct(YandexMarketProductRecord yaProduct, int shopCategoryId, IEnumerable<SpecificationAttribute> allSpecAttrList)
		{
			Product product;
			var variant = _productService.GetProductVariantBySku(yaProduct.Articul);

			bool isUpdating = variant != null;

			// create or update item
			if (isUpdating)
			{
				// product = variant.Product;
				return false; // Не обновляем существующие товары
			}
			else
			{
				product = new Product
					{
						CreatedOnUtc = DateTime.UtcNow,
						Published = true,
					};

				variant = new ProductVariant
					{
						ProductId = product.Id,
						Published = true,
						DisplayOrder = 1,
						CreatedOnUtc = DateTime.UtcNow,
						OrderMinimumQuantity = 1,
						OrderMaximumQuantity = 10,						
						ManageInventoryMethodId = 0,
						DisplayStockAvailability = true,
						DisplayStockQuantity = true,
						IsFreeShipping = false,
						IsShipEnabled = true,
						AdditionalShippingCharge = 20,
						IsTaxExempt = true,
						Price = 0,
						AvailableForPreOrder = true,
					};
			}

			//product										
			product.UpdatedOnUtc = DateTime.UtcNow;			
			product.Name = yaProduct.Name;
			product.FullDescription = yaProduct.FullDescription;
			product.ShortDescription = CreateShortDescription(yaProduct.Specifications);
			
			//variant						
			variant.UpdatedOnUtc = DateTime.UtcNow;
			variant.Sku = yaProduct.Articul;
			variant.Name = yaProduct.Name;

			// Insert or Updating
			if (isUpdating)
			{
				_productService.UpdateProduct(product);
				_productService.UpdateProductVariant(variant);
			}
			else
			{
				try
				{				
					_productService.InsertProduct(product);
					variant.ProductId = product.Id;
					_productService.InsertProductVariant(variant);
				
					SaveCategory(shopCategoryId, product.Id);
					SaveSpecList(product, yaProduct.Specifications, shopCategoryId, allSpecAttrList);


					SavePictures(product, yaProduct.ImageUrl_1);

					//search engine name					
					var seName = product.ValidateSeName("", product.Name, true);
					_urlRecordService.SaveSlug(product, seName, 0);
				}
				catch 
				{
					if (product.Id > 0)
						_productService.DeleteProduct(product);
					throw;
				}
			}

			return true;
		}

		
		private string CreateShortDescription(IEnumerable<YandexMarketSpecRecord> specList)
		{
			return "";
		
			//var attribsForShortDescription = new List<string>()
			//	{
			//		"Тип фильтра",
			//		"Число ступеней очистки",
			//		"Отдельный кран"
			//	};

			//var ulShortDescription = new XElement("ul");
			//var xmlShortDescription = 
			//	new XElement("div", new XAttribute("class", "ShortDescription"),
			//		ulShortDescription);
			
			//foreach (var yandexMarketSpecRecord in specList.Where(yandexMarketSpecRecord => attribsForShortDescription.Contains(yandexMarketSpecRecord.Key)))
			//{
			//	ulShortDescription.Add(new XElement("li", yandexMarketSpecRecord.Key + ": " + yandexMarketSpecRecord.Value));
			//}

			//return xmlShortDescription.ToString();
		}

		private void SaveSpecList(Product product, IEnumerable<YandexMarketSpecRecord> specList, int shopCategoryId, IEnumerable<SpecificationAttribute> allSpecAttrList)
		{			
			
			var psaList = new List<ProductSpecificationAttribute>();

			var category = _categoryService.GetCategoryById(shopCategoryId);

			foreach (var yandexMarketSpecRecord in specList)
			{
				var attributeOptionId = FindAttributeOptionId(yandexMarketSpecRecord.Key, yandexMarketSpecRecord.Value, allSpecAttrList);


				bool isShowInFilter;
				bool isShowInShortDescription;
				YandexMarketHelpers.IsSpecAllow(yandexMarketSpecRecord.Key, new List<Category>() { category }, out isShowInFilter, out isShowInShortDescription);

				var psa = new ProductSpecificationAttribute()
				{
					SpecificationAttributeOptionId = attributeOptionId,
					ProductId = product.Id,
					AllowFiltering = isShowInFilter,
					ShowOnProductPage = true,
				};

				psaList.Add(psa);
			}

			_specificationAttributeService.InsertProductSpecificationAttributeList(psaList);
		}

		private int FindAttributeOptionId(string attrName, string attrOptName, IEnumerable<SpecificationAttribute> allSpecAttrList)
		{
			SpecificationAttribute resultAttrName;
			
			resultAttrName = allSpecAttrList.FirstOrDefault(x => x.Name == attrName);
			
			if (resultAttrName == null)
				throw new Exception("Cant find Product attribute by name " + attrName);

			var resultAttrOptName = resultAttrName.SpecificationAttributeOptions.FirstOrDefault(s => s.Name == attrOptName);
			
			if (resultAttrOptName == null)
				throw new Exception("Cant find Product attributeOpt by name " + attrOptName);

			return resultAttrOptName.Id;
		}

		private void SavePictures(Product product, string picturesSourceUrls)
		{
			if (_pictureService.GetPicturesByProductId(product.Id).Any())// картинки не апдейтим
				return;

			var picturesSourceUrlsList = picturesSourceUrls.Split(';');

			int i = 1;
			foreach (var curPicUrl in picturesSourceUrlsList)
			{
				if(curPicUrl == "")
					continue;

				var curPicUrlFull = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProductsCatalog", curPicUrl);

				var productPicture = new ProductPicture();
				productPicture.DisplayOrder = i++;

				productPicture.Picture = _pictureService.InsertPicture(
					System.IO.File.ReadAllBytes(curPicUrlFull),
					"image/jpeg",
					_pictureService.GetPictureSeName(product.Name),
					true);

				product.ProductPictures.Add(productPicture);
			}

			_productService.UpdateProduct(product);
		}

		private void SaveCategory(int shopCategoryId, int productId)
		{
			var productCategory = new ProductCategory()
				{
					ProductId = productId,
					CategoryId = shopCategoryId,
					IsFeaturedProduct = false,
					DisplayOrder = 1
				};
			_categoryService.InsertProductCategory(productCategory);
		}

		private void CheckStopAction()
		{
			if (mIsStopProducsImport)
			{
				var msg = "Stopped by user.";
				_logger.Debug(msg);
				throw new Exception(msg);
			}
		}
	}
}
