namespace Nop.Plugin.Misc.YandexMarketParser.Data
{
	using System.Data.Entity.ModelConfiguration;

	using Nop.Plugin.Misc.YandexMarketParser.Domain;

	public partial class YandexMarketSpecRecordMap : EntityTypeConfiguration<YandexMarketSpecRecord>
    {
		public YandexMarketSpecRecordMap()
        {
			this.ToTable(YandexMarketParserObjectContext.TableNameSpec);
            this.HasKey(x => x.Id);
        }
    }
}