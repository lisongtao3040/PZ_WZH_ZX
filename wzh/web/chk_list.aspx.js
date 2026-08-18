// 检查一览 - 分页状态
var pageIndex = 1;
var pageSize = 20;
var totalCount = 0;
var totalPages = 1;

// 状态文字
function getStatusTxt(status) {
    if (status == '0') return '检查中';
    if (status == '1') return '完了';
    if (status == '2') return '继承';
    if (status == '4') return '手入力';
    return status;
}

// 结果文字
function getResultTxt(result) {
    if (result == 'NG') return '<span style="color:red;font-weight:bold;">NG</span>';
    if (result == 'OK') return '<span style="color:green;font-weight:bold;">OK</span>';
    return result;
}

// 截取日期取前 19 位 yyyy-MM-dd HH:mm:ss
function fmtDateTime(val) {
    if (!val) return '';
    return val.substring(0, 19);
}

// 两个条件都为空时显示引导信息
function showNeedCondition() {
    totalCount = 0;
    totalPages = 1;
    updatePager();
    $('#chkListBody').html('<tr><td colspan="18" class="list_empty">请至少输入一个检索条件（预定检查日 或 开始检查日）</td></tr>');
}

// 4 个检索条件是否全为空
function isSearchEmpty() {
    return $('#tbxYoteiChkDateFrom').val().trim() === ''
        && $('#tbxYoteiChkDateTo').val().trim() === ''
        && $('#tbxChkStartDateFrom').val().trim() === ''
        && $('#tbxChkStartDateTo').val().trim() === '';
}

// 检索一览（AJAX → ChkListHandler.ashx）
function loadList() {
    var yotei_chk_date_from = $('#tbxYoteiChkDateFrom').val().trim();
    var yotei_chk_date_to = $('#tbxYoteiChkDateTo').val().trim();
    var chk_start_date_from = $('#tbxChkStartDateFrom').val().trim();
    var chk_start_date_to = $('#tbxChkStartDateTo').val().trim();

    // 所有条件都为空时显示引导信息
    if (isSearchEmpty()) {
        showNeedCondition();
        return;
    }

    $('#chkListBody').html('<tr><td colspan="18" class="list_empty">加载中...</td></tr>');

    $.ajax({
        type: "POST",
        url: "ChkListHandler.ashx",
        contentType: "application/x-www-form-urlencoded; charset=utf-8",
        data: {
            yotei_chk_date_from: yotei_chk_date_from,
            yotei_chk_date_to: yotei_chk_date_to,
            chk_start_date_from: chk_start_date_from,
            chk_start_date_to: chk_start_date_to,
            page_index: pageIndex,
            page_size: pageSize
        },
        dataType: "json",
        success: function (data) {
            if (!data.success) {
                $('#chkListBody').html('<tr><td colspan="18" class="list_empty">检索失败：' + (data.message || '') + '</td></tr>');
                return;
            }

            totalCount = data.total;
            totalPages = totalCount === 0 ? 1 : Math.ceil(totalCount / pageSize);

            // 超出最大页时回到最后一页
            if (pageIndex > totalPages) {
                pageIndex = totalPages;
                loadList();
                return;
            }

            var rows = data.data;
            if (!rows || rows.length === 0) {
                $('#chkListBody').html('<tr><td colspan="18" class="list_empty">没有符合条件的数据</td></tr>');
            } else {
                var html = '';
                for (var i = 0; i < rows.length; i++) {
                    var r = rows[i];
                    html += '<tr>';
                    html += '<td style="text-align:center;"><input type="button" value="删除" class="btn_common_new" onclick="deleteRow(\'' + (r.ck_id || '') + '\',' + (r.rn || '') + ')" /></td>';
                    html += '<td>' + (r.rn || '') + '</td>';
                    html += '<td>' + (r.cd || '') + '</td>';
                    html += '<td>' + (r.no || '') + '</td>';
                    html += '<td>' + (r.department_cd || '') + '</td>';
                    html += '<td>' + (r.line_cd || '') + '</td>';
                    html += '<td>' + (r.chk_user || '') + '</td>';
                    html += '<td>' + fmtDateTime(r.yotei_chk_date) + '</td>';
                    html += '<td>' + fmtDateTime(r.chk_start_date) + '</td>';
                    html += '<td>' + fmtDateTime(r.chk_end_date) + '</td>';
                    html += '<td>' + getStatusTxt(r.status) + '</td>';
                    html += '<td>' + getResultTxt(r.result) + '</td>';
                    html += '<td>' + (r.chk_times || '') + '</td>';
                    html += '<td>' + (r.suu || '') + '</td>';
                    html += '<td>' + (r.qianpin || '') + '</td>';
                    html += '<td>' + (r.shared_ck_id || '') + '</td>';
                    html += '<td>' + (r.upd_user || '') + '</td>';
                    html += '<td>' + fmtDateTime(r.upd_date) + '</td>';
                    html += '</tr>';
                }
                $('#chkListBody').html(html);
            }

            // 分页信息
            updatePager();
        },
        error: function (message) {
            $('#chkListBody').html('<tr><td colspan="18" class="list_empty">请求失败</td></tr>');
        }
    });
}

// 填充页码下拉框
function fillPageSelect() {
    var ddl = $('#ddlPage');
    var current = pageIndex;
    ddl.empty();
    for (var i = 1; i <= totalPages; i++) {
        ddl.append('<option value="' + i + '">' + i + '</option>');
    }
    ddl.val(current);
}

// 更新分页按钮状态与信息
function updatePager() {
    $('#pagerInfo').text('第 ' + pageIndex + ' / ' + totalPages + ' 页　共 ' + totalCount + ' 条');

    $('#btnFirst').prop('disabled', pageIndex <= 1);
    $('#btnPrev').prop('disabled', pageIndex <= 1);
    $('#btnNext').prop('disabled', pageIndex >= totalPages);
    $('#btnLast').prop('disabled', pageIndex >= totalPages);

    fillPageSelect();
}

// 删除行（按 ck_id 删除 t_check 和 t_check_ms）
function deleteRow(ck_id, rn) {
    if (!ck_id) {
        alert('ck_id 为空，无法删除');
        return;
    }
    // RN=1 是根数据，需要两次确认
    if (rn == '1') {
        if (!confirm('这是根数据，真的要删除吗？')) {
            return;
        }
        if (!confirm('删除吗？')) {
            return;
        }
    } else {
        if (!confirm('确定要删除 ck_id=' + ck_id + ' 的检查数据吗？\n（同时删除 t_check 和 t_check_ms 中该 ck_id 的数据）')) {
            return;
        }
    }

    $.ajax({
        type: "POST",
        url: "ChkListHandler.ashx",
        contentType: "application/x-www-form-urlencoded; charset=utf-8",
        data: {
            action: 'delete',
            ck_id: ck_id
        },
        dataType: "json",
        success: function (data) {
            if (data.success) {
                alert('删除成功');
                loadList();
            } else {
                alert('删除失败：' + (data.message || ''));
            }
        },
        error: function () {
            alert('请求失败');
        }
    });
}

// 清除检索条件
function clearSearch() {
    $('#tbxYoteiChkDateFrom').val('');
    $('#tbxYoteiChkDateTo').val('');
    $('#tbxChkStartDateFrom').val('');
    $('#tbxChkStartDateTo').val('');
    pageIndex = 1;
    loadList();
}

$(document).ready(function () {
    // 日期控件（4 个输入框）
    $("#tbxYoteiChkDateFrom,#tbxYoteiChkDateTo,#tbxChkStartDateFrom,#tbxChkStartDateTo").datepicker({
        dateFormat: "yy-mm-dd"
    });

    // 检索按钮（所有条件都空时弹提示）
    $("#btnSearch").click(function () {
        if (isSearchEmpty()) {
            alert('请至少输入一个检索条件（预定检查日 或 开始检查日）');
            return;
        }
        pageIndex = 1;
        loadList();
    });

    // 清除按钮
    $("#btnClear").click(function () {
        clearSearch();
    });

    // 分页按钮
    $("#btnFirst").click(function () {
        pageIndex = 1;
        loadList();
    });
    $("#btnPrev").click(function () {
        if (pageIndex > 1) {
            pageIndex--;
            loadList();
        }
    });
    $("#btnNext").click(function () {
        if (pageIndex < totalPages) {
            pageIndex++;
            loadList();
        }
    });
    $("#btnLast").click(function () {
        pageIndex = totalPages;
        loadList();
    });

    // 页码下拉框跳转
    $("#ddlPage").change(function () {
        var p = parseInt($(this).val(), 10);
        if (p >= 1 && p <= totalPages) {
            pageIndex = p;
            loadList();
        }
    });

    // 每页行数变更
    $("#ddlPageSize").change(function () {
        pageSize = parseInt($(this).val(), 10);
        pageIndex = 1;
        loadList();
    });

    // 回车触发检索（所有条件都空时弹提示）
    $("#tbxYoteiChkDateFrom,#tbxYoteiChkDateTo,#tbxChkStartDateFrom,#tbxChkStartDateTo").keydown(function (e) {
        if (e.keyCode == 13) {
            if (isSearchEmpty()) {
                alert('请至少输入一个检索条件（预定检查日 或 开始检查日）');
                return;
            }
            pageIndex = 1;
            loadList();
            e.preventDefault ? e.preventDefault() : e.returnValue = false;
        }
    });

    // 初始加载
    loadList();
});
