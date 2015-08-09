
angular.module('blogAdmin').controller('ThemeController', ["$rootScope", "$scope", "$location", "$filter", "dataService", function ($rootScope, $scope, $location, $filter, dataService) {
    $scope.items = [];
    $scope.customFields = [];
    $scope.package = {};
    $scope.fltr = 'themes';

    $scope.id = ($location.search()).id;
    $scope.activeTheme = ($location.search()).active;
    
    $scope.load = function () {
        dataService.getItems('/api/packages', { take: 0, skip: 0, filter: $scope.fltr, order: "LastUpdated desc" })
        .success(function (data) {
            angular.copy(data, $scope.items);
            $scope.loadCustomFields();
        })
        .error(function () {
            toastr.error($rootScope.lbl.errorLoadingPackages);
        });
    }

    $scope.loadCustomFields = function () {
        $scope.customFields = [];
        for (var i = 0, len = $scope.items.length; i < len; i++) {
            if ($scope.items[i].Id === $scope.id) {
                angular.copy($scope.items[i], $scope.package);

                if ($scope.package) {
                    $scope.removeEmptyReviews();
                }
            }
        }
        dataService.getItems('/api/customfields', { filter: 'CustomType == "THEME" && ObjectId == "' + $scope.id + '"' })
        .success(function (data) {
            angular.copy(data, $scope.customFields);
        })
        .error(function () {
            toastr.error($rootScope.lbl.errorLoadingCustomFields);
        });
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
            $("#modal-theme-edit").modal('hide');
        })
        .error(function () {
            toastr.error($rootScope.lbl.updateFailed);
            spinOff();
            $("#modal-theme-edit").modal('hide');
        });
    }

    $scope.load();

    $scope.uninstallPackage = function () {
        spinOn();
        dataService.updateItem("/api/packages/uninstall/" + $scope.id, $scope.id)
        .success(function (data) {
            toastr.success($rootScope.lbl.completed);
            window.history.back();
        })
        .error(function () {
            toastr.error($rootScope.lbl.failed);
            spinOff();
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

}]);