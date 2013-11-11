namespace Nop.Services.SiteParsers
{
	public interface IF5PriceParserService
	{
		VendorParserModel ParseAndShow(bool isUpdateCacheFromInternet);

		string SetVendorPrices(bool isUpdateCacheFromInternet);
	}
}