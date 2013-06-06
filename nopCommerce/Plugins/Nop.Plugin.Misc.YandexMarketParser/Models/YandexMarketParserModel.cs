namespace Nop.Plugin.Misc.YandexMarketParser.Models
{
	using System.Collections.Generic;

	using Nop.Web.Framework;
	using Nop.Web.Framework.Mvc;

	public class YandexMarketParserModel : BaseNopModel
	{
		public YandexMarketParserModel()
		{
			ProductList = new List<Product>();
		}

		public string HelloWord = "bebebe";

		public List<Product> ProductList { get; set; }

		public string CatalogName { get; set; }

		public bool IsTest { get; set; }
	}
}