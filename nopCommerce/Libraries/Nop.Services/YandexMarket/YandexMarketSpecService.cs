namespace Nop.Services.YandexMarket
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Nop.Core;
	using Nop.Core.Caching;
	using Nop.Core.Data;
	using Nop.Core.Domain.YandexMarket;

	/// <summary>
	/// Tax rate service
	/// </summary>
	public partial class YandexMarketSpecService : IYandexMarketSpecService
	{
		#region Constants
		private const string YANDEXMARKETSpec_BY_CATEGORY_KEY = "Nop.YandexMarketSpec.category-{0}-{1}-{2}";
		private const string YANDEXMARKETSpec_PATTERN_KEY = "Nop.YandexMarketSpec.";
		#endregion

		#region Fields

		private readonly IRepository<YandexMarketSpecRecord> _specRepository;
		private readonly ICacheManager _cacheManager;

		#endregion

		#region Ctor
		
		public YandexMarketSpecService(ICacheManager cacheManager,
			IRepository<YandexMarketSpecRecord> specRepository)
		{
			this._cacheManager = cacheManager;
			this._specRepository = specRepository;
		}

		#endregion

		#region Methods
		
		public virtual void Delete(YandexMarketSpecRecord spec)
		{
			if (spec == null)
				throw new ArgumentNullException("YandexMarketSpecRecord");

			this._specRepository.Delete(spec);

			this._cacheManager.RemoveByPattern(YANDEXMARKETSpec_PATTERN_KEY);
		}

		public virtual IPagedList<YandexMarketSpecRecord> GetByCategory(int categoryId, int pageIndex = 0, int pageSize = int.MaxValue)
		{
			// Получить выборку из базы и приплюсовать форматирование продукта
			var key = string.Format(YANDEXMARKETSpec_BY_CATEGORY_KEY, pageIndex, pageSize, categoryId);

			var result = this._cacheManager.Get(key, () =>
			{				
				var products = this._specRepository.Table.Where(tr => tr.ProductRecord.YandexMarketCategoryRecordId == categoryId).Select(x => x.ProductRecord);
				foreach (var currentProduct in products)	 // formatting			
					currentProduct.FormatMe();

				var allSpecs = new List<YandexMarketSpecRecord>();
				foreach (var currentProduct in products)	// collecting
					allSpecs.AddRange(currentProduct.Specifications);

				return new PagedList<YandexMarketSpecRecord>(allSpecs, pageIndex, pageSize);
			});



			return result;
		}
		
		public virtual YandexMarketSpecRecord GetById(int specId)
		{
			if (specId == 0)
				return null;

			return this._specRepository.GetById(specId);
		}
		
		public virtual void Insert(YandexMarketSpecRecord spec)
		{
			if (spec == null)
				throw new ArgumentNullException("spec");

			this._specRepository.Insert(spec);

			this._cacheManager.RemoveByPattern(YANDEXMARKETSpec_PATTERN_KEY);
		}

		public virtual void Update(YandexMarketSpecRecord spec)
		{
			if (spec == null)
				throw new ArgumentNullException("spec");

			this._specRepository.Update(spec);

			this._cacheManager.RemoveByPattern(YANDEXMARKETSpec_PATTERN_KEY);
		}
		#endregion
	}
}
