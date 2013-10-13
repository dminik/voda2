namespace Nop.Services.SiteParsers
{
	using Nop.Services.SiteParsers.Xls;

	public interface IOstatkiPriceParserService
	{
		OstatkiParserModel ParseAndShow(bool isUpdateCacheFromInternet);
		string SetExistingInBoyarka(bool isUpdateCacheFromInternet);		
	}
}