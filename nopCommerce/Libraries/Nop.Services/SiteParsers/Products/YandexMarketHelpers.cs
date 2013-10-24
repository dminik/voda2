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


			// не показываем для некоторых категорий некоторые спецификации
			var allowFilters = YandexMarketHelpers.GetAllowFilteringForProductSelector;

			foreach (var currentAllowFilter in allowFilters)
			{
				if (currentAllowFilter.FilterName == specName)
				{
					// ага, есть какое то правило для этой спецификации


					ExceptedCategory exceptedCategory = null;

					// это правило имеет исключение для проверяемой спецификации?
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
						// применяем исключение
						isShowInShortDescription = exceptedCategory.ShowInShortDescription;
						isShowInFilter = exceptedCategory.ShowInFilter;
					}
					else
					{
						// применяем основное правило
						isShowInShortDescription = currentAllowFilter.ShowInShortDescription;
						isShowInFilter = currentAllowFilter.ShowInFilter;
					}

					break;
				}

			} // end for

		}

		public static readonly IEnumerable<AllowFiltering> GetAllowFilteringForProductSelector = new List<AllowFiltering>()
			{
				new AllowFiltering() { FilterName = "Производитель", ShowInShortDescription = false, },
				new AllowFiltering() { FilterName = "Bluetooth" },
				new AllowFiltering() { FilterName = "Количество SIM-карт" },
				new AllowFiltering() { FilterName = "Размер экрана, дюймы" },
				new AllowFiltering() { FilterName = "Количество мегапикселей камеры" },
				new AllowFiltering() { FilterName = "Тип корпуса" },
				new AllowFiltering() { FilterName = "Сенсорный экран" },
				new AllowFiltering() { FilterName = "FM-радио" },
				new AllowFiltering() { FilterName = "Android" },
				new AllowFiltering() { FilterName = "Количество клавиш" },
				new AllowFiltering() { FilterName = "Подключение" },
				new AllowFiltering() { FilterName = "Тип подключения" },
				new AllowFiltering() { FilterName = "Разрешение матрицы" },
				new AllowFiltering() { FilterName = "Тип матрицы" },
				new AllowFiltering() { FilterName = "Встроенный микрофон" },
				new AllowFiltering() { FilterName = "Формат акустики" },
				new AllowFiltering() { FilterName = "Общая выходная мощность колонок, RMS (Вт)" },
				new AllowFiltering() { FilterName = "Материал корпуса" },
				new AllowFiltering() { FilterName = "USB-подключение" },
				new AllowFiltering() { FilterName = "Совместимость" },
				new AllowFiltering() { FilterName = "Тип манипулятора" },
				new AllowFiltering() { FilterName = "Количество выходных разъемов питания" },
				new AllowFiltering() { FilterName = "Выходная мощность" },
				new AllowFiltering() { FilterName = "Время автономной работы" },
				new AllowFiltering() { FilterName = "Мощность блока питания" }, //блока питания
				new AllowFiltering() { FilterName = "Блок питания" },
				new AllowFiltering() { FilterName = "Коннектор питания мат.платы" },
				new AllowFiltering() { FilterName = "Коннектор питания видеокарт" },
				new AllowFiltering() { FilterName = "Разъемы для подключения HDD/FDD/SATA" },
				// 
				new AllowFiltering() { FilterName = "Материал" }, //корпуса
				new AllowFiltering() { FilterName = "Тип оборудования" },
				new AllowFiltering() { FilterName = "Цвета, использованные в оформлении" },
				new AllowFiltering() { FilterName = "Внутренняя корзина для HDD" },
				new AllowFiltering() { FilterName = "Крепление HDD" },
				new AllowFiltering() { FilterName = "Порты" },
				new AllowFiltering() { FilterName = "Индикаторы" },
				new AllowFiltering() { FilterName = "Размещение блока питания" },
				new AllowFiltering() { FilterName = "Наличие блока питания" },
				new AllowFiltering() { FilterName = "Формат" },
				new AllowFiltering() { FilterName = "Чип" },
				new AllowFiltering() { FilterName = "Видеопамять" },
				new AllowFiltering() { FilterName = "Разрядность шины памяти" },
				new AllowFiltering() { FilterName = "Тип памяти" },
				new AllowFiltering() { FilterName = "Тип чипа" },
				new AllowFiltering()
					{
						FilterName = "Тип",
						ShowInFilter = false,
						ExceptedCategotiesNames =
							new List<ExceptedCategory>()
								{
									new ExceptedCategory() { CategoryName = "Струйные цветные" },
									new ExceptedCategory() { CategoryName = "Лазерные цветные" },
									new ExceptedCategory() { CategoryName = "Лазерные черно-белые" },
								}
					},
				new AllowFiltering() { FilterName = "Уровень шума", ShowInFilter = false, },
				new AllowFiltering() { FilterName = "Размеры вентилятора", ShowInFilter = false, },
				new AllowFiltering() { FilterName = "Пропускная способность интерфейса" },
				new AllowFiltering() { FilterName = "Скорость вращения шпинделя" },
				new AllowFiltering() { FilterName = "Буфер" },
				new AllowFiltering()
					{
						FilterName = "Емкость",
						ExceptedCategotiesNames =
							new List<ExceptedCategory>()
								{
									new ExceptedCategory() { CategoryName = "Компактные цифровые фотокамеры" },
									new ExceptedCategory() { CategoryName = "Смартфоны" },
									new ExceptedCategory() { CategoryName = "Бумага" },
									new ExceptedCategory() { CategoryName = "Кухонные плиты" },
									new ExceptedCategory() { CategoryName = "Планшеты" }
								}
					},
				new AllowFiltering() { FilterName = "Частота работы процессора" },
				new AllowFiltering() { FilterName = "Частота шины CPU" },
				new AllowFiltering() { FilterName = "Ядро" },
				new AllowFiltering() { FilterName = "Количество ядер" },
				new AllowFiltering() { FilterName = "Гнездо процессора" },
				new AllowFiltering()
					{
						FilterName = "Питание",
						ExceptedCategotiesNames =
							new List<ExceptedCategory>()
								{
									new ExceptedCategory() { CategoryName = "Компактные цифровые фотокамеры" },
									new ExceptedCategory() { CategoryName = "DVD и Blu-ray плееры" },
									new ExceptedCategory() { CategoryName = "Струйные цветные" },
									new ExceptedCategory() { CategoryName = "Лазерные цветные" },
									new ExceptedCategory() { CategoryName = "Лазерные черно-белые" },
									new ExceptedCategory() { CategoryName = "Домашние кинотеатры" },
									new ExceptedCategory() { CategoryName = "Кухонные плиты" },
									new ExceptedCategory() { CategoryName = "Зеркальные цифровые фотокамеры" }
								}
					},
				new AllowFiltering() { FilterName = "Внешний" },
				new AllowFiltering() { FilterName = "Гнездо процессора" },
				new AllowFiltering() { FilterName = "Чипсет материнской платы (Северный мост)" },
				new AllowFiltering() { FilterName = "Частота функционирования" },
				new AllowFiltering() { FilterName = "Объем памяти" },
				new AllowFiltering() { FilterName = "Диагональ" },
				new AllowFiltering() { FilterName = "Разрешение" },
				new AllowFiltering() { FilterName = "Тип наушников" },
				new AllowFiltering() { FilterName = "Регулятор громкости" },
				new AllowFiltering() { FilterName = "Допустимая мощность" },
				new AllowFiltering() { FilterName = "Цвет" },
				new AllowFiltering() { FilterName = "Материал" },
				new AllowFiltering() { FilterName = "Пыле-влаго защита" },
				new AllowFiltering() { FilterName = "Технологии" },
				new AllowFiltering() { FilterName = "Серия" },
				new AllowFiltering() { FilterName = "Соотношение сторон" },
				new AllowFiltering() { FilterName = "ТV-приемник" },
				new AllowFiltering() { FilterName = "Оптический привод" },
				new AllowFiltering() { FilterName = "Видимая область" },
				new AllowFiltering() { FilterName = "Операционная система" },
				new AllowFiltering() { FilterName = "Оперативная память" },
				new AllowFiltering() { FilterName = "Жесткий диск" },
				new AllowFiltering()
					{
						FilterName = "Процессор",
						ExceptedCategotiesNames =
							new List<ExceptedCategory>() { new ExceptedCategory() { CategoryName = "Электронные книги" } }
					},
				new AllowFiltering() { FilterName = "Видеосистема" },
				new AllowFiltering() { FilterName = "Функция диктофона" },
				new AllowFiltering() { FilterName = "Использование в качестве внешней памяти" },
				new AllowFiltering() { FilterName = "Форматы записей" },
				new AllowFiltering() { FilterName = "Режим точки доступа (Soft AP)" },
				new AllowFiltering() { FilterName = "Скорость беспроводного соединения" },
				new AllowFiltering() { FilterName = "Количество портов LAN" },
				new AllowFiltering() { FilterName = "Корпус (материал)" },
				new AllowFiltering() { FilterName = "Цвета, использованные в оформлении" },
				new AllowFiltering() { FilterName = "Тип экрана" },
				new AllowFiltering() { FilterName = "Материал проводника" },
				new AllowFiltering() { FilterName = "Длина, м" },
				new AllowFiltering() { FilterName = "Покрытие разъемов" },
				new AllowFiltering() { FilterName = "Коннектор на входе" },
				new AllowFiltering() { FilterName = "Коннектор на выходе" },
				new AllowFiltering() { FilterName = "Минимальная диагональ" },
				new AllowFiltering() { FilterName = "Максимальная диагональ" },
				new AllowFiltering() { FilterName = "Стандарт" },
				new AllowFiltering() { FilterName = "Максимальная нагрузка, кг" },
				new AllowFiltering() { FilterName = "Поворот, град." },
				new AllowFiltering() { FilterName = "Наклон, град." },
				new AllowFiltering() { FilterName = "Количество точек поворота (для поворотных креплений)" },
				new AllowFiltering() { FilterName = "Максимальная дистанция от стены, мм" },
				new AllowFiltering() { FilterName = "Минимальная дистанция от стены, мм" },
				new AllowFiltering() { FilterName = "Максимальный стандарт VESA" },
				new AllowFiltering()
					{
						FilterName = "Типоразмер",
						ExceptedCategotiesNames =
							new List<ExceptedCategory>() { new ExceptedCategory() { CategoryName = "Компактные цифровые фотокамеры" } }
					},
				new AllowFiltering() { FilterName = "Тип плеера" },
				new AllowFiltering() { FilterName = "Комплект акустических систем" },
				new AllowFiltering() { FilterName = "Мощность суммарная" },
				new AllowFiltering() { FilterName = "Функция караоке" },
				new AllowFiltering() { FilterName = "Оптический зум" },
				new AllowFiltering()
					{
						FilterName = "Интерфейс",
						ShowInFilter = false,
						ExceptedCategotiesNames =
							new List<ExceptedCategory>()
								{
									new ExceptedCategory() { CategoryName = "Компактные цифровые фотокамеры" },
									new ExceptedCategory() { CategoryName = "Зеркальные цифровые фотокамеры" }
								}
					},
				new AllowFiltering() { FilterName = "Поддержка текстовых форматов", ShowInFilter = false, },
				new AllowFiltering() { FilterName = "Поддержка носителей", ShowInFilter = false, },
				new AllowFiltering() { FilterName = "Воспроизведение форматов", ShowInFilter = false, },
				new AllowFiltering() { FilterName = "Мощность суммарная", ShowInFilter = false, },
				new AllowFiltering() { FilterName = "Функция караоке", ShowInFilter = false, },
				new AllowFiltering() { FilterName = "Размер" },
				new AllowFiltering() { FilterName = "Количество, шт." },
				new AllowFiltering() { FilterName = "Плотность, г/м" },
				new AllowFiltering() { FilterName = "Двустроронняя" },
				new AllowFiltering() { FilterName = "Самоклеющаяся" },
				new AllowFiltering() { FilterName = "Art paper" },
				new AllowFiltering() { FilterName = "Магнитная" },
				new AllowFiltering() { FilterName = "Фотобумага" },
				new AllowFiltering() { FilterName = "Максимальное разрешение изображения" },
				new AllowFiltering() { FilterName = "Скорость печати" },
				new AllowFiltering() { FilterName = "Технология печати" },
				new AllowFiltering() { FilterName = "Тип загрузки" },
				new AllowFiltering() { FilterName = "Класс потребления энергии" },
				new AllowFiltering() { FilterName = "Класс отжима" },
				new AllowFiltering() { FilterName = "Загрузка белья, кг" },
				new AllowFiltering() { FilterName = "Максимальная скорость вращения, об./мин." },
				new AllowFiltering() { FilterName = "Тип управления" },
				new AllowFiltering() { FilterName = "Материал бака" },
				new AllowFiltering() { FilterName = "Fuzzy Logic" },
				new AllowFiltering() { FilterName = "Сушка" },
				new AllowFiltering() { FilterName = "Уровень шума при отжиме, дБ" },
				new AllowFiltering() { FilterName = "Уровень шума" },
				new AllowFiltering() { FilterName = "Объем общий, л." },
				new AllowFiltering() { FilterName = "Объем холодильной камеры, л." },
				new AllowFiltering() { FilterName = "Объем морозильной камеры, л." },
				new AllowFiltering() { FilterName = "Количество камер" },
				new AllowFiltering() { FilterName = "Класс энергопотребления" },
				new AllowFiltering() { FilterName = "Тип управления" },
				new AllowFiltering() { FilterName = "Расположение морозильной камеры" },
				new AllowFiltering() { FilterName = "Антибактериальное покрытие" },
				new AllowFiltering() { FilterName = "Перевешиваемые двери" },
				new AllowFiltering() { FilterName = "Количество компрессоров" },
				new AllowFiltering() { FilterName = "Суперзаморозка" },
				new AllowFiltering() { FilterName = "Суперохлаждение" },
				new AllowFiltering() { FilterName = "Тип варочной панели" },
				new AllowFiltering() { FilterName = "Тип духовки" },
				new AllowFiltering() { FilterName = "Наличие дисплея" },
				new AllowFiltering() { FilterName = "Наличие гриля" },
				new AllowFiltering() { FilterName = "Рабочая поверхность" },
				new AllowFiltering() { FilterName = "Наличие электроподжига" },
				new AllowFiltering() { FilterName = "Объем духовки" },
				new AllowFiltering() { FilterName = "Вертел в духовке" },
				new AllowFiltering() { FilterName = "Конвекция в духовке" },
				new AllowFiltering() { FilterName = "Газ-контроль конфорок" },
				new AllowFiltering() { FilterName = "Индикаторы остаточного тепла" },
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