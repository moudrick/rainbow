<%@ Control Inherits="Rainbow.DesktopModules.Installer.PackageManager" CodeBehind="PackageManager.ascx.cs" Language="c#" AutoEventWireup="false" %>
<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
     <table cellSpacing=0 cellPadding=4 width="98%" border=0 
      >
        <tr vAlign=top>
          <td width=150>&nbsp; </td>
          <td width="100%">
            <table cellSpacing=0 cellPadding=3 width=750 
            border=0 runat="server" ID="Table1">
        <TR>
          <TD class=SubHead noWrap colSpan=5><tra:label id=Label1 runat="server" TextKey="INSTALLER_PACKAGE_MANAGEMENT" Text="Install Packages Management"></tra:label></TD></TR>
        <TR>
          <TD class=SubHead noWrap colSpan=5>
                  <hr noShade SIZE=1></TD></TR>
              <TR>
                <TD class=SubHead noWrap width=106></TD>
                <TD width=6></TD>
                <TD></TD>
                <TD width=10></TD>
                <TD class=Normal width=250></TD></TR>
              <TR>
                <TD class=SubHead noWrap width=106><tra:label id=Label7 runat="server" TextKey="INSTALLER_PACKAGE_FILES_INSTALL" Text="Install Package File"></tra:label>:</TD>
                <TD width=6></TD>
                <TD><asp:listbox id=InstallerFileList runat="server" AutoPostBack="True" Width="450px"></asp:listbox></TD>
                <TD width=10></TD>
                <TD class=Normal width=250></TD></TR>
              <TR>
                <TD class=SubHead noWrap width=106></TD>
                <TD width=6></TD>
                <TD></TD>
                <TD width=10></TD>
                <TD class=Normal width=250></TD></TR>
              <TR>
                <TD class=SubHead noWrap width=106><tra:label id=Label8 runat="server" TextKey="INSTALLER_PACKAGE_FILES_UNINSTALL" Text="Uninstall Package File"></tra:label></TD>
                <TD width=6></TD>
                <TD><asp:listbox id=UninstallerFileList runat="server" AutoPostBack="True" Width="450px"></asp:listbox></TD>
                <TD width=10></TD>
                <TD class=Normal width=250></TD></TR></table>
            <table id=tablePackageInfo cellSpacing=0 cellPadding=3 width=750 border=0 
             runat="server">
              <TR>
                <TD class=SubHead colSpan=2>
                  <HR noShade SIZE=1></TD></TR>
              <TR>
                <td class=SubHead width=116><tra:label id=Label2 runat="server" TextKey="INSTALLER_PACKAGEINFO_NAME" Text="Name"></tra:label></td>
                <td><asp:label id=lblPackageName runat="server"></asp:label></td></TR>
              <TR>
                <TD class=SubHead width=116><tra:label id=Label6 runat="server" TextKey="INSTALLER_PACKAGEINFO_VERSION" Text="Version"></tra:label></TD>
                <TD><asp:label id=lblPackageVersion runat="server"></asp:label>&nbsp;<asp:label id=lblIsBeta runat="server" CssClass="Error" Visible="False">BETA for testing only</asp:label></TD></TR>
              <TR>
                <TD class=SubHead width=116><tra:label id=Label3 runat="server" TextKey="INSTALLER_PACKAGEINFO_DESCRIPTION" Text="Description"></tra:label></TD>
                <TD><asp:label id=lblPackageDescription runat="server"></asp:label></TD></TR>
              <TR>
                <TD class=SubHead width=116><tra:label id="Label9" runat="server" TextKey="INSTALLER_PACKAGEINFO_AUTHOR" Text="Author"></tra:label></TD>
                <TD><asp:label id="lblPackageAuthor" runat="server"></asp:label></TD></TR>

              <TR>
                <TD class=SubHead width=116><tra:label id=Label4 runat="server" TextKey="INSTALLER_PACKAGEINFO_INFOURL" Text="more Information"></tra:label></TD>
                <TD><asp:HyperLink id=linkPackageInformationUrl runat="server" Target="_blank"></asp:HyperLink></TD></TR>
              <TR>
                <TD class=SubHead width=116><tra:label id=Label5 runat="server" TextKey="INSTALLER_PACKAGEINFO_TYPE" Text="Type of Package"></tra:label></TD>
                <TD><asp:label id=lblPackageType runat="server"></asp:label></TD></TR>
              <TR>
                <TD class=SubHead width=116></TD>
                <TD></TD></TR>
        <TR>
          <TD class=SubHead width=116>
<tra:label id=Label10 runat="server" Text="Type of Package" TextKey="INSTALLER_MODULES">Modules in  the Package</tra:label></TD>
          <TD>
<asp:Label id=lblModulesInPackage runat="server"></asp:Label></TD></TR>
              <TR>
                <TD class=SubHead colSpan=2>
                  <hr noShade SIZE=1>
                </TD></TR>
              <tr>
                <td class=SubHead vAlign=top width=116></td>
                <td></td></tr>
              </table>
              <table cellSpacingcellPadding=3 width=750 border=0 ="0">
              <TR>
                <TD class=SubHead vAlign=top width=116></TD>
                <TD><tra:linkbutton class=CommandButton id=InstallBtn runat="server" TextKey="INSTALL_PACKAGE" Text="Install Package" Enabled="False"></tra:linkbutton> 
<tra:linkbutton class=CommandButton id=UninstallBtn runat="server" TextKey="DELETE_PACKAGE" Text="Uninstall Package" CausesValidation="False" Enabled="False"></tra:linkbutton></TD></TR></table>
            <p><asp:label id=lblErrorDetail runat="server" cssclass="Error"></asp:label></p></td></tr></table>
