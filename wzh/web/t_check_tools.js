$(document).ready(function () {

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
            
            var allScanned = true;
            var barcode = $(this).val().toUpperCase();
            $(this).val(barcode);
            //
            $(".barcode").each(function () {
                //如果匹配了扫描CD
                if (barcode == $(this).attr("placeholder")) {
                    $(this).val(barcode);
                    $(this).css("background-color", "green");                   
                }
                //如果值与预设相等
                if ($(this).val() != $(this).attr("placeholder")) {
                    allScanned = false;
                }
            });

            $(this)[0].select();
            if (allScanned == true) {
                //alert("所有治具正确，迁移到检查画面！");
                $("#btnUpdToolsFlg").click();
            }
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