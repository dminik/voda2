namespace Nop.Admin.Models.YandexMarket
{
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Web.Mvc;

	using Nop.Web.Framework.Mvc;

	public class YandexMarketParserModel : BaseNopModel
	{
		public YandexMarketParserModel()
		{
			this.AvailableCategories = new List<SelectListItem>();			
			this.ParseNotMoreThen = 2;
			this.IsTest = true;
			this.CategoryId = 1;
		}
		
		public int CategoryId { get; set; }
		public IList<SelectListItem> AvailableCategories { get; set; }		

		public bool IsTest { get; set; }

		public int ParseNotMoreThen { get; set; }



		[DisplayName("Имя категории")]
		[StringLength(40)]		
		public string AddCategoryName { get; set; }	



	}
}