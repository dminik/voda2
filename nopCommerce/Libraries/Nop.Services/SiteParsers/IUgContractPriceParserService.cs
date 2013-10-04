namespace Nop.Services.SiteParsers
{	
	public interface IUgContractPriceParserService
	{
		OstatkiFileParserModel ParseAndShow(bool isUpdateCacheFromInternet);
		string ApplyImport(bool isUpdateCacheFromInternet);		
	}
}