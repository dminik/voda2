namespace Nop.Services.SiteParsers
{
	using System;

	using Nop.Core.Infrastructure;
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

	        if (scheduleTask.LastSuccessUtc != null)
	        {
				if (scheduleTask.LastSuccessUtc.Value.Date < DateTime.UtcNow.Date)
		        {
			       _priceManagerService.ApplyImportAll(false); //daily update
		        }
				else
				{
					// int x = 0;
					// Skip because today already was successfull update
				}
	        }
	        else
	        {
				_priceManagerService.ApplyImportAll(false);// first time update
	        }
		}
    }
}
