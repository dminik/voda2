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
	using Nop.Core.Domain.Catalog;
	using Nop.Core.Domain.YandexMarket;
	using Nop.Services.Catalog;
	using Nop.Services.Media;
	using Nop.Services.Security;
	using Nop.Services.Seo;
	using Nop.Services.SiteParsers;
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
		private readonly IManufacturerService _manufacturerService;
		private readonly IPictureService _pictureService;
		private readonly ISpecificationAttributeService _specificationAttributeService;

		public YandexMarketProductController(IYandexMarketProductService yandexMarketProductService,
			IProductService productService,
			IUrlRecordService urlRecordService,
			ICategoryService categoryService,
			IManufacturerService manufacturerService,
			IPictureService pictureService,
			ISpecificationAttributeService specificationAttributeService)
		{
			_yandexMarketProductService = yandexMarketProductService;
			this._productService = productService;
			this._urlRecordService = urlRecordService;
			this._categoryService = categoryService;
			this._manufacturerService = manufacturerService;
			this._pictureService = pictureService;
			_specificationAttributeService = specificationAttributeService;
		}

		#region Grid actions

		[HttpPost]
		[GridAction(EnableCustomBinding = true)]
		public ActionResult ListProduct(int parserCategoryId, GridCommand command)
		{
			var records = _yandexMarketProductService.GetByCategory(parserCategoryId, command.Page - 1, command.PageSize);
				
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

			return ListProduct(product.YandexMarketCategoryRecordId, command);
		}

		[GridAction(EnableCustomBinding = true)]
		public ActionResult DeleteProduct(int id, GridCommand command)
		{
			var product = _yandexMarketProductService.GetById(id);
			int parserCategoryId = product.YandexMarketCategoryRecordId;

			if (product != null) this._yandexMarketProductService.Delete(product);

			return ListProduct(parserCategoryId, command);
		}

		#endregion

		[HttpPost]
		public ActionResult ImportProductList(int parserCategoryId, int shopCategoryId)
		{
			// TODO цикл по активным категориям

			var records = _yandexMarketProductService.GetByCategory(parserCategoryId);

			foreach (var curYaProduct in records)
			{
				ImportYaProduct(curYaProduct, shopCategoryId);
			}

			return Content("Success");
		}

		private void ImportYaProduct(YandexMarketProductRecord yaProduct, int shopCategoryId)
		{
			Product product;
			var variant = _productService.GetProductVariantBySku(yaProduct.Name);
			bool isUpdating = variant != null;

			// create or update item
			if (isUpdating)
			{
				product = variant.Product;
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
						ManageInventoryMethodId = 1,
						DisplayStockAvailability = true,
						DisplayStockQuantity = true,
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
				_productService.InsertProduct(product);
				variant.ProductId = product.Id;
				_productService.InsertProductVariant(variant);
				
				SaveCategory(shopCategoryId, product.Id);
				SaveSpecList(product, yaProduct.Specifications);


				SavePictures(product, yaProduct.ImageUrl_1);

				//search engine name
				var seName = product.ValidateSeName(product.Name, product.Name, true);
				_urlRecordService.SaveSlug(product, seName, 0);
			}
		}

		private string CreateShortDescription(IEnumerable<YandexMarketSpecRecord> specList)
		{
			return "";
		
			var attribsForShortDescription = new List<string>()
				{
					"Тип фильтра",
					"Число ступеней очистки",
					"Отдельный кран"
				};

			var ulShortDescription = new XElement("ul");
			var xmlShortDescription = 
				new XElement("div", new XAttribute("class", "ShortDescription"),
					ulShortDescription);
			
			foreach (var yandexMarketSpecRecord in specList.Where(yandexMarketSpecRecord => attribsForShortDescription.Contains(yandexMarketSpecRecord.Key)))
			{
				ulShortDescription.Add(new XElement("li", yandexMarketSpecRecord.Key + ": " + yandexMarketSpecRecord.Value));
			}

			return xmlShortDescription.ToString();
		}

		private void SaveSpecList(Product product, IEnumerable<YandexMarketSpecRecord> specList)
		{
			

			var allSpecAttrList = _specificationAttributeService.GetSpecificationAttributes();

			foreach (var yandexMarketSpecRecord in specList)
			{
				var attributeOptionId = FindAttributeOptionId(yandexMarketSpecRecord.Key, yandexMarketSpecRecord.Value, allSpecAttrList);

				var psa = new ProductSpecificationAttribute()
				{
					SpecificationAttributeOptionId = attributeOptionId,
					ProductId = product.Id,
					AllowFiltering = YandexMarketHelpers.GetAllowFilteringForProductSelector().Contains(yandexMarketSpecRecord.Key),
					ShowOnProductPage = true,
				};

				_specificationAttributeService.InsertProductSpecificationAttribute(psa);
			}
		}

		private int FindAttributeOptionId(string attrName, string attrOptName, IEnumerable<SpecificationAttribute> allSpecAttrList)
		{
			var resultAttrName = allSpecAttrList.SingleOrDefault(x => x.Name == attrName);

			if (resultAttrName == null)
				throw new Exception("Cant find Product attribute by name " + attrName);

			var resultAttrOptName = resultAttrName.SpecificationAttributeOptions.SingleOrDefault(s => s.Name == attrOptName);

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
	}
}
