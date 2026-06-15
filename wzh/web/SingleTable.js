var pubBase64 = new Base64();



; (function (global) {
    //开启严格模式，规范代码，提高浏览器运行效率
    "use strict";
    var dbTable = function (JqstrMainPanel, JqstrMsPanel, JqstrPageNumPanel, TableEnName, TableCnName, PageIndex, userCd, userName, ColParams, PubInitParams, awaysWhere, OnePageRowCount, onInitSelMs) {
        if (onInitSelMs == undefined) {
            this.InitSelMs = true;
        } else {
            this.InitSelMs = onInitSelMs;
        }
        //alert(this.InitSelMs);

        var ctrlDown = false,
            ctrlKey = 17,
            cmdKey = 91,
            vKey = 86,
            cKey = 67;
        var that = this;
        this.JqstrMainPanel = JqstrMainPanel;               //主区域   <article>
        //this.JqstrMsPanel = JqstrMsPanel;                 //明细区域  .jqPanelTable       未使用！！！
        //this.JqstrPageNumPanel = JqstrPageNumPanel;       //页数区域  .jqPanelNumOfPages  未使用！！！
        this.JqstrPageNumPanel = ".jqPanelNumOfPages";
        this.OnePageRowCount = OnePageRowCount;                          //每页行数
        this.AllDataRowCount = 0;                           //总行数
        this.PageIndex = PageIndex;                         //当前第几页
        this.TableEnName = TableEnName;   //英文表名
        this.TableCnName = TableCnName;   //汉字表名
        this.Columns = {};
        this.Rows = {};
        this.awaysWhere = awaysWhere;
        this.userCd = userCd;
        this.userName = userName;
        this.RowSepartor = "‡";
        this.ColumnSepartor = "†";
        this.ColParams = ColParams;
        this.UnEditColsName = ["upd_user", "upd_date", "ins_user", "ins_date", "button"];
        this.DateColsName = ["upd_date", "ins_date", "yotei_chk_date", "chk_start_date", "chk_end_date"
            , "执行检查日"
            , "生成实际日"
            , "检查日期"
            , "检查日期YMD"
            , "检查开始时间"
            , "检查终了时间"
            , "纳期日"
            , "生成实际日YMD"
        ];
        this.FixColsName = ["upd_date", "ins_date", "upd_user", "ins_user", "生成实际日YMD"];
        this.preWhereStr = "";
        this.ctrlDown = false;
        //表格对象
        this.tableEle = $("<table class='db_table'></table>");
        this.divHeaderEle = {};
        this.divBodyEle = {};
        this.tableTheaderEle = {};
        this.tableTbodyEle = {};
        var Ens = [];
        //判断是否是视图
        if (TableEnName.substring(0, 2) == "v_") {
            this.IsView = true;
        } else {
            this.IsView = false;
        }
        //设置列宽
        this.GetTableColsWidthInfo = function () {
            var jqHeader = that.tableTheaderEle.find("tr:first");
            var jqBody = that.tableTbodyEle.find("tr:first");
            var i, len;
            var allWidthSum = 0;
            var arrWds = [];
            var maxWidth;
            for (i = 0, len = that.Columns.length; i < len; i++) {
                maxWidth = 0
                if (!that.ColParams[Ens[i]].visible) {//如果项目表示属性False 那么不表示
                    continue;
                } else if (i == 0) {//控
                    maxWidth = that.IsView ? 60 : 60;//如果是视图
                } else if (i == 1) {//号
                    maxWidth = 100;
                } else if (that.DateColsName.indexOf(Ens[i]) >= 0) { //日期列 ： 宽度
                    maxWidth = that.IsView ? 180 : 90;//如果是视图
                } else {
                    maxWidth = Math.max(jqHeader.children('td').eq(i).width(), jqBody.children('td').eq(i).width());
                    if (maxWidth < 50) maxWidth = 50;
                }
                //如果设置了宽度
                if (that.ColParams[Ens[i]].width > 0) maxWidth = that.ColParams[Ens[i]].width;
                arrWds.push(maxWidth);
                allWidthSum = allWidthSum + maxWidth;
            }
            return { sumWidth: allWidthSum, arrWds: arrWds };
        };

        //设置列宽
        this.SetColumnsWidth = function () {
            var pianyi = 50;//偏移量
            var jqHeaderTr = that.tableTheaderEle.find("tr:first");
            var jqBodyTr = that.tableTbodyEle.find("tr:first");
            var i;
            var cnt = jqHeaderTr.children('td').length;
            //清理宽度
            that.tableTheaderEle.css("width", "auto");
            that.tableTheaderEle.find("td").css("width", "auto");
            that.tableTbodyEle.css("width", "auto");

            var widthInfo;
            //循环两场
            for (i = 0; i <= 1; i++) {
                widthInfo = that.GetTableColsWidthInfo();
                (that.tableTheaderEle).width(widthInfo.sumWidth + pianyi);//偏移
                (that.tableTbodyEle).width(widthInfo.sumWidth + pianyi);  //偏移
            }
            //最后一列不设置宽度
            for (i = 0; i <= cnt - 2; i++) {
                var tdH = jqHeaderTr.children('td').eq(i);
                var tdB = jqBodyTr.children('td').eq(i);
                tdH.width(widthInfo.arrWds[i]);
                tdB.width(tdH.width());
            }

            that.ResizeControls();
            return;
        };

        //设置明细区域大小Size
        this.ResizeControls = function () {
            that.divBodyEle.height($(window).height() - 230);
            var dWd = $(document).width() - 50;
            var tblWd = that.tableTbodyEle.width();
            var wd = dWd > tblWd ? tblWd : dWd;
            that.divHeaderEle.width(wd);
            if (that.divBodyEle.height() > that.tableTbodyEle.height()) {
                that.divBodyEle.width(wd + 5);
            } else {
                that.divBodyEle.width(wd + 20);
            }
        }
        //行种类
        var rowTypeParam = { general: 0, ins: 1 };
        //是否时可编辑行
        this.IsEditCell = function (colIdx, rowType) {
            if (colIdx <= 1) { //如果第一二列
                return false;
            } else if (that.Columns[colIdx].pk == "P") { //isInsCell
                return (rowType == rowTypeParam.ins); //rowType: 0:普通 1：登录
            } else if (that.UnEditColsName.indexOf(Ens[colIdx]) >= 0) {//["upd_user", "upd_date", "ins_user", "ins_date", "button"]
                return false;
            }
            return true;
        };
        //画面覆盖
        this.coverEle = {};
        this.Cover = function () {
            //var arr = [];
            //arr.push("<div id='cover' style='position: absolute; width: 100%; height: 100%; left: 0px; top: 0px; z-index: 100000; opacity: 0.5; background-color: #ddd; text-align: center;'></div>");
            //arr.push("");
            return $("<div id='cover' style='position: absolute; width: 100%; height: 100%; left: 0px; top: 0px; z-index: 100000; opacity: 0.5; background-color: #ddd; text-align: center;'></div>");
        }

        //表示画面信息
        this.Msg = function (title, txt, fn, debugTxt) {
            var arr = [];
            arr.push("<div id='msg_panel' class='div_msg_panel'>");
            arr.push("<div id='msg_header' class='div_msg_panel_header'>");
            arr.push(title == '' ? "Error" : title);
            //if (title == '') {
            //    arr.push("Error");
            //} else {
            //    arr.push(title);
            //}
            arr.push("</div>");
            arr.push("<div id='msg_body' class='div_msg_panel_body'>");
            arr.push(txt.replace(/\\u003cbr\\u003e/g, '<br>'));
            arr.push("</div>");
            arr.push("<div id='msg_footer' class='div_msg_panel_footer'>");
            arr.push("<input class='msg_body_close' type='button' value='关闭'>");
            arr.push("</div>");
            if (debugTxt != undefined) {
                arr.push("<div id='msg_debug' class='div_msg_panel_debug'>");
                arr.push(debugTxt.replace(/\\u003cbr\\u003e/g, '<br>'));
                arr.push("</div>");
            }
            arr.push("</div>");
            var ele = $(arr.join(""));
            ele.appendTo("body");
            var msg_body_close = ele.find(".msg_body_close");
            msg_body_close.focus();
            msg_body_close.click(function () {
                setTimeout(function () {
                    ele.remove();
                    if (fn != undefined) {
                        fn();
                    }
                }, 20);
            });
        };

        //画面行
        this.Ajax = function (fncName, jsonData) {
            var rtv = "";
            var thatArguments = arguments;
            var ele = that.Cover();
            ele.appendTo("body");
            $.ajax({
                type: "POST",
                url: "TableApi.asmx/" + fncName,
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

        //获得表信息
        this.InitTableInfo = function (tableName) {
            var rtv = that.Ajax("GetTableInfoJson", JSON.stringify({ tableName: tableName }));
            //System
            if (rtv.result == 'NG' || rtv.data.length == 0) {
                return rtv;
                //} else if (rtv.data.length == 0) {
                //    return rtv;
            } else {
                //第一列 删除按钮 追加前两列 （No 和 按钮列）
                var objDel = [
                    {
                        table_name: tableName,
                        columns_name_en: '🔨',
                        columns_name_cn: '控',
                        columns_type: 'varchar',
                        columns_length: 20,
                        pk: '',
                        DefaultValue: '',
                        IsNullable: '1'
                    }
                ];
                var objNo = [
                    {
                        table_name: tableName,
                        columns_name_en: 'No',
                        columns_name_cn: '号',
                        columns_type: 'varchar',
                        columns_length: 20,
                        pk: '',
                        DefaultValue: '',
                        IsNullable: '1'
                    }
                ];
                objDel = objDel.concat(objNo);
                //var btnCol = [{ table_name: tableName, columns_name_en: 'button', columns_name_cn: '编辑', columns_type: 'varchar', columns_length: 20, pk: '', DefaultValue: '', IsNullable: '1' }];
                that.Columns = objDel.concat(rtv.data);

                //填充属性
                var c, len;
                for (c = 0, len = that.Columns.length; c < len; c++) {
                    Ens.push(that.Columns[c].columns_name_en);
                    //列对象
                    if (that.ColParams[Ens[c]] == undefined) {
                        that.ColParams[Ens[c]] = {};
                    }
                    //列对象 宽度
                    if (that.ColParams[Ens[c]].width == undefined) {
                        that.ColParams[Ens[c]].width = 0;
                    }
                    //reg
                    if (that.ColParams[Ens[c]].reg == undefined) {
                        that.ColParams[Ens[c]].reg = null;
                    }
                    //tooltip
                    if (that.ColParams[Ens[c]].tooltip == undefined) {
                        that.ColParams[Ens[c]].tooltip = null;
                    }
                    //cType
                    if (that.ColParams[Ens[c]].cType == undefined) {
                        that.ColParams[Ens[c]].cType = 'cell';
                    }
                    if (that.ColParams[Ens[c]].url == undefined) {
                        that.ColParams[Ens[c]].url = null;
                    }
                    if (that.ColParams[Ens[c]].click == undefined) {
                        that.ColParams[Ens[c]].click = null;
                    }
                    if (that.ColParams[Ens[c]].visible == undefined) {
                        that.ColParams[Ens[c]].visible = true;
                    }
                    if (that.ColParams[Ens[c]].readCookie == undefined) {
                        that.ColParams[Ens[c]].readCookie = '';
                    }
                    if (that.ColParams[Ens[c]].sel_row_enable == undefined) {
                        that.ColParams[Ens[c]].sel_row_enable = true;
                    }
                    if (that.ColParams[Ens[c]].ms_row_enable == undefined) {
                        that.ColParams[Ens[c]].ms_row_enable = true;
                    }
                    if (that.ColParams[Ens[c]].ins_row_enable == undefined) {
                        that.ColParams[Ens[c]].ins_row_enable = true;
                    }
                    if (that.ColParams[Ens[c]].IsEmpty_enable == undefined) {
                        that.ColParams[Ens[c]].IsEmpty_enable = false;
                    }
                }
                return true;
            }
        };

        //获得条件字符串
        this.GetSelWhere = function () {
            var colIdx = 0;
            var arr = [];
            $(".tr_sel").find("td").each(function () {
                if (colIdx >= 2) {
                    if (that.DateColsName.indexOf(Ens[colIdx]) >= 0) {
                        var arrDate = [];
                        $(this).find("input").each(function () {
                            arrDate.push($(this).val());
                        })
                        arr.push(arrDate.join("|"));
                    }
                    else {
                        if ($(this).attr("cType") == 'listbox') {
                            arr.push($(this).find(".lstValue").text());
                        } else {
                            arr.push($(this).text());
                        }
                    }
                }
                colIdx++;
            });
            return arr.length > 0 ? arr.join(that.ColumnSepartor) : "";
        };

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
        };

        //明细 数据
        this.InitTableData = function (tableName, pageIndex, OnePageRowCount, rowValues) {
            var rtv;
            rtv = that.Ajax("GetCntAndMs", JSON.stringify({ tableName: tableName, pageIndex: pageIndex, OnePageRowCount: OnePageRowCount, rowValues: rowValues, awaysWhere: awaysWhere, maxCnt:0 }));
            return rtv;
        };

        //明细 数据
        this.InitTableData2 = function (tableName, pageIndex, OnePageRowCount, rowValues) {
            var rtv;
            rtv = that.Ajax("GetCntAndMs", JSON.stringify({ tableName: tableName, pageIndex: pageIndex, OnePageRowCount: OnePageRowCount, rowValues: rowValues, awaysWhere: awaysWhere, maxCnt:50000 }));
            return rtv;
        };

        //明细 行数
        this.InitAllPageCount = function (tableName, rowValues) {
            var rtv = that.Ajax("GetTableCountJson", JSON.stringify({ tableName: tableName, rowValues: rowValues, awaysWhere: awaysWhere }));
            that.AllDataRowCount = rtv.data;
            return rtv;
        };

        //获得对象
        this.GetObj = function () {
            var i, len;
            var obj = arguments[0];
            if (obj == undefined) return null;
            for (i = 1, len = arguments.length; i < len; i++) {
                if (obj[arguments[i]] == undefined) {
                    return null;
                } else {
                    obj = obj[arguments[i]];
                }
            }
            return obj;
        };

        //检查单元格输入内容
        this.ChkCellValue = function (e, checkNull, action) {
            var td = that.findParentTag(e, 'td');
            var colIdx = td.index();
            var column = that.GetObj(that.Columns, colIdx);
            if (column == null) {
                return { rtv: false, msg: '属性不存在：(that.Columns, colIdx）' + colIdx };
            }
            var txt;
            if (td.attr("cType") == 'listbox') {
                txt = td.find(".lstValue").text();
            } else {
                txt = $(e).is('td') ? $(e).text() : $(e).val()
            }

            if (that.ColParams[Ens[colIdx]].IsEmpty_enable && txt == "") {
                return { rtv: true, msg: '' };
            }

            if (column.IsNullable == "0" || column.pk == "P") {
                if (checkNull && txt.Trim() == "") {
                    return { rtv: false, msg: '项目必须输入' };
                }
            }

            if (that.ColParams[Ens[colIdx]].reg != null) {
                if (!that.ColParams[Ens[colIdx]].reg.test(txt)) {
                    return { rtv: false, msg: '输入规则不符合' };
                }
            }

            return { rtv: true, msg: '' };
        };

        //Parent Tag
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

        //Prev Cell
        this.findPreEnabledCell = function (e) {
            var td = that.findParentTag(e, "td");
            if (td == null) return null;
            var preTd;
            if (td.prev().length > 0) {
                preTd = td.prev();
                while (preTd.attr("IsEdit") != "1") {
                    if (preTd.prev().length > 0) {
                        preTd = preTd.prev();
                    } else {
                        return that.findPreRowFirstEnabledCell(td);
                        //return null;
                    }
                }
                return preTd.attr("IsEdit") == "1" ? preTd : null;
            } else {
                return that.findPreRowFirstEnabledCell(td);
                //return null;
            }
        };

        //Next Cell
        this.findNextEnabledCell = function (e) {
            var td = that.findParentTag(e, "td");
            if (td == null) return null;
            var preTd;
            if (td.next().length > 0) {
                preTd = td.next();
                while (preTd.attr("IsEdit") != "1") {
                    if (preTd.next().length > 0) {
                        preTd = preTd.next();
                    } else {
                        return that.findNextRowFirstEnabledCell(td);
                        //return null;
                    }
                }
                return preTd.attr("IsEdit") == "1" ? preTd : null;
            } else {
                return that.findNextRowFirstEnabledCell(td);
            }
        };

        //Up Cell
        this.findUpEnabledCell = function (e) {
            var tr = that.findParentTag(e, "tr");
            var td = that.findParentTag(e, "td");
            if (tr.prev().length == 0) {
                return null;
            } else {
                return tr.prev().children().get(td.index());
            }
        };

        //Down Cell
        this.findDownEnabledCell = function (e) {
            var tr = that.findParentTag(e, "tr");
            var td = that.findParentTag(e, "td");
            if (tr.next().length == 0) {
                return null;
            } else {
                return tr.next().children().get(td.index());
            }
        };

        //获得下一个可用Cell
        this.findNextRowFirstEnabledCell = function (e) {
            var tr = that.findParentTag(e, "tr");
            if (tr.next().length == 0) {
                return null;
            } else {
                return that.findNextEnabledCell(tr.next().children().get(0));
            }
        };

        //获得前一个可用Cell
        this.findPreRowFirstEnabledCell = function (e) {
            var tr = that.findParentTag(e, "tr");
            //var td = that.findParentTag(e, "td");
            if (tr.prev().length == 0) {
                return null;
            } else {
                return that.findPreEnabledCell(tr.prev().children().get(tr.prev().children().length - 1));
            }
        };

        //Cell 按键按下
        this.cellKeyDown = function (e, obj) {
            if (that.ctrlDown && (e.keyCode == vKey)) { }
            //left
            if ((event.shiftKey && e.keyCode == 9) || (event.shiftKey && e.keyCode == 13 || e.keyCode == 37)) {
                var tmpCell = that.findPreEnabledCell(obj);
                if (tmpCell != null) {
                    that.divBodyEle.scrollLeft(that.divBodyEle.scrollLeft() - $(obj).width());
                }
                that.SetCellFocus(tmpCell);
                event.stopPropagation(); //  阻止事件冒泡
                return false;
            } else if (e.keyCode == 13 || e.keyCode == 9 || e.keyCode == 39) {//shift tab enter right
                var tmpCell = that.findNextEnabledCell(obj);
                that.SetCellFocus(tmpCell);
                e.keyCode = 0;
                event.stopPropagation(); //  阻止事件冒泡
                return false;
            } else if (e.keyCode == 27) {// ESC
                $(obj).val(txt);
            } else if (e.keyCode == 38) { //Up
                if ($(obj).is('select')) {
                    return true;
                } else {
                    var tmpCell = that.findUpEnabledCell(obj);
                    that.SetCellFocus(tmpCell);
                    event.stopPropagation(); //  阻止事件冒泡
                    return false;
                }
            } else if (e.keyCode == 40) { //DOWN
                if ($(obj).is('select')) {
                    return true;
                } else {
                    var tmpCell = that.findDownEnabledCell(obj);
                    that.SetCellFocus(tmpCell);
                    event.stopPropagation(); //  阻止事件冒泡
                    return false;
                }
            }
        }

        //设置单元格焦点
        this.SetCellFocus = function (tmpCell) {
            if (tmpCell != null) {
                tmpCell.find('div').focus();
            }
        };

        //判断 数组第一项 或 两个值 是否相等
        this.isEq = function (v1, v2) {
            if (v1.constructor === Array && v2.constructor === Array) {
                return v1[0] == v2[0];
            } else {
                return v1 == v2;
            }
        }

        //Cell 输入
        this.cellInput = function () {

            var cellTd = that.findParentTag(this, "td");
            var cellTr = cellTd.parent();
            var colIdx = cellTd.index();
            var cType = cellTd.attr("cType");
            var rType = cellTr.attr("rowType");

            if (rType == 'h' && that.ColParams[Ens[colIdx]].sel_row_enable == false) { //检索行 && 不让检索
                return false;
            } else if (rType == '0' && that.ColParams[Ens[colIdx]].ms_row_enable == false) {//更新行 && 不可编辑
                return false;
            } else if (rType == '0' && that.IsView) {// 明细行 如果是 VIEW 编辑不可
                return true;
            } else if (rType == '1' && that.ColParams[Ens[colIdx]].ins_row_enable == false) {//登录行&& 不可登录
                return false;
            }

            var IsEdit = cellTd.attr("IsEdit");
            if (colIdx == 0 || colIdx == 1 || IsEdit == "0") {
                return false;
            }
            var clientColParam = that.ColParams[that.Columns[colIdx].columns_name_en];

            if (cType == 'listbox' && !$(this).is('select')) {
                var divInput = $(this);

                var lss = [];
                var txt = $(this).find(".lstValue").text();

                //lstValue
                var lstSize = 3;
                if (clientColParam != undefined) {

                    if ("ListDataSql" in clientColParam) {
                        var tmRIdx, rlen;
                        var rows = clientColParam.ListData;
                        lstSize = rows.length + 1;
                        if (lstSize >= 7) lstSize = 7;
                        lss.push("<select multiple='multiple' size='" + lstSize + "' class='select_list'>");
                        lss.push("<option value=''></option>");

                        clientColParam['listbox'] = [];
                        for (tmRIdx = 0, rlen = rows.length; tmRIdx < rlen; tmRIdx++) {

                            //listDataFilter

                            if (rows[tmRIdx][clientColParam.listDataFilter.key] == eval(clientColParam.listDataFilter.value)) {
                                lss.push("<option value='" + rows[tmRIdx].id + "'>" + rows[tmRIdx].id + ':' + rows[tmRIdx].name + "</option>");
                                clientColParam['listbox'].push({ text: rows[tmRIdx].name, value: rows[tmRIdx].id });
                            }
                        }
                        lss.push("</select>");


                    } else if (clientColParam.listboxDataSource != undefined) {//从数据库取得下拉框值

                        if (eval(clientColParam.listboxDataSourceParamChk.value) == false) {
                            that.findParentTag(cellTd, "tr").find('td').get(3).focus();
                            alert(clientColParam.listboxDataSourceParamChk.msg);
                            return false;
                        }

                        var rtv;
                        rtv = that.Ajax("GetListBoxStr", JSON.stringify({ sql: eval(clientColParam.listboxDataSource) }));
                        var rows;
                        if (rtv.result == 'NG') {
                            that.Msg("", "系统错误：" + rtv.msg, function () { }, rtv.msg);
                            return false;
                            //return rtv;
                        } else if (rtv.data.length == 0) {
                            that.Msg("", "InitBody 没有数据，不能加载下一步操作!!!");
                            return false;
                            //return rtv;
                        } else {
                            rows = rtv.data;
                            var tmRIdx, rlen;
                            if (clientColParam != undefined) {
                                if (clientColParam.listbox != undefined) {
                                    lstSize = rows.length + 1;
                                    if (lstSize >= 7) lstSize = 7;
                                    lss.push("<select multiple='multiple' size='" + lstSize + "' class='select_list'>");
                                    lss.push("<option value=''></option>");
                                    for (tmRIdx = 0, rlen = rows.length; tmRIdx < rlen; tmRIdx++) {
                                        lss.push("<option value='" + rows[tmRIdx].id + "'>" + rows[tmRIdx].id + ':' + rows[tmRIdx].name + "</option>");
                                    }
                                    lss.push("</select>");
                                }
                            }
                        }
                    } else {
                        //如果是List 是 参数设置的
                        if (clientColParam != undefined) {
                            if (clientColParam.listbox != undefined) {
                                lstSize = clientColParam.listbox.length + 1;
                                if (lstSize >= 7) lstSize = 7;
                                lss.push("<select multiple='multiple' size='" + lstSize + "' class='select_list'>");
                                var i, len;
                                lss.push("<option value=''></option>");
                                for (i = 0, len = clientColParam.listbox.length; i < len; i++) {
                                    lss.push("<option value='" + clientColParam.listbox[i].value + "'>" + clientColParam.listbox[i].value + ":" + clientColParam.listbox[i].text + "</option>");
                                }
                                lss.push("</select>");
                            }
                        }
                    }
                }

                var ele = $(lss.join(""));

                ele.val(txt);
                //$(this).text("");
                var listbox = $(ele);
                listbox.appendTo(this);
                listbox.focus();

                //listbox.css("margin-top", cellTd.height());
                //listbox.css("margin-left", -(cellTd.width() / 2));

                var left = $(cellTd).offset().left;
                var top = $(cellTd).offset().top;
                listbox.offset({ "left": left, "top": top + $(cellTd).height() });

                if (listbox.width() < 100) {
                    listbox.width(100);
                }

                //cellTd.css("color", "blue");
                //cellTd.css("font-weight", "bold");
                //cellTd.css("border", "3px solid #000");
                //divInput.css("color", "blue");
                //divInput.css("font-weight", "bold");
                //divInput.css("border-bottom", "1px solid red");

                var old_background = divInput.css("background-color");
                divInput.css("background-color", "#c1fffd");



                if (cellTd.attr("oldv") == undefined) {
                    cellTd.attr("oldv", txt);
                }

                //按键按下  上下左右
                listbox.keydown(function (e) {
                    return that.cellKeyDown(e, this);
                });

                var lstOldValue = listbox.val();

                listbox.on("change blur click", function () {
                    $(".tooltip").css("left", -1000);

                    divInput.css("background-color", old_background);

                    if (cellTd.attr("oldv") != $(this).val()) {
                        cellTr.attr("isupdate", "1");
                        if (cellTd.attr("changeColor") == '0') {

                        } else {
                            divInput.css({ "background": "#FFFF00" });
                        }
                    }

                    var valueChange = !that.isEq(lstOldValue, $(this).val());
                    divInput.html(that.ListCellTxt(colIdx, $(this).val(), 'listbox'));


                    if (valueChange) {
                        var tmpCell = that.findNextEnabledCell(cellTd);
                        if (tmpCell != null) {
                            tmpCell.focus();
                        }

                        if (clientColParam.changeAction != undefined) {
                            eval(clientColParam.changeAction);
                        }

                    }

                    listbox = null;

                    //font-weight:normal
                    //divInput.css("color", "#000");
                    //divInput.css("font-weight", "normal");
                    //divInput.css("border-bottom", "1px solid #333");
                });

            } else if (cType == 'cell' && !$(this).is('.input')) {

                var txt = $(this).text();
                var width = $(cellTd).width();
                var height = $(cellTd).height();
                var left = $(cellTd).offset().left;
                var top = $(cellTd).offset().top;
                var divInput = $(this);

                if (cellTd.attr("oldv") == undefined) {
                    cellTd.attr("oldv", txt);
                }
                //divInput.height(cellTd.height());
                var divHeight = divInput.height();
                var divWidth = divInput.width();
                $(this).text("");

                //var ipt = $('<input type="text" value="' + txt + '" class="jqInput" />');

                var ipt = $('<textarea class="jqInput">' + txt + '</textarea>');


                ipt.appendTo(this);
                ipt.height(divHeight - 8);
                //ipt.width(divWidth+20);

                var tmTooltip = that.ColParams[Ens[colIdx]].tooltip;

                if (tmTooltip != null) {
                    $(".tooltip").css("display", "block");
                    $(".tooltip").html(tmTooltip);
                    $(".tooltip").offset({ "left": left - 80, "top": top + ipt.height() + 10 });
                    setTimeout(function () { $(".tooltip").css("left", -1000); }, 5000);
                }

                ipt.attr("maxlength", that.Columns[$(cellTd).index()].columns_length);
                ipt.focus();
                ipt.select();



                ipt.keydown(function (e) {
                    return that.cellKeyDown(e, this);
                });
                //return;
                //文本框输入时
                ipt.bind("input propertychange", function () {
                    if (that.ColParams[Ens[colIdx]].reg != null) {
                        if ($(this).val() != '') {
                            if (!that.ColParams[Ens[colIdx]].reg.test($(this).val())) {
                                $(".tooltip").css("display", "block");
                                $(".tooltip").html(tmTooltip + "<br><br><br>输入规则不符合,内容恢复到变更前");
                                $(".tooltip").offset({ "left": left - 80, "top": top + ipt.height() + 10 });
                                $(this).val(cellTd.attr("oldv"));
                                setTimeout(function () { $(".tooltip").css("left", -1000); }, 5000);
                                event.stopPropagation(); //  阻止事件冒泡
                                return false;
                            } else {
                                $(".tooltip").html(tmTooltip);
                            }
                        }
                        if (!that.ColParams[Ens[colIdx]].reg.test($(this).val())) {
                            return { rtv: false, msg: '输入规则不符合' };
                        }
                    }
                });

                //失去焦点
                ipt.blur(function () {
                    $(".tooltip").css("left", -1000);
                    if (cellTd.attr("oldv") != $(this).val()) {
                        cellTd.attr("oldv", $(this).val());
                        cellTr.attr("isupdate", "1");
                        if (cellTd.attr("changeColor") == '0') {

                        } else {
                            divInput.css({ "background": "#FFFF00" });
                        }
                        if (clientColParam.changeAction != undefined) {
                            eval(clientColParam.changeAction);
                        }

                    }

                    divInput.html($(this).val() || "");
                    ipt = null;
                });

                //粘贴
                ipt.on('paste', function () {
                    var element = this;
                    $("#paste").val("");
                    $("#paste").show();
                    $("#paste").focus();

                    setTimeout(function () {
                        var text = $("#paste").val();
                        var tmpTd = cellTd;
                        var tmpTr = null;

                        var rs = text.split('\n');
                        var i, j, leni, lenj;
                        for (i = 0, leni = rs.length; i < leni; i++) {

                            if (rs[i] != '') {
                                if (tmpTr == null) {
                                    tmpTr = cellTr;
                                } else {
                                    //下一行
                                    if (tmpTr.next().length == 0) {
                                        var tr = tmpTr.clone();
                                        tr.find('.jqBtnNew').remove();
                                        tr.find('.jqBtnSel').remove();
                                        tr.find('.jqBtnRemoveRow').remove();

                                        //初始化 列                                        
                                        for (j = 0, lenj = tr.find('td').length; j < lenj; j++) {
                                            //if (j <= 1) {
                                            //    tmpTd.find('div').text('');
                                            //}
                                            if (tmpTd.attr("cType") == 'listbox') {
                                                tmpTd.find('div').find('a').text('');
                                            } else {
                                                tmpTd.find('div').text('');
                                            }
                                            //下来列表联动 readCookie
                                            if (that.ColParams[Ens[j]].readCookie != '') {
                                                tmpTd = $(tr.children("td").get(j));
                                                if (tmpTd.attr("cType") == 'listbox') {
                                                    tmpTd.find('div').html(that.ListCellTxt(j, $.cookie(that.ColParams[Ens[j]].readCookie), 'listbox'));
                                                } else {
                                                    tmpTd.find('div').text($.cookie(that.ColParams[Ens[j]].readCookie));
                                                }
                                            }
                                        }

                                        tr.find('td').find('div').focus(that.cellInput);
                                        //var removeRow = $("<input type='button' class='jqBtnRemoveRow' value='➖'/>");

                                        if (that.IsView == false) {
                                            var removeRow = $("<a class='jqBtnRemoveRow linkBtn tooltip_btn' title='删除本行'/>删行</a>");
                                            removeRow.appendTo($(tr.find('td').get(1)).find('div'));
                                            removeRow.click(function () {
                                                that.findParentTag($(this), 'tr').remove();
                                            });
                                        }


                                        tr.appendTo($(that.tableTbodyEle));
                                        tmpTr = tr;
                                    } else {
                                        tmpTr = tmpTr.next();
                                    }
                                }

                                var cs = rs[i].split('\t');
                                tmpTd = $(tmpTr.children("td").get(colIdx));
                                for (j = 0, lenj = cs.length; j < lenj; j++) {
                                    tmpTd.attr("oldv", tmpTd.find('div').text());

                                    //如果是可登录行
                                    if (that.ColParams[Ens[tmpTd.index()]].ins_row_enable) {
                                        //设置值
                                        if (tmpTd.attr("cType") == 'listbox') {
                                            tmpTd.find('div').html(that.ListCellTxt(tmpTd.index(), cs[j], 'listbox'));
                                        } else {
                                            tmpTd.find('div').text(cs[j]);
                                        }
                                        //设置背景色
                                        if (tmpTd.attr("oldv") != cs[j]) {
                                            tmpTr.attr("isupdate", "1");
                                            if (tmpTd.attr("changeColor") == '0') {

                                            } else {
                                                tmpTd.css({ "background": "#FFFF00" });
                                            }
                                        }


                                    }
                                    tmpTd = tmpTd.next();
                                }
                            }
                        }
                        $("#paste").val('');
                        $("#paste").hide();
                    }, 100);
                });
            }
        }

        //登录数据
        this.InsData = function () {

            var arrRS = [];
            var chkResult = true;

            that.tableTbodyEle.find("tr[rowType='1']").each(function () {
                var tr = that.findParentTag(this, "tr");
                var colIdx = 0;
                var arr = [];

                tr.find("td").each(function () {
                    if (colIdx >= 2) {
                        var td = $(this);
                        var chkRtv = that.ChkCellValue(td, true, 'ins');
                        if (!chkRtv.rtv) {
                            that.Msg('', chkRtv.msg, function () {
                                td.find('div').focus();
                            });
                            chkResult = false;
                            return false;
                        }
                        arr.push(that.GetTdValue($(this)));
                    }
                    colIdx++;
                });

                if (!chkResult) return false;

                arrRS.push(arr.join(that.ColumnSepartor));
            });

            if (!chkResult) return false;
            var rtv = that.Ajax("InsTableJson", JSON.stringify({ tableName: that.TableEnName, rowValues: arrRS.join(that.RowSepartor), user: that.userCd }));
            if (rtv.result == 'NG') {
                that.Msg("", "系统错误：" + rtv.msg, function () { }, rtv.msg);
                return false;
                //return rtv;
            } else if (rtv.data.length == 0) {
                that.Msg("", "登录出错 0，不能加载下一步操作!!!");
                return false;
                //return rtv;
            } else {
                if (that.InitBody(that.preWhereStr)) {
                    that.SetColumnsWidth(that.Columns);
                }
                alert(rtv.data + "登录成功");
                return true;

            }

            //if (rtv != "OK") {
            //    //alert3("【" + rtv + "】 ", "dbTable.InsTableJson", arguments);
            //    that.Msg("", rtv);
            //    //that.Msg(rtv);
            //    return false;
            //} else {
            //    if (that.InitBody(that.preWhereStr)) {
            //        that.SetColumnsWidth(that.Columns);
            //    }
            //    alert("登录成功");
            //}

        };

        //删除数据
        this.DelData = function () {
            if (!confirm("真的要删除么？")) return false;

            var tr = that.findParentTag(this, "tr");
            var colIdx = 0;
            var arr = [];
            tr.find("td").each(function () {
                if (colIdx >= 2) {
                    arr.push(that.GetTdValue($(this)));
                }
                colIdx++;
            });
            var rtv = that.Ajax("DelTableJson", JSON.stringify({ tableName: that.TableEnName, rowValues: arr.join(that.ColumnSepartor), user: that.userCd }));
            if (rtv.result == 'NG') {
                that.Msg("", "系统错误：" + rtv.msg, function () { }, rtv.msg);
                return false;
                //return rtv;
            } else if (rtv.data.length == 0) {
                that.Msg("", "删除出错 0 ，不能加载下一步操作!!!");
                return false;
                //return rtv;
            } else {
                if (that.InitBody(that.preWhereStr)) {
                    that.SetColumnsWidth(that.Columns);
                }
                alert("删除成功");

            }


            //if (rtv != "OK") {
            //    alert3("【" + rtv + "】 ", "dbTable.UpdTableJson", arguments);
            //    return false;
            //} else {

            //}
        };

        //更新数据
        this.UpdData = function () {
            var arrRS = [];
            //检查
            var chkResult = true;
            var haveUpd = false;
            var colIdx = 0;
            //.attr("rowType")
            that.tableTbodyEle.find(".trMs").each(function () {
                if ($(this).attr("rowType") == "1") {
                    return true;
                } else if ($(this).attr("isupdate") == undefined) {
                    return true;
                } else if ($(this).attr("isupdate") == "1") {
                    var arr = [];
                    haveUpd = true;
                    colIdx = 0;
                    $(this).find("td").each(function () {
                        if (colIdx >= 2) {
                            var td = $(this);

                            var chkRtv = that.ChkCellValue(td, true, 'upd');
                            if (!chkRtv.rtv) {

                                that.Msg('', chkRtv.msg, function () {
                                    td.find('div').focus();

                                });
                                chkResult = false;
                                return false;
                            }

                            //if ($(this).attr("cType") == 'listbox') {
                            //    arr.push($(this).find(".lstValue").text());
                            //} else {

                            //}
                            arr.push(that.GetTdValue($(this)));

                        }
                        colIdx++;

                    });
                    if (!chkResult) {
                        return false;
                    }
                    arrRS.push(arr.join(that.ColumnSepartor));
                }
            });

            if (!haveUpd) {
                alert("没有要更新的数据");
                return false;
            }
            if (!chkResult) {
                return false;
            }

            var rtv = that.Ajax("UpdTableJson", JSON.stringify({ tableName: that.TableEnName, rowValues: arrRS.join(that.RowSepartor), user: that.userCd }));
            if (rtv.result == 'NG') {
                that.Msg("", "系统错误：" + rtv.msg, function () { }, rtv.msg);
                return false;
                //return rtv;
            } else if (rtv.data.length == 0) {
                that.Msg("", "删除出错 0 ，不能加载下一步操作!!!");
                return false;
                //return rtv;
            } else {
                if (confirm(rtv.data + "更新成功， 重新检索吗？")) {
                    if (that.InitBody(that.preWhereStr)) {
                        that.SetColumnsWidth(that.Columns);
                    }
                }
                return true;
            }
            //if (rtv != "OK") {
            //    alert3("【" + rtv + "】 ", "dbTable.UpdTableJson", arguments);
            //    return false;
            //} else {
            //    if (confirm("更新成功， 重新检索吗？")) {
            //        if (that.InitBody(that.preWhereStr)) {
            //            that.SetColumnsWidth(that.Columns);
            //        }
            //    }

            //}
            //return true;
        };

        //获得单元格值 用列idx
        this.GetTdValueByIdx = function (e, idx) {
            var td = that.findParentTag(e, "tr").find('td').get(idx);
            var value = that.GetTdValue(td);
            return value;
        }

        //获得单元格值
        this.GetTdValue = function (e) {
            if ($(e).attr("cType") == 'listbox') {
                return $(e).find(".lstValue").text();
            } else {
                return $(e).text();
            }
        }

        //删除当前页的值
        this.DelThisPageData = function () {
            var arrRS = [];
            //检查
            var chkResult = true;
            var haveUpd = false;
            var colIdx = 0;
            //.attr("rowType")
            that.tableTbodyEle.find("tr").each(function () {
                if ($(this).attr("rowType") == "0") {
                    var arr = [];
                    haveUpd = true;
                    colIdx = 0;
                    $(this).find("td").each(function () {
                        if (colIdx >= 2) {
                            arr.push(that.GetTdValue($(this)));
                        }
                        colIdx++;

                    });
                    if (!chkResult) {
                        return false;
                    }
                    arrRS.push(arr.join(that.ColumnSepartor));
                }
            });

            if (!haveUpd) {
                alert("没有要删除的数据");
                return false;
            }
            if (!chkResult) {
                return false;
            }

            if (that.PageIndex > 1) {
                that.PageIndex = that.PageIndex - 1;
            }

            var rtv = that.Ajax("DelThisPageDataJson", JSON.stringify({ tableName: that.TableEnName, rowValues: arrRS.join(that.RowSepartor), user: that.userCd }));
            if (rtv.result == 'NG') {
                that.Msg("", "系统错误：" + rtv.msg, function () { }, rtv.msg);
                return false;
                //return rtv;
            } else if (rtv.data.length == 0) {
                that.Msg("", "删除出错 0 ，不能加载下一步操作!!!");
                return false;
                //return rtv;
            } else {
                if (confirm("删除成功， 重新检索吗？")) {
                    if (that.InitBody(that.preWhereStr)) {
                        that.SetColumnsWidth(that.Columns);
                    }
                }
                return true;
            }

        };

        //删除当前页的值
        this.DelThisChooseData = function () {
            var arrRS = [];
            //检查
            var chkResult = true;
            var haveUpd = false;
            var colIdx = 0;
            //.attr("rowType")
            that.tableTbodyEle.find("tr").each(function () {
                if ($(this).attr("rowType") == "0") {
                    var arr = [];
                    haveUpd = true;
                    colIdx = 0;
                    //获得选择的行
                    if ($($($(this).find("td")[1]).find('div')[0]).find('.cb').is(':checked')) {
                        $(this).find("td").each(function () {
                            if (colIdx >= 2) {
                                arr.push(that.GetTdValue($(this)));
                            }
                            colIdx++;
                        });
                        if (!chkResult) {
                            return false;
                        }
                        arrRS.push(arr.join(that.ColumnSepartor));
                    }

                }
            });

            if (!haveUpd) {
                alert("没有要删除的数据");
                return false;
            }
            if (!chkResult) {
                return false;
            }

            if (that.PageIndex > 1) {
                that.PageIndex = that.PageIndex - 1;
            }

            var rtv = that.Ajax("DelThisPageDataJson", JSON.stringify({ tableName: that.TableEnName, rowValues: arrRS.join(that.RowSepartor), user: that.userCd }));
            if (rtv.result == 'NG') {
                that.Msg("", "系统错误：" + rtv.msg, function () { }, rtv.msg);
                return false;
                //return rtv;
            } else if (rtv.data.length == 0) {
                that.Msg("", "删除出错 0 ，不能加载下一步操作!!!");
                return false;
                //return rtv;
            } else {
                if (confirm("删除成功， 重新检索吗？")) {
                    if (that.InitBody(that.preWhereStr)) {
                        that.SetColumnsWidth(that.Columns);
                    }
                }
                return true;
            }

        };

        //出力选择数据EXCEL
        this.OutExcelChooseData = function () {

            var rowIdx, colIdx, rlen, len;

            var tbl = $("<table></table>")
            var tr;

            var rs = [];

            rs.push("<tr>")
            for (colIdx = 1, len = that.Columns.length; colIdx < len; colIdx++) {
                rs.push("<td>")
                rs.push(that.Columns[colIdx]["columns_name_cn"]);
                rs.push("</td>")
            }
            rs.push("</tr>")

            var cDx;
            for (rowIdx = 0, rlen = that.Rows.length; rowIdx < rlen; rowIdx++) {

                if ($($(".cb")[rowIdx]).is(':checked')) {
                    //获得选择的行
                    //if ($($($(that.Rows[rowIdx]).find("td")[1]).find('div')[0]).find('.cb').is(':checked')) {
                    rs.push("<tr>")
                    cDx = 0;
                    for (var cell in that.Rows[rowIdx]) {
                        if (cDx > 0) {
                            rs.push("<td>")
                            rs.push(that.Rows[rowIdx][cell]);
                            rs.push("</td>")
                            //if (isNumber(that.Rows[rowIdx][cell])) {
                            //    rs.push("<td>")
                            //    rs.push(that.Rows[rowIdx][cell]);
                            //    rs.push("</td>")
                            //} else {
                            //    rs.push("<td>=\"")
                            //    rs.push(that.Rows[rowIdx][cell]);
                            //    rs.push("\"</td>")
                            //}

                        }
                        cDx = cDx + 1;
                    }
                    rs.push("</tr>")
                }
            }


            tbl[0].innerHTML = rs.join("");


            //表格添加到EXCEL
            tbl.table2excel({
                exclude: ".noExl",
                name: "Excel Document Name",
                filename: TableCnName + "_" + new Date().toISOString().replace(/[\-\:\.]/g, "") + ".xls",
                fileext: ".xls",
                exclude_img: true,
                exclude_links: true,
                exclude_inputs: true,
                preserveColors: true
            });
        };

        //出力本页EXCEL
        this.OutExcelThisPage = function () {

            var rowIdx, colIdx, rlen, len;

            var tbl = $("<table></table>")
            var tr;

            var rs = [];

            rs.push("<tr>")
            for (colIdx = 1, len = that.Columns.length; colIdx < len; colIdx++) {
                rs.push("<td>")
                rs.push(that.Columns[colIdx]["columns_name_cn"]);
                rs.push("</td>")
            }
            rs.push("</tr>")

            var cDx;
            for (rowIdx = 0, rlen = that.Rows.length; rowIdx < rlen; rowIdx++) {
                rs.push("<tr>")
                cDx = 0;
                for (var cell in that.Rows[rowIdx]) {
                    if (cDx > 0) {
                        rs.push("<td>")
                        rs.push(that.Rows[rowIdx][cell]);
                        rs.push("</td>")
                        //if (isNumber(that.Rows[rowIdx][cell])) {
                        //    rs.push("<td>")
                        //    rs.push(that.Rows[rowIdx][cell]);
                        //    rs.push("</td>")
                        //} else {
                        //    rs.push("<td>=\"")
                        //    rs.push(that.Rows[rowIdx][cell]);
                        //    rs.push("\"</td>")
                        //}
                    }
                    cDx = cDx + 1;
                }
                rs.push("</tr>")
            }


            tbl[0].innerHTML = rs.join("");


            //表格添加到EXCEL
            tbl.table2excel({
                exclude: ".noExl",
                name: "Excel Document Name",
                filename: TableCnName + "_" + new Date().toISOString().replace(/[\-\:\.]/g, "") + ".xls",
                fileext: ".xls",
                exclude_img: true,
                exclude_links: true,
                exclude_inputs: true,
                preserveColors: true
            });
        };


        //出力本页EXCEL
        this.OutExcelAll = function () {

            var rowIdx, colIdx, rlen, len;

            var tbl = $("<table></table>")
            var tr;
            var rtv;
            var rs = [];

            rs.push("<tr>")
            for (colIdx = 1, len = that.Columns.length; colIdx < len; colIdx++) {
                rs.push("<td>")
                rs.push(that.Columns[colIdx]["columns_name_cn"]);
                rs.push("</td>")
            }
            rs.push("</tr>")

            var whereStr = that.GetSelWhere();
            rtv = that.InitTableData2(that.TableEnName, -1, that.OnePageRowCount, whereStr)


            if (rtv.data.MS.length > 20000) {
                alert("数据大于2万，不能出力。");
                return false;
            }

            var cDx;
            for (rowIdx = 0, rlen = rtv.data.MS.length; rowIdx < rlen; rowIdx++) {
                rs.push("<tr>")
                cDx = 0;
                for (var cell in rtv.data.MS[rowIdx]) {
                    if (cDx > 0) {
                        rs.push("<td>=\"")
                        rs.push(rtv.data.MS[rowIdx][cell]);
                        rs.push("\"</td>")
                        //if (isNumber(rtv.data.MS[rowIdx][cell])) {
                        //    rs.push("<td>")
                        //    rs.push(rtv.data.MS[rowIdx][cell]);
                        //    rs.push("</td>")
                        //} else {
                        //    rs.push("<td>=\"")
                        //    rs.push(rtv.data.MS[rowIdx][cell]);
                        //    rs.push("\"</td>")
                        //}

                    }
                    cDx = cDx + 1;
                }
                rs.push("</tr>")
            }


            tbl[0].innerHTML = rs.join("");


            //表格添加到EXCEL
            tbl.table2excel({
                exclude: ".noExl",
                name: "Excel Document Name",
                filename: TableCnName + "_" + new Date().toISOString().replace(/[\-\:\.]/g, "") + ".xls",
                fileext: ".xls",
                exclude_img: true,
                exclude_links: true,
                exclude_inputs: true,
                preserveColors: true
            });
        };



        //获得TD的 HTML
        this.GetTdEleByColumn = function (colIdx, name, rType) {
            var cType = ""; //判断是否只读
            var cClass = ""; //设置TD样式
            if (colIdx == 0 || colIdx == 1) { //如果第一列
                cType = "txt";
            } else if (that.Columns[colIdx].pk == "P") { //如果是主键
                cType = "cell";
                cClass = "key_cell disabled_cell";
            } else if (!that.IsEditCell(colIdx, "0")) {
                //cType = "cell";
                cClass = "disabled_cell";
            } else {
                cType = "cell";
            }

            var width = 0;
            var clientColParam = that.ColParams[Ens[colIdx]];
            if (clientColParam != undefined) {
                if (clientColParam.cType != undefined) {
                    cType = clientColParam.cType;
                }
                if (clientColParam.width != undefined) {
                    width = clientColParam.width;
                }
            }

            var arrTd = [];
            arrTd.push("<td");
            arrTd.push(" cType=\"" + cType + "\"");
            arrTd.push(" class=\"c" + colIdx + " " + cClass + "\"");
            //if (width > 0) {
            //    arrTd.push(" style=\"width:" + width + "px \"");
            //}

            //arrTd.push(" tabindex=\"-1\"");
            arrTd.push(">");
            arrTd.push("<div class=''");
            //arrTd.push(" tabindex=\"-1\"");
            arrTd.push(">");
            arrTd.push(that.Columns[colIdx][name]);
            arrTd.push("</div>");
            arrTd.push("</td>");

            var ele = $(arrTd.join(""));
            if (!that.ColParams[Ens[colIdx]].visible) {
                ele.hide();
            }
            return ele;
        };

        //获得 检索行的TD的 HTML
        this.GetTdEleBySelColumn = function (colIdx, name, rType) {
            var cType = ""; //判断是否只读
            var cClass = ""; //设置TD样式


            var IsEdit = "0";
            if (colIdx == 0 || colIdx == 1) { //如果第一列
                IsEdit = "0";
            } else {
                //IsEdit = "1";

                if (rType == 'h' && that.ColParams[Ens[colIdx]].sel_row_enable == false) {//标题
                    IsEdit = "0";
                } else if (rType == '0' && that.ColParams[Ens[colIdx]].ms_row_enable == false) {//明显行
                    IsEdit = "0";
                } else if (rType == '1' && that.ColParams[Ens[colIdx]].ins_row_enable == false) {//登录行
                    IsEdit = "0";
                } else {
                    IsEdit = "1";
                }
            }



            if (colIdx == 0 || colIdx == 1) { //如果第一列
                cType = "txt";
            } else if (that.Columns[colIdx].pk == "P") { //如果是主键
                cType = "cell";
                cClass = "key_cell";
            } else if (that.DateColsName.indexOf(Ens[colIdx]) >= 0) {
                cType = "two_date";
                //cClass = "two_date";
            } else if (!that.IsEditCell(colIdx, "0")) {
                cType = "cell";
                cClass = "";
            } else {
                cType = "cell";
            }


            var clientColParam = that.ColParams[Ens[colIdx]];
            if (clientColParam != undefined) {
                if (clientColParam.cType != undefined) {
                    cType = clientColParam.cType;
                }
            }

            var arrTd = [];
            arrTd.push("<td");
            arrTd.push(" cType=\"" + cType + "\"");
            arrTd.push(" class=\"c" + colIdx + " " + cClass + "\"");
            //arrTd.push(" tabindex=\"-1\"");
            arrTd.push(" IsEdit=\"" + IsEdit + "\"");
            arrTd.push(" changeColor=\"0\"");
            arrTd.push(">");

            if (colIdx == 0) {

                //arrTd.push(" tabindex=\"-1\"");
                //arrTd.push(">");
                arrTd.push("<div class=''>");
                //arrTd.push("<input type='button' class='jqBtnClear linkBtn tooltip_btn' value='清' title='清空本行按钮'/>");
                //如果是视图
                //if (that.IsView == false) {
                //}
                arrTd.push("<a class='jqBtnClear linkBtn tooltip_btn' title='清空本行按钮'/>清除</a>");



                arrTd.push("</div>");
            } else if (colIdx == 1) {
                //arrTd.push(" tabindex=\"-1\"");
                //arrTd.push(">");
                arrTd.push("<div class=''>");
                //arrTd.push("<input type='button' class='jqBtnSel linkBtn tooltip_btn' value='检' title='检索按钮' />");
                arrTd.push("<a class='jqBtnSel linkBtn tooltip_btn' title='检索按钮'/>检索</a>");

                arrTd.push("<input type=\"checkbox\" class='cbAll'>");
                arrTd.push("</div>");
            } else if (that.DateColsName.indexOf(Ens[colIdx]) >= 0) {
                arrTd.push("<div class=''>");
                arrTd.push("<a class='towDateContainer'>");

                var timeTypeCls = "";
                if (Ens[colIdx].indexOf("时间") >= 0 || Ens[colIdx] == "chk_start_date" || Ens[colIdx] == "chk_end_date") {
                    //timeTypeCls = "jqTxtDateTime";
                    timeTypeCls = "jqTxtDate";
                } else {
                    timeTypeCls = "jqTxtDate";
                }

                arrTd.push("<input type='text' class='" + timeTypeCls + " tooltip_btn " + Ens[colIdx] + "_start' placeholder='开始时间' title='开始时间: yyyy-mm-dd（可以只输入一个）' />");

                if (that.IsView == false) {
                    arrTd.push("");
                } else {
                    arrTd.push("<br>");
                }

                arrTd.push("<input type='text' class='" + timeTypeCls + " tooltip_btn " + Ens[colIdx] + "_end'  placeholder='终了时间' title='终了时间: yyyy-mm-dd（可以只输入一个）'/>");

                arrTd.push("</a>");
                arrTd.push("</div>");
            } else {
                arrTd.push("<div tabindex=\"-1\">");
                arrTd.push("</div>");
            }
            arrTd.push("</td>");

            var ele = $(arrTd.join(""));
            if (!that.ColParams[Ens[colIdx]].visible) {
                ele.hide();
            }
            if (that.ColParams[Ens[colIdx]].readCookie != '') {
                if (cType == 'listbox') {
                    ele.html(that.ListCellTxt(colIdx, $.cookie(that.ColParams[Ens[colIdx]].readCookie), 'listbox'));
                } else {
                    ele.text($.cookie(that.ColParams[Ens[colIdx]].readCookie));
                }
            }



            return ele;

            //return $(arrTd.join(""));
        };
        //获得TD 用 行
        this.GetTdEleByRow = function (colIdx, rowIdx, name, rType) {

            var cType = "";     //判断是否只读
            var cClass = "";    //设置TD样式
            var IsEdit = "0";

            var IsEdit = "0";
            if (colIdx == 0 || colIdx == 1) { //如果第一列
                IsEdit = "0";
            } else {
                //IsEdit = "1";
                if (rType == 'h' && that.ColParams[Ens[colIdx]].sel_row_enable == false) {
                    IsEdit = "0";
                } else if (rType == '0' && that.ColParams[Ens[colIdx]].ms_row_enable == false) {
                    IsEdit = "0";
                } else if (rType == '1' && that.ColParams[Ens[colIdx]].ins_row_enable == false) {
                    IsEdit = "0";
                } else {
                    IsEdit = "1";
                }
            }

            //是不是可编辑单元格
            //if (that.IsEditCell(colIdx, rowType)) {
            //    IsEdit = "1";
            //} else {
            //    IsEdit = "0";
            //}

            if (colIdx == 0) {
                cType = "button";
            } else if (colIdx == 1) {
                cType = "cell";
            } else if (that.Columns[colIdx].pk == "P") {      //如果是主键
                cType = "cell";
                if (rType == "0") {
                    cClass = "key_cell ";
                } else {
                    cClass = "key_cell_ins";
                }
                if (IsEdit == 0) {
                    cClass += " disabled_cell";
                }
            } else if (IsEdit == "0") {
                cType = "cell";
                cClass = "disabled_cell";
            } else {
                cType = "cell";
            }

            var clientColParam = that.ColParams[Ens[colIdx]];
            if (clientColParam != undefined) {
                if (clientColParam.cType != undefined) {
                    cType = clientColParam.cType;
                }
            }

            var arrTd = [];
            arrTd.push("<td");
            arrTd.push(" IsEdit=\"" + IsEdit + "\"");
            arrTd.push(" cType=\"" + cType + "\"");
            arrTd.push(" class=\"c" + colIdx + " " + cClass + "\"");

            arrTd.push(">");
            arrTd.push("<div");
            if (IsEdit == "1") {
                arrTd.push(" tabindex=\"-1\"");
            }
            arrTd.push(">");
            if (rType == "0") {
                if (colIdx == 0) {

                    if (that.IsView == false) {
                        //❌
                        //arrTd.push("<input type='button' class='jqBtnDel linkBtn tooltip_btn' title='删除本行数据' value='×'/>");
                        arrTd.push("<a class='jqBtnDel linkBtn tooltip_btn' title='行删除'/>删除</a>");
                    }

                } else {
                    var dbValue = that.Rows[rowIdx][name];
                    if (colIdx == 1) {
                        //var clientColParamNo = that.ColParams['No'];

                        //NO 列
                        if (that.ColParams[Ens[colIdx]].url != null) {
                            arrTd.push("<a");
                            arrTd.push(" href='" + that.ColParams[Ens[colIdx]].url + "'");
                            if (that.ColParams[Ens[colIdx]].click != null) {
                                arrTd.push(" onclick='" + that.ColParams[Ens[colIdx]].click + "'");
                            }
                            arrTd.push(" class='no'");
                            arrTd.push(" >");
                            arrTd.push(dbValue);
                            arrTd.push("</a>");
                        } else {
                            arrTd.push(dbValue);
                        }

                        arrTd.push("<input type=\"checkbox\" class='cb'>");

                    } else {
                        if (cType == 'listbox') {
                            arrTd.push(that.ListCellTxt(colIdx, dbValue, cType));
                        } else {
                            arrTd.push(dbValue);
                        }

                    }
                }
            } else {
                if (colIdx == 0) {
                    //arrTd.push("<input type='button' class='jqBtnNew tooltip_btn' value='✔️' title='登录所有追加行数据'/>");
                    arrTd.push("<a class='jqBtnNew linkBtn tooltip_btn' title='登录'/>登录</a>");
                } else if (colIdx == 1) {

                }
            }
            arrTd.push("</div>");
            arrTd.push("</td>");

            var ele = $(arrTd.join(""));
            if (!that.ColParams[Ens[colIdx]].visible) {
                ele.hide();
            }

            //readCookie 后一个下拉框 与前面值联动
            if (that.ColParams[Ens[colIdx]].readCookie != '') {
                if (cType == 'listbox') {
                    ele.html(that.ListCellTxt(colIdx, $.cookie(that.ColParams[Ens[colIdx]].readCookie), 'listbox'));
                } else {
                    ele.text($.cookie(that.ColParams[Ens[colIdx]].readCookie));
                }
            }

            return ele;

            //return $(arrTd.join(""));
        };


        //获得TD 用 行
        this.GetTdEleByRowView = function (colIdx, rowIdx, name, rType) {
            var cType = "";     //判断是否只读
            var cClass = "";    //设置TD样式
            var IsEdit = "0";
            var IsEdit = "0";
            if (colIdx == 0 || colIdx == 1) { //如果第一列
                IsEdit = "0";
            } else {
                if (rType == 'h' && that.ColParams[Ens[colIdx]].sel_row_enable == false) {
                    IsEdit = "0";
                } else if (rType == '0' && that.ColParams[Ens[colIdx]].ms_row_enable == false) {
                    IsEdit = "0";
                } else if (rType == '1' && that.ColParams[Ens[colIdx]].ins_row_enable == false) {
                    IsEdit = "0";
                } else {
                    IsEdit = "1";
                }
            }

            if (colIdx == 0) {
                cType = "button";
            } else if (colIdx == 1) {
                cType = "cell";
            } else if (that.Columns[colIdx].pk == "P") {      //如果是主键
                cType = "cell";
                if (rType == "0") {
                    cClass = "key_cell ";
                } else {
                    cClass = "key_cell_ins";
                }
                if (IsEdit == 0) {
                    cClass += " disabled_cell";
                }
            } else if (IsEdit == "0") {
                cType = "cell";
                cClass = "disabled_cell";
            } else {
                cType = "cell";
            }

            var clientColParam = that.ColParams[Ens[colIdx]];
            if (clientColParam != undefined) {
                if (clientColParam.cType != undefined) {
                    cType = clientColParam.cType;
                }
            }

            var arrTd = [];
            arrTd.push("<td");
            arrTd.push(" IsEdit=\"" + IsEdit + "\"");
            arrTd.push(" cType=\"" + cType + "\"");
            arrTd.push(" class=\"c" + colIdx + " " + cClass + "\"");
            arrTd.push(">");
            arrTd.push("<div");
            if (IsEdit == "1") {
                arrTd.push(" tabindex=\"-1\"");
            }
            arrTd.push(">");
            if (rType == "0") {
                if (colIdx == 0) {
                    if (that.IsView == false) {
                        //❌
                        //arrTd.push("<input type='button' class='jqBtnDel linkBtn tooltip_btn' title='删除本行数据' value='×'/>");
                        arrTd.push("<a class='jqBtnDel linkBtn tooltip_btn' title='行删除'/>删除</a>");
                    }

                } else {
                    var dbValue = that.Rows[rowIdx][name];
                    if (colIdx == 1) {
                        //NO 列
                        if (that.ColParams[Ens[colIdx]].url != null) {
                            arrTd.push("<a");
                            arrTd.push(" href='" + that.ColParams[Ens[colIdx]].url + "'");
                            if (that.ColParams[Ens[colIdx]].click != null) {
                                arrTd.push(" onclick='" + that.ColParams[Ens[colIdx]].click + "'");
                            }
                            arrTd.push(" class='no'");
                            arrTd.push(" >");
                            arrTd.push(dbValue);
                            arrTd.push("</a>");
                        } else {
                            arrTd.push(dbValue);
                        }
                        arrTd.push("<input type=\"checkbox\" class='cb'>");
                    } else if (Ens[colIdx] == '图片') {
                        if (dbValue == null) {

                        } else {
                            var pics = dbValue.split('|');
                            var ii;
                            for (ii = 0; ii <= pics.length - 1; ii++) {
                                if (pics[ii] != '') {
                                    arrTd.push("<a");
                                    arrTd.push(" href='#'");
                                    arrTd.push(" onclick=\"OpenImgPage('" + pics[ii] + "')\">");
                                    arrTd.push(pics[ii]);
                                    arrTd.push("</a>");
                                }

                            }
                        }
                    } else if (Ens[colIdx] == '图片M') {
                        if (dbValue == null) {

                        } else {
                            var pics = dbValue.split('|');
                            if (pics.length == 2) {
                                arrTd.push("<a");
                                arrTd.push(" href='#'");
                                arrTd.push(" onclick=\"OpenImgPage2('" + pics[0] + "','" + pics[1] + "')\">");
                                arrTd.push(pics[0]);
                                arrTd.push("</a>");
                            }

                        }
                    } else {
                        if (cType == 'listbox') {
                            arrTd.push(that.ListCellTxt(colIdx, dbValue, cType));
                        } else {
                            arrTd.push(dbValue);
                        }

                    }
                }
            } else {
                if (colIdx == 0) {
                    arrTd.push("<a class='jqBtnNew linkBtn tooltip_btn' title='登录'/>登录</a>");
                } else if (colIdx == 1) {

                }
            }
            arrTd.push("</div>");
            arrTd.push("</td>");

            var ele = $(arrTd.join(""));
            if (!that.ColParams[Ens[colIdx]].visible) {
                ele.hide();
            }

            //readCookie 后一个下拉框 与前面值联动
            if (that.ColParams[Ens[colIdx]].readCookie != '') {
                if (cType == 'listbox') {
                    ele.html(that.ListCellTxt(colIdx, $.cookie(that.ColParams[Ens[colIdx]].readCookie), 'listbox'));
                } else {
                    ele.text($.cookie(that.ColParams[Ens[colIdx]].readCookie));
                }
            }

            return ele;
        };



        this.ListCellTxt = function (colIdx, dbValue, cType) {
            if (dbValue == null) return "";
            if (dbValue.constructor === Array) {
                dbValue = dbValue[0];
            }

            //if (dbValue == "") return "";
            if (cType == 'listbox') dbValue = dbValue.split(":")[0];
            //return "";
            var arrTd = [];
            arrTd.push("<a class='lstValue'>" + dbValue + "</a>");
            if (cType == 'listbox') {

                //dbValue = dbValue.split(":")[0];  
                var clientColParam = that.ColParams[Ens[colIdx]];
                if (clientColParam != undefined) {
                    if (clientColParam.listbox != undefined) {
                        var i, len;
                        arrTd.push("<a class='lstName'>");

                        if (dbValue != "") arrTd.push(":");

                        for (i = 0, len = clientColParam.listbox.length; i < len; i++) {
                            if (dbValue == clientColParam.listbox[i].value) {
                                arrTd.push(clientColParam.listbox[i].text);
                            }
                        }
                        arrTd.push("</a>");
                    }
                }
            }
            return arrTd.join("");
        }

        //清除单元格值
        this.ClearCell = function (e, colIdx) {
            var td = $(that.findParentTag(e, "tr").find('td').get(colIdx));
            if (that.DateColsName.indexOf(Ens[colIdx]) >= 0) {
                td.find("input").val('');
            } else if (that.ColParams[Ens[colIdx]].ins_row_enable) {

                if (that.ColParams[Ens[colIdx]].cType == 'listbox') {
                    td.find('div').html(that.ListCellTxt(colIdx, '', 'listbox'));
                } else {
                    td.find('div').text('');
                }


            }
        };


        //加载标题
        this.InitHeader = function () {
            var colIdx = 0;
            var len;
            if (!that.InitTableInfo(that.TableEnName)) return false;

            //清空头部与数据部
            that.tableTheaderEle.html("");

            //【汉字名】行 Header
            var tableHeaderTrCnEle = $("<tr class='trHeader'></tr>").appendTo(that.tableTheaderEle);
            for (colIdx = 0, len = that.Columns.length; colIdx < len; colIdx++) {
                that.GetTdEleByColumn(colIdx, "columns_name_cn", '-1')
                    .appendTo(tableHeaderTrCnEle);
            }
            //【英文名】行 Header
            var tableHeaderTrEnEle = $("<tr class='trHeader'></tr>").appendTo(that.tableTheaderEle);
            for (colIdx = 0, len = that.Columns.length; colIdx < len; colIdx++) {
                that.GetTdEleByColumn(colIdx, "columns_name_en", '-1')
                    .appendTo(tableHeaderTrEnEle);
            }
            //【检索行】行 Header
            var tableHeaderTrEnEle = $("<tr rowType='h' class='tr_sel trHeader' title=''></tr>").appendTo(that.tableTheaderEle);
            for (colIdx = 0, len = that.Columns.length; colIdx < len; colIdx++) {
                //if (colIdx <= 1 || colIdx > that.Columns.length - 4) {
                //if (colIdx <= 1 || that.DateColsName.indexOf(Ens[colIdx]) >= 0) {
                if (colIdx <= 1) {
                    that.GetTdEleBySelColumn(colIdx, "columns_name_en", 'h')
                        .appendTo(tableHeaderTrEnEle);
                } else {
                    var tds = that.GetTdEleBySelColumn(colIdx, "columns_name_en", 'h');
                    tds.appendTo(tableHeaderTrEnEle);
                    tds.attr('title', '检索条件行：如果输入【%】或【_】或【*】,那么进行模糊匹配(%:1个多个任意字符  _:一个任意字符 *:只要含有)');
                    tds.addClass('tooltip_btn');
                    tds.find('div').focus(that.cellInput);
                    //tds.find('div').focus(function () { alert();});
                }
            }

            //全选
            $(".cbAll").click(function () {

                $(".cb").prop("checked", $(this).is(':checked'));

                //prop("checked", false);
                //if ($(this).is(':checked')) {
                //    $(".cb").attr("checked", "checked");
                //} else {

                //}

            });

            //检索 按钮
            $(".jqBtnSel").click(function () {

                show_query_hint('query_hint');

                setTimeout(function () {
                    var whereStr = that.GetSelWhere();
                    var oldPageIndex = that.PageIndex;
                    that.PageIndex = 1;
                    that.InitSelMs = true;

                    var scTop = that.divBodyEle.scrollTop();
                    var scLeft = that.divBodyEle.scrollLeft();
                    if (that.InitBody(whereStr)) {
                        that.SetColumnsWidth(that.Columns);
                        that.preWhereStr = whereStr;
                    } else {
                        that.PageIndex = oldPageIndex;
                    }
                    that.divBodyEle.scrollTop(scTop);
                    that.divBodyEle.scrollLeft(scLeft);

                    try {
                        changeColor(that);
                    } catch (e1) {

                    }

                    queryHintCallback('query_hint');

                }, 200);
            });

            $.datepicker.regional['zh-CN'] = {
                clearText: '清除',
                clearStatus: '清除已选日期',
                closeText: '关闭',
                closeStatus: '不改变当前选择',
                prevText: '<上月',
                prevStatus: '显示上月',
                prevBigText: '<<',
                prevBigStatus: '显示上一年',
                nextText: '下月>',
                nextStatus: '显示下月',
                nextBigText: '>>',
                nextBigStatus: '显示下一年',
                currentText: '今天',
                currentStatus: '显示本月',
                monthNames: ['一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月'],
                monthNamesShort: ['一', '二', '三', '四', '五', '六', '七', '八', '九', '十', '十一', '十二'],
                monthStatus: '选择月份',
                yearStatus: '选择年份',
                weekHeader: '周',
                weekStatus: '年内周次',
                dayNames: ['星期日', '星期一', '星期二', '星期三', '星期四', '星期五', '星期六'],
                dayNamesShort: ['周日', '周一', '周二', '周三', '周四', '周五', '周六'],
                dayNamesMin: ['日', '一', '二', '三', '四', '五', '六'],
                dayStatus: '设置 DD 为一周起始',
                dateStatus: '选择 m月 d日, DD',
                dateFormat: 'yy-mm-dd',
                initStatus: '请选择日期',

            };
            $.datepicker.setDefaults($.datepicker.regional['zh-CN']);

            // tableHeaderTrEnEle.find(".jqTxtDate").datepicker({
            tableHeaderTrEnEle.find(".jqTxtDate").datepicker({
                //dateFormat: "yy-mm-dd hh:mm:ss"
                //dateFormat: "yy-mm-dd",
                //showSecond: true,
                //timeFormat: 'hh:mm:ss',
                //stepHour: 1,
                //stepMinute: 1,
                //stepSecond: 1

                //    dayNamesMin:dayNamesMin,
                //monthNamesShort:monthNamesShort,
                dateFormat: "yy-mm-dd",
                //changeMonth: true,
                //changeYear: true,
                //showSecond: true,
                //timeFormat: "hh:mm:ss",
                //stepHour: 1,
                //stepMinute: 1,
                //stepSecond: 1


            });


            ////$(".ui_timepicker").datetimepicker({
            tableHeaderTrEnEle.find(".jqTxtDateTime").datetimepicker({
                defaultDate: '',
                dateFormat: "yy-mm-dd",
                showSecond: true,
                timeFormat: 'HH:mm:ss',
                stepHour: 1,
                stepMinute: 1,
                stepSecond: 1
            })



            tableHeaderTrEnEle.find(".jqTxtDate").blur(function () {

                //var v;
                //var Y;
                //var M;
                //var D;
                //Y = "";
                //M = "";
                //D = "";
                //v = $(this).val();
                //if (v == '') {
                //    $(this).css({ "background": "#fff" });
                //    return true;
                //}

                //if (v.split("/").length == 3) {
                //    Y = v.split("-")[0];
                //    M = v.split("-")[1];
                //    D = v.split("-")[2];
                //} else {
                //    v = v.replace(/-/g, '');
                //    if (v.length == 6) {  //6桁の場合
                //        if (v.substring(0, 2) > 70) {
                //            v = "19" + v;
                //        } else {
                //            v = "20" + v;
                //        }

                //    } else if (v.length == 4) {  //4桁の場合
                //        var dd = new Date();
                //        v = dd.getFullYear() + v;

                //    }

                //    if (v.length == 8) {
                //        Y = v.substring(0, 4);
                //        M = v.substring(4, 6);
                //        D = v.substring(6, 8);
                //    }
                //}

                //if (Y.length == 2 && Y.substring(0, 2) > 70) {
                //    Y = "19" + Y;
                //}

                //if (Y.length == 2 && Y.substring(0, 2) <= 70) {
                //    Y = "20" + Y;
                //}

                //if (Y == 'undefined' || Y == undefined || M == 'undefined' || M == undefined || D == 'undefined' || D == undefined || M.length > 2 || D.length > 2 || Y.length == 3 || Y < "1753") {
                //    //$(this).css({ "background": "red" });
                //    alert("日期格式不正确 ， 被清空。");
                //    $(this).val("");
                //    $(this).focus();
                //    return false;
                //}

                //if (M.length == 1) { M = "0" + M; }
                //if (D.length == 1) {
                //    D = "0" + D
                //}
                //var di = new Date(Y, M - 1, D);
                //if (di.getFullYear() == Y && di.getMonth() == M - 1 && di.getDate() == D) {
                //    $(this).val(Y + "-" + M + "-" + D);
                //    return true;
                //} else {
                //    alert("日期格式不正确 ， 被清空。");
                //    $(this).val("");
                //    $(this).focus();
                //    return false;
                //}

            });

            //清空 按钮
            $(".jqBtnClear").click(function () {
                var colIdx = 0;
                $(".tr_sel").find("td").each(function () {
                    if (colIdx <= 1) {
                    } else if (that.DateColsName.indexOf(Ens[colIdx]) >= 0) {
                        $(this).find("input").val('');
                    } else {
                        if (that.ColParams[Ens[colIdx]].ins_row_enable) {
                            $(this).find('div').text('');
                        }
                    }

                    colIdx++;
                });
            });
            return true;
        };
        //加载明细部
        this.InitBody = function (rowValues) {
            var colIdx = 0;
            var rowIdx = 0;
            var tabindex = 1;
            var len;
            var i;
            //装载表信息
            if (!that.InitTableInfo(that.TableEnName)) return false;
            if (!that.InitSelMs) return false;
            var rtv = that.InitTableData(that.TableEnName, that.PageIndex, that.OnePageRowCount, rowValues);

            if (rtv.result == 'NG') {//如果数据返回NG
                that.Msg("", "系统错误：" + rtv.msg, function () { }, rtv.msg);
                return false;
            } else if (rtv.data.length == 0) {//如果没有数据
                that.Msg("", "InitBody 没有数据，不能加载下一步操作!!!");
                return false;
            } else {//其他
                that.Rows = rtv.data.MS;
                that.AllDataRowCount = rtv.data.CNT[0].cnt;
            }

            //清理 明细 页码
            $(that.JqstrPageNumPanel).html("");
            $(".trMs").remove();

            //表格
            that.tableEle["JQ"] = that;

            var rlen;
            for (rowIdx = 0, rlen = that.Rows.length; rowIdx < rlen; rowIdx++) {
                var tableMsTrEnEle = $("<tr class='trMs' rowType='0'></tr>").appendTo(that.tableTbodyEle);
                colIdx = 0;
                var tdHeight;
                for (var cell in that.Rows[rowIdx]) {
                    //0:普通行
                    var tdEle;
                    if (that.IsView) {
                        tdEle = that.GetTdEleByRowView(colIdx, rowIdx, cell, "0");
                    } else {
                        tdEle = that.GetTdEleByRow(colIdx, rowIdx, cell, "0");
                    }

                    tdEle.appendTo(tableMsTrEnEle).find('div').focus(that.cellInput);
                    if (colIdx == 0) tdEle.find(".jqBtnDel").click(that.DelData);
                    colIdx++;
                }


                if (rowIdx == rlen - 1) {
                    if (TableEnName == "v_A05_check_scx_one") {
                        tableMsTrEnEle.find('td').css("background-color", "yellow");
                    }
                }
            }

            //如果不是视图
            if (that.IsView == false) {
                //登录行 jqBtnNew
                var tableHeaderTrEditorEle = $("<tr class='trMs' rowType='1'></tr>").appendTo(that.tableTbodyEle);
                //$("<th>No</th>").appendTo(tableHeaderTrEnEle);
                for (colIdx = 0, len = that.Columns.length; colIdx < len; colIdx++) {
                    var tdEle = that.GetTdEleByRow(colIdx, rowIdx, cell, "1");
                    tdEle.appendTo(tableHeaderTrEditorEle).find('div').focus(that.cellInput);
                    if (colIdx == 0) tdEle.find(".jqBtnNew").click(that.InsData);
                }

                var removeAllRow = $("<a class='jqBtnDel linkBtn tooltip_btn' title='删除'/>清除↘</a>");
                removeAllRow.appendTo($(tableHeaderTrEditorEle.find('td').get(1)).find('div'));
                removeAllRow.click(function () {
                    tableHeaderTrEditorEle.find("td").each(function () {
                        if ($(this).index() >= 2) {
                            $(this).find('div').text('');
                        }
                        colIdx++;
                    });
                    while (tableHeaderTrEditorEle.next().length > 0) {
                        tableHeaderTrEditorEle.next().remove();
                    }
                });


                $('.jqBtnUpd').unbind("click");
                $(".jqBtnUpd").click(that.UpdData);

                //删除本页
                $('.jqBtnDelThisPage').unbind("click");
                $(".jqBtnDelThisPage").click(that.DelThisPageData);

                $('.jqBtnDelChoose').unbind("click");
                $(".jqBtnDelChoose").click(that.DelThisChooseData);
            }

            //出力本页数据
            $("#btnSaveExcelThisPage").unbind("click");
            $("#btnSaveExcelThisPage").click(that.OutExcelThisPage);

            //出力选择数据
            $("#btnSaveExcelChooseData").unbind("click");
            $("#btnSaveExcelChooseData").click(that.OutExcelChooseData);

            //出力所有数据
            $("#btnSaveExcelAll").unbind("click");
            $("#btnSaveExcelAll").click(that.OutExcelAll);

            var allPageCount = Math.ceil(that.AllDataRowCount / that.OnePageRowCount);

            for (i = 1; i <= allPageCount; i++) {
                if (i == that.PageIndex - 8) {
                    $("<a>...</a>").appendTo($(that.JqstrPageNumPanel));
                }

                if ((i >= that.PageIndex - 8 && i <= that.PageIndex + 8) || i == 1 || i == allPageCount) {

                    if (that.PageIndex == i) {
                        $("<b class='choosePage'>" + i + "</b>").appendTo($(that.JqstrPageNumPanel));
                    } else {
                        $("<a href='#'>" + i + "</a>").appendTo($(that.JqstrPageNumPanel)).click(function () {
                            show_query_hint('query_hint');

                            that.PageIndex = parseInt($(this).text());
                            setTimeout(function () {
                                if (that.InitBody(that.preWhereStr)) {
                                    that.SetColumnsWidth(that.Columns);
                                }
                                queryHintCallback('query_hint');
                            }, 100);

                        });
                    }

                }
                if (i == that.PageIndex + 8) {
                    $("<a>...</a>").appendTo($(that.JqstrPageNumPanel));
                }
                //if (i % 30 == 0) {
                //    $("<br>").appendTo($(that.JqstrPageNumPanel))
                //}
            }
            $("<b class='pageNumInfo'>  （第:" + that.PageIndex + "/" + allPageCount + "页)</b>").appendTo($(that.JqstrPageNumPanel));
            return true;
        }
        //主页面HTML
        this.InitMainPanel = function () {
            var arr = [];
            //头部按钮行
            arr.push("<div class='div_buttons_panel'>")
            //如果是视图
            if (that.IsView == false) {
                arr.push("  <input type='button' class='jqBtnUpd' value='更新本页全部'/>")
                arr.push("  <input type='button' class='jqBtnDelThisPage' value='删除本页数据'/>")
                arr.push("  <input type='button' class='jqBtnDelChoose' value='删除选择数据'/>")
                arr.push("  <a> * 数据可从EXCEL内 复制，粘贴到本画面 </a>");
            } else {

            }
            arr.push("  <input type='button' class='' id = 'btnSaveExcelThisPage' value='出力本页EXCEL'/>")
            arr.push("  <input type='button' class='' id = 'btnSaveExcelAll' value='出力全部EXCEL'/>")
            arr.push("  <input type='button' class='' id = 'btnSaveExcelChooseData' value='出力选择EXCEL'/>")
            arr.push("</div>")

            arr.push("<div class='divHeader'>")
            arr.push("  <table class='tableHeader'>")
            arr.push("  </table>")
            arr.push("</div>")

            //明细部
            arr.push("<div class='divBody'>")
            arr.push("  <table class='tableBody' cellSpacing ='0' cellPadding ='0'>")
            arr.push("  </table>")
            arr.push("</div>")
            arr.push("<div class='jqPanelNumOfPages'>")
            arr.push("</div>")
            arr.push("<textarea id='paste' class='pastetxtArea'></textarea>")
            $(arr.join('')).appendTo($(that.JqstrMainPanel));

            //表格对象
            that.tableEle = $("<table class='db_table'></table>");
            that.divHeaderEle = $(".divHeader");
            that.divBodyEle = $(".divBody");
            that.tableTheaderEle = $(".tableBody");
            that.tableTbodyEle = $(".tableBody");

        }
        //加载下拉框数据
        this.InitAllListBoxData = function () {

            for (var key in that.ColParams) {
                if ("ListDataSql" in that.ColParams[key]) {
                    var rtv = that.Ajax("GetListBoxStr", JSON.stringify({ sql: that.ColParams[key]["ListDataSql"] }));
                    if (rtv.result == 'NG') {
                        that.Msg("", "系统错误：加载下拉框数据时：" + rtv.msg, function () { }, rtv.msg);
                        return false;
                    } else if (rtv.data.length == 0) {
                        //that.Msg("", "InitBody 没有数据，不能加载下一步操作!!!");
                        //return false;
                    } else {
                        that.ColParams[key]["ListData"] = rtv.data;
                        var tmRIdx, rlen;
                        var rows = that.ColParams[key].ListData;
                        that.ColParams[key]['listbox'] = [];
                        that.ColParams[key]['listbox'].push({ text: '', value: '' });
                        for (tmRIdx = 0, rlen = rows.length; tmRIdx < rlen; tmRIdx++) {
                            //listDataFilter
                            //if (rows[tmRIdx][that.ColParams[key].listDataFilter.key] == eval(that.ColParams[key].listDataFilter.value)) {                               
                            that.ColParams[key]['listbox'].push({ text: rows[tmRIdx].name, value: rows[tmRIdx].id });
                            //}
                        }
                    }
                }
            }

            return;
            //that.ColParams
            var c, len;
            for (c = 0, len = Object.keys(that.ColParams).length; c < len; c++) {
                if ("ListDataSql" in that.ColParams[c]) {
                    var sql = that.ColParams[c]["ListDataSql"];
                    var rtv = that.Ajax("GetListBoxStr", JSON.stringify({ sql: sql }));
                    if (rtv.result == 'NG') {

                        that.Msg("", "系统错误：加载下拉框数据时：" + rtv.msg, function () { }, rtv.msg);
                        return false;
                    } else if (rtv.data.length == 0) {
                        //that.Msg("", "InitBody 没有数据，不能加载下一步操作!!!");
                        //return false;
                    } else {
                        that.ColParams["ListData"] = rtv.data;





                    }
                }
            }
        }

        //加载
        this.Init = function () {
            that.InitMainPanel();
            ////装载 Header
            //if (!that.InitHeader()) return false;
            ////装载 Body
            //if (!that.InitBody("")) return false;
            that.InitHeader();
            that.InitAllListBoxData();

            that.InitBody("");

            that.SetColumnsWidth();
        }();




        $(window).resize(function () {
            //尺寸设定
            that.ResizeControls();
        });

        that.divBodyEle.scroll(function () { //开始监听滚动条

            //return;
            setTimeout(function () {
                //把提示框隐藏
                $(".tooltip").css("left", -1000);

                //获取当前滚动条高度
                var scLeft = that.divBodyEle.scrollLeft();
                var scTop = that.divBodyEle.scrollTop();

                $(".c0").css("left", scLeft);
                $(".c1").css("left", scLeft);

                $(".trHeader").find("td").css("top", scTop);
                //that.divHeaderEle.scrollLeft(scLeft);
                //如果不固定 Key 列
                if (PubInitParams.fixKeyCols == true) {
                    $(".key_cell").css("left", scLeft);
                    $(".key_cell_ins").css("left", scLeft);
                }

            }, 0);

        });


        that.divHeaderEle.scroll(function () { //开始监听滚动条
            setTimeout(function () {
                var scLeft = that.divHeaderEle.scrollLeft();
                that.divBodyEle.scrollLeft(scLeft);
            }, 10);
        });

        $("<div class='tooltip'>I am a tooltip<br>I am a tooltip<br>I am a tooltip<br>!</div>").appendTo("body");
        $("header .topdiv_left").html(TableCnName + "<div class='divUser'>" + '(' + userCd + ')' + userName + "</div>");

        $(document).keydown(function (e) {
            if (e.keyCode == ctrlKey || e.keyCode == cmdKey) that.ctrlDown = true;
        }).keyup(function (e) {
            if (e.keyCode == ctrlKey || e.keyCode == cmdKey) that.ctrlDown = false;
        });


        var x = -10; //tooltip偏移鼠标的横坐标
        var y = -60; //tooptip偏移鼠标的纵坐标
        var myTitle;
        //1. 鼠标移至新闻，去掉系统默认的tooltip，自定义tooltip
        //2. 鼠标移出新闻，还原系统默认的tooltip，移除自定义的tooltip
        //3. 鼠标在新闻上移动，设置自定义的tooltip的位置
        $(".tooltip_btn").mouseover(function (e) {
            myTitle = this.title;
            this.title = "";
            var tooltip = "<div id='tooltip_btn' style=''><div class='smBqbk'></div>" + myTitle + "</div>";
            $("body").append(tooltip);
            //$("#tooltip_btn").css({
            //    "top": (e.pageY + y) + "px",
            //    "left": (e.pageX + x) + "px"
            //}).show("fast").hide(5000);
        }).mouseout(function () {
            this.title = myTitle;
            $("#tooltip_btn").remove();
        }).mousemove(function (e) {
            $("#tooltip_btn").css({
                "top": (e.pageY + y) + "px",
                "left": (e.pageX + x) + "px"
            });
        });


    };

    //兼容CommonJs规范
    if (typeof module !== "undefined" && module.exports) module.exports = dbTable;
    //兼容AMD/CMD规范
    if (typeof define === "function") define(function () { return dbTable; });

    global.dbTable = dbTable;

})(this);

//🌫🧊🛑❌🎋🎋⚠⬛◼◾▪⛔🔖〰
function alert3(title, fncname, args) {
    var msg = [];
    msg.push("┏╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌┓");
    msg.push("⛔ " + title);
    msg.push("┗╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌┛");
    msg.push("🎫 " + fncname);

    for (var items in args) {
        msg.push(" 🔖 [" + items + "] : [" + args[items] + "]");
    }

    alert(msg.join("\n"));
}


/**
 * String.Trim
 * 文字列のトリム処理
 * @return
 */
String.prototype.Trim = function () { return this.replace(/^\s+|\s+$/g, ""); }

function OpenImgPage(idx) {
    window.open("Pics1.aspx?kbn=chk&idx=" + idx);
}
function OpenImgPage2(picNm, picTm) {
    window.open("Pics1.aspx?kbn=ms&picNm=" + picNm + "&picTm=" + picTm);
}


/**
 * @description  * 显示查询等待层
 * @param query_hint
 */
function show_query_hint(query_hint) {
    var query_hint = document.getElementById(query_hint);
    query_hint.style.display = "block";
}

/**
 * @description 查询结果回调函数
 * @param query_hint 要隐藏的提示层id
 */
function queryHintCallback(query_hint) {
    var query_hint = document.getElementById(query_hint);
    query_hint.style.display = "none";
}