using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Services.FileParsers
{
	using System.Globalization;
	
	public class OstatkiFileParser
	{
		const char mColumnDivider = '\t';

		public static IEnumerable<ProductLine> ParseOstatkiFile(string filePath, out List<string> errors)
		{		
			errors = new List<string>();
			string[] lines = System.IO.File.ReadAllLines(filePath, Encoding.Default);

			var headerLineIndex = GetStringNumberWithTableHeader(lines);

			var articulColumnIndex = GetColumnIndexByName(lines[headerLineIndex], "Код");
			var priceColumnIndex = GetColumnIndexByName(lines[headerLineIndex], "Цена");
			var amountColumnIndex = GetColumnIndexByName(lines[headerLineIndex], "Кво");

			var resultProductLines = new List<ProductLine>();
			
			for (int i = headerLineIndex + 1; i < lines.Count(); i++)
			{
				var currentLineColumns = lines[i].Split(mColumnDivider);

				if (currentLineColumns[articulColumnIndex].Length < 7) // articul должен быть длинне 2 символов
					continue;

				var newProductLine = new ProductLine();

				try
				{
					newProductLine.Articul = currentLineColumns[articulColumnIndex];
					newProductLine.Amount = int.Parse(currentLineColumns[amountColumnIndex].Replace(",000", "").Replace(",00", "").Replace(",0", ""));
					newProductLine.Price = int.Parse(currentLineColumns[priceColumnIndex].Replace(",000", "").Replace(",00", "").Replace(",0", ""));
				}
				catch (Exception ex)
				{
					errors.Add(lines[i]);
					continue;
				}
				
				resultProductLines.Add(newProductLine);
			}

			return resultProductLines;
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
