namespace Nop.Admin.Models.FileParser
{
	using System.Collections.Generic;

	using Nop.Services.FileParsers;
	using Nop.Web.Framework.Mvc;

	public class VendorFileParserModel : BaseNopEntityModel
	{
		public IEnumerable<ProductLineVendor> ProductLineList { get; set; }
		public IEnumerable<string> ErrorList { get; set; }       
	}
}