using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Services.Events;

namespace Nop.Services.Catalog
{
	using System.Data;

	using Nop.Core;
	using Nop.Data;

	/// <summary>
    /// Specification attribute service
    /// </summary>
	public partial class SpecificationAttributeService : BaseEntity, ISpecificationAttributeService, ISpecificationAttributeService2
    {
		#region Nested classes

		public class SpecificationAttributeOptionWithCount : BaseEntity
		{
			public int SpecificationAttributeOptionId { get; set; }
			public int ProductCount { get; set; }
		}

		#endregion

        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// {1} : allow filtering
        /// {2} : show on product page
        /// </remarks>
        private const string PRODUCTSPECIFICATIONATTRIBUTE_ALLBYPRODUCTID_KEY = "Nop.productspecificationattribute.allbyproductid-{0}-{1}-{2}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY = "Nop.productspecificationattribute.";
        #endregion

        #region Fields
        
        private readonly IRepository<SpecificationAttribute> _specificationAttributeRepository;
        private readonly IRepository<SpecificationAttributeOption> _specificationAttributeOptionRepository;
        private readonly IRepository2<ProductSpecificationAttribute> _productSpecificationAttributeRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
		private readonly IDataProvider _dataProvider;
		private readonly IDbContext2 _dbContext;

        #endregion

        #region Ctor

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="cacheManager">Cache manager</param>
		/// <param name="specificationAttributeRepository">Specification attribute repository</param>
		/// <param name="specificationAttributeOptionRepository">Specification attribute option repository</param>
		/// <param name="productSpecificationAttributeRepository">Product specification attribute repository</param>
		/// <param name="eventPublisher">Event published</param>
		/// <param name="dataProvider">data Provider</param>
		public SpecificationAttributeService(ICacheManager cacheManager,
            IRepository<SpecificationAttribute> specificationAttributeRepository,
            IRepository<SpecificationAttributeOption> specificationAttributeOptionRepository,
            IRepository2<ProductSpecificationAttribute> productSpecificationAttributeRepository,
            IEventPublisher eventPublisher,
			IDataProvider dataProvider,
			IDbContext2 dbContext)
        {
            _cacheManager = cacheManager;
            _specificationAttributeRepository = specificationAttributeRepository;
            _specificationAttributeOptionRepository = specificationAttributeOptionRepository;
            _productSpecificationAttributeRepository = productSpecificationAttributeRepository;
            _eventPublisher = eventPublisher;
			_dataProvider = dataProvider;
			_dbContext = dbContext;
        }

        #endregion

        #region Methods

        #region Specification attribute

        /// <summary>
        /// Gets a specification attribute
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <returns>Specification attribute</returns>
        public virtual SpecificationAttribute GetSpecificationAttributeById(int specificationAttributeId)
        {
            if (specificationAttributeId == 0)
                return null;

            return _specificationAttributeRepository.GetById(specificationAttributeId);
        }

        /// <summary>
        /// Gets specification attributes
        /// </summary>
        /// <returns>Specification attributes</returns>
        public virtual IList<SpecificationAttribute> GetSpecificationAttributes()
        {
            var query = from sa in _specificationAttributeRepository.Table
                        orderby sa.DisplayOrder
                        select sa;
            var specificationAttributes = query.ToList();
            return specificationAttributes;
        }

        /// <summary>
        /// Deletes a specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        public virtual void DeleteSpecificationAttribute(SpecificationAttribute specificationAttribute)
        {
            if (specificationAttribute == null)
                throw new ArgumentNullException("specificationAttribute");

            _specificationAttributeRepository.Delete(specificationAttribute);

            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(specificationAttribute);
        }

        /// <summary>
        /// Inserts a specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        public virtual void InsertSpecificationAttribute(SpecificationAttribute specificationAttribute)
        {
            if (specificationAttribute == null)
                throw new ArgumentNullException("specificationAttribute");

            _specificationAttributeRepository.Insert(specificationAttribute);

            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(specificationAttribute);
        }

        /// <summary>
        /// Updates the specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        public virtual void UpdateSpecificationAttribute(SpecificationAttribute specificationAttribute)
        {
            if (specificationAttribute == null)
                throw new ArgumentNullException("specificationAttribute");

            _specificationAttributeRepository.Update(specificationAttribute);

            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(specificationAttribute);
        }

        #endregion

        #region Specification attribute option

        /// <summary>
        /// Gets a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOptionId">The specification attribute option identifier</param>
        /// <returns>Specification attribute option</returns>
        public virtual SpecificationAttributeOption GetSpecificationAttributeOptionById(int specificationAttributeOptionId)
        {
            if (specificationAttributeOptionId == 0)
                return null;

            return _specificationAttributeOptionRepository.GetById(specificationAttributeOptionId);
        }

        /// <summary>
        /// Gets a specification attribute option by specification attribute id
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <returns>Specification attribute option</returns>
        public virtual IList<SpecificationAttributeOption> GetSpecificationAttributeOptionsBySpecificationAttribute(int specificationAttributeId)
        {
            var query = from sao in _specificationAttributeOptionRepository.Table
                        orderby sao.DisplayOrder
                        where sao.SpecificationAttributeId == specificationAttributeId
                        select sao;
            var specificationAttributeOptions = query.ToList();
            return specificationAttributeOptions;
        }

		public virtual IList<SpecificationAttributeOption> GetSpecificationAttributeOptionsBySpecificationAttributeList(IEnumerable<int> specificationAttributeOptionIdList)
		{
			var query = from sao in _specificationAttributeOptionRepository.Table						
						where specificationAttributeOptionIdList.Contains(sao.Id)
						select sao;
			var specificationAttributeOptions = query.ToList();
			return specificationAttributeOptions;
		}

        /// <summary>
        /// Deletes a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        public virtual void DeleteSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption)
        {
            if (specificationAttributeOption == null)
                throw new ArgumentNullException("specificationAttributeOption");

            _specificationAttributeOptionRepository.Delete(specificationAttributeOption);

            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(specificationAttributeOption);
        }

        /// <summary>
        /// Inserts a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        public virtual void InsertSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption)
        {
            if (specificationAttributeOption == null)
                throw new ArgumentNullException("specificationAttributeOption");

            _specificationAttributeOptionRepository.Insert(specificationAttributeOption);

            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(specificationAttributeOption);
        }

		/// <summary>
		/// Inserts a product specification attribute mapping
		/// </summary>
		/// <param name="productSpecificationAttributeList">Product specification attribute mapping</param>
		public virtual void InsertProductSpecificationAttributeList(IEnumerable<ProductSpecificationAttribute> productSpecificationAttributeList)
		{
			_productSpecificationAttributeRepository.InsertList(productSpecificationAttributeList);

			_cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);			
		}

        /// <summary>
        /// Updates the specification attribute
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        public virtual void UpdateSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption)
        {
            if (specificationAttributeOption == null)
                throw new ArgumentNullException("specificationAttributeOption");

            _specificationAttributeOptionRepository.Update(specificationAttributeOption);

            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(specificationAttributeOption);
        }

		/// <summary>
		/// Search products count
		/// </summary>		
		public virtual IEnumerable<SpecificationAttributeOptionWithCount> SearchProductsCount(
			out int positiveQuantityCount,
			IList<int> categoryIds = null,			
			bool? featuredProducts = null,
			decimal? priceMin = null,
			decimal? priceMax = null,			
			IList<int> filteredSpecs = null,			
			bool showWithPositiveQuantity = false)
		{						
			//validate "categoryIds" parameter
			if (categoryIds != null && categoryIds.Contains(0))
				categoryIds.Remove(0);

			//pass category identifiers as comma-delimited string
			string commaSeparatedCategoryIds = "";
			if (categoryIds != null)
			{
				for (int i = 0; i < categoryIds.Count; i++)
				{
					commaSeparatedCategoryIds += categoryIds[i].ToString();
					if (i != categoryIds.Count - 1)
					{
						commaSeparatedCategoryIds += ",";
					}
				}
			}
				
			string commaSeparatedSpecIds = "";
			if (filteredSpecs != null)
			{
				((List<int>)filteredSpecs).Sort();
				for (int i = 0; i < filteredSpecs.Count; i++)
				{
					commaSeparatedSpecIds += filteredSpecs[i].ToString();
					if (i != filteredSpecs.Count - 1)
					{
						commaSeparatedSpecIds += ",";
					}
				}
			}
				
			//prepare parameters
			var pCategoryIds = _dataProvider.GetParameter();
			pCategoryIds.ParameterName = "CategoryIds";
			pCategoryIds.Value = (object)commaSeparatedCategoryIds;
			pCategoryIds.DbType = DbType.String;
						
			var pPriceMin = _dataProvider.GetParameter();
			pPriceMin.ParameterName = "PriceMin";
			pPriceMin.Value = priceMin.HasValue ? (object)priceMin.Value : DBNull.Value;
			pPriceMin.DbType = DbType.Decimal;

			var pPriceMax = _dataProvider.GetParameter();
			pPriceMax.ParameterName = "PriceMax";
			pPriceMax.Value = priceMax.HasValue ? (object)priceMax.Value : DBNull.Value;
			pPriceMax.DbType = DbType.Decimal;			

			var pFilteredSpecs = _dataProvider.GetParameter();
			pFilteredSpecs.ParameterName = "FilteredSpecs";
			pFilteredSpecs.Value = (object)commaSeparatedSpecIds;
			pFilteredSpecs.DbType = DbType.String;

			var pShowWithPositiveQuantity = _dataProvider.GetParameter();
			pShowWithPositiveQuantity.ParameterName = "ShowWithPositiveQuantity";
			pShowWithPositiveQuantity.Value = showWithPositiveQuantity;
			pShowWithPositiveQuantity.DbType = DbType.Boolean;

			var pPositiveQuantityCount = _dataProvider.GetParameter();
            pPositiveQuantityCount.ParameterName = "PositiveQuantityCount";
            pPositiveQuantityCount.Direction = ParameterDirection.Output;
            pPositiveQuantityCount.DbType = DbType.Int32;

                //invoke stored procedure
			var products = _dbContext.ExecuteStoredProcedureList2<SpecificationAttributeOptionWithCount>(
				"ProductLoadAllPaged2",
				pCategoryIds,				
				pPriceMin,
				pPriceMax,
				pFilteredSpecs,
				pShowWithPositiveQuantity,
				pPositiveQuantityCount);

			positiveQuantityCount = (pPositiveQuantityCount.Value != DBNull.Value) ? Convert.ToInt32(pPositiveQuantityCount.Value) : 0;
			
			return products;							
		}

        #endregion

        #region Product specification attribute

        /// <summary>
        /// Deletes a product specification attribute mapping
        /// </summary>
        /// <param name="productSpecificationAttribute">Product specification attribute</param>
        public virtual void DeleteProductSpecificationAttribute(ProductSpecificationAttribute productSpecificationAttribute)
        {
            if (productSpecificationAttribute == null)
                throw new ArgumentNullException("productSpecificationAttribute");

            _productSpecificationAttributeRepository.Delete(productSpecificationAttribute);

            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(productSpecificationAttribute);
        }

        /// <summary>
        /// Gets a product specification attribute mapping collection
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product specification attribute mapping collection</returns>
        public virtual IList<ProductSpecificationAttribute> GetProductSpecificationAttributesByProductId(int productId)
        {
            return GetProductSpecificationAttributesByProductId(productId, null, null);
        }

        /// <summary>
        /// Gets a product specification attribute mapping collection
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="allowFiltering">0 to load attributes with AllowFiltering set to false, 0 to load attributes with AllowFiltering set to true, null to load all attributes</param>
        /// <param name="showOnProductPage">0 to load attributes with ShowOnProductPage set to false, 0 to load attributes with ShowOnProductPage set to true, null to load all attributes</param>
        /// <returns>Product specification attribute mapping collection</returns>
        public virtual IList<ProductSpecificationAttribute> GetProductSpecificationAttributesByProductId(int productId, 
            bool? allowFiltering, bool? showOnProductPage)
        {
            string allowFilteringCacheStr = "null";
            if (allowFiltering.HasValue)
                allowFilteringCacheStr = allowFiltering.ToString();
            string showOnProductPageCacheStr = "null";
            if (showOnProductPage.HasValue)
                showOnProductPageCacheStr = showOnProductPage.ToString();
            string key = string.Format(PRODUCTSPECIFICATIONATTRIBUTE_ALLBYPRODUCTID_KEY, productId, allowFilteringCacheStr, showOnProductPageCacheStr);
            
            return _cacheManager.Get(key, () =>
            {
                var query = _productSpecificationAttributeRepository.Table;
                query = query.Where(psa => psa.ProductId == productId);
                if (allowFiltering.HasValue)
                    query = query.Where(psa => psa.AllowFiltering == allowFiltering.Value);
                if (showOnProductPage.HasValue)
                    query = query.Where(psa => psa.ShowOnProductPage == showOnProductPage.Value);
                query = query.OrderBy(psa => psa.DisplayOrder);

                var productSpecificationAttributes = query.ToList();
                return productSpecificationAttributes;
            });
        }

        /// <summary>
        /// Gets a product specification attribute mapping 
        /// </summary>
        /// <param name="productSpecificationAttributeId">Product specification attribute mapping identifier</param>
        /// <returns>Product specification attribute mapping</returns>
        public virtual ProductSpecificationAttribute GetProductSpecificationAttributeById(int productSpecificationAttributeId)
        {
            if (productSpecificationAttributeId == 0)
                return null;
            
            return _productSpecificationAttributeRepository.GetById(productSpecificationAttributeId);
        }

        /// <summary>
        /// Inserts a product specification attribute mapping
        /// </summary>
        /// <param name="productSpecificationAttribute">Product specification attribute mapping</param>
        public virtual void InsertProductSpecificationAttribute(ProductSpecificationAttribute productSpecificationAttribute)
        {
            if (productSpecificationAttribute == null)
                throw new ArgumentNullException("productSpecificationAttribute");

            _productSpecificationAttributeRepository.Insert(productSpecificationAttribute);

            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(productSpecificationAttribute);
        }

        /// <summary>
        /// Updates the product specification attribute mapping
        /// </summary>
        /// <param name="productSpecificationAttribute">Product specification attribute mapping</param>
        public virtual void UpdateProductSpecificationAttribute(ProductSpecificationAttribute productSpecificationAttribute)
        {
            if (productSpecificationAttribute == null)
                throw new ArgumentNullException("productSpecificationAttribute");

            _productSpecificationAttributeRepository.Update(productSpecificationAttribute);

            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(productSpecificationAttribute);
        }

        #endregion

        #endregion
    }
}
