# Introduction #

[RainbowPortal](http://code.google.com/p/rainbow) Architecture main elements are:

  * Provider
  * Module


# Providers #

You can consider providers as both a point of customization and [Data Mapper](http://www.designpatternsfor.net/default.aspx?pid=22) design pattern applicaiton all-in-one.

You can override existing providers as a wrapper, or even develop a fully custom implementation.

Most providers implemented as [Singletones](http://www.dofactory.com/Patterns/PatternSingleton.aspx), so they contain `static ExactProvider Instanse { get; }` member.

The exact implementation to use in this member is set in the [web.config.standard](http://code.google.com/p/rainbow/source/browse/NET_2_0/devint/trunk/WebSites/Rainbow/web.config.standard) specific sections - <sub>http://rainbow.googlecode.com/svn/NET_2_0/devint/trunk/WebSites/Rainbow/web.config.standard</sub>.

**Rainbow 1.6** already has a pair of providers (`Rainbow.Web.SqlUrlBuilderProvider, Rainbow.Configuration.Log4NetLogProvider` in [Web.config](http://code.google.com/p/rainbow/source/browse/NET_2_0/devint/trunk/WebSites/Rainbow/web.config.standard) specific sections - <sub>http://rainbow.googlecode.com/svn/NET_1_1/trunk/Rainbow/Web.config</sub>)

More providers were extracted in **NET\_2\_0/devint/trunk** code (`PortalProvider, PortalPageProvider, MonitoringProvider, ModuleProvider`), and their standard default implementation for MsSql database (`Rainbow.Framework.Providers.MsSql` namespace/project). Some Module manipulation sql-dependent code (`using System.Data.SqlClient`) is still not isolated to provider implementation, it is to be done soon.

## Geographic provider ##

Geographic provider is used for getting Countries List and some other Geographic lists (see interface and usages)

It is used in **Register pages** (common base is `class RegisterFull` in [RegisterFull.ascx.cs](http://code.google.com/p/rainbow/source/browse/NET_2_0/devint/trunk/Projects/Rainbow.Framework.Web.UI.WebControls/Modules/RegisterFull.ascx.cs) file).
Also, it is well tested in Unit Tests (`class GeographicProviderTests`, see [GeographicProviderTests.cs](http://code.google.com/p/rainbow/source/browse/NET_2_0/devint/trunk/Tests/Rainbow.Tests.Data.MsSql/GeographicProviderTests.cs)). Or rather its default implementation `class SqlGeographicProvider` in [SqlGeographicProvider.cs](http://code.google.com/p/rainbow/source/browse/NET_2_0/devint/trunk/Providers/Rainbow.Framework.Providers.MsSql/SqlGeographicProvider.cs) is tested there.

# Modules #