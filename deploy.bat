set BASE=%~dp0
set SRC_DIR=%BASE%\bin\Debug
set DEST_DIR=%BASE%\deployed

if not exist "%DEST_DIR%" mkdir "%DEST_DIR%"

xcopy "%SRC_DIR%" "%DEST_DIR%" /e /f /y