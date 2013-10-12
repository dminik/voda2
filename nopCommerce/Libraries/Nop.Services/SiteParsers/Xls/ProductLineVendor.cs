namespace Nop.Services.SiteParsers.Xls
{
	using System;

	public class ProductLineVendor : ProductLine
	{
		
		public int PriceRaschet { get; set; }
		public int PriceBase { get; set; }
		public int PriceDiff { get; set; }
		

		public new string ToString()
		{
			return base.ToString()
				+ "PriceRaschet = " + this.PriceRaschet + Environment.NewLine
				+ "PriceBase = " + this.PriceBase + Environment.NewLine
				+ "PriceDiff = " + this.PriceDiff + Environment.NewLine + Environment.NewLine;
		}
	}
}
