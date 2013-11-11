namespace Nop.Admin.Models.YandexMarket
{
	using System;
	using System.Collections.Generic;	
	using System.Text;
	
	public static class ListExtention
	{
		// Call like     var html = ToHtmlList(people, x => x.LastName, x => x.FirstName);
		public static string ToHtmlList<T>(this List<T> list, params Func<T, object>[] fxns)
		{
			var sb = new StringBuilder();
			sb.Append("<TABLE>\n");
			foreach (var item in list)
			{
				sb.Append("<TR>\n");
				foreach (var fxn in fxns)
				{
					sb.Append("<TD>");
					sb.Append(fxn(item));
					sb.Append("</TD>");
				}
				sb.Append("</TR>\n");
			}
			sb.Append("</TABLE>");

			return sb.ToString();
		}
	}	
}