var sht;
var eve;
// 数组长度很长
var tableHTML = null;
var user_id = '';
var table_name = "";

function GetSelParam() {
    var e = $("#SpreadSheet_Sel").parent()[0];

    let keyParam = [];
    for (let i = 0; i <= e.ss_row.arrCells.length - 1; i++) {
        //如果是主键
        //if (e.ss_row.arrCells[i].text != "") {
        //ColumnNameEn
        keyParam.push({
            ColumnNameEn: e.ss_row.arrCells[i].ColumnNameEn,
            value: e.ss_row.arrCells[i].text
        });
        //}
    }
    return keyParam;
}

function SEL(e) {
    //let sqlParam = [];
    //let keyParam = GetSelParam();
    InitBlankSheet(GetSelParam());
}
function CLEAR(e) {

    var e = $("#SpreadSheet_Sel").parent()[0];
    var tr = $("#SpreadSheet_Sel").parent().parent()[0];
    let keyParam = [];
    for (let i = 0; i <= e.ss_row.arrCells.length - 1; i++) {
        //如果是主键
        if (e.ss_row.arrCells[i].text != "") {
            //ColumnNameEn
            e.ss_row.arrCells[i].text = "";

            $(tr).children().eq(i).text("");
        }
    }

}


function UPD(e) {

    let sqlParam = [];
    let keyParam = [];
    for (let i = 0; i <= e.ss_row.arrCells.length - 1; i++) {
        //如果是主键
        if (e.ss_row.arrCells[i].isKey == "1") {
            //ColumnNameEn
            keyParam.push({
                ColumnNameEn: e.ss_row.arrCells[i].ColumnNameEn,
                value: e.ss_row.arrCells[i].text
            });
        }
    }
    //更新检查
    for (let i = 0; i <= e.ss_row.arrCells.length - 1; i++) {
        if (e.ss_row.arrCells[i].isSqlCol == "1") {
            //ColumnNameEn
            sqlParam.push({
                ColumnNameEn: e.ss_row.arrCells[i].ColumnNameEn,
                value: e.ss_row.arrCells[i].text
            });

            if (e.ss_row.arrCells[i].ipt_rule != "") {
                for (let j = 0; j <= e.ss_row.arrCells[i].ipt_rule.length - 1; j++) {
                    if (!e.ss_row.arrCells[i].ipt_rule[j].rule.test(e.ss_row.arrCells[i].text)) {
                        alert(e.ss_row.arrCells[i].ipt_rule[j].msg);
                        return false;
                    }
                }
            }
        }
    }

    //AJAX 调用更新行
    let rtv = WebMethod("AChkDB_SpreadSheet", "UpdMsOneRow",
        JSON.stringify({
            table_name: table_name,
            user_id: user_id,
            keyParam: keyParam,
            sqlParam: sqlParam
        }
        ));

    if (rtv == "OK") {
        let parentElement = document.getElementById("footerTable");
        parentElement.innerHTML = "";
        InitBlankSheet(GetSelParam());
        alert(rtv);
    } else {
        alert(rtv);
    }
}

//删除按钮
function DEL(e) {

    let sqlParam = [];
    let keyParam = [];
    for (let i = 0; i <= e.ss_row.arrCells.length - 1; i++) {
        if (e.ss_row.arrCells[i].isKey == "1") {
            //ColumnNameEn
            keyParam.push({
                ColumnNameEn: e.ss_row.arrCells[i].ColumnNameEn,
                value: e.ss_row.arrCells[i].text
            });
        }
    }


    let rtv = WebMethod("AChkDB_SpreadSheet", "DelMsOneRow",
        JSON.stringify({
            table_name: table_name,
            keyParam: keyParam
        }
        ));

    if (rtv == "OK") {

        let parentElement = document.getElementById("footerTable");
        parentElement.innerHTML = "";
        InitBlankSheet(GetSelParam());

        alert(rtv);
    } else {

        alert(rtv);
    }
}
//localStorage.getItem
function $$LG(keyName) {
    return localStorage.getItem(keyName) == null ? '' : localStorage.getItem(keyName);
}

//加载Sheet数据
function InitBlankSheet(keyParam) {
    //主键字段
    let arrKeys = ["guid"];
    //不被更新的字段
    let arrNoUse = ["chk_user_name", "kakunin_user_name", "syounin_user_name", "ins_sya", "ins_date", "upd_sya", "upd_date"];
    //加载的表信息

    table_name_cn = $$LG('achk_table_name_cn');
    user_id = $$LG('achk_user_id');
    table_name = $$LG('achk_table_name_en');

    //table_name = "m_xzzl";

    //var table_name_cn = localStorage.getItem('achk_table_name_cn') == null ? '' : localStorage.getItem('achk_table_name_cn');
    //user_id = localStorage.getItem('achk_user_id') == null ? '' : localStorage.getItem('achk_user_id');
    //table_name = localStorage.getItem('achk_table_name_en') == null ? '' : localStorage.getItem('achk_table_name_en');


    var ds, dtInfo, dtMs;
    ds = WebMethod("AChkDB_SpreadSheet", "GetTableInfoByJouken",
        JSON.stringify({
            table_name: table_name,
            keyParam: keyParam
        }
        ));
    dtInfo = ds.tableInfo;
    dtMs = ds[table_name];

    //加载Key项目
    for (let i = 0, myLength = dtInfo.length; i < myLength; i++) {
        if (dtInfo[i].PrimaryKey == true) {
            arrKeys.push(dtInfo[i].ColumnNameEn);
        }
    }

    if (tableHTML) {
        tableHTML.divScroll.remove();
        tableHTML.divloading.remove();
        tableHTML.table.remove();
        tableHTML = null;
    }
    sht = new SpreadSheet("sheet1");
    sht.FixColsLength = dtInfo.length + 2;
    sht.FixRowsLength = dtMs.length + 1;

    eve = new SpreadSheetInitEvent(sht);

    let colsCnt = dtInfo.length + 2;

    //表头第一列
    let hss1 = [
        {},
        {
            text: table_name_cn,
            style: {
                color: '#fff',
                width: "100px",
                backgroundColor: '#0E8388',
                'text-align': 'center'
            }
        }
    ];

    var widths = ['90px', '180px', '80px', '100px', '100px', '80px', '200px', '80px', '190px', '90px', '90px', '90px', '90px', '90px', '90px', '90px'];
    //剩余列追加
    for (let i = hss1.length - 1; i <= colsCnt; i++) {
        if (i <= dtInfo.length - 1) {
            
            hss1.push({
                text_mark: '', text: '',
                style: {
                    border: "1",
                    width: GetChildsProperty(dtInfo[i].ColumnNameEn + ".style.width", widths[i]),
                    'font-size': '10px', backgroundColor: '#fff'
                }
            });
        } else {
            hss1.push({
                text_mark: '', text: '',
                style: {
                    border: "0",
                    width: "100px",
                    'font-size': '10px', backgroundColor: '#fff'
                }
            });
        }
    }

    //更新按钮列
    hss1.push({
        text_mark: '', text: '',
        style: {
            border: "0",
            width: "100px",
            'font-size': '10px', backgroundColor: '#fff'
        }
    });

    //删除按钮列
    hss1.push({
        text_mark: '', text: '',
        style: {
            border: "0",
            width: "100px",
            'font-size': '10px', backgroundColor: '#fff'
        }
    });

    //定义2行 项目名作为行数据
    let hss2 = [{}];

    //给每个单元格添加属性
    for (let r = 0; r <= dtInfo.length - 1; r++) {
        hss2.push({
            text: dtInfo[r].ColumnNameCn,
            style: { color: '#fff', backgroundColor: '#0E8388', 'text-align': 'center' },
            Attributes: { readonly: true }
        });
    }

    hss2.push({}, {});

    const hs = [hss1, hss2];

    //绑定表头数据
    for (let r = 0; r <= hs.length - 1; r++) {
        sht.AddTitleRow();
        for (let c = 0; c <= colsCnt; c++) {
            if (c <= hs[r].length - 1) {
                sht.AddTitleCell(r, c);
                sht.TitleRows(r).Cells(c).text = getPropertyOrDefault(hs[r][c], "text", "");
                sht.TitleRows(r).Cells(c).style = getPropertyOrDefault(hs[r][c], "style", {});
                sht.TitleRows(r).Cells(c).Attributes = getPropertyOrDefault(hs[r][c], "Attributes", null);
                //SetAttributesObj(sht.TitleRows(r).Cells(c), getPropertyOrDefault(hs[r][c], "Attributes", null));
            } else {
                sht.AddTitleCell(r, c);
                sht.TitleRows(r).Cells(c).text = c;
            }
        }
    }



    //数据明细
    let ms = [];

    //定义3行 检索条件用
    let kensakuRow = [{}];

    //给每个单元格添加属性
    for (let c = 0; c <= dtInfo.length - 1; c++) {

        let tmpTxt = "";

        if (c + 1 <= keyParam.length - 1) tmpTxt = keyParam[c + 1].value;


        kensakuRow.push({
            text: tmpTxt,
            style: { color: '#fff', backgroundColor: '#455d76', 'text-align': 'center' },
            Attributes: {
                ColumnNameEn: dtInfo[c].ColumnNameEn,
                ColumnType: dtInfo[c].ColumnType
            }
        });
    }
    //更新按钮
    kensakuRow.push({
        text: "",
        style: {},
        Attributes: {
            cellType: Param_CellType.sel_button,
            buttonClick: ""
        }
    });
    kensakuRow.push({
        text: "",
        style: {},
        Attributes: {
            cellType: Param_CellType.clear_button,
            buttonClick: ""
        }
    });
    ms.push(kensakuRow);

    //添加明细数据
    for (let r = 0; r <= dtMs.length - 1; r++) {
        //定义1行 //添加第一个空数据
        let arrMsRow = [{}];
        for (let c = 0; c <= dtInfo.length - 1; c++) {
            //如果是key项目
            if (arrKeys.includes(dtInfo[c].ColumnNameEn)) {
                arrMsRow.push({
                    text: dtMs[r][dtInfo[c].ColumnNameEn],
                    style: { backgroundColor: '#efefef' },
                    Attributes: {
                        readonly: true,
                        'isKey': '1',
                        ColumnNameEn: dtInfo[c].ColumnNameEn

                    }
                });

            } else if (arrNoUse.includes(dtInfo[c].ColumnNameEn)) {//如果是不使用数据

                arrMsRow.push({
                    text: dtMs[r][dtInfo[c].ColumnNameEn],
                    style: { backgroundColor: '#efefef' },
                    Attributes: {
                        readonly: true,
                        'isUpdMark': '1',
                        ColumnNameEn: dtInfo[c].ColumnNameEn
                    }
                });
            } else {//其他的数据
                arrMsRow.push({
                    text: dtMs[r][dtInfo[c].ColumnNameEn],
                    style: {},
                    Attributes: {
                        'isSqlCol': '1',
                        ColumnNameEn: dtInfo[c].ColumnNameEn,
                        ipt_rule: GetChildsProperty(dtInfo[c].ColumnNameEn + ".ipt_rule", ""),
                        ipt_rule_msg: GetChildsProperty(dtInfo[c].ColumnNameEn + ".ipt_rule_msg", "")
                    }
                });

            }

        }

        //更新按钮
        arrMsRow.push({
            text: "",
            style: {},
            Attributes: {
                cellType: Param_CellType.upd_button,
                buttonClick: "UPD(this)"
            }
        });

        //删除按钮
        arrMsRow.push({
            text: "",
            style: {},
            Attributes: { cellType: Param_CellType.del_button }
        });
        ms.push(arrMsRow);
    }

    //遍历做成的数据数组，作成DOM
    for (let r = 0; r <= ms.length - 1; r++) {
        sht.AddRow();
        for (let c = 0; c <= colsCnt + 4; c++) {
            if (c <= ms[r].length - 1) {
                sht.Rows(r).AddCell();
                sht.Rows(r).Cells(c).text = getPropertyOrDefault(ms[r][c], "text", "");
                sht.Rows(r).Cells(c).style = getPropertyOrDefault(ms[r][c], "style", {});
                SetAttributesObj(sht.Rows(r).Cells(c), getPropertyOrDefault(ms[r][c], "Attributes", null));
            } else {
                sht.Rows(r).AddCell();
                sht.Rows(r).Cells(c).text = getPropertyOrDefault(ms[r][c], "text", "");
                sht.Rows(r).Cells(c).style = getPropertyOrDefault(ms[r][c], "style", { border: "0", backgroundColor: '#efefef' });
                SetAttributesObj(sht.Rows(r).Cells(c), getPropertyOrDefault(ms[r][c], "Attributes", null));
            }
        }
    }

    tableHTML = new TableGenerator(sht);
    //加载表头html事件
    eve.EleInitTopTitleEvent(sht);

    let footerTable = document.querySelector("#footerTable");

    //表头第一列
    let bottomHs1 = [{
        tagName: 'th',
        text: ''
    }
    ];

    //剩余列追加
    for (let c = bottomHs1.length - 1; c <= colsCnt; c++) {

        if (c >= 0 && c <= dtInfo.length - 1) {
            if (arrKeys.includes(dtInfo[c].ColumnNameEn)) {
                bottomHs1.push({
                    tagName: 'th',
                    text_mark: '', text: '',
                    style: {
                        border: "0",
                        width: GetChildsProperty(dtInfo[c].ColumnNameEn + ".style.width", widths[c]),
                        'font-size': '10px', backgroundColor: '#efefef'
                    }
                });
            } else if (arrNoUse.includes(dtInfo[c].ColumnNameEn)) {
                bottomHs1.push({
                    tagName: 'th',
                    text_mark: '', text: '',
                    style: {
                        backgroundColor: '#efefef',
                        width: GetChildsProperty(dtInfo[c].ColumnNameEn + ".style.width", widths[c]),

                    },
                    Attributes: {
                        readonly: true,
                        'isUpdMark': '1',
                        ColumnNameEn: dtInfo[c].ColumnNameEn
                    }
                });
            } else {
                bottomHs1.push({
                    tagName: 'td',
                    text_mark: '', text: '',
                    style: {
                        border: "1",
                        width: GetChildsProperty(dtInfo[c].ColumnNameEn + ".style.width", widths[c]),
                        backgroundColor: '#fff'
                    },
                    Attributes: {
                        'contenteditable': 'true',
                        'isSqlCol': '1',
                        'ColumnNameEn': dtInfo[c].ColumnNameEn,
                        ipt_rule: GetChildsProperty(dtInfo[c].ColumnNameEn + ".ipt_rule", ""),
                        ipt_rule_msg: GetChildsProperty(dtInfo[c].ColumnNameEn + ".ipt_rule_msg", "")
                    }
                });
            }

        } else {
            //bottomHs1.push({
            //    tagName: 'th',
            //    text_mark: '', text: '',
            //    style: { border: "0", backgroundColor: 'red' }
            //});
        }


        //给每个单元格添加属性
        for (let c = 0; c <= dtInfo.length - 1; c++) {
            $(sht.Rows(1).Cells(c).ss_cell).click(function (event) {
                alert();
                event.preventDefault();
                event.stopPropagation();
                return false;
            });

        }

    }


    function GetInsRowInfo(hss1, addInsButon) {
        let tr1 = document.createElement("tr");
        tr1.className = "table-row";

        for (c = 0; c <= hss1.length - 1; c++) {
            let cell;
            cell = document.createElement(hss1[c].tagName);
            cell.className = "table-cell";
            Object.assign(cell.style, hss1[c].style);
            SetAttributesObj(cell, getPropertyOrDefault(hss1[c], "Attributes", null));
            tr1.appendChild(cell);


            cell.addEventListener('paste', (event) => {
                //$($("#footerTable").find("tr")[0]).find("td")[]
                //var pasteAre = $("<textarea id='paste' class='pastetxtArea'></textarea>").appendTo(cell);
                //event.currentTarget.cellIndex
                ///alert(this.cellIndex);
                $("#paste").val("");
                $("#paste").show();
                $("#paste").focus();
                var cellIndex = event.currentTarget.cellIndex;
                var td = event.currentTarget;
                var tr = $(td).parent();
                setTimeout(function () {

                    //if ($("#paste").length == 0) {
                    //    //复制粘贴用
                    //    $("<textarea id='paste' class='pastetxtArea'></textarea>").appendTo($(".fffooter"));

                    //}
                    //$("<textarea id='paste' class='pastetxtArea'></textarea>").appendTo($(".fffooter"));
                    //navigator.clipboard.readText()
                    var text = $("#paste").val();
                    if (!text) return;
                    // 按照换行和 tab 分割成数组
                    const rows = text.split('\n');
                    if (rows[rows.length - 1] == "") {
                        rows.pop();
                    }

                    //const rows = text.split('\n');
                    setTimeout(function () {
                        let acTd = td;
                        let acTr = tr;
                        for (let i = 0; i < rows.length; i++) {
                            if (rows[i] == "") break;
                            let arr = rows[i].split('\t');
                            for (let j = 0; j <= arr.length - 1; j++) {
                                if (acTd.tagName == "TD") {
                                    acTd.innerText = arr[j];
                                }
                                acTd = $(acTd).next()[0];
                            }

                            if ($(acTr).next().length <= 0 && i < rows.length - 1) {
                                let tempTr = GetInsRowInfo(bottomHs1, false);
                                footerTable.appendChild(tempTr);
                                acTr = $(tempTr)[0];
                                acTd = $(tempTr).children().eq(cellIndex)[0];
                            }
                        }
                    }, 100
                    );
                    $("#paste").hide();

                }, 10);
            });


        }

        let sousaCell;
        sousaCell = document.createElement("th");
        sousaCell.className = "table-cell";
        Object.assign(sousaCell.style, hss1[hss1.length-1].style);
        SetAttributesObj(sousaCell, getPropertyOrDefault(hss1[hss1.length - 1], "Attributes", null));

        var btnIns = document.createElement("input");
        btnIns.setAttribute("type", "button");
        btnIns.setAttribute("value", "登録");
        btnIns.className = "ins-btn";
        btnIns.addEventListener('click', function (event) {

            let sqlParams = [];


            // 获取所有的 <tr> 元素
            let tr = this.parentNode.parentNode;
            // 遍历每个 <tr> 元素
            let ruleRtv = true;
            $("#footerTable").find("tr").each(function (index) {

                // 获取当前 <tr> 元素中所有的 <td> 元素
                let tdList = $(this)[0].querySelectorAll("td");
                let sqlParam = [];

                // 遍历每个 <td> 元素，并输出其内容
                tdList.forEach(td => {
                    if (td.getAttribute('isSqlCol')) {
                        if (td.getAttribute('isSqlCol') == "1") {
                            sqlParam.push({
                                ColumnNameEn: td.getAttribute('ColumnNameEn'),
                                value: td.innerText
                            });
                        }
                    }

                    if (td.ipt_rule != "") {
                        for (let j = 0; j <= td.ipt_rule.length - 1; j++) {
                            if (!td.ipt_rule[j].rule.test(td.innerText)) {
                                alert(td.ipt_rule[j].msg);
                                ruleRtv = false;
                                return false;
                            }
                        }
                    }
                });
                if (ruleRtv == false) return false;
                sqlParams.push(sqlParam);
            });



            if (ruleRtv == false) return false;

            let rtv = WebMethod("AChkDB_SpreadSheet", "InsMsRows",
                JSON.stringify({
                    table_name: table_name,
                    user_id: user_id,
                    sqlParams: sqlParams
                }
                ));

            if (rtv == "OK") {

                let parentElement = document.getElementById("footerTable");
                parentElement.innerHTML = "";
                InitBlankSheet(GetSelParam());

                alert(rtv);
            } else {

                alert(rtv);
            }


        });

        let sousaCell2 = document.createElement("th");
        sousaCell2.className = "table-cell";
        Object.assign(sousaCell2.style, hss1[hss1.length - 1].style);
        SetAttributesObj(sousaCell2, getPropertyOrDefault(hss1[hss1.length - 1], "Attributes", null));
        var btnClear = document.createElement("input");
        btnClear.setAttribute("type", "button");
        btnClear.setAttribute("value", "クリア");
        btnClear.className = "ins-btn";

        btnClear.addEventListener('click', function (event) {

            footerTable.innerHTML = "";
            let tr1 = GetInsRowInfo(bottomHs1, true);
            footerTable.appendChild(tr1);


        });

        if (addInsButon) {
            sousaCell.appendChild(btnIns);
            sousaCell2.appendChild(btnClear);
        }

        tr1.appendChild(sousaCell);
        tr1.appendChild(sousaCell2);

        return tr1;
    }

    footerTable.innerHTML = "";
    //$("<textarea id='paste' class='pastetxtArea'></textarea>").appendTo($(".footerDiv"));

    let tr1 = GetInsRowInfo(bottomHs1, true);
    footerTable.appendChild(tr1);



    $(".divloading").width($(".table-container").width());
    //$(".footerDivLR").width($(".table-container").width());

}

window.addEventListener('load', function () {

    localStorage.setItem('achk_table_name_cn', '不良替代品');
    localStorage.setItem('achk_table_name_en', 'm_bl_manage');


    InitBlankSheet([]);
    $("#paste").hide();

});

