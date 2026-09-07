$(document).ready(function () {

    $("#btnUpd").click(function () {
        var tp_no = $("#tbxScan").val().toUpperCase().Trim();
        $("#tbxScan").val("    ");
        if ($("#btnUpdTp" + tp_no).length == 0) {
            $("#lblMsg").text("未找到托盘");
            return;
        }

        $("#btnUpdTp" + tp_no).click();
    });

    //CD 作番 解决半角输入法问题
    $("#tbxScan").focus(function () {
        var that = $(this);
        that.select();
        that.attr("type", "password");
        setTimeout(function () {
            that.attr("type", "tel");
        }, 100);
    });

    $("#tbxScan").keydown(function (e) {
        //如果回车 检查是否时标签 ，如果时标签  那么读取
        if (e.keyCode == 13) { 
            var tp_no = $(this).val().toUpperCase().Trim();
            $(this).val("    ");
            $("#btnUpdTp" + tp_no).click();
            e.preventDefault ? e.preventDefault() : e.returnValue = false;
        }
    });

    $("#tbxScan").focus(function() {
        $(this).select();
    });
    $("#tbxScan").click(function () {
        $(this).select();
    });

    $("#tbxScan").focus();
});