using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;

namespace Nop.Data
{
	/// <summary>
	/// Entity Framework repository
	/// </summary>
	public partial class EfRepository<T>
	{
		public void SaveChanges()
		{
			try
			{
			   
				this._context.SaveChanges();
			}
			catch (DbEntityValidationException dbEx)
			{
				var msg = string.Empty;

				foreach (var validationErrors in dbEx.EntityValidationErrors)
					foreach (var validationError in validationErrors.ValidationErrors)
						msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

				var fail = new Exception(msg, dbEx);
				//Debug.WriteLine(fail.Message, fail);
				throw fail;
			}
		}

	  
	}
}