using System.Collections.Generic;

namespace Nop.Services.SiteParsers
{
	public static class YandexMarketHelpers
	{
		public static List<string> GetAllowFilteringForProductSelector()
		{
			return new List<string>()
				{
					"��� �������",					
					"��������� ����",
					"�������������",					
					"����� ��� ��������� ��������",
					"�������������",
					"������������� ������������������",
					"������� �������",
					"�������� �������",					
				};
		}
	}
}