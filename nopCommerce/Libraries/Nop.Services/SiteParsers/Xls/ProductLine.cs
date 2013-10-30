namespace Nop.Services.SiteParsers.Xls
{
	using System;

	public class ProductLine
	{
		public string Articul { get; set; }
		public string Name { get; set; }
		public decimal Price { get; set; }
		public int Amount { get; set; }

		public new string ToString()
		{
			return "Articul = " + this.Articul + Environment.NewLine
				+ "Name = " + this.Name + Environment.NewLine
				+ "Price = " + this.Price + Environment.NewLine
				+ "Amount = " + this.Amount + Environment.NewLine + Environment.NewLine;
		}
	}
}
