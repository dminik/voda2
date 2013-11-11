using System.Linq;

namespace Nop.Core.Data
{
	/// <summary>
	/// Repository
	/// </summary>
	public partial interface IRepository<T>
	{
		void SaveChanges();
	}
}
