function isInteger(obj) {
    return obj % 1 === 0 && obj.indexOf(".") == -1 && obj != '';
}
String.prototype.trim = function () {
    return this.replace(/(^\s*)|(\s*$)/g, '');
}
String.prototype.ltrim = function () {
    return this.replace(/(^\s*)/g, '');
}
String.prototype.rtrim = function () {
    return this.replace(/(\s*$)/g, '');
}
String.prototype.right = function (length) {
    if (this.length - length >= 0 && this.length >= 0 && this.length - length <= this.length) {
        return this.substring(this.length - length, this.length);
    } else {
        return this
    }
}
String.prototype.left = function (length) {
    if (this.length - length >= 0 && this.length >= 0 && this.length - length <= this.length) {
        return this.substring(0, length);
    } else {
        return this
    }
}

Date.prototype.Format = function (formatStr) {
    var str = formatStr;
    var Week = ['日', '一', '二', '三', '四', '五', '六'];

    str = str.replace(/yyyy|YYYY/, this.getFullYear());
    str = str.replace(/yy|YY/, (this.getYear() % 100) > 9 ? (this.getYear() % 100).toString() : '0' + (this.getYear() % 100));

    str = str.replace(/MM/, this.getMonth() > 9 ? this.getMonth().toString() : '0' + this.getMonth());
    str = str.replace(/M/g, this.getMonth());

    str = str.replace(/w|W/g, Week[this.getDay()]);

    str = str.replace(/dd|DD/, this.getDate() > 9 ? this.getDate().toString() : '0' + this.getDate());
    str = str.replace(/d|D/g, this.getDate());

    str = str.replace(/hh|HH/, this.getHours() > 9 ? this.getHours().toString() : '0' + this.getHours());
    str = str.replace(/h|H/g, this.getHours());
    str = str.replace(/mm/, this.getMinutes() > 9 ? this.getMinutes().toString() : '0' + this.getMinutes());
    str = str.replace(/m/g, this.getMinutes());

    str = str.replace(/ss|SS/, this.getSeconds() > 9 ? this.getSeconds().toString() : '0' + this.getSeconds());
    str = str.replace(/s|S/g, this.getSeconds());

    return str;
}

function DateDiff(date1, date2) {

    var a = date1.Format("YYYY-MM-DD");
    var b = date2.Format("YYYY-MM-DD");

    var difValue = (date2 - date1) / (1000 * 60 * 60 * 24);
    return Math.floor(difValue);
}

//function DateDiff(interval, date1, date2) {
//    var objInterval = {
//        'D': 1000 * 60 * 60 * 24, 'H': 1000 * 60 * 60,
//        'M': 1000 * 60, 'S': 1000, 'T': 1
//    };
//    interval = interval.toUpperCase();
//    var dt1 = Date.parse(date1.Format("YYYY-MM-DD").replace(/-/g, '/'));
//    var dt2 = Date.parse(date2.Format("YYYY-MM-DD").replace(/-/g, '/'));
//    try {
//        return Math.round((dt2 - dt1) / eval('(objInterval.' + interval + ')'));
//    }
//    catch (e) {
//        return e.message;
//    }
//}




$(document).ready(function () {

    var correctLevel = QRCode.CorrectLevel.L;
    if ($("#hidQR1").val() != '') {
        document.getElementById("QR1").innerHTML = "";
        try {
            var qrcode = new QRCode(document.getElementById("QR1"), {
                text:"http://10.160.192.131/Wzh2023/Snap.aspx?no="+ $("#hidQR1").val(),
                width: 200,
                height: 200,
                colorDark: "#000000",
                colorLight: "#ffffff",
                correctLevel: correctLevel
            });
        } catch (e1) { }
    }




    var that = this;
    this.cd = $("#tbxCd").text();
    this.no = $("#tbxNo").text();

    var oldImgWidth = 0;

    this.Ajax = function (fncName, jsonData) {
        var rtv = "";
        var thatArguments = arguments;
        var ele = that.Cover();
        ele.appendTo("body");
        $.ajax({
            type: "POST",
            url: "Result.asmx/" + fncName,
            async: false,
            contentType: "application/json;charset=utf-8",
            data: jsonData,
            dataType: "json",
            success: function (data) {
                rtv = {
                    result: 'OK', data: jQuery.parseJSON(data.d), msg: ''
                };
            },
            error: function (message) {
                alert(message);
                //throw new Error("AJAX 出错");
                rtv = {
                    result: 'NG', data: null, msg: message.responseText.split('\\r\\n').join('<br>').split('\\u0027').join('\'')
                };
                
            }
        });
        setTimeout(function () {
            ele.remove();
        }, 200);
        return rtv;
    };
    this.Cover = function () {
        var arr = [];
        arr.push("<div id='cover' style='position: absolute; width: 100%; height: 100%; left: 0px; top: 0px; z-index: 100000; opacity: 0.5; background-color: #ddd; text-align: center;'>");
        arr.push("</div>");
        return $(arr.join(""));
    }
    //Ajax 是否返回有数据
    this.IsRtvNoData = function (rtv) {
        if (rtv == "") {
            return true;
        } else if (rtv == "AJAX_NG") {
            return true;
        } else if (rtv.length == 0) {
            return true;
        } else {
            return false;
        }
        //that.GetSelWhere()
    };

    this.AllRltTxt = function () {
        var rtvCnt;
        var loujian;
        $.ajax({
            type: "POST",
            url: "Result.asmx/" + "GetResultSum",
            async: false,
            contentType: "application/json;charset=utf-8",
            data: JSON.stringify({
                ck_id: $("#tbxCk_id").text(),
                ky: new Date(),

            }),
            dataType: "json",
            success: function (data) {


                var map = {};
                var cnt = 0;

                map["合"] = data.d.split(",")[0];
                map["微"] = data.d.split(",")[1];
                map["警"] = data.d.split(",")[2];
                map["轻"] = data.d.split(",")[3];
                map["中"] = data.d.split(",")[4];
                map["重"] = data.d.split(",")[5];
                map["误"] = data.d.split(",")[6];
                map["漏检"] = data.d.split(",")[7];

                rtvCnt = 0;
                rtvCnt = rtvCnt + parseInt(data.d.split(",")[3]);
                rtvCnt = rtvCnt + parseInt(data.d.split(",")[4]);
                rtvCnt = rtvCnt + parseInt(data.d.split(",")[5]);
                rtvCnt = rtvCnt + parseInt(data.d.split(",")[6]);
                rtvCnt = rtvCnt + parseInt(data.d.split(",")[7]);

                loujian = parseInt(data.d.split(",")[7]);
                //$(".JQ_JIEGUO").each(function () {
                //    //合轻微中重警
                //    if (map[$(this).text().trim()]) {
                //        map[$(this).text().trim()] += 1;
                //    } else {
                //        map[$(this).text().trim()] = 1;
                //    }
                //    cnt++;
                //});

                var arr = [];
                arr.push(that.GetKmRltSuu('合', map));
                arr.push(that.GetKmRltSuu('微', map));
                arr.push(that.GetKmRltSuu('警', map));
                arr.push(that.GetKmRltSuu('轻', map));
                arr.push(that.GetKmRltSuu('中', map));
                arr.push(that.GetKmRltSuu('重', map));
                arr.push(that.GetKmRltSuu('误', map));
                arr.push(that.GetKmRltSuu('漏检', map, '漏检'));
                arr.push("/" + data.d.split(",")[8]);

                $("#SumRlt").html(arr.join(" "));

            },
            error: function (message) {
                rtv = {
                    result: 'NG', data: null, msg: message.responseText.split('\\r\\n').join('<br>').split('\\u0027').join('\'')
                };
            }
        });

        return { rtvCnt: rtvCnt, loujian: loujian };

    };


    $("#btnComplete").click(function () {

        var rtv;
        rtv = that.AllRltTxt();
        if (rtv.loujian > 0) {
            alert("有漏检，不能完了");
            return false;
        }

        if (rtv.rtvCnt > 0) {
            if (confirm($("#SumRlt").text() + "   有NG项目，要完了么？")) {
                return true;
            } else {
                event.stopPropagation();    //  阻止事件冒泡
                return false;
            }
        }
        //that.SelectIn1Cell(this);
    });

    this.GetKmRltSuu = function (key, map, name) {

        var kname;
        if (name == undefined) {
            kname = key;
        } else {
            kname = name;
        }

        if (key == "") {
            return "<a style='color:red'>漏检" + ':' + (map[key] ? map[key] : "0") + "</a>";
        }

        if (('合微警').indexOf(key) >= 0) {
            return "<a style='color:green'>" + kname + ':' + (map[key] ? map[key] : "0") + "</a>";
        } else {
            return "<a  style='color:red'>" + kname + ':' + (map[key] ? map[key] : "0") + "</a>";
        }

    };
    //刷新总体项目计数
    that.AllRltTxt();

    //缩小
    $(".jqBflMin").mousedown(function (e) {
        $("#imgLook").width($("#imgLook").width() / 1.5);
        $("#imgLook").height($("#imgLook").height() / 1.5);
        return false;
    });
    //放大
    $(".jqBflMax").mousedown(function (e) {
        $("#imgLook").width($("#imgLook").width() * 1.5);
        $("#imgLook").height($("#imgLook").height() * 1.5);
        return false;
    });



    var old_pic_w1;
    var old_pic_h1;
    var old_div_pic_w1;
    var old_div_pic_h1;

    var old_pic_w2;
    var old_pic_h2;
    var old_div_pic_w2;
    var old_div_pic_h2;

    function onTouchStart(e) {

        if ($(e.currentTarget).attr("xf") == "0") {
            //元尺寸保存
            old_pic_w1 = $("#imgLook").width();
            old_pic_h1 = $("#imgLook").height();

            old_div_pic_w1 = $(".ImgDiv").width();
            old_div_pic_h1 = $(".ImgDiv").height();

            $(".imgButton").css("position", "fixed");
            $(".ImgDiv").css("z-index", 1000);
            $(".ImgDiv").css("position", "fixed");

            $(".ImgDiv").css("left", "0");
            //$(".ImgDiv").css("top", document.body.offsetHeight - 450);
            $(".ImgDiv").css("top", 1);

            var h, w;
            w = document.body.offsetWidth;
            h = document.body.offsetWidth / old_pic_w1 * old_pic_h1;


            $(".ImgDiv").width(w - 30);
            $(".ImgDiv").height(document.body.offsetHeight - 30);
            $("#imgLook").width(w);
            $("#imgLook").height(h);

            //$(this).val("取消");
            $(e.currentTarget).attr("xf", "1");

            $(".keyBdDiv").hide();
        } else {
            $(".imgButton").css("position", "");
            $(".ImgDiv").css("z-index", 1);
            $(".ImgDiv").css("position", "");
            $(".ImgDiv").css("left", "");
            $(".ImgDiv").css("top", "");
            $(".ImgDiv").width(496);
            $(".ImgDiv").height(396);

            //$("#imgLook").width(old_pic_w);
            //$("#imgLook").height(old_pic_h);

            $("#imgLook").width(500);
            $("#imgLook").height(500 / old_pic_w1 * old_pic_h1);

            //$(this).val("悬浮");
            $(e.currentTarget).attr("xf", "0");
            $(".keyBdDiv").show();
        }
    }

    function onTouchEnd(e) {
        //元尺寸保存
        $(".imgButton").css("position", "");
        $(".ImgDiv").css("z-index", 1);
        $(".ImgDiv").css("position", "");
        $(".ImgDiv").css("left", "");
        $(".ImgDiv").css("top", "");
        $(".ImgDiv").width(496);
        $(".ImgDiv").height(396);
        $("#imgLook").width(500);
        $("#imgLook").height(500 / old_pic_w1 * old_pic_h1);
        //$(this).val("悬浮");
        $(e.currentTarget).attr("xf", "0");
        $(".keyBdDiv").show();
        e.preventDefault ? e.preventDefault() : e.returnValue = false;
    }
    var dom = document.getElementById('xuanKK');
    dom.addEventListener('touchstart', onTouchStart, false);

    dom.addEventListener('touchend', onTouchEnd, false);
    //$("#xuanKK").mousedown(function () {

    //    if ($(this).attr("xf") == "0") {
    //        //元尺寸保存
    //        old_pic_w1 = $("#imgLook").width();
    //        old_pic_h1 = $("#imgLook").height();

    //        old_div_pic_w1 = $(".ImgDiv").width();
    //        old_div_pic_h1 = $(".ImgDiv").height();

    //        $(".imgButton").css("position", "fixed");
    //        $(".ImgDiv").css("z-index", 1000);
    //        $(".ImgDiv").css("position", "fixed");

    //        $(".ImgDiv").css("left", "0");
    //        //$(".ImgDiv").css("top", document.body.offsetHeight - 450);
    //        $(".ImgDiv").css("top", 1);

    //        var h, w;
    //        w = document.body.offsetWidth;
    //        h = document.body.offsetWidth / old_pic_w1 * old_pic_h1;


    //        $(".ImgDiv").width(w - 30);
    //        $(".ImgDiv").height(document.body.offsetHeight - 30);
    //        $("#imgLook").width(w);
    //        $("#imgLook").height(h);

    //        //$(this).val("取消");
    //        $(this).attr("xf", "1");

    //        $(".keyBdDiv").hide();
    //    } else {
    //        $(".imgButton").css("position", "");
    //        $(".ImgDiv").css("z-index", 1);
    //        $(".ImgDiv").css("position", "");
    //        $(".ImgDiv").css("left", "");
    //        $(".ImgDiv").css("top", "");
    //        $(".ImgDiv").width(496);
    //        $(".ImgDiv").height(396);

    //        //$("#imgLook").width(old_pic_w);
    //        //$("#imgLook").height(old_pic_h);

    //        $("#imgLook").width(500);
    //        $("#imgLook").height(500 / old_pic_w1 * old_pic_h1);

    //        //$(this).val("悬浮");
    //        $(this).attr("xf", "0");
    //        $(".keyBdDiv").show();
    //    }
    //});

    $(".imgButton").css("top",document.body.offsetHeight - 440);

    

    //$("#xuanKK").mouseup(function () {
    //    //元尺寸保存
    //    $(".imgButton").css("position", "");
    //    $(".ImgDiv").css("z-index", 1);
    //    $(".ImgDiv").css("position", "");
    //    $(".ImgDiv").css("left", "");
    //    $(".ImgDiv").css("top", "");
    //    $(".ImgDiv").width(496);
    //    $(".ImgDiv").height(396);
    //    $("#imgLook").width(500);
    //    $("#imgLook").height(500 / old_pic_w1 * old_pic_h1);
    //    //$(this).val("悬浮");
    //    $(this).attr("xf", "0");
    //    $(".keyBdDiv").show();
    //});

    //悬浮
    $("#xuanfu").click(function () {

        if ($(this).attr("xf") == "0") {
            //元尺寸保存
            old_pic_w2 = $("#imgLook").width();
            old_pic_h2 = $("#imgLook").height();

            old_div_pic_w2 = $(".ImgDiv").width();
            old_div_pic_h2 = $(".ImgDiv").height();

            $(".imgButton").css("position", "fixed");
            $(".ImgDiv").css("z-index", 1000);
            $(".ImgDiv").css("position", "fixed");

            $(".ImgDiv").css("left", "0");
            $(".ImgDiv").css("top", document.body.offsetHeight - 650);

            var h, w;
            w = document.body.offsetWidth;
            h = document.body.offsetWidth / old_pic_w2 * old_pic_h2;


            $(".ImgDiv").width(w - 30);
            $(".ImgDiv").height(640);
            $("#imgLook").width(w);
            $("#imgLook").height(h);

            $(this).val("取消");
            $(this).attr("xf", "1");
            $(".keyBdDiv").hide();
        } else {
            $(".imgButton").css("position", "");
            $(".ImgDiv").css("z-index", 1);
            $(".ImgDiv").css("position", "");
            $(".ImgDiv").css("left", "");
            $(".ImgDiv").css("top", "");
            $(".ImgDiv").width(496);
            $(".ImgDiv").height(396);

            //$("#imgLook").width(old_pic_w);
            //$("#imgLook").height(old_pic_h);

            $("#imgLook").width(500);
            $("#imgLook").height(500 / old_pic_w2 * old_pic_h2);

            $(this).val("悬浮");
            $(this).attr("xf", "0");
            $(".keyBdDiv").show();
        }
    });

    $(".ImgDiv").mousedown(function (e) {

        return false;
    });
    $(".keyBdDiv").mousedown(function (e) {

        return false;
    });
    $(".gvDiv").mousedown(function (e) {

        //return false;
    });


    //小键盘按下
    $(".keybd td").mousedown(function () {
        //$(this).hide();
        //$(this).fadeIn(500);
        $(this).fadeTo(100, 0.1);
        $(this).fadeTo(100, 1);
        //$(this).stop().fadeTo(1000,1);
        var txt = $(this).text();
        if (('合轻微中重警').indexOf(txt) >= 0) {

            if ($(this).attr("IsEnable") == "1") {

                var ac = $(document.activeElement);
                var tr = that.findParentTag(ac, 'tr');
                var td = that.findParentTag(ac, "td");
                td.next().text(txt);

                if (('合微警').indexOf(txt) >= 0) {
                    td.next().css("background", "green");
                } else {
                    td.next().css("background", "red");
                }

                var fixResult;

                if (txt == "合") fixResult = "OK";
                if (txt == "轻") fixResult = "M1";
                if (txt == "微") fixResult = "SD";
                if (txt == "中") fixResult = "M2";
                if (txt == "重") fixResult = "M3";
                if (txt == "警") fixResult = "JN";
                if (txt == "误") fixResult = "NG";

                that.SetResultCell(ac, null, fixResult, txt);
                that.NextIn1Focus();
            }
            //$(tr.children().get(td.index() + 1)).text(txt);
            //td.next().text(txt);
            //SetSelectResult(txt);
            //UpdateRow();
            //SetNextFocus(event, obj, false);
        } else if (txt == '回车') {
           
            if ($(this).attr("IsEnable") == "1") {
                var ac = $(document.activeElement);
                that.ChkResultAction($(ac));
                that.NextIn1Focus();
            }
        } else if (txt == '删') {
            if ($(this).attr("IsEnable") == "1") {
                $(document.activeElement).val('');
                var ac = $(document.activeElement);
                //that.ChkResultAction($(ac));

                that.clearRlt($(ac));
                that.SelectIn1Cell($(ac));

            }

        } else {
            if (that.acTr.attr("chk_fs") == "1") { //如果是目视
                return false;
            } else {
                if ($(this).attr("IsEnable") == "1") {
                    $(document.activeElement).val($(document.activeElement).val() + txt);
                }
            }
        }
        return false;
    });

    //选择父tag
    this.findParentTag = function (e, tagName) {
        var tag = $(e);
        var idx = 0;
        while (!tag.is(tagName)) {
            tag = tag.parent();
            idx++;
            if (idx > 10) {
                return null;
            }
        }
        return tag.is(tagName) ? tag : null;
    };
    //下一个输入框
    this.NextIn1Focus = function () {
        var e = $(document.activeElement);
        var tr = that.findParentTag(e, 'tr');
        var td = that.findParentTag(e, "td");
        if (tr.next().length == 0) {
            that.SelectIn1Cell($(document.activeElement));
            return false;
        } else {
            //$(tr.next().children().get(td.index())).find('.jqIn1').focus();
            //$(tr.next().children().get(5)).find('.jqIn1').focus();
            tr.next().find(".jqIn1").focus();

            that.SelectIn1Cell($(document.activeElement));
            return true;
        }
    };


    this.acTr = null;
    this.acIn_1 = null;

    $(".jqIn1").focus(function () {
        $(".keyBdDiv").show();
        that.acTr = that.findParentTag(this, 'tr');
        that.SelectIn1Cell(this);
        that.acIn_1 = $(this);
        that.arrKeyDown = [];

        $(this).attr('type', 'tel');

        var pic_name = that.acTr.attr("pic_name");



        if (pic_name.indexOf("|") > 0) {

            $("#imgLook").attr("src", "Img.aspx?pic_name=" + pic_name);





            if ($("#imgLook").attr("old_pic_name") != pic_name) {
                $("#imgLook").height($(".ImgDiv").height() - 40);
                if ($("#imgLook").width() > $(".ImgDiv").width()) {
                    $("#imgLook").width($(".ImgDiv").width());
                }
                //alert(pic_name);
                $("#imgLook").attr("hh", $("#imgLook").height());
                $("#imgLook").attr("ww", $("#imgLook").width());
            }



        } else {

            if ($("#imgLook").attr("src") != "./Image/ChkImgs/" + pic_name) {
                if (pic_name == "") {
                    $("#imgLook").attr("src", "./Image/ChkImgs/无图.jpg");
                } else {
                    $("#imgLook").attr("src", "./Image/ChkImgs/" + pic_name);
                }

                $("#imgLook").height($(".ImgDiv").height() - 40);
                if ($("#imgLook").width() > $(".ImgDiv").width()) {
                    $("#imgLook").width($(".ImgDiv").width());
                }

                $("#imgLook").attr("hh", $("#imgLook").height());
                $("#imgLook").attr("ww", $("#imgLook").width());

            }
        }

        $("#imgLook").attr("old_pic_name", pic_name);

        //document.activeElement.blur();//屏蔽默认键盘弹出；
        //$("#imgLook").load(function () {
        //    //$("<p> ok " + $(e).width() + ":" + $(e).height() + "</p>").appendTo("#msgTool");
        //}).error(function () {
        //    //$("<p> error " + imgsrc + "</p>").appendTo("#msgTool");
        //    //$(e).attr("src", "http://www.mainaer.com/uploadfiles/mainaer/nopic.gif");
        //});
    });


    $("#imgLook").on('error', function () {
        console.log("error loading image");
        $("#imgLook").attr("src", "./Image/ChkImgs/未上传.jpg");

    });





    this.arrKeyDown = [];
    $(".jqIn1").keydown(function (e) {
        if (that.acTr.attr("chk_fs") == "9") { //如果是目视
            that.arrKeyDown.push("");
            setTimeout(function () {
                if (that.arrKeyDown.length == 1) {
                    that.acIn_1.select();
                }
                that.arrKeyDown.splice(0, 1);

            }, 600);
        } else if (that.acTr.attr("chk_fs") == "1") {//如果是扫描
            if (e.which != 13) {
                if ($(this).is('[readonly]')) {
                    $(this).val('');
                    $(this).removeAttr('readonly');
                }
            }
        } else {
            if (e.which != 13) {
                if ($(this).is('[readonly]')) {
                    $(this).val('');
                    $(this).removeAttr('readonly');
                }
            }

        }

        event.stopPropagation();    //  阻止事件冒泡
        if (e.which == 13) {
            $(this).attr("readonly", "readonly");
            e.preventDefault ? e.preventDefault() : e.returnValue = false;
        }
    });

    this.SelectIn1Cell = function (e) {
        var tr = that.acTr;
        //$(".panBtn").find("td").css("color", "#ccc");
        $(".suu").find("td").css("color", "#ccc");
        //$(".panBtn").find("td").css("color", "#000");
        $(".suu").find("td").attr("IsEnable", "0");

        $(".panBtn").find("td").attr("IsEnable", "1");
        $(".panBtn").find("td").css("color", "#000");

        if (tr.attr("chk_fs") == "1") { //如果是扫描

            //setTimeout(function () {
            //    $(e).select();                
            //}, 10);

            // $(".suu").css("visibility", "hidden");
            // $(".keybd").show();

            //$(".keybd").find("td").css("color", "#000");

            $(".suu").find("td").attr("IsEnable", "0");
            $(".suu").find("td").css("color", "#ccc");

            //特殊标签  ，可以
            //if ((tr.attr("k_type") == "fix001" || tr.attr("k_type") == "fix002") && $(e).val() != '') {//标签日期
            if ($(e).val()!='') {
                $(".panBtn").find("td").attr("IsEnable", "1");
                $(".panBtn").find("td").css("color", "#000");
            }else{
                $(".panBtn").find("td").attr("IsEnable", "0");
                $(".panBtn").find("td").css("color", "#ccc");
            }




            //$(".keybd").css("visibility","hidden");
            //$(".keyBdDiv").css("background-image", "url('image/Scan.jpg')");

        } else if (tr.attr("chk_fs") == "9") { //如果是目视
            //$(".suu").hide();
            $//(".suu").css("visibility", "hidden");
            //$(".keybd").show();
            //if ($(tr.find('td').get(0)).text().trim().indexOf( '小口标签')>0) {
            //    $("#tdJinggao").css("visibility", "hidden");
            //} else {
            //    $("#tdJinggao").css("visibility", "hidden");
            //}
            $(".keyBdDiv").css("background-image", "url('')");
        } else {
            //$(".keybd").show();
            //$(".suu").show();
            //$(".suu").css("visibility", "visible");

            //if ($(tr.find('td').get(0)).text().trim().indexOf('小口标签') > 0) {
            //    $("#tdJinggao").css("visibility", "hidden");
            //} else {
            //    $("#tdJinggao").css("visibility", "hidden");
            //}

            $(".suu").find("td").attr("IsEnable", "1");
            $(".suu").find("td").css("color", "#000");

            $(".keyBdDiv").css("background-image", "url('')");
        }

        //if (tr.attr("k_type") == "fix004") {
        //    $(".panBtn").css("visibility", "hidden");
        //} else {
        //    $(".panBtn").css("visibility", "visible");
        //}

        $(".delkeybord").attr("IsEnable", "1");
        $(".delkeybord").css("color", "#000");
        //$(".panBtn").css("visibility", "visible");



        event.stopPropagation();    //  阻止事件冒泡
    }


    $(".jqIn1").blur(function () {
        //$(".keybd").hide();
        //$(".panBtn").find("td").css("color", "#ccc");
        //$(".suu").find("td").css("color", "#ccc");

        SetCellFontSize(this);

    });



    $(".jqIn1").change(function (e) {
        var tr = that.acTr;
        if (tr.attr("chk_fs") == "1") { //如果是扫描
        } else {
            that.ChkResultAction($(this));
        }
    });


    $(".jqIn1").keydown(function (e) {
        if (e.which == 13) {
            that.ChkResultAction($(this));
            that.NextIn1Focus();
            
            e.preventDefault ? e.preventDefault() : e.returnValue = false;
            return false;
        }
    });

    $(".mark").change(function (e) {
        that.UpdMark($(this));
    });

    $(".mark").keydown(function (e) {
        if (e.which == 13) {
            that.UpdMark($(this));
            that.NextIn1Focus();
            e.preventDefault ? e.preventDefault() : e.returnValue = false;
            return false;
        }
    });

    $(".mark").focus(function (e) {
        $(".keyBdDiv").hide();
    });


    //主检查
    this.ChkResultAction = function (e) {
        if ($(e).val() == '') {
            return true;
        }
        var rtv = that.ChkResult(e);
        //alert(1);
        that.SetResultCell(e, rtv);
        // alert(2);
    }

    this.UpdMark = function (e) {
        var tr = that.findParentTag(e, 'tr');
        var ck_id = $("#tbxCk_id").text();
        var id = tr.attr("id");
        var mark = tr.find(".mark").val();
        var upd_user = $("#tbxUserCd").text();

        var rtv;
        rtv = that.Ajax("UpdMark",
            JSON.stringify({
                ck_id: ck_id,
                id: id,
                mark: mark,
                upd_user: upd_user
            }));
        //rtv.data.CNT[0].cnt
        //rtv.data.MS
        return rtv;

    }

    this.clearRlt = function (e) {
        var tr = that.findParentTag(e, 'tr');
        var rltCell = tr.find('.JQ_JIEGUO');
        var ck_id = $("#tbxCk_id").text();
        var id = tr.attr("id");
        var in_1 = $(e).val();
        var mark = tr.find(".mark").val();
        var upd_user = $("#tbxUserCd").text();
        var result;
        rltCell.text('');
        rltCell.css("background-color", "#fff");
        tr.find('.JZ').text("");
        var rtv;
        rtv = that.Ajax("UpdCheckMsResult",
            JSON.stringify({
                ck_id: ck_id,
                id: id,
                in_1: '',
                result: '',
                mark: mark,
                upd_user: upd_user
            }));

    }

    //设置结果单元格
    this.SetResultCell = function (e, rlt, fixResult, fixResultTxt) {
        var tr = that.findParentTag(e, 'tr');
        var rltCell = tr.find('.JQ_JIEGUO');

        //ByVal ck_id As String, ByVal id As String, ByVal in_1 As String, ByVal result As String, ByVal mark As String, ByVal upd_user As String
        var ck_id = $("#tbxCk_id").text();
        var id = tr.attr("id");
        var in_1 = $(e).val();
        var mark = tr.find(".mark").val();
        var upd_user = $("#tbxUserCd").text();
        var result;

        if (fixResult == undefined) {
            if (rlt.result == "JN") {
                result = 'JN';
                rltCell.text('警');
                rltCell.css("background-color", "red");
                tr.find('.JZ').text("(" + GetFormulaChkStr(tr.attr("k1"), "") + "," + tr.attr("k3") + "," + tr.attr("k2") + ")");
                var rtv;
                rtv = that.Ajax("UpdCheckMsResult",
                    JSON.stringify({
                        ck_id: ck_id,
                        id: id,
                        in_1: in_1,
                        result: result,
                        mark: mark,
                        upd_user: upd_user
                    }));
                //刷新总体项目计数
                that.AllRltTxt();
                return rtv;
            }


            if (rlt.result) {
                result = 'OK';
                rltCell.text('合');
                rltCell.css("color", "#00");
                rltCell.css("background-color", "#93FF93");
                tr.find('.JZ').text("");
                var rtv;
                rtv = that.Ajax("UpdCheckMsResult",
                    JSON.stringify({
                        ck_id: ck_id,
                        id: id,
                        in_1: in_1,
                        result: result,
                        mark: mark,
                        upd_user: upd_user
                    }));
                //刷新总体项目计数
                that.AllRltTxt();
                return rtv;
            } else {
                result = 'NG';
                rltCell.text('误');

                rltCell.css("background-color", "#FF2D2D");
                // tr.find('.JZ').text("(" + GetFormulaChkStr(tr.attr("k1"), "") + "," + tr.attr("k2") + "," + tr.attr("k3")  + ")" + rlt.jizhun);
                tr.find('.JZ').text("(" + GetFormulaChkStr(tr.attr("k1"), "") + "," + tr.attr("k3") + "," + tr.attr("k2") + ")");



                var rtv;
                rtv = that.Ajax("UpdCheckMsResult",
                    JSON.stringify({
                        ck_id: ck_id,
                        id: id,
                        in_1: in_1,
                        result: result,
                        mark: mark,
                        upd_user: upd_user
                    }));
                //刷新总体项目计数
                that.AllRltTxt();
                return rtv;
            }
        } else {
            result = fixResult;
            rltCell.text(fixResultTxt);
            if (["OK", "SD"].indexOf(result) >= 0) {
                rltCell.css("background-color", "#93FF93");
                tr.find('.JZ').text("");
            } else {
                rltCell.css("background-color", "#FF2D2D");
                if (rlt == null) {

                } else {
                    //tr.find('.JZ').text("(" + tr.attr("k1") + "," + tr.attr("k2") + "," + tr.attr("k3") + ")" + rlt.jizhun);
                    tr.find('.JZ').text("(" + tr.attr("k1") + "," + tr.attr("k3") + "," + tr.attr("k2") + ")");
                }

            }
            var rtv;
            rtv = that.Ajax("UpdCheckMsResult",
                JSON.stringify({
                    ck_id: ck_id,
                    id: id,
                    in_1: in_1,
                    result: result,
                    mark: mark,
                    upd_user: upd_user
                }));
            //刷新总体项目计数
            that.AllRltTxt();
            return rtv;
        }
    }




    //装载生产明细书

    function ReadBarCode(cd) {

        var arr;
        cd = cd.toUpperCase();
        arr = cd.split("/");
        this.allText = [];
        var i;
        for (i = 0; i <= arr.length - 1; i++) {
            this.allText.push(arr[i].trim());
        }


        //新标签
        if (cd.length == 136) {
            this.kind = "2";
            //this.zuofan = cd.split("/")[0].trim().replace(/-/g, "");
            //this.zhiPinCd = cd.split("/")[1].trim().replace(/-/g, "");
            this.zuofan = cd.split("/")[0].trim();
            this.zhiPinCd = cd.split("/")[1].trim();

            this.kunBaoSuu = ""; //已经是数量了
            this.tuoPanXuHao = "";
            this.xiangXian = "";
            //alert(11);
        } else {
            //元标签
            if (cd.split("/").length == 8) {
                this.kind = "2";
                //this.zuofan = cd.split("/")[7].trim().replace(/-/g, "");
                //this.zhiPinCd = cd.split("/")[1].trim().replace(/-/g, "");

                this.zuofan = cd.split("/")[7].trim();
                this.zhiPinCd = cd.split("/")[1].trim();

                this.kunBaoSuu = cd.split("/")[3].trim().replace(/-/g, "");
                this.tuoPanXuHao = cd.split("/")[6].trim().replace(/-/g, "");
                this.xiangXian = cd.split("/")[5].trim().replace(/-/g, "");
                //alert(22);
            } else if (cd.split("/").length >= 5) {
                this.kind = "2";
                //this.zuofan = cd.split("/")[0].trim().replace(/-/g, "");
                //this.zhiPinCd = cd.split("/")[1].trim().replace(/-/g, "");
                this.zuofan = cd.split("/")[0].trim();
                this.zhiPinCd = cd.split("/")[1].trim();

                this.kunBaoSuu = cd.split("/")[2].trim().replace(/-/g, "");
                this.tuoPanXuHao = cd.split("/")[3].trim().replace(/-/g, "");
                this.xiangXian = cd.split("/")[4].trim().replace(/-/g, "");

                this.lotRiQi = cd.split("/")[12];
                //alert(33);
            } else if (cd.right(2) == "/C") {
                this.kind = "3";
                this.cd = cd.left(16).trim().replace(/-/g, "");
                this.lotRiQi = cd.substring(16, 25).trim();

                if (this.lotRiQi.left(2) != 'Y7') {
                    this.cd = cd.split(" ")[0];
                    this.lotRiQi = cd.split(" ")[1];
                    return this;
                } else {
                    return this;
                }



            } else {
                this.kind = "1";
                this.cd = cd;
                this.cd = this.cd.replace(/ /g, "");
                this.cd = this.cd.replace(/-/g, "");
            }
        }
        return this;
    }



    this.ChkResult = function (e) {
        var in1_value = $(e).val();

        var tr = that.findParentTag(e, 'tr');

        var k_type = tr.attr("k_type"); //fix001,fix002
        var k1 = tr.attr("k1");
        var k2 = tr.attr("k2");
        var k3 = tr.attr("k3");
        var chk_fmt = tr.attr("chk_fmt");//公式
        var chk_fs = tr.attr("chk_fs");//检查方式 ，扫描，手输入
        var suu = tr.attr("suu");//数量

        //固定扫描生产明细书 ， 设置值 ，并且检查     
        if (k_type == "fix001") {//扫描表情信息 含有输入值 （商品CD）      9006160965/CHADDLW3A1BTAXX/2/3/4
            try {
                var BarCd = new ReadBarCode(in1_value);

                if (BarCd.kind == "2") {
                    var inIdx = BarCd.allText.indexOf(that.cd.toUpperCase());
                    if (BarCd.allText.indexOf(that.cd.toUpperCase()) >= 0 && parseInt(BarCd.kunBaoSuu) == parseInt(suu)) {
                        $(e).val(BarCd.allText[inIdx].toUpperCase());
                        return { result: true, jizhun: '' }
                    } else {
                        return { result: false, jizhun: '商品CD:' + that.cd + '数量：' + BarCd.kunBaoSuu }
                    }
                } else if (BarCd.kind == "3" || BarCd.kind == "1") {
                    if (in1_value.indexOf(that.cd.toUpperCase()) >= 0) {
                        $(e).val(that.cd.toUpperCase());
                        return { result: true, jizhun: '' }
                    } else {
                        return { result: false, jizhun: '商品CD:' + that.cd }
                    }
                }


            } catch (err) {
                return { result: false, jizhun: '扫描标签错误：' + err.description };
            }

        } else if (k_type == "fix002") {//日期 3 前后3天以内       XXXXXXXXXXXCCCCC Y7210611/C
            try {

                var BarCd = new ReadBarCode(in1_value);

                //var temp;
                //temp = ('20' + BarCd.lotRiQi).replace("Y7", "").toUpperCase();

                ////    '小口标签日期测试，1.有没有标签，有就算合格 2.没有就算NG   3.有的话扫描到的日期如果和当前 要在当前日期的加减3天以内，则为正确 不是的话就是OW
                //if (parseFloat(temp) + 3 >= parseFloat(getNowFormatDate()) && parseFloat(temp) - 3 <= parseFloat(getNowFormatDate())) {
                //    return { result: true, jizhun: '' }
                //} else {
                //    return { result: false, jizhun: '前后3日以内' }
                //}
                if (BarCd.kind == "3" || BarCd.kind == "2") {
                    var ymd = '20' + BarCd.lotRiQi.replace("Y7", "").toUpperCase();
                    var y = parseInt(ymd.substring(0, 4));
                    var m = parseInt(ymd.substring(4, 6));
                    var d = parseInt(ymd.substring(6, 8));

                    if (m == 1) {
                        m = 12;
                        y = y - 1;
                    } else {
                        m = m - 1;
                    }

                    var lotDate = new Date(y, m, d);
                    var nowDate = new Date();

                    var stepDays = DateDiff(lotDate, nowDate);

                    $(e).val(BarCd.lotRiQi);    //lot 日期
                    tr.find(".mark").val($("#hid_jxs_name").val());


                    if (stepDays > 3 || stepDays < -3) {
                        return { result: "JN", jizhun: '前后3日以内' }
                    } else {
                        return { result: true, jizhun: '' }
                    }
                }
            } catch (err) {
                return { result: false, jizhun: '扫描标签错误：' + err.description };
            }
        } else if (k_type == "fix003") {
            if (in1_value.indexOf(k1) >= 0) {
                $(e).val(k1);
                return { result: true, jizhun: '' }
            } else {
                return { result: false, jizhun: '标签不包含基准值1' }
            }

        } else if (k_type == "fix004") {
            if (in1_value.indexOf(k1) >= 0) {
                $(e).val(k1);
                return { result: true, jizhun: '' }
            } else {
                return { result: false, jizhun: '标签不包含基准值1' }
            }


        } else if (k_type == "1") {//与基准值1相等
            //扫描
            if (chk_fs == '1') {
                if (in1_value.replace(/-/g, "") == k1.replace(/-/g, "")) {
                    //$(e).val(Fixed(in1_value));
                    return { result: true, jizhun: '' }
                } else {
                    return { result: false, jizhun: '与基准值1不相等 基准值1：' + k1 }
                }
            } else {
                if (in1_value == k1) {
                    $(e).val(Fixed(in1_value));
                    return { result: true, jizhun: '' }
                } else {
                    return { result: false, jizhun: '与基准值1不相等 基准值1：' + k1 }
                }
            }


        } else if (k_type == "2") {//2:含有基准值1
            if (in1_value == '') {
                return { result: false, jizhun: '没有输入值 基准值1：' + k1 }
            } else if ((in1_value).indexOf(k1) >= 0) {
                return { result: true, jizhun: '' }
            } else {
                return { result: false, jizhun: '不含有基准值1 基准值1：' + k1 }
            }
        } else if (k_type == "3") {

            k1 = GetFormulaChkStr(k1, in1_value);
            k2 = GetFormulaChkStr(k2, in1_value);
            k3 = GetFormulaChkStr(k3, in1_value);

            if (floatSub(parseFloat(k1) ,parseFloat(k2)) <= parseFloat(in1_value) &&
                parseFloat(in1_value) <= floatAdd(parseFloat(k1), parseFloat(k3))) {
                $(e).val(Fixed(in1_value));
                return { result: true, jizhun: '' }
            } else {
                $(e).val(Fixed(in1_value));
                return { result: false, jizhun: 'k_type:' + k_type + '基准值1：' + k1 + '基准值2：' + k2 + '基准值3：' + k3 }
            }
        } else if (k_type == "4") {

            k1 = GetFormulaChkStr(k1, in1_value);
            k2 = GetFormulaChkStr(k2, in1_value);
            k3 = GetFormulaChkStr(k3, in1_value);


            
            if (floatSub(parseFloat(k1) ,parseFloat(k2)) < parseFloat(in1_value) &&
                parseFloat(in1_value) < floatAdd(parseFloat(k1), parseFloat(k3))) {
                $(e).val(Fixed(in1_value));
                return { result: true, jizhun: '' }
            } else {
                $(e).val(Fixed(in1_value));
                return { result: false, jizhun: 'k_type:' + k_type + '基准值1：' + k1 + '基准值2：' + k2 + '基准值3：' + k3 }
            }
        } else if (k_type == "5") {//输入值 < K1

            k1 = GetFormulaChkStr(k1, in1_value);
            k2 = GetFormulaChkStr(k2, in1_value);
            k3 = GetFormulaChkStr(k3, in1_value);
            if (parseFloat(in1_value) < parseFloat(k1)) {
                $(e).val(Fixed(in1_value));
                return { result: true, jizhun: '' }
            } else {
                $(e).val(Fixed(in1_value));
                return { result: false, jizhun: 'k_type:' + k_type + '基准值1：' + k1 + '基准值2：' + k2 + '基准值3：' + k3 }
            }

        } else if (k_type == "6") {//输入值 <= K1

            k1 = GetFormulaChkStr(k1, in1_value);
            k2 = GetFormulaChkStr(k2, in1_value);
            k3 = GetFormulaChkStr(k3, in1_value);
            if (parseFloat(in1_value) <= parseFloat(k1)) {
                $(e).val(Fixed(in1_value));
                return { result: true, jizhun: '' }
            } else {
                $(e).val(Fixed(in1_value));
                return { result: false, jizhun: 'k_type:' + k_type + '基准值1：' + k1 + '基准值2：' + k2 + '基准值3：' + k3 }
            }
        } else if (k_type == "7") {//输入值 > K1
            k1 = GetFormulaChkStr(k1, in1_value);
            k2 = GetFormulaChkStr(k2, in1_value);
            k3 = GetFormulaChkStr(k3, in1_value);
            if (parseFloat(in1_value) > parseFloat(k1)) {
                $(e).val(Fixed(in1_value));
                return { result: true, jizhun: '' }
            } else {
                $(e).val(Fixed(in1_value));
                return { result: false, jizhun: 'k_type:' + k_type + '基准值1：' + k1 + '基准值2：' + k2 + '基准值3：' + k3 }
            }
        } else if (k_type == "8") {//输入值 >= K1

            k1 = GetFormulaChkStr(k1, in1_value);
            k2 = GetFormulaChkStr(k2, in1_value);
            k3 = GetFormulaChkStr(k3, in1_value);
            if (parseFloat(in1_value) >= parseFloat(k1)) {
                $(e).val(Fixed(in1_value));
                return { result: true, jizhun: '' }
            } else {
                $(e).val(Fixed(in1_value));
                return { result: false, jizhun: 'k_type:' + k_type + '基准值1：' + k1 + '基准值2：' + k2 + '基准值3：' + k3 }
            }
        } else if (k_type == "9") {

            k1 = GetFormulaChkStr(k1, in1_value);
            k2 = GetFormulaChkStr(k2, in1_value);
            k3 = GetFormulaChkStr(k3, in1_value);

            if (0 <= parseFloat(in1_value) &&
                parseFloat(in1_value) <= parseFloat(k1) / 1000) {
                $(e).val(Fixed(in1_value));
                return { result: true, jizhun: '' }
            } else {
                $(e).val(Fixed(in1_value));
                return { result: false, jizhun: 'k_type:' + k_type + '基准值1：' + k1 + '基准值2：' + k2 + '基准值3：' + k3 }
            }
        } else if (k_type == "10") {

            k1 = GetFormulaChkStr(k1, in1_value);
            k2 = GetFormulaChkStr(k2, in1_value);
            k3 = GetFormulaChkStr(k3, in1_value);

            if (0 <= parseFloat(in1_value) &&
                parseFloat(in1_value) <= parseFloat(k1)) {
                $(e).val(Fixed(in1_value));
                return { result: true, jizhun: '' }
            } else {
                $(e).val(Fixed(in1_value));
                return { result: false, jizhun: 'k_type:' + k_type + '基准值1：' + k1 + '基准值2：' + k2 + '基准值3：' + k3 }
            }


        } else if (k_type == "99") {
            k1 = GetFormulaChkStr(k1, in1_value);
            k2 = GetFormulaChkStr(k2, in1_value);
            k3 = GetFormulaChkStr(k3, in1_value);
            chk_fmt = GetFormulaChkStr2(chk_fmt, in1_value);

            if (eval(chk_fmt)) {
                $(e).val(Fixed(in1_value));
                return { result: true, jizhun: '' }
            } else {
                $(e).val(Fixed(in1_value));
                return { result: false, jizhun: chk_fmt }
            }



        }
        return { result: false, jizhun: 'js 预想外基准类型' + k_type }
    }


    //设置明细区域大小
    //$(".gvDiv").height($(".keyBdDiv").offset().top - $(".gv").offset().top-40);

    $(".gvDiv").height(document.body.offsetHeight - $(".gv").offset().top - 450);

    //$(window).resize(function () {
    //    $(".gvDiv").height($(".keyBdDiv").offset().top - $(".gv").offset().top - 40);
    //});

    $(".jqIn1").attr("readonly", "readonly");
    $(".jqIn1").get(0).focus();

    $(".jqIn1").each(function () {
        SetCellFontSize(this);
        //$(this).height($(this).parent().height());
    });


});

//保留两位小数
function Fixed(v) {
    if (v == "") return "";
    var re = /^[0-9]+.?[0-9]*/;//判断字符串是否为数字//判断正整数/[1−9]+[0−9]∗]∗/ 
    if (re.test(v)) {
        return parseFloat(v).toFixed(2);
    } else {
        return v;
    }
}


//置换 检查EVAL字符串
function GetFormulaChkStr(str, in1) {

    if (str == undefined) {
        return "";
    }

    var gv = $(".gv");
    str = str.replace(/\{h\}/g, gv.attr("h"));
    str = str.replace(/\{w\}/g, gv.attr("w"));
    str = str.replace(/\{dh\}/g, gv.attr("dh"));
    str = str.replace(/\{dw\}/g, gv.attr("dw"));
    str = str.replace(/\{sw\}/g, gv.attr("sw"));
    str = str.replace(/\{kw\}/g, gv.attr("kw"));
    str = str.replace(/\{in1\}/g, in1);

    var reg = new RegExp("[a-zA-Z]");

    if (reg.test(str)) {
        return str;
    } else {
        if (str == "") {
            return "";
        } else {
            return eval(str);
        }

    }

}

function GetFormulaChkStr2(str, in1, k1, k2, k3) {


    var gv = $(".gv");
    str = str.replace(/\{k1\}/g, k1);
    str = str.replace(/\{k2\}/g, k2);
    str = str.replace(/\{k3\}/g, k3);
    str = str.replace(/\{in1\}/g, in1);
    return str;
}

//设置文字大小
function SetCellFontSize(e) {
    var v = $(e).val();

    if (v.length > 9) {
        $(e).css('font-size', (30 / (v.length / 9) * 1) + 'px');
    } else {
        $(e).css('font-size', '30px');
    }
}


function add(num1, num2) {
    const num1Len = (num1.toString().split('.')[1]).length;
    const num2Len = (num2.toString().split('.')[1]).length;
    const maxLen = Math.pow(10, Math.max(num1Len, num2Len));
    return (num1 * maxLen + num2 * maxLen) / maxLen;
}

/**
 * 解决两个数相加精度丢失问题
 * @param a
 * @param b
 * @returns {Number}
 */
function floatAdd(a, b) {
    var c, d, e;
    if (undefined == a || null == a || "" == a || isNaN(a)) { a = 0; }
    if (undefined == b || null == b || "" == b || isNaN(b)) { b = 0; }
    try {
        c = a.toString().split(".")[1].length;
    } catch (f) {
        c = 0;
    }
    try {
        d = b.toString().split(".")[1].length;
    } catch (f) {
        d = 0;
    }
    e = Math.pow(10, Math.max(c, d));
    return (floatMul(a, e) + floatMul(b, e)) / e;
}
/**
 * 解决两个数相减精度丢失问题
 * @param a
 * @param b
 * @returns {Number}
 */
function floatSub(a, b) {
    var c, d, e;
    if (undefined == a || null == a || "" == a || isNaN(a)) { a = 0; }
    if (undefined == b || null == b || "" == b || isNaN(b)) { b = 0; }
    try {
        c = a.toString().split(".")[1].length;
    } catch (f) {
        c = 0;
    }
    try {
        d = b.toString().split(".")[1].length;
    } catch (f) {
        d = 0;
    }
    e = Math.pow(10, Math.max(c, d));
    return (floatMul(a, e) - floatMul(b, e)) / e;
}
/**
 * 解决两个数相乘精度丢失问题
 * @param a
 * @param b
 * @returns {Number}
 */
function floatMul(a, b) {
    var c = 0,
        d = a.toString(),
        e = b.toString();
    try {
        c += d.split(".")[1].length;
    } catch (f) { }
    try {
        c += e.split(".")[1].length;
    } catch (f) { }
    return Number(d.replace(".", "")) * Number(e.replace(".", "")) / Math.pow(10, c);
}
/**
 * 解决两个数相除精度丢失问题
 * @param a
 * @param b
 * @returns
 */
function floatDiv(a, b) {
    var c, d, e = 0,
        f = 0;
    try {
        e = a.toString().split(".")[1].length;
    } catch (g) { }
    try {
        f = b.toString().split(".")[1].length;
    } catch (g) { }
    return c = Number(a.toString().replace(".", "")), d = Number(b.toString().replace(".", "")), floatMul(c / d, Math.pow(10, f - e));
}