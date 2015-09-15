@echo off
rem Copyright (c) Wave Coorporation. All rights reserved.

setlocal
set error=0

set fxcpath="C:\Program Files (x86)\Windows Kits\8.1\bin\x86\fxc.exe"

call :CompileShader vs vsDualTexture

call :CompileShader ps psDualTexture
call :CompileShader ps psDualTexture FIRST
call :CompileShader ps psDualTexture SECON
call :CompileShader ps psDualTexture FIRST SECON MUL
call :CompileShader ps psDualTexture FIRST SECON ADD
call :CompileShader ps psDualTexture FIRST SECON MSK

call :CompileShader ps psDualTexture LIT
call :CompileShader ps psDualTexture LIT FIRST
call :CompileShader ps psDualTexture LIT SECON
call :CompileShader ps psDualTexture LIT FIRST SECON MUL
call :CompileShader ps psDualTexture LIT FIRST SECON ADD
call :CompileShader ps psDualTexture LIT FIRST SECON MSK

echo.

if %error% == 0 (
    echo Shaders compiled ok
) else (
    echo There were shader compilation errors!
)

endlocal
exit /b

:CompileShader
if [%3]==[] goto NotParam
if [%4]==[] goto three
if [%5]==[] goto four
if [%6]==[] goto five
if [%7]==[] goto six

set fxc=%fxcpath% /nologo DualTexture.fx /T %1_4_0_level_9_3 /D %3 /D %4 /D %5 /D %6 /D %7 /E %2 /Fo %1DualMaterial_%3_%4_%5_%6_%7.fxo
goto End

: six
set fxc=%fxcpath% /nologo DualTexture.fx /T %1_4_0_level_9_3 /D %3 /D %4 /D %5 /D %6 /E %2 /Fo %1DualMaterial_%3_%4_%5_%6.fxo
goto End

: five
set fxc=%fxcpath% /nologo DualTexture.fx /T %1_4_0_level_9_3 /D %3 /D %4 /D %5 /E %2 /Fo %1DualMaterial_%3_%4_%5.fxo
goto End

: four
set fxc=%fxcpath% /nologo DualTexture.fx /T %1_4_0_level_9_3 /D %3 /D %4 /E %2 /Fo %1DualMaterial_%3_%4.fxo
goto End

: three
set fxc=%fxcpath% /nologo DualTexture.fx /T %1_4_0_level_9_3 /D %3 /E %2 /Fo %1DualMaterial_%3.fxo
goto End

: NotParam
set fxc=%fxcpath% /nologo DualTexture.fx /T %1_4_0_level_9_3 /E %2 /Fo %1DualMaterial.fxo

: End
echo.
echo %fxc%
%fxc% || set error=1
exit /b


call :CompileShader ps psDualTexture
call :CompileShader ps psDualTexture LIT
call :CompileShader ps psDualTexture LIT MUL
call :CompileShader ps psDualTexture LIT ADD
call :CompileShader ps psDualTexture LIT MSK
