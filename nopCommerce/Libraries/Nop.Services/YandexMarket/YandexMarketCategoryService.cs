namespace Nop.Services.YandexMarket
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Mvc;

	using Nop.Core;
	using Nop.Core.Caching;
	using Nop.Core.Data;
	using Nop.Core.Domain.YandexMarket;

	/// <summary>
	/// Tax rate service
	/// </summary>
	public partial class YandexMarketCategoryService : IYandexMarketCategoryService
	{
		#region Constants
		private const string YANDEXMARKETCATEGORY_ALL_KEY = "Nop.YandexMarketCategory.all-{0}-{1}";
		private const string YANDEXMARKETCATEGORY_PATTERN_KEY = "Nop.YandexMarketCategory.";
		#endregion

		#region Fields

		private readonly IRepository<YandexMarketCategoryRecord> _categoryRepository;
		private readonly ICacheManager _cacheManager;

		#endregion

		#region Ctor

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="cacheManager">Cache manager</param>
		/// <param name="categoryRepository">Tax rate repository</param>
		public YandexMarketCategoryService(ICacheManager cacheManager,
			IRepository<YandexMarketCategoryRecord> categoryRepository)
		{
			this._cacheManager = cacheManager;
			this._categoryRepository = categoryRepository;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Deletes a tax rate
		/// </summary>
		/// <param name="category">Tax rate</param>
		public virtual void Delete(YandexMarketCategoryRecord category)
		{
			if (category == null)
				throw new ArgumentNullException("YandexMarketCategoryRecord");

			this._categoryRepository.Delete(category);

			this._cacheManager.RemoveByPattern(YANDEXMARKETCATEGORY_PATTERN_KEY);
		}

		/// <summary>
		/// Gets all tax rates
		/// </summary>
		/// <returns>Tax rates</returns>
		public virtual IPagedList<YandexMarketCategoryRecord> GetAll(int pageIndex = 0, int pageSize = int.MaxValue)
		{
			string key = string.Format(YANDEXMARKETCATEGORY_ALL_KEY, pageIndex, pageSize);

			var result = this._cacheManager.Get(key, () =>
			{
				var query = from tr in this._categoryRepository.Table
							orderby tr.Name
							select tr;
				var records = new PagedList<YandexMarketCategoryRecord>(query, pageIndex, pageSize);
				return records;
			});

			return result;
		}


		/// <summary>
		/// Gets a tax rate
		/// </summary>
		/// <param name="categoryId">Tax rate identifier</param>
		/// <returns>Tax rate</returns>
		public virtual YandexMarketCategoryRecord GetById(int categoryId)
		{
			if (categoryId == 0)
				return null;

			return this._categoryRepository.GetById(categoryId);
		}

		/// <summary>
		/// Inserts a tax rate
		/// </summary>
		/// <param name="category">Tax rate</param>
		public virtual void Insert(YandexMarketCategoryRecord category)
		{
			if (category == null)
				throw new ArgumentNullException("category");

			this._categoryRepository.Insert(category);

			this._cacheManager.RemoveByPattern(YANDEXMARKETCATEGORY_PATTERN_KEY);
		}

		/// <summary>
		/// Updates the tax rate
		/// </summary>
		/// <param name="category">Tax rate</param>
		public virtual void Update(YandexMarketCategoryRecord category)
		{
			if (category == null)
				throw new ArgumentNullException("category");

			this._categoryRepository.Update(category);

			this._cacheManager.RemoveByPattern(YANDEXMARKETCATEGORY_PATTERN_KEY);
		}

		public List<SelectListItem> GetCategoriesForDDL()
		{
			var result = new List<SelectListItem>();
			result.Add(new SelectListItem() { Text = "---", Value = "0" });

			var categories = GetAll();
			var ddlList = categories.Select(c => new SelectListItem() { Text = c.Name, Value = c.Id.ToString() }).ToList();
			result.AddRange(ddlList);

			return result;
		}

		#endregion
	}
}
