namespace Nop.Admin.Models.YandexMarket
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Text;
	using System.Web.Mvc;

	using Nop.Web.Framework.Mvc;

	public static class ListExtention
	{
		// Call like     var html = ToHtmlList(people, x => x.LastName, x => x.FirstName);
		public static string ToHtmlList<T>(this List<T> list, params Func<T, object>[] fxns)
		{
			var sb = new StringBuilder();
			sb.Append("<TABLE>\n");
			foreach (var item in list)
			{
				sb.Append("<TR>\n");
				foreach (var fxn in fxns)
				{
					sb.Append("<TD>");
					sb.Append(fxn(item));
					sb.Append("</TD>");
				}
				sb.Append("</TR>\n");
			}
			sb.Append("</TABLE>");

			return sb.ToString();
		}
	}

	public class YandexMarketProductModel : BaseNopEntityModel
	{
		

		public YandexMarketProductModel()
		{
			this.Specifications = new List<YandexMarketSpecModel>();
		}

		public YandexMarketProductModel(string name, string imageUrl_1, int yandexMarketCategoryRecordId, List<YandexMarketSpecModel> specifications, int id = 0)
			: this()
		{
			this.Id = id;
			this.Name = name;
			this.ImageUrl_1 = imageUrl_1;
			this.Specifications = specifications;
			this.YandexMarketCategoryRecordId = yandexMarketCategoryRecordId;
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

		private List<YandexMarketSpecModel> specifications;
		[DisplayName("Спецификации")]		
		public List<YandexMarketSpecModel> Specifications
		{
			get
			{
				return specifications;
			}
			set
			{
				specifications = value;
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

			result.Append("ImageUrl_1 = " + this.ImageUrl_1 + ";" + Environment.NewLine);
			
			return result.ToString();
		}
	}
}