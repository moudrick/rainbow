<%@ Control Inherits="Rainbow.DesktopModules.ModuleDefsAll_OFM" CodeBehind="ModuleDefsAll.ascx.cs" Language="c#" AutoEventWireup="false" %>
<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<asp:datalist id="defsList" runat="server" DataKeyField="GeneralModDefID">
	<ItemTemplate>
		<span nowrap>
		&nbsp;<tra:ImageButton id="ImageButton1" runat="server" TextKey="EDIT_THIS_ITEM" AlternateText="Edit this item" ImageUrl='<%# CurrentTheme.GetImage("Buttons_Edit", "Edit.gif").ImageUrl %>' />
		&nbsp;<asp:Label id=Label1 runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "FriendlyName") %>' CssClass="Normal" />
		&nbsp;&nbsp;&nbsp;File: <%# DataBinder.Eval(Container.DataItem, "DesktopSrc") %>
		</span>
	</ItemTemplate>
</asp:datalist>