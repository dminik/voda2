namespace Nop.Core.Domain.YandexMarket
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;

	public class FormatterBase
	{
		public static FormatterBase Create(string url)
		{
			if(url.Contains("yugcontract"))
				return new FormatterUgContract();

			return new FormatterBase();
		}

		public virtual YandexMarketProductRecord Format(YandexMarketProductRecord product)
		{
			return product;
		}

		protected string GetValByRegExp(string inputString, string pattern)
		{
			Match m = Regex.Match(inputString, pattern,
								RegexOptions.IgnoreCase | RegexOptions.Compiled,
								TimeSpan.FromSeconds(1));

			if (m.Success)			
				return m.Groups[1].ToString();							
			else			
				return "";			
		}

		protected List<string> GetValByRegExp(string inputString, string pattern, int groupCount)
		{
			Match m = Regex.Match(inputString, pattern,
								RegexOptions.IgnoreCase | RegexOptions.Compiled,
								TimeSpan.FromSeconds(1));

			if (m.Success)
			{
				var resultList = new List<string>();

				for (int i = 0; i < groupCount; i++)
				{
					resultList.Add(m.Groups[i+1].ToString());
				}

				return resultList;
			}
			else 
				return null;
		}

		protected bool IsProductContainString(YandexMarketProductRecord product, string str)
		{
			str = str.ToLower();

			if (product.Name.ToLower().Contains(str)) 
				return true;
			else if (product.FullDescription.ToLower().Contains(str))
				return true;
			
			return false;			
		}
	}
}
