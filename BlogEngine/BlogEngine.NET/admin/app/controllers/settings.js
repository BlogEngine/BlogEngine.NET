angular.module('blogAdmin').controller('SettingsController', ["$rootScope", "$scope", "$location", "$log", "$http", "dataService", function ($rootScope, $scope, $location, $log, $http, dataService) {
    $scope.settings = {};
    $scope.lookups = {};
    $scope.UserVars = UserVars;
    $scope.SiteVars = SiteVars;
    $scope.selfRegistrationInitialRole = {};
    $scope.ServerTime = moment(ServerTime).format("YYYY-MM-DD HH:mm");
    $scope.UtcTime = moment(UtcTime).format("YYYY-MM-DD HH:mm");
    $scope.singleUserBlog = SingleUserBlog;

    $scope.moderationEnabled = 0;
    $scope.commentsProvider = 0;

    $scope.feedOptions = [
        { "OptionName": "RSS 2.0", "OptionValue": "Rss", "IsSelected": false },
        { "OptionName": "Atom 1.0", "OptionValue": "Atom", "IsSelected": false }
    ];
    $scope.closeDaysOptions = [
        { "OptionName": $rootScope.lbl.never, "OptionValue": "0", "IsSelected": false },
        { "OptionName": "1", "OptionValue": "1", "IsSelected": false },
        { "OptionName": "2", "OptionValue": "2", "IsSelected": false },
        { "OptionName": "3", "OptionValue": "3", "IsSelected": false },
        { "OptionName": "7", "OptionValue": "7", "IsSelected": false },
        { "OptionName": "10", "OptionValue": "10", "IsSelected": false },
        { "OptionName": "14", "OptionValue": "14", "IsSelected": false },
        { "OptionName": "21", "OptionValue": "21", "IsSelected": false },
        { "OptionName": "30", "OptionValue": "30", "IsSelected": false },
        { "OptionName": "60", "OptionValue": "60", "IsSelected": false },
        { "OptionName": "90", "OptionValue": "90", "IsSelected": false },
        { "OptionName": "180", "OptionValue": "180", "IsSelected": false },
        { "OptionName": "365", "OptionValue": "365", "IsSelected": false }
    ];
    $scope.commentsPerPageOptions = [
        { "OptionName": "5", "OptionValue": "5", "IsSelected": false },
        { "OptionName": "10", "OptionValue": "10", "IsSelected": false },
        { "OptionName": "15", "OptionValue": "15", "IsSelected": false },
        { "OptionName": "20", "OptionValue": "20", "IsSelected": false },
        { "OptionName": "50", "OptionValue": "50", "IsSelected": false }
    ];
    $scope.whiteListOptions = [
        { "OptionName": "0", "OptionValue": "0", "IsSelected": false },
        { "OptionName": "1", "OptionValue": "1", "IsSelected": false },
        { "OptionName": "2", "OptionValue": "2", "IsSelected": false },
        { "OptionName": "3", "OptionValue": "3", "IsSelected": false },
        { "OptionName": "4", "OptionValue": "4", "IsSelected": false },
        { "OptionName": "5", "OptionValue": "5", "IsSelected": false }
    ];
    $scope.blackListOptions = [
        { "OptionName": "0", "OptionValue": "0", "IsSelected": false },
        { "OptionName": "1", "OptionValue": "1", "IsSelected": false },
        { "OptionName": "2", "OptionValue": "2", "IsSelected": false },
        { "OptionName": "3", "OptionValue": "3", "IsSelected": false },
        { "OptionName": "4", "OptionValue": "4", "IsSelected": false },
        { "OptionName": "5", "OptionValue": "5", "IsSelected": false }
    ];
    $scope.timeZoneOptions = [
        { "OptionName": "(GMT -12:00) Eniwetok, Kwajalein", "OptionValue": "-12", "IsSelected": false },
        { "OptionName": "(GMT -11:00) Midway Island, Samoa", "OptionValue": "-11", "IsSelected": false },
        { "OptionName": "(GMT -10:00) Hawaii", "OptionValue": "-10", "IsSelected": false },
        { "OptionName": "(GMT -9:00) Alaska", "OptionValue": "-9", "IsSelected": false },
        { "OptionName": "(GMT -8:00) Pacific Time (US &amp; Canada)", "OptionValue": "-8", "IsSelected": false },
        { "OptionName": "(GMT -7:00) Mountain Time (US &amp; Canada)", "OptionValue": "-7", "IsSelected": false },
        { "OptionName": "(GMT -6:00) Central Time (US &amp; Canada), Mexico City", "OptionValue": "-6", "IsSelected": false },
        { "OptionName": "(GMT -5:00) Eastern Time (US &amp; Canada), Bogota, Lima", "OptionValue": "-5", "IsSelected": false },
        { "OptionName": "(GMT -4:30) Caracas", "OptionValue": "-4.5", "IsSelected": false },
        { "OptionName": "(GMT -4:00) Atlantic Time (Canada), La Paz, Santiago", "OptionValue": "-4", "IsSelected": false },
        { "OptionName": "(GMT -3:30) Newfoundland", "OptionValue": "-3.5", "IsSelected": false },
        { "OptionName": "(GMT -3:00) Brazil, Buenos Aires, Georgetown", "OptionValue": "-3", "IsSelected": false },
        { "OptionName": "(GMT -2:00) Mid-Atlantic", "OptionValue": "-2", "IsSelected": false },
        { "OptionName": "(GMT -1:00 hour) Azores, Cape Verde Islands", "OptionValue": "-1", "IsSelected": false },
        { "OptionName": "(GMT) Western Europe Time, London, Greenwich", "OptionValue": "0", "IsSelected": false },
        { "OptionName": "(GMT +1:00 hour) Brussels, Copenhagen, Madrid, Paris", "OptionValue": "1", "IsSelected": false },
        { "OptionName": "(GMT +2:00) Kaliningrad, South Africa, Cairo", "OptionValue": "2", "IsSelected": false },
        { "OptionName": "(GMT +3:00) Baghdad, Riyadh, Moscow, St. Petersburg", "OptionValue": "3", "IsSelected": false },
        { "OptionName": "(GMT +3:30) Tehran", "OptionValue": "3.5", "IsSelected": false },
        { "OptionName": "(GMT +4:00) Abu Dhabi, Muscat, Yerevan, Baku, Tbilisi", "OptionValue": "4", "IsSelected": false },
        { "OptionName": "(GMT +4:30) Kabul", "OptionValue": "4.5", "IsSelected": false },
        { "OptionName": "(GMT +5:00) Ekaterinburg, Islamabad, Karachi, Tashkent", "OptionValue": "5", "IsSelected": false },
        { "OptionName": "(GMT +5:30) Mumbai, Kolkata, Chennai, New Delhi", "OptionValue": "5.5", "IsSelected": false },
        { "OptionName": "(GMT +5:45) Kathmandu", "OptionValue": "5.75", "IsSelected": false },
        { "OptionName": "(GMT +6:00) Almaty, Dhaka, Colombo", "OptionValue": "6", "IsSelected": false },
        { "OptionName": "(GMT +6:30) Yangon, Cocos Islands", "OptionValue": "6.5", "IsSelected": false },
        { "OptionName": "(GMT +7:00) Bangkok, Hanoi, Jakarta", "OptionValue": "7", "IsSelected": false },
        { "OptionName": "(GMT +8:00) Beijing, Perth, Singapore, Hong Kong", "OptionValue": "8", "IsSelected": false },
        { "OptionName": "(GMT +9:00) Tokyo, Seoul, Osaka, Sapporo, Yakutsk", "OptionValue": "9", "IsSelected": false },
        { "OptionName": "(GMT +9:30) Adelaide, Darwin", "OptionValue": "9.5", "IsSelected": false },
        { "OptionName": "(GMT +10:00) Eastern Australia, Guam, Vladivostok", "OptionValue": "10", "IsSelected": false },
        { "OptionName": "(GMT +11:00) Magadan, Solomon Islands, New Caledonia", "OptionValue": "11", "IsSelected": false },
        { "OptionName": "(GMT +12:00) Auckland, Wellington, Fiji, Kamchatka", "OptionValue": "12", "IsSelected": false }
    ];

    $scope.load = function () {
        spinOn();
        dataService.getItems('/api/lookups')
        .success(function (data) {
            angular.copy(data, $scope.lookups);
            $scope.loadSettings();
        })
        .error(function () {
            toastr.error($rootScope.lbl.errorLoadingSettings);
            spinOff();
        });
    }

    $scope.loadSettings = function () {
        dataService.getItems('/api/settings')
        .success(function (data) {
            angular.copy(data, $scope.settings);
            $scope.selectedLanguage = selectedOption($scope.lookups.Cultures, $scope.settings.Culture);
            $scope.selectedDeskTheme = selectedOption($scope.lookups.InstalledThemes, $scope.settings.DesktopTheme);
            $scope.selfRegistrationInitialRole = selectedOption($scope.lookups.SelfRegisterRoles, $scope.settings.SelfRegistrationInitialRole);
            $scope.selFeedFormat = selectedOption($scope.feedOptions, $scope.settings.SyndicationFormat);
            $scope.selCloseDays = selectedOption($scope.closeDaysOptions, $scope.settings.DaysCommentsAreEnabled);
            $scope.selCommentsPerPage = selectedOption($scope.commentsPerPageOptions, $scope.settings.CommentsPerPage);
            $scope.selTimeZone = selectedOption($scope.timeZoneOptions, $scope.settings.Timezone);

            $scope.whiteListSelected = selectedOption($scope.whiteListOptions, $scope.settings.CommentWhiteListCount);
            $scope.blackListSelected = selectedOption($scope.blackListOptions, $scope.settings.CommentBlackListCount);

            $scope.commentsProvider = $scope.settings.ModerationType;
            if ($scope.settings.ModerationType === 0) {
                $scope.commentsProvider = 1;
                $scope.moderationEnabled = 0;
            }
            
            spinOff();
        })
        .error(function () {
            toastr.error($rootScope.lbl.errorLoadingSettings);
            spinOff();
        });
    }

    $scope.save = function () {
        if (!$('#form').valid()) {
            return false;
        }
        $scope.settings.DesktopTheme = $scope.selectedDeskTheme.OptionValue;
        $scope.settings.Culture = $scope.selectedLanguage.OptionValue;
        if ($scope.selfRegistrationInitialRole) {
            $scope.settings.SelfRegistrationInitialRole = $scope.selfRegistrationInitialRole.OptionValue;
        }
        $scope.settings.SyndicationFormat = $scope.selFeedFormat.OptionValue;
        $scope.settings.DaysCommentsAreEnabled = $scope.selCloseDays.OptionValue;
        $scope.settings.CommentsPerPage = $scope.selCommentsPerPage.OptionValue;
        $scope.settings.Timezone = $scope.selTimeZone.OptionValue;

        $scope.settings.CommentWhiteListCount = $scope.whiteListSelected.OptionValue;
        $scope.settings.CommentBlackListCount = $scope.blackListSelected.OptionValue;

        $scope.settings.txtErrorTitle = $scope.txtErrorTitle;

        $scope.settings.ModerationType = $scope.commentsProvider;
        if ($scope.moderationEnabled === 0 && $scope.commentsProvider === 1) {
            $scope.settings.ModerationType = 0;
        }

        dataService.updateItem("/api/settings", $scope.settings)
        .success(function (data) {
            toastr.success($rootScope.lbl.settingsUpdated);
            $scope.load();
        })
        .error(function () { toastr.error($rootScope.lbl.updateFailed); });
    }

    $scope.setComProvider = function (provider) {
        //$scope.settings.ModerationType = provider;
    }

    $scope.exportToXml = function() {
        location.href = SiteVars.ApplicationRelativeWebRoot + 'blogml.axd';
    }

    $scope.importClickOnce = function () {
        var url = 'http://dotnetblogengine.net/clickonce/blogimporter/blog.importer.application?url=';
        url += SiteVars.AbsoluteWebRoot + '&username=' + UserVars.Name;
        location.href = url;
    }

    $(document).ready(function () {
        $('#form').validate({
            rules: {
                txtName: { required: true },
                txtTimeOffset: { required: true, number: true },
                txtPostsPerPage: { required: true, number: true },
                txtDescriptionCharacters: { required: true, number: true },
                txtDescriptionCharactersForPosts: { required: true, number: true },
                txtRemoteFileDownloadTimeout: { required: true, number: true },
                txtRemoteMaxFileSize: { required: true, number: true },
                txtFeedAuthor: { email: true },
                txtEndorsement: { url: true },
                txtAlternateFeedUrl: { url: true },
                txtpostsPerFeed: { number: true },
                txtEmail: { email: true },
                txtSmtpServerPort: { number: true },
                txtThemeCookieName: { required: true }
            }
        });
    });

    $scope.uploadFile = function(files) {
        var fd = new FormData();
        fd.append("file", files[0]);

        dataService.uploadFile("/api/upload?action=import", fd)
        .success(function (data) {
            toastr.success($rootScope.lbl.importedFromBlogML);
        })
        .error(function () { toastr.error($rootScope.lbl.importFailed); });
    }

    $scope.testEmail = function () {
        dataService.updateItem("/api/settings?action=testEmail", $scope.settings)
        .success(function (data) {
            if (data) {
                toastr.error(data);
            }
            else {
                toastr.success($rootScope.lbl.completed);
            }
        })
        .error(function () { toastr.error($rootScope.lbl.failed); });
    }

    $scope.load();

    $scope.loadTheme = function () {
        var theme = $("#selDesktopTheme option:selected").text();
        window.location.assign("#/shared/package?id=" + theme);
    }
}]);