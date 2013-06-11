namespace Nop.Plugin.Misc.YandexMarketParser
{
	using System.Web.Mvc;
	using System.Web.Routing;

	using Nop.Plugin.Misc.YandexMarketParser.Controllers;
	using Nop.Web.Framework.Mvc.Routes;

	public partial class RouteProvider : IRouteProvider
	{
		public void RegisterRoutes(RouteCollection routes)
		{
			routes.MapRoute("Plugin.Misc.YandexMarketParser.Category.Add",
				 "Plugins/YandexMarketParser/Category/Add",
				 new { controller = "YandexMarketCategory", action = "Add" },
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
