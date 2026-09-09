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

    // CD 筛选：虚拟键盘
    buildCdKeypad();
    $('#txtCdFilter').focus(function () {
        $('#cdKeypad').show();
    });
    $('#txtCdFilter').click(function () {
        $('#cdKeypad').show();
    });
    $('#txtCdFilter').blur(function () {
        $('#cdKeypad').hide();
    });
    // 键盘按钮点击不抢焦点（保持输入框 focus，键盘不消失）
    $('#cdKeypad').mousedown(function (e) {
        e.preventDefault();
    });
    $('#cdKeypad').on('click', '.ftl-key', function () {
        var ch = $(this).data('ch');
        if (ch === 'done') {
            $('#cdKeypad').hide();
        } else if (ch === 'back') {
            delCdChar();
        } else {
            addCdChar(ch);
        }
    });
    $('#btnCdClear').click(function () {
        $('#txtCdFilter').val('');
        doCdFilter();
        $('#txtCdFilter').focus();
    });
});

// 当前视图模式: 'row' | 'panel'
var ftlViewMode = 'row';
var ftlData = [];      // 最近取得的数据
var ftlCdDir = 0;      // CD排序: 0不排序 / 1升序 / -1降序
var ftlCdFilter = '';  // CD筛选关键字

// 虚拟键盘：仅大写英文与数字
var CD_KEYS = [
    ['Q', 'W', 'E', 'R', 'T', 'Y', 'U', 'I', 'O', 'P'],
    ['A', 'S', 'D', 'F', 'G', 'H', 'J', 'K', 'L'],
    ['Z', 'X', 'C', 'V', 'B', 'N', 'M'],
    ['1', '2', '3', '4', '5', '6', '7', '8', '9', '0']
];

// 构建虚拟键盘 DOM（只构建一次）
function buildCdKeypad() {
    if ($('#cdKeypad').length > 0) return;
    var html = '<div id="cdKeypad" class="ftl-keypad" style="display:none;">';
    $.each(CD_KEYS, function (r, row) {
        html += '<div class="ftl-keypad-row">';
        $.each(row, function (c, ch) {
            html += '<button type="button" class="ftl-key" data-ch="' + ch + '">' + ch + '</button>';
        });
        html += '</div>';
    });
    html += '<div class="ftl-keypad-row">' +
        '<button type="button" class="ftl-key ftl-key-wide" data-ch="back">退格</button>' +
        '<button type="button" class="ftl-key ftl-key-wide ftl-key-done" data-ch="done">完成</button>' +
        '</div>';
    html += '</div>';
    $('body').append(html);
}

// 应用 CD 筛选并刷新视图
function doCdFilter() {
    ftlCdFilter = $('#txtCdFilter').val().toUpperCase();
    $('#txtCdFilter').val(ftlCdFilter);
    applyView();
}

// 虚拟键盘输入一个字符
function addCdChar(ch) {
    var el = $('#txtCdFilter');
    if (el.val().length < 30) {
        el.val(el.val() + ch);
    }
    doCdFilter();
}

// 虚拟键盘退格
function delCdChar() {
    var el = $('#txtCdFilter');
    el.val(el.val().slice(0, -1));
    doCdFilter();
}

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
                ftlData = res.data;
                applyView();
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

// 按 CD 排序（台车分组整体移动，不拆散台车号 group；组内也按 CD 排）
function sortByCd(data, dir) {
    var g = groupByTray(data);
    var trays = [];
    $.each(g.order, function (i, key) {
        var items = g.groups[key].slice().sort(function (a, b) {
            return a.sapCode > b.sapCode ? 1 : (a.sapCode < b.sapCode ? -1 : 0);
        });
        trays.push({ key: key, items: items });
    });
    // 以每组最小 CD 决定台车顺序
    trays.sort(function (a, b) {
        var ka = a.items[0].sapCode;
        var kb = b.items[0].sapCode;
        var r = ka > kb ? 1 : (ka < kb ? -1 : 0);
        return dir * r;
    });
    var out = [];
    $.each(trays, function (i, t) {
        $.each(t.items, function (j, it) { out.push(it); });
    });
    return out;
}

// 用当前 CD 筛选 + CD 排序 刷新行/面板两个视图
function applyView() {
    var data = ftlData;

    // CD 筛选（部分匹配；每行仍按台车分组渲染，不拆散 group）
    if (ftlCdFilter !== '') {
        var f = ftlCdFilter.toUpperCase();
        data = $.grep(ftlData, function (it) {
            return it.sapCode && it.sapCode.toUpperCase().indexOf(f) >= 0;
        });
    }
    if (ftlCdDir !== 0) {
        data = sortByCd(data, ftlCdDir);
    }

    $('#lblStatus').text('共 ' + data.length + ' 条');
    renderRowView(data);
    renderPanelView(data);
}

// 点击“CD”表头：升序 → 降序 → 还原
function toggleCdSort() {
    ftlCdDir = (ftlCdDir === 0) ? 1 : (ftlCdDir === 1 ? -1 : 0);
    applyView();
}

// HTML 转义，防止XSS
function esc(str) {
    return $('<div>').text(str).html();
}

// ===== 行视图渲染（单行表：台车号/生产线/订单号/CD/储备计划/初检/三方/操作）=====
function renderRowView(data) {
    var g = groupByTray(data);
    var html = '<table class="ftl-row-table"><colgroup>' +
        '<col class="col-tray-id"/>' +
        '<col class="col-line"/><col class="col-order"/>' +
        '<col class="col-cd"/><col class="col-bian"/>' +
        '<col class="col-first"/><col class="col-third"/>' +
        '<col class="col-action"/>' +
        '</colgroup>' +
        '<thead><tr>' +
        '<th>台车号</th><th>生产线</th><th>订单号</th>' +
        '<th class="ftl-sort-cd" onclick="toggleCdSort()" title="点击按CD排序">CD <span class="ftl-sort-arrow">' + (ftlCdDir === 1 ? '▲' : (ftlCdDir === -1 ? '▼' : '⇅')) + '</span></th><th>储备计划</th><th>初检</th><th>三方</th><th>操作</th>' +
        '</tr></thead>' +
        '<tbody>';

    $.each(g.order, function (i, groupKey) {
        var items = g.groups[groupKey];
        var rowspan = items.length;

        $.each(items, function (j, item) {
            var isOK = (item.result && item.result.trim() === 'OK');

            html += '<tr class="ftl-data-row">';
            if (j === 0) {
                html += '<td rowspan="' + rowspan + '" class="ftl-group-cell">' + esc(groupKey) + '<br/>';
                html += '<button type="button" class="ftl-btn-call" onclick="callAgv(\'' + esc(groupKey) + '\',\'' + esc(item.stationNo) + '\',this)">呼叫</button>';
                html += '</td>';
            }
            html += '<td>' + esc(item.lineCodeShort) + (item.line_name ? '(' + esc(item.line_name) + ')' : '') + '</td>';
            var isBianBichu = (item.bianCode && item.bianCode.indexOf('備蓄') >= 0);
            html += '<td>' + esc(item.OrderNo) + '</td>';
            html += '<td>' + esc(item.sapCode) + '</td>';
            html += '<td' + (isBianBichu ? ' class="ftl-bian-bichu"' : '') + '>' + esc(item.bianCode) + '</td>';
            html += '<td>' + esc(item.firstCheck) + '</td>';
            html += '<td>' + esc(item.thirdParty) + '</td>';
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

// ===== 面板视图渲染（台车号/生产线/订单号/CD/储备计划/初检/三方 + 操作）=====
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
            var isBianBichu = (item.bianCode && item.bianCode.indexOf('備蓄') >= 0);
            html += '<div class="ftl-panel-item">';
            html += panelRow('生产线', item.lineCodeShort);
            html += panelRow('订单号', item.OrderNo);
            html += panelRow('CD', item.sapCode);
            html += '<div class="ftl-panel-row"><span class="ftl-panel-label">储备计划:</span><span class="ftl-panel-val' + (isBianBichu ? ' ftl-bian-bichu' : '') + '">' + esc(item.bianCode) + '</span></div>';
            html += panelRow('初检', item.firstCheck);
            html += panelRow('三方', item.thirdParty);
            if (!isOK) {
                html += '<div class="ftl-panel-actions">';
                html += '<button type="button" class="ftl-btn-check" onclick="onCheckClick(\'' + esc(item.stationNo) + '\',\'' + esc(item.sapCode) + '\',\'' + esc(item.OrderNo) + '\')">检查</button>';
                html += '<button type="button" class="ftl-btn-autook" onclick="onAutoOkClick(\'' + esc(item.stationNo) + '\',\'' + esc(item.sapCode) + '\',\'' + esc(item.OrderNo) + '\')">自动OK</button>';
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
        resizable: false
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
