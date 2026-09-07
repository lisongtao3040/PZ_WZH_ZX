
//编辑
function Edit(ck_id) {

    if (confirm('真的要编辑么？')) {
        $("#hid_ck_id").val(ck_id);
        $("#btnEdit").click();
    }
    return false;
}

function Del(ck_id) {

    if (confirm('真的要删除么？')) {
        $("#hid_ck_id").val(ck_id);
        $("#btnDel").click();
    }
    return false;
}


function QianPin(e, ck_id) {
    //alert($(e).is(':checked'));
    var upd_user = $("#UserHeader_lblUserCd").text();
    var qianpin;
    if ($(e).is(':checked')) {
        qianpin = "0";
    } else {
        qianpin = "1";
    }

    var rtv;
    rtv = Ajax("UpdCheckResult",
        JSON.stringify({
            ck_id: ck_id,
            qianpin: qianpin,
            upd_user: upd_user
        }));
    //rtv.data.CNT[0].cnt
    //rtv.data.MS
    //return rtv;

    return false;
}

var oldRow = "";
var sinkiMsg = "";

function ChooseRow(e, cd, no, jxs_name) {
    var qianpin;
    $("#tbxCd").val(cd);
    $("#tbxNo").val(no);
    $("#hid_jxs_name").val(jxs_name);
    //
    //NG再检查 按钮
    if ($(e).attr("buliang") == '1' && $(e).attr("ss_id") == '') {
        $("#btnReChk").removeAttr("disabled");
    } else {
        $("#btnReChk").attr("disabled", "true");
    }

    $("#hid_ck_id").val($(e).attr("ck_id"));

    if (oldRow != "") {
        $(oldRow).css('background-color', 'transparent');
    }

    $(e).css('background-color', 'yellow');

    oldRow = e;

    //if ($(e).attr("ck_id") == '') {
    //    $("#btnNewChk").attr("disabled", "true");
    //} else {
    //    $("#btnNewChk").removeAttr("disabled");
    //}
    if (cd == '' || no == '') {
        $("#btnNewChk").attr("disabled", "true");
    } else {
        $("#btnNewChk").removeAttr("disabled");
    }




    if ($(e).attr("ck_id") != '' && $(e).attr("chk_zumi") == '1') {
        $("#btnSetDefault").removeAttr("disabled");
    } else {

        $("#btnSetDefault").attr("disabled", "true");
    }

    if ($(e).text().indexOf("初①") != -1 || $(e).text().indexOf("初②") != -1) {
        $("#btnSetDefault").attr("disabled", "true");
    }

    if ($(e).text().indexOf("初①") != -1) {
        sinkiMsg = "①初检的CD：初次生产，需要确认机种。";
    } else if ($(e).text().indexOf("初②") != -1) {
        sinkiMsg = "②相同CD：需要三方召合确认机种。";
    } else {
        sinkiMsg = "";
    }

    //$("#btnNewChk").attr("disabled", "true");
    //$("#btnSetDefault").attr("disabled", "true");
    return false;
}



var Ajax = function (fncName, jsonData) {
    var rtv = "";
    var thatArguments = arguments;
    var ele = Cover();
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
var Cover = function () {
    var arr = [];
    arr.push("<div id='cover' style='position: absolute; width: 100%; height: 100%; left: 0px; top: 0px; z-index: 100000; opacity: 0.5; background-color: #ddd; text-align: center;'>");
    arr.push("</div>");
    return $(arr.join(""));
}




function ReadBarCode(cd) {

    var arr;
    arr = cd.split("/");
    this.allText = [];
    var i;
    for (i = 0; i <= arr.length - 1; i++) {
        this.allText.push(arr[i].trim());
    }

    //关于标签扫不上，现在接受的标签有下面几种
    //接受标签
    //种类1. 136位的，【/】分割      （[1]:cd,[0]zuofan)
    //种类2. 【/】分割成 8 份的      （[1]:cd,[7]zuofan)
    //种类3. 【/】分割成 5 份的 以上 （[1]:cd,[0]zuofan)
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
            //alert(33);
        } else if (cd.right(2) == "/C") {
            this.kind = "3";
            this.cd = cd.left(16).trim().replace(/-/g, "");
            this.lotRiQi = cd.substring(16, 25).trim();
        } else {
            this.kind = "1";
            this.cd = cd;
            this.cd = this.cd.replace(/ /g, "");
            this.cd = this.cd.replace(/-/g, "");
        }
    }
    return this;
}


$(document).ready(function () {
    //CD 作番 解决半角输入法问题
    $("#tbxCd,#tbxNo").click(function () {
        var that = $(this);
        that.select();
        //that.hide();
        //that.attr("type", "password");
        //setTimeout(function() {
        //    that.attr("type", "tel");            

        //}, 100);
        //setTimeout(function () {

        //    that.show();
        //    that.select();
        //}, 300);
    });

    //扫描    9006160965/CHADDLW3A1BTAXX/0001234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456
    $("#tbxCd,#tbxNo").keydown(function (e) {
        //如果回车 检查是否时标签 ，如果时标签  那么读取
        if (e.keyCode == 13) {
            var BarCd = new ReadBarCode($(this).val());
            if (BarCd.kind == "2") {     //新标签
                //alert($(this).val());
                //alert(BarCd.zhiPinCd);
                //alert(BarCd.zuofan);
                $("#tbxCd").val(BarCd.zhiPinCd);
                $("#tbxNo").val(BarCd.zuofan);

                $("#btnSel").click();
            }
            e.preventDefault ? e.preventDefault() : e.returnValue = false;
        }
    });

    //清除托盘No
    $("#btnClearTp").click(function () {
        $("#tbxTpNo").val('');
    });

    $("#btnNewChk").click(function () {
        var that = $(this);
        if (sinkiMsg != "") {
            alert(sinkiMsg);
        }
        setTimeout(function () { $("#btnNewChk").attr("disabled", "true"); }, 1);
    });

    $("#btnSetDefault").click(function () {
        var that = $(this);
        setTimeout(function () { $("#btnSetDefault").attr("disabled", "true"); }, 1);
    });
    //托盘No选择
    $("#tbxTpNo").click(function () {
        var that = $(this);
        that.select();
    });
    //不良
    $("#btnBuliang").click(function () {
        var that = $(this);
        var bu = [];
        if ($('#cb1').is(':checked')) {
            bu.push('1');
        }
        if ($('#cb2').is(':checked')) {
            bu.push('2');
        }
        if ($('#cb3').is(':checked')) {
            bu.push('3');
        }
        if ($('#cb4').is(':checked')) {
            bu.push('4');
        }

        var buTxt = "";
        if (bu.length > 0) {
            buTxt = bu.join("-");
        }
        var rinei = $("#ddlHiinai").val();
        window.open("./t_BuliangList.aspx?bu=" + buTxt + "&rinei=" + rinei);

        //window.open("./t_BuliangList.aspx?bu=" + buTxt+"&rinei=" & rinei);
    });


    //不良
    $("#btnScsj").click(function () {
        var that = $(this);
        var bu = [];
        if ($('#cb1').is(':checked')) {
            bu.push('1');
        }
        if ($('#cb2').is(':checked')) {
            bu.push('2');
        }
        if ($('#cb3').is(':checked')) {
            bu.push('3');
        }
        if ($('#cb4').is(':checked')) {
            bu.push('4');
        }

        var buTxt = "";
        if (bu.length > 0) {
            buTxt = bu.join("-");
        }
        var rinei = $("#ddlHiinai").val();
        window.open("./t_scsj.aspx?bu=" + buTxt);

        //window.open("./t_BuliangList.aspx?bu=" + buTxt+"&rinei=" & rinei);
    });


    
    //录入托盘No
    $("#tbxTpNo").keydown(function (e) {
        //如果回车 检查是否时标签 ，如果时标签  那么读取
        if (e.keyCode == 13) {
            var txt;
            txt = $(this).val();
            var arr;
            arr = txt.split("/");
            if (arr.length == 3) {
                $(this).val(arr[2]);
            }
            //$("#tbxTpNo").val();
            $("#btnTpChkList").click();
            e.preventDefault ? e.preventDefault() : e.returnValue = false;
        }
    });



    //默认选择CD
    //$("#tbxCd").click();

    //$("#tbxCd").hide();
    //$("#tbxCd").attr("type", "password");
    //setTimeout(function () {
    //    $("#tbxCd").attr("type", "tel");
    //    $("#tbxCd").show();
    //    $("#tbxCd").select();
    //    $("#tbxCd").click();
    //}, 10);
    //$("#tbxCd").select();
    //$("#tbxCd").hide();
    //setTimeout(function () {
    //    $("#tbxCd").attr("type", "tel");

    //}, 100);
    //setTimeout(function () {
    $("#tbxCd").attr("type", "tel");
    $("#tbxNo").attr("type", "tel");
    //    $("#tbxCd").show();
    //    $("#tbxCd").val('');
    $("#tbxCd").select();
    //}, 300);

});