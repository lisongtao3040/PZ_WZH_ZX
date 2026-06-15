class SpreadSheetInitEvent {
    /**
    * 构造函数
    * @param {SpreadSheet} objSpreadSheet - 表格
    */
    constructor(objSpreadSheet) {
        this.objSpreadSheet = objSpreadSheet;
        this.fillHandle = null;
        this.fillHandleLeft = 0;
        this.Init(this.objSpreadSheet);
    }
    Init = function (objSpreadSheet) {
        var that = this;
        objSpreadSheet.AddTitleCell = function (rowNo, colNo, cellType = 0, text = "", value = "", formula = "", className = "table-cell") {
            objSpreadSheet.arrTitleRows[rowNo].AddCell();
            if(rowNo==0)
                objSpreadSheet.colCount++;
        }
            objSpreadSheet.AddTitleRow = function () {
                let ss_row = new SpreadSheetRow({ rowNo: this.TitleRowCount, isTitle: 0 });
                that.InitRow(ss_row);
                objSpreadSheet.arrTitleRows.push(ss_row);
                objSpreadSheet.TitleRowCount++;
            }
            objSpreadSheet.AddRow = function () {
                let ss_row = new SpreadSheetRow({ rowNo: this.rowCount, isTitle: 1 });
                that.InitRow(ss_row);
                objSpreadSheet.arrDataRow.push(ss_row);
                objSpreadSheet.rowCount++;
            }
            objSpreadSheet.Rows = function (i) {
                return objSpreadSheet.arrDataRow[i];
            }
            objSpreadSheet.MsRows = function (i) {
                return objSpreadSheet.arrDataRow[i];
            }
            objSpreadSheet.TitleRows = function (i) {
                return objSpreadSheet.arrTitleRows[i];
            }
        }
        /**
      * 构造函数
      * @param {SpreadSheet} objSpreadSheet - 表格
      */
        InitRow = function (objSpreadRow) {
            objSpreadRow.AddCell = function (param = {
                rowNo: objSpreadRow.rowNo
                , colNo: objSpreadRow.cellIdx
                , cellType: Param_CellType.td
                , text: ''
                , value: ''
                , formula: ''
                , className: ''
                , innerHTML: ''
                , style: {}
            }) {

                let cell
                if (this.cellIdx == 0) {
                    cell = new SpreadSheetCell(param);
                    objSpreadRow.arrCells.push(cell);
                } else {
                    cell = new SpreadSheetCell(param);
                    objSpreadRow.arrCells.push(cell);
                }
                objSpreadRow.cellCount++;
                objSpreadRow.cellIdx++;
            }
            objSpreadRow.Cells = function (colNo) {
                return objSpreadRow.arrCells[colNo];
            }
        }
        /**
        * 构造函数
        * @param {SpreadSheet} objSpreadSheet - 表格
        */
        EleInitTopTitleEvent = function (objSpreadRow) {
            var that = this;
            const table = document.querySelector('table');
            const cols = table.querySelectorAll('th');
            let currentCol = null;
            let initialX = null;
            let currentWidth = null;
            let objfillhandle = null;
            let objfillhandleLeft = null;
            let diffX;
            cols.forEach(col => {
                col.addEventListener('mousedown', function (event) {
                    currentCol = event.target;
                    initialX = event.clientX;
                    currentWidth = currentCol.offsetWidth;
                    //objfillhandle = document.querySelector('#fill-handle');
                    //objfillhandleLeft = objfillhandle.style.left.replace('px', '') + '';
                });
            });
            document.addEventListener('mousemove', function (event) {
                if (currentCol) {
                    diffX = event.clientX - initialX;
                    currentCol.style.width = `${currentWidth + diffX}px`;
                    //objfillhandle = document.querySelector('#fill-handle');
                    //objfillhandle.style.left = `${parseFloat(objfillhandleLeft) + diffX}px`;
                    //$(objfillhandle).hide();
                }
            });
            document.addEventListener('mouseup', function (event) {
                if (currentCol) {
                    var c = parseInt(currentCol.getAttribute('c'));

                    $($($("#footerTable").find('tr')[0]).children().eq(c)[0])[0].style.width = `${currentWidth + diffX}px`;

                    //that.objSpreadSheet.TitleRows(0).Cells(c).width = currentCol.style.width.replace('px', '');
                }
                currentCol = null;
                initialX = null;
                currentWidth = null;
            });
        }
    }
