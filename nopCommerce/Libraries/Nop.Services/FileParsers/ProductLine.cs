using System;

namespace Nop.Services.FileParsers
{	
	public class ProductLine
	{
		public string Articul { get; set; }
		public int Price { get; set; }
		public int Amount { get; set; }

		public new string ToString()
		{
			return "Articul = " + Articul + Environment.NewLine
				+ "Price = " + Price + Environment.NewLine
				+ "Amount = " + Amount + Environment.NewLine + Environment.NewLine;
		}
	}
}
