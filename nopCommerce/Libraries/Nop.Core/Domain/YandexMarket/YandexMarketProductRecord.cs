﻿namespace Nop.Core.Domain.YandexMarket
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

		public YandexMarketProductRecord(string name, string imageUrl_1, int yandexMarketCategoryRecordId, List<YandexMarketSpecRecord> specifications)
			: this()
		{
			this.Name = name;
			this.ImageUrl_1 = imageUrl_1;
			this.Specifications = specifications;
			this.YandexMarketCategoryRecordId = yandexMarketCategoryRecordId;
		}
		
		public string Name { get; set; }
				
		public string ImageUrl_1 { get; set; }
		
		public int YandexMarketCategoryRecordId { get; set; }

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
