$(function () {
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
var ftlMessage = '';   // API からのお知らせ（例：閲覧できる部門がない場合）

// ===== 一览显示项目（key = 表 [v_TwoMetresFullTrayDetailPinzhi] 的列名）=====
// 顺序 = 画面表示顺序。台车号在行视图里用「合并单元格」显示（见 renderRowView）
var FTL_COLUMNS = [
    { key: 'existTrolleyNo',    label: '台车号',   colClass: 'ftlpz-col-tray' },
    { key: 'stationDepartment', label: '部门',     colClass: 'ftlpz-col-dept' },
    { key: 'stationNo',         label: '站点号',   colClass: 'ftlpz-col-station' },
    { key: 'operatorLine',      label: '操作线',   colClass: 'ftlpz-col-opline' },
    { key: 'productCode',       label: '商品CD',   colClass: 'ftlpz-col-cd' },
    { key: 'packageAmount',     label: '数量',     colClass: 'ftlpz-col-amount' },
    { key: 'destination',       label: '目的地',   colClass: 'ftlpz-col-dest' },
    { key: 'BianCode',          label: '储备计划', colClass: 'ftlpz-col-bian' },
    { key: 'Dn',                label: '工单号',   colClass: 'ftlpz-col-order' },
    { key: 'jizhong',           label: '机种',     colClass: 'ftlpz-col-jizhong' },
    { key: 'lineCodeShort',     label: '生产线号', colClass: 'ftlpz-col-line' }
];

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

// 从 ASMX 取得数据并渲染（品质版：FullTrayListPZApi）
// 画面右上の lblUserCd（ユーザーコード）を送り、閲覧可能な部門のデータだけ取得する
function loadData() {
    var userCd = $('#lblUserCd').text().trim();
    $('#lblStatus').text('加载中...');
    $.ajax({
        url: 'FullTrayListPZApi.asmx/GetData',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: JSON.stringify({ userCd: userCd }),
        success: function (response) {
            var res = JSON.parse(response.d);
            if (res.success) {
                ftlData = res.data;
                ftlMessage = res.message || '';
                applyView();   // 件数（共／储备）は applyView 内で表示する
            } else {
                $('#lblStatus').text('加载失败：' + res.message);
            }
        },
        error: function () {
            $('#lblStatus').text('请求失败，请重试');
        }
    });
}

// 按台车号(existTrolleyNo)分组
function groupByTray(data) {
    var groups = {};
    var order = [];
    $.each(data, function (i, item) {
        var key = item.existTrolleyNo;
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
            return a.productCode > b.productCode ? 1 : (a.productCode < b.productCode ? -1 : 0);
        });
        trays.push({ key: key, items: items });
    });
    // 以每组最小 CD 决定台车顺序
    trays.sort(function (a, b) {
        var ka = a.items[0].productCode;
        var kb = b.items[0].productCode;
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
            return it.productCode && it.productCode.toUpperCase().indexOf(f) >= 0;
        });
    }
    if (ftlCdDir !== 0) {
        data = sortByCd(data, ftlCdDir);
    }

    $('#lblStatus').text('共 ' + data.length + ' 条，储备 ' + countBichu(data) + ' 条' + (ftlMessage ? '　' + ftlMessage : ''));
    renderRowView(data);
    renderPanelView(data);
}

// 储备計画に「備蓄」を含むか（表示・件数カウントで共用）
function hasBichu(item) {
    return !!(item.BianCode && String(item.BianCode).indexOf('備蓄') >= 0);
}

// 「備蓄」を含む明細の件数（＝储备件数）
function countBichu(data) {
    var count = 0;
    $.each(data, function (i, item) {
        if (hasBichu(item)) count++;
    });
    return count;
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

// CD 按 “-” 分段：各段使用不同颜色
var FTL_CD_COLORS = ['#00008B', '#C00000', '#006400', '#8B008B', '#B8860B', '#008B8B'];
function renderCdColored(cd) {
    if (cd === null || cd === undefined || cd === '') return '';
    var parts = String(cd).split('-');
    var html = '';
    for (var i = 0; i < parts.length; i++) {
        if (i > 0) html += '-';
        html += '<span style="color:' + FTL_CD_COLORS[i % FTL_CD_COLORS.length] + '">' + esc(parts[i]) + '</span>';
    }
    return html;
}

// ===== 行视图渲染（台车号を先頭列でセル結合、其余10项目を行ごと表示。按钮なし）=====
function renderRowView(data) {
    var html = '<table class="ftl-row-table"><colgroup>';
    $.each(FTL_COLUMNS, function (i, col) {
        html += '<col class="' + col.colClass + '"/>';
    });
    html += '</colgroup><thead><tr>';
    $.each(FTL_COLUMNS, function (i, col) {
        if (col.key === 'productCode') {
            // CD列：クリックで並べ替え
            html += '<th class="ftl-sort-cd" onclick="toggleCdSort()" title="点击按CD排序">' +
                esc(col.label) + ' <span class="ftl-sort-arrow">' + cdSortArrow() + '</span></th>';
        } else {
            html += '<th>' + esc(col.label) + '</th>';
        }
    });
    html += '</tr></thead><tbody>';

    // 台车号ごとに行をまとめ、先頭列（台车号）は rowspan でセル結合する
    var g = groupByTray(data);
    $.each(g.order, function (i, groupKey) {
        var items = g.groups[groupKey];
        $.each(items, function (j, item) {
            html += '<tr class="ftl-data-row">';
            if (j === 0) {
                // この台車の明細数ぶん結合（既存 t_FullTrayList と同じ .ftl-group-cell）
                html += '<td rowspan="' + items.length + '" class="ftl-group-cell">' + esc(groupKey) + '</td>';
            }
            $.each(FTL_COLUMNS, function (k, col) {
                if (col.key === 'existTrolleyNo') return; // 台车号は結合セルで表示済み
                html += cellHtml(col, item);
            });
            html += '</tr>';
        });
    });

    html += '</tbody></table>';
    $('#rowViewBody').html(html);
}

// CD列の並べ替えマーク
function cdSortArrow() {
    return (ftlCdDir === 1) ? '▲' : (ftlCdDir === -1 ? '▼' : '⇅');
}

// 1項目分の <td> を作る（CD は色分け、储备计划は「備蓄」のみ表示）
function cellHtml(col, item) {
    var val = (item[col.key] === null || item[col.key] === undefined) ? '' : item[col.key];

    if (col.key === 'productCode') {
        return '<td class="ftlpz-cell-cd">' + renderCdColored(val) + '</td>';
    }
    if (col.key === 'lineCodeShort') {
        return '<td class="ftlpz-cell-line">' + esc(val) + '</td>';
    }
    if (col.key === 'BianCode') {
        // 储备计划に「備蓄」を含む場合のみ「備蓄」と表示（既存 t_FullTrayList と同じ）
        return hasBichu(item)
            ? '<td class="ftl-bian-bichu">備蓄</td>'
            : '<td></td>';
    }
    return '<td>' + esc(val) + '</td>';
}

// ===== 面板视图渲染（台车号ごとにカード表示。台车号はカードヘッダ、其余10项目を表示。按钮なし）=====
function renderPanelView(data) {
    var g = groupByTray(data);
    var html = '';

    $.each(g.order, function (i, groupKey) {
        var items = g.groups[groupKey];

        html += '<div class="ftl-tray-panel">';
        html += '<div class="ftl-panel-header">台车号: ' + esc(groupKey) + '（共' + items.length + '件）</div>';

        $.each(items, function (j, item) {
            html += '<div class="ftl-panel-item">';
            $.each(FTL_COLUMNS, function (k, col) {
                if (col.key === 'existTrolleyNo') return; // 台车号はカードヘッダに表示済み
                var val = (item[col.key] === null || item[col.key] === undefined) ? '' : item[col.key];
                if (col.key === 'productCode') {
                    html += '<div class="ftl-panel-row"><span class="ftl-panel-label">' + esc(col.label) + ':</span>' +
                        '<span class="ftl-panel-val ftl-panel-cd">' + renderCdColored(val) + '</span></div>';
                } else if (col.key === 'BianCode') {
                    // 储备计划に「備蓄」を含む場合のみ「備蓄」と表示（既存 t_FullTrayList と同じ）
                    var bichu = hasBichu(item);
                    html += '<div class="ftl-panel-row"><span class="ftl-panel-label">' + esc(col.label) + ':</span>' +
                        '<span class="ftl-panel-val' + (bichu ? ' ftl-bian-bichu' : '') + '">' + (bichu ? '備蓄' : '') + '</span></div>';
                } else {
                    html += panelRow(col.label, val);
                }
            });
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

// ===== 本画面は一覧表示のみ（呼出・検査・自動OK ボタンは持たない）=====
