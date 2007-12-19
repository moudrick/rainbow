@echo off
set source=.
set destination=..\..\CVSROOT\Rainbow
@echo *************************************************
@echo updating the rainbow-portal 
@echo source:      %source%
@echo destination: %destination%
@echo *************************************************

copy %source%\bin\Rainbow.Mobile.dll  %destination%\bin\
rem copy %source%\mobile_web.config %destination%\mobile_web.config
md   %destination%\Modules
xcopy %source%\Modules\*.ascx  %destination%\Modules\ /x /v /s /h /y

copy %source%\Mobile.htm  %destination%\
copy %source%\Mobile.aspx  %destination%\

copy %source%\Design\Themes\Default\*.*  %destination%\Design\Themes\Default\ 
copy %source%\Design\Themes\DefaultAlternate\*.*  %destination%\Design\Themes\DefaultAlternate\