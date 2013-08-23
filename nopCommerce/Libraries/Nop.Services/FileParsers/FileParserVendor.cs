using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Services.FileParsers
{
	using System.Globalization;

	public class FileParserVendor
	{
		const char mColumnDivider = '\t';

		public static IEnumerable<ProductLineVendor> ParseFile(string filePath, out List<string> errors)
		{		
			errors = new List<string>();
			string[] lines = System.IO.File.ReadAllLines(filePath, Encoding.Default);

			var headerLineIndex = GetStringNumberWithTableHeader(lines);

			var articulColumnIndex = GetColumnIndexByName(lines[headerLineIndex], "Код товара");
			var nameColumnIndex = GetColumnIndexByName(lines[headerLineIndex], "Название товара");
			var priceRaschetColumnIndex = GetColumnIndexByName(lines[headerLineIndex], "Расч. цена");
			var priceColumnIndex = GetColumnIndexByName(lines[headerLineIndex], "Цена продажи");
			var priceBaseColumnIndex = GetColumnIndexByName(lines[headerLineIndex], "Базовая цена");
			var priceDiffColumnIndex = GetColumnIndexByName(lines[headerLineIndex], "Цена продажи – Базовая цена");
			

			var resultProductLineVendors = new List<ProductLineVendor>();
			
			for (int i = headerLineIndex + 1; i < lines.Count(); i++)
			{
				var currentLineColumns = lines[i].Split(mColumnDivider);

				if (currentLineColumns[articulColumnIndex].Length != 7) // articul должен быть 7 символов
					continue;

				var newProductLineVendor = new ProductLineVendor();

				try
				{
					newProductLineVendor.Articul = currentLineColumns[articulColumnIndex];
					newProductLineVendor.Name = currentLineColumns[nameColumnIndex];

					newProductLineVendor.PriceRaschet = GetInt(currentLineColumns[priceRaschetColumnIndex]);
					newProductLineVendor.Price = GetInt(currentLineColumns[priceColumnIndex]);
					newProductLineVendor.PriceBase = GetInt(currentLineColumns[priceBaseColumnIndex]);
					newProductLineVendor.PriceDiff = GetInt(currentLineColumns[priceDiffColumnIndex]);
				}
				catch (Exception ex)
				{
					errors.Add(lines[i]);
					continue;
				}
				
				resultProductLineVendors.Add(newProductLineVendor);
			}

			return resultProductLineVendors;
		}

		private static int GetInt(string str)
		{
			if (str == "") return 0;

			var partStr = str.Split(',')[0];
			var result = int.Parse(partStr);
			return result;
		}

		private static int GetColumnIndexByName(string line, string columnName)
		{			
			var lineColumns = line.Split(mColumnDivider);
			for (var i = 0; i < lineColumns.Length; i++)
			{
				if (lineColumns[i] == columnName) return i;
			}

			throw new Exception("Column Index By Name " + columnName + " not found in line " + line + ".");
		}

		private static int GetStringNumberWithTableHeader(string[] lines)
		{
			for (int i=0; i < lines.Count(); i++)
			{
				if (lines[i].Contains("Цена")) 
					return i;
			}

			throw new Exception("Header line not found.");
		}
	}

}
