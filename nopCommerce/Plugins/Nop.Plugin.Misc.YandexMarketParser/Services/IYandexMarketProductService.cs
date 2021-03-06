namespace Nop.Plugin.Misc.YandexMarketParser.Services
{
	using System.Collections.Generic;

	using Nop.Core;
	using Nop.Plugin.Misc.YandexMarketParser.Domain;

	/// <summary>
	/// Tax rate service interface
	/// </summary>
	public partial interface IYandexMarketProductService
	{
		/// <summary>
		/// Deletes a tax rate
		/// </summary>
		/// <param name="record">Tax rate</param>
		void Delete(YandexMarketProductRecord record);

		/// <summary>
		/// Gets all tax rates
		/// </summary>
		/// <returns>Tax rates</returns>
		IPagedList<YandexMarketProductRecord> GetByCategory(int categoryId, int pageIndex = 0, int pageSize = int.MaxValue);

		
		/// <summary>
		/// Gets a tax rate
		/// </summary>
		/// <param name="recordId">Tax rate identifier</param>
		/// <returns>Tax rate</returns>
		YandexMarketProductRecord GetById(int recordId);

		/// <summary>
		/// Inserts a tax rate
		/// </summary>
		/// <param name="record">Tax rate</param>
		void Insert(YandexMarketProductRecord record);

		/// <summary>
		/// Updates the tax rate
		/// </summary>
		/// <param name="record">Tax rate</param>
		void Update(YandexMarketProductRecord record);

		void InsertList(IEnumerable<YandexMarketProductRecord> recordList);
	}
}
