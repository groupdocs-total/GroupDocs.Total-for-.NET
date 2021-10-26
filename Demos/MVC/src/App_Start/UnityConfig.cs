using GroupDocs.Total.MVC.Products.Search.Domain;
using GroupDocs.Total.MVC.Products.Search.Domain.SingleIndex;
using GroupDocs.Total.MVC.Products.Search.Domain.ViewerCache;
using System.Web.Http;
using Unity;
using Unity.WebApi;

namespace GroupDocs.Total.MVC
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();

            container.RegisterType<ILogger, Logger>(TypeLifetime.Transient);
            container.RegisterType<ISearchService, SingleIndexService>(TypeLifetime.Singleton);
            container.RegisterType<Settings>(TypeLifetime.Singleton);
            container.RegisterType<DemoValidatingService>(TypeLifetime.Singleton);
            container.RegisterType<StorageService>(TypeLifetime.Singleton);
            container.RegisterType<DictionaryStorageService>(TypeLifetime.Singleton);
            container.RegisterType<DocumentStatusService>(TypeLifetime.Singleton);
            container.RegisterType<HtmlCacheService>(TypeLifetime.Singleton);
            container.RegisterType<IndexFactoryService>(TypeLifetime.Singleton);
            container.RegisterType<TaskQueueService>(TypeLifetime.Singleton);
            container.RegisterType<ReindexingService>(TypeLifetime.Singleton);

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}
