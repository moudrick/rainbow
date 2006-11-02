<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<%@ Control language="c#" Inherits="Rainbow.DesktopModules.AddModule.AddPage" CodeBehind="AddTab.ascx.cs" AutoEventWireup="false" %>
<table align="center" border="0">
	<tr>
		<td vAlign="top" height="20"><tra:literal id="tabParentLabel" TextKey="AT_PAGEPARENT" Text="Page parent:" runat="server"></tra:literal></td>
		<td vAlign="top" height="20"><Asp:DropDownList id="parentTabDropDown" runat="server" CssClass="NormalTextBox" DataValueField="TabID"
				UseUniqueID="true" DataTextField="TabName" Width="200px"></Asp:DropDownList><tra:label id="lblErrorNotAllowed" TextKey="ERROR_NOT_ALLOWED_PARENT" runat="server" CssClass="Error"
				Visible="False" EnableViewState="False">Not allowed to choose that parent</tra:label></td>
	</tr>
	<tr>
		<td vAlign="top"><tra:literal id="tabVisibleLabel" TextKey="AM_PAGEVISIBLETO" Text="Page Visible To:" runat="server"></tra:literal><BR>
		</td>
		<td vAlign="top"><Asp:DropDownList id="PermissionDropDown" runat="server" Width="200px" Height="80" UseUniqueID="true"
				EnableViewState="False" CookieMemory="False" CssClass="NormalTextBox"></Asp:DropDownList></td>
	</tr>
	<tr>
		<td vAlign="top"><tra:literal id="lbl_ShowMobile" TextKey="AT_SHOWMOBILE" Text="Show Mobile:" runat="server"></tra:literal></td>
		<td vAlign="top"><asp:checkbox id="cb_ShowMobile" runat="server"></asp:checkbox></td>
	</tr>
	<tr>
		<td vAlign="top"><tra:literal id="lbl_MobileTabName" TextKey="AT_MOBILETABNAME" Text="Mobile Tab Name:" runat="server"></tra:literal></td>
		<td vAlign="top"><asp:textbox id="tb_MobileTabName" runat="server" Width="200px" MaxLength="50"></asp:textbox></td>
	</tr>
	<tr>
		<td vAlign="top"><tra:literal id="tabTitleLabel" TextKey="AT_TABTITLE" Text="Tab Title:" runat="server"></tra:literal></td>
		<td vAlign="top" noWrap><asp:textbox id="TabTitleTextBox" Text="New Tab" runat="server" EnableViewState="false" MaxLength="50"
				cssclass="NormalTextBox" width="200px">New Tab</asp:textbox>&nbsp;&nbsp;<tra:linkbutton id="AddTabButton" TextKey="AT_ADDTAB" runat="server" cssclass="CommandButton">Add this tab</tra:linkbutton></td>
	</tr>
	<tr>
		<td vAlign="top" colSpan="2"><tra:Literal id="Literal1" TextKey="AT_JUMPTOTAB" Text="Would you like to jump to the new tab?"
				runat="server"></tra:Literal>&nbsp;&nbsp;<asp:RadioButtonList id="rbl_JumpToTab" runat="server" Width="136px" RepeatDirection="Horizontal" Height="24px">
				<asp:ListItem Value="No" Selected="True">No</asp:ListItem>
				<asp:ListItem Value="Yes">Yes</asp:ListItem>
			</asp:RadioButtonList></td>
	</tr>
</table>
<div align="center" class="Error"><tra:Literal id="moduleError" runat="server" Visible="False" Text="There was an error adding this module. It has been logged."
		TextKey="AM_MODULEADDERROR" EnableViewState="False"></tra:Literal></div>