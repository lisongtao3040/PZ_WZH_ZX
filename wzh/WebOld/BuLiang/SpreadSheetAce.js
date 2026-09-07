function EditorInit(id, languageStr, theme) {
    //初始化对象
    let editor = ace.edit(id);
    //设置风格和语言（更多风格和语言，请到github上相应目录查看）
    language = languageStr
    //editor.setTheme('ace/theme/' + theme);
    //editor.session.setMode('ace/mode/' + language);
    editor.setTheme("ace/theme/sqlserver");
    editor.session.setMode("ace/mode/sql")
    //字体大小
    editor.setFontSize(14);
    //设置只读（true时只读，用于展示代码）
    editor.setReadOnly(false);
    //自动换行,设置为off关闭
    //editor.setOption('wrap', 'free');
    editor.setOption('wrap', 'free');
    //启用提示菜单
    ace.require('ace/ext/language_tools');
    editor.setOptions({
        enableBasicAutocompletion: true,
        enableSnippets: true,
        enableLiveAutocompletion: true
    });
    return editor;
}
