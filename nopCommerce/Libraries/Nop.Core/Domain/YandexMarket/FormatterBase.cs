namespace Nop.Core.Domain.YandexMarket
{
	using System;
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
	}
}
