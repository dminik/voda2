namespace Nop.Plugin.Misc.YandexMarketParser.Services
{
	using System.Collections.Generic;

	using Nop.Core;
	using Nop.Plugin.Misc.YandexMarketParser.Domain;

	/// <summary>
	/// Tax rate service interface
	/// </summary>
	public partial interface IYandexMarketCategoryService
	{
		/// <summary>
		/// Deletes a tax rate
		/// </summary>
		/// <param name="taxRate">Tax rate</param>
		void Delete(YandexMarketCategoryRecord taxRate);

		/// <summary>
		/// Gets all tax rates
		/// </summary>
		/// <returns>Tax rates</returns>
		IPagedList<YandexMarketCategoryRecord> GetAll(int pageIndex = 0, int pageSize = int.MaxValue);

		
		/// <summary>
		/// Gets a tax rate
		/// </summary>
		/// <param name="taxRateId">Tax rate identifier</param>
		/// <returns>Tax rate</returns>
		YandexMarketCategoryRecord GetById(int taxRateId);

		/// <summary>
		/// Inserts a tax rate
		/// </summary>
		/// <param name="taxRate">Tax rate</param>
		void Insert(YandexMarketCategoryRecord taxRate);

		/// <summary>
		/// Updates the tax rate
		/// </summary>
		/// <param name="taxRate">Tax rate</param>
		void Update(YandexMarketCategoryRecord taxRate);
	}
}
