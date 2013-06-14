namespace Nop.Admin.Models.YandexMarket
{
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Web.Mvc;

	using Nop.Web.Framework.Mvc;

	public class YandexMarketSpecImportModel : BaseNopModel
	{
		public YandexMarketSpecImportModel()
		{
			this.AvailableCategories = new List<SelectListItem>();						
			this.CategoryId = 1;
		}
		
		public int CategoryId { get; set; }
		
		public List<SelectListItem> AvailableCategories { get; set; }		
	}
}