namespace Nop.Services.YandexMarket
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Nop.Core;
	using Nop.Core.Caching;
	using Nop.Core.Data;
	using Nop.Core.Domain.YandexMarket;
	using Nop.Services.Catalog;
	using Nop.Services.SiteParsers.Xls;

	/// <summary>
	/// Tax rate service
	/// </summary>
	public sealed partial class YandexMarketProductService : IYandexMarketProductService
	{
		#region Constants
		private const string YANDEXMARKETProduct_BY_CATEGORY_KEY = "Nop.YandexMarketProduct.byCategory-{0}-{1}-{2}-{3}";
		private const string YANDEXMARKETProduct_BY_CATEGORY_KEY_WITH_FANTOMS = "Nop.YandexMarketProduct.allfantoms-{0}-{1}-{2}-{3}";
		private const string YANDEXMARKETProduct_PATTERN_KEY = "Nop.YandexMarketProduct.";
		#endregion

		#region Fields

		private readonly IRepository2<YandexMarketProductRecord> _productRepository;
		private readonly ICacheManager _cacheManager;
		private readonly IYandexMarketCategoryService _yandexMarketCategoryService;
		private readonly IProductService _productService;
		private readonly IYugCatalogPriceParserService _yugCatalogPriceParserService;

		#endregion

		#region Ctor
		
		public YandexMarketProductService(
			ICacheManager cacheManager,
			IRepository2<YandexMarketProductRecord> productRepository,
			IYandexMarketCategoryService yandexMarketCategoryService,
			IProductService productService,
			IYugCatalogPriceParserService yugCatalogPriceParserService)
		{
			this._cacheManager = cacheManager;
			this._productRepository = productRepository;
			this._yandexMarketCategoryService = yandexMarketCategoryService;
			this._productService = productService;
			_yugCatalogPriceParserService = yugCatalogPriceParserService;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Deletes a tax rate
		/// </summary>
		/// <param name="product">Tax rate</param>
		public void Delete(YandexMarketProductRecord product)
		{
			if (product == null)
				throw new ArgumentNullException("YandexMarketProductRecord");

			this._productRepository.Delete(product);

			this._cacheManager.RemoveByPattern(YANDEXMARKETProduct_PATTERN_KEY);
		}

		public void DeleteByCategory(int categoryId)
		{
			var itemsToDelete = GetByCategory(categoryId, withFantoms: true).ToList();
			itemsToDelete.ForEach(Delete);			
		}



		/// <summary>
		/// Gets all tax rates
		/// </summary>
		/// <returns>Tax rates</returns>
		public IPagedList<YandexMarketProductRecord> GetByCategory(int categoryId, bool isNotImportedOnly = false, int pageIndex = 0, int pageSize = int.MaxValue, bool withFantoms = false, bool withVendorExisting = true)
		{			
			if(categoryId == -1)
				return new PagedList<YandexMarketProductRecord>(new List<YandexMarketProductRecord>(), 0, 1);
			
			
			IQueryable<YandexMarketProductRecord> query = this._productRepository.Table;

			if (!withFantoms)
				query = query.Where(tr => tr.Name != "NotInPriceList");

			if (categoryId != 0)
				query = query.Where(tr => tr.YandexMarketCategoryRecordId == categoryId);


			IEnumerable<YandexMarketProductRecord> res1 = query.ToList();

			if (isNotImportedOnly)
			{					
				var shopCategory = _yandexMarketCategoryService.GetById(categoryId);
				var shopCategoryId = shopCategory != null ? shopCategory.ShopCategoryId : 0;
				var allShopProductsArtikuls = _productService.SearchProductVariants(shopCategoryId, 0, 0, "", false, 0, 2147483647, showHidden: true)
																	.Select(x => x.Sku).ToList();

				res1 = res1.Where(x => allShopProductsArtikuls.Contains(x.Articul) == false);
			}

			if (withVendorExisting)
			{
				var vendorArtikultList = _yugCatalogPriceParserService.ParseAndShow(false).ProductLineList.Select(x => x.Articul).ToList();

				res1 = res1.Where(yaProduct => vendorArtikultList.Contains(yaProduct.Articul));
			}



			res1 = res1.OrderBy(tr => tr.Name);

			var records = new PagedList<YandexMarketProductRecord>(res1.ToList(), pageIndex, pageSize);

			for (int i = 0; i < records.Count(); i++) 
				records[i] = records[i].Clone().FormatMe();

			return records;
			
		}		

		/// <summary>
		/// Gets a tax rate
		/// </summary>
		/// <param name="productId">Tax rate identifier</param>
		/// <returns>Tax rate</returns>
		public YandexMarketProductRecord GetById(int productId)
		{
			if (productId == 0)
				return null;

			var obj = this._productRepository.GetById(productId);
			_productRepository.Detach(obj);
			return obj.FormatMe();
		}

		/// <summary>
		/// Inserts a tax rate
		/// </summary>
		/// <param name="product">Tax rate</param>
		public void Insert(YandexMarketProductRecord product)
		{
			if (product == null)
				throw new ArgumentNullException("product");

			this._productRepository.Insert(product);

			this._cacheManager.RemoveByPattern(YANDEXMARKETProduct_PATTERN_KEY);
		}

		public void InsertList(IEnumerable<YandexMarketProductRecord> recordList)
		{
			foreach (var yandexMarketProductRecord in recordList) 
				this.Insert(yandexMarketProductRecord);
		}

		/// <summary>
		/// Updates the tax rate
		/// </summary>
		/// <param name="product">Tax rate</param>
		public void Update(YandexMarketProductRecord product)
		{
			if (product == null)
				throw new ArgumentNullException("product");

			this._productRepository.Update(product);

			this._cacheManager.RemoveByPattern(YANDEXMARKETProduct_PATTERN_KEY);
		}
		#endregion
	}
}
