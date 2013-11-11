namespace Nop.Admin.Models.YandexMarket
{
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Web.Mvc;

	using Nop.Web.Framework.Mvc;

	public class OstatkiFileParserModel : BaseNopModel
	{
		public OstatkiFileParserModel()
		{
			
		}
		
		[DisplayName("Принудительно скачать новые данные с инета")]
		public bool IsForceDownloadingNewData { get; set; }		
	}
}