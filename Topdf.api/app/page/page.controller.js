(function () {
    'use strict';

    angular.module('app.page')
        .controller('signInCtrl', ['$scope', '$window', '$http', signInCtrl])
        .controller('forgotPwdCtrl', ['$scope', '$window', '$http', forgotPwdCtrl])
        .controller('signUpCtrl', ['$scope', '$window', signUpCtrl])
        .controller('invoiceCtrl', ['$scope', '$window', invoiceCtrl])
        .controller('authCtrl', ['$scope', '$window', '$location', authCtrl])
    ;
    function signInCtrl($scope, $window, $http) {
        $scope.signInError = false;

        $scope.resetPwd = function () {            
            if ($scope.userName == null || $scope.userName.length == 0) {
                $scope.signInError = true;
            }
            if ($scope.password == null || $scope.password.length == 0) {
                $scope.signInError = true;
            }
            if (!$scope.signInError) {
                var pwdEncoded = $scope.encode($scope.password);
                var value = {
                    Email: $scope.userName,
                    Password: pwdEncoded
                };

                //$http.post('http://topdfdev.azurewebsites.net/api/topdf/login', value).then(function (resp) {
                $http.post('api/topdf/login', value).then(function (resp) {
                    console.log(JSON.stringify(resp));
                }, function (err) {
                    $scope.signInError = true;
                    console.log(JSON.stringify(err));
                });
            }
        }
        $scope.encode = function (input) {
            var keyStr = 'ABCDEFGHIJKLMNOP' +
                'QRSTUVWXYZabcdef' +
                'ghijklmnopqrstuv' +
                'wxyz0123456789+/' +
                '=';

            var output = "";
            var chr1, chr2, chr3 = "";
            var enc1, enc2, enc3, enc4 = "";
            var i = 0;

            do {
                chr1 = input.charCodeAt(i++);
                chr2 = input.charCodeAt(i++);
                chr3 = input.charCodeAt(i++);

                enc1 = chr1 >> 2;
                enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
                enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
                enc4 = chr3 & 63;

                if (isNaN(chr2)) {
                    enc3 = enc4 = 64;
                } else if (isNaN(chr3)) {
                    enc4 = 64;
                }

                output = output +
                        keyStr.charAt(enc1) +
                        keyStr.charAt(enc2) +
                        keyStr.charAt(enc3) +
                        keyStr.charAt(enc4);
                chr1 = chr2 = chr3 = "";
                enc1 = enc2 = enc3 = enc4 = "";
            } while (i < input.length);

            return output;
        };

        $scope.decode = function (input) {
            var keyStr = 'ABCDEFGHIJKLMNOP' +
                'QRSTUVWXYZabcdef' +
                'ghijklmnopqrstuv' +
                'wxyz0123456789+/' +
                '=';
            var output = "";
            var chr1, chr2, chr3 = "";
            var enc1, enc2, enc3, enc4 = "";
            var i = 0;

            // remove all characters that are not A-Z, a-z, 0-9, +, /, or =
            var base64test = /[^A-Za-z0-9\+\/\=]/g;
            if (base64test.exec(input)) {
                alert("There were invalid base64 characters in the input text.\n" +
                        "Valid base64 characters are A-Z, a-z, 0-9, '+', '/',and '='\n" +
                        "Expect errors in decoding.");
            }
            input = input.replace(/[^A-Za-z0-9\+\/\=]/g, "");

            do {
                enc1 = keyStr.indexOf(input.charAt(i++));
                enc2 = keyStr.indexOf(input.charAt(i++));
                enc3 = keyStr.indexOf(input.charAt(i++));
                enc4 = keyStr.indexOf(input.charAt(i++));

                chr1 = (enc1 << 2) | (enc2 >> 4);
                chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
                chr3 = ((enc3 & 3) << 6) | enc4;

                output = output + String.fromCharCode(chr1);

                if (enc3 != 64) {
                    output = output + String.fromCharCode(chr2);
                }
                if (enc4 != 64) {
                    output = output + String.fromCharCode(chr3);
                }

                chr1 = chr2 = chr3 = "";
                enc1 = enc2 = enc3 = enc4 = "";

            } while (i < input.length);

            return output;
        }
    }

    function forgotPwdCtrl($scope, $window, $http) {
        $scope.forgotPwdError = false;
        $scope.isEmailValid = true;

        $scope.resetPwd = function () {
            $scope.forgotPwdError = false;
            $scope.isEmailValid = true;
            $scope.showConfirmation = false;

            if ($scope.resetEmail == null || $scope.resetEmail.length == 0) {
                $scope.isEmailValid = false;
            }
            if (!$scope.forgotPwdError && $scope.isEmailValid) {
                //$http.post('http://topdfdev.azurewebsites.net/api/topdf/login', value).then(function (resp) {
                $http.post('api/topdf/ResetPwdAndEmail', $scope.resetEmail).then(function (resp) {
                    $scope.showConfirmation = true;
                }, function (err) {
                    $scope.forgotPwdError = true;
                    console.log(JSON.stringify(err));
                });
            }
        }
    }

    function signUpCtrl($scope, $window) {
        $scope.signUp = function () {

        };
    }

    function invoiceCtrl($scope, $window) {
        var printContents, originalContents, popupWin;

        $scope.printInvoice = function () {
            printContents = document.getElementById('invoice').innerHTML;
            originalContents = document.body.innerHTML;
            popupWin = window.open();
            popupWin.document.open();
            popupWin.document.write('<html><head><link rel="stylesheet" type="text/css" href="styles/main.css" /></head><body onload="window.print()">' + printContents + '</html>');
            popupWin.document.close();
        }
    }

    function authCtrl($scope, $window, $location) {
        $scope.login = function () {
            $location.url('/')
        }

        $scope.signup = function () {
            $location.url('/')
        }

        $scope.reset = function () {
            $location.url('/')
        }

        $scope.unlock = function () {
            $location.url('/')
        }
    }


})();



