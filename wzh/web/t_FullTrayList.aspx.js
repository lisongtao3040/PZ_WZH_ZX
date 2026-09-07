$(function () {
    ftlOpUserID = $('#UserHeader_lblUserCd').text().trim();
    ftlOpUserName = $('#UserHeader_lblUserName').text().trim();

    loadData();

    $('#btnSearch').click(function () {
        loadData();
    });

    $('#btnToggleView').click(function () {
        toggleView();
    });
});

// 当前视图模式: 'row' | 'panel'
var ftlViewMode = 'row';

// 从 ASMX 取得数据并渲染
function loadData() {
    $('#lblStatus').text('加载中...');
    $.ajax({
        url: 'FullTrayListApi.asmx/GetData',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: '{}',
        success: function (response) {
            var res = JSON.parse(response.d);
            if (res.success) {
                $('#lblStatus').text('共 ' + res.data.length + ' 条');
                renderRowView(res.data);
                renderPanelView(res.data);
            } else {
                $('#lblStatus').text('加载失败：' + res.message);
            }
        },
        error: function () {
            $('#lblStatus').text('请求失败，请重试');
        }
    });
}

// 按 stationNo（编号/托盘）分组
function groupByTray(data) {
    var groups = {};
    var order = [];
    $.each(data, function (i, item) {
        var key = item.trayNo;
        if (!groups[key]) {
            groups[key] = [];
            order.push(key);
        }
        groups[key].push(item);
    });
    return { groups: groups, order: order };
}

// HTML 转义，防止XSS
function esc(str) {
    return $('<div>').text(str).html();
}

// ===== 行视图渲染（每条数据显示2行）=====
function renderRowView(data) {
    var g = groupByTray(data);
    var colCnt = 6;
    var html = '<table class="ftl-row-table"><colgroup>' +
        '<col class="col-tray-id"/>' +
        '<col class="col-id"/><col class="col-status"/>' +
        '<col class="col-tray-status"/><col class="col-line"/><col class="col-jizhong"/><col class="col-action-m"/>' +
        '</colgroup>' +
        '<thead>' +
        '<tr>' +
        '<th rowspan="2">台车号</th>' +
        '<th>编号(托盘)</th><th>状态</th><th>托盘状态编号</th><th>生产线</th><th>机种 / 结果</th><th>数量</th>' +
        '</tr>' +
        '<tr>' +
        '<th colspan="2">明细书</th><th>订单号</th><th>CD</th><th>向先</th><th>操作</th>' +
        '</tr>' +
        '</thead>' +
        '<tbody>';

    $.each(g.order, function (i, groupKey) {
        var items = g.groups[groupKey];
        var rowspan = items.length * 2;

        $.each(items, function (j, item) {
            var isOK = (item.result && item.result.trim() === 'OK');
            var hasResult = (item.result && item.result.trim() !== '');
            var isLast = (j === items.length - 1);

            // 第1行：编号 / 状态 / 托盘状态编号 / 生产线 / 机种+结果 / 数量
            html += '<tr class="ftl-data-row ftl-row-top">';
            if (j === 0) {
                html += '<td rowspan="' + rowspan + '" class="ftl-group-cell">' + esc(groupKey) + '<br/>';
                html += '<button type="button" class="ftl-btn-call" onclick="callAgv(\'' + esc(groupKey) + '\',\'' + esc(item.stationNo) + '\',this)">呼叫</button>';
                html += '</td>';
            }
            html += '<td>' + esc(item.stationNo) + '</td>';
            html += '<td>' + esc(item.innerCodeDes) + '</td>';
            html += '<td>' + esc(item.stationUse) + '</td>';
            html += '<td>' + esc(item.lineCodeShort) + (item.line_name ? '(' + esc(item.line_name) + ')' : '') + '</td>';
            html += '<td>' + esc(item.jizhong);
            if (hasResult) {
                html += '&nbsp;<span class="ftl-result-badge ftl-result-' + esc(item.result) + '">' + esc(item.result) + '</span>';
            }
            html += '</td>';
            html += '<td style="text-align:right;">' + esc(item.packageAmount) + '</td>';
            html += '</tr>';

            // 第2行：明细书(colspan=2) / 订单号 / CD / 向先 / 操作
            html += '<tr class="ftl-data-row ftl-row-bot' + (isLast ? ' ftl-row-last' : '') + '">';
            html += '<td colspan="2" class="ftl-cell-truncate" title="' + esc(item.Ttxt) + '">' + esc(item.Ttxt) + '</td>';
            html += '<td>' + esc(item.OrderNo) + '</td>';
            html += '<td>' + esc(item.sapCode) + '</td>';
            html += '<td>' + esc(item.destination) + '</td>';
            html += '<td>';
            if (!isOK) {
                html += '<button type="button" class="ftl-btn-check" onclick="onCheckClick(\'' + esc(item.stationNo) + '\',\'' + esc(item.sapCode) + '\',\'' + esc(item.OrderNo) + '\')">检查</button>';
                html += '<button type="button" class="ftl-btn-autook" onclick="onAutoOkClick(\'' + esc(item.stationNo) + '\',\'' + esc(item.sapCode) + '\',\'' + esc(item.OrderNo) + '\')">自动OK</button>';
            }
            html += '</td>';
            html += '</tr>';
        });
    });

    html += '</tbody></table>';
    $('#rowViewBody').html(html);
}

// ===== 面板视图渲染 =====
function renderPanelView(data) {
    var g = groupByTray(data);
    var html = '';

    $.each(g.order, function (i, groupKey) {
        var items = g.groups[groupKey];
        html += '<div class="ftl-tray-panel">';
        // 面板标题：呼叫按钮始终显示（groupKey = trayNo）
        html += '<div class="ftl-panel-header">台车号: ' + esc(groupKey) + '（共' + items.length + '件）';
        html += '&nbsp;<button type="button" class="ftl-btn-call" onclick="callAgv(\'' + esc(groupKey) + '\',\'' + esc(items[0].stationNo) + '\',this)">呼叫</button>';
        html += '</div>';

        $.each(items, function (j, item) {
            var isOK = (item.result && item.result.trim() === 'OK');
            var hasResult = (item.result && item.result.trim() !== '');
            html += '<div class="ftl-panel-item">';
            html += panelRow('台车号', item.trayNo);
            html += panelRow('状态', item.innerCodeDes);
            html += panelRow('托盘状态编号', item.stationUse);
            html += panelRow('生产线', item.lineCodeShort);
            html += panelRow('明细书', item.Ttxt);
            html += panelRow('订单号', item.OrderNo);
            html += panelRow('CD', item.sapCode);
            html += panelRow('数量', item.packageAmount);
            html += panelRow('向先', item.destination);
            html += panelRow('机种', item.jizhong);
            if (hasResult) {
                html += panelRow('检查结果', item.result,
                    item.result === 'OK' ? 'color:#006600;font-weight:bold;' :
                    item.result === 'NG' ? 'color:#cc0000;font-weight:bold;' : '');
            }
            if (!isOK) {
                html += '<div class="ftl-panel-actions">';
                html += '<button type="button" class="ftl-btn-check" onclick="onCheckClick(\'' + esc(item.stationNo) + '\',\'' + esc(item.sapCode) + '\',\'' + esc(item.OrderNo) + '\')">检查</button>';
                html += '</div>';
            }
            html += '</div>'; // ftl-panel-item
        });

        html += '</div>'; // ftl-tray-panel
    });

    $('#panelViewBody').html(html);
}

function panelRow(label, val, style) {
    var styleAttr = style ? ' style="' + style + '"' : '';
    return '<div class="ftl-panel-row"><span class="ftl-panel-label">' + esc(label) + ':</span><span class="ftl-panel-val"' + styleAttr + '>' + esc(val) + '</span></div>';
}

// ===== 行/面板 视图切换 =====
function toggleView() {
    if (ftlViewMode === 'row') {
        ftlViewMode = 'panel';
        $('#rowView').hide();
        $('#panelView').show();
        $('#btnToggleView').val('切换→行视图');
    } else {
        ftlViewMode = 'row';
        $('#panelView').hide();
        $('#rowView').show();
        $('#btnToggleView').val('切换→面板视图');
    }
}

// ===== AGV 呼叫 =====
// callIEQ: 呼叫设备编号，根据实际业务填写
var ftlCallIEQ = 'PAD';
var ftlOpUserID = '';
var ftlOpUserName = '';

// 当前待执行的呼叫参数
var agvPending = {};

// 点击呼叫按钮 → 显示确认对话框
function callAgv(trolleyNo, stationNo1, btnEl) {
    agvPending = {
        callIEQ: ftlCallIEQ,
        trolleyNo: trolleyNo,
        stationNo1: stationNo1,
        stationNo2: '',
        opUserID: ftlOpUserID,
        opUserName: ftlOpUserName,
        btnEl: btnEl
    };

    // 对话框中填入参数明细
    $('#dp_callIEQ').text(ftlCallIEQ || '（未设定）');
    $('#dp_trolleyNo').text(trolleyNo);
    $('#dp_stationNo1').text(stationNo1);
    $('#dp_opUserID').text(ftlOpUserID || '（未设定）');
    $('#dp_opUserName').text(ftlOpUserName || '（未设定）');

    $('#agvCallDialog').dialog({
        title: 'AGV 呼叫确认',
        modal: true,
        width: 480,
        resizable: false,
        buttons: [
            {
                text: '执行',
                'class': 'ftl-dlg-exec',
                click: function () {
                    $(this).dialog('close');
                    executeAgv();
                }
            },
{
                text: '取消',
                click: function () {
                    $(this).dialog('close');
                }
            }
        ]
    });
}

// 对话框"执行"后实际调用接口
function executeAgv() {
    var p = agvPending;
    $(p.btnEl).prop('disabled', true).text('呼叫中...');

    $.ajax({
        url: 'FullTrayListApi.asmx/CallAgv',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: JSON.stringify({
            callIEQ: p.callIEQ,
            trolleyNo: p.trolleyNo,
            stationNo1: p.stationNo1,
            stationNo2: p.stationNo2,
            opUserID: p.opUserID,
            opUserName: p.opUserName
        }),
        success: function (response) {
            var res = JSON.parse(response.d);
            if (res.success) {
                var msg = '呼叫API调用成功';
                if (res.outBl === 'true') {
                    msg += '\n台车呼叫成功';
                }
                msg += '\n' + res.outMess;
                alert(msg);
                $(p.btnEl).text('已呼叫');
            } else {
                alert('呼叫失败：' + res.message);
                $(p.btnEl).prop('disabled', false).text('呼叫');
            }
        },
        error: function () {
            alert('呼叫请求失败，请重试');
            $(p.btnEl).prop('disabled', false).text('呼叫');
        }
    });
}

// 执行3：从浏览器直接 AJAX POST 到 AGV 接口（不经后台中转）
function executeAgv3() {
    var p = agvPending;
    $(p.btnEl).prop('disabled', true).text('呼叫中...');

    $.ajax({
        url: 'http://10.160.192.20:8083/webSerAgvCall.asmx/taskCall_New',
        type: 'POST',
        contentType: 'application/x-www-form-urlencoded',
        data: {
            callIEQ: p.callIEQ,
            callTaskTypeID: '98',
            trolleyNo: p.trolleyNo,
            stationNo1: p.stationNo1,
            stationNo2: p.stationNo2,
            dueTime: '',
            opUserID: p.opUserID,
            opUserName: p.opUserName,
            TrolleyType: ''
        },
        success: function (res) {
            alert('呼叫成功：' + res);
            $(p.btnEl).text('已呼叫');
        },
        error: function (xhr, status, err) {
            alert('呼叫失败：' + (err || status));
            $(p.btnEl).prop('disabled', false).text('呼叫');
        }
    });
}

// ===== 検査ボタン：隠しフィールド経由で Server.Transfer =====
function onCheckClick(stationNo, sapCode, orderNo) {
    document.getElementById('hid_chk_cd').value = sapCode;
    document.getElementById('hid_chk_no').value = orderNo;
    document.getElementById('btnChkServer').click();
}

// ===== 自动OK ボタン：確認後に後台専用関数を呼び出す =====
function onAutoOkClick(stationNo, sapCode, orderNo) {
    $('#autoOkConfirmDialog').data('params', { stationNo: stationNo, sapCode: sapCode, orderNo: orderNo });
    $('#autoOkConfirmDialog').dialog({
        title: '自动OK 确认',
        modal: true,
        width: 360,
        resizable: false,
        buttons: [
            {
                text: '确认',
                'class': 'ftl-dlg-exec',
                click: function () {
                    var p = $(this).data('params');
                    $(this).dialog('close');
                    document.getElementById('hid_chk_cd').value = p.sapCode;
                    document.getElementById('hid_chk_no').value = p.orderNo;
                    document.getElementById('btnAutoOkServer').click();
                }
            },
            {
                text: '取消',
                click: function () {
                    $(this).dialog('close');
                }
            }
        ]
    });
}
