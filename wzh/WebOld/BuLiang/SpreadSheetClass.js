
const Param_CellType = {
    th: 1,
    td: 2,
    upd_button: 10,
    del_button: 11,
    ins_button: 12,
    sel_button: 13,
    clear_button: 14
}

//更好的方法因具体需求和应用场景而异，但以下是一些可能的建议：
//在 SpreadSheetCell 类中添加计算值（value）的方法，而不是将其作为属性存储。这样可以避免在对单元格进行更改时出现同步问题。
//在 SpreadSheetRow 类中添加添加单元格（addCell）、删除单元格（deleteCell）和获取单元格（getCell）的方法，而不是直接在数组上进行操作。这样可以确保对单元格数组进行的更改都是有效的，不会引起错误或不一致性。
//将选定单元格的相关逻辑从 SpreadSheet 类中拆分出来，并创建一个新的 Selection 类。这样可以使代码更具可读性和可维护性，同时减少 SpreadSheet 类的复杂性。
//将电子表格的创建和渲染逻辑与电子表格数据的处理逻辑分离，以创建更灵活和可扩展的代码结构。例如，可以创建一个独立的 SpreadsheetRenderer 类，负责将电子表格数据渲染为 HTML，而不是将该逻辑与电子表格类混合在一起。
//使用面向对象编程原则和设计模式来提高代码质量和可维护性。例如，可以使用单例模式来确保应用程序中只有一个电子表格实例，或使用工厂模式来创建不同类型的单元格。
// SpreadSheetCell 类
class SpreadSheetCell {
    // 构造函数，接受一个 options 对象参数，包含单元格的各个属性
    constructor(options = { cellType: Param_CellType.td, text: '', value: '', formula: '', className: '', innerHTML: '', Attributes: {}, style: {} }) {
        // 检查 rowNo 是否为数字，如果不是则抛出类型错误
        if (typeof options.rowNo !== 'number' || isNaN(options.rowNo)) {
            throw new TypeError('rowNo 必须是一个数字');
        }
        // 检查 colNo 是否为数字，如果不是则抛出类型错误
        if (typeof options.colNo !== 'number' || isNaN(options.colNo)) {
            throw new TypeError('colNo 必须是一个数字');
        }
        // 设置单元格的行号和列号属性
        this.rowNo = options.rowNo;
        this.colNo = options.colNo;
        // 设置单元格的类型、文本、值、公式、类名、innerHTML、属性和样式等属性
        this.cellType = options.cellType;
        this.text = options.text;
        this.value = options.value;
        this.formula = options.formula;
        this.className = options.className;
        this.innerHTML = options.innerHTML;
        this.style = options.style;
        this.Attributes = options.Attributes;
    }
}

class SpreadSheetRow {
    // 构造函数，接受一个 options 对象参数，包含行号和是否是标题行两个属性
    constructor(options = { rowNo: 0, isTitle: false }) {
        this.rowNo = options.rowNo; // 行号
        this.isTitle = options.isTitle; // 是否是标题行
        this.arrCells = []; // 单元格数据数组
        this.cellIdx = 0; // 当前单元格索引
        this.cellCount = 0; // 单元格数量
    }
}

class SpreadSheet {
    /**
    * 构造函数
    * @param {string} sheetName - 表格名称
    */
    constructor(sheetName) {
        this.sheetName = sheetName;
        //this.lastRowNum = 0;
        this.arrDataRow = [];
        this.arrTitleRows = [];
        this.rowCount = 0;
        this.colCount = 0;
        this.TitleRowCount = 0;
        this.isSelecting = false;
        this.start_row = -1
        this.start_col = -1
        this.end_row = -1
        this.end_col = -1
        this.FixColsLength = -1;
        this.FixRowsLength = -1;
   /*   this.arrSelectedRange = [];*/
        //this.ele_tbody = null;
        //this.ele_thead = null;
        this.ele_table = null;
    }
}

