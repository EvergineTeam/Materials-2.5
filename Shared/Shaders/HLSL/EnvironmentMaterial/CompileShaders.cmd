@echo off
rem Copyright © 2018 Wave Engine S.L. All rights reserved.

setlocal
set error=0

set fxcpath="C:\Program Files (x86)\Windows Kits\10\bin\x86\fxc.exe"

call :CompileShader vs vsEnvironmentMaterial
call :CompileShader vs vsEnvironmentMaterial LIT
call :CompileShader vs vsEnvironmentMaterial LIT DIFF
call :CompileShader vs vsEnvironmentMaterial DIFF
call :CompileShader vs vsEnvironmentMaterial NORMAL
call :CompileShader vs vsEnvironmentMaterial DIFF NORMAL

call :CompileShader ps psEnvironmentMaterial
call :CompileShader ps psEnvironmentMaterial LIT
call :CompileShader ps psEnvironmentMaterial LIT DIFF
call :CompileShader ps psEnvironmentMaterial DIFF

call :CompileShader ps psEnvironmentMaterial ENV
call :CompileShader ps psEnvironmentMaterial LIT ENV
call :CompileShader ps psEnvironmentMaterial LIT FRES DIFF ENV
call :CompileShader ps psEnvironmentMaterial LIT DIFF ENV
call :CompileShader ps psEnvironmentMaterial FRES DIFF ENV
call :CompileShader ps psEnvironmentMaterial DIFF ENV

call :CompileShader ps psEnvironmentMaterial ENV NORMAL
call :CompileShader ps psEnvironmentMaterial FRES DIFF ENV NORMAL
call :CompileShader ps psEnvironmentMaterial DIFF ENV NORMAL

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

set fxc=%fxcpath% /nologo EnvironmentMaterial.fx /T %1_4_0_level_9_3 /D %3 /D %4 /D %5 /D %6 /E %2 /Fo %1EnvironmentMaterial_%3_%4_%5_%6.fxo
goto End

: five
set fxc=%fxcpath% /nologo EnvironmentMaterial.fx /T %1_4_0_level_9_3 /D %3 /D %4 /D %5 /E %2 /Fo %1EnvironmentMaterial_%3_%4_%5.fxo
goto End

: four
set fxc=%fxcpath% /nologo EnvironmentMaterial.fx /T %1_4_0_level_9_3 /D %3 /D %4 /E %2 /Fo %1EnvironmentMaterial_%3_%4.fxo
goto End

: three
set fxc=%fxcpath% /nologo EnvironmentMaterial.fx /T %1_4_0_level_9_3 /D %3 /E %2 /Fo %1EnvironmentMaterial_%3.fxo
goto End

: NotParam
set fxc=%fxcpath% /nologo EnvironmentMaterial.fx /T %1_4_0_level_9_3 /E %2 /Fo %1EnvironmentMaterial.fxo

: End
echo.
echo %fxc%
%fxc% || set error=1
exit /b


