<%@ register src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" tagname="Banner"
    tagprefix="portal" %>
<%@ register src="~/Design/DesktopLayouts/DesktopFooter.ascx" tagname="Footer" tagprefix="foot" %>
<%@ page autoeventwireup="false" inherits="Rainbow.Content.Web.Modules.PagePropertyPage"
    language="c#" codefile="PropertyPage.aspx.cs" %>
<html>
<head id="Head1" runat="server">
</head>
<body id="Body1" runat="server">
    <form id="Form1" runat="server">
        <div id="zenpanes" class="zen-main">
            <div class="rb_DefaultPortalHeader">
                <portal:banner id="SiteHeader" runat="server" />
            </div>
            <div class="div_ev_Table">
                <table align="center" border="0" cellpadding="4" cellspacing="0">
                    <tr valign="top">
                        <td>
                            <table cellpadding="0" cellspacing="0" width="600">
                                <tr>
                                    <td align="left" class="Head" nowrap="nowrap">
                                        <rbfwebui:localize id="Literal1" runat="server" text="Module settings" textkey="MODULESETTINGS_SETTINGS">
                                        </rbfwebui:localize>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <hr noshade="noshade" size="1" />
                                        <rbfwebui:settingstable id="EditTable" runat="server">
                                        </rbfwebui:settingstable>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:placeholder id="PlaceHolderButtons" runat="server"></asp:placeholder>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
            <div>
                <foot:footer id="Footer" runat="server" />
            </div>
        </div>
    </form>
</body>
</html>
