namespace Nop.Plugin.Misc.YandexMarketParser.Domain
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Text;

	using Nop.Core;

	public partial class ProductRecord : BaseEntity
	{		
		public ProductRecord()
		{
			this.Specifications = new Dictionary<string, string>();
		}

		[DisplayName("Название")]
		[StringLength(100)]
		[Required(ErrorMessage = "Поле обязательно к заполнению")]
		public string Title { get; set; }

		public Dictionary<string, string> Specifications { get; set; }

		[DisplayName("Рисунок1")]
		[StringLength(1000)]
		// [Required(ErrorMessage = "Поле обязательно к заполнению")]
		public string ImageUrl_1 { get; set; }


		public override string ToString()
		{
			var result = new StringBuilder();

			result.Append("----------------------");
			result.Append("Title: " + this.Title + ";" + Environment.NewLine);

			foreach (var currentSecification in this.Specifications)
			{
				result.Append(currentSecification.Key + " = " + currentSecification.Value + ";" + Environment.NewLine);
			}

			result.Append("ImageUrl_1 = " + this.ImageUrl_1 + ";" + Environment.NewLine);
			
			return result.ToString();
		}
	}
}

