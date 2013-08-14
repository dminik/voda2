namespace Nop.Services.SiteParsers
{
	public class UgContractParser : BaseParser
	{
		protected override string CssSelectorForProductLinkInProductList { get { return "a.b-offers__name"; } }
		protected override string CssSelectorForNextLinkInProductList { get { return "a.b-pager__next"; } }
		protected override string CssSelectorForProductNameInProductPage { get { return "h1.b-page-title"; } }
		protected override string CssSelectorForProductPictureInProductPage { get { return "div.b-model-microcard__img img"; } }
		protected override string CssSelectorForProductSpecsRowsInProductPage { get { return "table.b-properties tr:has(th.b-properties__label)"; } }
		protected override string CssSelectorForProductSpecKeyInProductPage { get { return ".b-properties__label span"; } }
		protected override string CssSelectorForProductSpecValInProductPage { get { return ".b-properties__value"; } }
		protected override string GetLinkToSpecsTabOfProduct(string productLink) { return productLink.Replace("model.xml", "model-spec.xml"); }
	}
}
