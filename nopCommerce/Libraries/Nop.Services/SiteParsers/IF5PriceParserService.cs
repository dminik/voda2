namespace Nop.Services.SiteParsers
{
	public interface IF5PriceParserService
	{
		VendorFileParserModel ParseAndShow(bool isUpdateCacheFromInternet);

		string ApplyImport(bool isUpdateCacheFromInternet);
	}
}