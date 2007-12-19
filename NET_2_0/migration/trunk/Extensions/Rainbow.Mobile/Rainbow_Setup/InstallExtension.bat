@echo off
cls


set source=.\
set destination=..\..\..\CVSROOT\Rainbow



@echo *******************************************************
@echo * Adding the rainbow mobile extiension to your portal 
@echo *                                                     
@echo * source:      %source%                               
@echo * destination: %destination%                          
@echo *******************************************************

@echo !!!!!!!!!
@echo !!!!!!!!! Please review the  source and destination information.
@echo !!!!!!!!! Ensure that they are correct and remark the exit command in this file
@echo !!!!!!!!!
pause
exit

@echo *******************************************************
@echo starting...
@echo .
copy %source%\bin\Rainbow.Mobile.dll  %destination%\bin\

md   %destination%\Modules
xcopy %source%\Modules\*.ascx  %destination%\Modules\ /x /v /s /h /y

copy %source%\Mobile.htm  %destination%\
copy %source%\Mobile.aspx  %destination%\
copy %source%\Mobile.aspx  %destination%\MobileDefault.aspx

copy %source%\Design\Themes\Default\*.*  %destination%\Design\Themes\Default\ 
copy %source%\Design\Themes\DefaultAlternate\*.*  %destination%\Design\Themes\DefaultAlternate\ 
@echo *******************************************************
@echo * updating the rainbow-portal complete                
@echo * go to        %destination%\
@echo * and copy/modify web.config with the 
@echo * changes documented in the mobile_web.config
@echo *******************************************************
pause