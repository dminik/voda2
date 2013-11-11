namespace Nop.Data.Mapping.YandexMarket
{
	using System.Data.Entity.ModelConfiguration;

	using Nop.Core.Domain.YandexMarket;

	public partial class YandexMarketCategoryRecordMap : EntityTypeConfiguration<YandexMarketCategoryRecord>
	{
		public YandexMarketCategoryRecordMap()
		{
			this.ToTable("YandexMarketCategory");
			this.HasKey(x => x.Id);
		}
	}
}