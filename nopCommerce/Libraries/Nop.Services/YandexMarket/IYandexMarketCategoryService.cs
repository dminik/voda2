namespace Nop.Services.YandexMarket
{
	using System.Collections.Generic;
	using System.Web.Mvc;

	using Nop.Core;
	using Nop.Core.Domain.YandexMarket;

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
		/// Gets all tax rates
		/// </summary>
		/// <returns>Tax rates</returns>
		IPagedList<YandexMarketCategoryRecord> GetActive(int pageIndex = 0, int pageSize = int.MaxValue);
		
	
		YandexMarketCategoryRecord GetById(int id);

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

		List<SelectListItem> GetCategoriesForDDL();

		void SetActiveAllParserCategoties(bool isActive);
	}
}
