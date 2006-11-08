<%@ page autoeventwireup="false" codefile="SecurityRoles.aspx.cs" inherits="Rainbow.Content.Web.Modules.SecurityRoles"
    language="c#" %>

<%@ register src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" tagname="Banner"
    tagprefix="portal" %>
<%@ register src="~/Design/DesktopLayouts/DesktopFooter.ascx" tagname="Footer" tagprefix="foot" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server"><title></title>
</head>
<body id="Body1" runat="server">
    <form id="Form1" runat="server">
        <div id="zenpanes" class="zen-main">
            <div class="rb_DefaultPortalHeader">
                <portal:banner id="Banner1" runat="server" showtabs="false" />
            </div>
            <div class="div_ev_Table">
                <table border="0" cellpadding="2" cellspacing="2" width="98%">
                    <tr>
                        <td colspan="2">
                            <table cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <td align="left">
                                        <span id="title" runat="server" class="Head">Role Membership</span>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <hr noshade="noshade" size="1" />
                                    </td>
                                </tr>
                            </table>
                            <rbfwebui:label id="Message" runat="server" cssclass="NormalRed">
                            </rbfwebui:label>
                        </td>
                    </tr>
                    <tr>
                        <td width="11">
                            &nbsp;
                        </td>
                        <td>
                            <table cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <td>
                                        <asp:textbox id="windowsUserName" runat="server" cssclass="NormalTextBox" text="DOMAIN\username"
                                            visible="False">
                                        </asp:textbox>
                                    </td>
                                    <td class="Normal" nowrap="nowrap">
                                        <rbfwebui:linkbutton id="addNew" runat="server" cssclass="CommandButton" textkey="ROLE_ADD_NEW_USER"
                                            visible="False">Create new user and add to role</rbfwebui:linkbutton>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:dropdownlist id="allUsers" runat="server" cssclass="NormalTextBox" datatextfield="Email"
                                            datavaluefield="ProviderUserKey">
                                        </asp:dropdownlist>
                                    </td>
                                    <td nowrap="nowrap">
                                        <rbfwebui:linkbutton id="addExisting" runat="server" cssclass="CommandButton" textkey="ROLE_ADD_USER">Add existing user to role</rbfwebui:linkbutton>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr valign="top">
                        <td width="11">
                            &nbsp;
                        </td>
                        <td nowrap="nowrap">
                            <asp:datalist id="usersInRole" runat="server" repeatcolumns="2">
                                <itemstyle width="225" />
                                <itemtemplate>
                                    &#160;&#160;
                                    <rbfwebui:imagebutton id="Imagebutton1" runat="server" alternatetext="Remove this user from role"
                                        commandname="delete" imageurl='<%# CurrentTheme.GetImage("Buttons_Delete", "Delete.gif").ImageUrl %>' />
                                    <rbfwebui:label id="Label1" runat="server" cssclass="Normal" text='<%# Container.DataItem %>'>
                                    </rbfwebui:label>
                                </itemtemplate>
                            </asp:datalist>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <hr noshade="noshade" size="1" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <rbfwebui:linkbutton id="saveBtn" runat="server" cssclass="CommandButton" textkey="ROLE_SAVE_CHANGES">Save Role Changes</rbfwebui:linkbutton>
                        </td>
                    </tr>
                </table>
            </div>
            <div class="rb_AlternatePortalFooter">
                <foot:footer id="Footer" runat="server" />
            </div>
        </div>
    </form>
</body>
</html>
