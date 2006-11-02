<%@ Register Src="~/Design/DesktopLayouts/DesktopFooter.ascx" TagName="Footer" TagPrefix="foot" %>
<%@ Register Src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" TagName="Banner"
    TagPrefix="portal" %>

<%@ Page AutoEventWireup="false" CodeFile="ModuleSettings.aspx.cs" Inherits="Rainbow.Admin.ModuleSettingsPage"
    Language="c#" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>

</head>
<body id="Body1" runat="server"> 
    <form id="Form1" runat="server">
        <div id="zenpanes" class="zen-main">
            <div class="rb_DefaultPortalHeader">
                <portal:Banner ID="Banner1" runat="server" showtabs="false" />
            </div>
            <div class="div_ev_Table">
                <table border="0" cellpadding="4" cellspacing="0" width="98%">
                    <tr valign="top">
                        <td width="150">&nbsp;</td>
                        <td width="*">
                            <table border="0" cellpadding="2" cellspacing="1">
                            <cols><col width="200" /><col width="400" /></cols>
                                <tr>
                                    <td colspan="4" class="Head" height="20">
                                        <rbfwebui:Localize ID="Literal1" runat="server" Text="Module base settings" TextKey="MODULESETTINGS_BASE_SETTINGS">
                                        </rbfwebui:Localize>
                                        <hr noshade="noshade" size="1" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" height="50">
                                        <rbfwebui:Localize ID="Literal2" runat="server" Text="Module type" TextKey="MODULESETTINGS_MODULE_TYPE">
                                        </rbfwebui:Localize>: </td>
                                    <td colspan="3" nowrap="nowrap" height="38">&nbsp;<rbfwebui:Label ID="moduleType" runat="server"
                                        CssClass="NormalBold" Width="300"></rbfwebui:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead">
                                        <rbfwebui:Localize ID="Literal18" runat="server" Text="Module name" TextKey="MODULESETTINGS_MODULE_NAME">
                                        </rbfwebui:Localize>: </td>
                                    <td colspan="3">&nbsp;<asp:TextBox ID="moduleTitle" runat="server" CssClass="NormalTextBox"
                                        Width="300"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead">
                                        <rbfwebui:Localize ID="Literal3" runat="server" Text="Cache Timeout" TextKey="MODULESETTINGS_CACHE_TIMEOUT">
                                        </rbfwebui:Localize>: </td>
                                    <td colspan="3">&nbsp;<asp:TextBox ID="cacheTime" runat="server" CssClass="NormalTextBox"
                                        Width="100"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead">
                                        <rbfwebui:Localize ID="Literal13" runat="server" Text="Move to tab" TextKey="MODULESETTINGS_MOVE_TO_TAB">
                                        </rbfwebui:Localize>:</td>
                                    <td colspan="3">&nbsp;<asp:DropDownList ID="tabDropDownList" runat="server" CssClass="NormalTextBox"
                                        DataSource="<%# portalTabs %>" DataTextField="Name" DataValueField="ID" Width="300px">
                                    </asp:DropDownList></td>
                                </tr>
                                <tr>
                                    <td>&nbsp; </td>
                                    <td colspan="3">
                                        <hr noshade="noshade" size="1" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top">
                                        <rbfwebui:Localize ID="Literal5" runat="server" Text="Roles that can view" TextKey="MODULESETTINGS_ROLE_VIEW">
                                        </rbfwebui:Localize>: </td>
                                    <td colspan="3" nowrap="nowrap">
                                        <asp:CheckBoxList ID="authViewRoles" runat="server" CellPadding="0" CellSpacing="0"
                                            CssClass="Normal" RepeatColumns="2">
                                        </asp:CheckBoxList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top">
                                        <rbfwebui:Localize ID="Literal4" runat="server" Text="Roles that can edit" TextKey="MODULESETTINGS_ROLES_EDIT">
                                        </rbfwebui:Localize>:</td>
                                    <td colspan="3" nowrap="nowrap">
                                        <asp:CheckBoxList ID="authEditRoles" runat="server" CellPadding="0" CellSpacing="0"
                                            CssClass="Normal" RepeatColumns="2">
                                        </asp:CheckBoxList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top">
                                        <rbfwebui:Localize ID="Literal6" runat="server" Text="Roles that can add" TextKey="MODULESETTINGS_ROLES_ADD">
                                        </rbfwebui:Localize>: </td>
                                    <td colspan="3" nowrap="nowrap">
                                        <asp:CheckBoxList ID="authAddRoles" runat="server" CellPadding="0" CellSpacing="0"
                                            CssClass="Normal" RepeatColumns="2">
                                        </asp:CheckBoxList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top">
                                        <rbfwebui:Localize ID="Literal7" runat="server" Text="Roles that can delete" TextKey="MODULESETTINGS_ROLES_DELETE">
                                        </rbfwebui:Localize>: </td>
                                    <td colspan="3" nowrap="nowrap">
                                        <asp:CheckBoxList ID="authDeleteRoles" runat="server" CellPadding="0" CellSpacing="0"
                                            CssClass="Normal" RepeatColumns="2">
                                        </asp:CheckBoxList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top">
                                        <rbfwebui:Localize ID="Literal8" runat="server" Text="Roles that can edit properties"
                                            TextKey="MODULESETTINGS_ROLES_EDIT_COLLECTION">
                                        </rbfwebui:Localize>: </td>
                                    <td colspan="3" nowrap="nowrap">
                                        <asp:CheckBoxList ID="authPropertiesRoles" runat="server" CellPadding="0" CellSpacing="0"
                                            CssClass="Normal" RepeatColumns="2">
                                        </asp:CheckBoxList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top">
                                        <rbfwebui:Localize ID="Literal16" runat="server" Text="Roles that can move modules"
                                            TextKey="MODULESETTINGS_ROLES_MOVE_MODULES">
                                        </rbfwebui:Localize>: </td>
                                    <td colspan="3" nowrap="nowrap">
                                        <asp:CheckBoxList ID="authMoveModuleRoles" runat="server" CellPadding="0" CellSpacing="0"
                                            CssClass="Normal" RepeatColumns="2">
                                        </asp:CheckBoxList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top">
                                        <rbfwebui:Localize ID="Literal17" runat="server" Text="Roles that can delete modules"
                                            TextKey="MODULESETTINGS_ROLES_DELETE_MODULES">
                                        </rbfwebui:Localize>: </td>
                                    <td colspan="3" nowrap="nowrap">
                                        <asp:CheckBoxList ID="authDeleteModuleRoles" runat="server" CellPadding="0" CellSpacing="0"
                                            CssClass="Normal" RepeatColumns="2">
                                        </asp:CheckBoxList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>&nbsp; </td>
                                    <td colspan="3">
                                        <hr noshade="noshade" size="1" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top">
                                        <rbfwebui:Localize ID="Literal9" runat="server" Text="Enable workflow" TextKey="MODULESETTINGS_SUPPORT_WORKFLOW">
                                        </rbfwebui:Localize>: </td>
                                    <td colspan="3">
                                        <asp:CheckBox ID="enableWorkflowSupport" runat="server" AutoPostBack="True" /></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top">
                                        <rbfwebui:Localize ID="Literal10" runat="server" Text="Approve roles" TextKey="MODULESETTINGS_ROLES_APPROVING">
                                        </rbfwebui:Localize>: </td>
                                    <td colspan="3" nowrap="nowrap">
                                        <asp:CheckBoxList ID="authApproveRoles" runat="server" CellPadding="0" CellSpacing="0"
                                            CssClass="Normal" RepeatColumns="2">
                                        </asp:CheckBoxList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" valign="top">
                                        <rbfwebui:Localize ID="Literal11" runat="server" Text="Publishing roles" TextKey="MODULESETTINGS_ROLES_PUBLISHING">
                                        </rbfwebui:Localize>: </td>
                                    <td colspan="3" nowrap="nowrap">
                                        <asp:CheckBoxList ID="authPublishingRoles" runat="server" CellPadding="0" CellSpacing="0"
                                            CssClass="Normal" RepeatColumns="2">
                                        </asp:CheckBoxList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>&nbsp; </td>
                                    <td colspan="3">
                                        <hr noshade="noshade" size="1" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" nowrap="nowrap">
                                        <rbfwebui:Localize ID="Literal12" runat="server" Text="Show to mobile users" TextKey="SHOWMOBILE">
                                        </rbfwebui:Localize>: </td>
                                    <td colspan="3">
                                        <asp:CheckBox ID="ShowMobile" runat="server" CssClass="Normal" /></td>
                                </tr>
                                <tr>
                                    <td>&nbsp; </td>
                                    <td colspan="3">
                                        <hr noshade="noshade" size="1" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="SubHead" nowrap="nowrap">
                                        <rbfwebui:Localize ID="Literal14" runat="server" Text="Show on every page?" TextKey="MODULESETTINGS_SHOW_EVERYWHERE">
                                        </rbfwebui:Localize>: </td>
                                    <td colspan="3">
                                        <asp:CheckBox ID="showEveryWhere" runat="server" CssClass="Normal" /></td>
                                </tr>
                                <tr>
                                    <td class="SubHead" nowrap="nowrap">
                                        <rbfwebui:Localize ID="Literal15" runat="server" Text="Can collapse window?" TextKey="MODULESETTINGS_SHOW_COLLAPSABLE">
                                        </rbfwebui:Localize>: </td>
                                    <td colspan="3">
                                        <asp:CheckBox ID="allowCollapsable" runat="server" CssClass="Normal" /></td>
                                </tr>
                                <tr>
                                    <td>&nbsp; </td>
                                    <td colspan="3">
                                        <hr noshade="noshade" size="1" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>&nbsp; </td>
                                    <td colspan="3">
                                        <asp:PlaceHolder ID="PlaceHolderButtons" runat="server"></asp:PlaceHolder>
                                    </td> 
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
            <div class="rb_AlternatePortalFooter">
                <foot:Footer ID="Footer" runat="server" />
            </div>
        </div>
    </form>
</body>
</html>
