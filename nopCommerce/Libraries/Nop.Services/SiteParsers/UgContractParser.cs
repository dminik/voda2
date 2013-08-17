namespace Nop.Services.SiteParsers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Nop.Core.Domain.YandexMarket;

	public class UgContractParser : BaseParser
	{
		// List
		protected override string CssSelectorForProductLinkInProductList { get { return ".goods-list:not(.goods-list-small) div.cat-item a"; } }
		protected override string CssSelectorForNextLinkInProductList { get { return "[title=���������]"; } }
		
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
			const string toErase1 = "���� �� �������� ������������ ������ � �������� ������, �������� ������ � �������";
			const string toErase2 = "Ctrl+Enter";
			const string toErase3 = ", ����� �������� ��� �� ����.";
			product.FullDescription = product.FullDescription.Replace(toErase1, "").Replace(toErase2, "").Replace(toErase3, "");

			product.Articul = product.Articul.Replace("���: ", "");

			product.Name = product.Name.Replace("��������� ������� ", "");

			foreach (var curSpec in product.Specifications)
			{				
				var s2 = "<p>";

				int i = 0;  // �������� ����������, �������������� �������� �����
				int x = -1; // ��� ��� ����� IndexOf() ���������� "-1" ���� ������ ��������� ��������� �� �������, �� ���������� ������������ ���������������, ������ �, ��� � ������ ����
				int count = -1; // ���������� ���������� ��������� (�������� �����)
				while (i != -1)
				{
					i = curSpec.Value.IndexOf(s2, x + 1); // �������� ������ ������� ���������  �+1 �������, ��� �������� ����� � 0-�� �������, ������ � ����� "�"
					x = i; // �������������� ����������� ����� ������� ������� ��������, ��� � ����� (�+1) ������ �� ����������
					count++;  // ����������� �� ������� ���� ����������
				}

				if (count == 1)
				{
					curSpec.Value = curSpec.Value.Replace("<p>", "").Replace("</p>", "").Trim();
				}
			}

			var manufactureName = GetManufactureFromName(product.Name);
			if (manufactureName != string.Empty)
				product.Specifications.Add(new YandexMarketSpecRecord("�������������", manufactureName));

			var megapixels = GetMegapixelsFromSpecs(product.Specifications);
			if (megapixels != string.Empty)
				product.Specifications.Add(new YandexMarketSpecRecord("���������� ������������ ������", megapixels));

			var displaySize = GetDisplaySize(product.Specifications);
			if (displaySize != string.Empty)
				product.Specifications.Add(new YandexMarketSpecRecord("������ ������, �����", displaySize));
			
			if (IsBluetooth(product.Specifications))
				product.Specifications.Add(new YandexMarketSpecRecord("Bluetooth", "����"));

			if (IsRadio(product.Specifications))
				product.Specifications.Add(new YandexMarketSpecRecord("FM-�����", "����"));

			if (IsDisplaySensor(product.Specifications))
				product.Specifications.Add(new YandexMarketSpecRecord("��������� �����", "��"));

			var simAmount = GetSimAmount(product);
			if (simAmount > 1 && !product.Specifications.Any(x => x.Key == "���������� SIM-����"))
				product.Specifications.Add(new YandexMarketSpecRecord("���������� SIM-����", simAmount.ToString()));

			
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
			const string pattern = @"([0-9]+(?:\.[0-9]+)?) �������";

			var spec = specs.SingleOrDefault(x => x.Key == "�������� ������");

			if (spec != null)
			{
				var megapixels = GetValByRegExp(spec.Value, pattern);
				return megapixels;
			}
			
			return "";
		}

		private string GetDisplaySize(IEnumerable<YandexMarketSpecRecord> specs)
		{
			const string pattern = "([0-9]+(?:\\.[0-9]+)?)\"";

			var spec = specs.SingleOrDefault(x => x.Key == "�������");

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
			return specs.SingleOrDefault(x => x.Value.ToLower().Contains("�����")) != null;
		}

		private bool IsDisplaySensor(IEnumerable<YandexMarketSpecRecord> specs)
		{
			return specs.SingleOrDefault(x => x.Key == "�������" && x.Value.ToLower().Contains("���������")) != null;
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
