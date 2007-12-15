Rainbow Portal 2.0 Beta 1 Developer Preview 

Changes :

Contributions by the MGF Team (Lead: Jose Arrarte) :


Added ASP.NET Ajax Support
Added ASPNETDB Support for Membership, Roles, Profiles, Web Parts
XHMTL is now on by default
Fixed many open Community issues
Organized Modules into two sections
Added Geographical Provider



Preview Deployment Instructions : 

I. DEPLOYMENT FOR PREVIEW USING nant.build SCRIPT

II.1 Requirements

I.1.1 Windows XP, Windows 2003 Server, Windows Vista
I.1.2 IIS 6.0
I.1.3 .Net Framework 2.0
I.1.4 MS SQL Server 2005 Express edition - (local)\SQLEXPRESS instance
I.1.5 nant 0.85 release (freeware opensource, http://nant.sf.net/)

I.2. Deployment

I.2.1 Download and unpack nant package if necessary. 
Add full path to bin/nant.exe from the package to your PATH environment variable.

I.2.2 Download and unpack this package.

I.2.3 Run "nant deploy.preview" or just "nant" command line from the root of the folder you have unpacked this package.

CAUTION!!! This action will automatically create new databases "Rainbow" and "tests-rainbow" and "Rainbow" web applcation.
The databases will be overwritten, so back up back up them if you have them and still need their content.

I.2.4 Open http://localhost/Roinbow link in your browser.


II. DEPLOYMENT FOR PREVIEW USING Visual Studio.

II.1 Requirements

II.1.1 Windows 2000, Windows 2000 Server, Windows XP, Windows 2003 Server, Windows Vista
II.1.2 IIS 6.0
II.1.3 .Net Framework 2.0
II.1.4 MS SQL Server 2005 - (local) instance
II.1.5 Visual Studio 2005 (8.0)

I.2. Deployment

I.2.1 Unpack this package.

I.2.3 Create IIS ASP.Net 2.0 web application named Rainbow in the default web site. 
Set Local Path o  the application to full name of WebSites\Rainbow\ subfolder in the unpacked folder.

I.2.3 Create database using WebSites\Rainbow\Setup\Scripts\setup.bat file.

CAUTION!!! This task will automatically create new "Rainbow" database!
The database will be overwritten, so back up it if you have it and still need its content.

I.2.4 Copy WebSites\Rainbow\web.config.standard file to WebSites\Rainbow\web.config, 
then find there all the phrases "CHANGE ME TO A VALID CONNECTION STRING" and change them to valid connection string.
In most preview deployment cases it should be literally "server=(local)\SQLEXPRESS;Trusted_Connection=true;database=Rainbow".
Also, you have to copy Projects\Rainbow.Tests\App.config.standard file to Projects\Rainbow.Tests\App.config.

I.2.5 Open Rainbow.sln file in Visual Studio 2005.

I.2.6 Build the solution by studio.

I.2.7 Run web site "Rainbow" from Webs solution folder.


Note : This is a Beta Release and should by no means be
       deployed into production as this is a Preview.  If
       the user of this Rainbow Beta 1 deploys in production,
       the Rainbow portal team nor its community will be 
       held liable for issues or downtime.  This software is for developers
       and CMS website owners who would like to see what
       is in store for Rainbow 2.0.

Known Issues:

   Little bugs with ASPNETDB will occur as we slowly integrate this 
new membership layer into Rainbow.  Zen doesnt work with rewrite currently
as code needs to be migrated + bugs fixed.  Please use the classic themes
for now.


Please report new issues : http://code.google.com/p/rainbow/issues/list

Make sure your issue isnt listed before you report.


Released by : Eric Ramseur
2-7-2007
