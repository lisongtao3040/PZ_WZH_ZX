@echo off
setlocal

set "inputString=LXJGCDSDB006.lixil.lan"

rem 直接提取点前的内容
for /f "tokens=1 delims=." %%a in ("%inputString%") do (
    set "outputString=%%a"
)

echo %outputString%
pause
endlocal
