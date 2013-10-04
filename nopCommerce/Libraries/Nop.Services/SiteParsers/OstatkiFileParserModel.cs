namespace Nop.Services.SiteParsers
{
	using System.Collections.Generic;

	using Nop.Services.FileParsers;

	public class OstatkiFileParserModel
	{
		public IEnumerable<ProductLine> ProductLineList { get; set; }
		public IEnumerable<string> ErrorList { get; set; }       
	}
}