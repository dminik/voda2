namespace Nop.Admin.Models.YandexMarket
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Text;
	
	using Nop.Web.Framework.Mvc;
	
	public class YandexMarketProductModel : BaseNopEntityModel
	{		
		public YandexMarketProductModel()
		{
			this.Specifications = new List<YandexMarketSpecModel>();
		}

		public YandexMarketProductModel(string articul, string name, string fullDescription, string imageUrl_1, int yandexMarketCategoryRecordId, List<YandexMarketSpecModel> specifications, int id = 0)
			: this()
		{
			this.Id = id;
			this.Articul =  articul;
			this.Name = name;
			this.FullDescription = fullDescription;
			this.ImageUrl_1 = imageUrl_1;
			this.Specifications = specifications;
			this.YandexMarketCategoryRecordId = yandexMarketCategoryRecordId;
		}

		[DisplayName("Артикул")]
		public string Articul { get; set; }

		[DisplayName("Название")]
		[StringLength(100)]
		[Required(ErrorMessage = "Поле обязательно к заполнению")]		
		public string Name { get; set; }

		[DisplayName("Описание")]
		public string FullDescription { get; set; }
		
		[DisplayName("Рисунки")]			
		public string ImageUrl_1 { get; set; }

		
		public string ImagesHtml
		{
			get
			{
				if (ImageUrl_1 == null) return "";

				var sb = new StringBuilder();
				sb.Append("<ul>\n");
				foreach (var curImagePath in ImageUrl_1.Split(';'))
				{					
					sb.Append("<li> <img src='http://localhost/ProductsCatalog/" + curImagePath + "' /> </li>");																					
				}

				sb.Append("</ul>");

				return sb.ToString();
			}
		}


		[DisplayName("Категория")]
		public int YandexMarketCategoryRecordId { get; set; }
		
			
		public List<YandexMarketSpecModel> Specifications{get; set; }

		[DisplayName("Спецификации")]	
		public string SpecificationsHtml 
		{
			get
			{
				return Specifications.ToHtmlList(spec => spec.Key, spec => spec.Value); 
			} 			
		}
		
		public override string ToString()
		{
			var result = new StringBuilder();

			result.Append("----------------------");
			result.Append("Title: " + this.Name + ";" + Environment.NewLine);

			foreach (var currentSecification in this.Specifications)
			{
				result.Append(currentSecification.Key + " = " + currentSecification.Value + ";" + Environment.NewLine);
			}

			result.Append("ImageUrl_1 = " + this.ImageUrl_1 + ";" + Environment.NewLine + " FullDescription = " + FullDescription + Environment.NewLine);
			
			return result.ToString();
		}
	}
}