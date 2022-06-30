@echo OFF
SETLOCAL EnableDelayedExpansion


REM These two empty lines are required
set CONTENT=
set FILE=appsettings.Development.json
for /f "delims=" %%x in ('type %FILE%') do set "CONTENT=!CONTENT!%%x!N!"
echo !CONTENT!