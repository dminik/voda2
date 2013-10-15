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
				 new AllowFiltering() { Name = "Производитель"},
				 new AllowFiltering() { Name = "Bluetooth"},
				 new AllowFiltering() { Name = "Количество SIM-карт"},
				 new AllowFiltering() { Name = "Размер экрана, дюймы"},
				 new AllowFiltering() { Name = "Количество мегапикселей камеры"},
				 new AllowFiltering() { Name = "Тип корпуса"},
				 new AllowFiltering() { Name = "Сенсорный экран"},
				 new AllowFiltering() { Name = "FM-радио"},
				 new AllowFiltering() { Name = "Android"},
				 new AllowFiltering() { Name = "Количество клавиш"},
				 new AllowFiltering() { Name = "Подключение"},
				 new AllowFiltering() { Name = "Тип подключения"},
				 new AllowFiltering() { Name = "Разрешение матрицы"},
				 new AllowFiltering() { Name = "Тип матрицы"},
				 new AllowFiltering() { Name = "Встроенный микрофон"},
				 new AllowFiltering() { Name = "Формат акустики"},
				 new AllowFiltering() { Name = "Общая выходная мощность колонок, RMS (Вт)"},
				 new AllowFiltering() { Name = "Материал корпуса"},
				 new AllowFiltering() { Name = "USB-подключение"},
				 new AllowFiltering() { Name = "Совместимость"},
				 new AllowFiltering() { Name = "Тип манипулятора"},
				 new AllowFiltering() { Name = "Количество выходных разъемов питания"},
				 new AllowFiltering() { Name = "Выходная мощность"},
				 new AllowFiltering() { Name = "Время автономной работы"},

				 new AllowFiltering() { Name = "Мощность блока питания"},//блока питания
				 new AllowFiltering() { Name = "Блок питания"},
				 new AllowFiltering() { Name = "Коннектор питания мат.платы"},
				 new AllowFiltering() { Name = "Коннектор питания видеокарт"},
				 new AllowFiltering() { Name = "Разъемы для подключения HDD/FDD/SATA"},

				 new AllowFiltering() { Name = "Материал"},//корпуса
				 new AllowFiltering() { Name = "Тип оборудования"},
				 new AllowFiltering() { Name = "Цвета, использованные в оформлении"},
				 new AllowFiltering() { Name = "Внутренняя корзина для HDD"},
				 new AllowFiltering() { Name = "Крепление HDD"},
				 new AllowFiltering() { Name = "Порты"},
				 new AllowFiltering() { Name = "Индикаторы"},
				 new AllowFiltering() { Name = "Размещение блока питания"},
				 new AllowFiltering() { Name = "Наличие блока питания"},
				 new AllowFiltering() { Name = "Формат"},

				 new AllowFiltering() { Name = "Чип"},
				 new AllowFiltering() { Name = "Видеопамять"},
				 new AllowFiltering() { Name = "Разрядность шины памяти"},
				 new AllowFiltering() { Name = "Тип памяти"},
				 new AllowFiltering() { Name = "Тип чипа"},
					
				 new AllowFiltering() { Name = "Пропускная способность интерфейса"},
				 new AllowFiltering() { Name = "Скорость вращения шпинделя"},
				 new AllowFiltering() { Name = "Буфер"},
				 new AllowFiltering() { Name = "Емкость", ExceptedCategotiesNames = new List<string>(){"Компактные цифровые фотокамеры"}},
					
				 new AllowFiltering() { Name = "Частота работы процессора"},
				 new AllowFiltering() { Name = "Частота шины CPU"},
				 new AllowFiltering() { Name = "Ядро"},
				 new AllowFiltering() { Name = "Количество ядер"},
				 new AllowFiltering() { Name = "Гнездо процессора"},

				 new AllowFiltering() { Name = "Питание", ExceptedCategotiesNames = new List<string>(){"Компактные цифровые фотокамеры"}},

				 new AllowFiltering() { Name = "Внешний"},

				 new AllowFiltering() { Name = "Гнездо процессора"},
				 new AllowFiltering() { Name = "Чипсет материнской платы (Северный мост)"},

				 new AllowFiltering() { Name = "Частота функционирования"}, 
				 new AllowFiltering() { Name = "Объем памяти"},
				 new AllowFiltering() { Name = "Диагональ"},
				 new AllowFiltering() { Name = "Разрешение"},

				 new AllowFiltering() { Name = "Тип наушников"}, 
				 new AllowFiltering() { Name = "Регулятор громкости"},
				 new AllowFiltering() { Name = "Допустимая мощность"},

				 new AllowFiltering() { Name = "Цвет"}, 
				 new AllowFiltering() { Name = "Материал"},
				 new AllowFiltering() { Name = "Пыле-влаго защита"},
				 new AllowFiltering() { Name = "Технологии"},
				 new AllowFiltering() { Name = "Серия"},
				 new AllowFiltering() { Name = "Соотношение сторон"},
				 new AllowFiltering() { Name = "ТV-приемник"},
				 new AllowFiltering() { Name = "Оптический привод"},
				 new AllowFiltering() { Name = "Видимая область"},
				 new AllowFiltering() { Name = "Операционная система"},
				 new AllowFiltering() { Name = "Оперативная память"}, 
				 new AllowFiltering() { Name = "Жесткий диск"},
				 new AllowFiltering() { Name = "Процессор"},
				 new AllowFiltering() { Name = "Видеосистема"},
					
				 new AllowFiltering() { Name = "Функция диктофона"},
				 new AllowFiltering() { Name = "Использование в качестве внешней памяти"},
				 new AllowFiltering() { Name = "Форматы записей"},

				 new AllowFiltering() { Name = "Режим точки доступа (Soft AP)"},
				 new AllowFiltering() { Name = "Скорость беспроводного соединения"},
				 new AllowFiltering() { Name = "Количество портов LAN"},
				 new AllowFiltering() { Name = "Корпус (материал)"},
				 new AllowFiltering() { Name = "Цвета, использованные в оформлении"},
				 new AllowFiltering() { Name = "Тип экрана"},
				 new AllowFiltering() { Name = "Материал проводника"},
				 new AllowFiltering() { Name = "Длина, м"},
				 new AllowFiltering() { Name = "Покрытие разъемов"},
				 new AllowFiltering() { Name = "Коннектор на входе"}, 
				 new AllowFiltering() { Name = "Коннектор на выходе"}, 
					

				 new AllowFiltering() { Name = "Минимальная диагональ"},
				 new AllowFiltering() { Name = "Максимальная диагональ"},
				 new AllowFiltering() { Name = "Стандарт"},
				 new AllowFiltering() { Name = "Максимальная нагрузка, кг"},
				 new AllowFiltering() { Name = "Поворот, град."},
				 new AllowFiltering() { Name = "Наклон, град."},
				 new AllowFiltering() { Name = "Количество точек поворота (для поворотных креплений)"},
				 new AllowFiltering() { Name = "Максимальная дистанция от стены, мм"},
				 new AllowFiltering() { Name = "Минимальная дистанция от стены, мм"},
				 new AllowFiltering() { Name = "Максимальный стандарт VESA"},
				 new AllowFiltering() { Name = "Типоразмер", ExceptedCategotiesNames = new List<string>(){"Компактные цифровые фотокамеры"}},
				 new AllowFiltering() { Name = "Тип плеера"},
				 new AllowFiltering() { Name = "Комплект акустических систем"},
				 new AllowFiltering() { Name = "Мощность суммарная"},
				 new AllowFiltering() { Name = "Функция караоке"},
				 new AllowFiltering() { Name = "Оптический зум"},
				};
		}
	}
}