namespace Nop.Plagin.Misc.YandexMarketParser
{
	using System;
	using System.Web.Routing;

	using Nop.Core.Domain.Shipping;
	using Nop.Core.Plugins;
	using Nop.Services.Catalog;
	using Nop.Services.Common;
	using Nop.Services.Configuration;
	using Nop.Services.Localization;
	using Nop.Services.Shipping;
	using Nop.Services.Shipping.Tracking;

	public class YandexMarketParserPlagin : BasePlugin, IMiscPlugin
	{
		#region Fields

		//private readonly IShippingService _shippingService;
		// private readonly IShippingByWeightService _shippingByWeightService;
		//private readonly IPriceCalculationService _priceCalculationService;
		//private readonly ShippingByWeightSettings _shippingByWeightSettings;
		//private readonly ShippingByWeightObjectContext _objectContext;
		//private readonly ISettingService _settingService;

		#endregion

		#region Ctor
		public YandexMarketParserPlagin(
			// IShippingService shippingService,
			//IShippingByWeightService shippingByWeightService,
			//IPriceCalculationService priceCalculationService, 
			//ShippingByWeightSettings shippingByWeightSettings,
			//ShippingByWeightObjectContext objectContext,
			//ISettingService settingService
			)
		{
			//this._shippingService = shippingService;
			//this._shippingByWeightService = shippingByWeightService;
			//this._priceCalculationService = priceCalculationService;
			//this._shippingByWeightSettings = shippingByWeightSettings;
			//this._objectContext = objectContext;
			//this._settingService = settingService;
		}
		#endregion

	   
	   

	   
	   
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
			routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plagin.Misc.YandexMarketParser.Controllers" }, { "area", null } };
		}		
	}
}
