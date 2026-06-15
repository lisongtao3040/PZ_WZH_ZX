rem ===========================================================================
rem Batch Start 処理(HULFTでPowerShell呼出できない場合、このBAT利用)
rem E:\PS1\PsSendGrid.bat E:\INI_SET_DIR\input\lis6_ipt.txt 00001
rem ---------------------------------------------------------------------------
rem 前処理が必要な場合はここに記述

rem 引数セット
set filePath=%1
set ID=%2
set "currentPath=%~dp0"
powershell -executionPolicy Bypass -command "%currentPath%PsSendGrid.ps1 %filePath% %ID%"
