namespace Nop.Plugin.Misc.YandexMarketParser.Domain
{
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Runtime.Serialization;

	using Nop.Core;

	/// <summary>
	/// Represents a product record
	/// </summary>
	[DataContract]
	public partial class YandexMarketSpecRecord : BaseEntity
	{
		public YandexMarketSpecRecord()
		{
		}

		public YandexMarketSpecRecord(string key, string value)
			: this()
		{
			Key = key;
			Value = value;			
		}

		[DataMember]
		public string Key { get; set; }

		[DataMember]
		public string Value { get; set; }

		[DataMember]
		public int ProductRecordId { get; set; }

		[ScaffoldColumn(false)]
		[Display(AutoGenerateField = false)]
		public virtual YandexMarketProductRecord ProductRecord { get; set; }
	}
}