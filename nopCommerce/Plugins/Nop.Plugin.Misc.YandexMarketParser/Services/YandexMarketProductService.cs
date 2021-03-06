namespace Nop.Plugin.Misc.YandexMarketParser.Services
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Nop.Core;
	using Nop.Core.Caching;
	using Nop.Core.Data;
	using Nop.Plugin.Misc.YandexMarketParser.Domain;

	/// <summary>
	/// Tax rate service
	/// </summary>
	public partial class YandexMarketProductService : IYandexMarketProductService
	{
		#region Constants
		private const string YANDEXMARKETProduct_ALL_KEY = "Nop.YandexMarketProduct.all-{0}-{1}";
		private const string YANDEXMARKETProduct_PATTERN_KEY = "Nop.YandexMarketProduct.";
		#endregion

		#region Fields

		private readonly IRepository<YandexMarketProductRecord> _productRepository;
		private readonly ICacheManager _cacheManager;

		#endregion

		#region Ctor

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="cacheManager">Cache manager</param>
		/// <param name="productRepository">Tax rate repository</param>
		public YandexMarketProductService(ICacheManager cacheManager,
			IRepository<YandexMarketProductRecord> productRepository)
		{
			this._cacheManager = cacheManager;
			this._productRepository = productRepository;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Deletes a tax rate
		/// </summary>
		/// <param name="product">Tax rate</param>
		public virtual void Delete(YandexMarketProductRecord product)
		{
			if (product == null)
				throw new ArgumentNullException("YandexMarketProductRecord");

			this._productRepository.Delete(product);

			this._cacheManager.RemoveByPattern(YANDEXMARKETProduct_PATTERN_KEY);
		}

		/// <summary>
		/// Gets all tax rates
		/// </summary>
		/// <returns>Tax rates</returns>
		public virtual IPagedList<YandexMarketProductRecord> GetByCategory(int categoryId, int pageIndex = 0, int pageSize = int.MaxValue)
		{
			string key = string.Format(YANDEXMARKETProduct_ALL_KEY, pageIndex, pageSize);

			var result = _cacheManager.Get(key, () =>
			{
				var query = from tr in this._productRepository.Table
							orderby tr.Name
							where tr.YandexMarketCategoryRecordId == categoryId
							select tr;

				var records = new PagedList<YandexMarketProductRecord>(query, pageIndex, pageSize);
				return records;
			});

			return result;
		}		

		/// <summary>
		/// Gets a tax rate
		/// </summary>
		/// <param name="productId">Tax rate identifier</param>
		/// <returns>Tax rate</returns>
		public virtual YandexMarketProductRecord GetById(int productId)
		{
			if (productId == 0)
				return null;

			return this._productRepository.GetById(productId);
		}

		/// <summary>
		/// Inserts a tax rate
		/// </summary>
		/// <param name="product">Tax rate</param>
		public virtual void Insert(YandexMarketProductRecord product)
		{
			if (product == null)
				throw new ArgumentNullException("product");

			this._productRepository.Insert(product);

			this._cacheManager.RemoveByPattern(YANDEXMARKETProduct_PATTERN_KEY);
		}

		public virtual void InsertList(IEnumerable<YandexMarketProductRecord> recordList)
		{
			foreach (var yandexMarketProductRecord in recordList) 
				Insert(yandexMarketProductRecord);
		}

		/// <summary>
		/// Updates the tax rate
		/// </summary>
		/// <param name="product">Tax rate</param>
		public virtual void Update(YandexMarketProductRecord product)
		{
			if (product == null)
				throw new ArgumentNullException("product");

			this._productRepository.Update(product);

			this._cacheManager.RemoveByPattern(YANDEXMARKETProduct_PATTERN_KEY);
		}
		#endregion
	}
}
