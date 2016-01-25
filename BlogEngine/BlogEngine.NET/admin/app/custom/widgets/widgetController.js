angular.module('blogAdmin').controller('CustomWidgetsController', ["$rootScope", "$scope", "$location", "$filter", "DragDropHandler", "dataService", function ($rootScope, $scope, $location, $filter, DragDropHandler, dataService) {
    $scope.installedWidgets = [];
    $scope.fltr = 'widgets';

    $scope.load = function () {
        spinOn();
        dataService.getItems('/api/packages', { take: 0, skip: 0, filter: $scope.fltr, order: 'LastUpdated desc' })
        .success(function (data) {
            angular.copy(data, $scope.installedWidgets);
            spinOff();
        })
        .error(function () {
            toastr.error($rootScope.lbl.errorLoadingPackages);
            spinOff();
        });
    }

    $scope.items = {
        list1: [
            {
                id: 13,
                name: 'Administration'
            }
        ],
        list2: []
    };

    $scope.moveObject = function (from, to, fromList, toList) {
        var item = $scope.items[fromList][from];
        DragDropHandler.addObject(item, $scope.items[toList], to);
        $scope.items[fromList].splice(from, 1);
    }

    $scope.createObject = function (object, to, list) {
        var newItem = angular.copy(object);
        newItem.id = Math.ceil(Math.random() * 1000);
        DragDropHandler.addObject(newItem, $scope.items[list], to);
    };

    $scope.deleteItem = function (itemId) {
        for (var list in $scope.items) {
            if ($scope.items.hasOwnProperty(list)) {
                $scope.items[list] = _.reject($scope.items[list], function (item) {
                    return item.id == itemId;
                });
            }
        }
    };

    $('#myModal').on('show.bs.modal', function (e) {
        $scope.editId = $(e.relatedTarget).data('id');
        $(e.currentTarget).find('input[name="widgetId"]').val($scope.editId);
    });

    $scope.load();

    $(document).ready(function () {
        bindCommon();
    });
}]);