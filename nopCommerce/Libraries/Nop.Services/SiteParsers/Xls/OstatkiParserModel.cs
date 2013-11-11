namespace Nop.Services.SiteParsers.Xls
{
	using System.Collections.Generic;

	public class OstatkiParserModel
	{
		public IEnumerable<ProductLine> ProductLineList { get; set; }
		public IEnumerable<string> ErrorList { get; set; }       
	}
}