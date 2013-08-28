namespace Nop.Services.YandexMarket
{
	using System.Collections.Generic;
	using System.Linq;

	using Nop.Core.Domain.YandexMarket;

	public class FormatterUgContract : FormatterBase
	{		
		public override YandexMarketProductRecord Format(YandexMarketProductRecord product)
		{
			const string toErase1 = "���� �� �������� ������������ ������ � �������� ������, �������� ������ � �������";
			const string toErase2 = "Ctrl+Enter";
			const string toErase3 = ", ����� �������� ��� �� ����.";
			product.FullDescription = product.FullDescription.Replace(toErase1, "").Replace(toErase2, "").Replace(toErase3, "");

			product.Articul = product.Articul.Replace("���: ", "");

			product.Name = product.Name.Replace("��������� ������� ", "");

			foreach (var curSpec in product.Specifications)
			{				
				var s2 = "<";

				int i = 0;  // �������� ����������, �������������� �������� �����
				int x = -1; // ��� ��� ����� IndexOf() ���������� "-1" ���� ������ ��������� ��������� �� �������, �� ���������� ������������ ���������������, ������ �, ��� � ������ ����
				int count = -1; // ���������� ���������� ��������� (�������� �����)
				while (i != -1)
				{
					i = curSpec.Value.IndexOf(s2, x + 1); // �������� ������ ������� ���������  �+1 �������, ��� �������� ����� � 0-�� �������, ������ � ����� "�"
					x = i; // �������������� ����������� ����� ������� ������� ��������, ��� � ����� (�+1) ������ �� ����������
					count++;  // ����������� �� ������� ���� ����������
				}

				if (count <= 2)
				{
					curSpec.Value = curSpec.Value.Replace("<p>", "").Replace("</p>", "").Trim();
				}
			}

			var manufactureName = this.GetManufactureFromName(product.Name);
			if (manufactureName != string.Empty)
				product.Specifications.Add(new YandexMarketSpecRecord("�������������", manufactureName));

			var megapixels = this.GetMegapixelsFromSpecs(product.Specifications);
			if (megapixels != string.Empty)
				product.Specifications.Add(new YandexMarketSpecRecord("���������� ������������ ������", megapixels));

			var displaySize = this.GetDisplaySize(product.Specifications);
			if (displaySize != string.Empty)
				product.Specifications.Add(new YandexMarketSpecRecord("������ ������, �����", displaySize));
			
			if (this.IsBluetooth(product.Specifications))
				product.Specifications.Add(new YandexMarketSpecRecord("Bluetooth", "����"));

			if (this.IsAndroid(product.Specifications))
				product.Specifications.Add(new YandexMarketSpecRecord("Android", "��"));

			if (this.IsFlash(product.Specifications))
				product.Specifications.Add(new YandexMarketSpecRecord("�������", "����"));

			if (this.IsRadio(product.Specifications))
				product.Specifications.Add(new YandexMarketSpecRecord("FM-�����", "����"));

			if (this.IsDisplaySensor(product.Specifications))
				product.Specifications.Add(new YandexMarketSpecRecord("��������� �����", "��"));

			var simAmount = this.GetSimAmount(product);
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

		private string GetMegapixelsFromSpecs(IEnumerable<YandexMarketSpecRecord> specs)
		{
			const string pattern = @"([0-9]+(?:\.[0-9]+)?) [��]";

			var spec = specs.SingleOrDefault(x => x.Key.ToLower().Contains("������"));

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

		private bool IsAndroid(IEnumerable<YandexMarketSpecRecord> specs)
		{
			return specs.SingleOrDefault(x => x.Value.ToLower().Contains("android")) != null;
		}

		private bool IsFlash(IEnumerable<YandexMarketSpecRecord> specs)
		{
			return specs.SingleOrDefault(x => x.Value.ToLower().Contains("�������")) != null;
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
