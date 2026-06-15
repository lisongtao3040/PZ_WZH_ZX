//获得属性
function getPropertyOrDefault(obj, prop, defaultValue) {

    //if (eval("typeof obj." + prop) !== 'undefined') {
    //    // obj 有 text 属性
    //    return defaultValue;
    //} else {

    //    // obj 没有 text 属性
    //}

    try {
        if (prop in obj || obj.hasOwnProperty(prop)) {
            return obj[prop];
        } else {
            return defaultValue;
        }
    } catch (e1) {
        return defaultValue;
    }
}

//设置Attr到Dom
function SetAttributesObj(tdElement, attributes) {
    if (attributes == null) return;
    try {
        for (var key in attributes) {

            if (key == "ipt_rule") {
                console.log(1);
            }

            if (tdElement instanceof HTMLElement) {
                tdElement[key] = attributes[key];
                tdElement.setAttribute(key, attributes[key]);
            } else {
                tdElement[key] = attributes[key];
            }
        }
        //for (var key in attributes) {
        //    if (key == "cellType") {
        //        tdElement.cellType = attributes[key];
        //    } else if (key == "rowspan") {
        //        tdElement.rowSpan = attributes[key];
        //    } else if (attributes.hasOwnProperty(key)) {
        //        tdElement.setAttribute(key, attributes[key]);
        //    }
        //}
    } catch (e1) {
        //return defaultValue;
    }
}