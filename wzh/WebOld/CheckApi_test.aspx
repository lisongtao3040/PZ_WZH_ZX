<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CheckApi_test.aspx.vb" Inherits="CheckApi_test" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>CheckApi Debug</title>
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <style>
        body { font-family: 'Microsoft YaHei', Arial, sans-serif; margin: 20px; background: #f5f5f5; }
        .container { max-width: 1200px; margin: 0 auto; }
        h1 { color: #333; border-bottom: 2px solid #4CAF50; padding-bottom: 10px; }
        .panel { background: #fff; border-radius: 8px; padding: 20px; margin-bottom: 20px; box-shadow: 0 2px 8px rgba(0,0,0,0.1); }
        .panel-title { font-size: 16px; font-weight: bold; margin-bottom: 15px; color: #4CAF50; }
        label { display: inline-block; font-weight: bold; margin: 5px 0; }
        .input-area { display: flex; gap: 10px; align-items: flex-start; flex-wrap: wrap; }
        textarea { width: 100%; max-width: 500px; height: 80px; padding: 8px; border: 1px solid #ccc; border-radius: 4px; font-size: 14px; font-family: Consolas, monospace; }
        .btn { background: #4CAF50; color: white; padding: 10px 24px; border: none; border-radius: 4px; cursor: pointer; font-size: 14px; }
        .btn:hover { background: #45a049; }
        .btn:disabled { background: #ccc; cursor: not-allowed; }
        .result-area { margin-top: 15px; }
        pre { background: #f8f8f8; border: 1px solid #ddd; border-radius: 4px; padding: 15px; overflow-x: auto; font-size: 13px; max-height: 600px; overflow-y: auto; }
        .status-bar { padding: 10px; margin: 10px 0; border-radius: 4px; display: none; }
        .status-success { background: #d4edda; color: #155724; border: 1px solid #c3e6cb; display: block; }
        .status-error { background: #f8d7da; color: #721c24; border: 1px solid #f5c6cb; display: block; }
        table { width: 100%; border-collapse: collapse; margin-top: 10px; }
        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; font-size: 13px; }
        th { background: #4CAF50; color: white; position: sticky; top: 0; }
        tr:nth-child(even) { background: #f9f9f9; }
        tr:hover { background: #f1f1f1; }
        .result-ok { color: #28a745; font-weight: bold; }
        .result-ng { color: #dc3545; font-weight: bold; }
        .url-info { background: #e8f5e9; padding: 8px 12px; border-radius: 4px; margin: 10px 0; font-family: Consolas, monospace; font-size: 13px; }
        .loading { display: inline-block; width: 16px; height: 16px; border: 2px solid #f3f3f3; border-top: 2px solid #4CAF50; border-radius: 50%; animation: spin 1s linear infinite; margin-right: 8px; vertical-align: middle; }
        @keyframes spin { 0% { transform: rotate(0deg); } 100% { transform: rotate(360deg); } }
        .example-btns { margin: 10px 0; }
        .example-btn { background: #e7f3e7; color: #333; border: 1px solid #4CAF50; padding: 4px 12px; border-radius: 12px; cursor: pointer; font-size: 12px; margin: 2px; }
        .example-btn:hover { background: #4CAF50; color: white; }
        .method-info { background: #fff3cd; padding: 8px 12px; border-radius: 4px; margin: 10px 0; font-family: Consolas, monospace; font-size: 13px; border-left: 4px solid #ffc107; }
        .method-info strong { color: #856404; }
        .json-view { background: #f8f8f8; border: 1px solid #ddd; border-radius: 4px; padding: 15px; font-size: 13px; line-height: 1.6; max-height: 600px; overflow-y: auto; }
        .json-view .key { color: #881280; }
        .json-view .string { color: #1a6600; }
        .json-view .number { color: #1750eb; }
        .json-view .bool { color: #1750eb; }
        .json-view .null { color: #808080; }
        .json-view .bracket { color: #333; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="container">
        <h1>CheckApi Debug Page</h1>

        <div class="panel">
            <div class="panel-title">Call API Method</div>
            <div class="method-info">
                <strong>POST</strong> CheckApi.asmx/GetCheckResult<br/>
                <strong>Content-Type:</strong> application/json; charset=utf-8<br/>
                <strong>Request Body:</strong> {"nos":"9006995645,9006979524"}
            </div>
            <div class="url-info">
                WebService URL: <span id="wsUrl">CheckApi.asmx</span> / GetCheckResult
            </div>
            <label for="txtNos">����NO (comma separated):</label>
            <div class="input-area">
                <textarea id="txtNos" placeholder="e.g. 9006995645,9006979524">9006995645,9006979524</textarea>
            </div>
            <div class="example-btns">
                <span style="font-size:12px;color:#666;">Quick fill:</span>
                <button type="button" class="example-btn" onclick="setExample('9006995645,9006979524')">9006995645,9006979524</button>
                <button type="button" class="example-btn" onclick="setExample('9006995645')">9006995645 only</button>
                <button type="button" class="example-btn" onclick="setExample('9006995645,9006979524,9006979525')">3 items</button>
            </div>
            <div style="margin-top: 10px;">
                <button type="button" class="btn" id="btnCall" onclick="callWebService()">Call API</button>
                <button type="button" class="btn" style="background: #6c757d;" onclick="clearResult()">Clear</button>
            </div>
        </div>

        <div id="statusBar" class="status-bar"></div>

        <div class="panel">
            <div class="panel-title">Result</div>
            <div class="result-area">
                <div id="resultSummary" style="display:none; margin-bottom: 10px; font-size: 14px;">
                    Total: <span id="totalCount">0</span> records
                </div>
                <div id="resultTableBlock" style="display:none;">
                    <div style="margin-bottom: 8px;">
                        <label><input type="checkbox" id="chkShowJson" onchange="toggleView()" /> Show JSON</label>
                    </div>
                    <div style="margin-bottom: 10px; padding: 10px; background: #f8f8f8; border: 1px solid #ddd; border-radius: 4px; font-size: 13px;">
                        <strong>Raw JSON Response (Formatted):</strong>
                        <pre id="rawJsonDisplay" style="word-break: break-all; max-height: 250px; overflow-y: auto; font-family: Consolas, monospace; margin-top: 5px; white-space: pre-wrap; background: #fff; padding: 10px; border: 1px solid #eee; border-radius: 3px;"></pre>
                    </div>
                    <div id="resultTableView" style="max-height: 500px; overflow-y: auto;">
                        <table>
                            <thead>
                                <tr>
                                    <th>no</th>
                                    <th>cd</th>
                                    <th>result</th>
                                    <th>status</th>
                                    <th>ck_id</th>
                                    <th>line_cd</th>
                                    <th>chk_user</th>
                                    <th>qianpin</th>
                                    <th>chk_end_date</th>
                                </tr>
                            </thead>
                            <tbody id="tableBody"></tbody>
                        </table>
                    </div>
                    <div id="resultJsonView" style="display:none;">
                        <div id="jsonStructure" class="json-view"></div>
                    </div>
                </div>
                <div id="resultEmpty">
                    <pre id="jsonOutput">Enter NO and click "Call API"</pre>
                </div>
            </div>
        </div>
    </div>
    </form>

    <script type="text/javascript">
        function setExample(val) {
            document.getElementById('txtNos').value = val;
        }

        function showStatus(msg, isError) {
            var bar = document.getElementById('statusBar');
            bar.innerHTML = msg;
            bar.className = 'status-bar ' + (isError ? 'status-error' : 'status-success');
            bar.style.display = 'block';
        }

        function hideStatus() {
            document.getElementById('statusBar').style.display = 'none';
        }

        function clearResult() {
            document.getElementById('resultEmpty').style.display = 'block';
            document.getElementById('resultTableBlock').style.display = 'none';
            document.getElementById('resultSummary').style.display = 'none';
            document.getElementById('chkShowJson').checked = false;
            document.getElementById('jsonOutput').textContent = 'Enter NO and click "Call API"';
            hideStatus();
        }

        function toggleView() {
            var showJson = document.getElementById('chkShowJson').checked;
            document.getElementById('resultJsonView').style.display = showJson ? 'block' : 'none';
            document.getElementById('resultTableView').style.display = showJson ? 'none' : 'block';
        }

        function escapeHtml(str) {
            return String(str)
                .replace(/&/g, '&')
                .replace(/</g, '<')
                .replace(/>/g, '>')
                .replace(/"/g, '"')
                .replace(/'/g, '&#039;');
        }

        function formatJsonValue(val) {
            if (val === null || val === undefined) {
                return '<span class="null">null</span>';
            }
            var t = typeof val;
            if (t === 'string') {
                return '<span class="string">"' + escapeHtml(val) + '"</span>';
            } else if (t === 'number') {
                return '<span class="number">' + val + '</span>';
            } else if (t === 'boolean') {
                return '<span class="bool">' + val + '</span>';
            } else if (Array.isArray(val)) {
                return '<span class="bracket">[</span><span class="number">' + val.length + ' items</span><span class="bracket">]</span>';
            } else if (t === 'object') {
                return '<span class="bracket">{</span><span class="number">' + Object.keys(val).length + ' keys</span><span class="bracket">}</span>';
            }
            return escapeHtml(String(val));
        }

        function buildJsonHtml(obj, indent) {
            indent = indent || 0;
            var pad = '  '.repeat(indent);
            var pad2 = '  '.repeat(indent + 1);
            var html = '';

            if (obj === null || obj === undefined) {
                return '<span class="null">null</span>';
            }

            var t = typeof obj;

            if (t === 'string') {
                return '<span class="string">"' + escapeHtml(obj) + '"</span>';
            } else if (t === 'number' || t === 'boolean') {
                return '<span class="' + t + '">' + obj + '</span>';
            } else if (Array.isArray(obj)) {
                if (obj.length === 0) {
                    return '<span class="bracket">[</span><span class="bracket">]</span>';
                }
                html += '<span class="bracket">[</span><br/>';
                for (var i = 0; i < obj.length; i++) {
                    html += pad2;
                    html += buildJsonHtml(obj[i], indent + 1);
                    if (i < obj.length - 1) {
                        html += '<span class="bracket">,</span>';
                    }
                    html += '<br/>';
                }
                html += pad + '<span class="bracket">]</span>';
                return html;
            } else if (t === 'object') {
                var keys = Object.keys(obj);
                if (keys.length === 0) {
                    return '<span class="bracket">{</span><span class="bracket">}</span>';
                }
                html += '<span class="bracket">{</span><br/>';
                for (var i = 0; i < keys.length; i++) {
                    html += pad2;
                    html += '<span class="key">"' + escapeHtml(keys[i]) + '"</span>';
                    html += '<span class="bracket">:</span> ';
                    html += buildJsonHtml(obj[keys[i]], indent + 1);
                    if (i < keys.length - 1) {
                        html += '<span class="bracket">,</span>';
                    }
                    html += '<br/>';
                }
                html += pad + '<span class="bracket">}</span>';
                return html;
            }

            return escapeHtml(String(obj));
        }

        function callWebService() {
            var nos = document.getElementById('txtNos').value.trim();
            if (!nos) {
                showStatus('Please enter NO!', true);
                return;
            }

            var btn = document.getElementById('btnCall');
            btn.disabled = true;
            btn.innerHTML = '<span class="loading"></span>Requesting...';
            hideStatus();
            document.getElementById('jsonOutput').textContent = 'Requesting...';

            $.ajax({
                type: 'POST',
                url: 'CheckApi.asmx/GetCheckResult',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                data: JSON.stringify({ nos: nos }),
                success: function(response) {
                    var data = typeof response.d === 'string' ? JSON.parse(response.d) : response.d;
                    displayResult(data);
                    showStatus('Success! Total: ' + (data.total || 0) + ' records', false);
                },
                error: function(xhr, status, error) {
                    var errMsg = 'Request failed: ' + error;
                    try {
                        var errData = JSON.parse(xhr.responseText);
                        if (errData && errData.Message) {
                            errMsg += '<br/>' + errData.Message;
                        }
                    } catch(e) {}
                    document.getElementById('jsonOutput').textContent = xhr.responseText || errMsg;
                    showStatus(errMsg, true);
                },
                complete: function() {
                    btn.disabled = false;
                    btn.innerHTML = 'Call API';
                }
            });
        }

        function displayResult(data) {
            var jsonStr = JSON.stringify(data, null, 2);
            // Display raw JSON string
            document.getElementById('rawJsonDisplay').textContent = jsonStr;

            // Build structured JSON view
            document.getElementById('jsonStructure').innerHTML = buildJsonHtml(data, 0);

            var summary = document.getElementById('resultSummary');
            document.getElementById('totalCount').textContent = data.total || 0;

            var tbody = document.getElementById('tableBody');
            tbody.innerHTML = '';

            if (data.data && data.data.length > 0) {
                for (var i = 0; i < data.data.length; i++) {
                    var row = data.data[i];
                    var resultClass = '';
                    if (row.result === 'OK') resultClass = 'result-ok';
                    else if (row.result === 'NG') resultClass = 'result-ng';

                    var tr = document.createElement('tr');
                    tr.innerHTML = 
                        '<td>' + (row.no || '') + '</td>' +
                        '<td>' + (row.cd || '') + '</td>' +
                        '<td class="' + resultClass + '">' + (row.result || '') + '</td>' +
                        '<td>' + (row.status || '') + '</td>' +
                        '<td>' + (row.ck_id || '') + '</td>' +
                        '<td>' + (row.line_cd || '') + '</td>' +
                        '<td>' + (row.chk_user || '') + '</td>' +
                        '<td>' + (row.qianpin || '') + '</td>' +
                        '<td>' + (row.chk_end_date || '') + '</td>';
                    tbody.appendChild(tr);
                }
            } else {
                var tr = document.createElement('tr');
                tr.innerHTML = '<td colspan="9" style="text-align:center;color:#999;">No data</td>';
                tbody.appendChild(tr);
            }

            summary.style.display = 'block';
            document.getElementById('resultEmpty').style.display = 'none';
            document.getElementById('resultTableBlock').style.display = 'block';
            document.getElementById('resultTableView').style.display = 'block';
            document.getElementById('resultJsonView').style.display = 'none';
            document.getElementById('chkShowJson').checked = false;
        }
    </script>
</body>
</html>