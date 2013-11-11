namespace Nop.Web.Models.Home
{
	using Nop.Web.Framework.Mvc;
	using Nop.Web.Models.Topics;

	public partial class HomeModel : BaseNopEntityModel
	{		
		public string MetaKeywords { get; set; }
		public string MetaDescription { get; set; }
		public string MetaTitle { get; set; }
		public TopicModel Topic { get; set; }	
		
	}
}