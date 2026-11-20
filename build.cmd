@echo off
chcp 65001 >nul
title Build RC-Tool
cd /d "%~dp0"

set "CSC=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if not exist "%CSC%" set "CSC=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe"
if not exist "%CSC%" (
    echo [ERROR] csc.exe not found - .NET Framework 4.x is required.
    pause
    exit /b 1
)

taskkill /F /IM "RC-Tool.exe" >nul 2>nul
if exist "RC-Tool.exe" del /q "RC-Tool.exe"

set "ICON="
if exist "app.ico" set "ICON=/win32icon:app.ico"

"%CSC%" /nologo /target:winexe /platform:anycpu /optimize+ /utf8output %ICON% ^
 /out:"RC-Tool.exe" ^
 /r:System.dll /r:System.Core.dll /r:System.Windows.Forms.dll /r:System.Drawing.dll ^
 /r:System.Web.Extensions.dll RC-Tool.cs

if errorlevel 1 (
    echo.
    echo [ERROR] compile failed.
    pause
    exit /b 1
)

echo [OK] built, launching...
start "" "RC-Tool.exe"
exit /b 0
