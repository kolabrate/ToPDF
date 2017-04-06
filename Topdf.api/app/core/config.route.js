(function () {
    'use strict';

    angular.module('app')
        .config(['$routeProvider', function($routeProvider) {
            var routes, setRoutes;

            routes = [
                'dashboard','dashboard/createNew',
                'ui/typography', 'ui/buttons', 'ui/icons', 'ui/grids', 'ui/widgets', 'ui/components', 'ui/boxes', 'ui/timeline', 'ui/pricing-tables', 'ui/maps',
                'table/static', 'table/dynamic', 'table/responsive',
                'form/elements', 'form/layouts', 'form/validation', 'form/wizard','form/signUpWiz',
                'chart/echarts', 'chart/echarts-line', 'chart/echarts-bar', 'chart/echarts-pie', 'chart/echarts-scatter', 'chart/echarts-more',
                'page/404', 'page/500', 'page/blank', 'page/forgot-password', 'page/invoice', 'page/lock-screen', 'page/profile', 'page/invoice', 'page/signin', 'page/signup',
                'app/tasks', 'app/calendar'
            ]

            setRoutes = function(route) {
                var config, url;
                url = '/' + route;
                config = {
                    templateUrl: 'app/' + route + '.html'
                };
                $routeProvider.when(url, config);
                return $routeProvider;
            };

            routes.forEach(function(route) {
                return setRoutes(route);
            });

            $routeProvider
                .when('/', {redirectTo: '/dashboard'})
                .when('/dashboard', {templateUrl: 'app/dashboard/dashboard.html'})
                .when('/404', {templateUrl: 'app/page/404.html'})
                .otherwise({ redirectTo: '/404'});

        }]
    ).run( function($rootScope, $location) {

        // register listener to watch route changes
        $rootScope.$on( "$routeChangeStart", function(event, next, current) {
            if ($rootScope.loggedUser == null) {
                var publicRoutes = [
                    'app/page/signin.html', 'app/page/signup.html', 'app/page/forgot-password.html', 'app/form/signUpWiz.html'
                ];
                if (publicRoutes.indexOf(next.templateUrl)<=0) {
                    //$location.path("page/signin");
                }
            }         
        });
    });
})(); 