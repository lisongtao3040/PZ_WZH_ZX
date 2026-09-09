//Default.aspx 登录画面：简单输入检查
function CheckLoginInput() {
    var userCd = $("#tbxUserCd");
    var pwd = $("#tbxPassword");

    //用户CD：必须输入 + 半角英数字
    if (IsEmpty(userCd.val())) {
        alert2("【用户CD】- 必须输入！", function () { userCd.focus(); });
        return false;
    }
    if (!chkHankakuEisuuji(userCd.val())) {
        alert2("【用户CD】- 请输入半角英数字！", function () { userCd.focus(); });
        return false;
    }

    //密码：必须输入 + 半角英数字
    if (IsEmpty(pwd.val())) {
        alert2("【密码】- 必须输入！", function () { pwd.focus(); });
        return false;
    }
    if (!chkHankakuEisuuji(pwd.val())) {
        alert2("【密码】- 请输入半角英数字！", function () { pwd.focus(); });
        return false;
    }

    return true;
}