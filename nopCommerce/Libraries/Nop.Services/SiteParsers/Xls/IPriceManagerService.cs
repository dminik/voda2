namespace Nop.Services.SiteParsers.Xls
{
	public interface IPriceManagerService
	{
		string ApplyImportAll(bool isForceDownloadingNewData);
	}
}