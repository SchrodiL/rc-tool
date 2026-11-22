@echo off
chcp 65001 >nul
title Build RC-Tool (C# native)
cd /d "%~dp0"

rem ---- 1. locate the compiler that ships with .NET Framework 4.x ----
set "CSC=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if not exist "%CSC%" set "CSC=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe"
if not exist "%CSC%" (
    echo [ERROR] csc.exe not found - .NET Framework 4.x is required.
    pause
    exit /b 1
)

rem ---- 2. close old instance and clean ----
taskkill /F /IM "RC-Tool.exe" >nul 2>nul
if exist "RC-Tool.exe" del /q "RC-Tool.exe"

rem ---- 3. compile with the machine's own compiler (its refs = its runtime) ----
rem     Optional resources, NOT shipped in the repo:
rem       fw_v210.ufw / fw_v238.ufw            - built-in official firmware images
rem       jl_firmware_upgrade_x86.dll          - official USB upgrade engine
rem     Put them next to this script to enable built-in firmware flashing.
set "ICONOPT="
if exist "app.ico" set "ICONOPT=/win32icon:app.ico"
set "RESOPT="
if exist "fw_v210.ufw"   set "RESOPT=%RESOPT% /resource:fw_v210.ufw,fw_v210.ufw"
if exist "fw_v238.ufw"   set "RESOPT=%RESOPT% /resource:fw_v238.ufw,fw_v238.ufw"
if exist "jl_firmware_upgrade_x86.dll" set "RESOPT=%RESOPT% /resource:jl_firmware_upgrade_x86.dll,jl_firmware_upgrade_x86.dll"

echo [build] compiling with system csc ...
"%CSC%" /nologo /target:winexe /platform:x86 /optimize+ /utf8output %ICONOPT% ^
 /out:"RC-Tool.exe" ^
 /r:System.dll /r:System.Core.dll /r:System.Windows.Forms.dll /r:System.Drawing.dll ^
 /r:System.Web.Extensions.dll %RESOPT% RC-Tool.cs

if errorlevel 1 (
    echo.
    echo [ERROR] compile failed - copy the error lines above.
    pause
    exit /b 1
)
if not exist "RC-Tool.exe" (
    echo [ERROR] RC-Tool.exe not produced.
    pause
    exit /b 1
)

echo [OK] built, launching ...
start "" "RC-Tool.exe"
exit /b 0
