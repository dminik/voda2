namespace Nop.Services.YandexMarket
{
	using Nop.Core;
	using Nop.Core.Domain.YandexMarket;

	public interface IYandexMarketSpecService
	{
		void Delete(YandexMarketSpecRecord spec);

		IPagedList<YandexMarketSpecRecord> GetByCategory(int categoryId, int pageIndex = 0, int pageSize = int.MaxValue);

		YandexMarketSpecRecord GetById(int specId);

		void Insert(YandexMarketSpecRecord spec);

		void Update(YandexMarketSpecRecord spec);
	}
}