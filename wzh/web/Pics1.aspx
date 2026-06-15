<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Pics1.aspx.vb" Inherits="Pics1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta charset="UTF-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" type="text/css" href="App_Themes/Css/htmleaf-demo.css" />
    <script type="text/javascript" src="./jquery/jquery-3.6.0.min.js"></script>
    <script type="text/javascript" src="./js/jQueryRotate.js"></script>
    <title></title>
</head>
<body style="width:100%;height:100%;min-width:1024px; height:500px;">
    <form id="form1" runat="server">

        <div style="position:absolute;z-index:1000000;left:0px;top:0px;">
            <input type="button" value="正常顺序" onclick="ImgRotate(-1)" />
            <input type="button" value="顺时针旋转90度" onclick="ImgRotate(0)" />
            <input type="button" value="逆时针旋转90度" onclick="ImgRotate(1)" />
            <input type="button" value="顺时针旋转180度" onclick="ImgRotate(2)" />
            <input type="button" value="顺时针旋转270度" onclick="ImgRotate(3)" />


        </div>
        <div style="margin-top:30px;">
            <asp:Image ID="Image1" runat="server" />
        </div>
        <script>

            $(document).ready(function () {
                if ($('#Image1').width() > $('#Image1').height()) {
                    //$('#Image1').width(document.documentElement.clientWidth - 100);
                    $('#Image1').height(window.screen.height - 200);
                } else {
                    $('#Image1').height(window.screen.height - 200);
                    //document.body.offsetHeight
                }
            });

            //示例三：点击图片旋转角度（这段js放在示例一后面没有效果，暂不知原因）
            var value = 0;       
            //示例四：图片旋转不同角度
            var ImgRotate = function(i){
                switch(i)
                {
                    case -1:
                        $('#Image1').rotate(0);
                        break;
                    case 0:
                        $('#Image1').rotate(90);
                        break;
                    case 1:
                        $('#Image1').rotate(-90);
                        break;    
                    case 2:
                        $('#Image1').rotate(180);
                        break;    
                    case 3:
                        $('#Image1').rotate(270);
                        break;                    
                }
            }

        </script>
    </form>
</body>
</html>
