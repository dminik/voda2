namespace Nop.Services.SiteParsers.Xls
{
	public interface IYugCatalogPriceParserService
	{
		OstatkiParserModel ParseAndShow(bool isUpdateCacheFromInternet);
		string SetExistingInVendor(bool isUpdateCacheFromInternet);		
	}
}