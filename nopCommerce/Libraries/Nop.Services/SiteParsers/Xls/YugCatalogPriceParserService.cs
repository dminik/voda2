namespace Nop.Services.SiteParsers.Xls
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;

	using Nop.Core.Domain.Security;
	using Nop.Core.IO;
	using Nop.Core.Infrastructure;
	using Nop.Services.Catalog;
	using Nop.Services.Logging;

	public class YugCatalogPriceParserService : BasePriceParser<ProductLineVendor>, IYugCatalogPriceParserService
	{		
		public YugCatalogPriceParserService(IProductService productService, ILogger logger)
			: base(productService, logger)
		{
			
		}

		protected override string XlsFileName { get { return "YugCatalog.xls"; } }


		//protected override string SheetNameFirst 
		//{ 
		//	get 
		//	{
		//		return "�����-����� �� " + ; // �����-����� �� 12-10-2013 15-22 
		//	} 
		//}

		protected override string UrlBase { get { return "http://wholesale.yugcontract.ua"; } }
		protected override string UrlAuthorizationGet { get { return this.UrlBase + "/login/?authen_dest=http://wholesale.yugcontract.ua/wholesale/main.html"; } }
		protected override string UrlAuthorizationPost { get { return this.UrlBase + "/authen"; } }
		protected override string UrlAuthorizationPostParams
		{
			get
			{
				var securitySettings = EngineContext.Current.Resolve<SecuritySettings>();
				
				return "authen_dest=%2Fwholesale%2Fmain.html&"
					+ "user_login=" + securitySettings.F5SiteLogin
					+ "&user_password=" + securitySettings.F5SitePassword;
			}
		}
		
		protected override string UrlDownload 
		{ 
			get
			{				
				return this.UrlBase + "/xls_im/";
			} 
		}

		//protected override HttpWebResponse AfterAutherisation(HttpWebResponse httpWebResponse)
		//{
		//	httpWebResponse = this.HttpQuery(
		//			UrlDownload,
		//			false,
		//			"",
		//			"");

		//	CheckResponseCorrect(httpWebResponse, "", HttpStatusCode.Redirect);


		//	var redirectedUrl = httpWebResponse.ResponseUri.AbsoluteUri;

		//	httpWebResponse = this.HttpQuery(
		//			redirectedUrl,
		//			false,
		//			"",
		//			"");





		//	return httpWebResponse;
		//}

		protected override void PostProcessing()
		{
			if (this.ResultList.Count() < 6000)
				throw new Exception("Hm... " + this.GetType().Name + ".Price list less then 6000 lines. Count=" + this.ResultList.Count());
		}

		protected override ProductLineVendor GetObjectFromReader(IDataReader reader)
		{
			var newProductLineVendor = new ProductLineVendor();
			
			try
			{
				var existing = XlsProvider.GetFieldValueFromReader<string>(reader, "????? ????") == "����";// ����� ����

				if (!existing) 
					return null;

				var artikul = XlsProvider.GetFieldValueFromReader<string>(reader, "???"); // ���

				int intArticil;
				if (!int.TryParse(artikul, out intArticil)) // articul ������ ���� 7 ��������
					return null;

				newProductLineVendor.Articul = artikul;
				newProductLineVendor.Name = XlsProvider.GetFieldValueFromReader<string>(reader, "?????");// �����

				//newProductLineVendor.PriceRaschet = GetInt(XlsProvider.GetFieldValueFromReader<string>(reader, "����� ����"));
				//newProductLineVendor.Price = GetInt(XlsProvider.GetFieldValueFromReader<string>(reader, "���� �������"));
				//newProductLineVendor.PriceBase = GetInt(XlsProvider.GetFieldValueFromReader<string>(reader, "������� ����"));
				//newProductLineVendor.PriceDiff = GetInt(XlsProvider.GetFieldValueFromReader<string>(reader, "���� ������� � ������� ����"));

			}
			catch
			{
				// errors.Add(worksheet.Cells[currentRow, GetColumnIndex(properties, "��� ������")].Value as string);
				return null;
			}
			return newProductLineVendor;
		}

		public OstatkiParserModel ParseAndShow(bool isUpdateCacheFromInternet)
		{
			var newSpecsOnly = this._Parse(isUpdateCacheFromInternet);
			return newSpecsOnly;
		}

		/// <summary>
		/// Set existing in Boyarka
		/// </summary>
		/// <param name="isUpdateCacheFromInternet"></param>
		/// <returns></returns>
		public string SetExistingInVendor(bool isUpdateCacheFromInternet)
		{
			var priceList = this._Parse(isUpdateCacheFromInternet);

			string notFoundArticules = ""; 
			string foundArticules = ""; 
			int successCounter = 0;

			/*
			 * ������� ��������� ������ � ���
			 * 1. ���������� ��� ���� � ����� � 0
			 * 2. ���� ����� ���� � ���������� ��� � ������ - ������ ����
			 * 2. ���� ����� ���� � ����������, �� ������ ��������� "��"
			 * 2. ���� ����� ���� � ������, �� ������ ��������� "���" � �������� ������ "��"
			 * 
			 * 
			                     ������ ��� �����               ����� ���� � ����������               ����� � ������  
			 ����           0 (�� ���������� ��� �� �����)       100                                         100		
			 ���������              ��                           ��                                          ���			
		     �������� ������        ���                          ���                                          ��
			 
			*/



			var productVariantList = this._productService.GetProductVariants().ToList();
			foreach (var currentProductVariant in productVariantList)
			{
				var curProductLine = priceList.ProductLineList.SingleOrDefault(x => x.Articul == currentProductVariant.Sku);
				var isProductInPriceList = curProductLine != null;
			
				if (!isProductInPriceList)
				{
					notFoundArticules += currentProductVariant.Sku + ", ";
					continue;
				}
			
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
				}

				//if (isNeedUpdate)
				//	_productService.UpdateProductVariant(currentProductVariant);

				successCounter++;
				foundArticules += curProductLine.Articul + ", ";
			}

			if (productVariantList.Count > 0) // �������� ���������� ����� ��������� (���� ���������)
				this._productService.UpdateProductVariant(productVariantList[0]);

			var msg = "";

			if (notFoundArticules == "")
				msg = "Success for all!";
			else
			{
				msg = "Success for " + successCounter + " from " + priceList.ProductLineList.Count();
			}

			this.mLogger.Debug(msg);
			return msg;
		}
		
		private OstatkiParserModel _Parse(bool isUpdateCacheFromInternet)
		{			
			List<string> errors;

			var list = this.GetPriceListFromCache(out errors, isUpdateCacheFromInternet);

			var resultModel = new OstatkiParserModel { ProductLineList = list, ErrorList = errors };

			return resultModel;			
		}
	}
}
