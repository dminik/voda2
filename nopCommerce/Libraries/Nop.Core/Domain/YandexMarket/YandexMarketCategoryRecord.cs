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
	public partial class YandexMarketCategoryRecord : BaseEntity
	{       
		public string Name { get; set; }		
	}
}