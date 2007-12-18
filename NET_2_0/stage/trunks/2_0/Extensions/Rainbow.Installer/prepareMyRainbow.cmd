@echo off
cls
rem ---------------------------------------------------------------------
rem ---------------------------------------------------------------------
set myRainbowRoot=C:\@WORK\Rainbow-Portal\RainbowCVS\CVSROOT\Rainbow
rem ---------------------------------------------------------------------
rem ---------------------------------------------------------------------
rem 
rem 
rem 
rem  ---------------------------------------------------------------------
echo RAINBOW root is set to: 
echo .
echo %myRainbowRoot%
echo .
echo please ensure setting YOUR correct RAINBOW root folder in this file
echo .
pause
stop
rem  ---------------------------------------------------------------------
echo ---------------------------------------------------------------------
echo --- this will prepare your rainbow for the Installer v2 preview
echo .
echo creating folders...
md %myRainbowRoot%\Installer
md %myRainbowRoot%\Installer\Install
md %myRainbowRoot%\Installer\Uninstall
md %myRainbowRoot%\app_support\Installer
md %myRainbowRoot%\DesktopModules\Installer

echo copying files...
copy .\Rainbow.Installer\app_support\rbd.2.0.xsd %myRainbowRoot%\DesktopModules\Installer\

echo .
echo done!
echo now you can build the Rainbow Installer V2 Solution.
echo there is a postbuild action inside the 
echo Rainbow.Modules.Installer project you should look at 
echo to ensure correct path to RAINBOW root 
echo is given for your configuration.
echo .
pause