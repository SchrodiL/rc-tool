@echo off
chcp 65001 >nul
title Upload RC-Tool to GitHub
cd /d "%~dp0"

where git >nul 2>nul
if errorlevel 1 (
    echo [ERROR] Git is not installed. Install it first: https://git-scm.com/download/win
    pause
    exit /b 1
)

if not exist ".git" (
    echo [ERROR] .git folder missing - unzip the full package.
    pause
    exit /b 1
)

echo.
echo 1) Create an EMPTY repository on GitHub first (do NOT add a README):
echo    https://github.com/new
echo.
set /p URL=2) Paste your repo URL (https://github.com/USERNAME/REPO.git): 

if "%URL%"=="" (
    echo [ERROR] empty URL
    pause
    exit /b 1
)

git remote remove origin >nul 2>nul
git remote add origin %URL%
echo.
echo Pushing... (a browser login window may pop up the first time)
git push -u origin main
if errorlevel 1 (
    echo.
    echo [ERROR] push failed - check the URL and your GitHub login.
) else (
    echo.
    echo [OK] uploaded successfully.
)
pause
