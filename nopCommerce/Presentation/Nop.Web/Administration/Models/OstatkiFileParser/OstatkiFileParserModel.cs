namespace Nop.Admin.Models.OstatkiFileParser
{
	using System.Collections.Generic;

	using Nop.Services.FileParsers;
	using Nop.Web.Framework.Mvc;

	public class OstatkiFileParserModel : BaseNopEntityModel
	{
		public IEnumerable<ProductLine> ProductLineList { get; set; }
		public IEnumerable<string> ErrorList { get; set; }       
	}
}