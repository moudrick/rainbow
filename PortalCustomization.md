# Overview #

There are three main locations to change the look and feel of the web portal in the 2.0 version of Rainbow and below. – the built in portal controls (UI), theme, and the desktop layout.  Each of these has different things it can customize

Note: The 2.1 version of Rainbow uses Master Pages and is a different process to customize.

# Details #

## Built In Portal Controls (UI) ##
Internal portal controls are used for changing the logo, page title, desktop layout, theme, and more.  To get to the portal controls, log into the portal, go to the admin pages, and find the portal controls.  When editing a portal from the portal module, you can change the default logo, the default page title, default description, keywords, and etc.  It is also where you can choose the Theme and Desktop Layout (Page Layout) that your portal is using, as well as many other options.
## Theme ##
The themes are located in the \design\theme folder of Rainbow.  Each theme has it's own folder for the CSS, theme.xml, icons, and more.

**CSS** – The css file is where changes to colors are usually made, as well as format, locations, line wrapping, fonts, and much more.  Most of the themes contain a single CSS file with all the options, although a few themes do have multiple CSS files.  This is a standard Cascading Style Sheet page, and should be mostly documented in the CSS file itself.

**Theme.xml** – The theme.xml file is located in the \design\theme\ThemeName folder and contains customizable information about what graphics are used for buttons on the website, as well as some title formatting.  Historically, we have mostly only used this file for changing the button graphics on the site

**Icons** – The icons folder in the \design\theme\ThemeName\Icons folder contains all the icons used in a particular them.  It is the repository for the icons that the theme.xml file will point to.

## Desktop Layout ##
Desktop Layout is in the \Design\DesktopLayouts folder.  Each customized website will typically have its own desktop layout file with its own unique header and footer.  Typically all changes to a header or footer will be done in this area, including graphics and logos.  Many pages use the default pages that are in the root of the DesktopLayouts file, so it’s suggested that after making changes to a layout that the ascx files are copied to the desktoplayouts folder as well.  Don’t forget when you make changes to a layout to change the default ones located in the root of the desktop layout folder as well.

**DesktopDefault.ascx** – Although this is the default page, this one will probably never be changed or edited.  It is just a three part page that has a header control, the main body control (desktopthreepanes), and the footer.

**DesktopFooter.ascx** – This is the footer control.  Any changes to links at the bottom of the page or the graphics at the bottom of the page are changed in this file.

**DesktopPortalBanner.ascx** – This is the header control.  Any changes to the header other than changing the logo is made to this file.  Be careful not to remove anything that you don’t understand, as there are a number of important controls on this header that only show up when logged in.

**DesktopThreePanes.ascx** – This is the template for the main body.  This file is never changed, much like the desktopdefault.ascx

## Help ##

In addition, there is a help web page that comes up when you click the Help button in the top right corner of the portal.  It is designed to give you a good location to provide help for your website, or help on Rainbow Portal for your users.  Basically the help just opens an XML document and formats it using an XSLT.

**Rainbow.xml** - This file is located in the \rb\_documentation\rainbow folder.  Currently it just has a single page with information on it, but it can be turned into a fairly extensive help.  It's reasonably easy to change this xml file by looking at the help page as you compare with the xml.

**viewer.xslt** - This is the XSL Transform that arranges and displays the Rainbow.xml into the help web page.  It is located in the \rb\_documentation\xsl folder.


