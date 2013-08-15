namespace Nop.Services.SiteParsers
{
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
		protected override string CssSelectorForProductSpecValInProductPage { get { return "td.val p"; } }
		
		
	}
}
