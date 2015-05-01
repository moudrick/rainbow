# Software Requirements #

Rainbow Portal is a .Net 2.0 Open Source Content Management System based on ASP.NET, C# and SQL Server. It has the following minimum software requirements to be run as a web application:

  * Microsoft IIS (Internet Management Server) 5.0 or later**:
> _(Rainbow can also be run using Cassini Personal Web Server. This method is not discussed in this document)_**

  * Microsoft .NET Framework 2.0

  * A minimum of SQL Server Express 2005 installed on the application server

> OR

  * Microsoft SQL Server 2000 or greater

Rainbow Portal is not designed to function with a Microsoft Access Database (97/XP/2003/2007) and is not supported.

# Installation #

## New Setup ##
**Create an empty database called Rainbow (or whatever you wish) in the above database options**

**Give a user dbo access to that database ( this will change to lower access)**

**Change the connection string in web.config with your user and db
> Please note that if you are using the 2.0 version ( not 2.1 or beta 2.1 ),
> the connection string is in**

&lt;appsettings&gt;

 and not the new
> Connection Strings methodology.  This feature was added in the 2.1 version.

**Control - F5 in VS 2005 OR browse to the directory configured in IIS**

## Updating database ##
**Backup your database and files**

**Extract the new 2.0 files to a location.  Please note that you must convert your custom modules to 2.0 in order for them to work correctly.**

**Point the 2.0 web.config to your old rainbow database.**

**Browse to default.aspx which should prompt ( a redirect to update.aspx) an update**

**Click the button to apply the update**

Issues with updating

You may have issues with updating if you heavily modified Rainbow or added modules which dont work on the .NET 2.0 framework.  Also, the scripts to update your database might conflict if you changed your db from the standard.  These issues will be logged in your rb\_logs folder.  You can disable the install/upgrade of a module/rainbow feature by editing the History.xml file in Setup\Scripts.  This way, if there is a minor issue, you can skip its script and install the rest to complete the update.