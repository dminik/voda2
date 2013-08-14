namespace Nop.Services.SiteParsers
{
	public class UgContractParser : BaseParser
	{
		// List
		protected override string CssSelectorForProductLinkInProductList { get { return "div.cat-item a"; } }
		protected override string CssSelectorForNextLinkInProductList { get { return "[title=следующая]"; } }
		
		// Product
		protected override string CssSelectorForProductNameInProductPage { get { return "h1.good-info-header"; } }
		protected override string CssSelectorForProductPictureInProductPage { get { return ".lightbox img"; } }

		// Product specs
		protected override string GetLinkToSpecsTabOfProduct(string productLink) { return productLink + "#chars"; }
		protected override string CssSelectorForProductSpecsRowsInProductPage { get { return ".properties-table tr"; } }
		protected override string CssSelectorForProductSpecKeyInProductPage { get { return "td.prop"; } }
		protected override string CssSelectorForProductSpecValInProductPage { get { return "td.val p"; } }
		
	}
}
