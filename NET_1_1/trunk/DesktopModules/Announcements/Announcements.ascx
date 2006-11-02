<%@ Control Language="c#" Inherits="Rainbow.DesktopModules.Announcements" CodeBehind="Announcements.ascx.cs" AutoEventWireup="false" %>
<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<%@ Register TagPrefix="cc2" Namespace="Rainbow.UI.WebControls" Assembly="Rainbow" %>
<asp:DataList id="myDataList" CellPadding="4" runat="server">
	<ItemTemplate>
		<tra:HyperLink TextKey="EDIT" Text="Edit" id="editLink" ImageUrl='<%# CurrentTheme.GetImage("Buttons_Edit", "Edit.gif").ImageUrl %>' NavigateUrl='<%# Rainbow.HttpUrlBuilder.BuildUrl("~/DesktopModules/Announcements/AnnouncementsEdit.aspx",PageID,"ItemID=" + DataBinder.Eval(Container.DataItem,"ItemID") + "&mid=" + ModuleID )%>' Visible="<%# IsEditable %>" runat="server" />
		<asp:Label cssclass="ItemTitle" Text='<%# DataBinder.Eval(Container.DataItem, "Title") %>' runat="server" />
		<br />
		<span class="Normal">
			<%# DataBinder.Eval(Container.DataItem,"Description") %>
			&nbsp;
			<tra:HyperLink id="moreLink" NavigateUrl='<%# DataBinder.Eval(Container.DataItem,"MoreLink") %>' Visible='<%# DataBinder.Eval(Container.DataItem,"MoreLink") != String.Empty %>' runat="server" TextKey="READ_FULL_ANNOUNCE" Text="Read full announce" />
		</span>
		<br />
	</ItemTemplate>
</asp:DataList>
<div align="center"><cc2:Paging id="pgModules" runat="server" /></div>