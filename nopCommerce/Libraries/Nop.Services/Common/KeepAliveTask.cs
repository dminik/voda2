﻿using System.Net;
using Nop.Core;
using Nop.Services.Stores;
using Nop.Services.Tasks;

namespace Nop.Services.Common
{
	/// <summary>
	/// Represents a task for keeping the site alive
	/// </summary>
	public partial class KeepAliveTask : ITask
	{
		private readonly IStoreContext _storeContext;

		public KeepAliveTask(IStoreContext storeContext)
		{
			this._storeContext = storeContext;
		}

		/// <summary>
		/// Executes a task
		/// </summary>
		public void Execute()
		{
			// dminikk
#if !DEBUG
			 string url = _storeContext.CurrentStore.Url + "keepalive/index";
#else
			string url = "http://localhost/keepalive/index";
#endif
			
			using (var wc = new WebClient())
			{
				wc.DownloadString(url);
			}
		}
	}
}
