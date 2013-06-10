namespace Nop.Plugin.Misc.YandexMarketParser
{
	using System;
	using System.Web.Routing;

	using Nop.Core.Domain.Shipping;
	using Nop.Core.Plugins;
	using Nop.Plugin.Misc.YandexMarketParser.Data;
	using Nop.Services.Catalog;
	using Nop.Services.Common;
	using Nop.Services.Configuration;
	using Nop.Services.Localization;
	using Nop.Services.Shipping;
	using Nop.Services.Shipping.Tracking;

	public class YandexMarketParserPlugin : BasePlugin, IMiscPlugin
	{
		#region Fields
		
		private readonly YandexMarketParserObjectContext _objectContext;

		#endregion

		#region Ctor
		public YandexMarketParserPlugin(YandexMarketParserObjectContext objectContext)
		{
			_objectContext = objectContext;			
		}
		#endregion


		/// <summary>
		/// Install plugin
		/// </summary>
		public override void Install()
		{			
			//data
			_objectContext.Install();		
			base.Install();
		}

		/// <summary>
		/// Uninstall plugin
		/// </summary>
		public override void Uninstall()
		{		
			//data
			_objectContext.Uninstall();			
			base.Uninstall();
		}

	   
	   
		/// <summary>
		/// Gets a route for provider configuration
		/// </summary>
		/// <param name="actionName">Action name</param>
		/// <param name="controllerName">Controller name</param>
		/// <param name="routeValues">Route values</param>
		public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
		{
			actionName = "Configure";
			controllerName = "YandexMarketParser";
			routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Misc.YandexMarketParser.Controllers" }, { "area", null } };
		}		
	}
}
