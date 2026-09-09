//尺寸设定
function SizeInit() {
    $("#cover").height($(window).height());
}
function CoverIt() {
    $("#cover").height($(window).height());
    $("#cover").show();
}
$(document).ready(function () {

    var userAgent = window.navigator.userAgent.toLowerCase();
    if (userAgent.indexOf('msie') != -1 ||
            userAgent.indexOf('trident') != -1) {
        console.log('Internet Explorerをお使いですね');
        alert("请使用谷歌浏览器！Chrome！");
    } else if (userAgent.indexOf('edge') != -1) {
        console.log('Edgeをお使いですね');
        alert("请使用谷歌浏览器！Chrome！");
    } else if (userAgent.indexOf('chrome') != -1) {
        console.log('Google Chromeをお使いですね');
    } else if (userAgent.indexOf('safari') != -1) {
        console.log('Safariをお使いですね');
        alert("请使用谷歌浏览器！Chrome！");
    } else if (userAgent.indexOf('firefox') != -1) {
        console.log('FireFoxをお使いですね');
        alert("请使用谷歌浏览器！Chrome！");
    } else if (userAgent.indexOf('opera') != -1) {
        console.log('Operaをお使いですね');
        alert("请使用谷歌浏览器！Chrome！");
    } else {
        console.log('そんなブラウザは知らん');

    }



    $("form").submit(function (e) {

    });

    //尺寸设定
    SizeInit();
    //取消覆盖
    $("#cover").hide();

});



//输入框 必须入力检查
function InputMustInput(e) {
    var that = $(e);
    var InputType = that.attr("MustInput");
    if (InputType == undefined) {
    } else {
        if (InputType == "IsTrue") {
            if (IsEmpty($(e).val())) {
                alert2("【" + $(e).attr("InputName") + "】- 必须输入！", function () { that.focus(); });
                rtv = false;
                return false;
            }
        }
    }
    return true;
}

$(window).resize(function () {
    //尺寸设定
    SizeInit();

});


/*自定义消息对话框（动态生成HTML，不依赖页面上的 #dialog/#dialogMsg）*/
function alert2(msg, okfnc, errType, text_color) {
    //❌ ‼ ⁉ ❗ ❓
    var type = (errType == undefined) ? "○" : errType;

    //errType → 主题色 + 副标题
    var theme = {
        "○": { color: "#095074", text: "Information" },
        "❌": { color: "#e74c3c", text: "Error" },
        "‼": { color: "#e67e22", text: "Warning" },
        "❗": { color: "#e67e22", text: "Warning" },
        "⁉": { color: "#e67e22", text: "Warning" },
        "❓": { color: "#095074", text: "Confirm" },
        "?": { color: "#095074", text: "Confirm" }
    };
    var s = theme[type] || { color: "#095074", text: "Information" };

    //先移除上次弹窗，避免叠加残留
    $(".jq-alert-layer").remove();

    var msgColor = (text_color == undefined) ? "#333" : text_color;

    var html = ""
        + '<div class="jq-alert-layer" style="position:fixed;left:0;top:0;width:100%;height:100%;background:rgba(0,0,0,0.45);z-index:999999;display:flex;align-items:center;justify-content:center;">'
        +   '<div style="width:420px;max-width:92%;background:#fff;border-radius:12px;box-shadow:0 15px 50px rgba(0,0,0,0.35);overflow:hidden;animation:jqAlertIn 0.2s ease-out;font-family:Meiryo,\'Microsoft YaHei\',sans-serif;">'
        +       '<div style="height:6px;background:' + s.color + ';"></div>'
        +       '<div style="text-align:center;padding:24px 24px 0 24px;">'
        +           '<div style="font-size:48px;line-height:1.3;">' + type + '</div>'
        +           '<div style="color:' + s.color + ';font-size:15px;letter-spacing:2px;margin-top:2px;">' + s.text + '</div>'
        +       '</div>'
        +       '<div style="padding:14px 28px 8px 28px;text-align:center;font-size:22px;font-weight:bold;line-height:1.6;word-break:break-all;color:' + msgColor + ';">' + msg + '</div>'
        +       '<div style="text-align:center;padding:16px 24px 26px 24px;">'
        +           '<button type="button" style="width:170px;height:44px;background:' + s.color + ';color:#fff;font-size:20px;font-weight:bold;border:none;border-radius:6px;cursor:pointer;box-shadow:0 3px 8px rgba(0,0,0,0.2);">OK</button>'
        +       '</div>'
        +   '</div>'
        + '</div>';

    var $layer = $(html).appendTo(document.body);

    //弹出动画关键帧（只注入一次）
    if ($("#jqAlertCss").length == 0) {
        $("<style id='jqAlertCss'>@keyframes jqAlertIn{from{transform:scale(0.85);opacity:0;}to{transform:scale(1);opacity:1;}}</style>").appendTo("head");
    }

    //OK 关闭
    $layer.find("button").click(function () {
        $layer.fadeOut(120, function () {
            $layer.remove();
            if (okfnc != undefined) {
                setTimeout(function () { okfnc(); }, 0);
            }
        });
    });

    $layer.find("button").focus();
}

/**
 * String.Trim
 * 文字列のトリム処理
 * @return
 */
String.prototype.Trim = function () { return this.replace(/^\s+|\s+$/g, ""); }


//判断空
function IsEmpty(value) {
    try {
        if (value.Trim() == "") {
            return true;
        } else {
            return false;
        }
    } catch (e) {
        alert("FILE:Main.js/" + "Function:IsEmpty/" + e.message);
        return false;
    }
}

//半角英数字チェック（a～zとA～Zと0～9）
function chkHankakuEisuuji(strInputString) {
    if (strInputString.match(/[^a-z\^A-Z\^0-9]/) != null) {
        return false;
    }
    else {
        return true;
    }
}


//半角英数字チェック（a～zとA～Zと0～9）
function chkHankakuEi(strInputString) {
    if (strInputString.match(/[^a-z\^A-Z]/) != null) {
        return false;
    }
    else {
        return true;
    }
}


/* ************************************************************************************************
	関数名 chkHankakuSuuji()
		作成日 : 2000/03/23
		引数   : 1番目      -> チェックする文字列
		戻り値 : true or false(エラーの場合falseを返す)
		概要   : 半角数字チェック
				 半角数字(0～9)以外ならエラー表示
************************************************************************************************ */
function chkHankakuSuuji(str) {
    var ch;
    var wkstr = str;
    for (i = 0; i < wkstr.length; i++) {
        ch = wkstr.substring(i, i + 1);
        if (!(ch >= "0" && ch <= "9") && ch != ".") {
            return false;
        }
    }

    if ((str.split('.')).length - 1 > 1) {
        return false;
    }

    if (parseFloat(str).toString() == "NaN") {
        return false;
    } else {
        return true;
    }

}

function jqFanYe(idx)
{
    $(".jqFanYeIdx").val(idx);
    $(".jqFanYe").click()
}

function isNumber(value) {
    if (value === undefined || value === null || value === '') {
        return false
    }
    if (typeof (value) === 'string') {
        //正整数
        var reNumber = /^\d+$/
        //负整数
        var reNeNumber = /^-\d+$/
        //正实数
        var reRealNumber1 = /^[1-9]\d*[.]\d+$/  //非零开头
        var reRealNumber2 = /^0[.]\d+$/ //零开头
        //负实数
        var reNeRealNumber1 = /^-[1-9]\d*[.]\d+$/  //非零开头
        var reNeRealNumber2 = /^-0[.]\d+$/ //零开头

        if (reNumber.test(value) || reNeNumber.test(value)
        || reRealNumber1.test(value) || reRealNumber2.test(value)
        || reNeRealNumber1.test(value) || reNeRealNumber2.test(value)) {
            return true
        }
        else {
            return false
        }
    }
    else if (typeof (value) === 'number') {
        return true
    }
    else {
        return false
    }
}