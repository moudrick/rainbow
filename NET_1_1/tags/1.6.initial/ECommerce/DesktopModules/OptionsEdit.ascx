<%@ Control Language="c#" AutoEventWireup="false" Codebehind="OptionsEdit.ascx.cs" Inherits="Rainbow.ECommerce.DesktopModules.OptionsEdit" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<asp:textbox id="newOptionBox" runat="server" Width="240px" CssClass="NormalTextBox"></asp:textbox>
<tra:button id="btnAddOption" runat="server" TextKey="PRODUCT_ADD_OPTION" Text="Add Option"></tra:button>
<TABLE id="OptionsTable" cellSpacing="0" cellPadding="1" border="0">
	<TR>
		<TD noWrap><asp:listbox id="lbOptions" runat="server" Width="240px" CssClass="NormalTextBox"></asp:listbox></TD>
		<TD noWrap><tra:button id="btnDeleteOption" runat="server" TextKey="PRODUCT_DELETE_OPTION" Text="Delete"></tra:button></TD>
	</TR>
</TABLE>
