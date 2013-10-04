namespace Nop.Services.SiteParsers
{
	using System;
	using System.Data;
	using System.Linq;

	using Nop.Core.IO;
	using Nop.Services.FileParsers;

	public class UgContractPriceParser : BasePriceParser<ProductLineVendor>
	{		
		protected override string XlsFileName { get { return "Ostatki.xls"; } }

		protected override string UrlBase { get { return "http://yugpartner.com.ua"; } }
		protected override string UrlAuthorization { get { return UrlBase + "/auth/"; } }
		protected override string UrlAuthorizationPostParams { get { return "login=Oleynic&password=18072008y"; } }
		protected override string UrlDownload 
		{ 
			get
			{
				var today = DateTime.Now.ToString("dd.MM.yyyy");
				return UrlBase + "/report-f5/trade-balances/date" + today + "/suppliers4,95/excel/";
			} 
		}
		
		protected override void PostProcessing()
		{
			if (ResultList.Count() < 700)
				throw new Exception("Hm... Price list less then 700 lines. Count=" + ResultList.Count());
		}


		protected override ProductLineVendor GetObjectFromReader(IDataReader reader)
		{
			var newProductLineVendor = new ProductLineVendor();
			
			try
			{
				var artikul = XlsProvider.GetFieldValueFromReader<string>(reader, "��� ������");

				if (artikul.Length != 7) // articul ������ ���� 7 ��������
					return null;

				newProductLineVendor.Articul = artikul;
				//newProductLineVendor.Name = XlsProvider.GetFieldValueFromReader<string>(reader, "�������� ������");

				//newProductLineVendor.PriceRaschet = GetInt(XlsProvider.GetFieldValueFromReader<string>(reader, "����# ����"));
				//newProductLineVendor.Price = GetInt(XlsProvider.GetFieldValueFromReader<string>(reader, "���� �������"));
				//newProductLineVendor.PriceBase = GetInt(XlsProvider.GetFieldValueFromReader<string>(reader, "������� ����"));
				//newProductLineVendor.PriceDiff = GetInt(XlsProvider.GetFieldValueFromReader<string>(reader, "���� ������� � ������� ����"));

			}
			catch (Exception ex)
			{
				// errors.Add(worksheet.Cells[currentRow, GetColumnIndex(properties, "��� ������")].Value as string);
				return null;
			}
			return newProductLineVendor;
		}
	}
}
