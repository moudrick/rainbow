**nant.build** is developed for the project integrity control and support. It also has to be useful in developer's activity.

The integrity ingredients are:

  * auto-assign assebmly/file version info by current context values
  * auto-control successful project build
  * auto-control successful project tests, coverage and other Continuous Integration results
  * auto-create development envorinment
  * auto-create download packages
  * etc.

The file content and structure seems to be self-documented enough to be understood correctly. It is attained by purposeful containing objects naming and section comments.

Here are some rules and practices to use the file features on your local environment in the best way.

You can perform the following by running different targets of the nant.build:
  * create your local environment such as MS SQL databases and IIS web applications;
  * MS SQL databases can be created without running SQL server management studio (Enterprise manager for MS SQL 2000)
  * adjust your local configs to use this created environment;
  * build the sources without VS running
  * run the tests without other soft installed (except nant)

# General notes #

All targets can work both in subversioned working copies and unversioned (exported)

# Aliases #

Shortcuts for frequenly used action sets.


# Database works #

Database targets can work both with MS SQL 2k and MS SQL 2k5 database instances.
These targets are placed in **sql** section.

# Build targets #

## Version work targets ##

All native Rainbow assemblies are compiled using the single root VersionInfo.cs
To  nant buil.revision

### Assembly Version ###

Assembly version parts are composed in the following way.

MAJOR is hardcoded initially as 2.
MINOR is hardcoded initially as 1.

Their hardcode will be changed on version increment.

BUILD and REVISION value depends on the calling target way

BUILD is composed the following way
  * Build number on running from CCNet
  * 0 by default

REVISION
  * subversion Last Changes Rev number from **svn info** command for versioned working copies
  * subversion Last Changes Rev number from **downloads.svn.info.txt** file for unversioned copies
  * 0 by default

### File Version ###

File Version is used to control database structure version in Rainbow.Core assembly.
All other use the same File Version for uniformity.
File version is taken from the last Release node from **WebSites\Rainbow\Setup\Scripts\History.xml** file.

# apps #


# Deploying development environment #
For quality development all the developers would have similar
Since the developers and their initial environment are different, the deployed project environment should be customizeable.

The nant.build targets using gives a balance of these two polar requirements.

# Recommended developer's configuration #

# Recommended production configuration #

# Hosting services #
# Enterprise envorinment #

_...to be continued_
