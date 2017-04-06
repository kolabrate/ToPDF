(function () {
    'use strict';

    angular.module('app.ui.form.validation')
        .controller('signUpWizCtrl', ['$scope','$http', signUpWizCtrl])
        .controller('emailVerifyCtrl', ['$scope','$http','$routeParams', emailVerifyCtrl])

    function emailVerifyCtrl($scope, $http, $routeParams) {
        $scope.emailVerified = false;
        var param1 = $routeParams.Parameter1;
        $http.post('api/topdf/VerifyEmail', param1).then(function (resp) {
            $scope.emailVerified = true;
        }, function (err) {
            console.log(JSON.stringify(err));
        });
    }

    function signUpWizCtrl($scope, $http) {
        var original;
        $scope.onFirstStep = true;
        $scope.onSecondStep = false;
        $scope.onThirdStep = false;
        $scope.finished = false;
        $scope.cannotCreateUser = false;

        $scope.user = {
            email: '',
            password: '',
            confirmPassword: '',
            firstName: '',
            lastName: '',
            companyName: '',
            companyUrl: '',
            companyPh: '',
            addr1: '',
            addr2: '',
            city: '',
            state: '',
            country: '',
            postCode: '',
            avatar: '',
        };
        $scope.subcriptions = [];

        original = angular.copy($scope.user);

        $scope.revert = function () {
            $scope.user = angular.copy(original);
            $scope.form_signup1.$setPristine();
            return $scope.form_signup1.confirmPassword.$setPristine();
        };

        $scope.canRevert = function () {
            if ($scope.form_signup1 != null) {
                return !angular.equals($scope.user, original) || !$scope.form_signup1.$pristine;
            }
        };

        $scope.canSubmit = function () {
            if ($scope.form_signup1 != null) {
                return $scope.form_signup1.$valid && !angular.equals($scope.user, original);
            }
        };

        $scope.submitForm_1 = function () {
            if ($scope.form_signup1.$valid) {
                $scope.onFirstStep = false;
                $scope.onSecondStep = true;
                $scope.onThirdStep = false;
            }
        };

        $scope.submitForm_2 = function () {
            if ($scope.form_signup2.$valid) {
                $scope.onFirstStep = false;
                $scope.onSecondStep = false;
                $scope.onThirdStep = true;
            }
            $scope.getSubscriptions();
        };

        $scope.submitForm_3 = function () {
            if ($scope.form_signup3.$valid) {
                $scope.onFirstStep = false;
                $scope.onSecondStep = false;
                $scope.onThirdStep = false;

                $http.post('api/topdf/CreateUser', user).then(function (resp) {
                    $scope.finished = true;
                }, function (err) {
                    console.log(JSON.stringify(err));
                });
            }
        };

        $scope.getSubscriptions = function () {
            $http.get('api/topdf/Subscriptions', value).then(function (resp) {
                $scope.subcriptions = resp['data'];
            }, function (err) {
                console.log(JSON.stringify(err));
            });
        };
    }
})(); 