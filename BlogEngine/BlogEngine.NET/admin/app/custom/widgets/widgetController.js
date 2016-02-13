angular.module('blogAdmin').controller('CustomWidgetsController', ["$rootScope", "$scope", "$location", "$filter", "DragDropHandler", "dataService", function ($rootScope, $scope, $location, $filter, DragDropHandler, dataService) {
    $scope.widgetZones = {};
    $scope.vm = {};
    $scope.editSrc = {};
    $scope.editId = {};
    $scope.editTitle = {};

    $scope.load = function () {
        spinOn();
        $scope.widgetZones = {
            titles: [],
            list1: [], list2: [], list3: []
        };
        $scope.vm = {};

        dataService.getItems('/api/widgets', { })
        .success(function (data) {
            angular.copy(data, $scope.vm);
            var zones = $scope.vm.WidgetZones;
            for (i = 0; i < zones.length; i++) {
                $scope.widgetZones.titles.push(zones[i].Id);
            }
            if (zones.length > 0) { $scope.widgetZones.list1 = zones[0].Widgets; }
            if (zones.length > 1) { $scope.widgetZones.list2 = zones[1].Widgets; }
            if (zones.length > 2) { $scope.widgetZones.list3 = zones[2].Widgets; }

            spinOff();
        })
        .error(function () {
            toastr.error($rootScope.lbl.errorLoadingPackages);
            spinOff();
        });
    }

    $scope.loadEditForm = function (id, name, title) {
        var sharedSrc = SiteVars.ApplicationRelativeWebRoot + "Custom/Widgets/common.cshtml";
        var customSrc = SiteVars.ApplicationRelativeWebRoot + "Custom/Widgets/" + name + "/edit.cshtml";
        $scope.editId = id;
        $scope.editTitle = title;
        $("#txtWidgetTitle").val(title);
        $("#titleValidation").hide();
        $("#settingsFrame").contents().find('.field-validation-error').hide();
        $.ajax({
            type: 'HEAD',
            url: customSrc,
            async: false,
            success: function () {
                $scope.editSrc = customSrc + "?id=" + id;
            },
            error: function () {
                $scope.editSrc = sharedSrc + "?id=" + id;
            }
        });
        $("#edit-widget").modal();
    }

    $scope.closeEditForm = function () {
        $("#edit-widget").modal("hide");
    }

    $scope.moveObject = function (from, to, fromList, toList) {
        var item = $scope.widgetZones[fromList][from];
        DragDropHandler.addObject(item, $scope.widgetZones[toList], to);
        $scope.widgetZones[fromList].splice(from, 1);
    }

    $scope.createObject = function (object, to, list) {
        var newItem = angular.copy(object);
        newItem.Id = Math.ceil(Math.random() * 1000);
        DragDropHandler.addObject(newItem, $scope.widgetZones[list], to);
    };

    $scope.deleteItem = function (itemId, zones, zoneId) {
        for (var i = zones.length - 1; i >= 0; i--) {
            if (zones[i].Id === itemId) {
                zones.splice(i, 1);
            }
        }
    };

    $scope.save = function () {
        dataService.updateItem("/api/widgets/update/item", $scope.vm.WidgetZones)
        .success(function (data) {
            toastr.success($rootScope.lbl.completed);
            $scope.load();
            spinOff();
        })
        .error(function () {
            toastr.error($rootScope.lbl.updateFailed);
            spinOff();
        });
    }

    $scope.updateTitle = function () {
        if ($("#txtWidgetTitle").val().length > 0) {
            if ($scope.editTitle != $("#txtWidgetTitle").val()) {
                for (var i = 0; i < $scope.vm.WidgetZones.length; i++) {
                    for (var j = 0; j < $scope.vm.WidgetZones[i].Widgets.length; j++) {
                        if ($scope.vm.WidgetZones[i].Widgets[j].Id === $scope.editId) {
                            $scope.vm.WidgetZones[i].Widgets[j].Title = $("#txtWidgetTitle").val();
                        }
                    }
                }
                $scope.save();
                return true;
            }
            return false;
        }
        else {
            $("#titleValidation").show();
            $("#txtWidgetTitle").focus();
            return false;
        }
    }

    $scope.load();

    $(document).ready(function () {
        bindCommon();
        $("#titleValidation").hide();
    });
}]);

var updateTitle = function () {
    return angular.element($('#edit-widget')).scope().updateTitle();
}