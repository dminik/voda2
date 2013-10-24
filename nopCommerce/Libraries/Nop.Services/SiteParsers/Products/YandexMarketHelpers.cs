namespace Nop.Services.SiteParsers.Products
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Nop.Core.Domain.Catalog;

	public static class YandexMarketHelpers
	{
		public class AllowFiltering
		{
			public AllowFiltering()
			{
				ExceptedCategotiesNames = new List<ExceptedCategory>();
				ShowInFilter = true;
				ShowInShortDescription = true;
			}


			public string FilterName { get; set; }

			public bool ShowInFilter { get; set; }

			public bool ShowInShortDescription { get; set; }

			public List<ExceptedCategory> ExceptedCategotiesNames { get; set; }
		}

		public class ExceptedCategory
		{
			public ExceptedCategory()
			{
				ShowInFilter = false;
				ShowInShortDescription = false;
			}

			public string CategoryName { get; set; }

			public bool ShowInFilter { get; set; }

			public bool ShowInShortDescription { get; set; }
		}


		public static void IsSpecAllow(
			string specName,
			IEnumerable<Category> categoriesOfProduct,
			out bool isShowInFilter,
			out bool isShowInShortDescription)
		{
			isShowInFilter = false;
			isShowInShortDescription = false;


			// �� ���������� ��� ��������� ��������� ��������� ������������
			var allowFilters = YandexMarketHelpers.GetAllowFilteringForProductSelector;

			foreach (var currentAllowFilter in allowFilters)
			{
				if (currentAllowFilter.FilterName == specName)
				{
					// ���, ���� ����� �� ������� ��� ���� ������������


					ExceptedCategory exceptedCategory = null;

					// ��� ������� ����� ���������� ��� ����������� ������������?
					foreach (var curCategory in categoriesOfProduct)
					{
						//try
						//{
						exceptedCategory =
							currentAllowFilter.ExceptedCategotiesNames.FirstOrDefault(x => x.CategoryName == curCategory.Name);
						//}
						//catch (Exception ex)
						//{

						//}

						if (exceptedCategory != null) break;
					}


					if (exceptedCategory != null)
					{
						// ��������� ����������
						isShowInShortDescription = exceptedCategory.ShowInShortDescription;
						isShowInFilter = exceptedCategory.ShowInFilter;
					}
					else
					{
						// ��������� �������� �������
						isShowInShortDescription = currentAllowFilter.ShowInShortDescription;
						isShowInFilter = currentAllowFilter.ShowInFilter;
					}

					break;
				}

			} // end for

		}

		public static readonly IEnumerable<AllowFiltering> GetAllowFilteringForProductSelector = new List<AllowFiltering>()
			{
				new AllowFiltering() { FilterName = "�������������", ShowInShortDescription = false, },
				new AllowFiltering() { FilterName = "Bluetooth" },
				new AllowFiltering() { FilterName = "���������� SIM-����" },
				new AllowFiltering() { FilterName = "������ ������, �����" },
				new AllowFiltering() { FilterName = "���������� ������������ ������" },
				new AllowFiltering() { FilterName = "��� �������" },
				new AllowFiltering() { FilterName = "��������� �����" },
				new AllowFiltering() { FilterName = "FM-�����" },
				new AllowFiltering() { FilterName = "Android" },
				new AllowFiltering() { FilterName = "���������� ������" },
				new AllowFiltering() { FilterName = "�����������" },
				new AllowFiltering() { FilterName = "��� �����������" },
				new AllowFiltering() { FilterName = "���������� �������" },
				new AllowFiltering() { FilterName = "��� �������" },
				new AllowFiltering() { FilterName = "���������� ��������" },
				new AllowFiltering() { FilterName = "������ ��������" },
				new AllowFiltering() { FilterName = "����� �������� �������� �������, RMS (��)" },
				new AllowFiltering() { FilterName = "�������� �������" },
				new AllowFiltering() { FilterName = "USB-�����������" },
				new AllowFiltering() { FilterName = "�������������" },
				new AllowFiltering() { FilterName = "��� ������������" },
				new AllowFiltering() { FilterName = "���������� �������� �������� �������" },
				new AllowFiltering() { FilterName = "�������� ��������" },
				new AllowFiltering() { FilterName = "����� ���������� ������" },
				new AllowFiltering() { FilterName = "�������� ����� �������" }, //����� �������
				new AllowFiltering() { FilterName = "���� �������" },
				new AllowFiltering() { FilterName = "��������� ������� ���.�����" },
				new AllowFiltering() { FilterName = "��������� ������� ���������" },
				new AllowFiltering() { FilterName = "������� ��� ����������� HDD/FDD/SATA" },
				// 
				new AllowFiltering() { FilterName = "��������" }, //�������
				new AllowFiltering() { FilterName = "��� ������������" },
				new AllowFiltering() { FilterName = "�����, �������������� � ����������" },
				new AllowFiltering() { FilterName = "���������� ������� ��� HDD" },
				new AllowFiltering() { FilterName = "��������� HDD" },
				new AllowFiltering() { FilterName = "�����" },
				new AllowFiltering() { FilterName = "����������" },
				new AllowFiltering() { FilterName = "���������� ����� �������" },
				new AllowFiltering() { FilterName = "������� ����� �������" },
				new AllowFiltering() { FilterName = "������" },
				new AllowFiltering() { FilterName = "���" },
				new AllowFiltering() { FilterName = "�����������" },
				new AllowFiltering() { FilterName = "����������� ���� ������" },
				new AllowFiltering() { FilterName = "��� ������" },
				new AllowFiltering() { FilterName = "��� ����" },
				new AllowFiltering()
					{
						FilterName = "���",
						ShowInFilter = false,
						ExceptedCategotiesNames =
							new List<ExceptedCategory>()
								{
									new ExceptedCategory() { CategoryName = "�������� �������" },
									new ExceptedCategory() { CategoryName = "�������� �������" },
									new ExceptedCategory() { CategoryName = "�������� �����-�����" },
								}
					},
				new AllowFiltering() { FilterName = "������� ����", ShowInFilter = false, },
				new AllowFiltering() { FilterName = "������� �����������", ShowInFilter = false, },
				new AllowFiltering() { FilterName = "���������� ����������� ����������" },
				new AllowFiltering() { FilterName = "�������� �������� ��������" },
				new AllowFiltering() { FilterName = "�����" },
				new AllowFiltering()
					{
						FilterName = "�������",
						ExceptedCategotiesNames =
							new List<ExceptedCategory>()
								{
									new ExceptedCategory() { CategoryName = "���������� �������� ����������" },
									new ExceptedCategory() { CategoryName = "���������" },
									new ExceptedCategory() { CategoryName = "������" },
									new ExceptedCategory() { CategoryName = "�������� �����" },
									new ExceptedCategory() { CategoryName = "��������" }
								}
					},
				new AllowFiltering() { FilterName = "������� ������ ����������" },
				new AllowFiltering() { FilterName = "������� ���� CPU" },
				new AllowFiltering() { FilterName = "����" },
				new AllowFiltering() { FilterName = "���������� ����" },
				new AllowFiltering() { FilterName = "������ ����������" },
				new AllowFiltering()
					{
						FilterName = "�������",
						ExceptedCategotiesNames =
							new List<ExceptedCategory>()
								{
									new ExceptedCategory() { CategoryName = "���������� �������� ����������" },
									new ExceptedCategory() { CategoryName = "DVD � Blu-ray ������" },
									new ExceptedCategory() { CategoryName = "�������� �������" },
									new ExceptedCategory() { CategoryName = "�������� �������" },
									new ExceptedCategory() { CategoryName = "�������� �����-�����" },
									new ExceptedCategory() { CategoryName = "�������� ����������" },
									new ExceptedCategory() { CategoryName = "�������� �����" },
									new ExceptedCategory() { CategoryName = "���������� �������� ����������" }
								}
					},
				new AllowFiltering() { FilterName = "�������" },
				new AllowFiltering() { FilterName = "������ ����������" },
				new AllowFiltering() { FilterName = "������ ����������� ����� (�������� ����)" },
				new AllowFiltering() { FilterName = "������� ����������������" },
				new AllowFiltering() { FilterName = "����� ������" },
				new AllowFiltering() { FilterName = "���������" },
				new AllowFiltering() { FilterName = "����������" },
				new AllowFiltering() { FilterName = "��� ���������" },
				new AllowFiltering() { FilterName = "��������� ���������" },
				new AllowFiltering() { FilterName = "���������� ��������" },
				new AllowFiltering() { FilterName = "����" },
				new AllowFiltering() { FilterName = "��������" },
				new AllowFiltering() { FilterName = "����-����� ������" },
				new AllowFiltering() { FilterName = "����������" },
				new AllowFiltering() { FilterName = "�����" },
				new AllowFiltering() { FilterName = "����������� ������" },
				new AllowFiltering() { FilterName = "�V-��������" },
				new AllowFiltering() { FilterName = "���������� ������" },
				new AllowFiltering() { FilterName = "������� �������" },
				new AllowFiltering() { FilterName = "������������ �������" },
				new AllowFiltering() { FilterName = "����������� ������" },
				new AllowFiltering() { FilterName = "������� ����" },
				new AllowFiltering()
					{
						FilterName = "���������",
						ExceptedCategotiesNames =
							new List<ExceptedCategory>() { new ExceptedCategory() { CategoryName = "����������� �����" } }
					},
				new AllowFiltering() { FilterName = "������������" },
				new AllowFiltering() { FilterName = "������� ���������" },
				new AllowFiltering() { FilterName = "������������� � �������� ������� ������" },
				new AllowFiltering() { FilterName = "������� �������" },
				new AllowFiltering() { FilterName = "����� ����� ������� (Soft AP)" },
				new AllowFiltering() { FilterName = "�������� ������������� ����������" },
				new AllowFiltering() { FilterName = "���������� ������ LAN" },
				new AllowFiltering() { FilterName = "������ (��������)" },
				new AllowFiltering() { FilterName = "�����, �������������� � ����������" },
				new AllowFiltering() { FilterName = "��� ������" },
				new AllowFiltering() { FilterName = "�������� ����������" },
				new AllowFiltering() { FilterName = "�����, �" },
				new AllowFiltering() { FilterName = "�������� ��������" },
				new AllowFiltering() { FilterName = "��������� �� �����" },
				new AllowFiltering() { FilterName = "��������� �� ������" },
				new AllowFiltering() { FilterName = "����������� ���������" },
				new AllowFiltering() { FilterName = "������������ ���������" },
				new AllowFiltering() { FilterName = "��������" },
				new AllowFiltering() { FilterName = "������������ ��������, ��" },
				new AllowFiltering() { FilterName = "�������, ����." },
				new AllowFiltering() { FilterName = "������, ����." },
				new AllowFiltering() { FilterName = "���������� ����� �������� (��� ���������� ���������)" },
				new AllowFiltering() { FilterName = "������������ ��������� �� �����, ��" },
				new AllowFiltering() { FilterName = "����������� ��������� �� �����, ��" },
				new AllowFiltering() { FilterName = "������������ �������� VESA" },
				new AllowFiltering()
					{
						FilterName = "����������",
						ExceptedCategotiesNames =
							new List<ExceptedCategory>() { new ExceptedCategory() { CategoryName = "���������� �������� ����������" } }
					},
				new AllowFiltering() { FilterName = "��� ������" },
				new AllowFiltering() { FilterName = "�������� ������������ ������" },
				new AllowFiltering() { FilterName = "�������� ���������" },
				new AllowFiltering() { FilterName = "������� �������" },
				new AllowFiltering() { FilterName = "���������� ���" },
				new AllowFiltering()
					{
						FilterName = "���������",
						ShowInFilter = false,
						ExceptedCategotiesNames =
							new List<ExceptedCategory>()
								{
									new ExceptedCategory() { CategoryName = "���������� �������� ����������" },
									new ExceptedCategory() { CategoryName = "���������� �������� ����������" }
								}
					},
				new AllowFiltering() { FilterName = "��������� ��������� ��������", ShowInFilter = false, },
				new AllowFiltering() { FilterName = "��������� ���������", ShowInFilter = false, },
				new AllowFiltering() { FilterName = "��������������� ��������", ShowInFilter = false, },
				new AllowFiltering() { FilterName = "�������� ���������", ShowInFilter = false, },
				new AllowFiltering() { FilterName = "������� �������", ShowInFilter = false, },
				new AllowFiltering() { FilterName = "������" },
				new AllowFiltering() { FilterName = "����������, ��." },
				new AllowFiltering() { FilterName = "���������, �/�" },
				new AllowFiltering() { FilterName = "�������������" },
				new AllowFiltering() { FilterName = "�������������" },
				new AllowFiltering() { FilterName = "Art paper" },
				new AllowFiltering() { FilterName = "���������" },
				new AllowFiltering() { FilterName = "����������" },
				new AllowFiltering() { FilterName = "������������ ���������� �����������" },
				new AllowFiltering() { FilterName = "�������� ������" },
				new AllowFiltering() { FilterName = "���������� ������" },
				new AllowFiltering() { FilterName = "��� ��������" },
				new AllowFiltering() { FilterName = "����� ����������� �������" },
				new AllowFiltering() { FilterName = "����� ������" },
				new AllowFiltering() { FilterName = "�������� �����, ��" },
				new AllowFiltering() { FilterName = "������������ �������� ��������, ��./���." },
				new AllowFiltering() { FilterName = "��� ����������" },
				new AllowFiltering() { FilterName = "�������� ����" },
				new AllowFiltering() { FilterName = "Fuzzy Logic" },
				new AllowFiltering() { FilterName = "�����" },
				new AllowFiltering() { FilterName = "������� ���� ��� ������, ��" },
				new AllowFiltering() { FilterName = "������� ����" },
				new AllowFiltering() { FilterName = "����� �����, �." },
				new AllowFiltering() { FilterName = "����� ����������� ������, �." },
				new AllowFiltering() { FilterName = "����� ����������� ������, �." },
				new AllowFiltering() { FilterName = "���������� �����" },
				new AllowFiltering() { FilterName = "����� �����������������" },
				new AllowFiltering() { FilterName = "��� ����������" },
				new AllowFiltering() { FilterName = "������������ ����������� ������" },
				new AllowFiltering() { FilterName = "����������������� ��������" },
				new AllowFiltering() { FilterName = "�������������� �����" },
				new AllowFiltering() { FilterName = "���������� ������������" },
				new AllowFiltering() { FilterName = "��������������" },
				new AllowFiltering() { FilterName = "���������������" },
				new AllowFiltering() { FilterName = "��� �������� ������" },
				new AllowFiltering() { FilterName = "��� �������" },
				new AllowFiltering() { FilterName = "������� �������" },
				new AllowFiltering() { FilterName = "������� �����" },
				new AllowFiltering() { FilterName = "������� �����������" },
				new AllowFiltering() { FilterName = "������� ��������������" },
				new AllowFiltering() { FilterName = "����� �������" },
				new AllowFiltering() { FilterName = "������ � �������" },
				new AllowFiltering() { FilterName = "��������� � �������" },
				new AllowFiltering() { FilterName = "���-�������� ��������" },
				new AllowFiltering() { FilterName = "���������� ����������� �����" },
				new AllowFiltering() { FilterName = "XXXX" },
				new AllowFiltering() { FilterName = "XXXX" },
				new AllowFiltering() { FilterName = "XXXX" },
				new AllowFiltering() { FilterName = "XXXX" },
				new AllowFiltering() { FilterName = "XXXX" },
				new AllowFiltering() { FilterName = "XXXX" },
				new AllowFiltering() { FilterName = "XXXX" },
				new AllowFiltering() { FilterName = "XXXX" },
				new AllowFiltering() { FilterName = "XXXX" },
				new AllowFiltering() { FilterName = "XXXX" },
			};





	}
}