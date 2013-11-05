namespace Nop.Services.SiteParsers
{
	using System;

	using Nop.Core.Infrastructure;
	using Nop.Services.Logging;
	using Nop.Services.SiteParsers.Xls;
	using Nop.Services.Tasks;

	public partial class ParsePricesTask : ITask
    {
		private readonly IPriceManagerService _priceManagerService;

		public ParsePricesTask(IPriceManagerService priceManagerService)
        {
			_priceManagerService = priceManagerService;
        }

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
		{

			var scheduleTaskService = EngineContext.Current.Resolve<IScheduleTaskService>();
	        var name = typeof(ParsePricesTask).FullName + ", Nop.Services";
			var scheduleTask = scheduleTaskService.GetTaskByType(name);
	        var isNeed = false;

	        if (scheduleTask.LastSuccessUtc != null)
	        {
				if (scheduleTask.LastSuccessUtc.Value.Date < DateTime.UtcNow.Date)
				{
					isNeed = true; //daily update
		        }
				else
				{
					// int x = 0;
					// Skip because today already was successfull update
				}
	        }
	        else
	        {
				isNeed = true; // first time update
	        }

			if (isNeed)
			{
				var log = EngineContext.Current.Resolve<ILogger>();
				log.Debug("Start ParsePricesTask...");
				_priceManagerService.ApplyPriceDownloadAll();
				_priceManagerService.ApplyImportAll();
				log.Debug("End ParsePricesTask.");
			}
		}
    }
}
