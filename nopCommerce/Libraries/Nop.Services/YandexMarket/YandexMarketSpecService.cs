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
		private const string YANDEXMARKETSpec_PATTERN_KEY = "Nop.YandexMarketSpec.";
		#endregion

		#region Fields

		private readonly IRepository<YandexMarketSpecRecord> _specRepository;
		private readonly ICacheManager _cacheManager;
		private IYandexMarketProductService _yandexMarketProductService;

		#endregion

		#region Ctor
		
		public YandexMarketSpecService(ICacheManager cacheManager,
			IRepository<YandexMarketSpecRecord> specRepository,
			IYandexMarketProductService yandexMarketProductService)
		{
			this._cacheManager = cacheManager;
			this._specRepository = specRepository;
			_yandexMarketProductService = yandexMarketProductService;
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
			var products = _yandexMarketProductService.GetByCategory(categoryId);

			var allSpecs = new List<YandexMarketSpecRecord>();
			foreach (var currentProduct in products)	// collecting
				allSpecs.AddRange(currentProduct.Specifications);

			return new PagedList<YandexMarketSpecRecord>(allSpecs, pageIndex, pageSize);			
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
