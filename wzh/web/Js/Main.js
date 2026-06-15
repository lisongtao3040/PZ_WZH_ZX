//尺寸设定
function SizeInit() {
    $("#dialog_pd").height($(window).height());
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

    //var allIlikeTextBoxCheckResult = true;

    ////检查属性是否输入
    //$(".Ilike_TextBox").each(function () {
    //    var InputType = $(this).attr("InputType");
    //    var msg = "";
    //    if (InputType == undefined || InputType == '') {
    //        msg = $(".Ilike_TextBox").attr("id") + "的 InputType 未输入";
    //     }
    //    if (msg != '') {
    //        $(this).css("background", "red");
    //        alert(msg);
    //        allIlikeTextBoxCheckResult = false;
    //        return false;
    //    } else {
    //        return true;
    //    }
    //});

    //if (allIlikeTextBoxCheckResult) {


    /*失去焦点检查*/
    $(".Ilike_TextBox").blur(function () {
        var that = $(this);
        if ($(this).val() == "") {
            return true;
        }

        var InputType = $(this).attr("InputType");

        if (InputType == undefined) {
            alert($(this).attr("id") + "的 InputType 未输入");

        } else if (InputType == "Normal") {

        } else if (InputType == "Number") {
            if (!chkHankakuSuuji($(this).val())) {
                alert2("【" + $(this).attr("InputName") + "】- 请输入半角数字！", function () { that.focus(); });
                thatResult = false;
                return false;// break
            }

        } else if (InputType == "English") {
            if (!chkHankakuEi($(this).val())) {
                alert2("【" + $(this).attr("InputName") + "】- 请输入半角英字！", function () { that.focus(); });
                thatResult = false;
                return false;// break
            }

        } else if (InputType == "EnglishNumber") {
            if (!chkHankakuEisuuji($(this).val())) {
                alert2("【" + $(this).attr("InputName") + "】- 请输入半角英数字！", function () { that.focus(); });
                thatResult = false;
                return false;// break
            }

        }

        return true;

    });

    /*失去焦点检查*/
    $(".Ilike_TextBox").change(function () {
        //必须入力检查
        return InputMustInput(this);
    });

    $("form").submit(function (e) {

    });

    //尺寸设定
    SizeInit();
    //取消覆盖
    $("#cover").hide();

});

function CheckAllInput() {
    var rtv = true;
    $(".Ilike_TextBox").each(function () {
        //必须入力检查
        if (!InputMustInput(this)) { rtv = false; return false; }
    });
    return rtv;
}

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


/*自定义消息对话框*/
function alert2(msg, okfnc, errType,text_color) {
    //.ui-dialog-title
    $("#dialog_pd").show();
    //❌	‼	⁉	❗ ❓
    if (errType == undefined) {
        errType = "○";
    }




    $("#dialog").attr("title","" + errType + " Infomation");
    $("#dialogMsg").html(msg);

    $("#dialog").dialog({
        dialogClass: "no-close",
        width: $(window).width() * 0.6,
        height: $(window).height() * 0.6,
        buttons: [
          {
              text: "OK",
              click: function () {
                  $(this).dialog("close");
                  $("#dialog_pd").hide();
                  if (okfnc == undefined) {

                  } else {
                      setTimeout(function () { okfnc(); }, 0);
                  }
                  

              }
          }
        ]
    });

    $("#dialogMsg").css("color", text_color);
    $("#ui-id-1").css("color", text_color);

    $(".ui-dialog").css("zIndex", "99999");
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

    //return isNumber(str);
}

///**
//* 校验只要是数字（包含正负整数，0以及正负浮点数）就返回true
//**/

//function isNumber(val) {

//    var regPos = /^\d+(\.\d+)?$/; //非负浮点数
//    var regNeg = /^(-(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*)))$/; //负浮点数
//    if (regPos.test(val) && regNeg.test(val)) {
//        return true;
//    } else {
//        return false;
//    }

//}

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