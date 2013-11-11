namespace Nop.Plugin.Misc.YandexMarketParser.Models
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Text;
	using System.Web.Mvc;

	using Nop.Web.Framework;
	using Nop.Web.Framework.Mvc;

	public class YandexMarketProductModel : BaseNopEntityModel
	{
		public YandexMarketProductModel()
		{
			this.Specifications = new List<YandexMarketSpecModel>();
		}

		public YandexMarketProductModel(string name, string imageUrl_1, int yandexMarketCategoryRecordId, List<YandexMarketSpecModel> specifications, int id = 0)
			: this()
		{
			Id = id;
			Name = name;
			ImageUrl_1 = imageUrl_1;
			Specifications = specifications;
			YandexMarketCategoryRecordId = yandexMarketCategoryRecordId;
		}

		[DisplayName("Название")]
		[StringLength(100)]
		[Required(ErrorMessage = "Поле обязательно к заполнению")]		
		public string Name { get; set; }
		
		[DisplayName("Рисунок1")]
		[StringLength(1000)]		
		public string ImageUrl_1 { get; set; }

		[DisplayName("Категория")]
		public int YandexMarketCategoryRecordId { get; set; }

		[DisplayName("Спецификации")]		
		public List<YandexMarketSpecModel> Specifications { get; set; }
			
		public override string ToString()
		{
			var result = new StringBuilder();

			result.Append("----------------------");
			result.Append("Title: " + this.Name + ";" + Environment.NewLine);

			foreach (var currentSecification in this.Specifications)
			{
				result.Append(currentSecification.Key + " = " + currentSecification.Value + ";" + Environment.NewLine);
			}

			result.Append("ImageUrl_1 = " + this.ImageUrl_1 + ";" + Environment.NewLine);
			
			return result.ToString();
		}
	}
}