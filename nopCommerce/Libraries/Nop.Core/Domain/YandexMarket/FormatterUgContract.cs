namespace Nop.Core.Domain.YandexMarket
{
	using System;
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

			ReplaceByValue(product.Specifications, new List<string> { "�� ����� USB", "�� ���� USB", "�� USB 2.0", "�� USB" }, "�� USB �����", "�������");
			ReplaceByValue(product.Specifications, new List<string> { " ����� �����" }, " �����", "�������");
			ReplaceByValue(product.Specifications, new List<string> { "100-240 �" }, "�� �����������", "�������");


			   
    
    

			ReplaceByValue(product.Specifications, new List<string> { "��������/���.", "��/���", "��������/���."}, "rpm", "�������� �������� ��������");
			ReplaceByValue(product.Specifications, new List<string> { " �����", "\"" }, "", "������");
			ReplaceByValue(product.Specifications, new List<string> { " �����", "\"" }, "", "������");

			ReplaceByValue(product.Specifications, new List<string> { "MB", "��", "M�", "Mb", "��", "��", "�B", "MB", "MB", "�B",  }, "MB", "�����");

			   

			ReplaceByValue(product.Specifications, new List<string> { "-", " " }, "", "���������");
			ReplaceByValue(product.Specifications, new List<string> { "3" }, "III", "���������");
			ReplaceByValue(product.Specifications, new List<string> { "USBIII.0/2.0", "USB2.0/III.0" }, "USB2.0/3.0", "���������");

			ReplaceByValue(product.Specifications, new List<string> { "m-ATX", "mATX", "MicroATX", "Micro-ATX", "microATX", "micro ATX",  }, "micro-ATX", "������");
			ReplaceByValue(product.Specifications, new List<string> { "Mini-ITX", "miniITX", "MicroATX", "Micro-ATX",   }, "mini-ITX", "������");

			ReplaceByValue(product.Specifications, new List<string> { "Minitower" }, "MiniTower", "��� ������������");
			ReplaceByValue(product.Specifications, new List<string> { "Miditower", "Mediumtower", "Midi Tower" }, "MidiTower", "��� ������������");
			ReplaceByValue(product.Specifications, new List<string> { "�������������", "��������������", "��������������", "���������������" }, "��������������", "���������� ����� �������");
			ReplaceByValue(product.Specifications, new List<string> { "3D LED" }, "3D LED-���������", "���");
			ReplaceByValue(product.Specifications, new List<string> { "3D ���������� ���������", "���������� 3D ������" }, "3D ���������� ������", "���");
			ReplaceByValue(product.Specifications, new List<string> { "LED ���������", "LED" }, "LED-���������", "���");
			ReplaceByValue(product.Specifications, new List<string> { "LED" }, "LED-���������", "���");
			ReplaceByValue(product.Specifications, new List<string> { "�������������������", "��-���������" }, "��", "���");
			ReplaceByValue(product.Specifications, new List<string> { "-���������-���������-���������" }, "-���������", "���");
			ReplaceByValue(product.Specifications, new List<string> { "-���������-���������" }, "-���������", "���");
			ReplaceByValue(product.Specifications, new List<string> { "\"" }, "", "���������");
			ReplaceByValue(product.Specifications, new List<string> { "��������", "Full HD", "(1080p)", "HD Ready", "(720p)", " ", "," }, "", "����������");
			ReplaceByValue(product.Specifications, new List<string> { "x" }, "�", "����������");

			ReplaceByValue(product.Specifications, new List<string> { "������������" }, "������������", "��� �����������");
			ReplaceByValue(product.Specifications, new List<string> { "� ��������", "� ��������", "���������", "C ��������", "� �������", "���������" }, "���������", "��� �����������");

			ReplaceByValue(product.Specifications, new List<string> { "��������", "��������-��������" }, "��������", "��� ���������");
			ReplaceByValue(product.Specifications, new List<string> { "����������" }, "����������", "��� ���������");
			ReplaceByValue(product.Specifications, new List<string> { "�� �����", "���� (�� �������)", "���� (��������������� � ������ ����� ���������)" }, "����", "��������� ���������");

			ReplaceByValue(product.Specifications, new List<string> { " DDR3", " RAM", " SDRAM ����������� ������" }, "", "����������� ������");
			ReplaceByValue(product.Specifications, new List<string> { "1024 MB" }, "1 GB");
			ReplaceByValue(product.Specifications, new List<string> { "2048 MB" }, "2 GB");
			ReplaceByValue(product.Specifications, new List<string> { "3072 MB" }, "3 GB");
			ReplaceByValue(product.Specifications, new List<string> { "2048 GB" }, "1 TB");
			ReplaceByValue(product.Specifications, new List<string> { "HDD " }, "", "������� ����");


			ReplaceByValue(product.Specifications, new List<string> { "\"", " ������", ".0", " " }, "", "������� �������");

			ReplaceByValue(product.Specifications, new List<string> { "nVidia" }, "NVIDIA ", "������������");

			ReplaceByValue(product.Specifications, new List<string> { "Boot-up Linux", "MeeGo (Linux)", "Linux (MeeGo)", "Linux", "Linux" }, "Linux", "������������ �������");
			ReplaceByValue(product.Specifications, new List<string> { " 64-bit", " 64 bit", " (64-bit)", " (64 bit)", " (64bit)", " 64bit", " 32-bit" }, "", "������������ �������");
			ReplaceByValue(product.Specifications, new List<string> { "Dos" }, "DOS", "������������ �������");
			ReplaceByValue(product.Specifications, new List<string> { "Free DOS" }, "FreeDOS", "������������ �������");

    
			

			var size = this.GetSize(product.Name);
			if (size != string.Empty && product.Specifications.All(x => x.Key != "�������"))
				product.Specifications.Add(new YandexMarketSpecRecord("�������", size));

			ReplaceByValue(product.Specifications, new List<string> { "TB", "��", "T�" }, "��", "�������");
			ReplaceByValue(product.Specifications, new List<string> { "��", "��" }, "GB", "�������");
			ReplaceByValue(product.Specifications, new List<string> { ".0" }, "", "�������");
			ReplaceByValue(product.Specifications, new List<string> { "00" }, "", "�������");
			

			var manufactureName = this.GetManufactureFromName(product.Name);
			if (manufactureName != string.Empty && product.Specifications.All(x => x.Key != "�������������"))
				product.Specifications.Add(new YandexMarketSpecRecord("�������������", manufactureName));

			var megapixels = this.GetMegapixelsFromSpecs(product);
			if (megapixels != string.Empty && product.Specifications.All(x => x.Key != "���������� ������������ ������"))
				product.Specifications.Add(new YandexMarketSpecRecord("���������� ������������ ������", megapixels));

			if (product.Specifications.Any(x => x.Key == "���������� �������"))
			{
				megapixels = this.GetCameraMegapixelsFromSpecs(product.Specifications);
				if (megapixels != string.Empty)
					product.Specifications.Single(x => x.Key == "���������� �������").Value = megapixels;				
			}

			//var conObj = product.Specifications.FirstOrDefault(x => x.Key != "��������� �� �����");
			//if (conObj != null) product.Specifications.Remove(conObj);
			//conObj = product.Specifications.FirstOrDefault(x => x.Key != "��������� �� �����");
			//if (conObj != null) product.Specifications.Remove(conObj);

			if (product.Specifications.All(x => x.Key != "��������� �� �����"))
			{
				var connector = this.GetConnectorPapa(product.FullDescription);
				if (connector != string.Empty) product.Specifications.Add(new YandexMarketSpecRecord("��������� �� �����", connector));
			}

			ReplaceByValue(product.Specifications, new List<string> { "HDMI-A �����", "HDMI A \"����\"" }, "HDMI-A \"����\"", "��������� �� �����");
			ReplaceByValue(product.Specifications, new List<string> { "3.0" }, "", "��������� �� �����");
			ReplaceByValue(product.Specifications, new List<string> { "3x" }, "3 x", "��������� �� �����");
			ReplaceByValue(product.Specifications, new List<string> { "2x" }, "2 x", "��������� �� �����");
			ReplaceByValue(product.Specifications, new List<string> { "�", "�" }, "\"", "��������� �� �����");
			ReplaceByValue(product.Specifications, new List<string> { "�" }, "\"", "��������� �� �����");
			ReplaceByValue(product.Specifications, new List<string> { "3.5 �� \"����\"" }, "3.5 �� \"����\"", "��������� �� �����");

			//var conObj2 = product.Specifications.FirstOrDefault(x => x.Key != "��������� �� ������");
			//if (conObj2 != null) product.Specifications.Remove(conObj2);
			//conObj2 = product.Specifications.FirstOrDefault(x => x.Key != "��������� �� ������");
			//if (conObj2 != null) product.Specifications.Remove(conObj2);

			if (product.Specifications.All(x => x.Key != "��������� �� ������"))
			{
				var connector = this.GetConnectorMama(product.FullDescription);
				if (connector != string.Empty) product.Specifications.Add(new YandexMarketSpecRecord("��������� �� ������", connector));				
			}

			ReplaceByValue(product.Specifications, new List<string> { "HDMI-A �����", "HDMI A \"����\"", "HDMI A \"����\"" }, "HDMI-A \"����\"", "��������� �� ������");
			ReplaceByValue(product.Specifications, new List<string> { "3.0" }, "", "��������� �� ������");
			ReplaceByValue(product.Specifications, new List<string> { "3x" }, "3 x", "��������� �� ������");
			ReplaceByValue(product.Specifications, new List<string> { "2x" }, "2 x", "��������� �� ������");
			ReplaceByValue(product.Specifications, new List<string> { "3.5 �� \"����\"" }, "3.5 �� \"����\"", "��������� �� ������");
			ReplaceByValue(product.Specifications, new List<string> { "USB -B" }, "USB-B", "��������� �� ������");
			ReplaceByValue(product.Specifications, new List<string> { "�", "�" }, "\"", "��������� �� ������");
			ReplaceByValue(product.Specifications, new List<string> { "�" }, "\"", "��������� �� ������");
			ReplaceByValue(product.Specifications, new List<string> { "����" }, "����", "��������� �� ������");

			ReplaceByValue(product.Specifications, new List<string> { "�����������" }, "�����������", "��� ����������");
			ReplaceByValue(product.Specifications, new List<string> { "������������" }, "������������", "��� ����������");

			
			var tiporazmer = this.GetTiporazmer(product.FullDescription);
			if (tiporazmer != string.Empty && product.Specifications.All(x => x.Key != "����������"))
				product.Specifications.Add(new YandexMarketSpecRecord("����������", tiporazmer));


			var displaySize = this.GetDisplaySize(product.Specifications);
			if (displaySize != string.Empty && product.Specifications.All(x => x.Key != "������ ������, �����"))
				product.Specifications.Add(new YandexMarketSpecRecord("������ ������, �����", displaySize));
			
			if (this.IsBluetooth(product.Specifications) && product.Specifications.All(x => x.Key != "Bluetooth"))
				product.Specifications.Add(new YandexMarketSpecRecord("Bluetooth", "����"));

			if (this.IsWiFi(product.Specifications) && product.Specifications.All(x => x.Key != "Wi-Fi"))
				product.Specifications.Add(new YandexMarketSpecRecord("Wi-Fi", "����"));

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

			if (this.IsDisplaySensor(product) && product.Specifications.All(x => x.Key != "��������� �����"))
				product.Specifications.Add(new YandexMarketSpecRecord("��������� �����", "��"));

			if (this.IsOpticalZoom(product) && product.Specifications.All(x => x.Key != "���������� ���"))
				product.Specifications.Add(new YandexMarketSpecRecord("���������� ���", "��"));

			var simAmount = this.GetSimAmount(product);
			if (simAmount > 1 && product.Specifications.All(x => x.Key != "���������� SIM-����"))
				product.Specifications.Add(new YandexMarketSpecRecord("���������� SIM-����", simAmount.ToString()));

			var buttons = this.GetMouseButtonsFromSpecs(product.Specifications);
			if (buttons != string.Empty && product.Specifications.All(x => x.Key != "���������� ������"))
				product.Specifications.Add(new YandexMarketSpecRecord("���������� ������", buttons));

			var chip = this.GetChipFromSpecs(product.Specifications);
			if (chip != string.Empty && product.Specifications.All(x => x.Key != "��� ����"))
				product.Specifications.Add(new YandexMarketSpecRecord("��� ����", chip));

			ProcessBumagaSize(product);
			ProcessBumagaCount(product);
			ProcessHolodilnik(product);
			ProcessPlitas(product);
			ProcessHeaters(product);

			return product;
		}


		private string GetManufactureFromName(string name)
		{
			var manufactures = new List<string>() { 
				"Acer",
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
				"Bandridge", 
				"Valueline", 
				"Sony", 
				"Acme Made", 
				"Case Logic", 
				"Lowepro", 
				"Olympus", 
				"X-Digital", 
				"Casio", 
				"Nikon", 
				"THULE", 
				"Continent", 
				"SUMDEX", 
				"BOROFONE", 
				"DiGi", 
				"HOCO", 
				"Vogels", 
				"KOSS", 
				"PLEOMAX", 
				"SENNHEISER", 
				"Sony Ericsson", 
				"JVC", 
				"Ergo", 
				"Panasonic", 
				"ViewSonic",
				"Yarvik",
				"AOC",
				"Lenovo", 
				"Iriver",
				"Transcend",
				"Kodak",
				"teXet",
				"GOODRAM",
				"Hewlett Packard",
				"PQI",
				"Sandisk",
				"TP-Link",
				"TRENDnet",
				"Duracell",
				"UFO",
				"����� ����������",

				"Canon",
				"Casio",
				"Fujifilm",
				"Nikon",
				"Olympus",
				"Panasonic",
				"Pentax",
				"Sony",
				"BBK",
				"Sharp",

				"ColorWay",
				"Lomond",
				"Ballet",
				"Kodak",
				"Tetenal",
				"Konica Minolta",
				"Konica",				
				"Baltea",
				"Hi-Ti",
				"ARDO",
				"BEKO",
				"Candy",
				"Electrolux",
				"Hotpoint",
				"Indesit",
				"Zanussi",
				"Vestel",
				"Vestfrost",
				"Ariston",
				"Hitachi",
				"Liebherr",
				"Binatone",
				"ATMOR",
				"Atron",
				"Garanterm",
				"Gorenje",
				"Thermex",
				"Beurer",
				"Maxwell",
				"Vitek",
				"Ballet",
				"Flamingo",
				"Rotex",
				"Sharp",
				"Sharp",
				"Sharp",
				"Sharp",
				


				 "ARDO", "Ariston", "ATMOR", "Atron", "Ballet", "BEKO", "Beurer", "Binatone", "BOSCH", "BRAUN", "Candy", "Daewoo", "Electrolux", "Ergo", "Flamingo", "Garanterm", "Gorenje", "Hitachi", "Hotpoint", "Indesit", "KRUPS", "LG", "Liberton", "Liebherr", "MAXELL", "Maxwell", "Moulinex", "Panasonic", "Philips", "Philips DLO", "Rondell", "Rotex", "Rowenta", "Samsung", "Tefal", "Thermex", "Thomas", "UFO", "Vestel", "Vestfrost", "Vitek", "Zanussi", "Zelmer",



				};

			name = name.ToUpper();

			foreach (var manufacture in manufactures.Where(manufacture => name.Contains(manufacture.ToUpper()))) 
				return manufacture;

			return "";
		}

		private string GetSize(string name)
		{
			
				string pattern = @"([0-9]+) G";

				var size = this.GetValByRegExp(name, pattern);


				return size.Replace("00" , "");
		}

		private string GetMegapixelsFromSpecs(YandexMarketProductRecord product)
		{
			if (product.FullDescription.Contains("-�������������� ������"))
			{
				string pattern = @"([0-9]+(?:\.[0-9]+)?)-�������������� �����";

				var megapixels = this.GetValByRegExp(product.FullDescription, pattern);
				megapixels = megapixels.Replace(".0", "").Replace(".00", "");

				return megapixels;
			}

			var spec = product.Specifications.FirstOrDefault(x => 
					x.Key.ToLower().Contains("������") 
				||	x.Key.ToLower().Contains("�������������� ������� � �����������")
				|| x.Key.ToLower().Contains("������ �����������"));

			if (spec != null)
			{
				string pattern = @"([0-9]+(?:\.[0-9]+)?) [��Mm]";

				if (spec.Key.ToLower().Contains("������ �����������"))
				{
					pattern += "��";
				}
							
				var megapixels = this.GetValByRegExp(spec.Value, pattern);

				double dMegapixels;
				if (double.TryParse(megapixels, out dMegapixels))
				{
					if (dMegapixels > 5) 
						megapixels = ((int)dMegapixels).ToString();
				}

				megapixels = megapixels.Replace(".0", "").Replace(".00", "");
				
				return megapixels;
			}
			


			return "";
		}

		private string GetCameraMegapixelsFromSpecs(IEnumerable<YandexMarketSpecRecord> specs)
		{
			var spec = specs.FirstOrDefault(x => 
				x.Key.ToLower().Contains("���������� �������"));

			if (spec != null)
			{
				string pattern = @"([0-9]+(?:\.[0-9]+)?)";
				
				var megapixels = this.GetValByRegExp(spec.Value, pattern);
				megapixels = megapixels.Replace(".0", "").Replace(".00", "");

				if (double.Parse(megapixels) > 40)
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
			const string pattern = "([0-9]+(?:\\.[0-9]+)?)";

			var spec = specs.FirstOrDefault(x => x.Key == "�������");

			if (spec != null)
			{
				var result = this.GetValByRegExp(spec.Value, pattern).Replace(".0", "").Replace(".00", ""); 
				return result;
			}

			return "";
		}

		private bool IsBluetooth(IEnumerable<YandexMarketSpecRecord> specs)
		{			
			return specs.FirstOrDefault(x => x.Value.ToLower().Contains("bluetooth")) != null;
		}

		private bool IsWiFi(IEnumerable<YandexMarketSpecRecord> specs)
		{
			return specs.FirstOrDefault(x => x.Value.ToLower().Contains("wi-fi")) != null;
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

		private bool IsDisplaySensor(YandexMarketProductRecord product)
		{			
			var result = product.Specifications.FirstOrDefault(x => x.Key == "�������" && x.Value.ToLower().Contains("���������")) != null;

			if (!result) 
				result = product.FullDescription.Contains("��������");

			return result;
		}

		private bool IsOpticalZoom(YandexMarketProductRecord product)
		{
			var result = product.Specifications.FirstOrDefault(x => 
				   x.Value.ToLower().Contains("���������� ���")
				|| x.Value.ToLower().Contains("����������� ����")
				|| x.Value.ToLower().Contains("���������� �����")
				) != null;

			if (!result)
			{
				result = product.FullDescription.ToLower().Contains("���������� ���")
					|| product.FullDescription.ToLower().Contains("����������� ����")
				|| product.FullDescription.ToLower().Contains("���������� �����");
			}
			return result;
		}

		private int GetSimAmount(YandexMarketProductRecord product)
		{
			if (product.YandexMarketCategoryRecordId != 7 || product.YandexMarketCategoryRecordId != 8) // smarts and mobils only
				return 0;

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

		private string GetConnectorPapa(string description)
		{
			string pattern = "((?<=��������� �� ����� -)(?:.*)(?=</li>\\r\\n    <li>��������� �� ������))";

				var connector = this.GetValByRegExp(description, pattern);

				return connector.Replace("&nbsp;", "").Trim();	
		}

		private string GetConnectorMama(string description)
		{
			string pattern = "((?<=��������� �� ������ -)(?:.*)(?=</li>\\r\\n\\s?))";

			var connector = this.GetValByRegExp(description, pattern);

			return connector.Replace("&nbsp;", "").Trim();
		}

		private string GetTiporazmer(string description)
		{
			string pattern = "((?<=����������)(?:.*)(?=<))";

			var result = this.GetValByRegExp(description, pattern);

			if (result.Any())
			{
				result = result.ToUpper();

				result =
					result
						  .Replace("<span lang=\"EN-US\" style=\"mso-ansi-language EN-US;\">".ToUpper(), "")
						  .Replace("<span lang=\"EN-US\" style=\"mso-ansi-language: EN-US;\">".ToUpper(), "")
						  .Replace("<spanlang=\"en-us\"style=\"mso-ansi-languageen-us;\">".ToUpper(), "")
						  .Replace("</spanlang=\"en-us\"style=\"mso-ansi-languageen-us;\">".ToUpper(), "")
						  .Replace("</span>".ToUpper(), "")
						  .Replace("</font>".ToUpper(), "");

				result =
					result					  
						  .Replace(":", "")
						  .Replace("&nbsp;".ToUpper(), "")
						  .Replace(" ", "")
						  .Replace("�", "A")
						  .Replace("�", "C")
						  .Replace("�AC��A�", "�AC����")
						  .Replace("��R03,R6,R14,R20,6F22���������C�������AC����C�����A��������A����.������C���A�A�������C�����A�����-��2���.", "");

				
			}

			return result;

		}

		private void ProcessBumagaSize(YandexMarketProductRecord product)
		{
			if (product.YandexMarketCategoryRecordId != 69)
				return;
			
			string tiporazmer = "";


			string pattern = "([0-9]+\\s?[Xx��]\\s?[0-9]+)([\"']?)";

			var values = this.GetValByRegExp(product.Name, pattern, 2); 
			if (values != null)
			{
				var x = "x";
				tiporazmer = values[0].Replace("X", x).Replace("x", x).Replace("�", x).Replace("�", x);
				var isInches = values[1].Length > 0;

				if (isInches)
				{
					if (tiporazmer == "4x6") 
						tiporazmer = "10x15";
				}
			}
			else
			{
				if (product.Name.Contains(" A4") || product.Name.Contains(" �4")) 
					tiporazmer = "A4";
				else if (product.Name.Contains(" A3") || product.Name.Contains(" �3"))
					tiporazmer = "A3";
			}


			if (tiporazmer != string.Empty && product.Specifications.All(x => x.Key != "������"))
				product.Specifications.Add(new YandexMarketSpecRecord("������", tiporazmer));

			if (product.Specifications.All(x => x.Key != "���"))
			{
				if (product.Name.ToLower().Contains("���������"))
					product.Specifications.Add(new YandexMarketSpecRecord("���", "���������"));

				if (product.Name.ToLower().Contains("�������"))					
						product.Specifications.Add(new YandexMarketSpecRecord("���", "�������"));

				//if (IsProductContainString(product, "�����"))					
				//		product.Specifications.Add(new YandexMarketSpecRecord("���", "�����"));

				//if (IsProductContainString(product, "�������"))					
				//		product.Specifications.Add(new YandexMarketSpecRecord("���", "�������"));
			}

			if (product.Name.ToLower().Contains("PHOTO".ToLower()) || product.Name.ToLower().Contains("���������".ToLower()))
				if (product.Specifications.All(x => x.Key != "����������"))
				product.Specifications.Add(new YandexMarketSpecRecord("����������", "��"));

			if (product.Name.ToLower().Contains("������"))
				if (product.Specifications.All(x => x.Key != "���������"))
				product.Specifications.Add(new YandexMarketSpecRecord("���������", "��"));

			if (product.Name.ToLower().Contains("art"))
				if (product.Specifications.All(x => x.Key != "Art paper"))
				product.Specifications.Add(new YandexMarketSpecRecord("Art paper", "��"));

			if (product.Name.ToLower().Contains("�������".ToLower()))
				if (product.Specifications.All(x => x.Key != "�������������"))
				product.Specifications.Add(new YandexMarketSpecRecord("�������������", "��"));

			if (product.Name.ToLower().Contains("�������������".ToLower()))
				if (product.Specifications.All(x => x.Key != "�������������"))
				product.Specifications.Add(new YandexMarketSpecRecord("�������������", "��"));
			
		}

		private void ProcessBumagaCount(YandexMarketProductRecord product)
		{			
			if(product.YandexMarketCategoryRecordId != 69)
				return;

			string pattern = "([0-9]+)\\s?[��][��]";

			var value = this.GetValByRegExp(product.Name, pattern);

			if (value != string.Empty && product.Specifications.All(x => x.Key != "����������, ��."))
				product.Specifications.Add(new YandexMarketSpecRecord("����������, ��.", value));


			pattern = "([0-9]+)\\s?[��]/[��]";

			value = this.GetValByRegExp(product.Name, pattern);

			if (value != string.Empty && product.Specifications.All(x => x.Key != "���������, �/�"))
				product.Specifications.Add(new YandexMarketSpecRecord("���������, �/�", value));

		}

		private void AddNewSpec(YandexMarketProductRecord product, string specNewName, string specValue)
		{
			if (specValue != string.Empty && specValue != "0" && product.Specifications.All(x => x.Key != specNewName))
				product.Specifications.Add(new YandexMarketSpecRecord(specNewName, specValue));
		}

		private void ProcessSpec(string textToParse, YandexMarketProductRecord product, string pattern, string specNewName)
		{
			var value = this.GetValByRegExp(textToParse, pattern);
			AddNewSpec(product, specNewName, value);
		}

		private void ProcessHolodilnik(YandexMarketProductRecord product)
		{
			if (product.YandexMarketCategoryRecordId != 76)
				return;

			ProcessHolodilnikObems(product);
			ProcessHolodilnikRazmorozkas(product);
		}

		private void ProcessHolodilnikObems(YandexMarketProductRecord product)
		{			
			var specToParse = product.Specifications.FirstOrDefault(x => x.Key == "�����");

			if (specToParse == null)
				return;

			ProcessSpec(specToParse.Value, product, "�����: ([0-9]+) �", "����� �����, �.");
			ProcessSpec(specToParse.Value, product, "����� ����������� ������: ([0-9]+) �", "����� ����������� ������, �.");
			ProcessSpec(specToParse.Value, product, "����� ����������� ������: ([0-9]+) �", "����� ����������� ������, �.");						
		}
		
		private void ProcessHolodilnikRazmorozkas(YandexMarketProductRecord product)
		{
			var specToParse = product.Specifications.FirstOrDefault(x => x.Key == "��������������");

			if (specToParse == null)
				return;

			var val = specToParse.Value.Replace("\r", "").Replace("\n", "");
			val = val.Replace("�����������", "�����������").Replace(" ��������������", "").Replace("���", "������").Replace("��������� �������", "���������");

			ProcessSpec(val, product, "����������� ������:?</p><p>([\\w\\s]+)", "�������������� ����������� ������");
			ProcessSpec(val, product, "����������� ������:?</p><p>([\\w\\s]+)", "�������������� ����������� ������");			
		}


		private void ProcessPlitas(YandexMarketProductRecord product)
		{
			if (product.YandexMarketCategoryRecordId != 77)
				return;

			ReplaceByValue(product.Specifications, new List<string> { "������������� �����" }, "�����", "������� �����������");
			ReplaceByValue(product.Specifications, new List<string> { "�������������" }, "�����", "������� �����������");
			ReplaceByValue(product.Specifications, new List<string> { "� �����������", "&amp;#8211; �������������" }, "- �������������", "����� ��������");
			ReplaceByValue(product.Specifications, new List<string> { "�������" }, "�������", "����� ��������");
			
			ProcessPlitaGril(product);
			ProcessPlitaPodzig(product);						
		}

		private void ProcessPlitaGril(YandexMarketProductRecord product)
		{
			var specToParse = product.Specifications.FirstOrDefault(x => x.Key == "�����");

			if (specToParse == null)
				return;

			if (!specToParse.Value.ToLower().Contains("���")) 
				AddNewSpec(product, "������� �����", "��");
		}

		private void ProcessPlitaPodzig(YandexMarketProductRecord product)
		{
			var specToParse = product.Specifications.FirstOrDefault(x => x.Key == "�������������");

			if (specToParse == null)
				return;

			if (!specToParse.Value.ToLower().Contains("���"))
				AddNewSpec(product, "������� ��������������", "��");
		}

		private void ProcessHeaters(YandexMarketProductRecord product)
		{
			if (product.YandexMarketCategoryRecordId != 84)
				return;

			//ReplaceByValue(product.Specifications, new List<string> { "�������" }, "�������", "����� ��������");

			ProcessHeaterType(product);	
		}
		
		private void ProcessHeaterType(YandexMarketProductRecord product)
		{
			var specToParse = product.FullDescription.ToLower();

			if (specToParse.Contains("���������������"))
				AddNewSpec(product, "���", "T��������������");
			else if (specToParse.Contains("������������"))
				AddNewSpec(product, "���", "������������");
			else if (specToParse.Contains("����������"))
				AddNewSpec(product, "���", "������������");			
		}
	}
}
