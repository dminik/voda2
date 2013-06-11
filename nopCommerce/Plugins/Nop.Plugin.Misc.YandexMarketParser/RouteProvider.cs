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
				 "Plugins/YandexMarketParser/CategoryAdd",
				 new { controller = "YandexMarketCategory", action = "Add" },
				 new[] { "Nop.Plugin.Misc.YandexMarketParser.Controllers" }
			);

			//routes.MapRoute("Plugin.Misc.YandexMarketParser.Category.List",
			//	 "Plugins/YandexMarketParser/Category/List",
			//	 new { controller = "Category", action = "List" },
			//	 new[] { "Nop.Plugin.Misc.YandexMarketParser.Controllers" }
			//);

			//routes.MapRoute("Plugin.Misc.YandexMarketParser.Category.Update",
			//	 "Plugins/YandexMarketParser/Category/Update",
			//	 new { controller = "Category", action = "Update" },
			//	 new[] { "Nop.Plugin.Misc.YandexMarketParser.Controllers" }
			//);

			//routes.MapRoute("Plugin.Misc.YandexMarketParser.Category.Delete",
			//	 "Plugins/YandexMarketParser/Category/Delete",
			//	 new { controller = "Category", action = "Delete" },
			//	 new[] { "Nop.Plugin.Misc.YandexMarketParser.Controllers" }
			//);
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
