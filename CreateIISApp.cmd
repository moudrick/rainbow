@if ""=="%branch%" set branch=RainbowDotNet2/devint/trunk/
@if ""=="%webSiteName%" set webSiteName=1
@if ""=="%virtDirName%" set virtDirName=Rainbow2.0
@if ""=="%relativeApplicationPath%" set relativeApplicationPath=WebSites/Rainbow/

@echo Dim currentDir: currentDir = "%CD%" > cd.vbs
@echo Dim branchDir: branchDir = "%branch%%relativeApplicationPath%" >> cd.vbs
@echo Dim webSiteName: webSiteName = "%webSiteName%" >> cd.vbs
@echo Dim strVirtualDirectoryName: strVirtualDirectoryName = "%virtDirName%" >>cd.vbs

copy cd.vbs + CreateIISApp.vbs Create.vbs
cscript.exe Create.vbs
del cd.vbs
del Create.vbs