using System.Collections.Generic;

namespace Nop.Services.SiteParsers
{
	public static class YandexMarketHelpers
	{
		public static List<string> GetAllowFilteringForProductSelector()
		{
			return new List<string>()
				{
					"Тип фильтра",					
					"Отдельный кран",
					"Минерализатор",					
					"Помпа для повышения давления",
					"Производитель",
					"Рекомендуемая производительность",
					"Способы очистки",
					"Ступеней очистки",	
					"Bluetooth",
					"Количество SIM-карт",
					"Размер экрана, дюймы",
					"Количество мегапикселей камеры",
					"Тип корпуса",
					"Сенсорный экран",
					"FM-радио",
				};
		}
	}
}