namespace Nop.Core.Domain.YandexMarket
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Text;

	using Nop.Core;
	using Nop.Core.Domain.Localization;
	using Nop.Core.Domain.Security;
	using Nop.Core.Domain.Seo;
	using Nop.Core.Domain.Stores;

	public partial class YandexMarketProductRecord : BaseEntity
	{		
		public YandexMarketProductRecord()
		{
			this.Specifications = new List<YandexMarketSpecRecord>();
		}

		public YandexMarketProductRecord(string articul, string name, string fullDescription, string imageUrl_1, string url, int yandexMarketCategoryRecordId, List<YandexMarketSpecRecord> specifications)
			: this()
		{
			this.Articul = articul;
			this.Name = name;
			this.FullDescription = fullDescription;
			this.ImageUrl_1 = imageUrl_1;
			this.Specifications = specifications;
			this.YandexMarketCategoryRecordId = yandexMarketCategoryRecordId;
			this.Url = url;
		}
		
		public string Name { get; set; }
		public string Articul { get; set; }
		public string FullDescription { get; set; }
				
		public string ImageUrl_1 { get; set; }
		public string Url { get; set; }
		
		public int YandexMarketCategoryRecordId { get; set; }

		public virtual IList<YandexMarketSpecRecord> Specifications { get; set; }
	
		public virtual YandexMarketCategoryRecord YandexMarketCategoryRecord { get; set; }

		public override string ToString()
		{
			var result = new StringBuilder();

			result.Append("----------------------");
			result.Append("Articul: " + this.Articul + ";" + Environment.NewLine);
			result.Append("Title: " + this.Name + ";" + Environment.NewLine);

			foreach (var currentSecification in this.Specifications)
			{
				result.Append(currentSecification.Key + " = " + currentSecification.Value + ";" + Environment.NewLine);
			}

			result.Append("ImageUrl_1 = " + this.ImageUrl_1 + ";" 
				+ "Url = " + this.Url + ";"
				+ "FullDescription = " + FullDescription + Environment.NewLine);
			
			return result.ToString();
		}
	}
}

