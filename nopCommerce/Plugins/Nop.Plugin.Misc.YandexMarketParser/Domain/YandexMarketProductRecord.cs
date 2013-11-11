namespace Nop.Plugin.Misc.YandexMarketParser.Domain
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Runtime.Serialization;
	using System.Text;

	using Nop.Core;

	[DataContract]
	public partial class YandexMarketProductRecord : BaseEntity
	{		
		public YandexMarketProductRecord()
		{
			this.Specifications = new List<YandexMarketSpecRecord>();
		}

		public YandexMarketProductRecord(string name, string imageUrl_1, int yandexMarketCategoryRecordId, List<YandexMarketSpecRecord> specifications)
			: this()
		{
			Name = name;
			ImageUrl_1 = imageUrl_1;
			Specifications = specifications;
			YandexMarketCategoryRecordId = yandexMarketCategoryRecordId;
		}

		[DisplayName("Название")]
		[StringLength(100)]
		[Required(ErrorMessage = "Поле обязательно к заполнению")]
		[DataMember]
		public string Name { get; set; }
		
		[DisplayName("Рисунок1")]
		[StringLength(1000)]
		[DataMember]
		public string ImageUrl_1 { get; set; }

		[DataMember]
		public int YandexMarketCategoryRecordId { get; set; }

		[DisplayName("Спецификации")]		
		public virtual IList<YandexMarketSpecRecord> Specifications { get; set; }
	
		public virtual YandexMarketCategoryRecord YandexMarketCategoryRecord { get; set; }

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

