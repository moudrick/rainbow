<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<%@ Control language="c#" Inherits="Rainbow.DesktopModules.SecureDocuments" CodeBehind="SecureDocuments.ascx.cs" AutoEventWireup="false" %>
<asp:datagrid id="myDataGrid" BorderWidth="0px" width="100%" AutoGenerateColumns="false" EnableViewState="false"
	runat="server" AllowSorting="True" >
	<Columns>
		<asp:BoundColumn DataField="ItemID" Visible="False" />
		<asp:TemplateColumn ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="50" ItemStyle-Width="50">
			<ItemTemplate>
				<tra:HyperLink TextKey="EDIT" Text="Edit" id="editLink" ImageUrl='<%# CurrentTheme.GetImage("Buttons_Edit", "Edit.gif").ImageUrl %>'
					NavigateUrl='<%# Rainbow.HttpUrlBuilder.BuildUrl("~/DesktopModules/SecureDocuments/SecureDocumentsEdit.aspx","ItemID=" + DataBinder.Eval(Container.DataItem,"ItemID")  + "&mid=" + ModuleID) %>' runat="server" />
			</ItemTemplate>
		</asp:TemplateColumn>
		
		<tra:TemplateColumn SortExpression="FileFriendlyName" TextKey="TITLE" HeaderText="Title" HeaderStyle-CssClass="NormalBold"
					HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
			<ItemTemplate>
				<asp:label id="docLink" Text='<%# DataBinder.Eval(Container.DataItem,"FileFriendlyName") %>' runat="server" />
			</ItemTemplate>
		</tra:TemplateColumn>
		<tra:BoundColumn SortExpression="CreatedByUser" TextKey="DOCUMENT_OWNER" HeaderText="Owner" DataField="CreatedByUser"
			ItemStyle-CssClass="Normal" HeaderStyle-Cssclass="NormalBold" />
		<tra:BoundColumn SortExpression="Category" TextKey="DOCUMENT_AREA" HeaderText="Area" DataField="Category"
			ItemStyle-Wrap="false" ItemStyle-CssClass="Normal" HeaderStyle-Cssclass="NormalBold" />
		<tra:BoundColumn SortExpression="CreatedDate" TextKey="DOCUMENT_LAST_UPDATED" HeaderText="Last Updated"
			DataField="CreatedDate" DataFormatString="{0:d}" ItemStyle-CssClass="Normal" HeaderStyle-Cssclass="NormalBold" />
	</Columns>
</asp:datagrid>
