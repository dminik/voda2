namespace Nop.Admin.Controllers
{
	using System;
	using System.Web.Mvc;

	using Nop.Core.Infrastructure;
	using Nop.Services.Catalog;
	using Nop.Services.Logging;
	using Nop.Services.SiteParsers;
	using Nop.Services.Tasks;
	using Nop.Web.Framework.Controllers;


	[AdminAuthorize]
	public class OstatkiFileParserController : Controller
	{
		private readonly IProductService _productService;
		private readonly ILogger _logger;
		private readonly IUgContractPriceParserService _ugContractPriceParserService;
		private readonly IF5PriceParserService _f5PriceParserService;

		public OstatkiFileParserController(IProductService productService, ILogger logger, 
			IUgContractPriceParserService ugContractPriceParserService, IF5PriceParserService f5PriceParserService)
		{
			_productService = productService;
			_logger = logger;
			_ugContractPriceParserService = ugContractPriceParserService;
			_f5PriceParserService = f5PriceParserService;
		}

		public ActionResult OstatkiFileParser()
		{			
			return View();
		}
		
		[HttpPost]
		public ActionResult ParseAndShow()
		{
			var list = _ugContractPriceParserService.ParseAndShow(true);
			return Json(list);
		}

		[HttpPost]
		public ActionResult ApplyImportAll()
		{			
			var name = typeof(ParsePricesTask).FullName + ", Nop.Services";
			var scheduleTaskService = EngineContext.Current.Resolve<IScheduleTaskService>();
			var scheduleTask = scheduleTaskService.GetTaskByType(name);

			if (scheduleTask.LastSuccessUtc != null)
			{
				if (scheduleTask.LastSuccessUtc.Value.Date < DateTime.UtcNow.Date)
				{
					_f5PriceParserService.ApplyImport(true);
					_ugContractPriceParserService.ApplyImport(true);
				}
			}

			_f5PriceParserService.ApplyImport(true);
			_ugContractPriceParserService.ApplyImport(true);

			return Content("Success!");
		}

		[HttpPost]
		public ActionResult ApplyImport()
		{
			var msg = _ugContractPriceParserService.ApplyImport(true);
			return Content(msg);			
		}		
	}
}
