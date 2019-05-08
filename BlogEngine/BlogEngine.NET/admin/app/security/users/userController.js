﻿angular.module('blogAdmin').controller('UsersController', ["$rootScope", "$scope", "$filter", "dataService", function ($rootScope, $scope, $filter, dataService) {
    $scope.items = [];
    $scope.roles = [];
    $scope.editItem = {};
    $scope.isNewItem = false;
    $scope.focusInput = false;
    $scope.security = $rootScope.security;
    $scope.UserVars = UserVars;
    $scope.isAdmin = UserVars.IsAdmin === "True"; // more predictable (boolean)


    $scope.load = function () {
        if (!$scope.security.canManageUsers === true) {
            window.location.replace("../Account/Login.aspx");
        }
        dataService.getItems('/api/users', { take: 0, skip: 0, filter: "1 == 1", order: "UserName" })
        .success(function (data) {
            angular.copy(data, $scope.items);
            gridInit($scope, $filter);
            spinOff();
        })
        .error(function () {
            toastr.error($rootScope.lbl.errorLoadingUsers);
        });
    }

    $scope.launchProfilePage = function (user) {
        var userName = "";
        if (user.Profile.RecordId === "")
            userName = user.Profile.UserName;
        else
            userName = user.Profile.RecordId;

        location.href = "#/security/profile/?name=" + userName;
    }

    $scope.loadEditForm = function (id) {
        $scope.loadRoles(id);
        if (!id) {
            $scope.editItem = {};
            $scope.isNewItem = true;
            $("#modal-user-edit").modal();
            $scope.focusInput = true;
            return false;
        }
        else {
            $scope.isNewItem = false;
        }

        dataService.getItems('/api/users?id=' + id)
            .success(function (data) {
                angular.copy(data, $scope.editItem);
                $("#modal-user-edit").modal();
                $scope.focusInput = true;
            })
            .error(function () {
                toastr.error($rootScope.lbl.errorLoadingUser);
                spinOff();
            });
    };

    $scope.loadRoles = function (id) {
        if (!id) {
            id = "fakeusername";
        }
        dataService.getItems('/api/roles/getuserroles/' + id)
            .success(function (data) {
                angular.copy(data, $scope.roles);
                gridInit($scope, $filter);
            })
            .error(function () {
                toastr.error($rootScope.lbl.errorLoadingRoles);
            });
    };

    $scope.saveUser = function () {
        if (!$('#form').valid()) {
            return false;
        }
        spinOn();
        $scope.editItem.roles = $scope.roles;
        if ($scope.isNewItem) {
            dataService.addItem("/api/users", $scope.editItem)
            .success(function (data) {
                toastr.success($rootScope.lbl.userAdded);
                $scope.load();
                spinOff();
                $("#modal-user-edit").modal('hide');
                $scope.focusInput = false;
            })
            .error(function () {
                toastr.error($rootScope.lbl.errorAddingNewUser);
                spinOff();
                $("#modal-user-edit").modal('hide');
                $scope.focusInput = false;
            });
        }
        else {
            dataService.updateItem("/api/users/update/item", $scope.editItem)
            .success(function (data) {
                toastr.success($rootScope.lbl.userUpdatedShort);
                $scope.load();
                spinOff();
                $("#modal-user-edit").modal('hide');
                $scope.focusInput = false;
            })
            .error(function () {
                toastr.error($rootScope.lbl.updateFailed);
                spinOff();
                $("#modal-user-edit").modal('hide');
                $scope.focusInput = false;
            });
        }
    }

    $scope.processChecked = function (action) {
        processChecked("/api/users/processchecked/", action, $scope, dataService);
    }

    $scope.load();

    $(document).ready(function () {
        $('#form').validate({
            rules: {
                txtEmail: { required: true, email: true },
                txtUserName: { required: true },
                txtPassword: { required: true },
                txtConfirmPassword: { required: true, equalTo: '#txtPassword' }
            }
        });
    });

    $(document).ready(function () {
        bindCommon();
    });
}]);