using System.Collections.Generic;

namespace Nop.Services.SiteParsers
{
	public static class YandexMarketHelpers
	{
		public static List<string> GetAllowFilteringForProductSelector()
		{
			return new List<string>()
				{					
					"Производитель",					
					"Bluetooth",
					"Количество SIM-карт",
					"Размер экрана, дюймы",
					"Количество мегапикселей камеры",
					"Тип корпуса",
					"Сенсорный экран",
					"FM-радио",
					"Android",
					"Количество клавиш",
					"Подключение",
					"Тип подключения",
					"Разрешение матрицы",
					"Встроенный микрофон",
					"Формат акустики",
					"Общая выходная мощность колонок, RMS (Вт)",
					"Материал корпуса",
					"USB-подключение",

				};
		}
	}
}