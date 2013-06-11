namespace Nop.Plugin.Misc.YandexMarketParser.Data
{
	using System.Data.Entity.ModelConfiguration;

	using Nop.Plugin.Misc.YandexMarketParser.Domain;

	public partial class ProductRecordMap : EntityTypeConfiguration<ProductRecord>
    {
		public ProductRecordMap()
        {
			this.ToTable(YandexMarketParserObjectContext.TableNameProduct);
            this.HasKey(x => x.Id);
        }
    }
}