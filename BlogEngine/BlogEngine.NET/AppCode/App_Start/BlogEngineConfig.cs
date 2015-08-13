using App_Code.Controls;
using ASP.App_Start;
using BlogEngine.Core;
using BlogEngine.Core.Data;
using BlogEngine.Core.Data.Contracts;
using Microsoft.Practices.Unity;
using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Optimization;
using System.Web.UI;
using BlogEngine.NET.AppCode.Api;

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

                WidgetZone.PreloadWidgetsAsync("be_WIDGET_ZONE");
                Utils.LoadExtensions();

                BlogEngineConfig.RegisterBundles(BundleTable.Bundles);

                BlogEngineConfig.RegisterWebApi(GlobalConfiguration.Configuration);

                RegisterUnity(GlobalConfiguration.Configuration);

                ScriptManager.ScriptResourceMapping.AddDefinition("jquery",
                    new ScriptResourceDefinition
                    {
                        Path = "~/Scripts/jquery-1.9.1.min.js",
                        DebugPath = "~/Scripts/jquery-1.9.1.js",
                        CdnPath = "http://ajax.microsoft.com/ajax/jQuery/jquery-1.9.1.min.js",
                        CdnDebugPath = "http://ajax.microsoft.com/ajax/jQuery/jquery-1.9.1.js"
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
                "~/Content/Auto/*.css",
                "~/Modules/QuickNotes/Qnotes.css")
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

            // syntax FileManager 
            bundles.Add(new StyleBundle("~/Content/filemanager").Include(
                "~/admin/FileManager/FileManager.css",
                "~/admin/uploadify/uploadify.css",
                "~/admin/FileManager/jqueryui/jquery-ui.css",
                "~/admin/FileManager/JCrop/css/jquery.Jcrop.css")
            );
            bundles.Add(new ScriptBundle("~/Scripts/filemanager").Include(
                "~/admin/uploadify/swfobject.js",
                "~/admin/uploadify/jquery.uploadify.v2.1.4.min.js",
                "~/admin/FileManager/jqueryui/jquery-ui.min.js",
                "~/admin/FileManager/jquery.jeegoocontext.min.js",
                "~/admin/FileManager/JCrop/js/jquery.Jcrop.min.js",
                "~/admin/FileManager/FileManager-mini.js")
            );

            // new admin bundles
            bundles.IgnoreList.Clear();
            AddDefaultIgnorePatterns(bundles.IgnoreList);

            bundles.Add(
                new StyleBundle("~/Content/admincss")
                .Include("~/Content/bootstrap.min.css")
                .Include("~/Content/toastr.css")
                .Include("~/Content/font-awesome.min.css")
                .Include("~/Content/editor.css")
                .Include("~/Admin/star-rating.css")
                .Include("~/Content/admin.css")
                );

            bundles.Add(
                new ScriptBundle("~/scripts/blogadmin")
                .Include("~/scripts/jquery-2.1.1.js")
                .Include("~/scripts/jquery.form.js")
                .Include("~/scripts/jquery.validate.js")
                .Include("~/scripts/toastr.js")
                .Include("~/scripts/Q.js")
                .Include("~/Scripts/angular.min.js")
                .Include("~/Scripts/angular-route.min.js")
                .Include("~/Scripts/angular-animate.min.js")
                .Include("~/Scripts/angular-sanitize.min.js")
                .Include("~/admin/be-grid.js")
                .Include("~/admin/app.js")
                .Include("~/admin/controllers/dashboard.js")
                .Include("~/admin/controllers/blogs.js")
                .Include("~/admin/controllers/help.js")
                .Include("~/admin/controllers/about.js")
                .Include("~/admin/controllers/posts.js")
                .Include("~/admin/controllers/listpager.js")
                .Include("~/admin/controllers/pages.js")
                .Include("~/admin/controllers/tags.js")
                .Include("~/admin/controllers/categories.js")
                .Include("~/admin/controllers/files.js")
                .Include("~/admin/controllers/comments.js")
                .Include("~/admin/controllers/users.js")
                .Include("~/admin/controllers/roles.js")
                .Include("~/admin/controllers/profile.js")
                .Include("~/admin/controllers/settings.js")
                .Include("~/admin/controllers/tools.js")
                .Include("~/admin/controllers/commentfilters.js")
                .Include("~/admin/controllers/blogroll.js")
                .Include("~/admin/controllers/pings.js")
                .Include("~/admin/controllers/custom.js")
                .Include("~/admin/controllers/package.js")
                .Include("~/admin/controllers/common.js")
                .Include("~/admin/services.js")
                .Include("~/scripts/bootstrap.js")
                .Include("~/scripts/moment.js")
                .Include("~/admin/editors/summernote/summernote.js")
                );

            bundles.Add(
                new ScriptBundle("~/scripts/wysiwyg")
                .Include("~/scripts/jquery-2.1.1.js")
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
                .Include("~/admin/app.js")
                .Include("~/admin/editor/editor.js")
                .Include("~/admin/editor/postcontroller.js")
                .Include("~/admin/editor/pagecontroller.js")
                .Include("~/admin/be-grid.js")
                .Include("~/admin/controllers/files.js")
                .Include("~/admin/controllers/common.js")
                .Include("~/admin/services.js")
                );

            if (BlogConfig.DefaultEditor == "~/admin/editors/bootstrap-wysiwyg/editor.cshtml")
            {
                bundles.GetBundleFor("~/scripts/wysiwyg").Include("~/admin/editors/bootstrap-wysiwyg/jquery.hotkeys.js");
                bundles.GetBundleFor("~/scripts/wysiwyg").Include("~/admin/editors/bootstrap-wysiwyg/bootstrap-wysiwyg.js");
                bundles.GetBundleFor("~/scripts/wysiwyg").Include("~/admin/editors/bootstrap-wysiwyg/editor.js");
            }
            if (BlogConfig.DefaultEditor == "~/admin/editors/tinymce/editor.cshtml")
            {
                // tinymce plugings will not load when scripts compressed, do nothing
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

        static void RegisterUnity(System.Web.Http.HttpConfiguration config)
        {
            var unity = new UnityContainer();

            unity.RegisterType<SettingsController>();
            unity.RegisterType<PostsController>();
            unity.RegisterType<PagesController>();
            unity.RegisterType<BlogsController>();
            unity.RegisterType<StatsController>();
            unity.RegisterType<PackagesController>();
            unity.RegisterType<LookupsController>();
            unity.RegisterType<CommentsController>();
            unity.RegisterType<TrashController>();
            unity.RegisterType<TagsController>();
            unity.RegisterType<CategoriesController>();
            unity.RegisterType<CustomFieldsController>();
            unity.RegisterType<UsersController>();
            unity.RegisterType<RolesController>();
            unity.RegisterType<FileManagerController>();
            unity.RegisterType<CommentFilterController>();

            unity.RegisterType<ISettingsRepository, SettingsRepository>(new HierarchicalLifetimeManager());
            unity.RegisterType<IPostRepository, PostRepository>(new HierarchicalLifetimeManager());
            unity.RegisterType<IPageRepository, PageRepository>(new HierarchicalLifetimeManager());
            unity.RegisterType<IBlogRepository, BlogRepository>(new HierarchicalLifetimeManager());
            unity.RegisterType<IStatsRepository, StatsRepository>(new HierarchicalLifetimeManager());
            unity.RegisterType<IPackageRepository, PackageRepository>(new HierarchicalLifetimeManager());
            unity.RegisterType<ILookupsRepository, LookupsRepository>(new HierarchicalLifetimeManager());
            unity.RegisterType<ICommentsRepository, CommentsRepository>(new HierarchicalLifetimeManager());
            unity.RegisterType<ITrashRepository, TrashRepository>(new HierarchicalLifetimeManager());
            unity.RegisterType<ITagRepository, TagRepository>(new HierarchicalLifetimeManager());
            unity.RegisterType<ICategoryRepository, CategoryRepository>(new HierarchicalLifetimeManager());
            unity.RegisterType<ICustomFieldRepository, CustomFieldRepository>(new HierarchicalLifetimeManager());
            unity.RegisterType<IUsersRepository, UsersRepository>(new HierarchicalLifetimeManager());
            unity.RegisterType<IRolesRepository, RolesRepository>(new HierarchicalLifetimeManager());
            unity.RegisterType<IFileManagerRepository, FileManagerRepository>(new HierarchicalLifetimeManager());
            unity.RegisterType<ICommentFilterRepository, CommentFilterRepository>(new HierarchicalLifetimeManager());

            config.DependencyResolver = new IoCContainer(unity);
        }

        static void AddDefaultIgnorePatterns(IgnoreList ignoreList)
        {
            if (ignoreList == null)
                throw new ArgumentNullException("ignoreList");

            ignoreList.Ignore("*.intellisense.js");
            ignoreList.Ignore("*-vsdoc.js");

            //ignoreList.Ignore("*.debug.js", OptimizationMode.WhenEnabled);
            //ignoreList.Ignore("*.min.js", OptimizationMode.WhenDisabled);
            //ignoreList.Ignore("*.min.css", OptimizationMode.WhenDisabled);
        }
    }
}