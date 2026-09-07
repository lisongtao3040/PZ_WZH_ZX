
class SpreadSheetFileList {
    constructor() {

        //this.init();
        this.file = "";
    }


    init = function (user_id) {

        var that = this;

        $("#file-list").show();

        let data = {
            user_id: user_id
        }
        $.ajax({
            type: "POST",
            url: "AbwDBServ.asmx/GetFileListByUserID",
            async: false,
            contentType: "application/json;charset=utf-8",
            data: JSON.stringify(data),
            dataType: "json",
            success: function (data) {
                if (data.d == '') {
                    alert("ファイルがありません");
                    return;
                }

                const listFiles = data.d.split(",");
                that.OpenFileList(listFiles);

            },
            error: function (message) {
                alert("ERROR:" + message.responseText);
            }
        });
    }

    OpenFileList = function(files) {
        var that = this;
        // Get the file list div element
        const fileListDiv = document.getElementById('file-list');

        // 获取要删除的子元素
        const liElements = fileListDiv.getElementsByTagName('ul');

        // 遍历所有子元素并删除
        while (liElements.length > 0) {
            fileListDiv.removeChild(liElements[0]);
        }


        // Create an unordered list element
        const fileListUl = document.createElement('ul');



        // Add each file path to the list as a list item
        files.forEach(file => {
            const listItem = document.createElement('li');
            listItem.textContent = file;
            fileListUl.appendChild(listItem);

            listItem.addEventListener("click", function () {

                that.file = file;
     /*           $("#file-list").hide();*/
                var div = document.getElementById('file-list');
                div.style.display = 'none';
                var event = new Event('close');
                div.dispatchEvent(event);
                //alert(that.file);

            });
        });
        // Add the unordered list element to the file list div
        fileListDiv.appendChild(fileListUl);
    }
    


}








window.addEventListener('load', function () {
    $("#file-list").hide();
    // 第一个 onload 函数的代码
    //OpenFileList();
    //alert();
});
