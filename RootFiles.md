These files are available in cheap copies created after [revision 215](https://code.google.com/p/rainbow/source/detail?r=215). To merge them into copies created earlier you can use the following command line:

`svn merge -r 209:215 https://rainbow.googlecode.com/svn/NET_2_0/devint/trunk %YOUR_LOCAL_WORKING_COPY_ROOT%`

**IMPORTANT!!!**

**Please skip or discard these files changes on merge your sandbox changes into devint trunk or any other public trunk (such as stage or prod)**

### nant.build ###

  * Included to repository.
  * Standard build script.
  * The best practice is to avoid any changes to this file even in your sandbox (except of standard action suggestion)
  * Includes targets with all required actions and standard action sets.
  * Use http://nant.sourceforge.net/ to run it. Tested on 0.85 Release version.
  * Customizeable (see below).
  * Can be run both in svn checked out (versioned) working copy and in svn exported (unversioned) copy.

If you want to perform one of the following things:
  * overload/override the actions from this file
  * create your own action set
  * suggest a standard action / action set
please use described below **default.build** file for this.

See detailes at NantBuild. Any suggestions to improve the build flow are welcome.

### ccnet.config.include ###

  * Included to repository.
  * The best practice is to avoid any changes to this file even in your sandbox (except of suggestion to include in standard)
  * Contains project section to be included in ccnet.config with minor changes.
  * If your cheap copy (trunk/branch) things are considered as important for the whole team, you will be given a CCNet resource at http://source.iocluster.com/
with configuration based on this file for whole team monitoring your copy status.

### default.build ###

  * NOT included to repository!!!
  * Ignored by directory svn:ignore property.
  * The best practice is to avoid committing this file.
  * Local custom changes are allowed and sometimes even required
  * Custom initialization of standard build process.
  * Any custom actions for local build steps automation.

### default.build.standard ###

  * Included to repository.
  * To be manually renamed to **default.build** to impact the build process.

### VersionInfo.cs ###

  * NOT included to repository!!!
  * The best practice is to avoid committing this file.
  * To be created by standard build process (see below).

**IMPORTANT!!!**
**Included in most projects to sync version info with no update duplication.**
**To create the file without running the whole standard build process use command line `nant build.version`**

### VersionInfo.cs.standard ###

  * Included to repository.
  * The best practice is to avoid any changes to this file even in your sandbox
  * To be copied with filter chains substitution by standard build process to **VersionInfo.cs**.

### CreateIISApp.cmd and CreateIISApp.vbs ###

  * Included to repository.
  * Do not change these files.
  * Contain code to automatically create web applications in local IIS.
  * To be fully integrated to nant.build.

