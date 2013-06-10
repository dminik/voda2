namespace Nop.Plugin.Misc.YandexMarketParser.Data
{
	using System.Data.Entity.ModelConfiguration;

	using Nop.Plugin.Misc.YandexMarketParser.Domain;

	public partial class YandexMarketCategoryRecordMap : EntityTypeConfiguration<YandexMarketCategoryRecord>
    {
		public YandexMarketCategoryRecordMap()
        {
			this.ToTable(YandexMarketParserObjectContext.TableNameCategory);
            this.HasKey(x => x.Id);
        }
    }
}