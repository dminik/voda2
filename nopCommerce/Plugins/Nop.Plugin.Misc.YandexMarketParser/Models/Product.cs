namespace Nop.Plugin.Misc.YandexMarketParser.Models
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	public class Product
	{		
		public Product()
		{
			this.Specifications = new Dictionary<string, string>();
		}

		public string Title { get; set; }

		public Dictionary<string, string> Specifications { get; set; }

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

