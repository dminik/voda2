namespace Nop.Services.SiteParsers.Xls
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;

	using Nop.Core.Domain.Catalog;
	using Nop.Core.Infrastructure;
	using Nop.Services.Catalog;
	using Nop.Services.Logging;
	using Nop.Services.SiteParsers.Page;
	using Nop.Services.Tasks;
	using Nop.Services.YandexMarket;

	public class PriceManagerService : IPriceManagerService
	{
		private readonly IProductService _productService;
		private readonly ILogger _logger;
		private readonly IOstatkiPriceParserService _ostatkiPriceParserService;
		private readonly IF5PriceParserService _f5PriceParserService;
		private readonly IYugCatalogPriceParserService _yugCatalogPriceParserService;
		private readonly ISpecialPriceService _specialPriceService;

		public PriceManagerService(IProductService productService, 
			ILogger logger, 
			IOstatkiPriceParserService ugContractPriceParserService,
			IF5PriceParserService f5PriceParserService,
			IYugCatalogPriceParserService yugCatalogPriceParserService,
			ISpecialPriceService specialPriceService)			
		{
			this._productService = productService;
			this._logger = logger;
			this._ostatkiPriceParserService = ugContractPriceParserService;
			this._f5PriceParserService = f5PriceParserService;
			this._yugCatalogPriceParserService = yugCatalogPriceParserService;
			_specialPriceService = specialPriceService;
		}

		private IEnumerable<Category> GetUsedCategories(IEnumerable<ProductLine> products, IEnumerable<ProductVariant> allProductVariantList)
		{			
			var skuList = products.ToList().Select(x => x.Articul).ToList();

			var usedCategories = allProductVariantList
				.Where(x =>  skuList.Contains(x.Sku))
				.Select(pv => (pv.Product.ProductCategories.ToList()[0]).Category)
				.Distinct();

			return usedCategories;
		}

		public string ApplyImportAll(bool isForceDownloadingNewData)
		{
			this._logger.Debug("+++++ ApplyPriceImportAll start");
			var name = typeof(ParsePricesTask).FullName + ", Nop.Services";
			var scheduleTaskService = EngineContext.Current.Resolve<IScheduleTaskService>();
			var scheduleTask = scheduleTaskService.GetTaskByType(name);

			var isNeedUpdate = false;

			// Update prices if it need
			if (scheduleTask.LastSuccessUtc != null)
			{
				if (scheduleTask.LastSuccessUtc.Value.Date < DateTime.UtcNow.Date)
				{
					isNeedUpdate = true;
					this._logger.Debug("      Need daily update");
				}
			}
			else
			{
				this._logger.Debug("      Need first time update");
				isNeedUpdate = true;
			}

			if(isForceDownloadingNewData)
				isNeedUpdate = true;

			/*
			 * ������� ��������� ������ � ���
			 * 1. ���������� ��� ���� � ����� � 0
			 * 2. ���� ����� ���� � ���������� ��� � ������ - ������ ����
			 * 3. ���� ����� ���� � ����������, �� ������ ��������� "��"
			 * 4. ���� ����� ���� � ������, �� ������ ��������� "���" � �������� ������ "��"
			 * 
			 * 
			                     ������ ��� �����               ����� ���� � ����������               ����� � ������  
			 ����           0 (�� ���������� ��� �� �����)       100                                         100		
			 ���������              ��                           ��                                          ���			
		     �������� ������        ���                          ���                                          ��
			 
			*/
			
			var productVariantList = this._productService.GetProductVariants().ToList();
			
			var f5PriceList = this._f5PriceParserService.ParseAndShow(isNeedUpdate);
			var ostatkiPriceList = this._ostatkiPriceParserService.ParseAndShow(isNeedUpdate);
			var ugCatalogPriceList = this._yugCatalogPriceParserService.ParseAndShow(isNeedUpdate);
			var specialPriceList = _specialPriceService.GetAll(isNeedUpdate).ToList();

			var boyarkaCategoriesIds = GetUsedCategories(ostatkiPriceList.ProductLineList, productVariantList).Select(x => x.Id).ToList();

			var totalProducts = productVariantList.Count;
			var productsToSell = 0;
			var productsToSellInBoyarka = 0;
			var counterProductInSpecialPrice = 0;


			this._logger.Debug("     Updating products...");

			foreach (var currentProductVariant in productVariantList)
			{
				var productInF5Price = f5PriceList.ProductLineList.SingleOrDefault(x => x.Articul == currentProductVariant.Sku);
				var isProductInF5Price = productInF5Price != null;

				if (!isProductInF5Price)
				{
					// 1. ���������� ��� ���� � ����� � 0	
					if (currentProductVariant.Price != 0 || currentProductVariant.Published != false)
					{
						currentProductVariant.Price = 0;
						currentProductVariant.Published = false;
						currentProductVariant.AdminComment = "";
					}
					continue;
				}

				var productInBoyarka = ostatkiPriceList.ProductLineList.SingleOrDefault(x => x.Articul == currentProductVariant.Sku);
				var isProductInBoyarkaPrice = productInBoyarka != null;

				var productInVendor = ugCatalogPriceList.ProductLineList.SingleOrDefault(x => x.Articul == currentProductVariant.Sku);
				var isProductInVendorPrice = productInVendor != null;

				var productInSpecialPrice = specialPriceList.SingleOrDefault(x => currentProductVariant.Name.Contains(x.ProductName));
				var isProductInSpecialPrice = productInSpecialPrice != null;
				if (isProductInSpecialPrice)
				{
					counterProductInSpecialPrice++;
				} 


				//  2. ���� ����� ���� � ���������� ��� � ������ - ������ ����				
				if (isProductInVendorPrice || isProductInBoyarkaPrice)
				{
					decimal appliedTax;
					string priceCalcInfo;

					var productCategoryId = currentProductVariant.Product.ProductCategories.ToList()[0].CategoryId;

					var price = CalculatePrice(
						productCategoryId, 
						productInVendor, isProductInVendorPrice,
						productInF5Price, 
						productInBoyarka, isProductInBoyarkaPrice,
						productInSpecialPrice, isProductInSpecialPrice,
						boyarkaCategoriesIds,
						out appliedTax,
						out priceCalcInfo);

					if (currentProductVariant.Price != price || currentProductVariant.Published != true || currentProductVariant.AdminComment != priceCalcInfo)
					{
						currentProductVariant.Price = price;
						currentProductVariant.Published = true;
						currentProductVariant.AdminComment = priceCalcInfo;
					}
					productsToSell++;
				}
				else
				{
					// 1. ���������� ��� ���� � ����� � 0	
					if (currentProductVariant.Price != 0 || currentProductVariant.Published != false)
					{
						currentProductVariant.Price = 0;
						currentProductVariant.Published = false;
						currentProductVariant.AdminComment = "";
					}
					continue;
				}

				// 3. ���� ����� ���� � ����������, �� ������ ��������� "��"
				if (isProductInVendorPrice)
				{
					if(currentProductVariant.AvailableForPreOrder != true)
						currentProductVariant.AvailableForPreOrder = true;
				}

				// 4. ���� ����� ���� � ������, �� ������ ��������� "���" � �������� ������ "��"
				if (isProductInBoyarkaPrice)
				{
					if (currentProductVariant.AvailableForPreOrder != false)
						currentProductVariant.AvailableForPreOrder = false;

					productsToSellInBoyarka++;
				}

			}// END FOR

			if (productVariantList.Count > 0) // �������� ���������� ����� ��������� (���� ���������)
				this._productService.UpdateProductVariant(productVariantList[0]);

			var msgResult =	  "      totalProducts: " + totalProducts 
							+ ", productsToSell: " + productsToSell
			                + ", productsToSellInBoyarka: " + productsToSellInBoyarka
							+ ", productsInSpecialPrice: " + counterProductInSpecialPrice;

			this._logger.Debug(msgResult);

			this._logger.Debug("+++++ ApplyPriceImportAll end");

			return msgResult;
		}

		private decimal CalculatePrice(int productCategoryId, 
						ProductLine productInVendor, bool isProductInVendorPrice,
						ProductLineVendor productInF5Price,
						ProductLine productInBoyarka, bool isProductInBoyarkaPrice,
						SpecialPrice productInSpecialPrice, bool isProductInSpecialPrice,
						IEnumerable<int> boyarkaCategoriesIds,
						out decimal appliedTaxPercent,
						out string priceCalcInfo)
		{
			decimal returnPrice = 0;
			

			decimal onePercentPrice = productInF5Price.PriceBase / 100;
			decimal f5Percent = productInF5Price.PriceDiff / onePercentPrice;
			f5Percent = decimal.Round(f5Percent, 2, MidpointRounding.AwayFromZero);

			const decimal MyDiscontMnozhitelForCategoriesInBoyarka = 0.66m; // ��������� �� ���� � ��������� �������� (��������� �� �������� �������)
			const decimal MyDiscontMnozhitelForCategoriesInBoyarkaForNotExistProducts = 0.45m; // ��������� �� ���� � ��������� �������� (��������� �� �������� �������)
			const decimal MyPercentsForCategoriesNotInBoyarka = 5; // ��� ������� ������� ��� �������, �������� ������ �� ����������
			const decimal MyNatashasFeePercent = 2;// ������� ������� ���������, ������� ����������� ������
			const decimal MyMinFee = 15;
						
			appliedTaxPercent = 0;

			bool isboyarkaCategory = true;
			var appliedRule = "�� ���� ������� �� ���������. ������.";

			// ����� ���� � ������?
			if (isProductInBoyarkaPrice)
			{
				appliedTaxPercent = f5Percent * MyDiscontMnozhitelForCategoriesInBoyarka;
				appliedRule = "��������� �������: ����� ���� � ������, �������� ������� �� ���������� " + MyDiscontMnozhitelForCategoriesInBoyarka.ToString("0.00");
			}
			else if(isProductInVendorPrice) // ������ ��� � �����, �� ���� � �����
			{				
				isboyarkaCategory = boyarkaCategoriesIds.Contains(productCategoryId);

				// ���� ������ ���, �� �������� ��������� �������� (�����)
				if (isboyarkaCategory)
				{
					appliedTaxPercent = f5Percent * MyDiscontMnozhitelForCategoriesInBoyarkaForNotExistProducts;
					appliedRule = "��������� �������: ���� ������ ���, �� �������� ��������� ��������, �������� ������� �� ���������� " + MyDiscontMnozhitelForCategoriesInBoyarkaForNotExistProducts.ToString("0.00");
				}
				else // �������� ��������� � ������ �� ��������� (������������)
				{
					appliedTaxPercent = MyPercentsForCategoriesNotInBoyarka;
					appliedRule = "��������� �������: �������� ��������� � ������ �� ���������, ������������� ������� " + MyPercentsForCategoriesNotInBoyarka.ToString("0.00") + "%";
				}
			}
			else
				throw new Exception("my Strange situation");


			if (isProductInSpecialPrice)
			{
				// ����� ����� ��������������� ���� �������. ����������� ������� �������� �� �������� ����.
				appliedTaxPercent = (productInSpecialPrice.ProductPrice - productInF5Price.PriceBase) / onePercentPrice;
				appliedRule = "��������� ������� ��������������� ���� �������������";
			}


			// ��������� ���� c ������ �������
			var totalFee = productInF5Price.PriceBase * (appliedTaxPercent / 100);

			returnPrice = productInF5Price.PriceBase + totalFee;


			var natashasFee = productInF5Price.PriceBase * (MyNatashasFeePercent / 100); // ��� 2 �������� �� �������� ��������� 
			var myFee = totalFee - natashasFee;

			
			// ���� ��� ������� ������ ������, �� ����������� ���� � ������������ ����� ������� appliedTax
			if (myFee < MyMinFee)
			{
				appliedRule = "��������� ������� ���� ����������� ������� � " + MyMinFee.ToString("0.00 ���");
				
				var myFeePercent = MyMinFee / onePercentPrice;
				appliedTaxPercent = myFeePercent + MyNatashasFeePercent;

				totalFee = productInF5Price.PriceBase * (appliedTaxPercent / 100);
				myFee = totalFee - natashasFee;

				returnPrice = productInF5Price.PriceBase + totalFee;	
			
			}


			priceCalcInfo = 
				  "----- ������������ ���� ��������� ------ <br/>"

				+ "������� ������ = " + MyNatashasFeePercent.ToString("0.00") + "%<br/>"
				+ "��� ��������� �� ������� ������������ �� ������� F5 ��� ������������� ������� = " + MyDiscontMnozhitelForCategoriesInBoyarka.ToString("0.00") + "<br/><br/>"
				+ "��� ��������� �� ������� ������������ �� ������� F5 ��� ������������ ������ ������� = " + MyDiscontMnozhitelForCategoriesInBoyarkaForNotExistProducts.ToString("0.00") + "<br/><br/>"
				+ "������� ��� ������������� � ������ ������ ��������� ������� = " + MyPercentsForCategoriesNotInBoyarka.ToString("0.00") + "%<br/>"
				+ "��� ����������� ������� �� ������ = " + MyMinFee.ToString("0.00 ���") + "<br/>"


				+ "<br/>----- ��� �� ����� � ������  ------ <br/>"
				
				+ "���� � ������ = " + isProductInBoyarkaPrice + "<br/>"				
				+ "��������� ��������� � ������ = " + isboyarkaCategory + "<br/>"
				+ "���������� ���� = " + productInF5Price.PriceBase.ToString("0.00 ���") + "<br/>"
				+ "��������� ���� F5 = " + productInF5Price.PriceRaschet.ToString("0.00 ���") + "<br/>"
				+ "������� ������������ �� ������� F5= " + f5Percent.ToString("0.00") + "%<br/>"
				+ "���� ����� � ��������������� �����? = " + isProductInSpecialPrice + "<br/>"
				+ (isProductInSpecialPrice ? "��������������� ���� = " + productInSpecialPrice.ProductPrice.ToString("0.00 ���") + "<br/>" : "")


				+ "<br/>----- ��������� ���� � ��������  ------ <br/>"
																
				+ appliedRule + "<br/>"				
				+ "����������� ������� ��� ����� ������ = " + appliedTaxPercent.ToString("0.00") + "%<br/>"
				+ "����� ������� = " + totalFee.ToString("0.00 ���") + "<br/>" 
				+ "������� ������ = " + natashasFee.ToString("0.00 ���") + "<br/>"
				+ "��� ������� = " + myFee.ToString("0.00 ���") + "<br/>";

			return returnPrice;
		}
	}
}
