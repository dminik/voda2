namespace Nop.Core.Domain.Catalog
{
	using System.Collections.Generic;

	public class Filter
	{
		public List<int> CategoryIds = new List<int>();
		public List<int> AlreadyFilteredSpecOptionIds = new List<int>();
		public bool ShowWithPositiveQuantity;
	}
}