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
				};
		}
	}
}