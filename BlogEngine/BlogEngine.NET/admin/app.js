(function () {
    var app = angular.module("blogAdmin", ['ngRoute', 'ngAnimate', 'ngSanitize']);

    var config = ["$routeProvider", function ($routeProvider) {
        $routeProvider
        .when("/", { templateUrl: "views/dashboard.html" })
        .when("/blogs", { templateUrl: "views/blogs.html" })

        .when("/content", { templateUrl: "views/content/posts.html" })
        .when("/content/comments", { templateUrl: "views/content/comments.html" })
        .when("/content/pages", { templateUrl: "views/content/pages.html" })
        .when("/content/categories", { templateUrl: "views/content/categories.html" })
        .when("/content/tags", { templateUrl: "views/content/tags.html" })
        .when("/content/files", { templateUrl: "views/content/files.html" })

        .when("/custom", { templateUrl: "views/custom/index.html" })
        .when("/custom/themes", { templateUrl: "views/custom/themes.html" })
        .when("/custom/widgets", { templateUrl: "views/custom/widgets.html" })
        .when("/custom/packages", { templateUrl: "views/custom/packages.html" })

        .when("/users", { templateUrl: "views/users/index.html" })
        .when("/users/roles", { templateUrl: "views/users/roles.html" })
        .when("/users/profile", { templateUrl: "views/users/profile.html" })

        .when("/settings", { templateUrl: "views/settings/basic.html" })
        .when("/settings/advanced", { templateUrl: "views/settings/advanced.html" })
        .when("/settings/feed", { templateUrl: "views/settings/feed.html" })
        .when("/settings/email", { templateUrl: "views/settings/email.html" })

        .when("/settings/controls", { templateUrl: "views/settings/controls/contactform.html" })
        .when("/settings/controls/search", { templateUrl: "views/settings/controls/search.html" })
        .when("/settings/controls/recentposts", { templateUrl: "views/settings/controls/recentposts.html" })
        .when("/settings/controls/recentcomments", { templateUrl: "views/settings/controls/recentcomments.html" })
        .when("/settings/controls/blogroll", { templateUrl: "views/settings/controls/blogroll.html" })
        .when("/settings/controls/pings", { templateUrl: "views/settings/controls/pings.html" })
        .when("/settings/controls/error", { templateUrl: "views/settings/controls/error.html" })

        .when("/settings/comments", { templateUrl: "views/settings/comments/index.html" })
        .when("/settings/comments/filters", { templateUrl: "views/settings/comments/filters.html" })

        .when("/settings/customecode", { templateUrl: "views/settings/customecode.html" })

        .when("/settings/tools", { templateUrl: "views/settings/tools/check.html" })
        .when("/settings/tools/export", { templateUrl: "views/settings/tools/export.html" })
        .when("/settings/tools/import", { templateUrl: "views/settings/tools/import.html" })
        .otherwise({ redirectTo: "/" });
    }];
    app.config(config);

    app.directive('focusMe', ['$timeout', function ($timeout) {
        return function (scope, element, attrs) {
            scope.$watch(attrs.focusMe, function (value) {
                if (value) {
                    $timeout(function () {
                        element.focus();
                    }, 700);
                }
            });
        };
    }]);

    var run = ["$rootScope", "$log", function ($rootScope, $log) {

        $rootScope.lbl = BlogAdmin.i18n;
        $rootScope.SiteVars = SiteVars;
        $rootScope.security = new security();
        toastr.options.positionClass = 'toast-bottom-right';
        toastr.options.backgroundpositionClass = 'toast-bottom-right';
    }];

    app.run(run);

    var security = function () {
        // dashboard
        this.showTabDashboard = showTabDashboard();
        this.viewErrorMessages = viewErrorMessages();
        function showTabDashboard() { return UserVars.Rights.indexOf("ViewDashboard") > -1 ? true : false; }
        function viewErrorMessages() { return UserVars.Rights.indexOf("ViewDetailedErrorMessages") > -1 ? true : false; }

        // blogs
        this.showTabBlogs = showTabBlogs();
        function showTabBlogs() { return (SiteVars.IsPrimary == "True" && UserVars.IsAdmin == "True") ? true : false; }

        // content
        this.showTabContent = showTabContent();
        function showTabContent() { return UserVars.Rights.indexOf("EditOwnPosts") > -1 ? true : false; }

        // customization/packaging
        this.showTabCustom = showTabCustom();
        this.canManageExtensions = canManageExtensions();
        this.canManageThemes = canManageThemes();
        this.canManageWidgets = canManageWidgets();
        this.canManagePackages = canManagePackages();

        function showTabCustom() {
            return (UserVars.Rights.indexOf("ManageExtensions") > -1 ||
                UserVars.Rights.indexOf("ManageWidgets") > -1 ||
                UserVars.Rights.indexOf("ManageThemes") > -1 ||
                UserVars.Rights.indexOf("ManagePackages") > -1) ? true : false;
        }
        function canManageExtensions() { return UserVars.Rights.indexOf("ManageExtensions") > -1; }
        function canManageThemes() { return UserVars.Rights.indexOf("ManageThemes") > -1; }
        function canManageWidgets() { return UserVars.Rights.indexOf("ManageWidgets") > -1; }
        function canManagePackages() { return UserVars.Rights.indexOf("ManagePackages") > -1; }

        // users
        this.showTabUsers = showTabUsers();
        this.canManageUsers = canManageUsers();
        this.canManageRoles = canManageRoles();
        this.canManageProfile = canManageProfile();

        function showTabUsers() { return (UserVars.Rights.indexOf("EditOtherUsers") > -1) ? true : false; }
        function canManageUsers() { return UserVars.Rights.indexOf("EditOtherUsers") > -1 ? true : false; }
        function canManageRoles() { return UserVars.Rights.indexOf("EditRoles") > -1 ? true : false; }
        function canManageProfile() { return UserVars.Rights.indexOf("EditOwnUser") > -1 ? true : false; }

        // settings
        this.showTabSettings = showTabSettings();
        function showTabSettings() { return UserVars.Rights.indexOf("AccessAdminSettingsPages") > -1 ? true : false; }
    }
})();