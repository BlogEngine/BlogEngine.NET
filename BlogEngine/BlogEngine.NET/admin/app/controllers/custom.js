
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

    $scope.showRating = false;
    $scope.selectedRating = 0;
    $scope.author = UserVars.Name;
    $scope.activeTheme = ActiveTheme;
    
    if ($location.path().indexOf("/custom") == 0) {
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
        dataService.getItems('/api/packages', { take: 0, skip: 0, filter: $scope.fltr, order: "LastUpdated desc" })
        .success(function (data) {
            angular.copy(data, $scope.items);

            gridInit($scope, $filter);
            rowSpinOff($scope.items);

            var pkgId = getFromQueryString('pkgId');
            if (pkgId != null) {
                $scope.query = pkgId;
                $scope.search();
            }
        })
        .error(function () {
            toastr.error($rootScope.lbl.errorLoadingPackages);
        });
    }

    $scope.loadCustomFields = function (id) {
        $scope.editId = id;
        $scope.extEditSrc = SiteVars.RelativeWebRoot + "admin/Extensions/Settings.aspx?ext=" + id + "&enb=False";
        $scope.customFields = [];

        for (var i = 0, len = $scope.items.length; i < len; i++) {
            if ($scope.items[i].Id === id) {
                angular.copy($scope.items[i], $scope.package);

                if ($scope.package) {
                    $scope.removeEmptyReviews();

                    if($scope.package.SettingsUrl){
                        $scope.extEditSrc = $scope.package.SettingsUrl.replace("~/", SiteVars.RelativeWebRoot);
                    }
                }
            }
        }
        dataService.getItems('/api/customfields', { filter: 'CustomType == "THEME" && ObjectId == "' + id + '"' })
        .success(function (data) {
            angular.copy(data, $scope.customFields);
        })
        .error(function () {
            toastr.error($rootScope.lbl.errorLoadingCustomFields);
        });
    }

    $scope.relocate = function (loc) {
        $scope.pkgLocation = loc;
        $("#fltr-loc").removeClass("active");
        $("#fltr-gal").removeClass("active");
        $scope.load();
    }

    $scope.save = function () {
        spinOn();

        dataService.updateItem("/api/packages/update/foo", $scope.package)
        .success(function (data) {
            toastr.success($rootScope.lbl.completed);
        })
        .error(function () {
            toastr.error($rootScope.lbl.failed);
        });

        dataService.updateItem("/api/customfields", $scope.customFields)
        .success(function (data) {
            $scope.load();
            spinOff();
        })
        .error(function () {
            toastr.error($rootScope.lbl.updateFailed);
            spinOff();
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
        spinOn();
        dataService.updateItem("/api/packages/install/" + pkgId, pkgId)
        .success(function (data) {
            toastr.success($rootScope.lbl.completed);
            $scope.load();
            spinOff();
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
            spinOff();
        })
        .error(function () {
            toastr.error($rootScope.lbl.failed);
            spinOff();
        });
    }

    $scope.refreshGalleryList = function () {
        spinOn();
        dataService.updateItem("/api/packages/refresh/list", { })
        .success(function (data) {
            toastr.success($rootScope.lbl.completed);
            spinOff();
            $scope.load();
        })
        .error(function () {
            toastr.error($rootScope.lbl.failed);
            spinOff();
        });
    }

    $scope.enableExtension = function (item, enable) {
        $scope.package = item;
        $scope.package.Enabled = enable;
        $scope.save();
    }

    $scope.load();

    /*
    $scope.checkStar = function (item, rating) {
        if (item === rating) {
            return true;
        }
        return false;
    }

    $scope.setRating = function (rating) {
        $scope.selectedRating = rating;
    }

    $scope.showRatingForm = function (item, rating) {
        $scope.selectedRating = rating;

        dataService.getItems('/api/packages/' + item.Id)
        .success(function (data) {  
            $scope.package.Extra = data.Extra;
            $scope.removeEmptyReviews();
            $("#modal-rating").modal();
        })
        .error(function () {
            toastr.error($rootScope.lbl.failed);
        });
    }

    $scope.submitRating = function () {
        var author = $("#txtAuthor").val().length > 0 ? $("#txtAuthor").val() : $scope.author;
        var review = { "Name": author, "Rating": $scope.selectedRating, "Body": $("#txtReview").val() };

        dataService.updateItem("/api/packages/rate/" + $scope.package.Extra.Id, review)
        .success(function (data) {
            if (data != null) {
                data = JSON.parse(data);
            }
            if (data.length === 0) {
                toastr.success($rootScope.lbl.completed);
            }
            else {
                toastr.error(data);
            }
            $("#modal-rating").modal('hide');
            $scope.load();
        })
        .error(function () {
            toastr.error($rootScope.lbl.failed);
        });
    }

    $scope.removeEmptyReviews = function () {       
        if ($scope.package.Extra != null && $scope.package.Extra.Reviews != null) {
            var reviews = [];
            for (var i = 0; i < $scope.package.Extra.Reviews.length; i++) {
                var review = $scope.package.Extra.Reviews[i];
                if (review.Body.length > 0) {
                    reviews.push(review);
                }
            }
            $scope.package.Extra.Reviews = reviews;
        }
    }
    */
}]);