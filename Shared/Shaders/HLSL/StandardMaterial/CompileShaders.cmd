@echo off
rem Copyright (c) Weekend Game Studio. All rights reserved.

setlocal
set error=0

set fxcpath="C:\Program Files (x86)\Windows Kits\8.1\bin\x86\fxc.exe"

call :CompileShader vs vsMaterial
call :CompileShader vs vsMaterial VCOLOR
call :CompileShader vs vsMaterial VTEX
call :CompileShader vs vsMaterial VCOLOR VTEX
call :CompileShader ps psMaterial

call :CompileShader ps psMaterial LIT
call :CompileShader ps psMaterial LIT AMBI
call :CompileShader ps psMaterial LIT AMBI EMIS
call :CompileShader ps psMaterial LIT AMBI EMIS SPEC
call :CompileShader ps psMaterial LIT AMBI EMIS SPEC DIFF 
call :CompileShader ps psMaterial LIT AMBI EMIS SPEC DIFF VCOLOR
call :CompileShader ps psMaterial LIT AMBI EMIS SPEC DIFF VCOLOR ATEST
call :CompileShader ps psMaterial LIT AMBI SPEC
call :CompileShader ps psMaterial LIT AMBI SPEC DIFF 
call :CompileShader ps psMaterial LIT AMBI SPEC DIFF VCOLOR
call :CompileShader ps psMaterial LIT AMBI SPEC DIFF VCOLOR ATEST
call :CompileShader ps psMaterial LIT AMBI DIFF 
call :CompileShader ps psMaterial LIT AMBI DIFF VCOLOR
call :CompileShader ps psMaterial LIT AMBI DIFF VCOLOR ATEST
call :CompileShader ps psMaterial LIT AMBI VCOLOR
call :CompileShader ps psMaterial LIT AMBI VCOLOR ATEST
call :CompileShader ps psMaterial LIT AMBI ATEST
call :CompileShader ps psMaterial LIT EMIS
call :CompileShader ps psMaterial LIT EMIS SPEC 
call :CompileShader ps psMaterial LIT EMIS SPEC DIFF
call :CompileShader ps psMaterial LIT EMIS SPEC DIFF VCOLOR
call :CompileShader ps psMaterial LIT EMIS SPEC DIFF VCOLOR ATEST
call :CompileShader ps psMaterial LIT EMIS DIFF
call :CompileShader ps psMaterial LIT EMIS DIFF VCOLOR
call :CompileShader ps psMaterial LIT EMIS DIFF VCOLOR ATEST
call :CompileShader ps psMaterial LIT EMIS VCOLOR
call :CompileShader ps psMaterial LIT EMIS VCOLOR ATEST
call :CompileShader ps psMaterial LIT EMIS ATEST
call :CompileShader ps psMaterial LIT SPEC 
call :CompileShader ps psMaterial LIT SPEC DIFF
call :CompileShader ps psMaterial LIT SPEC DIFF VCOLOR
call :CompileShader ps psMaterial LIT SPEC DIFF VCOLOR ATEST
call :CompileShader ps psMaterial LIT SPEC VCOLOR
call :CompileShader ps psMaterial LIT SPEC VCOLOR ATEST
call :CompileShader ps psMaterial LIT SPEC ATEST
call :CompileShader ps psMaterial LIT DIFF
call :CompileShader ps psMaterial LIT DIFF VCOLOR
call :CompileShader ps psMaterial LIT DIFF VCOLOR ATEST
call :CompileShader ps psMaterial LIT DIFF ATEST
call :CompileShader ps psMaterial LIT VCOLOR
call :CompileShader ps psMaterial LIT VCOLOR ATEST		
call :CompileShader ps psMaterial LIT ATEST

call :CompileShader ps psMaterial AMBI
call :CompileShader ps psMaterial AMBI EMIS
call :CompileShader ps psMaterial AMBI EMIS SPEC
call :CompileShader ps psMaterial AMBI EMIS SPEC DIFF 
call :CompileShader ps psMaterial AMBI EMIS SPEC DIFF VCOLOR
call :CompileShader ps psMaterial AMBI EMIS SPEC DIFF VCOLOR ATEST
call :CompileShader ps psMaterial AMBI SPEC
call :CompileShader ps psMaterial AMBI SPEC DIFF 
call :CompileShader ps psMaterial AMBI SPEC DIFF VCOLOR
call :CompileShader ps psMaterial AMBI SPEC DIFF VCOLOR ATEST
call :CompileShader ps psMaterial AMBI DIFF 
call :CompileShader ps psMaterial AMBI DIFF VCOLOR
call :CompileShader ps psMaterial AMBI DIFF VCOLOR ATEST
call :CompileShader ps psMaterial AMBI VCOLOR
call :CompileShader ps psMaterial AMBI VCOLOR ATEST
call :CompileShader ps psMaterial AMBI ATEST
call :CompileShader ps psMaterial EMIS
call :CompileShader ps psMaterial EMIS SPEC 
call :CompileShader ps psMaterial EMIS SPEC DIFF
call :CompileShader ps psMaterial EMIS SPEC DIFF VCOLOR
call :CompileShader ps psMaterial EMIS SPEC DIFF VCOLOR ATEST
call :CompileShader ps psMaterial EMIS DIFF
call :CompileShader ps psMaterial EMIS DIFF VCOLOR
call :CompileShader ps psMaterial EMIS DIFF VCOLOR ATEST
call :CompileShader ps psMaterial EMIS VCOLOR
call :CompileShader ps psMaterial EMIS VCOLOR ATEST
call :CompileShader ps psMaterial EMIS ATEST
call :CompileShader ps psMaterial SPEC 
call :CompileShader ps psMaterial SPEC DIFF
call :CompileShader ps psMaterial SPEC DIFF VCOLOR
call :CompileShader ps psMaterial SPEC DIFF VCOLOR ATEST
call :CompileShader ps psMaterial SPEC VCOLOR
call :CompileShader ps psMaterial SPEC VCOLOR ATEST
call :CompileShader ps psMaterial SPEC ATEST
call :CompileShader ps psMaterial DIFF
call :CompileShader ps psMaterial DIFF VCOLOR
call :CompileShader ps psMaterial DIFF VCOLOR ATEST
call :CompileShader ps psMaterial DIFF ATEST
call :CompileShader ps psMaterial VCOLOR
call :CompileShader ps psMaterial VCOLOR ATEST		
call :CompileShader ps psMaterial ATEST

echo.

if %error% == 0 (
    echo Shaders compiled ok
) else (
    echo There were shader compilation errors!
)

endlocal
exit /b

:CompileShader
if [%3]==[] goto three
if [%4]==[] goto four
if [%5]==[] goto five
if [%6]==[] goto six
if [%7]==[] goto seven
if [%8]==[] goto eight
if [%9]==[] goto nine

set fxc=%fxcpath% /nologo StandardMaterial.fx /T %1_4_0_level_9_3 /I ..\Helpers.fxh /D %3 /D %4 /D %5 /D %6 /D %7 /D %9 /E %2 /Fo %1StandardMaterial_%3_%4_%5_%6_%7_%8_%9.fxo
goto end

:nine
set fxc=%fxcpath% /nologo StandardMaterial.fx /T %1_4_0_level_9_3 /I ..\Helpers.fxh /D %3 /D %4 /D %5 /D %6 /D %7 /D %8 /E %2 /Fo %1StandardMaterial_%3_%4_%5_%6_%7_%8.fxo
goto end

: eight
set fxc=%fxcpath% /nologo StandardMaterial.fx /T %1_4_0_level_9_3 /I ..\Helpers.fxh /D %3 /D %4 /D %5 /D %6 /D %7 /E %2 /Fo %1StandardMaterial_%3_%4_%5_%6_%7.fxo
goto end

:seven
set fxc=%fxcpath% /nologo StandardMaterial.fx /T %1_4_0_level_9_3 /I ..\Helpers.fxh /D %3 /D %4 /D %5 /D %6 /E %2 /Fo %1StandardMaterial_%3_%4_%5_%6.fxo
goto end

:six
set fxc=%fxcpath% /nologo StandardMaterial.fx /T %1_4_0_level_9_3 /I ..\Helpers.fxh /D %3 /D %4 /D %5 /E %2 /Fo %1StandardMaterial_%3_%4_%5.fxo
goto end

:five
set fxc=%fxcpath% /nologo StandardMaterial.fx /T %1_4_0_level_9_3 /I ..\Helpers.fxh /D %3 /D %4 /E %2 /Fo %1StandardMaterial_%3_%4.fxo
goto end

:four
set fxc=%fxcpath% /nologo StandardMaterial.fx /T %1_4_0_level_9_3 /I ..\Helpers.fxh /D %3 /E %2 /Fo %1StandardMaterial_%3.fxo
goto end

:three
set fxc=%fxcpath% /nologo StandardMaterial.fx /T %1_4_0_level_9_3 /I ..\Helpers.fxh /E %2 /Fo %1StandardMaterial%.fxo

:end
echo.
echo %fxc%
%fxc% || set error=1
exit /b

