namespace Nop.Services.YandexMarket
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Nop.Services.Configuration;
	using Nop.Services.SiteParsers.Page;
	
	/// <summary>
	/// Tax rate service
	/// </summary>
	public partial class SpecialPriceService : ISpecialPriceService
	{
		private readonly ISettingService _settingService;

		private const string PrefixTemplate = "specialpriceservice.";

		private const char Separator = '|';

		#region Ctor


		public SpecialPriceService(ISettingService settingService)
		{
			this._settingService = settingService;
		}

		#endregion

		#region Methods

		public void DownloadNewSpecialPriceToCache()
		{
			var parser = new SpecialPriceParser();
			var result = parser.Parse().ToList();

			foreach (var specialPrice in result)
			{
				this.Insert(specialPrice);
			}			
		}

		public virtual IEnumerable<SpecialPrice> GetAll(bool isUpdateCacheFromInternet)
		{
			if (isUpdateCacheFromInternet)
			{
				DownloadNewSpecialPriceToCache();
			}

			var list = _settingService.GetAllSettings().Where(x => x.Name.Contains(PrefixTemplate));
			var resultList = new List<SpecialPrice>();

			foreach (var curItem in list)
			{
				var arr = curItem.Value.Split(Separator);
				var newPrice = new SpecialPrice()
					{
						ProductName = arr[0], 
						ProductPrice = decimal.Parse(arr[1])
					};

				resultList.Add(newPrice);
			}

			return resultList;
		}

		/// <summary>
		/// Inserts a tax rate
		/// </summary>
		/// <param name="specialPrice">Tax rate</param>
		public virtual void Insert(SpecialPrice specialPrice)
		{
			if (specialPrice == null)
				throw new ArgumentNullException("specialPrice");

			var key = PrefixTemplate + specialPrice.ProductName;
			if (this._settingService.GetAllSettings().SingleOrDefault(x => x.Name == key) == null)
				_settingService.SetSetting(key, specialPrice.ProductName + Separator + specialPrice.ProductPrice);			
		}

		
		#endregion
	}
	
	
}
