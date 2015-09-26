angular.module('blogAdmin').controller('DashboardController', ["$rootScope", "$scope", "$location", "$log", "$filter", "dataService", function ($rootScope, $scope, $location, $log, $filter, dataService) {
    $scope.vm = {};
    $scope.itemToPurge = {};
    $scope.security = $rootScope.security;
    $scope.pager = {};
    $scope.pager.items = [];
    $scope.pagerCurrentPage = 0;
    $scope.focusInput = false;

    $scope.openLogFile = function () {
        $("#modal-log-file").modal();
        return false;
    }
    $scope.purgeLog = function () {
        dataService.updateItem('/api/logs/purgelog/file', $scope.itemToPurge)
        .success(function (data) {
            $scope.vm.Logs = [];
            $("#modal-log-file").modal('hide');
            toastr.success($rootScope.lbl.purged);
            return false;
        })
        .error(function (data) {
            toastr.error($rootScope.lbl.errorPurging);
        });
    }

    $scope.purge = function (id) {
        if (id) {
            $scope.itemToPurge = findInArray($scope.vm.Trash, "Id", id);
        }
        dataService.updateItem('/api/trash/purge/' + id, $scope.itemToPurge)
        .success(function (data) {
            for (var i = 0; i < $scope.vm.Trash.length; i++)
            if ($scope.vm.Trash[i].Id === id) {
                $scope.vm.Trash.splice(i, 1);
                break;
            }
            toastr.success($rootScope.lbl.purged);
            return false;
        })
        .error(function (data) {
            toastr.error($rootScope.lbl.errorPurging);
        });
    }
    $scope.purgeAll = function () {
        dataService.updateItem('/api/trash/purgeall/all')
        .success(function (data) {
            $scope.vm.Trash = [];
            toastr.success($rootScope.lbl.purged);
            return false;
        })
        .error(function (data) {
            toastr.error($rootScope.lbl.errorPurging);
        });
    }
    $scope.restore = function (id) {
        if (id) {
            $scope.itemToPurge = findInArray($scope.vm.Trash, "Id", id);
        }
        dataService.updateItem('/api/trash/restore/' + id, $scope.itemToPurge)
        .success(function (data) {
            for (var i = 0; i < $scope.vm.Trash.length; i++)
            if ($scope.vm.Trash[i].Id === id) {
                $scope.vm.Trash.splice(i, 1);
                break;
            }
            toastr.success($rootScope.lbl.restored);
            return false;
        })
        .error(function (data) {
            toastr.error($rootScope.lbl.errorRestoring);
        });
    }

    $scope.load = function () {
        if ($rootScope.security.showTabDashboard === false) {
            window.location = "../Account/Login.aspx";
        }
        $("#versionMsg").hide();
        spinOn();

        $scope.loadPackages();

        dataService.getItems('/api/dashboard')
        .success(function (data) {
            angular.copy(data, $scope.vm);
            $scope.pager.items = $scope.vm.Notes;
            listPagerInit($scope.pager);
        })
        .error(function (data) { toastr.success($rootScope.lbl.errorGettingStats); });
    }

    $scope.loadPackages = function () {
        if (!$scope.security.showTabCustom) {
            return;
        }
        dataService.getItems('/api/packages', { take: 5, skip: 0 })
        .success(function (data) {
            $scope.packages = [];
            angular.copy(data, $scope.packages);
            $scope.checkNewVersion();
            if ($scope.packages.length > 0) {
                $('#tr-gal-spinner').hide();
            }
            else { $('#div-gal-spinner').html(BlogAdmin.i18n.empty); }
        })
        .error(function () {
            toastr.error($rootScope.lbl.errorLoadingPackages);
        });
    }

    $scope.checkNewVersion = function () {
        if (!$scope.security.showTabCustom) {
            return;
        }
        var version = SiteVars.Version.substring(15, 22);
        $.ajax({
            url: SiteVars.ApplicationRelativeWebRoot + "api/setup?version=" + version,
            type: "GET",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data && data.length > 0) {
                    $("#vNumber").html(data);
                    $("#versionMsg").show();
                }
            }
        });
    }

    $scope.load();

    $scope.noteId = '';
    $scope.notePage = 1;
    $scope.addNote = function () {
        $scope.noteId = '';
        $("#txtAddNote").val('');
        $("#modal-add-note").modal();
        $scope.focusInput = true;
    }
    $scope.editNote = function (id) {
        $scope.noteId = id;
        var note = findInArray($scope.pager.items, 'Id', id);
        $("#txtEditNote").val(note.Note);
        $("#modal-edit-note").modal();
        $scope.focusInput = true;
    }
    $scope.deleteNote = function (id) {
        var note = { 'Id': id };
        dataService.deleteItem("/api/quicknotes/", note)
        .success(function (data) {
            toastr.success($rootScope.lbl.completed);
            $scope.load();
            $("#modal-edit-note").modal('hide');
        })
        .error(function () {
            toastr.error($rootScope.lbl.failed);
            $("#modal-edit-note").modal('hide');
        });
    }
    saveNote = function () {
        if ($scope.noteId === '') {
            $scope.addNewNote();
        }
        else {
            $scope.updateNote();
        }
        $scope.focusInput = false;
    }
    $scope.addNewNote = function () {
        if ($("#txtAddNote").val().length < 1) {
            toastr.error($rootScope.lbl.isRequiredField);
            return false;
        }
        var note = { 'Id': '', 'Note': $("#txtAddNote").val() };
        dataService.addItem("/api/quicknotes/add", note)
        .success(function (data) {
            toastr.success($rootScope.lbl.completed);
            $scope.load();
            $("#modal-add-note").modal('hide');
        })
        .error(function () {
            toastr.error($rootScope.lbl.failed);
            $("#modal-add-note").modal('hide');
        });
    }
    $scope.updateNote = function () {
        if ($("#txtEditNote").val().length < 1) {
            toastr.error($rootScope.lbl.isRequiredField);
            return false;
        }
        var note = { 'Id': $scope.noteId, 'Note': $("#txtEditNote").val() };
        dataService.updateItem("/api/quicknotes/" + $scope.noteId, note)
        .success(function (data) {
            toastr.success($rootScope.lbl.completed);
            $scope.load();
            $("#modal-edit-note").modal('hide');
        })
        .error(function () {
            toastr.error($rootScope.lbl.failed);
            $("#modal-edit-note").modal('hide');
        });
    }
    $scope.notePrevPage = function () {
        if ($scope.notePage > 1) {
            $scope.notePage--;
        }
        alert($scope.notePage);
    }
    $scope.noteNextPage = function () {
        $scope.notePage++;
        alert($scope.notePage);
    }

}]);