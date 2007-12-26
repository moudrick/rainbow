@if not exist default.build notepad README.TXT
@if not exist default.build copy default.build.standard default.build
@if not exist VersionInfo.cs copy VersionInfo.cs.default VersionInfo.cs
@call copy.standard.cmd WebSites\Rainbow\web.config 
@call copy.standard.cmd Projects\Rainbow.Tests\App.config
@call copy.standard.cmd Tests\Rainbow.Tests.Data.MsSql\App.config
