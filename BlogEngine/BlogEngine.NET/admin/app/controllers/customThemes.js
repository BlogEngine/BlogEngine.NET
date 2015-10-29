angular.module('blogAdmin').controller('CustomThemesController', ["$rootScope", "$scope", "$location", "$filter", "dataService", function ($rootScope, $scope, $location, $filter, dataService) {
    $scope.items = [];
    $scope.customFields = [];
    $scope.editId = "";
    $scope.package = {};
    $scope.activeTheme = ActiveTheme;
    $scope.IsPrimary = $rootScope.SiteVars.IsPrimary == "True";
    $scope.fltr = 'themes';
    $scope.order = 'DownloadCount desc';
    $scope.sortingOrder = 'DownloadCount';
    $scope.reverse = true;
    $scope.selectedRating = 0;
    $scope.author = UserVars.Name;

    if ($location.path().indexOf("/custom/themes/gallery") == 0) {
        $scope.fltr = 'all';
        $scope.galleryFilter = 'themes';
    }

    $scope.load = function () {
        spinOn();
        dataService.getItems('/api/packages', { take: 0, skip: 0, filter: $scope.fltr, order: 'LastUpdated desc' })
        .success(function (data) {
            angular.copy(data, $scope.items);
            gridInit($scope, $filter);
            if ($scope.galleryFilter) {
                $scope.setFilter();
            }
            var pkgId = getFromQueryString('pkgId');
            if (pkgId != null) {
                $scope.query = pkgId;
                $scope.search();
            }
            spinOff();
        })
        .error(function () {
            toastr.error($rootScope.lbl.errorLoadingPackages);
            spinOff();
        });
    }

    $scope.showInfo = function (id) {
        for (var i = 0, len = $scope.items.length; i < len; i++) {
            if ($scope.items[i].Id === id) {
                angular.copy($scope.items[i], $scope.package);

                if ($scope.package) {
                    if ($scope.package.SettingsUrl) {
                        $scope.extEditSrc = $scope.package.SettingsUrl.replace("~/", SiteVars.RelativeWebRoot);
                    }
                }
            }
        }
        $("#modal-info").modal();
    }

    $scope.showSettings = function (id) {
        $scope.editId = id;

        dataService.getItems('/api/customfields', { filter: 'CustomType == "THEME" && ObjectId == "' + id + '"' })
        .success(function (data) {
            angular.copy(data, $scope.customFields);
            $("#modal-settings").modal();
        })
        .error(function () {
            toastr.error($rootScope.lbl.errorLoadingCustomFields);
        });
    }

    $scope.setDefaultTheme = function (id) {
        spinOn();
        dataService.updateItem("/api/packages/settheme/" + id, id)
        .success(function (data) {
            ActiveTheme = id;
            $scope.activeTheme = id;
            toastr.success($rootScope.lbl.completed);
            $scope.load();
        })
        .error(function () {
            toastr.error($rootScope.lbl.failed);
            spinOff();
        });
    }

    $scope.setFilter = function () {
        if ($scope.galleryFilter == 'extensions') {
            $scope.gridFilter('PackageType', 'Extension', 'pub');
        }
        if ($scope.galleryFilter == 'themes') {
            $scope.gridFilter('PackageType', 'Theme', 'dft');
        }
    }

    $scope.checkStar = function (item, rating) {
        if (item === rating) {
            return true;
        }
        return false;
    }

    $scope.setRating = function (rating) {
        $scope.selectedRating = rating;
    }

    $scope.sortBy = function (ord) {
        $scope.sortingOrder = ord;
        $scope.reverse = true;
        $scope.load();
    }

    $scope.processChecked = function (action) {
        processChecked("/api/packages/processchecked/", action, $scope, dataService);
    }

    $scope.installPackage = function (pkgId) {
        spinOn();
        dataService.updateItem("/api/packages/install/" + pkgId, pkgId)
        .success(function (data) {
            toastr.success($rootScope.lbl.completed);
            $scope.load();
        })
        .error(function () {
            toastr.error($rootScope.lbl.failed);
            spinOff();
        });
    }

    $scope.uninstallPackage = function (pkgId) {
        spinOn();
        dataService.updateItem("/api/packages/uninstall/" + pkgId, pkgId)
        .success(function (data) {
            toastr.success($rootScope.lbl.completed);
            $scope.load();
        })
        .error(function () {
            toastr.error($rootScope.lbl.failed);
            spinOff();
        });
    }

    $scope.load();

    $(document).ready(function () {
        bindCommon();
    });
}]);