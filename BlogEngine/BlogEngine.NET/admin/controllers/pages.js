angular.module('blogAdmin').controller('PagesController', ["$scope", "$location", "$http", "$filter", "dataService", function ($scope, $location, $http, $filter, dataService) {
    $scope.items = [];
    $scope.fltr = 'pages';
    $scope.filter = ($location.search()).fltr;

    $scope.load = function () {
        var url = '/api/pages';
        var p = { take: 0, skip: 0 }
        dataService.getItems('/api/pages', p)
        .success(function (data) {
            angular.copy(data, $scope.items);
            gridInit($scope, $filter);
            if ($scope.filter) {
                $scope.setFilter($scope.filter);
            }
            rowSpinOff($scope.items);
        })
        .error(function () {
            toastr.error($rootScope.lbl.errorLoadingPages);
        });
    }

    $scope.load();

    $scope.processChecked = function (action) {
        processChecked("/api/pages/processchecked/", action, $scope, dataService);
    }

	$scope.setFilter = function (filter) {
	    if ($scope.filter === 'pub') {
	        $scope.gridFilter('IsPublished', true, 'pub');
	    }
	    if ($scope.filter === 'dft') {
	        $scope.gridFilter('IsPublished', false, 'dft');
	    }
	}

}]);