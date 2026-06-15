function $$LG(keyName) {
    return localStorage.getItem(keyName) == null ? '' : localStorage.getItem(keyName);
}

// JavaScript 代码
function showLoading() {
    var overlay;
    if (document.getElementById('loading-overlay')) {
        // 元素存在，执行相关逻辑
        overlay = document.getElementById('loading-overlay');
    } else {
        // 创建遮罩层
        overlay = document.createElement('div');
        overlay.id = 'loading-overlay';
        document.body.appendChild(overlay);

        // 创建旋转动画
        var spinner = document.createElement('div');
        spinner.id = 'loading-spinner';
        overlay.appendChild(spinner);
    }
    overlay.style.display = 'block';
}

//错误信息变形
function alertError(title,error,request){
    setTimeout(function(){           
        showLoading();}
        ,200);
 
    var overlay;
    if (document.getElementById('loading-alert2')) {
        // 元素存在，执行相关逻辑
        overlay = document.getElementById('loading-alert2');
    } else {
        // 创建遮罩层
        overlay = document.createElement('div');
        overlay.id = 'loading-alert2';
        document.body.appendChild(overlay);

        var divHeader=[];
        divHeader.push("<div class='div_title'>");
        divHeader.push("<span id='error_title' class='error-icon'>システムエラー</span>");
        divHeader.push("<input type='button' value='閉じる' class='btnAlertClose' id='btnAlertClose'>");
        divHeader.push("</div>");
        divHeader.push("<div class='div_body'>");
        divHeader.push("<iframe id='fraTxt'>");
        divHeader.push("</iframe>");
        divHeader.push("</div>");

        $(overlay).append($(divHeader.join("")));

        $("#btnAlertClose").click(function (event) {
            $('#loading-alert2').fadeOut(500);
            $('#loading-overlay').fadeOut(300);
        });

        //拖拽
        $(overlay).draggable({
            handle: $("#error_title"),
            drag: function () {
   
            },
            start: function () {

            },
            stop: function () {

            }
        });

        //改变尺寸
        $(overlay).resizable({
            stop: function () {

            }
        });
    }

    if(isJSON(error.responseText)){
        if($.parseJSON(error.responseText).Message){
            $("#fraTxt").contents().find('body')[0].innerHTML = jsonToHtml($.parseJSON(error.responseText));
            $("#fraTxt").contents().find('body').css('font-size','9px');
            $("#fraTxt").contents().find('body').css('color','#333');
            //$("#fraTxt").contents().find('body').css('font-weight','bolder');
            $("#fraTxt").contents().find('body').css('font-family',"'Hiragino Mincho Pro','serif'");
        }else{
            $("#fraTxt").contents().find('body').removeClass('font-size');
            $("#fraTxt").contents().find('body').html(error.responseText);
        }
    }else{
        $("#fraTxt").contents().find('body').removeClass('font-size');
        $("#fraTxt").contents().find('body').html(error.responseText);
    }

    if(request){
        $("#fraTxt").contents().find('body').html(
            $("#fraTxt").contents().find('body').html()+
             '<br>Request:'+JSON.stringify(request)
            );
    }
    if(title != '') $("#error_title").text(title+request.url);
    //overlay.style.display = 'block';
    $(overlay).fadeIn(300);
}

function jsonToHtml(json) {
    let html = [];
    for (const key in json) {
        if (json.hasOwnProperty(key)) {
            let v = json[key].replace(/場所/gi, function(match, p1) {
                return "<br>場所";
            });
            html.push(`<div><span>Name: ${key}</span><br>`);
            html.push(`<span>Value: ${v}</span></div>`);
        }
    }
    return html;
}
//Json to xml
function jsonToXml(json) {
    let xml = '';
    for (const key in json) {
        if (json.hasOwnProperty(key)) {
            xml += `<${key}>`;
            if (typeof json[key] === 'object') {
                xml += jsonToXml(json[key]);
            } else {
                xml += json[key];
            }
            xml += `</${key}>`;
        }
    }
    return xml;
}







function hideLoading() {
    $('#loading-overlay').fadeOut(100);
}

//function showLoadingAjax(request, successCallback, errorCallback) {
function showLoadingAjax(request) {
    // 创建遮罩层和旋转动画，省略代码...
    // 显示遮罩层和旋转动画
    //$('#overlay').show();
    showLoading();

    // 返回一个 Promise 对象
    return new Promise((resolve, reject) => {
        $.ajax({
            ...request,
                success: (data) => {
                    // 异步操作成功后隐藏等待效果并返回数据
                    //$('#overlay').hide();
                    hideLoading();
                    //onSuccess(data);
                    resolve(data);
                },
        error: (error) => {
            // 异步操作失败后隐藏等待效果并返回错误信息
            //$('#overlay').hide();
            hideLoading();
            //onError(error);
            reject(error);
        },
        });
});
}

//共通AJAX 呼出函数
function WebMethod(asmxName, functionName, param) {
    var rtv;
    $.ajaxSetup({ cache: false });
    $.ajax({
        type: "POST",
        url: asmxName + ".asmx/" + functionName,
        contentType: "application/json;charset=utf-8",
        async: false,//使用同步的方式,true为异步方式
        data: param,
        dataType: "json",
        success: function (data) {
            if(isJSON(data.d)){
                rtv = $.parseJSON(data.d);
            }else{
                rtv = data.d;
            }
        },
        error: function (message) {
            alert("提交失败:" + asmxName + "." + functionName + $.parseJSON(param) + message.responseText);
        }
    });
    return rtv;
}

function isJSON(str) {
    try {
        JSON.parse(str);
        return true;
    } catch (e) {
        return false;
    }
}



//btnAlertClose

//window.addEventListener('load', function () {

//    $("#btnAlertClose").click(function (event) {
//        $('#loading-alert2').fadeOut(100);
//        $('#loading-overlay').fadeOut(100);
//    });


//});