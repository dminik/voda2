namespace Nop.Services.SiteParsers
{	
	public interface IOstatkiPriceParserService
	{
		OstatkiParserModel ParseAndShow(bool isUpdateCacheFromInternet);
		string SetExistingInBoyarka(bool isUpdateCacheFromInternet);		
	}
}