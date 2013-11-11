namespace Nop.Data.Mapping.YandexMarket
{
	using System.Data.Entity.ModelConfiguration;

	using Nop.Core.Domain.YandexMarket;

	public partial class YandexMarketProductRecordMap : EntityTypeConfiguration<YandexMarketProductRecord>
	{
		public YandexMarketProductRecordMap()
		{
			this.ToTable("YandexMarketProduct");
			this.HasKey(x => x.Id);
		}
	}
}