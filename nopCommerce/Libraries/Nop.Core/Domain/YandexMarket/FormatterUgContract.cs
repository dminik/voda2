namespace Nop.Core.Domain.YandexMarket
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;

	public class FormatterUgContract : FormatterBase
	{		
		public override YandexMarketProductRecord Format(YandexMarketProductRecord product)
		{			
			const string toErase1 = "���� �� �������� ������������ ������ � �������� ������, �������� ������ � �������";
			const string toErase2 = "Ctrl+Enter";
			const string toErase3 = ", ����� �������� ��� �� ����.";
			product.FullDescription = product.FullDescription.Replace(toErase1, "").Replace(toErase2, "").Replace(toErase3, "");

			product.Articul = product.Articul.Replace("���: ", "");

			//product.Name = product.Name.Replace("��������� ������� ", "").Replace("�������� ", "");

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


			ReplaceByValue(product.Specifications, new List<string> { "MB", "��", "M�", "Mb" }, "MB", "�����������");


			ReplaceByValue(product.Specifications, new List<string> { "���", "-bit", "bit", "Mb", " ", "Bit" }, "", "����������� ���� ������");
			ReplaceByValue(product.Specifications, new List<string> { "12 V"}, "12V", "���� �������");
			ReplaceByValue(product.Specifications, new List<string> { "v." }, "", "���� �������");
			ReplaceByValue(product.Specifications, new List<string> { "2.2 12V" }, "12V v.2.2", "���� �������");

			ReplaceByValue(product.Specifications, new List<string> { "��������/���.", "��/���", "��������/���."}, "rpm", "�������� �������� ��������");
			ReplaceByValue(product.Specifications, new List<string> { " �����", "\"" }, "", "������");
			ReplaceByValue(product.Specifications, new List<string> { " �����", "\"" }, "", "������");

			ReplaceByValue(product.Specifications, new List<string> { "MB", "��", "M�", "Mb", "��", "��", "�B", "MB", "MB", "�B",  }, "MB", "�����");

			   
			ReplaceByValue(product.Specifications, new List<string> { "TB", "��", "T�" }, "��", "�������");
			ReplaceByValue(product.Specifications, new List<string> { "��", "��" }, "GB", "�������");
			ReplaceByValue(product.Specifications, new List<string> { ".0" }, "", "�������");
			ReplaceByValue(product.Specifications, new List<string> { "-", " " }, "", "���������");
			ReplaceByValue(product.Specifications, new List<string> { "3" }, "III", "���������");

			ReplaceByValue(product.Specifications, new List<string> { "m-ATX", "mATX", "MicroATX", "Micro-ATX", "microATX", "micro ATX",  }, "micro-ATX", "������");
			ReplaceByValue(product.Specifications, new List<string> { "Mini-ITX", "miniITX", "MicroATX", "Micro-ATX",   }, "mini-ITX", "������");

			ReplaceByValue(product.Specifications, new List<string> { "Minitower" }, "MiniTower", "��� ������������");
			ReplaceByValue(product.Specifications, new List<string> { "Miditower", "Mediumtower", "Midi Tower" }, "MidiTower", "��� ������������");
			ReplaceByValue(product.Specifications, new List<string> { "�������������", "��������������", "��������������", "���������������" }, "��������������", "���������� ����� �������");


			var manufactureName = this.GetManufactureFromName(product.Name);
			if (manufactureName != string.Empty && product.Specifications.All(x => x.Key != "�������������"))
				product.Specifications.Add(new YandexMarketSpecRecord("�������������", manufactureName));

			var megapixels = this.GetMegapixelsFromSpecs(product.Specifications);
			if (megapixels != string.Empty && product.Specifications.All(x => x.Key != "���������� ������������ ������"))
				product.Specifications.Add(new YandexMarketSpecRecord("���������� ������������ ������", megapixels));

			if (product.Specifications.Any(x => x.Key == "���������� �������"))
			{
				megapixels = this.GetCameraMegapixelsFromSpecs(product.Specifications);
				if (megapixels != string.Empty)
					product.Specifications.Single(x => x.Key == "���������� �������").Value = megapixels;				
			}

			var displaySize = this.GetDisplaySize(product.Specifications);
			if (displaySize != string.Empty && product.Specifications.All(x => x.Key != "������ ������, �����"))
				product.Specifications.Add(new YandexMarketSpecRecord("������ ������, �����", displaySize));

			if (this.IsBluetooth(product.Specifications) && product.Specifications.All(x => x.Key != "Bluetooth"))
				product.Specifications.Add(new YandexMarketSpecRecord("Bluetooth", "����"));

			if (this.IsUsbAcustika(product.Specifications, product.FullDescription) && product.Specifications.All(x => x.Key != "USB-�����������"))
				product.Specifications.Add(new YandexMarketSpecRecord("USB-�����������", "��"));

			if (this.IsExternal(product.Name) && product.Specifications.All(x => x.Key != "�������"))
				product.Specifications.Add(new YandexMarketSpecRecord("�������", "��"));

			if (this.IsAndroid(product.Specifications) && product.Specifications.All(x => x.Key != "Android"))
				product.Specifications.Add(new YandexMarketSpecRecord("Android", "��"));

			if (this.IsFlash(product.Specifications) && product.Specifications.All(x => x.Key != "�������"))
				product.Specifications.Add(new YandexMarketSpecRecord("�������", "����"));

			if (this.IsRadio(product.Specifications) && product.Specifications.All(x => x.Key != "FM-�����"))
				product.Specifications.Add(new YandexMarketSpecRecord("FM-�����", "����"));

			if (this.IsDisplaySensor(product.Specifications) && product.Specifications.All(x => x.Key != "��������� �����"))
				product.Specifications.Add(new YandexMarketSpecRecord("��������� �����", "��"));

			var simAmount = this.GetSimAmount(product);
			if (simAmount > 1 && product.Specifications.All(x => x.Key != "���������� SIM-����"))
				product.Specifications.Add(new YandexMarketSpecRecord("���������� SIM-����", simAmount.ToString()));

			var buttons = this.GetMouseButtonsFromSpecs(product.Specifications);
			if (buttons != string.Empty && product.Specifications.All(x => x.Key != "���������� ������"))
				product.Specifications.Add(new YandexMarketSpecRecord("���������� ������", buttons));

			var chip = this.GetChipFromSpecs(product.Specifications);
			if (chip != string.Empty && product.Specifications.All(x => x.Key != "��� ����"))
				product.Specifications.Add(new YandexMarketSpecRecord("��� ����", chip));
			
			return product;
		}


		private string GetManufactureFromName(string name)
		{
			var manufactures = new List<string>() { 
				"Asus", 
				"Alcatel", 
				"AMD",
				"Biostar",
				"ASRock",

				"ELITEGROUP",
				"Gigabyte",

				"HTC", 
				"Huawei",

				"Genius",

				"Fly", 		
		
				"LG", 
				"Logitech",	
			
				"Nokia",

				"Philips", 

				"Samsung", 
				"Sony",

				"Trust",

				"ZTE", 
				
				
				
				
				"Intel",
				"Kingston",
				"Silicon Power",
				
				"HIS",
				"Palit",
				"SAPPHIRE",
				"SPARKLE",
				"Sandisk",
				"Hitachi",
				"Seagate",
				"TOSHIBA",
				"Western Digital",
				"Lite-On",
				"NEC",
				"Pioneer",
				"Chieftec",
				"Codegen",
				"DeLux",
				"DTS",
				"Gembird",
				"Logicpower",
				"Maxxtro",
				"OKtet", 
				"Sparkman", 
				"Spire", 
				"THULE", 
				"AXES",
				"Chieftec",
				"Codegen",
				"Coolermaster",
				"FSP",
				"LinkWorld",
				"Logicpower",
				"Tuncmatik", 
				"ZTE", 
				"ZTE", 
				"ZTE", 
				"ZTE", 
				"ZTE", 
				"ZTE", 



				};

			name = name.ToUpper();

			foreach (var manufacture in manufactures.Where(manufacture => name.Contains(manufacture.ToUpper()))) 
				return manufacture;

			return "";
		}

		private string GetMegapixelsFromSpecs(IEnumerable<YandexMarketSpecRecord> specs)
		{			
			var spec = specs.FirstOrDefault(x => x.Key.ToLower().Contains("������"));

			if (spec != null)
			{
				string pattern = @"([0-9]+(?:\.[0-9]+)?) [��]";
							
				var megapixels = this.GetValByRegExp(spec.Value, pattern);
				megapixels = megapixels.Replace(".0", "").Replace(".00", "");
				
				return megapixels;
			}
			
			return "";
		}

		private string GetCameraMegapixelsFromSpecs(IEnumerable<YandexMarketSpecRecord> specs)
		{
			var spec = specs.FirstOrDefault(x => x.Key.ToLower().Contains("���������� �������"));

			if (spec != null)
			{
				string pattern = @"([0-9]+(?:\.[0-9]+)?)";
				
				var megapixels = this.GetValByRegExp(spec.Value, pattern);
				megapixels = megapixels.Replace(".0", "").Replace(".00", "");

				if (double.Parse(megapixels) > 30)
					megapixels = "1.3";

				return megapixels;
			}

			return "";
		}

		private string GetMouseButtonsFromSpecs(IEnumerable<YandexMarketSpecRecord> specs)
		{
			const string pattern = @"([0-9]+) ������";

			var spec = specs.FirstOrDefault(x => x.Key.ToLower().Contains("�������������� ��������������"));

			if (spec != null)
			{
				var foundValue = this.GetValByRegExp(spec.Value, pattern);
				return foundValue;
			}

			return "";
		}

		private string GetChipFromSpecs(IEnumerable<YandexMarketSpecRecord> specs)
		{			
			if (specs.FirstOrDefault(x => x.Value.ToLower().Contains("geforce")) != null) 
				return "GeForce";

			if (specs.FirstOrDefault(x => x.Value.ToLower().Contains("radeon")) != null)
				return "Radeon";

			return "";
		}

		private string GetDisplaySize(IEnumerable<YandexMarketSpecRecord> specs)
		{
			const string pattern = "([0-9]+(?:\\.[0-9]+)?)\"";

			var spec = specs.FirstOrDefault(x => x.Key == "�������");

			if (spec != null)
			{
				var result = this.GetValByRegExp(spec.Value, pattern);
				return result;
			}

			return "";
		}

		private bool IsBluetooth(IEnumerable<YandexMarketSpecRecord> specs)
		{			
			return specs.FirstOrDefault(x => x.Value.ToLower().Contains("bluetooth")) != null;
		}

		private bool IsUsbAcustika(IEnumerable<YandexMarketSpecRecord> specs, string fullDescr)
		{			
			return (specs.FirstOrDefault(x => x.Value.ToLower().Contains("usb")) != null || fullDescr.ToLower().Contains("usb"))
				&& specs.FirstOrDefault(x => x.Key.ToLower().Contains("������ ��������")) != null;
		}

		private bool IsAndroid(IEnumerable<YandexMarketSpecRecord> specs)
		{
			return specs.FirstOrDefault(x => x.Value.ToLower().Contains("android")) != null;
		}

		private bool IsExternal(string name)
		{
			return name.ToLower().Contains("external");
		}

		private bool IsFlash(IEnumerable<YandexMarketSpecRecord> specs)
		{
			return specs.FirstOrDefault(x => x.Value.ToLower().Contains("�������")) != null;
		}

		private bool IsRadio(IEnumerable<YandexMarketSpecRecord> specs)
		{
			return specs.FirstOrDefault(x => x.Value.ToLower().Contains("�����") && !x.Value.ToLower().Contains("����������")) != null; 
		}

		private bool IsDisplaySensor(IEnumerable<YandexMarketSpecRecord> specs)
		{
			return specs.FirstOrDefault(x => x.Key == "�������" && x.Value.ToLower().Contains("���������")) != null;
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

		private void ReplaceByValue(IEnumerable<YandexMarketSpecRecord> specs, IEnumerable<string> toReplaceValues, string replaceValue, string key = "" )
		{
			if (key != "")
			{
				if (specs.Any(x => x.Key == key))
				{
					var spec = specs.Single(x => x.Key == key);

					foreach (var curToRepl in toReplaceValues)
					{
						spec.Value = spec.Value.Replace(curToRepl, replaceValue);
					}
				}
			}
			else
			{
				foreach (var spec in specs)									
				{					
					foreach (var curToRepl in toReplaceValues)
					{
						spec.Value = spec.Value.Replace(curToRepl, replaceValue);
					}
				}
			}
		}
	}
}
