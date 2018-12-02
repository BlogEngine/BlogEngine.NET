using BlogEngine.Core;
using BlogEngine.Core.Helpers;
using BlogEngine.Core.Loggers;
using BlogEngine.Core.Web.HttpModules;
using System;
using System.Collections.Generic;
using System.Web;
using Unity;
using Unity.Lifetime;

// https://haacked.com/archive/2011/06/03/dependency-injection-with-asp-net-httpmodules.aspx/
[assembly: PreApplicationStartMethod(typeof(HttpModuleLoader), "Start")]
namespace BlogEngine.Core.Web.HttpModules
{
    public class HttpModuleLoader : IHttpModule
    {
        Lazy<IEnumerable<IHttpModule>> _modules
            = new Lazy<IEnumerable<IHttpModule>>(RetrieveModules);

        public static IAeLogger Logger { get; set; } = IocUtil.Current.Logger;

        public static void Start()
        {
            HttpApplication.RegisterModule(typeof(HttpModuleLoader));

            // BillKrat.2018.08.12 @IOC     HttpModules Registered
            IocUtil.Current.Container
                .RegisterType<IHttpModule, WwwSubDomainModule>("www", new ContainerControlledLifetimeManager())
                .RegisterType<IHttpModule, CompressionModule>("compress", new ContainerControlledLifetimeManager())
                .RegisterType<IHttpModule, ReferrerModule>("refer", new ContainerControlledLifetimeManager())
                .RegisterType<IHttpModule, UrlRewrite>("url",new ContainerControlledLifetimeManager())
                .RegisterType<IHttpModule, Security>("security", new ContainerControlledLifetimeManager())
                .RegisterType<IHttpModule, Right>("right", new ContainerControlledLifetimeManager())
                ;
        }

        private static IEnumerable<IHttpModule> RetrieveModules()
        {
            // BillKrat.2018.08.12 @IOC     HttpModules Resolved
            var modules = IocUtil.Current.Container.ResolveAll<IHttpModule>();
            return modules;
        }

        public void Dispose()
        {
            var modules = _modules.Value;
            foreach (var module in modules)
            {
                var disposableModule = module as IDisposable;
                if (disposableModule != null)
                {
                    disposableModule.Dispose();
                }
            }
        }

        public void Init(HttpApplication context)
        {
            Logger.LogDebug($"HTTP-MODULE: {GetType().Name}.Init(context) - initializing registered HttpModules");

            var modules = _modules.Value;
            foreach (var module in modules)
            {
                module.Init(context);
            }
        }
    }
}