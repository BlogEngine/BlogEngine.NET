﻿angular.module('blogAdmin').controller('ProfileController', ["$rootScope", "$scope", "$filter", "dataService", function ($rootScope, $scope, $filter, dataService) {
    $scope.user = {};
    $scope.noAvatar = SiteVars.ApplicationRelativeWebRoot + "Content/images/blog/noavatar.jpg";
    $scope.photo = $scope.noAvatar;
    $scope.UserVars = UserVars;
    $scope.IsAdmin = UserVars.IsAdmin === "True"; // more predictable (boolean)
    $scope.siteVars = SiteVars;
        
    $scope.focusInput = false;
    $scope.customFields = [];
    $scope.editItem = {};

    $scope.load = function () {
        spinOn();
        // if the name is provided in url parameter then use it
        $scope.userName = $scope.getParameterByName("name", UserVars.Name);

        dataService.getItems('/api/users/' + $scope.userName)
        .success(function (data) {
            angular.copy(data, $scope.user);
            $scope.loadCustom();
            $scope.setPhoto();
            spinOff();
        })
        .error(function () {
            toastr.error($rootScope.lbl.errorLoadingUser);
            spinOff();
        });
    }

    $scope.getParameterByName = function(name, defaultName){
        var url = window.location.href;
            name = name.replace(/[\[\]]/g, '\\$&');
        var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
            results = regex.exec(url);
        if (!results) return defaultName;
        if (!results[2]) return defaultName;
        return decodeURIComponent(results[2].replace(/\+/g, ' '));
    }

    $scope.save = function () {
        spinOn();
        dataService.updateItem("/api/users/saveprofile/item", $scope.user)
        .success(function (data) {
            toastr.success($rootScope.lbl.userUpdatedShort);
            if ($scope.customFields && $scope.customFields.length > 0) {
                $scope.updateCustom();
            }
            $scope.load();
            spinOff();
        })
        .error(function () {
            toastr.error($rootScope.lbl.updateFailed);
            spinOff();
        });
    }

    $scope.removePicture = function () {
        $scope.user.Profile.PhotoUrl = "";
        $scope.save();
    }

    $scope.changePicture = function (files) {
        var fd = new FormData();
        fd.append("file", files[0]);

        dataService.uploadFile("/api/upload?action=profile", fd)
        .success(function (data) {
            $scope.user.Profile.PhotoUrl = data;
            $scope.save();
        })
        .error(function () { toastr.error($rootScope.lbl.failed); });
    }

    $scope.setPhoto = function () {
        if ($scope.user.Profile.PhotoUrl) {
            $scope.photo = SiteVars.RelativeWebRoot + "image.axd?picture=/avatars/" +
                $scope.user.Profile.PhotoUrl + "&" + new Date().getTime();
        }
        else {
            $scope.photo = $scope.noAvatar;
        }
    }

    $scope.load();

    /* Custom fields */

    $scope.showCustom = function () {
        $("#modal-custom").modal();
        $scope.focusInput = true;
    }

    $scope.loadCustom = function () {
        $scope.customFields = [];

        dataService.getItems('/api/customfields', {
            filter: 'CustomType == "PROFILE" && objectid=="' + $scope.userName +'"',
        })
        .success(function (data) {
            angular.copy(data, $scope.customFields);
        })
        .error(function () {
            toastr.error($rootScope.lbl.errorLoadingCustomFields);
        });
    }

    $scope.saveCustom = function () {
        var customField = {
            "CustomType": "PROFILE",
            "ObjectId": $scope.userName,
            "Key": $("#txtKey").val(),
            "Value": $("#txtValue").val()
        };
        if (customField.Key === '') {
            toastr.error("Custom key is required");
            return false;
        }
        dataService.addItem("/api/customfields", customField)
        .success(function (data) {
            toastr.success('New item added');
            $scope.load();
            $("#modal-custom").modal('hide');
        })
        .error(function () {
            toastr.error($rootScope.lbl.updateFailed);
            $("#modal-custom").modal('hide');
        });
    }

    $scope.updateCustom = function () {
        dataService.updateItem("/api/customfields", $scope.customFields)
        .success(function (data) {
            spinOff();
        })
        .error(function () {
            toastr.error($rootScope.lbl.updateFailed);
            spinOff();
        });
    }

    $scope.deleteCustom = function (key, objId) {
        var customField = {
            "CustomType": "PROFILE",
            "Key": key,
            "ObjectId": objId
        };
        spinOn();
        dataService.deleteItem("/api/customfields", customField)
        .success(function (data) {
            toastr.success("Item deleted");
            spinOff();
            $scope.load();
        })
        .error(function () {
            toastr.error($rootScope.lbl.couldNotDeleteItem);
            spinOff();
        });
    }
    $(document).ready(function () {
        bindCommon();
    });
}]);