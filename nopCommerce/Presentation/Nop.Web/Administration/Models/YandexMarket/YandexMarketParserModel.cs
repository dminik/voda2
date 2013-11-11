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
			this.IsTest = false;			
			this.ShopCategoryId = 0;
			ParserCategoryId = -1;
			IsClearCategoryProductsBeforeParsing = false;
			IsNotImportedOnly = false;
			IsSetActiveAllParserCategoties = false;
			IsWithProductCountInCategories = false;
		}

		[DisplayName("Имя категории магазина в которую импортнутся спарсенные товары")]
		public int ShopCategoryId { get; set; }

		[DisplayName("Доступные категории магазина")]
		public List<SelectListItem> AvailableShopCategories { get; set; }		

		[DisplayName("Доступные категории парсера")]
		public List<SelectListItem> AvailableParserCategories { get; set; }

		[DisplayName("Подставить фиктивные спарсенные данные как результат")]
		public bool IsTest { get; set; }

		[DisplayName("Перед парсингом удалять все товары из категории")]
		public bool IsClearCategoryProductsBeforeParsing { get; set; }

		[DisplayName("Спарсить не более чем n товаров, затем остановится")]
		public int ParseNotMoreThen { get; set; }
		
		[DisplayName("Имя категории парсера")]
		[StringLength(40)]		
		public string AddParserCategoryName { get; set; }

		[DisplayName("Url категории парсера")]
		[StringLength(200)]
		public string AddParserCategoryUrl { get; set; }

		[DisplayName("Имя категории маганина")]
		public int AddShopCategoryId { get; set; }

		[DisplayName("Активирована")]
		public bool AddIsActive { get; set; }

		[DisplayName("Показать товары для этой категории")]
		public int ParserCategoryId{ get; set; }		

		[DisplayName("Показать только новые найденные товары")]
		public bool IsNotImportedOnly { get; set; }

		[DisplayName("Активировать/Деактивировать все категории парсера")]
		public bool IsSetActiveAllParserCategoties { get; set; }

		[DisplayName("Посчитать количество уже импортированных продуктов")]
		public bool IsWithProductCountInCategories { get; set; }
	}
}