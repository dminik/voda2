using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProductParser
{
	public class Product
	{		
		public Product()
		{
			Specifications = new Dictionary<string, string>();
		}

		public string Title { get; set; }

		public Dictionary<string, string> Specifications { get; set; }

		public string ImageUrl_1 { get; set; }


		public override string ToString()
		{
			var result = new StringBuilder();

			result.Append("----------------------");
			result.Append("Title: " + Title + Environment.NewLine);

			foreach (var currentSecification in Specifications)
			{
				result.Append(currentSecification.Key + " = " + currentSecification.Value + Environment.NewLine);
			}

			result.Append("ImageUrl_1 = " + ImageUrl_1 + Environment.NewLine);
			
			return result.ToString();
		}
	}
}

