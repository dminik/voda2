namespace Nop.Core.Domain.YandexMarket
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Linq;
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
			this.Articul = "";
			this.Name = "";
			this.FullDescription = "";
			this.ImageUrl_1 = "";						
			this.Url = "";
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

		private const string mNotInPriceList = "NotInPriceList";
		
		public string ImageUrl_1 { get; set; }
		public string Url { get; set; }
		
		public int YandexMarketCategoryRecordId { get; set; }

		public virtual IList<YandexMarketSpecRecord> Specifications { get; set; }

		[NotMapped]
		public bool IsNotInPriceList
		{
			get
			{
				return Name == mNotInPriceList;
			}
			set
			{
				Name = value ? mNotInPriceList : "";
			}
		}
	
		[NotMapped]
		public bool IsFormatted { get; set; }

		private FormatterBase mFormatter;
		[NotMapped]
		private FormatterBase Formatter 
		{ 
			get
			{
				return this.mFormatter ?? (this.mFormatter = FormatterBase.Create(this.Url));
			}
		}

		public YandexMarketProductRecord FormatMe()
		{
			if (IsFormatted)
				return this;
			
			this.Formatter.Format(this);
			this.IsFormatted = true;

			return this;
		}


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

		public YandexMarketProductRecord Clone()
		{
			var specifications = this.Specifications.Select(yandexMarketSpecRecord => yandexMarketSpecRecord.Clone()).ToList();

			return new YandexMarketProductRecord()
				{
					Id = Id,
					Articul = Articul,
					FullDescription = FullDescription,
					ImageUrl_1 = ImageUrl_1,
					IsFormatted = IsFormatted,					
					Name = Name,
					Url = Url,
					Specifications = specifications,
					YandexMarketCategoryRecordId = YandexMarketCategoryRecordId,
					// IsNotInPriceList = IsNotInPriceList,					
				};
		}
	}
}

