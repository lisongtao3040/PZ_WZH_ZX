// iframe 全屏弹出层
function openInFrame(url) {
    // 如果已存在则先移除
    $('#iframeOverlay').remove();

    var overlay = $(
        '<div id="iframeOverlay">' +
            '<button id="iframeClose" title="关闭">×</button>' +
            '<iframe src="' + url + '" frameborder="0"></iframe>' +
        '</div>'
    );

    // 点击关闭按钮移除弹出层
    $('#iframeClose', overlay).on('click', function () {
        overlay.remove();
    });

    $('body').append(overlay);
}

function login(table) {
	var user = $("#UserHeader_lblUserCd").text();
	var name = $("#UserHeader_lblUserName").text();
	var db = "wzh_new本番";

    var params = 'user=' + encodeURIComponent(user) + '&name=' + encodeURIComponent(name);
    if (db && table) {
        params += '&db=' + btoa(encodeURIComponent(db)) + '&table=' + btoa(encodeURIComponent(table));
    }
    openInFrame('http://10.160.192.131/DBTableKanri/TableMaint.aspx?' + params);
}

function loginView(view) {
	var user = $("#UserHeader_lblUserCd").text();
	var name = $("#UserHeader_lblUserName").text();
	var db = "wzh_new本番";

    var params = 'user=' + encodeURIComponent(user) + '&name=' + encodeURIComponent(name);
    if (db && view) {
        params += '&db=' + btoa(encodeURIComponent(db)) + '&view=' + btoa(encodeURIComponent(view));

        // 默认条件：v_A01_result_list_2026 按「检查日期=今天」过滤
        if (view === 'v_A01_result_list_2026') {
            var t = new Date();
            var y = t.getFullYear();
            var m = ('0' + (t.getMonth() + 1)).slice(-2);
            var d = ('0' + t.getDate()).slice(-2);
            params += '&filter=' + btoa(encodeURIComponent("检查日期=" + y + '-' + m + '-' + d));
        }else if (view === 'v_A02_check_result_ms_2026') {
            var t = new Date();
            var y = t.getFullYear();
            var m = ('0' + (t.getMonth() + 1)).slice(-2);
            var d = ('0' + t.getDate()).slice(-2);
            params += '&filter=' + btoa(encodeURIComponent("检查日期YMD=" + y + '-' + m + '-' + d));

        }else if (view === 'v_A03_mi_check_result_2026') {
            var t = new Date();
            var y = t.getFullYear();
            var m = ('0' + (t.getMonth() + 1)).slice(-2);
            var d = ('0' + t.getDate()).slice(-2);
            params += '&filter=' + btoa(encodeURIComponent("生成实际日YMD=" + y + '-' + m + '-' + d));
        }else if (view === 'v_A04_result_list_2026') {
            var t = new Date();
            var y = t.getFullYear();
            var m = ('0' + (t.getMonth() + 1)).slice(-2);
            var d = ('0' + t.getDate()).slice(-2);
            params += '&filter=' + btoa(encodeURIComponent("生成实际日=" + y + '-' + m + '-' + d));
        }else if (view === 'v_A05_check_scx_one') {
            var t = new Date();
            var y = t.getFullYear();
            var m = ('0' + (t.getMonth() + 1)).slice(-2);
            var d = ('0' + t.getDate()).slice(-2);
            params += '&filter=' + btoa(encodeURIComponent("检查日期=" + y + '-' + m + '-' + d));
        }

		

		

		

    }

	openInFrame('http://10.160.192.131/DBTableKanri/ViewDisplay.aspx?' + params);
    //window.location.href = 'ViewDisplay.aspx?' + params;
}