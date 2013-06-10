namespace Nop.Plugin.Misc.YandexMarketParser.Models
{
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Web.Mvc;

	using Nop.Web.Framework;
	using Nop.Web.Framework.Mvc;

	public class YandexMarketParserModel : BaseNopModel
	{
		public YandexMarketParserModel()
		{
			AvailableCategories = new List<SelectListItem>();
			ProductList = new List<Product>();
			ParseNotMoreThen = 2;
		}

		public string HelloWord = "bebebe";
		
		public int CategoryId { get; set; }
		public IList<SelectListItem> AvailableCategories { get; set; }


		public List<Product> ProductList { get; set; }

		public bool IsTest { get; set; }

		public int ParseNotMoreThen { get; set; }



		[DisplayName("Имя категории")]
		[StringLength(40)]
		[Required(ErrorMessage = "Поле обязательно к заполнению")]
		public string AddCategoryName { get; set; }	



	}
}