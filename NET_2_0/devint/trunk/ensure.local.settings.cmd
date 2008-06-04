@set local_settings_file=local.settings.build
@if not exist %local_settings_file% notepad README.TXT
@if not exist %local_settings_file% copy %local_settings_file%.standard %local_settings_file%
@if not exist VersionInfo.cs copy VersionInfo.cs.default VersionInfo.cs
@call copy.standard.cmd WebSites\Rainbow\web.config 
@call copy.standard.cmd Projects\Rainbow.Tests\App.config
@call copy.standard.cmd Tests\Rainbow.Tests.Data.MsSql\App.config
