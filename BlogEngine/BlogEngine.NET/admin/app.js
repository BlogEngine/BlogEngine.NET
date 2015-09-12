(function () {
    var app = angular.module("blogAdmin", ['ngRoute', 'ngSanitize']);

    var config = ["$routeProvider", function ($routeProvider) {
        $routeProvider
        .when("/", { templateUrl: "views/dashboard.html" })
        
        .when("/content/posts", { templateUrl: "views/content/posts.html" })
        .when("/content/blogs", { templateUrl: "views/content/blogs.html" })
        .when("/content/comments", { templateUrl: "views/content/comments.html" })
        .when("/content/pages", { templateUrl: "views/content/pages.html" })
        .when("/content/categories", { templateUrl: "views/content/categories.html" })
        .when("/content/tags", { templateUrl: "views/content/tags.html" })
        .when("/content/filters", { templateUrl: "views/content/filters.html" })

        .when("/custom/plugins", { templateUrl: "views/custom/plugins.html" })
        .when("/custom/themes", { templateUrl: "views/custom/themes.html" })
        .when("/custom/widgets", { templateUrl: "views/custom/widgets.html" })
        .when("/custom/gallery", { templateUrl: "views/custom/gallery.html" })
        .when("/custom/package", { templateUrl: "views/custom/package.html" })

        .when("/security/profile", { templateUrl: "views/security/profile.html" })
        .when("/security/roles", { templateUrl: "views/security/roles.html" })
        .when("/security/users", { templateUrl: "views/security/users.html" })

        .when("/settings/basic", { templateUrl: "views/settings/basic.html" })
        .when("/settings/feed", { templateUrl: "views/settings/feed.html" })
        .when("/settings/email", { templateUrl: "views/settings/email.html" })
        .when("/settings/comments", { templateUrl: "views/settings/comments.html" })
        .when("/settings/controls", { templateUrl: "views/settings/controls.html" })
        .when("/settings/advanced", { templateUrl: "views/settings/advanced.html" })

        .when("/settings/theme", { templateUrl: "views/settings/theme.html" })

        .when("/settings/controls/blogroll", { templateUrl: "views/settings/controls/blogroll.html" })
        .when("/settings/controls/pings", { templateUrl: "views/settings/controls/pings.html" })

        .when("/settings/tools", { templateUrl: "views/settings/tools/check.html" })

        .when("/help", { templateUrl: "views/help/index.html" })
        .when("/about", { templateUrl: "views/about/index.html" })

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

    app.directive("owlCarousel", function() {
        return {
            restrict: 'E',
            transclude: false,
            link: function (scope) {
                scope.initCarousel = function(element) {
                    var defaultOptions = { };
                    var customOptions = scope.$eval($(element).attr('data-options'));
                    for(var key in customOptions) {
                        defaultOptions[key] = customOptions[key];
                    }
                    $(element).owlCarousel(defaultOptions);
                };
            }
        };
    });
    app.directive('owlCarouselItem', [function() {
        return {
            restrict: 'A',
            transclude: false,
            link: function(scope, element) {
                if(scope.$last) {
                    scope.initCarousel(element.parent());
                }
            }
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