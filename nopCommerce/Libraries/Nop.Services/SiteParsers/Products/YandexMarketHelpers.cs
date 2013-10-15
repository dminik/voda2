namespace Nop.Services.SiteParsers.Products
{
	using System.Collections.Generic;

	public static class YandexMarketHelpers
	{
		public class AllowFiltering
		{
			public AllowFiltering()
			{
				ExceptedCategotiesNames = new List<string>();
			}


			public string Name { get; set; }

			public List<string> ExceptedCategotiesNames { get; set; }
		}


		public static List<AllowFiltering> GetAllowFilteringForProductSelector()
		{
			return new List<AllowFiltering>()
				{					
				 new AllowFiltering() { Name = "�������������"},
				 new AllowFiltering() { Name = "Bluetooth"},
				 new AllowFiltering() { Name = "���������� SIM-����"},
				 new AllowFiltering() { Name = "������ ������, �����"},
				 new AllowFiltering() { Name = "���������� ������������ ������"},
				 new AllowFiltering() { Name = "��� �������"},
				 new AllowFiltering() { Name = "��������� �����"},
				 new AllowFiltering() { Name = "FM-�����"},
				 new AllowFiltering() { Name = "Android"},
				 new AllowFiltering() { Name = "���������� ������"},
				 new AllowFiltering() { Name = "�����������"},
				 new AllowFiltering() { Name = "��� �����������"},
				 new AllowFiltering() { Name = "���������� �������"},
				 new AllowFiltering() { Name = "��� �������"},
				 new AllowFiltering() { Name = "���������� ��������"},
				 new AllowFiltering() { Name = "������ ��������"},
				 new AllowFiltering() { Name = "����� �������� �������� �������, RMS (��)"},
				 new AllowFiltering() { Name = "�������� �������"},
				 new AllowFiltering() { Name = "USB-�����������"},
				 new AllowFiltering() { Name = "�������������"},
				 new AllowFiltering() { Name = "��� ������������"},
				 new AllowFiltering() { Name = "���������� �������� �������� �������"},
				 new AllowFiltering() { Name = "�������� ��������"},
				 new AllowFiltering() { Name = "����� ���������� ������"},

				 new AllowFiltering() { Name = "�������� ����� �������"},//����� �������
				 new AllowFiltering() { Name = "���� �������"},
				 new AllowFiltering() { Name = "��������� ������� ���.�����"},
				 new AllowFiltering() { Name = "��������� ������� ���������"},
				 new AllowFiltering() { Name = "������� ��� ����������� HDD/FDD/SATA"},

				 new AllowFiltering() { Name = "��������"},//�������
				 new AllowFiltering() { Name = "��� ������������"},
				 new AllowFiltering() { Name = "�����, �������������� � ����������"},
				 new AllowFiltering() { Name = "���������� ������� ��� HDD"},
				 new AllowFiltering() { Name = "��������� HDD"},
				 new AllowFiltering() { Name = "�����"},
				 new AllowFiltering() { Name = "����������"},
				 new AllowFiltering() { Name = "���������� ����� �������"},
				 new AllowFiltering() { Name = "������� ����� �������"},
				 new AllowFiltering() { Name = "������"},

				 new AllowFiltering() { Name = "���"},
				 new AllowFiltering() { Name = "�����������"},
				 new AllowFiltering() { Name = "����������� ���� ������"},
				 new AllowFiltering() { Name = "��� ������"},
				 new AllowFiltering() { Name = "��� ����"},
					
				 new AllowFiltering() { Name = "���������� ����������� ����������"},
				 new AllowFiltering() { Name = "�������� �������� ��������"},
				 new AllowFiltering() { Name = "�����"},
				 new AllowFiltering() { Name = "�������", ExceptedCategotiesNames = new List<string>(){"���������� �������� ����������"}},
					
				 new AllowFiltering() { Name = "������� ������ ����������"},
				 new AllowFiltering() { Name = "������� ���� CPU"},
				 new AllowFiltering() { Name = "����"},
				 new AllowFiltering() { Name = "���������� ����"},
				 new AllowFiltering() { Name = "������ ����������"},

				 new AllowFiltering() { Name = "�������", ExceptedCategotiesNames = new List<string>(){"���������� �������� ����������"}},

				 new AllowFiltering() { Name = "�������"},

				 new AllowFiltering() { Name = "������ ����������"},
				 new AllowFiltering() { Name = "������ ����������� ����� (�������� ����)"},

				 new AllowFiltering() { Name = "������� ����������������"}, 
				 new AllowFiltering() { Name = "����� ������"},
				 new AllowFiltering() { Name = "���������"},
				 new AllowFiltering() { Name = "����������"},

				 new AllowFiltering() { Name = "��� ���������"}, 
				 new AllowFiltering() { Name = "��������� ���������"},
				 new AllowFiltering() { Name = "���������� ��������"},

				 new AllowFiltering() { Name = "����"}, 
				 new AllowFiltering() { Name = "��������"},
				 new AllowFiltering() { Name = "����-����� ������"},
				 new AllowFiltering() { Name = "����������"},
				 new AllowFiltering() { Name = "�����"},
				 new AllowFiltering() { Name = "����������� ������"},
				 new AllowFiltering() { Name = "�V-��������"},
				 new AllowFiltering() { Name = "���������� ������"},
				 new AllowFiltering() { Name = "������� �������"},
				 new AllowFiltering() { Name = "������������ �������"},
				 new AllowFiltering() { Name = "����������� ������"}, 
				 new AllowFiltering() { Name = "������� ����"},
				 new AllowFiltering() { Name = "���������"},
				 new AllowFiltering() { Name = "������������"},
					
				 new AllowFiltering() { Name = "������� ���������"},
				 new AllowFiltering() { Name = "������������� � �������� ������� ������"},
				 new AllowFiltering() { Name = "������� �������"},

				 new AllowFiltering() { Name = "����� ����� ������� (Soft AP)"},
				 new AllowFiltering() { Name = "�������� ������������� ����������"},
				 new AllowFiltering() { Name = "���������� ������ LAN"},
				 new AllowFiltering() { Name = "������ (��������)"},
				 new AllowFiltering() { Name = "�����, �������������� � ����������"},
				 new AllowFiltering() { Name = "��� ������"},
				 new AllowFiltering() { Name = "�������� ����������"},
				 new AllowFiltering() { Name = "�����, �"},
				 new AllowFiltering() { Name = "�������� ��������"},
				 new AllowFiltering() { Name = "��������� �� �����"}, 
				 new AllowFiltering() { Name = "��������� �� ������"}, 
					

				 new AllowFiltering() { Name = "����������� ���������"},
				 new AllowFiltering() { Name = "������������ ���������"},
				 new AllowFiltering() { Name = "��������"},
				 new AllowFiltering() { Name = "������������ ��������, ��"},
				 new AllowFiltering() { Name = "�������, ����."},
				 new AllowFiltering() { Name = "������, ����."},
				 new AllowFiltering() { Name = "���������� ����� �������� (��� ���������� ���������)"},
				 new AllowFiltering() { Name = "������������ ��������� �� �����, ��"},
				 new AllowFiltering() { Name = "����������� ��������� �� �����, ��"},
				 new AllowFiltering() { Name = "������������ �������� VESA"},
				 new AllowFiltering() { Name = "����������", ExceptedCategotiesNames = new List<string>(){"���������� �������� ����������"}},
				 new AllowFiltering() { Name = "��� ������"},
				 new AllowFiltering() { Name = "�������� ������������ ������"},
				 new AllowFiltering() { Name = "�������� ���������"},
				 new AllowFiltering() { Name = "������� �������"},
				 new AllowFiltering() { Name = "���������� ���"},
				};
		}
	}
}