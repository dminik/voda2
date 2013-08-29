using System.Collections.Generic;
using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Catalog
{
	using System.Collections.ObjectModel;

	/// <summary>
	/// Represents a specification attribute
	/// </summary>
	public partial class SpecificationAttribute : BaseEntity, ILocalizedEntity
	{
		private ICollection<SpecificationAttributeOption> _specificationAttributeOptions;

		/// <summary>
		/// Gets or sets the name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the display order
		/// </summary>
		public int DisplayOrder { get; set; }

		/// <summary>
		/// Gets or sets the specification attribute options
		/// </summary>
		public virtual ICollection<SpecificationAttributeOption> SpecificationAttributeOptions
		{
			get { return _specificationAttributeOptions ?? (_specificationAttributeOptions = new List<SpecificationAttributeOption>()); }
			protected set { _specificationAttributeOptions = value; }
		}

		public SpecificationAttribute Clone()
		{
			var newOb = new SpecificationAttribute()
			{
				Id = this.Id,
				Name = this.Name,				
				DisplayOrder = this.DisplayOrder,
				SpecificationAttributeOptions = new Collection<SpecificationAttributeOption>()
			};

			foreach (var specificationAttributeOption in SpecificationAttributeOptions)
			{
				newOb.SpecificationAttributeOptions.Add(specificationAttributeOption.Clone());
			}


			return newOb;
		}
	}
}
