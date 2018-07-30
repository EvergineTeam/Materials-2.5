@echo off
rem Copyright © 2018 Wave Engine S.L. All rights reserved.

setlocal
set error=0

set fxcpath="C:\Program Files (x86)\Windows Kits\10\bin\x86\fxc.exe"

call :CompileShader vs vs

call :CompileShader vs vs AMBI
call :CompileShader vs vs AMBI VTEX
call :CompileShader vs vs AMBI VTEX VCOLOR
call :CompileShader vs vs AMBI VCOLOR
call :CompileShader vs vs VTEX
call :CompileShader vs vs VTEX VCOLOR
call :CompileShader vs vs VCOLOR
call :CompileShader vs vs VLIT
call :CompileShader vs vs VLIT AMBI
call :CompileShader vs vs VLIT AMBI VTEX
call :CompileShader vs vs VLIT AMBI VTEX VCOLOR
call :CompileShader vs vs VLIT VTEX
call :CompileShader vs vs VLIT VTEX VCOLOR
call :CompileShader vs vs VLIT VCOLOR


call :CompileShader ps ps

call :CompileShader ps ps AMBI
call :CompileShader ps ps AMBI DIFF 
call :CompileShader ps ps AMBI DIFF VCOLOR
call :CompileShader ps ps AMBI DIFF VCOLOR ATEST
call :CompileShader ps ps AMBI VCOLOR
call :CompileShader ps ps AMBI VCOLOR ATEST
call :CompileShader ps ps AMBI ATEST
call :CompileShader ps ps DIFF
call :CompileShader ps ps DIFF VCOLOR
call :CompileShader ps ps DIFF VCOLOR ATEST
call :CompileShader ps ps DIFF ATEST
call :CompileShader ps ps VCOLOR
call :CompileShader ps ps VCOLOR ATEST
call :CompileShader ps ps ATEST

call :CompileShader ps ps LIT POINT 
call :CompileShader ps ps LIT POINT AMBI
call :CompileShader ps ps LIT POINT AMBI SPEC
call :CompileShader ps ps LIT POINT AMBI SPEC DIFF 
call :CompileShader ps ps LIT POINT AMBI SPEC DIFF VCOLOR
call :CompileShader ps ps LIT POINT AMBI SPEC DIFF VCOLOR ATEST
call :CompileShader ps ps LIT POINT AMBI DIFF 
call :CompileShader ps ps LIT POINT AMBI DIFF VCOLOR
call :CompileShader ps ps LIT POINT AMBI DIFF VCOLOR ATEST
call :CompileShader ps ps LIT POINT AMBI VCOLOR
call :CompileShader ps ps LIT POINT AMBI VCOLOR ATEST
call :CompileShader ps ps LIT POINT AMBI ATEST
call :CompileShader ps ps LIT POINT SPEC 
call :CompileShader ps ps LIT POINT SPEC DIFF
call :CompileShader ps ps LIT POINT SPEC DIFF VCOLOR
call :CompileShader ps ps LIT POINT SPEC DIFF VCOLOR ATEST
call :CompileShader ps ps LIT POINT DIFF
call :CompileShader ps ps LIT POINT DIFF VCOLOR
call :CompileShader ps ps LIT POINT DIFF VCOLOR ATEST
call :CompileShader ps ps LIT POINT DIFF ATEST
call :CompileShader ps ps LIT POINT VCOLOR
call :CompileShader ps ps LIT POINT VCOLOR ATEST
call :CompileShader ps ps LIT POINT ATEST

call :CompileShader ps ps LIT DIRECTIONAL 
call :CompileShader ps ps LIT DIRECTIONAL AMBI
call :CompileShader ps ps LIT DIRECTIONAL AMBI SPEC
call :CompileShader ps ps LIT DIRECTIONAL AMBI SPEC DIFF 
call :CompileShader ps ps LIT DIRECTIONAL AMBI SPEC DIFF VCOLOR
call :CompileShader ps ps LIT DIRECTIONAL AMBI SPEC DIFF VCOLOR ATEST
call :CompileShader ps ps LIT DIRECTIONAL AMBI DIFF 
call :CompileShader ps ps LIT DIRECTIONAL AMBI DIFF VCOLOR
call :CompileShader ps ps LIT DIRECTIONAL AMBI DIFF VCOLOR ATEST
call :CompileShader ps ps LIT DIRECTIONAL AMBI VCOLOR
call :CompileShader ps ps LIT DIRECTIONAL AMBI VCOLOR ATEST
call :CompileShader ps ps LIT DIRECTIONAL AMBI ATEST
call :CompileShader ps ps LIT DIRECTIONAL SPEC 
call :CompileShader ps ps LIT DIRECTIONAL SPEC DIFF
call :CompileShader ps ps LIT DIRECTIONAL SPEC DIFF VCOLOR
call :CompileShader ps ps LIT DIRECTIONAL SPEC DIFF VCOLOR ATEST
call :CompileShader ps ps LIT DIRECTIONAL DIFF
call :CompileShader ps ps LIT DIRECTIONAL DIFF VCOLOR
call :CompileShader ps ps LIT DIRECTIONAL DIFF VCOLOR ATEST
call :CompileShader ps ps LIT DIRECTIONAL DIFF ATEST
call :CompileShader ps ps LIT DIRECTIONAL VCOLOR
call :CompileShader ps ps LIT DIRECTIONAL VCOLOR ATEST
call :CompileShader ps ps LIT DIRECTIONAL ATEST

call :CompileShader ps ps LIT SPOT 
call :CompileShader ps ps LIT SPOT AMBI
call :CompileShader ps ps LIT SPOT AMBI SPEC
call :CompileShader ps ps LIT SPOT AMBI SPEC DIFF 
call :CompileShader ps ps LIT SPOT AMBI SPEC DIFF VCOLOR
call :CompileShader ps ps LIT SPOT AMBI SPEC DIFF VCOLOR ATEST
call :CompileShader ps ps LIT SPOT AMBI DIFF 
call :CompileShader ps ps LIT SPOT AMBI DIFF VCOLOR
call :CompileShader ps ps LIT SPOT AMBI DIFF VCOLOR ATEST
call :CompileShader ps ps LIT SPOT AMBI VCOLOR
call :CompileShader ps ps LIT SPOT AMBI VCOLOR ATEST
call :CompileShader ps ps LIT SPOT AMBI ATEST
call :CompileShader ps ps LIT SPOT SPEC 
call :CompileShader ps ps LIT SPOT SPEC DIFF
call :CompileShader ps ps LIT SPOT SPEC DIFF VCOLOR
call :CompileShader ps ps LIT SPOT SPEC DIFF VCOLOR ATEST
call :CompileShader ps ps LIT SPOT DIFF
call :CompileShader ps ps LIT SPOT DIFF VCOLOR
call :CompileShader ps ps LIT SPOT DIFF VCOLOR ATEST
call :CompileShader ps ps LIT SPOT DIFF ATEST
call :CompileShader ps ps LIT SPOT VCOLOR
call :CompileShader ps ps LIT SPOT VCOLOR ATEST
call :CompileShader ps ps LIT SPOT ATEST

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

set fxc=%fxcpath% /nologo /O3 ForwardMaterial.fx /T %1_4_0_level_9_3 /I ..\Helpers.fxh /D %3 /D %4 /D %5 /D %6 /D %7 /D %9 /E %2 /Fo %1ForwardMaterial_%3_%4_%5_%6_%7_%8_%9.fxo
goto end

:nine
set fxc=%fxcpath% /nologo /O3 ForwardMaterial.fx /T %1_4_0_level_9_3 /I ..\Helpers.fxh /D %3 /D %4 /D %5 /D %6 /D %7 /D %8 /E %2 /Fo %1ForwardMaterial_%3_%4_%5_%6_%7_%8.fxo
goto end

: eight
set fxc=%fxcpath% /nologo /O3 ForwardMaterial.fx /T %1_4_0_level_9_3 /I ..\Helpers.fxh /D %3 /D %4 /D %5 /D %6 /D %7 /E %2 /Fo %1ForwardMaterial_%3_%4_%5_%6_%7.fxo
goto end

:seven
set fxc=%fxcpath% /nologo /O3 ForwardMaterial.fx /T %1_4_0_level_9_3 /I ..\Helpers.fxh /D %3 /D %4 /D %5 /D %6 /E %2 /Fo %1ForwardMaterial_%3_%4_%5_%6.fxo
goto end

:six
set fxc=%fxcpath% /nologo /O3 ForwardMaterial.fx /T %1_4_0_level_9_3 /I ..\Helpers.fxh /D %3 /D %4 /D %5 /E %2 /Fo %1ForwardMaterial_%3_%4_%5.fxo
goto end

:five
set fxc=%fxcpath% /nologo /O3 ForwardMaterial.fx /T %1_4_0_level_9_3 /I ..\Helpers.fxh /D %3 /D %4 /E %2 /Fo %1ForwardMaterial_%3_%4.fxo
goto end

:four
set fxc=%fxcpath% /nologo /O3 ForwardMaterial.fx /T %1_4_0_level_9_3 /I ..\Helpers.fxh /D %3 /E %2 /Fo %1ForwardMaterial_%3.fxo
goto end

:three
set fxc=%fxcpath% /nologo /O3 ForwardMaterial.fx /T %1_4_0_level_9_3 /I ..\Helpers.fxh /E %2 /Fo %1ForwardMaterial%.fxo

:end
echo.
echo %fxc%
%fxc% || set error=1
exit /b

