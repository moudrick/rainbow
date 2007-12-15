@if not exist default.build notepad README.TXT
@if not exist default.build copy default.build.standard default.build
@if not exist VersionInfo.cs copy VersionInfo.cs.default VersionInfo.cs
@if not exist WebSites\Rainbow\web.config copy WebSites\Rainbow\web.config.standard WebSites\Rainbow\web.config
@if not exist Projects\Rainbow.Tests\App.config copy Projects\Rainbow.Tests\App.config.standard Projects\Rainbow.Tests\App.config
