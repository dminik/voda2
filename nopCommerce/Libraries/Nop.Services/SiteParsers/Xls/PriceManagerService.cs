namespace Nop.Services.SiteParsers.Xls
{
	using System;
	using System.Linq;

	using Nop.Core.Infrastructure;
	using Nop.Services.Catalog;
	using Nop.Services.Logging;
	using Nop.Services.Tasks;

	public class PriceManagerService : IPriceManagerService
	{
		private readonly IProductService _productService;
		private readonly ILogger _logger;
		private readonly IOstatkiPriceParserService _ostatkiPriceParserService;
		private readonly IF5PriceParserService _f5PriceParserService;
		private readonly IYugCatalogPriceParserService _yugCatalogPriceParserService;

		public PriceManagerService(IProductService productService, 
			ILogger logger, 
			IOstatkiPriceParserService ugContractPriceParserService,
			IF5PriceParserService f5PriceParserService,
			 IYugCatalogPriceParserService yugCatalogPriceParserService)			
		{
			this._productService = productService;
			this._logger = logger;
			this._ostatkiPriceParserService = ugContractPriceParserService;
			this._f5PriceParserService = f5PriceParserService;
			this._yugCatalogPriceParserService = yugCatalogPriceParserService;
		}
		
		public string ApplyImportAll()
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


			var totalProducts = productVariantList.Count;
			var productsToSell = 0;
			var productsToSellInBoyarka = 0;

			foreach (var currentProductVariant in productVariantList)
			{
				var productInVendor = ugCatalogPriceList.ProductLineList.SingleOrDefault(x => x.Articul == currentProductVariant.Sku);
				var isProductInVendor = productInVendor != null;

				var productInBoyarka = ostatkiPriceList.ProductLineList.SingleOrDefault(x => x.Articul == currentProductVariant.Sku);
				var isProductInBoyarka = productInBoyarka != null;

				var productInF5Price = f5PriceList.ProductLineList.SingleOrDefault(x => x.Articul == currentProductVariant.Sku);
				var isProductInF5Price = productInF5Price != null;

				//  2. ���� ����� ���� � ���������� ��� � ������ - ������ ����				
				if ((isProductInVendor || isProductInBoyarka) && isProductInF5Price)
				{
					var price = productInF5Price.Price > 5 ? productInF5Price.Price : 3; // ������ ������� 3 ������ ���� �� ������					
					currentProductVariant.Price = price;
					productsToSell++;
				}
				else
				{
					// 1. ���������� ��� ���� � ����� � 0					
					currentProductVariant.Price = 0;
					continue;
				}

				// 3. ���� ����� ���� � ����������, �� ������ ��������� "��"
				if (isProductInVendor)
					currentProductVariant.AvailableForPreOrder = true;

				// 4. ���� ����� ���� � ������, �� ������ ��������� "���" � �������� ������ "��"
				if (isProductInBoyarka)
				{
					currentProductVariant.AvailableForPreOrder = false;
					productsToSellInBoyarka++;
				}

			}// END FOR

			if (productVariantList.Count > 0) // �������� ���������� ����� ��������� (���� ���������)
				this._productService.UpdateProductVariant(productVariantList[0]);

			var msgResult =	  "      totalProducts: " + totalProducts 
							+ ", productsToSell: " + productsToSell
			                + ", productsToSellInBoyarka: " + productsToSellInBoyarka;

			this._logger.Debug(msgResult);

			this._logger.Debug("+++++ ApplyPriceImportAll end");

			return msgResult;
		}
	}
}
