using System;

namespace Nop.Services.FileParsers
{
	public class ProductLineVendor : ProductLine
	{
		
		public int PriceRaschet { get; set; }
		public int PriceBase { get; set; }
		public int PriceDiff { get; set; }
		

		public new string ToString()
		{
			return base.ToString()
				+ "PriceRaschet = " + PriceRaschet + Environment.NewLine
				+ "PriceBase = " + PriceBase + Environment.NewLine
				+ "PriceDiff = " + PriceDiff + Environment.NewLine + Environment.NewLine;
		}
	}
}
