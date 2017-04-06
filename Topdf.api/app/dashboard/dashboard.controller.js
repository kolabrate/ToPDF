(function () {
    'use strict';

    angular.module('app')
            //.config(['ngQuillConfigProvider', function (ngQuillConfigProvider) {
            //    ngQuillConfigProvider.set();
            //}])
        .controller('DashboardCtrl', ['$scope', '$rootScope', '$http','$location', DashboardCtrl])
    .controller('createNewCtrl', ['$scope', '$rootScope', '$http',  createNewCtrl])

    function DashboardCtrl($scope, $rootScope,$http, $location) {

        $scope.templates = {};
        $scope.user = {};
        $scope.messages = {};
        $scope.subscriptions = {};
        var userId = $rootScope.loggedUser;
        $http.get('api/topdf/Dashboard', userId).then(function (resp) {
            $scope.user = resp['Data'][0];
            $scope.SuccessCount = resp['Data'][1];
            $scope.ErrorCount = resp['Data'][1];
            $scope.messages = resp['Data'][2];
            $scope.templates = resp['Data'][3];
            $scope.subscriptions = resp['Data'][4];
        }, function (err) {
            console.log(JSON.stringify(err));
        });

        $scope.createNew = function () {
            $location.path("dashboard/createNew");
        };

        $scope.getChart = function (val1) {
            var val2 = 100 - val1;

            return pie = {
                calculable: true,
                series: [
                    {
                        name: 'Last One Week',
                        type: 'pie',
                        radius: ['50%', '70%'],
                        itemStyle: {
                            normal: {
                                label: {
                                    show: false
                                },
                                labelLine: {
                                    show: false
                                }
                            },
                        },
                        data: [
                            { value: val1, name: 'Success' },
                            { value: val2, name: 'Error' }
                        ]
                    }
                ]
            };
        }
    }

    function createNewCtrl($scope, $rootScope, $http) {
        $scope.fontFamily = ["Arial", "Comic Sans MS", "Courier New", "Georgia", "Lucida Sans Unicode", "Tahoma", "Times New Roman", "Trebuchet MS", "Verdana"];
        $scope.text = ["Normal", "Header 1", "Header 2", "Header 3", "Header 4", "Header 5", "Header 6"];
        $scope.fontSelected = "Arial";
        $scope.textSelected = "Normal";
        $scope.lines = [];
        $scope.onFirstStep = true;
        $scope.onSecondStep = false;
        var editor = $('.editor');

        $scope.pdfTemplate = {
            ContactEmail: '',
            DeliveryModeId: '',
            Desc: '',
            EmailErrorTo: '',
            InputType: '',
            PdfTemplateName: '',
            SampleData: '<ProofOfDeliveryNotification><MessageHeader><CreationDateTime/><RecipientParty><InternalID>0001301850</InternalID>		</RecipientParty>	</MessageHeader>	<PODNotificationDetail>		<ConsignmentNumber>7838909</ConsignmentNumber>		<DeliveryDate>20161214</DeliveryDate>		<TimeOfDelivery>225818</TimeOfDelivery>		<URL>http://vmsapdpd.lfxau.linfox.com:50000/TrackTrace/index.html?trackid=00000000101050081337</URL>        <DeliveryList>			<DeliveryNumber>00000000101050081337</DeliveryNumber>		</DeliveryList>	</PODNotificationDetail></ProofOfDeliveryNotification>',
            UserId: '',
            TemplateSections: '',
        };
        $scope.sampleXml = {};

        $scope.submitStep1 = function () {
            $scope.onFirstStep = false;
            $scope.onSecondStep = true;

            $http({
                method: 'POST',
                url: 'api/topdf/FormatData',
                headers: {
                    'Content-Type': 'text/xml'
                },
                data: $scope.pdfTemplate.SampleData,
            }).success(function (data, status, headers, config) {
                $scope.roleList = data;
            }).error(function (data, status, headers, config) {

            });
        };

        $scope.getHtml = function () {
            var lines = [];
            var html = $.parseHTML(quill.root.innerHTML);
            $.each(html, function (i, el) {
                var lineType = $scope.getNodeName(el);

                var line = {
                    lineType: lineType,
                    fontSize: "10px",
                    fontWeight: "bold",
                    value: "Hello World!",
                    fontColor: "red",
                    backColor: "white",
                    align: "left",
                    indent: "10"
                };
                el.children().each(function () {

                });

                lines.push(line);
            });
        };
        $scope.getChildLines = function (el) {
            line = {
                lineType: "Text",
                fontSize: "10px",
                fontWeight: "bold",
                value: "Hello World!",
                fontColor: "red",
                backColor: "white",
                align: "left",
                indent: "10"
            };
            el.children().each(function () {

            });
        };
        $scope.getNodeName = function () {

        };
        $scope.i = 0;
        $scope.buttonClicked1 = function (btnType) {
            //if (btnType == 'Text') {                
            //    var newdiv = document.createElement('div');
            //    newdiv.innerHTML = '<button type="button" class="btn btn-line-primary dragThis">Add text here...</button>';
            //    document.getElementById('editor').appendChild(newdiv);
            //    counter++;
            //}
            $scope.i = $scope.i + 1;
            var i = $scope.i;
            var li = $('<li>');
            //var btn = $("<div type='button' tabindex='-1' class='matrix-item draggable' data-value='" + i + "' title='" + "My title" + "'> Button " + i + "</div>").draggable({ containment: "#container" });
            var btn = $("<a href='javascript:;' class='dropdown-toggle' uib-dropdown-toggle data-toggle='dropdown'>Add Text here..</a>").draggable({ containment: "#container" });
            li.append(btn);
            $(".holder ul").append(li);
        };
        $scope.buttonClicked = function (btnType) {
            if (btnType == 'Text') {
                var line = {
                    lineType: "Text",
                    fontSize: "10px",
                    fontWeight: "bold",
                    fontColor: "red",
                    backColor: "white",
                    align: "left",
                    x: 0,
                    y:0,
                    text: "Add Text here..."
                };
                $scope.lines.push(line);
            }
            $('.draggable').draggable();

        };
        $scope.onstop = function (event, ui, line ) {
            line['x'] = event['offsetX'];
            line['y'] = event['offsetY'];
            console.log(line);
        };

        $scope.preview = function () {

            var html = quill.root.innerHTML;

            $http({
                method: 'POST',
                url: 'api/topdf/Preview',                
                headers: {
                    "Pragma": "no-cache",
                    "Expires": -1,
                    "Cache-Control": "no-cache"
                },
                data: html,
                responseType: 'arraybuffer'
            }).success(function (data, status, headers) {
                headers = headers();

                var filename = headers['x-filename'];
                var contentType = headers['content-type'];

                var linkElement = document.createElement('a');
                try {
                    var blob = new Blob([data], { type: contentType });
                    if (navigator.msSaveBlob) {
                        navigator.msSaveBlob(blob, filename);
                    }
                    else {
                        var url = window.URL.createObjectURL(blob);

                        linkElement.setAttribute('href', url);
                        linkElement.setAttribute("download", filename);

                        var clickEvent = new MouseEvent("click", {
                            "view": window,
                            "bubbles": true,
                            "cancelable": false
                        });
                        linkElement.dispatchEvent(clickEvent);
                    }
                    if (funcOnSuccess != null) {
                        funcOnSuccess();
                    }
                } catch (ex) {
                    console.log(JSON.stringify(ex));
                }
            }).error(function (data) {
                console.log(JSON.stringify(data));
            });
        };
    }   
})();
var curVal = 'nil';
function dragStart(event) {    
    var innerSpan = event.target.getElementsByTagName("span")[0];
    curVal = '{{'+innerSpan.getAttribute('data-path')+'}}';
}

function allowDrop(event) {
    event.preventDefault();
}

function drop(event) {
    event.preventDefault();
    //if (quill.getText() == defaultTxt) {
    //    quill.deleteText(0, defaultTxt.length);
    //}
    if (curVal != null) {
        var pos = quill.getSelection();
        quill.insertText(pos, curVal);
    }
}

function showImageUI()
{
    alert('this will be my image window');
}
$(document).ready(function () {
    $(".draggable").draggable({ axis: 'y', containment: '#container' });
    $(".droppable").droppable({
        drop: function (event, ui) {
            console.log('drop happened');
        }
    });
});

//var myAppModule = angular.module('quillTest', ['ngQuill']);
//myAppModule.config(['ngQuillConfigProvider', function (ngQuillConfigProvider) {
//    ngQuillConfigProvider.set();
//}]);
//myAppModule.controller('AppCtrl', [
//    '$scope',
//    '$timeout',
//    function ($scope, $timeout) {
//        $scope.title = '';
//        $scope.changeDetected = false;

//        $scope.editorCreated = function (editor) {
//            console.log(editor);
//        };
//        $scope.contentChanged = function (editor, html, text) {
//            $scope.changeDetected = true;
//            console.log('editor: ', editor, 'html: ', html, 'text:', text);
//        };
//    }
//]);