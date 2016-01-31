angular.module('blogAdmin').controller('CustomWidgetsController', ["$rootScope", "$scope", "$location", "$filter", "DragDropHandler", "dataService", function ($rootScope, $scope, $location, $filter, DragDropHandler, dataService) {
    $scope.widgetZones = {
        titles: [],
        list1: [],
        list2: [],
        list3: []
    };
    $scope.vm = {};
    $scope.fltr = 'widgets';

    $scope.load = function () {
        spinOn();
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

    $scope.deleteItem = function (itemId, zones) {
        for (var i = zones.length - 1; i >= 0; i--) {
            if (zones[i].Id === itemId) {
                zones.splice(i, 1);
            }
        }
        $scope.save();
    };

    $('#myModal').on('show.bs.modal', function (e) {
        $scope.editId = $(e.relatedTarget).data('Id');
        $(e.currentTarget).find('input[name="widgetId"]').val($scope.editId);
    });

    $scope.save = function () {
        var zones = $scope.vm.WidgetZones;
        if (zones.length > 0) { zones[0].Widgets = $scope.widgetZones.list1; }
        if (zones.length > 1) { zones[1].Widgets = $scope.widgetZones.list2; }
        if (zones.length > 2) { zones[2].Widgets = $scope.widgetZones.list3; }
    }

    $scope.load();

    $(document).ready(function () {
        bindCommon();
    });
}]);