namespace Nop.Data.Mapping.YandexMarket
{
	using System.Data.Entity.ModelConfiguration;

	using Nop.Core.Domain.YandexMarket;

	public partial class YandexMarketSpecRecordMap : EntityTypeConfiguration<YandexMarketSpecRecord>
    {
		public YandexMarketSpecRecordMap()
        {
			this.ToTable("YandexMarketSpec");
            this.HasKey(x => x.Id);
        }
    }
}