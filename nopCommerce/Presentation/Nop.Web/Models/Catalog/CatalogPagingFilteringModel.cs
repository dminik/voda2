using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.UI.Paging;

namespace Nop.Web.Models.Catalog
{
	using Filter = Nop.Core.Domain.Catalog.Filter;

	public class SpecOptionComp : IComparer<string>
	{		
		public int Compare(string x, string y)
		{
			x = this.GetFirstToken(x);
			y = this.GetFirstToken(y);

			double dX;
			var isXNum = double.TryParse(x, NumberStyles.Number, CultureInfo.CreateSpecificCulture("en-US"), out dX);

			double dY;
			var isYNum = double.TryParse(y, NumberStyles.Number, CultureInfo.CreateSpecificCulture("en-US"), out dY);

			if (isXNum && isYNum)
				return CompareDouble(dX, dY);

			if (isXNum) return 1;
			if (isYNum) return -1;
			
			return String.Compare(x, y, StringComparison.OrdinalIgnoreCase);
		}

		private int CompareDouble(double dX, double dY)
		{
			if (dX > dY)return 1;
			if (dX < dY) return -1;
			return 0;
		}

		private string GetFirstToken(string str)
		{
			str = str.Split(' ')[0];

			return str;
		}
	}

	public partial class CatalogPagingFilteringModel : BasePageableModel
    {
        #region Constructors

        public CatalogPagingFilteringModel()
        {
            this.AvailableSortOptions = new List<SelectListItem>();
            this.AvailableViewModes = new List<SelectListItem>();
            this.PageSizeOptions = new List<SelectListItem>();

            this.PriceRangeFilter = new PriceRangeFilterModel();
            this.SpecificationFilter = new SpecificationFilterModel();
	        OrderBy = (int)ProductSortingEnum.PriceAsc;
        }

        #endregion

        #region Properties
	    
        /// <summary>
        /// Price range filter model
        /// </summary>
        public PriceRangeFilterModel PriceRangeFilter { get; set; }

        /// <summary>
        /// Specification filter model
        /// </summary>
        public SpecificationFilterModel SpecificationFilter { get; set; }

		public int ShowWithPositiveQuantityCount { get; set; }
		public bool ShowWithPositiveQuantity { get; set; }
		public string ShowWithPositiveQuantityUrl { get; set; }

        public bool AllowProductSorting { get; set; }
        public IList<SelectListItem> AvailableSortOptions { get; set; }

        public bool AllowProductViewModeChanging { get; set; }
        public IList<SelectListItem> AvailableViewModes { get; set; }

        public bool AllowCustomersToSelectPageSize { get; set; }
        public IList<SelectListItem> PageSizeOptions { get; set; }

        /// <summary>
        /// Order by
        /// </summary>
        [NopResourceDisplayName("Categories.OrderBy")]
        public int OrderBy { get; set; }

        /// <summary>
        /// Product sorting
        /// </summary>
        [NopResourceDisplayName("Categories.ViewMode")]
        public string ViewMode { get; set; }
        

        #endregion

		string ExcludeQueryStringParams(string url, IWebHelper webHelper)
		{
			const string excludedQueryStringParams = "pagenumber"; //remove page filtering
			if (!String.IsNullOrEmpty(excludedQueryStringParams))
			{
				string[] excludedQueryStringParamsSplitted = excludedQueryStringParams.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string exclude in excludedQueryStringParamsSplitted)
				{
					url = webHelper.RemoveQueryString(url, exclude);
				}
			}

			return url;
		}

		public virtual void PrepareQuantFilters(bool isQuant, IWebHelper webHelper, IWorkContext workContext)
		{			
			const string QUERYSTRINGPARAM = "QuantFilter";

			string filterUrl = webHelper.ModifyQueryString(webHelper.GetThisPageUrl(true), QUERYSTRINGPARAM + "=" + !isQuant, null);
			filterUrl = ExcludeQueryStringParams(filterUrl, webHelper);
			ShowWithPositiveQuantityUrl = filterUrl;
			ShowWithPositiveQuantity = isQuant;
		}

		public virtual bool GetSelectedQuantFilter(IWebHelper webHelper)
		{
			const string QUERYSTRINGPARAM = "QuantFilter";
			string range = webHelper.QueryString<string>(QUERYSTRINGPARAM);
			if (String.IsNullOrEmpty(range))
				return false;

			return bool.Parse(range);			
		}
	   
	    #region Nested classes

        public partial class PriceRangeFilterModel : BaseNopModel
        {
            #region Const

            private const string QUERYSTRINGPARAM = "price";

            #endregion 

            #region Ctor

            public PriceRangeFilterModel()
            {
                this.Items = new List<PriceRangeFilterItem>();
            }

            #endregion

            #region Utilities

            /// <summary>
            /// Gets parsed price ranges
            /// </summary>
            protected virtual IList<PriceRange> GetPriceRangeList(string priceRangesStr, Filter filter, IPriceCalculationService priceCalculationService)
            {
				IEnumerable<int> allPrices = new List<int>();
				if(priceCalculationService != null)
					allPrices = priceCalculationService.GetPriceAmountForFilter(filter);

                var priceRanges = new List<PriceRange>();
                if (string.IsNullOrWhiteSpace(priceRangesStr))
                    return priceRanges;
                string[] rangeArray = priceRangesStr.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string str1 in rangeArray)
                {
                    string[] fromTo = str1.Trim().Split(new char[] { '-' });

                    decimal? from = null;
                    if (!String.IsNullOrEmpty(fromTo[0]) && !String.IsNullOrEmpty(fromTo[0].Trim()))
                        from = decimal.Parse(fromTo[0].Trim(), new CultureInfo("en-US"));

                    decimal? to = null;
                    if (!String.IsNullOrEmpty(fromTo[1]) && !String.IsNullOrEmpty(fromTo[1].Trim()))
                        to = decimal.Parse(fromTo[1].Trim(), new CultureInfo("en-US"));

					var amount = allPrices.Count(x => x >= (from ?? 1) && x <= (to ?? decimal.MaxValue));

					priceRanges.Add(new PriceRange() { From = from, To = to, Amount = amount });
                }
                return priceRanges;
            }

            protected virtual string ExcludeQueryStringParams(string url, IWebHelper webHelper)
            {
                var excludedQueryStringParams = "pagenumber"; //remove page filtering
                if (!String.IsNullOrEmpty(excludedQueryStringParams))
                {
                    string[] excludedQueryStringParamsSplitted = excludedQueryStringParams.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string exclude in excludedQueryStringParamsSplitted)
                        url = webHelper.RemoveQueryString(url, exclude);
                }

                return url;
            }

            #endregion

            #region Methods

			public virtual PriceRange GetSelectedPriceRange(IWebHelper webHelper, string priceRangesStr)
            {
                string range = webHelper.QueryString<string>(QUERYSTRINGPARAM);
                if (String.IsNullOrEmpty(range))
                    return null;
                string[] fromTo = range.Trim().Split(new char[] { '-' });
                if (fromTo.Length == 2)
                {
                    decimal? from = null;
                    if (!String.IsNullOrEmpty(fromTo[0]) && !String.IsNullOrEmpty(fromTo[0].Trim()))
                        from = decimal.Parse(fromTo[0].Trim(), new CultureInfo("en-US"));
                    decimal? to = null;
                    if (!String.IsNullOrEmpty(fromTo[1]) && !String.IsNullOrEmpty(fromTo[1].Trim()))
                        to = decimal.Parse(fromTo[1].Trim(), new CultureInfo("en-US"));

					var priceRangeList = GetPriceRangeList(priceRangesStr, new Filter(), null);
                    foreach (var pr in priceRangeList)
                    {
                        if (pr.From == from && pr.To == to)
                            return pr;
                    }
                }
                return null;
            }

            public virtual void LoadPriceRangeFilters(IPriceCalculationService priceCalculationService, Filter filter, string priceRangeStr, IWebHelper webHelper, IPriceFormatter priceFormatter)
            {
				var priceRangeList = GetPriceRangeList(priceRangeStr, filter, priceCalculationService);
                if (priceRangeList.Count > 0)
                {
                    this.Enabled = true;

                    var selectedPriceRange = GetSelectedPriceRange(webHelper, priceRangeStr);

                    this.Items = priceRangeList.ToList().Where(s => s.Amount > 0).Select(x =>
                    {
                        //from&to
                        var item = new PriceRangeFilterItem();
                        if (x.From.HasValue)
                            item.From = priceFormatter.FormatPrice(x.From.Value, true, false);
                        if (x.To.HasValue)
                            item.To = priceFormatter.FormatPrice(x.To.Value, true, false);
                        string fromQuery = string.Empty;
                        if (x.From.HasValue)
                            fromQuery = x.From.Value.ToString(new CultureInfo("en-US"));
                        string toQuery = string.Empty;
                        if (x.To.HasValue)
                            toQuery = x.To.Value.ToString(new CultureInfo("en-US"));

	                    item.Amount = x.Amount;

                        //is selected?
                        if (selectedPriceRange != null
                            && selectedPriceRange.From == x.From
                            && selectedPriceRange.To == x.To)
                            item.Selected = true;

                        //filter URL
                        string url = webHelper.ModifyQueryString(webHelper.GetThisPageUrl(true), QUERYSTRINGPARAM + "=" + fromQuery + "-" + toQuery, null);
                        url = ExcludeQueryStringParams(url, webHelper);
                        item.FilterUrl = url;


                        return item;
                    }).ToList();

                    if (selectedPriceRange != null)
                    {
                        //remove filter URL
                        string url = webHelper.RemoveQueryString(webHelper.GetThisPageUrl(true), QUERYSTRINGPARAM);
                        url = ExcludeQueryStringParams(url, webHelper);
                        this.RemoveFilterUrl = url;
                    }
                }
                else
                {
                    this.Enabled = false;
                }
            }
            
            #endregion

            #region Properties
            public bool Enabled { get; set; }
            public IList<PriceRangeFilterItem> Items { get; set; }
            public string RemoveFilterUrl { get; set; }

            #endregion
        }

        public partial class PriceRangeFilterItem : BaseNopModel
        {
            public string From { get; set; }
            public string To { get; set; }
            public string FilterUrl { get; set; }
            public int Amount { get; set; }
			public bool Selected { get; set; }
        }

        public partial class SpecificationFilterModel : BaseNopModel
        {
            #region Const

            private const string QUERYSTRINGPARAM = "specs";

            #endregion

            #region Ctor

            public SpecificationFilterModel()
            {
                this.AlreadyFilteredItems = new List<SpecificationFilterItem>();
                this.NotFilteredItems = new List<SpecificationFilterItem>();
            }

            #endregion

            #region Utilities

            protected virtual string ExcludeQueryStringParams(string url, IWebHelper webHelper)
            {
                var excludedQueryStringParams = "pagenumber"; //remove page filtering
                if (!String.IsNullOrEmpty(excludedQueryStringParams))
                {
                    string[] excludedQueryStringParamsSplitted = excludedQueryStringParams.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string exclude in excludedQueryStringParamsSplitted)
                    {
                        url = webHelper.RemoveQueryString(url, exclude);
                    }
                }

                return url;
            }
            
            protected virtual string GenerateFilteredSpecQueryParam(IList<int> optionIds)
            {
                string result = "";

                if (optionIds == null || optionIds.Count == 0)
                    return result;

                for (int i = 0; i < optionIds.Count; i++)
                {
                    result += optionIds[i];
                    if (i != optionIds.Count - 1)
                        result += ",";
                }
                return result;
            }

            #endregion

            #region Methods

            public virtual List<int> GetAlreadyFilteredSpecOptionIds(IWebHelper webHelper)
            {
                var result = new List<int>();

                string alreadyFilteredSpecsStr = webHelper.QueryString<string>(QUERYSTRINGPARAM);
                if (String.IsNullOrWhiteSpace(alreadyFilteredSpecsStr))
                    return result;

                foreach (var spec in alreadyFilteredSpecsStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    int specId = 0;
                    int.TryParse(spec.Trim(), out specId);
                    if (!result.Contains(specId))
                        result.Add(specId);
                }
                return result;
            }

			public virtual void PrepareSpecsFilters(
				int productsTotalAmount,
				IEnumerable<SpecificationAttributeService.SpecificationAttributeOptionWithCount> allOptionsCountTbl,
				IList<int> alreadyFilteredSpecOptionIds,
                IList<int> filterableSpecificationAttributeOptionIds,
                ISpecificationAttributeService specificationAttributeService, 
                IWebHelper webHelper,
                IWorkContext workContext)
            {	            				
                var allFilters = new List<SpecificationAttributeOptionFilter>();

	            if (filterableSpecificationAttributeOptionIds != null)
	            {		            
		            // Для оптимизации нужно вытащить сразу все спецификации и их значения 
		            var allSpecsAttribs = specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttributeList(filterableSpecificationAttributeOptionIds);

		            foreach (var sao in allSpecsAttribs) // цикл по айдишникам значений
		            {			            
			            if (sao != null)
			            {
				            var sa = sao.SpecificationAttribute; // вытаскиваем из БД спецификацию
				            if (sa != null)
				            {
								int existTimes = GetSpecificationAttributeOptionExistTimesInFilteredProducts(allOptionsCountTbl, sao.Id);

					            allFilters.Add(
						            new SpecificationAttributeOptionFilter
							            {
								            SpecificationAttributeId = sa.Id,
								            SpecificationAttributeName = sa.GetLocalized(x => x.Name, workContext.WorkingLanguage.Id),
								            SpecificationAttributeDisplayOrder = sa.DisplayOrder,
								            SpecificationAttributeOptionId = sao.Id,
								            SpecificationAttributeOptionName = sao.GetLocalized(x => x.Name, workContext.WorkingLanguage.Id),
								            SpecificationAttributeOptionDisplayOrder = sao.DisplayOrder,
								            SpecificationAttributeOptionExistTimesInFilteredProducts = existTimes
							            });
				            }
			            }
		            } // end for
	            }

	            //sort loaded options
                allFilters = allFilters.OrderBy(saof => saof.SpecificationAttributeDisplayOrder)
                    .ThenBy(saof => saof.SpecificationAttributeName)
                    .ThenBy(saof => saof.SpecificationAttributeOptionDisplayOrder)
					.ThenBy(saof => saof.SpecificationAttributeOptionName, new SpecOptionComp()).ToList();
                
                //get already filtered specification options
                var alreadyFilteredOptions = allFilters
                    .Where(x => alreadyFilteredSpecOptionIds.Contains(x.SpecificationAttributeOptionId))
                    .Select(x => x)
                    .ToList();

                //get not filtered specification options
                var notFilteredOptions = new List<SpecificationAttributeOptionFilter>();
                foreach (var saof in allFilters)
                {
                    //do not add already filtered specification options
                    if (alreadyFilteredOptions.FirstOrDefault(x => x.SpecificationAttributeId == saof.SpecificationAttributeId) != null)
                        continue;

                    //else add it
					if (productsTotalAmount != saof.SpecificationAttributeOptionExistTimesInFilteredProducts)	                            
						notFilteredOptions.Add(saof);
                }

                //prepare the model properties
                if (alreadyFilteredOptions.Count > 0 || notFilteredOptions.Count > 0)
                {
                    this.Enabled = true;
                    
                    this.AlreadyFilteredItems = alreadyFilteredOptions.ToList().Select(x =>
                    {
                        var item = new SpecificationFilterItem();
                        item.SpecificationAttributeName = x.SpecificationAttributeName;
						item.SpecificationAttributeOptionName = RemoveHtmlTags(x.SpecificationAttributeOptionName);

						//filter URL						
						var alreadyFilteredOptionIds = GetAlreadyFilteredSpecOptionIds(webHelper);
						if (alreadyFilteredOptionIds.Contains(x.SpecificationAttributeOptionId))
							alreadyFilteredOptionIds.Remove(x.SpecificationAttributeOptionId);// add to old list me

						string newQueryParam = GenerateFilteredSpecQueryParam(alreadyFilteredOptionIds);
						string filterUrl = webHelper.ModifyQueryString(webHelper.GetThisPageUrl(true), QUERYSTRINGPARAM + "=" + newQueryParam, null);
						filterUrl = ExcludeQueryStringParams(filterUrl, webHelper);
						item.FilterUrl = filterUrl;

                        return item;
                    }).ToList();

                    this.NotFilteredItems = notFilteredOptions.ToList().Select(x =>
                    {
                        var item = new SpecificationFilterItem();
                        item.SpecificationAttributeName = x.SpecificationAttributeName;
						item.SpecificationAttributeOptionName = RemoveHtmlTags(x.SpecificationAttributeOptionName);
						item.SpecificationAttributeOptionExistTimesInFilteredProducts = x.SpecificationAttributeOptionExistTimesInFilteredProducts;

                        //filter URL						
                        var alreadyFilteredOptionIds = GetAlreadyFilteredSpecOptionIds(webHelper);
                        if (!alreadyFilteredOptionIds.Contains(x.SpecificationAttributeOptionId))
							alreadyFilteredOptionIds.Add(x.SpecificationAttributeOptionId);// add to old list me

                        string newQueryParam = GenerateFilteredSpecQueryParam(alreadyFilteredOptionIds);
                        string filterUrl = webHelper.ModifyQueryString(webHelper.GetThisPageUrl(true), QUERYSTRINGPARAM + "=" + newQueryParam, null);
                        filterUrl = ExcludeQueryStringParams(filterUrl, webHelper);
                        item.FilterUrl = filterUrl;
                        
                        return item;
                    }).ToList();


                    //remove filter URL
                    string removeFilterUrl = webHelper.RemoveQueryString(webHelper.GetThisPageUrl(true), QUERYSTRINGPARAM);
                    removeFilterUrl = ExcludeQueryStringParams(removeFilterUrl, webHelper);
                    this.RemoveFilterUrl = removeFilterUrl;
                }
                else
                {
                    this.Enabled = false;
                }
            }
			
			private string RemoveHtmlTags(string str)
			{
				/*
						<p>С QWERTY-клавиатурой</p>
						<p>Моноблок</p>
				  
					replace to 
				  
						С QWERTY-клавиатурой,
						Моноблок
				 */

				var result = "";

				var strArr = str.Split(new string[] {"/p>"}, StringSplitOptions.None);
				
				foreach (string s in strArr)
				{															
					result += s.Replace("<p>", "").Replace("<", "").Replace("p>", "");
					result += ",";
				}

				result = result.TrimEnd(',');

				result = result.Replace(",", "<br/>");

				return result;
			}

			/// <summary>
			/// Вместо products - нужно получить все AttrOptionId 
			/// </summary>						
			private int GetSpecificationAttributeOptionExistTimesInFilteredProducts(
				IEnumerable<SpecificationAttributeService.SpecificationAttributeOptionWithCount> tbl, 
				int specAttrOptId)
			{
				return tbl.Where(x => x.SpecificationAttributeOptionId == specAttrOptId).Select(s => s.ProductCount).FirstOrDefault();
			}

	        #endregion

            #region Properties
            public bool Enabled { get; set; }
            public IList<SpecificationFilterItem> AlreadyFilteredItems { get; set; }
            public IList<SpecificationFilterItem> NotFilteredItems { get; set; }
            public string RemoveFilterUrl { get; set; }

            #endregion
        }

        public partial class SpecificationFilterItem : BaseNopModel
        {
            public string SpecificationAttributeName { get; set; }
            public string SpecificationAttributeOptionName { get; set; }
			public int SpecificationAttributeOptionExistTimesInFilteredProducts { get; set; }
            public string FilterUrl { get; set; }			
        }

        #endregion
    }
}