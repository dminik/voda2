namespace Nop.Core.Domain.YandexMarket
{
	using Nop.Core;
	using Nop.Core.Domain.Localization;
	using Nop.Core.Domain.Security;
	using Nop.Core.Domain.Seo;
	using Nop.Core.Domain.Stores;

	/// <summary>
	/// Represents a product record
	/// </summary>	
	public partial class YandexMarketSpecRecord : BaseEntity
	{
		public YandexMarketSpecRecord()
		{
		}

		public YandexMarketSpecRecord(string key, string value)
			: this()
		{
			this.Key = key;
			this.Value = value;			
		}
		
		public string Key { get; set; }
		
		public string Value { get; set; }
		
		public int ProductRecordId { get; set; }
		
		public virtual YandexMarketProductRecord ProductRecord { get; set; }
	}
}