@echo off
rem Copyright © 2018 Wave Engine S.L. All rights reserved.

setlocal
set error=0

set fxcpath="C:\Program Files (x86)\Windows Kits\10\bin\x86\fxc.exe"

call :CompileShader SkyboxEffect vs vsSkybox

call :CompileShader SkyboxEffect ps psSkybox

echo.

if %error% == 0 (
    echo Shaders compiled ok
) else (
    echo There were shader compilation errors!
)

endlocal
exit /b

:CompileShader
set fxc=%fxcpath% /nologo %1.fx /T %2_4_0_level_9_1 /I Structures.fxh /E %3 /Fo %1%3.fxo  
echo.
echo %fxc%
%fxc% || set error=1
exit /b

