Rainbow Package Installer V2
version 0.0.1 - 11.01.2005
 Mario[at]Hartmann[dot]net


Definition:
Package - is a various amount of files for installing and unninstalling a Rainbow Module, Rainbow Theme, Rainbow Layout, Rainbow Custom Extension
Rainbow Package Definition File - is a xml file to be verified against the Rainbow Installer schema wich defines all steps to be done for installing and uninstalling Rainbow Packages
Rainbow Package - is a zip file including all files needed for a install and uninstall of a package.

Read Spec at
http://support.rainbowportal.net/confluence/display/DOX/Rainbow+modules+installer+2.0


Before Installing and using it.

ensure there is this folder structure available:

<yourRainbowRoot>\Installer\
<yourRainbowRoot>\Installer\Install (read&write access granted for the rainbow application user usually ASPNET or NetworkServices)
<yourRainbowRoot>\Installer\Uninstall (read&write access granted for the rainbow application user usually ASPNET or NetworkServices)

copy \Rainbow.Installer\app_support\rbd.2.0.xsd file to <yourRainbowRoot>\app_support\Installer:

(you may add this file to your schema folder of Visual Studio to ensure you have intellisense support for the definition files while generation it manually. it is usually located at \Program files\Microsoft Visual Studio .NET 2003\Common7\Packages\schemas)

you may run the prepareMyRainbow.cmd to ensure files are copied and folders are created. :-)

after buiding the project you have to register the Rainbow Installer Module in your Rainbow.
go to current Rainbow Moudule Install page and add the module by pointing to 'DesktopModules/Installer/install.xml'.
now you have a new module (you must add it to a page) for adding packages.


you want to chat with me?

mail: mario[at]hartmann[dot]net
MSN-Id: mario[at]hartmann[dot]net
Skype: mario[dot]hartmann