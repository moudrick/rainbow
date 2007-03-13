<%@ Control Language="c#" AutoEventWireup="false" Codebehind="Recycler.ascx.cs" Inherits="Rainbow.DesktopModules.Recycler" %>
<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<asp:DataGrid id="DataGrid1" Width="95%" HorizontalAlign="Center" AlternatingItemStyle-BackColor="#fff5c9" CssClass="Normal" AutoGenerateColumns="False" AllowSorting="true"	runat="server">
	<HeaderStyle CssClass="NormalBold"/>
	<Columns>
		<asp:TemplateColumn SortExpression="ModuleTitle">
			<HeaderTemplate>
				<asp:LinkButton ID="linkButton1" Runat="server" CommandName="Sort" CommandArgument="ModuleName">Module Name</asp:LinkButton>
			</HeaderTemplate>
			<ItemTemplate>
				<asp:HyperLink id="Hyperlink2" NavigateUrl='<%# Rainbow.HttpUrlBuilder.BuildUrl("~/DesktopModules/Recycler/view.aspx","mid=" + DataBinder.Eval(Container.DataItem,"ModuleID")) %>' runat="server" >
					<%#DataBinder.Eval(Container.DataItem,"ModuleTitle")%>
				</asp:HyperLink>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:BoundColumn SortExpression="DateDeleted" DataField="DateDeleted" HeaderText="Date Deleted" DataFormatString="{0:MM/dd/yy}"/>
		<asp:BoundColumn SortExpression="DeletedBy" DataField="DeletedBy" HeaderText="Deleted By"/>
		<asp:BoundColumn SortExpression="TabName" DataField="OriginalTabName" HeaderText="Original Tab"/>
	</Columns>
</asp:DataGrid>
