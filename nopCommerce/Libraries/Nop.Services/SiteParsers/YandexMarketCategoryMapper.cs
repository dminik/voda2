namespace Nop.Services.SiteParsers
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Threading;
	using System.Web;

	using Nop.Core.Domain.Catalog;
	using Nop.Core.Domain.YandexMarket;
	using Nop.Services.Catalog;
	using Nop.Services.Logging;
	using Nop.Services.YandexMarket;

	using OpenQA.Selenium;
	using OpenQA.Selenium.IE;

	public class YandexMarketSpecMapper
	{
		private readonly IYandexMarketSpecService _yandexMarketSpecService;
		private readonly ISpecificationAttributeService _shopSpecService;

		public YandexMarketSpecMapper(
			IYandexMarketSpecService yandexMarketSpecService,
			ISpecificationAttributeService shopSpecService)
		{
			_yandexMarketSpecService = yandexMarketSpecService;
			_shopSpecService = shopSpecService;
		}


		public IList<SpecificationAttribute> GetSumOfYandexSpecsAndShopSpecs(int categoryId)
		{
			// Возвращает спецификаций магазина, дополненные из Яндекса

			var yandexMarketSpecList = _yandexMarketSpecService.GetByCategory(categoryId);
			var shopSpecList = _shopSpecService.GetSpecificationAttributes();

			foreach (var curYaSpec in yandexMarketSpecList)
			{
				// Find Spec
				var existedShopSpec = shopSpecList.SingleOrDefault(x => x.Name == curYaSpec.Key);
				if (existedShopSpec == null)
				{
					existedShopSpec = new SpecificationAttribute() { Name = curYaSpec.Key };
					shopSpecList.Add(existedShopSpec);
				}

				// Find Spec option
				var existedShopSpecOpt = existedShopSpec.SpecificationAttributeOptions.SingleOrDefault(x => x.Name == curYaSpec.Value);
				if (existedShopSpecOpt == null)
				{
					existedShopSpecOpt = new SpecificationAttributeOption(){ Name = curYaSpec.Value };
					existedShopSpec.SpecificationAttributeOptions.Add(existedShopSpecOpt);
				}
			}

			return shopSpecList;
		}

		public IList<SpecificationAttribute> GetNewYandexSpecsOnly(IList<SpecificationAttribute> fullSpecList)
		{
			var resultNewSpecList = new List<SpecificationAttribute>();

			foreach (var curSpec in fullSpecList)
			{
				var newSpec = new SpecificationAttribute() { Name = curSpec.Name };
				foreach (var curSpecOpt in curSpec.SpecificationAttributeOptions)
				{
					if(curSpecOpt.Id > 0)
						continue;

					newSpec.SpecificationAttributeOptions.Add(curSpecOpt);
				}

				if(newSpec.SpecificationAttributeOptions.Any())
					resultNewSpecList.Add(newSpec);
			}
			return resultNewSpecList;
		}
	}


	public class SpecMap
	{
		public YandexMarketSpecRecord YmSpec { get; set; }



	}

	public class ShopSpecToImport
	{


	}



}
