using BlogEngine.Core;
using BlogEngine.Core.Data;
using BlogEngine.Core.Data.Contracts;
using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Optimization;
using System.Web.UI;
using BlogEngine.NET.AppCode.Api;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;

namespace BlogEngine.NET.App_Start
{
    public class BlogEngineConfig
    {
        private static bool _initializedAlready = false;
        private readonly static object _SyncRoot = new Object();

        public static void Initialize(HttpContext context)
        {
            if (_initializedAlready) { return; }

            lock (_SyncRoot)
            {
                if (_initializedAlready) { return; }

                Utils.LoadExtensions();

                RegisterBundles(BundleTable.Bundles);

                RegisterWebApi(GlobalConfiguration.Configuration);

                RegisterDiContainer();

                ScriptManager.ScriptResourceMapping.AddDefinition("jquery",
                    new ScriptResourceDefinition
                    {
                        Path = "~/Scripts/jquery-2.1.4.min.js",
                        DebugPath = "~/Scripts/jquery-2.1.4.js",
                        CdnPath = "http://ajax.microsoft.com/ajax/jQuery/jquery-2.1.4.min.js",
                        CdnDebugPath = "http://ajax.microsoft.com/ajax/jQuery/jquery-2.1.4.js"
                    });

                _initializedAlready = true;
            }
        }

        public static void SetCulture(object sender, EventArgs e)
        {
            var culture = BlogSettings.Instance.Culture;
            if (!string.IsNullOrEmpty(culture) && !culture.Equals("Auto"))
            {
                CultureInfo defaultCulture = Utils.GetDefaultCulture();
                Thread.CurrentThread.CurrentUICulture = defaultCulture;
                Thread.CurrentThread.CurrentCulture = defaultCulture;
            }
        }

        static void RegisterBundles(BundleCollection bundles)
        {
            // for anonymous users
            bundles.Add(new StyleBundle("~/Content/Auto/css").Include(
                "~/Content/Auto/*.css")
            );
            bundles.Add(new ScriptBundle("~/Scripts/Auto/js").Include(
                "~/Scripts/Auto/*.js")
            );

            // for authenticated users
            bundles.Add(new StyleBundle("~/Content/Auto/cssauth").Include(
                "~/Content/Auto/*.css")
            );
            bundles.Add(new ScriptBundle("~/Scripts/Auto/jsauth").Include(
                "~/Scripts/Auto/*.js")
            );

            // syntax highlighter 
            var shRoot = "~/scripts/syntaxhighlighter/";
            bundles.Add(new StyleBundle("~/Content/highlighter").Include(
                shRoot + "styles/shCore.css",
                shRoot + "styles/shThemeDefault.css")
            );
            bundles.Add(new ScriptBundle("~/Scripts/highlighter").Include(
                shRoot + "scripts/XRegExp.js",
                shRoot + "scripts/shCore.js",
                shRoot + "scripts/shAutoloader.js",
                shRoot + "shActivator.js")
            );

            // new admin bundles
            bundles.IgnoreList.Clear();
            AddDefaultIgnorePatterns(bundles.IgnoreList);

            bundles.Add(
                new StyleBundle("~/Content/admincss")
                .Include("~/Content/bootstrap.min.css")
                .Include("~/Content/toastr.css")
                .Include("~/Content/font-awesome.min.css")
                .Include("~/Content/star-rating.css")
                );

            bundles.Add(
                new ScriptBundle("~/scripts/blogadmin")
                .Include("~/Scripts/jquery-{version}.js")
                .Include("~/Scripts/jquery.form.js")
                .Include("~/Scripts/jquery.validate.js")
                .Include("~/Scripts/jquery-ui.js")
                .Include("~/Scripts/toastr.js")
                .Include("~/Scripts/bootstrap.js")
                .Include("~/Scripts/moment.js")
                .Include("~/Scripts/Q.js")
                .Include("~/Scripts/angular.min.js")
                .Include("~/Scripts/angular-route.min.js")
                .Include("~/Scripts/angular-sanitize.min.js") 
                            
                .Include("~/admin/app/app.js")
                .Include("~/admin/app/listpager.js")
                .Include("~/admin/app/grid-helpers.js")
                .Include("~/admin/app/data-service.js")
                .Include("~/admin/app/editor/filemanagerController.js")
                .Include("~/admin/app/common.js")

                .Include("~/admin/app/dashboard/dashboardController.js")

                .Include("~/admin/app/content/blogs/blogController.js")
                .Include("~/admin/app/content/posts/postController.js")
                .Include("~/admin/app/content/pages/pageController.js")
                .Include("~/admin/app/content/tags/tagController.js")
                .Include("~/admin/app/content/categories/categoryController.js")
                .Include("~/admin/app/content/comments/commentController.js")
                .Include("~/admin/app/content/comments/commentFilters.js")

                .Include("~/admin/app/custom/plugins/pluginController.js")
                .Include("~/admin/app/custom/themes/themeController.js")
                .Include("~/admin/app/custom/widgets/widgetController.js")
                .Include("~/admin/app/custom/widgets/widgetGalleryController.js")

                .Include("~/admin/app/security/users/userController.js")
                .Include("~/admin/app/security/roles/roleController.js")
                .Include("~/admin/app/security/profile/profileController.js")

                .Include("~/admin/app/settings/settingController.js")
                .Include("~/admin/app/settings/tools/toolController.js")
                .Include("~/admin/app/settings/controls/blogrollController.js")
                .Include("~/admin/app/settings/controls/pingController.js")      
                );

            bundles.Add(
                new ScriptBundle("~/scripts/wysiwyg")
                .Include("~/scripts/jquery-{version}.js")
                .Include("~/scripts/jquery.form.js")
                .Include("~/scripts/jquery.validate.js")
                .Include("~/scripts/toastr.js")
                .Include("~/scripts/Q.js")  
                .Include("~/Scripts/angular.min.js")
                .Include("~/Scripts/angular-route.min.js")
                .Include("~/Scripts/angular-sanitize.min.js")                
                .Include("~/scripts/bootstrap.js")
                .Include("~/scripts/textext.js")
                .Include("~/scripts/moment.js")
                .Include("~/admin/app/app.js")
                .Include("~/admin/app/grid-helpers.js")
                .Include("~/admin/app/editor/editor-helpers.js")
                .Include("~/admin/app/editor/posteditorController.js")
                .Include("~/admin/app/editor/pageeditorController.js")
                .Include("~/admin/app/editor/filemanagerController.js")
                .Include("~/admin/app/common.js")
                .Include("~/admin/app/data-service.js")
                );

            if (BlogConfig.DefaultEditor == "~/admin/editors/bootstrap-wysiwyg/editor.cshtml")
            {
                bundles.GetBundleFor("~/scripts/wysiwyg").Include("~/admin/editors/bootstrap-wysiwyg/jquery.hotkeys.js");
                bundles.GetBundleFor("~/scripts/wysiwyg").Include("~/admin/editors/bootstrap-wysiwyg/bootstrap-wysiwyg.js");
                bundles.GetBundleFor("~/scripts/wysiwyg").Include("~/admin/editors/bootstrap-wysiwyg/editor.js");
            }
            if (BlogConfig.DefaultEditor == "~/admin/editors/tinymce/editor.cshtml")
            {
                // tinymce plugings won't load when compressed. added in post/page editors instead.
                //bundles.GetBundleFor("~/scripts/wysiwyg").Include("~/admin/editors/tinymce/tinymce.min.js");
                //bundles.GetBundleFor("~/scripts/wysiwyg").Include("~/admin/editors/tinymce/editor.js");
            }
            else
            {
                bundles.GetBundleFor("~/scripts/wysiwyg").Include("~/admin/editors/summernote/summernote.js");
                // change language here if needed
                //bundles.GetBundleFor("~/scripts/wysiwyg").Include("~/admin/editors/summernote/lang/summernote-ru-RU.js");
                bundles.GetBundleFor("~/scripts/wysiwyg").Include("~/admin/editors/summernote/editor.js");
            }
        }

        static void RegisterWebApi(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute("DefaultApiWithActionAndId", "api/{controller}/{action}/{id}");

            config.Filters.Add(new UnauthorizedAccessExceptionFilterAttribute());

            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Never;

            config.Services.Add(typeof(IExceptionLogger), new UnhandledExceptionLogger());
        }

        static void RegisterDiContainer()
        {
            var container = new Container();

            container.Register<ISettingsRepository, SettingsRepository>(Lifestyle.Transient);
            container.Register<IPostRepository, PostRepository>(Lifestyle.Transient);
            container.Register<IPageRepository, PageRepository>(Lifestyle.Transient);
            container.Register<IBlogRepository, BlogRepository>(Lifestyle.Transient);
            container.Register<IStatsRepository, StatsRepository>(Lifestyle.Transient);
            container.Register<IPackageRepository, PackageRepository>(Lifestyle.Transient);
            container.Register<ILookupsRepository, LookupsRepository>(Lifestyle.Transient);
            container.Register<ICommentsRepository, CommentsRepository>(Lifestyle.Transient);
            container.Register<ITrashRepository, TrashRepository>(Lifestyle.Transient);
            container.Register<ITagRepository, TagRepository>(Lifestyle.Transient);
            container.Register<ICategoryRepository, CategoryRepository>(Lifestyle.Transient);
            container.Register<ICustomFieldRepository, CustomFieldRepository>(Lifestyle.Transient);
            container.Register<IUsersRepository, UsersRepository>(Lifestyle.Transient);
            container.Register<IRolesRepository, RolesRepository>(Lifestyle.Transient);
            container.Register<IFileManagerRepository, FileManagerRepository>(Lifestyle.Transient);
            container.Register<ICommentFilterRepository, CommentFilterRepository>(Lifestyle.Transient);
            container.Register<IDashboardRepository, DashboardRepository>(Lifestyle.Transient);
            container.Register<IWidgetsRepository, WidgetsRepository>(Lifestyle.Transient);

            container.Verify();

            GlobalConfiguration.Configuration.DependencyResolver =
                new SimpleInjectorWebApiDependencyResolver(container);
        }

        static void AddDefaultIgnorePatterns(IgnoreList ignoreList)
        {
            if (ignoreList == null)
                throw new ArgumentNullException(nameof(ignoreList));

            ignoreList.Ignore("*.intellisense.js");
            ignoreList.Ignore("*-vsdoc.js");

            //ignoreList.Ignore("*.debug.js", OptimizationMode.WhenEnabled);
            //ignoreList.Ignore("*.min.js", OptimizationMode.WhenDisabled);
            //ignoreList.Ignore("*.min.css", OptimizationMode.WhenDisabled);
        }
    }
}