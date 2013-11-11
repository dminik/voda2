namespace Nop.Plugin.Misc.YandexMarketParser.Models
{
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Web.Mvc;

	using Nop.Plugin.Misc.YandexMarketParser.Domain;
	using Nop.Web.Framework;
	using Nop.Web.Framework.Mvc;

	public class YandexMarketParserModel : BaseNopModel
	{
		public YandexMarketParserModel()
		{
			AvailableCategories = new List<SelectListItem>();
			ProductList = new List<YandexMarketProductRecord>();
			ParseNotMoreThen = 2;
			IsTest = true;
			CategoryId = 1;
		}

		public string HelloWord = "bebebe";
		
		public int CategoryId { get; set; }
		public IList<SelectListItem> AvailableCategories { get; set; }


		public List<YandexMarketProductRecord> ProductList { get; set; }

		public bool IsTest { get; set; }

		public int ParseNotMoreThen { get; set; }



		[DisplayName("Имя категории")]
		[StringLength(40)]		
		public string AddCategoryName { get; set; }	



	}
}