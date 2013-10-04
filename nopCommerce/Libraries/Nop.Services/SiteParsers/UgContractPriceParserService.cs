namespace Nop.Services.SiteParsers
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;

	using Nop.Core.IO;
	using Nop.Services.Catalog;
	using Nop.Services.FileParsers;
	using Nop.Services.Logging;

	public class UgContractPriceParserService : BasePriceParser<ProductLineVendor>, IUgContractPriceParserService
	{		
		public UgContractPriceParserService(IProductService productService, ILogger logger)
			: base(productService, logger)
		{
			
		}

		protected override string XlsFileName { get { return "Ostatki.xls"; } }

		protected override string UrlBase { get { return "http://yugpartner.com.ua"; } }
		protected override string UrlAuthorization { get { return UrlBase + "/auth/"; } }
		protected override string UrlAuthorizationPostParams { get { return "login=Oleynic&password=18072008y"; } }
		protected override string UrlDownload 
		{ 
			get
			{
				var yesterday = DateTime.Now.AddDays(-1).ToString("dd.MM.yyyy");
				return UrlBase + "/report-f5/trade-balances/date" + yesterday + "/suppliers4,95/excel/";
			} 
		}
		
		protected override void PostProcessing()
		{
			if (ResultList.Count() < 700)
				throw new Exception("Hm... " + this.GetType().Name + ".Price list less then 700 lines. Count=" + ResultList.Count());
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

		public OstatkiFileParserModel ParseAndShow(bool isUpdateCacheFromInternet)
		{
			var newSpecsOnly = _Parse(isUpdateCacheFromInternet);
			return newSpecsOnly;
		}

		public string ApplyImport(bool isUpdateCacheFromInternet)
		{
			var list = _Parse(isUpdateCacheFromInternet);

			string notFoundArticules = "";
			string foundArticules = "";
			int successCounter = 0;

			/*
			                     ������ ��� �����               ����� ���� � ����������               ����� � ������
			 ����           0 (�� ���������� ��� �� �����)       ����                                       ����
			 ���������              ��                           ��                                          ���
		     �������� ������                     -               ���                                          ��
			 * 
			 * 
			 * ����������:
				���������� �� ����������� ��
				����������� � 2 ������ ��
				��� �������, �������� � ���� ������ ��
				����� �������� ��
				������ ������� - ���������� ��������

			*/

			foreach (var curProductLine in list.ProductLineList)
			{
				var currentProductVariant = _productService.GetProductVariantBySku(curProductLine.Articul);
				if (currentProductVariant == null)
				{
					notFoundArticules += curProductLine.Articul + ", ";
					continue;
				}

				var isNeedUpdate = false;

				//if (currentProductVariant.StockQuantity != curProductLine.Amount)
				//{
				//	currentProductVariant.StockQuantity = curProductLine.Amount;
				//	isNeedUpdate = true;
				//}

				//if (currentProductVariant.Price != curProductLine.Price)
				//{
				//	currentProductVariant.Price = curProductLine.Price;
				//	isNeedUpdate = true;
				//}

				if (currentProductVariant.AvailableForPreOrder != false)
				{
					currentProductVariant.AvailableForPreOrder = false;
					isNeedUpdate = true;
				}

				if (isNeedUpdate)
					_productService.UpdateProductVariant(currentProductVariant);

				successCounter++;
				foundArticules += curProductLine.Articul + ", ";
			}

			if (notFoundArticules == "")
				return "Success for all!";
			else
			{
				return "Success for " + successCounter + " from " + list.ProductLineList.Count()
					+ ". Not Found Articules in shop but they exist in file Ostatki:" + notFoundArticules
					+ ". Success Articules" + foundArticules;
			}	
		}

		private OstatkiFileParserModel _Parse(bool isUpdateCacheFromInternet)
		{			
			List<string> errors;

			var list = GetPriceListFromCache(out errors, true);

			var resultModel = new OstatkiFileParserModel { ProductLineList = list, ErrorList = errors };

			return resultModel;			
		}
	}
}
