namespace Nop.Services.SiteParsers
{
	using System.Collections.Generic;

	using Nop.Services.FileParsers;

	public class VendorFileParserModel
	{
		public IEnumerable<ProductLineVendor> ProductLineList { get; set; }
		public IEnumerable<string> ErrorList { get; set; }       
	}
}