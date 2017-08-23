@echo off
rem Copyright © 2017 Wave Engine S.L. All rights reserved.

setlocal
set error=0

set fxcpath="C:\Program Files (x86)\Windows Kits\10\bin\x86\fxc.exe"

call :CompileShader LPPGBuffer vs vsGBuffer
call :CompileShader LPPGBuffer vs vsGBuffer NORMAL

call :CompileShader LPPGBuffer ps psGBuffer
call :CompileShader LPPGBuffer ps psGBuffer DEPTH
call :CompileShader LPPGBuffer ps psGBuffer MRT
call :CompileShader LPPGBuffer ps psGBuffer NORMAL
call :CompileShader LPPGBuffer ps psGBuffer NORMAL DEPTH
call :CompileShader LPPGBuffer ps psGBuffer NORMAL MRT
echo.

if %error% == 0 (
    echo Shaders compiled ok
) else (
    echo There were shader compilation errors!
)

endlocal
exit /b

:CompileShader
if [%3]==[] goto two
if [%4]==[] goto three
if [%5]==[] goto four
if [%6]==[] goto five
if [%7]==[] goto six
if [%8]==[] goto seven
if [%9]==[] goto eight

:six
set fxc=%fxcpath% /nologo %1.fx /T %2_4_0_level_9_3 /I ..\Helpers.fxh /D %4 /D %5 /D %6 /E %3 /Fo %3_%4_%5_%6.fxo
goto end

:five
set fxc=%fxcpath% /nologo %1.fx /T %2_4_0_level_9_3 /I ..\Helpers.fxh /D %4 /D %5 /E %3 /Fo %3_%4_%5.fxo
goto end

:four
set fxc=%fxcpath% /nologo %1.fx /T %2_4_0_level_9_3 /I ..\Helpers.fxh /D %4 /E %3 /Fo %3_%4.fxo
goto end

:three
set fxc=%fxcpath% /nologo %1.fx /T %2_4_0_level_9_3 /I ..\Helpers.fxh /E %3 /Fo %3.fxo

:end
echo.
echo %fxc%
%fxc% || set error=1
exit /b
