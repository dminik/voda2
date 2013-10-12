namespace Nop.Services.SiteParsers
{
	using System.Collections.Generic;

	using Nop.Services.SiteParsers.Xls;

	public class OstatkiParserModel
	{
		public IEnumerable<ProductLine> ProductLineList { get; set; }
		public IEnumerable<string> ErrorList { get; set; }       
	}
}