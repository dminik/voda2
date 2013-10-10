namespace Nop.Services.SiteParsers
{
	using System;

	using Nop.Core.Infrastructure;
	using Nop.Services.Tasks;

	public partial class ParsePricesTask : ITask
    {
		private readonly IOstatkiPriceParserService _ugContractPriceParserService;
		private readonly IF5PriceParserService _f5PriceParserService;

		public ParsePricesTask(IOstatkiPriceParserService ugContractPriceParserService, IF5PriceParserService f5PriceParserService)
        {
			_ugContractPriceParserService = ugContractPriceParserService;
			_f5PriceParserService = f5PriceParserService;
        }

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
		{

			var scheduleTaskService = EngineContext.Current.Resolve<IScheduleTaskService>();
	        var name = typeof(ParsePricesTask).FullName + ", Nop.Services";
			var scheduleTask = scheduleTaskService.GetTaskByType(name);

	        if (scheduleTask.LastSuccessUtc != null)
	        {
				if (scheduleTask.LastSuccessUtc.Value.Date < DateTime.UtcNow.Date)
		        {
			        _f5PriceParserService.SetVendorPrices(true);
			        _ugContractPriceParserService.SetExistingInBoyarka(true);
		        }
	        }
	        else
	        {
				_f5PriceParserService.SetVendorPrices(true);
				_ugContractPriceParserService.SetExistingInBoyarka(true);
	        }
		}
    }
}
