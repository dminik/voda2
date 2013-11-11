namespace Nop.Plugin.Misc.YandexMarketParser.Models
{
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Web.Mvc;

	using Nop.Web.Framework;
	using Nop.Web.Framework.Mvc;

	public class YandexMarketSpecModel : BaseNopEntityModel
	{
		public YandexMarketSpecModel()
		{
		}

		public YandexMarketSpecModel(string key, string value)
			: this()
		{
			Key = key;
			Value = value;			
		}
		
		public string Key { get; set; }

		public string Value { get; set; }

		public int ProductRecordId { get; set; }
	}
}