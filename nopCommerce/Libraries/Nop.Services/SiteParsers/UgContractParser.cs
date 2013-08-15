namespace Nop.Services.SiteParsers
{
	using System.Collections.Generic;
	using System.Linq;

	using Nop.Core.Domain.YandexMarket;

	public class UgContractParser : BaseParser
	{
		// List
		protected override string CssSelectorForProductLinkInProductList { get { return "div.cat-item a"; } }
		protected override string CssSelectorForNextLinkInProductList { get { return "[title=следующая]"; } }
		
		// Product
		protected override string CssSelectorForProductArticulInProductPage { get { return ".good-info-number"; } } 
		protected override string CssSelectorForProductNameInProductPage { get { return "h1.good-info-header"; } }
		protected override string CssSelectorForProductPictureInProductPage { get { return ".lightbox img"; } }
		protected override string CssSelectorForProductFullDescriptionInProductPage { get { return "div.panes-item.goods-item-panes-item.content"; } }
		
		// Product specs
		protected override string GetLinkToSpecsTabOfProduct(string productLink) { return productLink + "#chars"; }
		protected override string CssSelectorForProductSpecsRowsInProductPage { get { return ".properties-table tr"; } }
		protected override string CssSelectorForProductSpecKeyInProductPage { get { return "td.prop"; } }
		protected override string CssSelectorForProductSpecValInProductPage { get { return "td.val"; } }

		protected override YandexMarketProductRecord ProductPostProcessing(YandexMarketProductRecord product)
		{
			const string toErase1 = "Если вы заметили некорректные данные в описании товара, выделите ошибку и нажмите";
			const string toErase2 = "Ctrl+Enter";
			const string toErase3 = ", чтобы сообщить нам об этом.";
			product.FullDescription = product.FullDescription.Replace(toErase1, "").Replace(toErase2, "").Replace(toErase3, "");

			product.Articul = product.Articul.Replace("Код: ", "");

			product.Name = product.Name.Replace("МОБИЛЬНЫЙ ТЕЛЕФОН ", "");

			var manufactureName = GetManufactureFromName(product.Name);
			if (manufactureName != string.Empty)
				product.Specifications.Add(new YandexMarketSpecRecord("Производитель", manufactureName));


			return product;
		}

		private string GetManufactureFromName(string name)
		{			  
			var manufactures = new List<string>() { "Sony", "Fly", "Alcatel", "LG", "Philips", "Nokia", "Samsung", };
			name = name.ToUpper();

			foreach (var manufacture in manufactures.Where(manufacture => name.Contains(manufacture.ToUpper()))) 
				return manufacture;

			return "";
		}




	}
}
