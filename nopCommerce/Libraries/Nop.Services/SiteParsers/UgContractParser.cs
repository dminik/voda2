namespace Nop.Services.SiteParsers
{
	using System.Collections.Generic;
	using System.Linq;

	using Nop.Core.Domain.YandexMarket;

	public class UgContractParser : BaseParser
	{
		// List
		protected override string CssSelectorForProductLinkInProductList { get { return "div.cat-item a"; } }
		protected override string CssSelectorForNextLinkInProductList { get { return "[title=следующая]"; } }
		
		// Product
		protected override string CssSelectorForProductArticulInProductPage { get { return ".good-info-number"; } } 
		protected override string CssSelectorForProductNameInProductPage { get { return "h1.good-info-header"; } }
		protected override string CssSelectorForProductPictureInProductPage { get { return ".lightbox img"; } }
		protected override string CssSelectorForProductPicturesInProductPage { get { return "div.small-photos ul#images-list li a"; } }
		protected override string CssSelectorForProductFullDescriptionInProductPage { get { return "div.panes-item.goods-item-panes-item.content"; } }
		
		// Product specs
		protected override string GetLinkToSpecsTabOfProduct(string productLink) { return productLink + "#chars"; }
		protected override string CssSelectorForProductSpecsRowsInProductPage { get { return ".properties-table tr"; } }
		protected override string CssSelectorForProductSpecKeyInProductPage { get { return "td.prop"; } }
		protected override string CssSelectorForProductSpecValInProductPage { get { return "td.val"; } }

		protected override YandexMarketProductRecord ProductPostProcessing(YandexMarketProductRecord product)
		{
			const string toErase1 = "Если вы заметили некорректные данные в описании товара, выделите ошибку и нажмите";
			const string toErase2 = "Ctrl+Enter";
			const string toErase3 = ", чтобы сообщить нам об этом.";
			product.FullDescription = product.FullDescription.Replace(toErase1, "").Replace(toErase2, "").Replace(toErase3, "");

			product.Articul = product.Articul.Replace("Код: ", "");

			product.Name = product.Name.Replace("МОБИЛЬНЫЙ ТЕЛЕФОН ", "");

			var manufactureName = GetManufactureFromName(product.Name);
			if (manufactureName != string.Empty)
				product.Specifications.Add(new YandexMarketSpecRecord("Производитель", manufactureName));

			var megapixels = GetMegapixelsFromSpecs(product.Specifications);
			if (megapixels != string.Empty)
				product.Specifications.Add(new YandexMarketSpecRecord("Количество мегапикселей камеры", megapixels));

			var displaySize = GetDisplaySize(product.Specifications);
			if (displaySize != string.Empty)
				product.Specifications.Add(new YandexMarketSpecRecord("Размер экрана, дюймы", displaySize));
			
			if (IsBluetooth(product.Specifications))
				product.Specifications.Add(new YandexMarketSpecRecord("Bluetooth", "Есть"));

			if (IsRadio(product.Specifications))
				product.Specifications.Add(new YandexMarketSpecRecord("FM-радио", "Есть"));

			if (IsDisplaySensor(product.Specifications))
				product.Specifications.Add(new YandexMarketSpecRecord("Сенсорный экран", "Да"));

			var simAmount = GetSimAmount(product);
			if (simAmount > 1 && !product.Specifications.Any(x => x.Key == "Количество SIM-карт"))
				product.Specifications.Add(new YandexMarketSpecRecord("Количество SIM-карт", simAmount.ToString()));

			
			return product;
		}

		private string GetManufactureFromName(string name)
		{			  
			var manufactures = new List<string>() { "Sony", "Fly", "Alcatel", "LG", "Philips", "Nokia", "Samsung", "HTC", "ZTE", "Huawei"};
			name = name.ToUpper();

			foreach (var manufacture in manufactures.Where(manufacture => name.Contains(manufacture.ToUpper()))) 
				return manufacture;

			return "";
		}

		private string GetMegapixelsFromSpecs(IList<YandexMarketSpecRecord> specs)
		{
			const string pattern = @"([0-9](?:\.[0-9])?) мегапик";

			var spec = specs.SingleOrDefault(x => x.Key == "Цифровая камера");

			if (spec != null)
			{
				var megapixels = GetValByRegExp(spec.Value, pattern);
				return megapixels;
			}
			
			return "";
		}

		private string GetDisplaySize(IEnumerable<YandexMarketSpecRecord> specs)
		{
			const string pattern = "([0-9](?:\\.[0-9])?)\"";

			var spec = specs.SingleOrDefault(x => x.Key == "Дисплей");

			if (spec != null)
			{
				var result = GetValByRegExp(spec.Value, pattern);
				return result;
			}

			return "";
		}

		private bool IsBluetooth(IEnumerable<YandexMarketSpecRecord> specs)
		{			
			return specs.SingleOrDefault(x => x.Value.ToLower().Contains("bluetooth")) != null;
		}

		private bool IsRadio(IEnumerable<YandexMarketSpecRecord> specs)
		{
			return specs.SingleOrDefault(x => x.Value.ToLower().Contains("радио")) != null;
		}

		private bool IsDisplaySensor(IEnumerable<YandexMarketSpecRecord> specs)
		{
			return specs.SingleOrDefault(x => x.Key == "Дисплей" && x.Value.ToLower().Contains("сенсорный")) != null;
		}

		private int GetSimAmount(YandexMarketProductRecord product)
		{			
			if (product.Name.ToLower().Contains("dual")
				|| product.Name.ToLower().Contains("duos")
				|| product.FullDescription.ToLower().Contains("2 sim")
				) return 2;

			if (product.Name.ToLower().Contains("triple")) 
				return 3;

			return 1;
		}
	}
}
