class TableGenerator {
    constructor(data) {
        //࿈࿈༑
        this.constChar = { enter: '࿈' };
        this.SpreadSheet = data;
        //this.rows = this.SpreadSheet.rowCount;
        //this.cols = this.SpreadSheet.colCount;
        //this.topLine = document.createElement("div");
        //this.topLine.className = "div-topLine";
        //this.container = document.createElement("div");
        //this.container.className = "div-container";
        this.divScroll = document.createElement("div");
        this.divScroll.className = "divScroll";
        this.divScroll = document.createElement("div");
        this.divScroll.className = "divScroll";
        this.divloading = document.createElement("div");
        this.divloading.className = "divloading";
        this.table = document.createElement("table");
        this.table.className = "table-container";
        this.thead = document.createElement("thead");
        this.tbody = document.createElement("tbody");
        //this.SpreadSheet.ele_table = this.table;
        //this.SpreadSheet.ele_thead = this.thead;
        //this.SpreadSheet.ele_tbody = this.tbody;
        this.currentChunk = 0; // 当前已加载的数据块
        this.chunkSize = 500; // 每次加载的数据块大小
        this.isFetching = false; // 是否正在加载数据
        this.isEnd = false; // 是否已经加载完所有数据
        this.currentCol = null;
        this.initialX = null;
        this.currentWidth = null;
        this.colDefaultWidth = 160;
        this.itemHeight = 30;
        this.itemWidth = this.colDefaultWidth;
        this.visibleCount = Math.ceil(document.documentElement.clientHeight / this.itemHeight);
        this.visibleCols = Math.ceil(document.documentElement.clientWidth / this.itemWidth);
        this.visibleCols = this.visibleCols < 60 ? 60 : this.visibleCols;
        this.arrEleSelectedRange = [];

        this.activeRow = -1;
        this.activeCol = -1;
        this.tdTextContent = null;
        this.dragging = false;

        this.init();

    }

    init = function () {
        // 元素高度
        // 可见元素数量
        let mxCols;

        if (this.SpreadSheet.FixColsLength == -1) {
            mxCols = this.visibleCols;
        } else {
            mxCols = this.SpreadSheet.FixColsLength;
        }

        let mxRows;
        if (this.SpreadSheet.FixRowsLength == -1) {
            mxRows = this.visibleCount;
        } else {
            mxRows = this.SpreadSheet.FixRowsLength;
        }



        this.renderFixTitleAndLeft0Col(0, mxCols);//加载表头
        this.renderWholeTableToPage();
        //
        this.renderZW_Row(this.SpreadSheet.TitleRows(0));

        // 渲染初始元素
        this.renderMsRows(0, mxRows, 0, mxCols + 1);
        this.divScroll.style.height = (this.itemHeight + 0) * this.SpreadSheet.rowCount + 'px';
        var that = this;

        function ReRenderBySizeScroll() {
            that.visibleCount = Math.ceil(document.documentElement.clientHeight / that.itemHeight) + that.SpreadSheet.TitleRows.length - 1 + 5;
            //document.body.scrollTop与document.documentElement.scrollTop两者有个特点，就是同时只会有一个值生效。比如document.body.scrollTop能取到值的时候，document.documentElement.scrollTop就会始终为0；反之亦然。所以，如果要得到网页的真正的scrollTop值，如果不考虑safari，可以这样：
            const scrollTop = document.documentElement.scrollTop || document.body.scrollTop;
            let startIndex = Math.ceil((scrollTop - 30) / (that.itemHeight));
            let endIndex = startIndex + that.visibleCount;
            if (startIndex < 0) {
                startIndex = 0;
            }
            if (startIndex >= 0) {
                if (that.SpreadSheet.FixRowsLength == -1) {
                    that.renderMsRows(startIndex, endIndex, 0, mxCols + 1);
                } else {
                    that.renderMsRows(startIndex, mxRows, 0, mxCols + 1);
                }

            }
        }

        function debounce(func, wait = 20, immediate = true) {
            let timeout;
            return function () {
                const context = this,
                    args = arguments;
                const later = function () {
                    timeout = null;
                    if (!immediate) func.apply(context, args);
                };
                const callNow = immediate && !timeout;
                clearTimeout(timeout);
                timeout = setTimeout(later, wait);
                if (callNow) func.apply(context, args);
            };
        }

        // 监听滚动事件
        window.addEventListener('scroll', debounce(function () { ReRenderBySizeScroll }));
        window.addEventListener('resize', debounce(function () { ReRenderBySizeScroll }));

        this.AddEvent();
        //最下端显示就加载
        //var ob = new IntersectionObserver(
        //    function (entries) {
        //        var entry = entries[0];
        //        if (entry.isIntersecting) {
        //            const scrollTop = document.body.scrollTop + document.documentElement.scrollTop;
        //            const startIndex = Math.floor(scrollTop / (itemHeight + 1));
        //            //const startIndex = Math.floor(scrollTop / (itemHeight + 1));
        //            const endIndex = startIndex + visibleCount + 10;
        //            that.renderMsRows(startIndex, endIndex, 0, mxCols);
        //            //console.log('scrollTop:' + scrollTop);
        //            //console.log('startIndex:' + startIndex);
        //            //console.log('endIndex:' + endIndex);
        //        }
        //    }, {
        //        thresholds: 0.1,
        //    }
        //    );
        //ob.observe(this.divloading);
    }
    renderWholeTableToPage = function () {

        if ($("#paste").length == 0) {
            //复制粘贴用
            $("<textarea id='paste' class='pastetxtArea'></textarea>").appendTo($(".fffooter"));

        }

        $("#paste").hide();

        document.body.appendChild(this.divScroll);
        this.table.appendChild(this.thead);
        this.table.appendChild(this.tbody);
        //document.body.appendChild(this.table);


        document.body.insertBefore(this.table, document.body.firstChild);
        this.divloading.innerText = "made by li songtao";
        document.body.appendChild(this.divloading);
    }
    AddEvent = function () {
        var that = this;
        this.tbody.addEventListener('paste', (event) => {
            //console.log("paste");
            if (this.activeRow >= 0 && this.activeCol >= 1) {
                $("#paste").val("");
                $("#paste").show();
                $("#paste").focus();
                setTimeout(function () {
                    //navigator.clipboard.readText()
                    var text = $("#paste").val();
                    if (!text) return;
                    // 按照换行和 tab 分割成数组
                    const rows = text.split('\n');
                    setTimeout(function () {
                        for (let i = 0; i < rows.length; i++) {
                            if (rows[i] == "") break;
                            let arr = rows[i].split('\t');
                            if (i == 0) that.ExCells(that.activeRow + rows.length + 10, that.activeCol + arr.length);
                            for (let j = 0; j < arr.length; j++) {

                                //if (that.tbody.childNodes[that.activeRow + i + 1].childNodes[that.activeCol + j].getAttribute("readonly") != "true") {
                                //    that.tbody.childNodes[that.activeRow + i + 1].childNodes[that.activeCol + j].innerHTML = arr[j];
                                //    that.SpreadSheet.Rows(that.activeRow + i).Cells(that.activeCol + j).innerHTML = arr[j];
                                //    that.SpreadSheet.Rows(that.activeRow + i).Cells(that.activeCol + j).text = arr[j];
                                //    that.SpreadSheet.Rows(that.activeRow + i).Cells(that.activeCol + j).value = arr[j];
                                //}


                                if (that.tbody.childNodes[that.activeRow + i + 1].childNodes[that.activeCol + j].ss_cell.readonly) {
                                    //return;
                                } else {
                                    that.tbody.childNodes[that.activeRow + i + 1].childNodes[that.activeCol + j].innerHTML = arr[j];
                                    that.SpreadSheet.Rows(that.activeRow + i).Cells(that.activeCol + j).innerHTML = arr[j];
                                    that.SpreadSheet.Rows(that.activeRow + i).Cells(that.activeCol + j).text = arr[j];
                                    that.SpreadSheet.Rows(that.activeRow + i).Cells(that.activeCol + j).value = arr[j];
                                }


                            }
                        }
                    }, 100
                    );
                    $("#paste").hide();
                }, 10);
            }
            // 处理粘贴事件
        })
        this.tbody.addEventListener('keydown', (event) => {
            if (event.key === 'Delete') {
                for (let i = 0; i < that.arrEleSelectedRange.length; i++) {
                    for (let j = 0; j <= that.arrEleSelectedRange[i].length - 1; j++) {
                        //if (that.arrEleSelectedRange[i][j].getAttribute("readonly") != "true") {
                        //    that.arrEleSelectedRange[i][j].SetAll_Text("");
                        //}

                        if (that.arrEleSelectedRange[i][j].ss_cell.readonly) {
                            //return;
                        } else {
                            that.arrEleSelectedRange[i][j].SetAll_Text("");
                        }
                    }
                }
                // 按下了Delete键
            }
        })

        this.tbody.addEventListener("mousedown", function (event) {
            //console.log("tbody:mousedown");
            const cell = event.target.closest("td");
            if (!cell) return;
            //console.log("tbody:mousedown:td");

            for (let i = 0; i < that.arrEleSelectedRange.length; i++) {
                for (let j = 0; j <= that.arrEleSelectedRange[i].length - 1; j++) {
                    that.arrEleSelectedRange[i][j].className = "table-cell";
                }
            }

            //清空选择单元格的数组
            that.arrEleSelectedRange = [];

            //console.log(cell.getAttribute('isSelect'));
            that.SpreadSheet.isSelecting = true;
            //console.log("mousedown:that.isSelecting" + that.SpreadSheet.isSelecting);
            that.start_row = cell.ss_cell.rowNo;
            that.start_col = cell.ss_cell.colNo;
            if (cell.getAttribute('isSelect') != null) {
                if (cell.getAttribute('isSelect') == "1") {
                    that.RemovefillHandle(cell);
                    //if (cell.getAttribute("readonly") != "true") {
                    //    cell.setAttribute('contenteditable', "true");
                    //}
                    if (cell.ss_cell.readonly) {
                        //return;
                    } else {
                        cell.setAttribute('contenteditable', "true");
                    }


                    cell.setAttribute('isSelect', "2");
                    cell.innerHTML = cell.innerHTML.replace(/\<a class\=\"br\">\⬎\<\/a\>/g, "<br>");
                    setTimeout(function () { cell.click(); }, 10);
                }
            }
            //event.preventDefault();
        })
        this.tbody.addEventListener("mouseup", function (event) {
            const cell = event.target.closest("td");
            if (!cell) {
                //fill-handle
                that.dragging = false;
            } else {
                //下拉单元格
                if (that.dragging) {
                    var fillCellsWithIncrementingTextValues = function (firstValue, betweenNumber) {
                        var currentTextValue = firstValue;
                        if (event.ctrlKey) {
                            //currentTextValue = cell.value.toString();
                            var lastNumberMatch = currentTextValue.match(/\d+$/);
                            if (lastNumberMatch) {
                                var lastNumberString = lastNumberMatch[0];
                                var lastNumberIndex = currentTextValue.lastIndexOf(lastNumberString);
                                var nextNumber = parseInt(lastNumberString) + betweenNumber;
                                var nextNumberString = nextNumber.toString();
                                currentTextValue = currentTextValue.substring(0, lastNumberIndex) + nextNumberString;
                            }
                        }
                        return currentTextValue;
                    }
                    var tbl_start_rowNo = that.tbody.childNodes[1].childNodes[0].ss_row.rowNo - 1;
                    for (let r = Math.min(that.start_row, that.end_row); r <= Math.max(that.start_row, that.end_row); r++) {
                        for (let c = Math.min(that.start_col, that.end_col); c <= Math.max(that.start_col, that.end_col); c++) {
                            if (!(r == that.start_row && c == that.start_col)) {
                                const txt = fillCellsWithIncrementingTextValues(that.tdTextContent, r - that.start_row);
                                //that.tbody.childNodes[r - tbl_start_rowNo].childNodes[c].innerText = txt;
                                //that.tbody.childNodes[r - tbl_start_rowNo].childNodes[c].ss_cell.innerHTML = txt;

                                that.tbody.childNodes[r - tbl_start_rowNo].childNodes[c].SetAll_Text(txt);
                                //that.SpreadSheet.Rows(r).Cells(c).innerHTML = txt;

                            }
                        }
                    }
                }
                //fill-handle
                that.dragging = false;
            }
            that.SpreadSheet.isSelecting = false;
            //event.preventDefault();
        })
        this.tbody.addEventListener("mousemove", function (event) {
            //console.log("mouseenter:that.isSelecting" + that.SpreadSheet.isSelecting);
            //console.log("tbody:mousemove");
            if (that.SpreadSheet.isSelecting) {
                const cell = event.target.closest("td");
                if (!cell) return;
                that.end_row = cell.ss_cell.rowNo;
                that.end_col = cell.ss_cell.colNo;


                //删除所有选择
                for (let i = 0; i < that.arrEleSelectedRange.length; i++) {
                    for (let j = 0; j <= that.arrEleSelectedRange[i].length - 1; j++) {
                        that.arrEleSelectedRange[i][j].className = "table-cell";
                    }
                }
                ////删除所有选择
                //for (let i = 0; i < that.arrEleSelectedRange.length; i++) {
                //    that.arrEleSelectedRange[i].className = "table-cell";
                //}
                //that.arrEleSelectedRange = [];
                ////that.data_selection = [];


                that.ExCells(that.activeRow + Math.max(that.start_row, that.end_row) + 10, Math.max(that.start_col, that.end_col));
                var tbl_start_rowNo = that.tbody.childNodes[1].childNodes[0].ss_row.rowNo - 1;
                for (let r = Math.min(that.start_row, that.end_row); r <= Math.max(that.start_row, that.end_row); r++) {

                    //var arrData_cell = [];
                    var arrData_ele = [];
                    for (let c = Math.min(that.start_col, that.end_col); c <= Math.max(that.start_col, that.end_col); c++) {
                        arrData_ele.push(that.tbody.childNodes[r - tbl_start_rowNo].childNodes[c]);
                        that.tbody.childNodes[r - tbl_start_rowNo].childNodes[c].className = "select_cell";
                    }
                    that.arrEleSelectedRange.push(arrData_ele);
                }
                //console.log("that.selection.length:" + that.arrEleSelectedRange.length);
            }
        })
    }

    renderFixTitleAndLeft0Col = function (startColIdx, endColIdx) {
        //扩展SpreadSheet 单元格
        this.ExCells(startColIdx, endColIdx);
        let allThWidth = 0;
        for (let i = 0; i <= this.SpreadSheet.TitleRowCount - 1; i++) {
            let tr = document.createElement("tr");
            //this.SpreadSheet.TitleRows(i).ele = tr;
            tr.className = "table-row";
            for (let j = startColIdx; j <= endColIdx - 1 + 1; j++) {
                let cell = document.createElement("th");
                cell.className = "table-cell";
                cell.setAttribute('c', j);

                cell.ss = this.SpreadSheet.thead;
                cell.ss_row = this.SpreadSheet.TitleRows(i);
                cell.ss_cell = this.SpreadSheet.TitleRows(i).Cells(j);

                if (j == 0) cell.className = "table-cell lt_cell";
                if (j > 0) {
                    cell.textContent = j > this.SpreadSheet.TitleRows(i).cellCount - 1 ? j : this.SpreadSheet.TitleRows(i).Cells(j).text;
                }
                //Style
                Object.assign(cell.style, this.SpreadSheet.TitleRows(i).Cells(j).style);


                SetAttributesObj(cell, this.SpreadSheet.TitleRows(i).Cells(j).Attributes)


                tr.appendChild(cell);
                if (i == 0) {
                    allThWidth = allThWidth + cell.style.width;
                }
            }
            this.thead.appendChild(tr);
        }
        this.table.style.width = 0;



    }

    ExCells = function (arrX, arrY) {
        var that = this;
        let colCnt = that.SpreadSheet.colCount;
        let rowCnt = that.SpreadSheet.rowCount;

        //TITLE

        if (colCnt <= arrY) {
            for (let i = 0; i <= that.SpreadSheet.TitleRowCount - 1; i++) {
                for (let j = colCnt; j <= arrY; j++) {
                    that.SpreadSheet.AddTitleCell(i, j);
                    if (i == 0) {
                        that.SpreadSheet.TitleRows(0).Cells(j).text = j;
                    }
                }
            }
        }


        if (colCnt <= arrY) {
            for (let i = 0; i <= rowCnt - 1; i++) {
                for (let j = colCnt; j <= arrY; j++) {
                    that.SpreadSheet.Rows(i).AddCell();
                }
            }
        }



        colCnt = that.SpreadSheet.colCount;
        rowCnt = that.SpreadSheet.rowCount;
        if (rowCnt <= arrX) {
            for (let i = rowCnt; i <= arrX; i++) {
                that.SpreadSheet.AddRow();
                for (let j = 0; j <= colCnt; j++) {
                    that.SpreadSheet.Rows(i).AddCell();
                }
            }
        }
    }
    //占位
    renderZW_Row = function (ss) {
        //占位行
        let rowZW = this.CrEleTr(ss);
        var cell = document.createElement("th");
        cell.className = "th-1";
        cell.id = "thZW";
        cell.textContent = "";
        cell.ss = ss;
        rowZW.appendChild(cell);
        this.tbody.appendChild(rowZW);
    }
    CrEleTr = function (ss) {
        var row = document.createElement("tr");
        row.className = "table-row";
        row.ss = ss;
        return row;
    }
    CrEleTh = function (ss, ss_row, ss_cell) {
        var cell = document.createElement("th");
        cell.ss = ss;
        cell.ss_row = ss_row;
        cell.ss_cell = ss_cell;
        cell.className = "table-cell";
        cell.textContent = ss_row.rowNo + 1;

        //Style
        Object.assign(cell.style, ss_cell.style);
        return cell;
    }
    RemovefillHandle = function (td) {
        return;
        //var fillHandles = td.querySelectorAll(".fill-handle");
        var fillHandles = document.querySelectorAll(".fill-handle");
        if (fillHandles.length <= 0) {
            //console.log(".fill-handle length:不存在");
        } else {
            //console.log(".fill-handle length:存在");
            fillHandles.forEach(element => {
                element.remove();
            });
        }
        td.ss_cell.text = td.innerText;
        td.ss_cell.value = td.textContent;
        td.ss_cell.innerHTML = td.innerHTML;
    }
    DofillHandle = function (td) {
        var that = this;
        //if (that.fillHandle != null) {
        //    // 从其父元素中删除该元素
        //    that.fillHandle.parentNode.removeChild(that.fillHandle);
        //    that.fillHandle = null;
        //}
        //var fillHandles = document.querySelectorAll(".fill-handle");
        //if (fillHandles.length <= 0) {
        //    //console.log(".fill-handle length:不存在");
        //} else {
        //    //console.log(".fill-handle length:存在");
        //    fillHandles.forEach(element => {
        //        //console.log(".fill-handle 删除");
        //        element.remove();
        //    });
        //}
        //this.fillHandle = null;
        // 创建填充手柄元素
        var fillHandle;
        fillHandle = document.querySelector("#fill-handle");
        if (!fillHandle) {
            fillHandle = document.createElement('div');
            fillHandle.className = 'fill-handle'; // 为填充手柄元素添加 ID，以便后续处理
            fillHandle.id = 'fill-handle';
            fillHandle.innerText = "";
            document.body.appendChild(fillHandle);
        } else {
            $(fillHandle).show();
        }
        // 设置填充手柄元素的样式
        //fillHandle.style.position = 'absolute';
        //fillHandle.style.width = '10px';
        //fillHandle.style.height = '10px';
        //fillHandle.style.backgroundColor = 'black';
        //fillHandle.style.border = '1px solid white';
        //fillHandle.style.bottom = '0';
        //fillHandle.style.right = '0';
        // 将填充手柄元素添加到文档中
        //td.appendChild(fillHandle);
        // 获取表格单元格元素的位置和大小
        var cellRect = td.getBoundingClientRect();
        // 计算填充手柄元素的位置
        var fillHandleLeft = window.pageXOffset + cellRect.left + td.offsetWidth - 8; // 8 为填充手柄元素宽度的一半
        var fillHandleTop = window.pageYOffset + cellRect.top + td.offsetHeight - 8; // 8 为填充手柄元素高度的一半
        // 设置填充手柄元素的位置
        fillHandle.style.left = fillHandleLeft + 'px';
        fillHandle.style.top = fillHandleTop + 'px';
        fillHandle.addEventListener("mousedown", function (event) {
            that.dragging = true;
            that.SpreadSheet.isSelecting = true;
            that.tdTextContent = td.textContent;
            //console.log("fillHandle:mousedown");
            event.preventDefault();
            event.stopPropagation();
        })
        //console.log(td.innerHTML);
        // document.addEventListener('mouseup', RemovefillHandle(td));
    }
    CrEleCell = function (ss, ss_row, ss_cell, arrX, arrY) {

        var cell;
        cell = document.createElement("td");

        //Attributes
        //if (ss_cell.Attributes) {
        //    for (const [attrName, attrValue] of Object.entries(ss_cell.Attributes)) {
        //        cell.setAttribute(attrName, attrValue);
        //    }
        //}


        cell.ss = ss;
        cell.ss_row = ss_row;
        cell.ss_cell = ss_cell;
        cell.className = "table-cell";
        //Style
        Object.assign(cell.style, ss_cell.style);

        if (ss_cell.innerHTML == '') {
            cell.textContent = ss_cell.text;
        } else {
            cell.innerHTML = ss_cell.innerHTML;
        }

        if (ss_cell.cellType == Param_CellType.upd_button) {
            let btn = document.createElement("button");
            btn.innerText = "更新";
            btn.id = "SpreadSheet_Upd";
            //btn.addEventListener('click',ss_cell.buttonClick);
            btn.addEventListener('click', function () { UPD(cell); });

            cell.appendChild(btn);

        } else if (ss_cell.cellType == Param_CellType.ins_button) {
            let btn = document.createElement("button");
            btn.innerText = "登録";
            btn.id = "SpreadSheet_Ins";
            cell.appendChild(btn);

        } else if (ss_cell.cellType == Param_CellType.del_button) {
            let btn = document.createElement("button");
            btn.innerText = "削除";
            btn.id = "SpreadSheet_Del";
            btn.addEventListener('click', function () { DEL(cell); });
            cell.appendChild(btn);

        } else if (ss_cell.cellType == Param_CellType.sel_button) {
            let btn = document.createElement("button");
            btn.innerText = "検索";
            btn.id = "SpreadSheet_Sel";
            btn.addEventListener('click', function () { SEL(cell); });
            cell.appendChild(btn);
        } else if (ss_cell.cellType == Param_CellType.clear_button) {
            let btn = document.createElement("button");
            btn.innerText = "クリア";
            btn.id = "SpreadSheet_clear";
            btn.addEventListener('click', function () { CLEAR(cell); });
            cell.appendChild(btn);
        }



        //sel_button

        cell.SetAll_Text = function (txt) {
            this.innerText = txt;
            this.ss_cell.text = txt;
            this.ss_cell.value = txt;
            this.ss_cell.innerHTML = this.innerHTML;

        }

        cell.setAttribute('tabindex', "0");


        var that = this;

        //ᛙᚩᚨᛎᚹ⚲𐱄𐰒𐰒𐰑𐰑𐰒𐭠𐭠𐰃𐰃𑁢𑁣󠄳󠄴󠄵󠅔𑀈🅅🅄🄽🅽🈀🕻🕽🗬🗸🝯🜏🆋🅍🄿🄟🀀🀁🀂🀃𑀃𑀇ʸʸʸʨ˽˾𑀖𑀓𑁿𑀭𑀤𑁠𓄢𓄙𓃁࿃࿅࿅࿇࿈࿉࿊࿌ྼ྿྾࿈࿈༑༐
        cell.addEventListener("focus", function () {
            this.style.userSelect = 'none';
            cell.setAttribute('isSelect', "1");
            //if (cell.getAttribute('isSelect') == "1") that.DofillHandle(cell);
            //console.log("that.DofillHandle(cell)");
            that.DofillHandle(cell);
            setTimeout(() => {
            }, 100);
            that.activeRow = this.ss_cell.rowNo;
            that.activeCol = this.ss_cell.colNo;
        });
        cell.addEventListener("blur", function (event) {

            this.style.userSelect = null;
            that.ExCells(cell.ss_cell.rowNo, cell.ss_cell.colNo);

            this.innerHTML = this.innerHTML.replace(/\<br\>/g, "<a class=\"br\">⬎</a>");
            this.ss_cell.innerHTML = this.innerHTML;
            this.ss_cell.text = this.innerText;

            that.RemovefillHandle(cell);
            cell.removeAttribute('isSelect');
            cell.removeAttribute('contenteditable');

            if (cell.ss_cell.ipt_rule != undefined && cell.ss_cell.ipt_rule != "") {
                for (let j = 0; j <= cell.ss_cell.ipt_rule.length - 1; j++) {
                    if (!cell.ss_cell.ipt_rule[j].rule.test(this.innerText)) {
                        cell.style.color = "red";
                        event.preventDefault();
                        event.stopPropagation();
                        return false;
                    } else {
                        cell.style.color = "#000";
                    }
                }
            }


        });
        cell.addEventListener("keydown", function () {
            var ev = (typeof event != 'undefined') ? window.event : e;
            if (ev.keyCode == 13 || event.ctrlKey) {
                return false;
            } else {

                if (cell.getAttribute('contenteditable') == null) {
                    that.RemovefillHandle(this);

                    //if (cell.getAttribute("readonly") != "true") {
                    //    cell.setAttribute('contenteditable', "true");
                    //    this.innerHTML = this.innerHTML.replace(/\<a class\=\"br\">\⬎\<\/a\>/g, "<br>");
                    //    const range = document.createRange();
                    //    range.selectNodeContents(this);
                    //    const selection = window.getSelection();
                    //    selection.removeAllRanges();
                    //    selection.addRange(range);

                    //}

                    if (cell.ss_cell.readonly) {

                    } else {
                        cell.setAttribute('contenteditable', "true");
                        this.innerHTML = this.innerHTML.replace(/\<a class\=\"br\">\⬎\<\/a\>/g, "<br>");
                        const range = document.createRange();
                        range.selectNodeContents(this);
                        const selection = window.getSelection();
                        selection.removeAllRanges();
                        selection.addRange(range);
                    }

                }
            }
        });
        return cell;
    }
    renderMsRows = function (startIndex, endIndex, startColIdx, endColIdx) {

        $(this.divScroll).hide();
        //扩展SpreadSheet 单元格
        this.ExCells(Math.max(startIndex, endIndex) + 5, Math.max(startColIdx, endColIdx));
        var thZW = document.querySelector('#thZW');
        let removeCnt = 0;
        // 移除不在范围内的元素
        for (let i = 0; i < this.tbody.children.length; i++) {
            const itemIndex = parseInt(this.tbody.children[i].getAttribute('data-index'));
            if (itemIndex >= 0) {
                removeCnt++;
                this.tbody.removeChild(this.tbody.children[i]);
                i--;
            }
        }
        thZW.style.height = (startIndex * 30) + 'px';
        //console.log('removeCnt:' + removeCnt);
        //console.log('special.style.height:' + thZW.style.height);
        for (let i = startIndex; i <= endIndex - 1; i++) {

            if (!this.tbody.querySelector(`[data-index="${i}"]`)) {

                //第一列
                let row = this.CrEleTr(this.SpreadSheet.Rows(i));
                row.setAttribute('data-index', i);

                let cell_th = this.CrEleTh(this.tbody, this.SpreadSheet.Rows(i), this.SpreadSheet.Rows(i).Cells(0));
                row.appendChild(cell_th);


                for (let j = 1; j < endColIdx; j++) {
                    let v = "";
                    let html = "";
                    let cell;

                    //if (typeof this.SpreadSheet.Rows(i).Cells(j) !== 'undefined') {
                    //    console.log(1);
                    //    // obj 有 text 属性
                    //} else {
                    //    console.log(2);
                    //    // obj 没有 text 属性
                    //}

                    cell = this.CrEleCell(this.SpreadSheet.tbody,
                        this.SpreadSheet.Rows(i),
                        this.SpreadSheet.Rows(i).Cells(j),
                        i,
                        j,
                        this.SpreadSheet.Rows(i).Cells(j).cellType);

                    row.appendChild(cell);
                }
                this.tbody.appendChild(row);
            }
        }


        $(this.divScroll).show();
    }
}
