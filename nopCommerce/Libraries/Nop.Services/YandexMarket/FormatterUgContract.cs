namespace Nop.Services.YandexMarket
{
	using System.Collections.Generic;
	using System.Linq;

	using Nop.Core.Domain.YandexMarket;

	public class FormatterUgContract : FormatterBase
	{		
		public override YandexMarketProductRecord Format(YandexMarketProductRecord product)
		{
			const string toErase1 = "Если вы заметили некорректные данные в описании товара, выделите ошибку и нажмите";
			const string toErase2 = "Ctrl+Enter";
			const string toErase3 = ", чтобы сообщить нам об этом.";
			product.FullDescription = product.FullDescription.Replace(toErase1, "").Replace(toErase2, "").Replace(toErase3, "");

			product.Articul = product.Articul.Replace("Код: ", "");

			product.Name = product.Name.Replace("МОБИЛЬНЫЙ ТЕЛЕФОН ", "");

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

			var manufactureName = this.GetManufactureFromName(product.Name);
			if (manufactureName != string.Empty)
				product.Specifications.Add(new YandexMarketSpecRecord("Производитель", manufactureName));

			var megapixels = this.GetMegapixelsFromSpecs(product.Specifications);
			if (megapixels != string.Empty)
				product.Specifications.Add(new YandexMarketSpecRecord("Количество мегапикселей камеры", megapixels));

			var displaySize = this.GetDisplaySize(product.Specifications);
			if (displaySize != string.Empty)
				product.Specifications.Add(new YandexMarketSpecRecord("Размер экрана, дюймы", displaySize));
			
			if (this.IsBluetooth(product.Specifications))
				product.Specifications.Add(new YandexMarketSpecRecord("Bluetooth", "Есть"));

			if (this.IsAndroid(product.Specifications))
				product.Specifications.Add(new YandexMarketSpecRecord("Android", "Да"));

			if (this.IsFlash(product.Specifications))
				product.Specifications.Add(new YandexMarketSpecRecord("Вспышка", "Есть"));

			if (this.IsRadio(product.Specifications))
				product.Specifications.Add(new YandexMarketSpecRecord("FM-радио", "Есть"));

			if (this.IsDisplaySensor(product.Specifications))
				product.Specifications.Add(new YandexMarketSpecRecord("Сенсорный экран", "Да"));

			var simAmount = this.GetSimAmount(product);
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

		private string GetMegapixelsFromSpecs(IEnumerable<YandexMarketSpecRecord> specs)
		{
			const string pattern = @"([0-9]+(?:\.[0-9]+)?) [Мм]";

			var spec = specs.SingleOrDefault(x => x.Key.ToLower().Contains("камера"));

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

		private bool IsAndroid(IEnumerable<YandexMarketSpecRecord> specs)
		{
			return specs.SingleOrDefault(x => x.Value.ToLower().Contains("android")) != null;
		}

		private bool IsFlash(IEnumerable<YandexMarketSpecRecord> specs)
		{
			return specs.SingleOrDefault(x => x.Value.ToLower().Contains("вспышка")) != null;
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
