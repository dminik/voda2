namespace Nop.Services.SiteParsers.Categories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Newtonsoft.Json;

	using Nop.Core.Domain.YandexMarket;

	public class UgContractParserCategory : BaseParserCategories
	{
		YandexMarketCategoryRecord mNeededCategory = null;

		// List
		//protected override string CssSelectorForProductLinkInProductList { get { return ".goods-list:not(.goods-list-small) div.cat-item a"; } }
		//protected override string CssSelectorForNextLinkInProductList { get { return "[title=следующая]"; } }
		//protected override string NextLinkInProductListName { get { return "следующая"; } }
		
		//// Product
		//protected override string CssSelectorForProductArticulInProductPage { get { return ".good-info-number"; } } 
		//protected override string CssSelectorForProductNameInProductPage { get { return "h1.good-info-header"; } }
		//protected override string CssSelectorForProductPictureInProductPage { get { return ".lightbox img"; } }
		//protected override string CssSelectorForProductPicturesInProductPage { get { return "div.small-photos ul#images-list li a"; } }
		//protected override string CssSelectorForProductFullDescriptionInProductPage { get { return "div.panes-item.goods-item-panes-item.content"; } }
		
		//// Product specs
		//protected override string GetLinkToSpecsTabOfProduct(string productLink) { return productLink + "#chars"; }
		//protected override string CssSelectorForProductSpecsRowsInProductPage { get { return ".properties-table tr"; } }
		//protected override string CssSelectorForProductSpecKeyInProductPage { get { return "td.prop"; } }
		//protected override string CssSelectorForProductSpecValInProductPage { get { return "td.val"; } }		

		class JsonCategory
		{
#pragma warning disable 649
			public string id;
#pragma warning restore 649
#pragma warning disable 649
// ReSharper disable InconsistentNaming
			public int level;
// ReSharper restore InconsistentNaming
#pragma warning restore 649
#pragma warning disable 649
// ReSharper disable InconsistentNaming
			public string pid;
// ReSharper restore InconsistentNaming
#pragma warning restore 649
// ReSharper disable InconsistentNaming
#pragma warning disable 169
			public bool expanded = false;
#pragma warning restore 169
// ReSharper restore InconsistentNaming
#pragma warning disable 649
// ReSharper disable InconsistentNaming
			public bool have_children;
// ReSharper restore InconsistentNaming
#pragma warning restore 649
#pragma warning disable 649
// ReSharper disable InconsistentNaming
			public string name;
// ReSharper restore InconsistentNaming
#pragma warning restore 649
// ReSharper disable InconsistentNaming
			public bool active = false;
// ReSharper restore InconsistentNaming
// ReSharper disable InconsistentNaming
			public bool is_first = false;
// ReSharper restore InconsistentNaming
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
			public bool show = false;
// ReSharper restore UnusedMember.Local
// ReSharper restore InconsistentNaming
#pragma warning disable 649
// ReSharper disable InconsistentNaming
			public string url;
// ReSharper restore InconsistentNaming
#pragma warning restore 649

		}
		
		private string UrlCategoryForCompare
		{
			get
			{
				if (!base.UrlCategoryForParsing.Contains("yugcontract.ua/shop/"))
					base.UrlCategoryForParsing = "yugcontract.ua/shop/" + base.UrlCategoryForParsing;

				if (!base.UrlCategoryForParsing.Contains("http://"))
					base.UrlCategoryForParsing = "http://" + base.UrlCategoryForParsing;

				return base.UrlCategoryForParsing.TrimEnd('/');
			}			
		}

		private string ToFulUrl(string partUrl)
		{
			return "http://yugcontract.ua/shop/" + partUrl;
		}

		protected override YandexMarketCategoryRecord GetParserCategory()
		{			
			var resultCategoriesHierarchy = new List<YandexMarketCategoryRecord>();

			string source = mDriver.PageSource;
			var indexStart = source.IndexOf("var CATEGORIES = [", System.StringComparison.Ordinal) + "var CATEGORIES = [".Length - 1;
			var indexEnd = source.IndexOf("}];", indexStart, System.StringComparison.Ordinal);

			var categoriesText = source.Substring(indexStart, indexEnd - indexStart + 2);

			var categoriesArray = JsonConvert.DeserializeObject<JsonCategory[]>(categoriesText);

			// всем елементам меню верхнего уровня
			foreach (var currentCategory in categoriesArray.Where(x => x.level == 0))
			{
				var category = ProcessCategory(currentCategory, categoriesArray);
				resultCategoriesHierarchy.Add(category);

				if (category.Url == UrlCategoryForCompare)
					mNeededCategory = category;
			}

			return mNeededCategory;
		}

		private YandexMarketCategoryRecord ProcessCategory(JsonCategory category, JsonCategory[] categoriesArray)
		{
			var newCategory = new YandexMarketCategoryRecord()
			{
				IsActive = true,
				Name = category.name,
				ParentId = int.Parse(category.pid),
				Url = ToFulUrl(category.url),
				Id = int.Parse(category.id),
				Children = new List<YandexMarketCategoryRecord>(),
			};

			if (newCategory.Url == UrlCategoryForCompare)
				mNeededCategory = newCategory;

			if (category.have_children)
			{
				var children = categoriesArray.Where(x => x.pid == category.id);
				foreach (var child in children)
				{
					var newChild = ProcessCategory(child, categoriesArray);
					newCategory.Children.Add(newChild);
				}
			}

			return newCategory;
		}
	}
}
