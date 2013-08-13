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
			this.AvailableShopCategories = new List<SelectListItem>();	
			this.AvailableParserCategories = new List<SelectListItem>();			
			this.ParseNotMoreThen = 2;
			this.IsTest = true;
			this.ParserCategoryId = 1;
			this.ShopCategoryId = 0;
		}

		[DisplayName("Имя категории магазина в которую импортнутся спарсенные товары")]
		public int ShopCategoryId { get; set; }

		[DisplayName("Доступные категории магазина")]
		public List<SelectListItem> AvailableShopCategories { get; set; }

		[DisplayName("Текущая категория парсера")]
		public int ParserCategoryId { get; set; }

		[DisplayName("Доступные категории парсера")]
		public List<SelectListItem> AvailableParserCategories { get; set; }

		[DisplayName("Подставить фиктивные спарсенные данные как результат")]
		public bool IsTest { get; set; }

		[DisplayName("Спарсить не более чем n товаров, затем остановится")]
		public int ParseNotMoreThen { get; set; }

		[DisplayName("URL списка товаров для распасивания")]
		[StringLength(200)]
		[MinLength(4)]
		[Required]
		public string ProductsPageUrl { get; set; }	

		[DisplayName("Имя категории парсера")]
		[StringLength(40)]		
		public string AddParserCategoryName { get; set; }	



	}
}