namespace Nop.Services.SiteParsers.Xls
{
	using System;

	public class ProductLineVendor : ProductLine
	{
		
		public decimal PriceRaschet { get; set; }
		public decimal PriceBase { get; set; }
		public decimal PriceDiff { get; set; }
		

		public new string ToString()
		{
			return base.ToString()
				+ "PriceRaschet = " + this.PriceRaschet + Environment.NewLine
				+ "PriceBase = " + this.PriceBase + Environment.NewLine
				+ "PriceDiff = " + this.PriceDiff + Environment.NewLine + Environment.NewLine;
		}
	}
}
