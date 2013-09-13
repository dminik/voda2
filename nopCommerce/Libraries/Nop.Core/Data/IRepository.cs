using System.Linq;

namespace Nop.Core.Data
{
	using System.Collections.Generic;

	/// <summary>
	/// Repository
	/// </summary>
	public partial interface IRepository2<T> : IRepository<T>
		where T : BaseEntity
	{
		void InsertList(IEnumerable<T> entityList);
	}

	/// <summary>
	/// Repository
	/// </summary>
	public partial interface IRepository<T> where T : BaseEntity
	{
		T GetById(object id);
		void Insert(T entity);
		void Update(T entity);
		void Delete(T entity);
		IQueryable<T> Table { get; }
	}
}
