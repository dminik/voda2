namespace Nop.Admin.Models.YandexMarket
{
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;

	using Nop.Web.Framework.Mvc;

	public class YandexMarketCategoryModel : BaseNopEntityModel
	{
		[DisplayName("Категория")]
		[StringLength(1024)]
		[Required(ErrorMessage = "Поле обязательно к заполнению")]
		public string Name { get; set; }

		[DisplayName("Url Категории")]
		[StringLength(1024)]
		[Required(ErrorMessage = "Поле обязательно к заполнению")]
		public string Url { get; set; }

		[DisplayName("Имя Категории магазина")]
		[StringLength(1024)]
		[Required(ErrorMessage = "Поле обязательно к заполнению")]
		public string ShopCategoryName { get; set; }
		public int ShopCategoryId { get; set; }

		[DisplayName("Активно")]				
		public bool IsActive { get; set; }

		[DisplayName("Импортировано уже в магазин")]
		public int AlreadyImportedProducts { get; set; }
	}
}