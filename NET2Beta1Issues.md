#Issues for the .NET 2.0 Beta 1 Release

# Introduction #

This wiki page is for issues with Rainbow 2.0 Beta 1 Release


## Current Issues ##

### ASP.NET Ajax Considerations ###

Rainbow Portal 2.0 Beta 1 comes with ASP.NET Ajax RC.  You should make sure you install at least this version for the portal to function correctly.  If you install the final release of ASP.NET Ajax, some considerations should be followed.  This issue is cross-listed with this [blog](http://tinyurl.com/29rxku) entry and information can be found there.


### Connection Strings ###

The new beta 1 requires the use of 4 connection strings.  The reason for this is because the new beta 1 supports ASPNETDB as its abstracted Membership layer and uses various providers ( 4 ) to connect to this layer.  A common issue noted is the menu not showing up or a configuration issue with your connection string.  This is usually caused by a faulty connection string OR the use of the single quote ( ' ) incorrectly in a connection string.  Please use the below as a sample for connection strings in rainbow:


**Please Note:**  Always include the Application Name in ASPNETDB supported applications.  Many membership issues stem from the Application Name attribute not being set.

```
<connectionStrings>

    <add name="ConnectionString" connectionString="Data Source=.\SQLEXPRESS;database=Telerik.Rainbow;Integrated Security=True;Application Name=Rainbow" providerName="System.Data.SqlClient" />

    <add name="Providers.ConnectionString" connectionString="Data Source=.\SQLEXPRESS;database=Telerik.Rainbow;Integrated Security=True;Application Name=Rainbow"  providerName="System.Data.SqlClient"/>

    <add name="RainbowProviders.ConnectionString" connectionString="Data Source=.\SQLEXPRESS;database=Telerik.Rainbow;Integrated Security=True;Application Name=Rainbow" providerName="System.Data.SqlClient"/>

    <add name="Main.ConnectionString" connectionString="Data Source=.\SQLEXPRESS;database=Telerik.Rainbow;Integrated Security=True;Application Name=Rainbow" providerName="System.Data.SqlClient"/>
  
</connectionStrings>
```