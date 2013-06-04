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

		public override string ToString()
		{
			var result = new StringBuilder();

			result.Append("----------------------");
			result.Append("Title: " + Title + Environment.NewLine);

			foreach (var currentSecification in Specifications)
			{
				result.Append(currentSecification.Key + " = " + currentSecification.Value + Environment.NewLine);
			}

			return result.ToString();
		}
	}
}

