
angular.module('blogAdmin').controller('HelpController', ["$rootScope", "$scope", "$filter", "dataService", function ($rootScope, $scope, $filter, dataService) {
    $scope.items = [];

    $scope.selectedSocial = 0;
    $scope.radioClick = function (id) {
        if (id == 'radio1') { $scope.selectedSocial = 0; }
        if (id == 'radio2') { $scope.selectedSocial = 1; }
        if (id == 'radio3') { $scope.selectedSocial = 2; }

        $('#social-buttons label').removeClass('active');
        $('#' + id).addClass("active");
    }

}]);