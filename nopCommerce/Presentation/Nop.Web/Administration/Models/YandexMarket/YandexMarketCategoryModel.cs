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
	}
}