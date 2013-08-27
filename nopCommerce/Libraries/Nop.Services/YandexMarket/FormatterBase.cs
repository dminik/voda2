namespace Nop.Services.YandexMarket
{
	using System;
	using System.Text.RegularExpressions;

	using Nop.Core.Domain.YandexMarket;

	public abstract class FormatterBase
	{
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
