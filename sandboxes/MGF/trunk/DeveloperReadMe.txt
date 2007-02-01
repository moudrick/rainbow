Rainbow Portal 2007 DeveloperReadMe.txt

The Rainbow Portal 2007 team welcomes new developers!

To make it as simple as possible to get up and running with Rainbow, we've
provideded the following quick start instructions.

JOIN THE RAINBOW DEVELOPER TEAM

1. Contact Eric Ramseur at ramseur@gmail.com to request to join the team.
2. Eric will give you access to the http://community.rainbowportal.net/
   Rainbow Community. Eric will give you instructions on creating your 
   statement of work.  You will primarily use the Developer Forum at
   http://community.rainbowportal.net/forums/31/ShowForum.aspx to collaborate
   with the other developers.  You will also be able to maintain your own
   blog about the work you are doing on this site.
3. Join the Rainbow Recruits Yahoo Group at
   http://tech.groups.yahoo.com/group/RainbowRecruits/ to receive other 
   developer communications.
4. Eric will ask Alexey Moudrick at moudrick@gmail.com to grant you access to 
   the Rainbow Portal project at Novell Forge SVN site at
   http://forge.novell.com/modules/xfmod/project/?rainbow.  You will be able
   to browse the repository at 
   https://forgesvn1.novell.com/viewsvn/rainbow/ and use Subversion
   to access the repository at
   https://forgesvn1.novell.com/svn/rainbow/.  Alexey will create a sandbox for
   you and provide additional instructions on best practices.
5. Download and install Subversion from http://subversion.tigris.org/.
6. Download and install the Tortoise Subversion GUI at 
   http://tortoisesvn.tigris.org/.
7. Subversion documentation is available at http://svnbook.red-bean.com/.


CREATE A BRANCH FROM DEVINT

Rainbow developers must create a branch of the developer integration code at
https://forgesvn1.novell.com/svn/rainbow/RainbowDotNet2/devint/trunk/.

1. Create a branch remotely using the following Subversion command replacing
   the "<...>" areas with your sandbox name, the current devint revision
   number, and your user name: 
   svn copy https://forgesvn1.novell.com/svn/rainbow/RainbowDotNet2/devint/trunk/ https://forgesvn1.novell.com/svn/rainbow/sandboxes/<YourSandbox/branches/devint_trunk_<RevisionNumber>/ -m "making as branch of the devint trunk in the <Your User Name> sandbox" 
2. Create a local directory. For example:
   C:\svn\Rainbow\sandboxes\<YourSandboxName>\branches\devint_trunk_<RevisionNumber>.
3. Change to the local devint_trunk_xxx branch directory.
4. Do an svn check out (svn co) from 
   https://forgesvn1.novell.com/svn/rainbow/sandboxes/yogibar/branches/devint_trunk_634/
   to your local devint_trunk_xxx branch directory.
5. Do an svn update.
6. Do an svn info to verify.  Should look something link this

Path: .
URL: https://forgesvn1.novell.com/svn/rainbow/sandboxes/<YourSandbox>/branches/devint_trunk_<RevNo>
Repository Root: https://forgesvn1.novell.com/svn/rainbow
Repository UUID: 05d1f32f-6609-0410-a211-bd1a7dc92a9f
Revision: 646
Node Kind: directory
Schedule: normal
Last Changed Author: <YourUserId>
Last Changed Rev: 646
Last Changed Date: 2006-10-24 08:46:55 -0400 (Tue, 24 Oct 2006)

Note that you can use the Tortoise GUI to do the above also. Please contact
Alexey Moudrick at moudrick@gmail.com if you have questions about using 
Novell Forge, Subversion, or Tortoise to branch from devint and create your
local copy.  Alexey can will also help merge changes back into devint when
they are ready to be tested with other developer's code.


INSTALLING RAINBOW 2007 FOR DEVELOPMENT

1. Create a MS SQLSERVER 2000 or 2005 database for your Rainbow portal. You
   might name it RainbowDevint for example.
2. The WebSites\Rainbow\web.config.standard can be renamed to web.conf.
3. Update the web.conf file and ensure that you have valid connection strings
   to the database you just created.
4. Make sure you have VS 2005 installed with the Web Development Projects
   update from MSDN at http://msdn2.microsoft.com/en-us/asp.net/aa336619.aspx
5. Double-click on the Rainbow.sln file to open the solution in Visual Studio
   2005.
6. Click on OK if prompted about ASP.NET 2.0.
7. If necessary, delete the Projects\Rainbow.Tests\App.config file and replace
   it by renaming the App.config.standard to App.config.
8. Build the projects, the website, then the entire solution.
9. Be sure to set the privileges for ASP.NET user to access the 
   WebSites\Rainbow folder.  Certain folders require write access, 
   such as the WebSites\Rainbow\rb_logs folder.
10. Open Default.aspx and press ctrl-f5 to run the project.
11. To debug, you can click Debug > Attach to process... and select the
    aspnet_wp.exe process.
    

COORDINATING WITH OTHER DEVELOPERS

Please contact Eric Ramseur at ramseur@gmail.com to find out what the
priorities are and coordinate with other developers.  You can tell from forum
and blog activity at http://community.rainbowportal.net who is currently 
active.

Please contact Alexey Moudrick at moudrick@gmail.com to coordinate moving
code into devint.

Thanks again for joining the Rainbow Portal 2007 team!






   



   
