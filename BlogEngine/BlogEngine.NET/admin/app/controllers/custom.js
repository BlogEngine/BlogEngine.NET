
angular.module('blogAdmin').controller('CustomController', ["$rootScope", "$scope", "$location", "$filter", "dataService", function ($rootScope, $scope, $location, $filter, dataService) {
    $scope.items = [];
    $scope.customFields = [];
    $scope.editId = "";
    $scope.package = {};
    $scope.fltr = 'all';
    $scope.itemsPerPage = 20;
    $scope.root = $rootScope.SiteVars.ApplicationRelativeWebRoot;
    $scope.IsPrimary = $rootScope.SiteVars.IsPrimary == "True";
    $scope.security = $rootScope.security;

    $scope.id = ($location.search()).id;
    $scope.theme = ($location.search()).id;
    $scope.lst = ($location.search()).lst;
    $scope.galleryFilter = getFromQueryString('ftr');

    $scope.order = 'DownloadCount desc';
    $scope.sortingOrder = 'DownloadCount';
    $scope.reverse = true;

    $scope.showRating = false;
    $scope.selectedRating = 0;
    $scope.author = UserVars.Name;
    $scope.activeTheme = ActiveTheme;
    $scope.spin = true;
    
    if ($location.path().indexOf("/custom/plugins") == 0) {
        $scope.fltr = 'extensions';
    }
    if ($location.path().indexOf("/custom/themes") == 0) {
        $scope.fltr = 'themes';
    }
    if ($location.path().indexOf("/custom/widgets") == 0) {
        $scope.fltr = 'widgets';
    }

    if ($scope.lst && $scope.lst.length > 0) {
        $scope.fltr = $scope.lst;
    }

    $scope.load = function () {
        $scope.spin = true;
        dataService.getItems('/api/packages', { take: 0, skip: 0, filter: $scope.fltr, order: 'LastUpdated desc' })
        .success(function (data) {
            angular.copy(data, $scope.items);

            gridInit($scope, $filter);
            if ($scope.galleryFilter) {
                $scope.setFilter();
            }
            $scope.spin = false;

            var pkgId = getFromQueryString('pkgId');
            if (pkgId != null) {
                $scope.query = pkgId;
                $scope.search();
            }
        })
        .error(function () {
            toastr.error($rootScope.lbl.errorLoadingPackages);
            $scope.spin = false;
        });
    }

    $scope.loadCustomFields = function (id) {
        $scope.editId = id;
        //$scope.extEditSrc = SiteVars.RelativeWebRoot + "admin/Extensions/Settings.aspx?ext=" + id + "&enb=False";
        //$scope.customFields = [];

        //for (var i = 0, len = $scope.items.length; i < len; i++) {
        //    if ($scope.items[i].Id === id) {
        //        angular.copy($scope.items[i], $scope.package);

        //        if ($scope.package) {
        //            $scope.removeEmptyReviews();

        //            if($scope.package.SettingsUrl){
        //                $scope.extEditSrc = $scope.package.SettingsUrl.replace("~/", SiteVars.RelativeWebRoot);
        //            }
        //        }
        //    }
        //}
        dataService.getItems('/api/customfields', { filter: 'CustomType == "THEME" && ObjectId == "' + id + '"' })
        .success(function (data) {
            angular.copy(data, $scope.customFields);
            $("#modal-settings").modal();
        })
        .error(function () {
            toastr.error($rootScope.lbl.errorLoadingCustomFields);
        });
    }

    $scope.save = function () {
        $scope.spin = true;

        dataService.updateItem("/api/packages/update/foo", $scope.package)
        .success(function (data) {
            toastr.success($rootScope.lbl.completed);
            $scope.spin = false;
        })
        .error(function () {
            toastr.error($rootScope.lbl.failed);
            $scope.spin = false;
        });

        dataService.updateItem("/api/customfields", $scope.customFields)
        .success(function (data) {
            $scope.load();
            $scope.spin = false;
        })
        .error(function () {
            toastr.error($rootScope.lbl.updateFailed);
            $scope.spin = false;
        });
    }

    $scope.processChecked = function (action) {
        processChecked("/api/packages/processchecked/", action, $scope, dataService);
    }

    $scope.pkgLinkType = function (locVersion, galVersion) {
        if (locVersion === '') {
            return "download";
        }
        if (locVersion === galVersion) {
            return "installed";
        }
        if (locVersion < galVersion) {
            return "refresh";
        }
    }

    $scope.installPackage = function (pkgId) {
        $scope.spin = true;
        dataService.updateItem("/api/packages/install/" + pkgId, pkgId)
        .success(function (data) {
            toastr.success($rootScope.lbl.completed);
            $scope.load();
            $scope.spin = false;
        })
        .error(function () {
            toastr.error($rootScope.lbl.failed);
            $scope.spin = false;
        });
    }

    $scope.uninstallPackage = function (pkgId) {
        $scope.spin = true;
        dataService.updateItem("/api/packages/uninstall/" + pkgId, pkgId)
        .success(function (data) {
            toastr.success($rootScope.lbl.completed);
            $scope.load();
            $scope.spin = false;
        })
        .error(function () {
            toastr.error($rootScope.lbl.failed);
            $scope.spin = false;
        });
    }

    $scope.refreshGalleryList = function () {
        $scope.spin = true;
        dataService.updateItem("/api/packages/refresh/list", { })
        .success(function (data) {
            toastr.success($rootScope.lbl.completed);
            $scope.spin = false;
            $scope.load();
        })
        .error(function () {
            toastr.error($rootScope.lbl.failed);
            $scope.spin = false;
        });
    }

    $scope.setFilter = function () {
        if ($scope.galleryFilter == 'plugin') {
            $scope.gridFilter('PackageType', 'Extension', 'pub');
        }
        if ($scope.galleryFilter == 'theme') {
            $scope.gridFilter('PackageType', 'Theme', 'dft');
        }
    }

    $scope.setDefaultTheme = function (id) {
        $scope.spin = true;
        dataService.updateItem("/api/packages/settheme/" + id, id)
        .success(function (data) {
            ActiveTheme = id;
            $scope.activeTheme = id;
            toastr.success($rootScope.lbl.completed);
            $scope.load();
            $scope.spin = false;
        })
        .error(function () {
            toastr.error($rootScope.lbl.failed);
            $scope.spin = false;
        });
    }

    $scope.checkStar = function (item, rating) {
        if (item === rating) {
            return true;
        }
        return false;
    }

    $scope.sortBy = function (ord) {
        $scope.sortingOrder = ord;
        $scope.reverse = true;
        $scope.load();
    }

    $scope.showSettings = function (id) {
        $scope.loadCustomFields(id);   
    }

    $scope.showPluginSettings = function (id) {
        $scope.editId = id;
        $scope.extEditSrc = SiteVars.RelativeWebRoot + "admin/Extensions/Settings.aspx?ext=" + id + "&enb=False";

        for (var i = 0, len = $scope.items.length; i < len; i++) {
            if ($scope.items[i].Id === id) {
                angular.copy($scope.items[i], $scope.package);

                if ($scope.package) {
                    //$scope.removeEmptyReviews();

                    if($scope.package.SettingsUrl){
                        $scope.extEditSrc = $scope.package.SettingsUrl.replace("~/", SiteVars.RelativeWebRoot);
                    }
                }
            }
        }
        $("#modal-settings").modal();
    }

    $scope.showInfo = function (id) {
        for (var i = 0, len = $scope.items.length; i < len; i++) {
            if ($scope.items[i].Id === id) {
                angular.copy($scope.items[i], $scope.package);

                if ($scope.package) {
                    //$scope.removeEmptyReviews();

                    if($scope.package.SettingsUrl){
                        $scope.extEditSrc = $scope.package.SettingsUrl.replace("~/", SiteVars.RelativeWebRoot);
                    }
                }
            }
        }
        $("#modal-info").modal();
    }

    $scope.showPluginInfo = function (id) {
        for (var i = 0, len = $scope.items.length; i < len; i++) {
            if ($scope.items[i].Id === id) {
                angular.copy($scope.items[i], $scope.package);

                if ($scope.package) {
                    //$scope.removeEmptyReviews();

                    if ($scope.package.SettingsUrl) {
                        $scope.extEditSrc = $scope.package.SettingsUrl.replace("~/", SiteVars.RelativeWebRoot);
                    }
                }
            }
        }
        $("#modal-info").modal();
    }

    $scope.load();

    $(document).ready(function () {
        bindCommon();
    });
}]);