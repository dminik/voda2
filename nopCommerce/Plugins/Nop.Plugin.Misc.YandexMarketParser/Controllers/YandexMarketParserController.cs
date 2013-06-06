namespace Nop.Plugin.Misc.YandexMarketParser.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Text;
	using System.Threading;
	using System.Web.Mvc;

	using Nop.Core.Domain.Directory;
	using Nop.Plugin.Misc.YandexMarketParser.Models;
	using Nop.Services.Configuration;
	using Nop.Services.Directory;
	using Nop.Services.Localization;
	using Nop.Services.Security;
	using Nop.Services.Shipping;
	using Nop.Web.Framework.Controllers;

	[AdminAuthorize]
    public class YandexMarketParserController : Controller
    {
		//private readonly IShippingService _shippingService;
		//private readonly ICountryService _countryService;
		//private readonly IStateProvinceService _stateProvinceService;
		//private readonly ShippingByWeightSettings _shippingByWeightSettings;
		//private readonly IShippingByWeightService _shippingByWeightService;
		//private readonly ISettingService _settingService;
		//private readonly ILocalizationService _localizationService;
		//private readonly IPermissionService _permissionService;

		//private readonly ICurrencyService _currencyService;
		//private readonly CurrencySettings _currencySettings;
		//private readonly IMeasureService _measureService;
		//private readonly MeasureSettings _measureSettings;

		//public ShippingByWeightController(IShippingService shippingService,
		//	ICountryService countryService, IStateProvinceService stateProvinceService,
		//	ShippingByWeightSettings shippingByWeightSettings,
		//	IShippingByWeightService shippingByWeightService, ISettingService settingService,
		//	ILocalizationService localizationService, IPermissionService permissionService,
		//	ICurrencyService currencyService, CurrencySettings currencySettings,
		//	IMeasureService measureService, MeasureSettings measureSettings)
		//{
		//	this._shippingService = shippingService;
		//	this._countryService = countryService;
		//	this._stateProvinceService = stateProvinceService;
		//	this._shippingByWeightSettings = shippingByWeightSettings;
		//	this._shippingByWeightService = shippingByWeightService;
		//	this._settingService = settingService;
		//	this._localizationService = localizationService;
		//	this._permissionService = permissionService;

		//	this._currencyService = currencyService;
		//	this._currencySettings = currencySettings;
		//	this._measureService = measureService;
		//	this._measureSettings = measureSettings;
		//}


		[AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new YandexMarketParserModel();
          
	        model.HelloWord = "Ne beebe";

			return View("Nop.Plugin.Misc.YandexMarketParser.Views.YandexMarketParser.Configure", model);
        }

		
		[HttpPost]
		[AdminAuthorize]
		[ChildActionOnly]
		public ActionResult Configure(YandexMarketParserModel model)
		{
			if (!model.IsTest)
			{
				if (!ModelState.IsValid) return Configure();

				var parser = new Parser("Atoll", model.ParseNotMoreThen);
				model = parser.Parse();
			}
			else
			{
				model = new YandexMarketParserModel()
				{
					IsTest = true,
					CatalogName = "testCatalogName",
					ProductList =
						new List<Product>()
							{
								new Product()
									{
										Title = "Product 1",
										ImageUrl_1 = "url1",
										Specifications = new Dictionary<string, string>() { { "key1", "value1" }, { "key2", "value2" }, }
									},
								new Product()
									{
										Title = "Product 2",
										ImageUrl_1 = "url2",
										Specifications = new Dictionary<string, string>() { { "key1", "value1" }, { "key2", "value2" }, }
									}
							}
				};
			}

			return View("Nop.Plugin.Misc.YandexMarketParser.Views.YandexMarketParser.Configure", model);
		}		
    }
}
