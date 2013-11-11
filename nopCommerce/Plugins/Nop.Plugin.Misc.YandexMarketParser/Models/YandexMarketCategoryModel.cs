namespace Nop.Plugin.Misc.YandexMarketParser.Models
{
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Web.Mvc;

	using Nop.Web.Framework;
	using Nop.Web.Framework.Mvc;

	public class YandexMarketCategoryModel : BaseNopEntityModel
	{
		[DisplayName("Категория")]
		[StringLength(1024)]
		[Required(ErrorMessage = "Поле обязательно к заполнению")]
		public string Name { get; set; }
	}
}