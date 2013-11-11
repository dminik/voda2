namespace Nop.Admin.Models.YandexMarket
{
	using Nop.Web.Framework.Mvc;

	public class YandexMarketSpecModel : BaseNopEntityModel
	{
		public YandexMarketSpecModel()
		{
		}

		public YandexMarketSpecModel(string key, string value)
			: this()
		{
			this.Key = key;
			this.Value = value;			
		}
		
		public string Key { get; set; }

		public string Value { get; set; }

		public int ProductRecordId { get; set; }
	}
}