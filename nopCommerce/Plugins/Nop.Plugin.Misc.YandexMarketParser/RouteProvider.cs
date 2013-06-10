namespace Nop.Plugin.Misc.YandexMarketParser
{
	using System.Web.Mvc;
	using System.Web.Routing;

	using Nop.Web.Framework.Mvc.Routes;

	public partial class RouteProvider : IRouteProvider
	{
		public void RegisterRoutes(RouteCollection routes)
		{
			routes.MapRoute("Plugin.Misc.YandexMarketParser.AddCategory",
				 "Plugins/YandexMarketParser/AddCategory",
				 new { controller = "YandexMarketParser", action = "AddCategory" },
				 new[] { "Nop.Plugin.Misc.YandexMarketParser.Controllers" }
			);
		}
		public int Priority
		{
			get
			{
				return 0;
			}
		}
	}
}
