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
		
		[DisplayName("Рисунок1")]
		[StringLength(1000)]		
		public string ImageUrl_1 { get; set; }

		[DisplayName("Категория")]
		public int YandexMarketCategoryRecordId { get; set; }

		private List<YandexMarketSpecModel> mSpecifications;
		[DisplayName("Спецификации")]		
		public List<YandexMarketSpecModel> Specifications
		{
			get
			{
				return this.mSpecifications;
			}
			set
			{
				this.mSpecifications = value;
				SpecificationsHtml = Specifications.ToHtmlList(spec => spec.Key, spec => spec.Value);
			}
		}

		public string SpecificationsHtml { get; set; }
			

		//.ToHtmlList(spec => spec.Key, spec => spec.Value)

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