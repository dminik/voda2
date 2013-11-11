using System.Web.Mvc;
using Nop.Web.Framework.Security;

namespace Nop.Web.Controllers
{
	using Nop.Core;
	using Nop.Core.Caching;
	using Nop.Services.Localization;
	using Nop.Services.Topics;
	using Nop.Web.Infrastructure.Cache;
	using Nop.Web.Models.Home;
	using Nop.Web.Models.Topics;

	public partial class HomeController : BaseNopController
	{
		 #region Fields

		private readonly ITopicService _topicService;
		private readonly IWorkContext _workContext;
		private readonly IStoreContext _storeContext;		
		private readonly ICacheManager _cacheManager;

		#endregion

		#region Constructors

		public HomeController(ITopicService topicService,			
			IWorkContext workContext, 
			IStoreContext storeContext,
			ICacheManager cacheManager)
		{
			this._topicService = topicService;
			this._workContext = workContext;
			this._storeContext = storeContext;			
			this._cacheManager = cacheManager;
		}

		#endregion

		#region Utilities

		[NonAction]
		protected HomeModel PrepareHomeModel(string systemName)
		{
			//load by store
			var topic = _topicService.GetTopicBySystemName(systemName, _storeContext.CurrentStore.Id);
			if (topic == null)
				return null;

			var topicModel = new TopicModel()
			{
				Id = topic.Id,
				SystemName = topic.SystemName,
				IncludeInSitemap = topic.IncludeInSitemap,
				IsPasswordProtected = topic.IsPasswordProtected,
				Title = topic.IsPasswordProtected ? "" : topic.GetLocalized(x => x.Title),
				Body = topic.IsPasswordProtected ? "" : topic.GetLocalized(x => x.Body),
				MetaKeywords = topic.GetLocalized(x => x.MetaKeywords),
				MetaDescription = topic.GetLocalized(x => x.MetaDescription),
				MetaTitle = topic.GetLocalized(x => x.MetaTitle),
			};

			var model = new HomeModel()
				{
					Topic = topicModel,

					MetaKeywords = topicModel.MetaKeywords,
					MetaDescription = topicModel.MetaDescription,
					MetaTitle = topicModel.MetaTitle,
				};

			return model;
		}

		#endregion

		#region Methods

		[NopHttpsRequirement(SslRequirement.No)]
		public ActionResult Index()
		{
			string systemName = "HomePageText";
			var cacheKey = string.Format(ModelCacheEventConsumer.TOPIC_MODEL_KEY, systemName, _workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id);
			var cacheModel = _cacheManager.Get(cacheKey, () => PrepareHomeModel(systemName));
			
			return View(cacheModel);
		}       

		#endregion

	}
}
