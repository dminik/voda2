namespace Nop.Services.YandexMarket
{
	using System.Collections.Generic;

	using Nop.Services.SiteParsers.Page;

	public interface ISpecialPriceService
	{
		IEnumerable<SpecialPrice> GetAll(bool isUpdateCacheFromInternet);
	}	
}
