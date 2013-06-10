namespace Nop.Plugin.Misc.YandexMarketParser
{
	using Autofac;
	using Autofac.Core;
	using Autofac.Integration.Mvc;

	using Nop.Core.Data;
	using Nop.Core.Infrastructure;
	using Nop.Core.Infrastructure.DependencyManagement;
	using Nop.Data;
	using Nop.Plugin.Misc.YandexMarketParser.Data;
	using Nop.Plugin.Misc.YandexMarketParser.Domain;
	using Nop.Plugin.Misc.YandexMarketParser.Services;

	public class DependencyRegistrar : IDependencyRegistrar
	{
		public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
		{
			builder.RegisterType<YandexMarketCategoryService>().As<IYandexMarketCategoryService>().InstancePerHttpRequest();

			//data layer
			var dataSettingsManager = new DataSettingsManager();
			var dataProviderSettings = dataSettingsManager.LoadSettings();

			if (dataProviderSettings != null && dataProviderSettings.IsValid())
			{
				//register named context
				builder.Register<IDbContext>(c => new YandexMarketParserObjectContext(dataProviderSettings.DataConnectionString))
					.Named<IDbContext>("nop_object_context_YandexMarketParserObjectContext")
					.InstancePerHttpRequest();

				builder.Register<YandexMarketParserObjectContext>(c => new YandexMarketParserObjectContext(dataProviderSettings.DataConnectionString))
					.InstancePerHttpRequest();
			}
			else
			{
				//register named context
				builder.Register<IDbContext>(c => new YandexMarketParserObjectContext(c.Resolve<DataSettings>().DataConnectionString))
					.Named<IDbContext>("nop_object_context_YandexMarketParserObjectContext")
					.InstancePerHttpRequest();

				builder.Register<YandexMarketParserObjectContext>(c => new YandexMarketParserObjectContext(c.Resolve<DataSettings>().DataConnectionString))
					.InstancePerHttpRequest();
			}

			//override required repository with our custom context
			builder.RegisterType<EfRepository<YandexMarketCategoryRecord>>()
				.As<IRepository<YandexMarketCategoryRecord>>()
				.WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_YandexMarketParserObjectContext"))
				.InstancePerHttpRequest();
		}

		public int Order
		{
			get { return 1; }
		}
	}
}
