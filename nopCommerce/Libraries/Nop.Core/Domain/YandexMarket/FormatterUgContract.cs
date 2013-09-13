namespace Nop.Core.Domain.YandexMarket
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;

	public class FormatterUgContract : FormatterBase
	{		
		public override YandexMarketProductRecord Format(YandexMarketProductRecord product)
		{			
			const string toErase1 = "Если вы заметили некорректные данные в описании товара, выделите ошибку и нажмите";
			const string toErase2 = "Ctrl+Enter";
			const string toErase3 = ", чтобы сообщить нам об этом.";
			product.FullDescription = product.FullDescription.Replace(toErase1, "").Replace(toErase2, "").Replace(toErase3, "");

			product.Articul = product.Articul.Replace("Код: ", "");

			//product.Name = product.Name.Replace("МОБИЛЬНЫЙ ТЕЛЕФОН ", "").Replace("СМАРТФОН ", "");

			foreach (var curSpec in product.Specifications)
			{								
				var s2 = "<";

				int i = 0;  // Числовая переменная, контролирующая итерации цикла
				int x = -1; // Так как метод IndexOf() возвращает "-1" если первое вхождение подстроки не найдено, то приходится использовать вспомагательную, вместо і, что б начать цикл
				int count = -1; // Записываем количество вхождений (итераций цикла)
				while (i != -1)
				{
					i = curSpec.Value.IndexOf(s2, x + 1); // получаем индекс первого вхождения  х+1 говорит, что начинать нужно с 0-го индекса, тоесть с буквы "П"
					x = i; // соответственно присваиваем номер индекса первого значения, что б потом (х+1) начать со следующего
					count++;  // Увеличиваем на единицу наше количество
				}

				if (count <= 2)
				{
					curSpec.Value = curSpec.Value.Replace("<p>", "").Replace("</p>", "").Trim();
				}
			}


			ReplaceByValue(product.Specifications, new List<string> { "MB", "МВ", "MВ", "Mb" }, "MB", "Видеопамять");


			ReplaceByValue(product.Specifications, new List<string> { "бит", "-bit", "bit", "Mb", " ", "Bit" }, "", "Разрядность шины памяти");
			ReplaceByValue(product.Specifications, new List<string> { "12 V"}, "12V", "Блок питания");
			ReplaceByValue(product.Specifications, new List<string> { "v." }, "", "Блок питания");
			ReplaceByValue(product.Specifications, new List<string> { "2.2 12V" }, "12V v.2.2", "Блок питания");

			ReplaceByValue(product.Specifications, new List<string> { "оборотов/мин.", "об/мин", "оборотов/мин."}, "rpm", "Скорость вращения шпинделя");
			ReplaceByValue(product.Specifications, new List<string> { " дюйма", "\"" }, "", "Формат");
			ReplaceByValue(product.Specifications, new List<string> { " дюйма", "\"" }, "", "Формат");

			ReplaceByValue(product.Specifications, new List<string> { "MB", "МВ", "MВ", "Mb", "МБ", "Мб", "МB", "MB", "MB", "МB",  }, "MB", "Буфер");

			   
			ReplaceByValue(product.Specifications, new List<string> { "TB", "ТВ", "TВ" }, "ТВ", "Емкость");
			ReplaceByValue(product.Specifications, new List<string> { "Гб", "ГБ" }, "GB", "Емкость");
			ReplaceByValue(product.Specifications, new List<string> { ".0" }, "", "Емкость");
			ReplaceByValue(product.Specifications, new List<string> { "-", " " }, "", "Интерфейс");
			ReplaceByValue(product.Specifications, new List<string> { "3" }, "III", "Интерфейс");

			ReplaceByValue(product.Specifications, new List<string> { "m-ATX", "mATX", "MicroATX", "Micro-ATX", "microATX", "micro ATX",  }, "micro-ATX", "Формат");
			ReplaceByValue(product.Specifications, new List<string> { "Mini-ITX", "miniITX", "MicroATX", "Micro-ATX",   }, "mini-ITX", "Формат");

			ReplaceByValue(product.Specifications, new List<string> { "Minitower" }, "MiniTower", "Тип оборудования");
			ReplaceByValue(product.Specifications, new List<string> { "Miditower", "Mediumtower", "Midi Tower" }, "MidiTower", "Тип оборудования");
			ReplaceByValue(product.Specifications, new List<string> { "Горизонтально", "Горизонтальоне", "Горизонтальнео", "Горизонтальноее" }, "Горизонтальное", "Размещение блока питания");


			var manufactureName = this.GetManufactureFromName(product.Name);
			if (manufactureName != string.Empty && product.Specifications.All(x => x.Key != "Производитель"))
				product.Specifications.Add(new YandexMarketSpecRecord("Производитель", manufactureName));

			var megapixels = this.GetMegapixelsFromSpecs(product.Specifications);
			if (megapixels != string.Empty && product.Specifications.All(x => x.Key != "Количество мегапикселей камеры"))
				product.Specifications.Add(new YandexMarketSpecRecord("Количество мегапикселей камеры", megapixels));

			if (product.Specifications.Any(x => x.Key == "Разрешение матрицы"))
			{
				megapixels = this.GetCameraMegapixelsFromSpecs(product.Specifications);
				if (megapixels != string.Empty)
					product.Specifications.Single(x => x.Key == "Разрешение матрицы").Value = megapixels;				
			}

			var displaySize = this.GetDisplaySize(product.Specifications);
			if (displaySize != string.Empty && product.Specifications.All(x => x.Key != "Размер экрана, дюймы"))
				product.Specifications.Add(new YandexMarketSpecRecord("Размер экрана, дюймы", displaySize));

			if (this.IsBluetooth(product.Specifications) && product.Specifications.All(x => x.Key != "Bluetooth"))
				product.Specifications.Add(new YandexMarketSpecRecord("Bluetooth", "Есть"));

			if (this.IsUsbAcustika(product.Specifications, product.FullDescription) && product.Specifications.All(x => x.Key != "USB-подключение"))
				product.Specifications.Add(new YandexMarketSpecRecord("USB-подключение", "Да"));

			if (this.IsExternal(product.Name) && product.Specifications.All(x => x.Key != "Внешний"))
				product.Specifications.Add(new YandexMarketSpecRecord("Внешний", "Да"));

			if (this.IsAndroid(product.Specifications) && product.Specifications.All(x => x.Key != "Android"))
				product.Specifications.Add(new YandexMarketSpecRecord("Android", "Да"));

			if (this.IsFlash(product.Specifications) && product.Specifications.All(x => x.Key != "Вспышка"))
				product.Specifications.Add(new YandexMarketSpecRecord("Вспышка", "Есть"));

			if (this.IsRadio(product.Specifications) && product.Specifications.All(x => x.Key != "FM-радио"))
				product.Specifications.Add(new YandexMarketSpecRecord("FM-радио", "Есть"));

			if (this.IsDisplaySensor(product.Specifications) && product.Specifications.All(x => x.Key != "Сенсорный экран"))
				product.Specifications.Add(new YandexMarketSpecRecord("Сенсорный экран", "Да"));

			var simAmount = this.GetSimAmount(product);
			if (simAmount > 1 && product.Specifications.All(x => x.Key != "Количество SIM-карт"))
				product.Specifications.Add(new YandexMarketSpecRecord("Количество SIM-карт", simAmount.ToString()));

			var buttons = this.GetMouseButtonsFromSpecs(product.Specifications);
			if (buttons != string.Empty && product.Specifications.All(x => x.Key != "Количество клавиш"))
				product.Specifications.Add(new YandexMarketSpecRecord("Количество клавиш", buttons));

			var chip = this.GetChipFromSpecs(product.Specifications);
			if (chip != string.Empty && product.Specifications.All(x => x.Key != "Тип чипа"))
				product.Specifications.Add(new YandexMarketSpecRecord("Тип чипа", chip));
			
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
			var spec = specs.FirstOrDefault(x => x.Key.ToLower().Contains("камера"));

			if (spec != null)
			{
				string pattern = @"([0-9]+(?:\.[0-9]+)?) [Мм]";
							
				var megapixels = this.GetValByRegExp(spec.Value, pattern);
				megapixels = megapixels.Replace(".0", "").Replace(".00", "");
				
				return megapixels;
			}
			
			return "";
		}

		private string GetCameraMegapixelsFromSpecs(IEnumerable<YandexMarketSpecRecord> specs)
		{
			var spec = specs.FirstOrDefault(x => x.Key.ToLower().Contains("разрешение матрицы"));

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
			const string pattern = @"([0-9]+) клавиш";

			var spec = specs.FirstOrDefault(x => x.Key.ToLower().Contains("дополнительные характеристики"));

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

			var spec = specs.FirstOrDefault(x => x.Key == "Дисплей");

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
				&& specs.FirstOrDefault(x => x.Key.ToLower().Contains("формат акустики")) != null;
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
			return specs.FirstOrDefault(x => x.Value.ToLower().Contains("вспышка")) != null;
		}

		private bool IsRadio(IEnumerable<YandexMarketSpecRecord> specs)
		{
			return specs.FirstOrDefault(x => x.Value.ToLower().Contains("радио") && !x.Value.ToLower().Contains("радиосвязь")) != null; 
		}

		private bool IsDisplaySensor(IEnumerable<YandexMarketSpecRecord> specs)
		{
			return specs.FirstOrDefault(x => x.Key == "Дисплей" && x.Value.ToLower().Contains("сенсорный")) != null;
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
