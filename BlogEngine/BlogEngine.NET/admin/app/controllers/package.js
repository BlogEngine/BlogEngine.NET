
angular.module('blogAdmin').controller('PackageController', ["$rootScope", "$scope", "$location", "$filter", "dataService", function ($rootScope, $scope, $location, $filter, dataService) {
    $scope.items = [];
    $scope.customFields = [];
    $scope.package = {};
    $scope.fltr = 'themes';
    $scope.showRating = false;
    $scope.selectedRating = 0;
    $scope.author = UserVars.Name;
    $scope.id = ($location.search()).id;
    $scope.activeTheme = ActiveTheme;
    
    $scope.load = function () {
        dataService.getItems('/api/packages/' + $scope.id)
        .success(function (data) {
            angular.copy(data, $scope.package);
            $scope.selectedRating = $scope.package.Rating;

            $scope.extEditSrc = SiteVars.RelativeWebRoot + "admin/Extensions/Settings.aspx?ext=" + $scope.id + "&enb=False";
            if ($scope.package.SettingsUrl) {
                $scope.extEditSrc = $scope.package.SettingsUrl.replace("~/", SiteVars.RelativeWebRoot);
            }

            $scope.removeEmptyReviews();
            $scope.loadCustomFields();
        })
        .error(function () {
            toastr.error($rootScope.lbl.errorLoadingPackages);
        });
    }

    $scope.loadCustomFields = function () {
        $scope.customFields = [];
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

    $scope.checkStar = function (item, rating) {
        if (item === rating) {
            return true;
        }
        return false;
    }

    $scope.setRating = function (rating) {
        $scope.selectedRating = rating;
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
            $scope.load();
        })
        .error(function () {
            toastr.error($rootScope.lbl.failed);
        });
    }

    $scope.setPriority = function (upDown) {
        if (upDown == 'up') {
            $scope.package.Priority++;
        }
        else {
            if ($scope.package.Priority > 0) {
                $scope.package.Priority--;
            }
        }
        $scope.save();
    }

    $scope.load();
    
}]);

function setIframeBg() {
    var x = document.getElementById("settingsFrame");
    var y = x.contentDocument;
    y.body.style.backgroundColor = "#eee";
}