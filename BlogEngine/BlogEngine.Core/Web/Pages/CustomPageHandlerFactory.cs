using BlogEngine.Core.Helpers;
using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using Unity;

namespace BlogEngine.Core.Web.Pages
{
    // https://stackoverflow.com/questions/589374/how-to-use-dependency-injection-with-asp-net-web-forms
    public class CustomPageHandlerFactory : PageHandlerFactory
    {
        // BillKrat.2018.08.12 @IOC Custom Pages

        public static IUnityContainer Container { get; set; } = IocUtil.Current.Container;

        private static object GetInstance(Type type)
        {
            return Container.Resolve(type);
        }

        public override IHttpHandler GetHandler(HttpContext cxt,
            string type, string vPath, string path)
        {
            var page = base.GetHandler(cxt, type, vPath, path);

            if (page == null)  return page;

            Type pageType = page.GetType().BaseType;

            // setter injection  BillKrat.2018.08.18 @IOC
            Container.BuildUp(page.GetType(), page);

            var ctor = GetInjectableCtor(pageType);

            // constructor injection 
            if (ctor != null)
            {
                var arguments =
                    (from parameter in ctor.GetParameters()
                        select GetInstance(parameter.ParameterType)).ToArray();

                ctor.Invoke(page, arguments);
            }
            return page;
        }


        private static ConstructorInfo GetInjectableCtor(Type type)
        {
            var overloadedPublicConstructors = (
                from constructor in type.GetConstructors()
                where constructor.GetParameters().Length > 0
                select constructor).ToArray();

            if (overloadedPublicConstructors.Length == 0)
            {
                return null;
            }

            if (overloadedPublicConstructors.Length == 1)
            {
                return overloadedPublicConstructors[0];
            }

            throw new Exception(string.Format(
                "The type {0} has multiple public " +
                "ctors and can't be initialized.", type));
        }
    }
    
}
