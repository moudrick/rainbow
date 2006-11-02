<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<%@ Control Language="c#" Inherits="Rainbow.DesktopModules.ArticlesInline" codebehind="ArticlesInline.ascx.cs" autoeventwireup="false" %>
<asp:datalist id="myDataList" runat="server" CellPadding="4" Width="100%">
	<ItemTemplate>
		<DIV class="Normal">
			<DIV>
				<tra:HyperLink id=editLink runat="server" Visible="<%# IsEditable %>" NavigateUrl='<%# Rainbow.HttpUrlBuilder.BuildUrl("~/DesktopModules/Articles/ArticlesEdit.aspx",PageID,"ItemID=" + DataBinder.Eval(Container.DataItem,"ItemID") + "&amp;mid=" + ModuleID )%>' ImageUrl='<%# CurrentTheme.GetImage("Buttons_Edit", "Edit.gif").ImageUrl %>' Text="Edit" TextKey="EDIT">
				</tra:HyperLink>
				<asp:Label id=StartDate runat="server" Visible="<%# ShowDate %>" Text='<%# DataBinder.Eval(Container.DataItem,"StartDate", "{0:d}") %>' CssClass="ItemDate">
				</asp:Label>
				<asp:Label id=Separator runat="server" Visible="<%# ShowDate %>">&#160;-&#160;</asp:Label>
				<asp:LinkButton cssClass="Head" id=Title runat="server" Visible='<%# (bool) (DataBinder.Eval(Container.DataItem, "Description").ToString().Length != 0) %>' CommandName='View' CommandArgument='<%# DataBinder.Eval(Container.DataItem,"ItemID")%>' Text='<%# DataBinder.Eval(Container.DataItem,"Title") %>'>
				</asp:LinkButton><br>
				<asp:Label id=SubTitle cssClass="SubHead" runat="server" Visible='<%# ((string)DataBinder.Eval(Container.DataItem,"Subtitle")).Length > 0 %>' Text='<%# DataBinder.Eval(Container.DataItem,"SubTitle") %>'>
				</asp:Label>&nbsp;
				<asp:Label id="Expired" Visible='<%# DataBinder.Eval(Container.DataItem,"Expired").ToString() == "1"  %>' runat="server" CssClass="Error" Text='Expired'/>
			</DIV>
			<DIV class="NormalItalic" style="MARGIN-TOP: 6px; margin-botton: 6px"><%# DataBinder.Eval(Container.DataItem,"Abstract").ToString().Replace("\n","<br>") %></DIV>
		</DIV>
	</ItemTemplate>
</asp:datalist>
<asp:Panel ID="ArticleDetail" runat="server" Visible="False">
	<TABLE cellSpacing="0" cellPadding="0" width="100%">
		<TR>
			<TD height="19">
				<tra:LinkButton id="goBackTop" runat="server" TextKey="BACK" Text="Back"></tra:LinkButton></TD>
			<TD align="right" height="19">
				<tra:hyperlink id=editLinkDetail runat="server" Visible="<%# IsEditable %>" TextKey="EDIT" Text="Edit">
				</tra:hyperlink></TD>
		</TR>
		<TR>
			<TD class="Normal" colSpan="2">
				<P>
					<asp:Label id="TitleDetail" runat="server" cssClass="Head"></asp:Label><BR>
					<asp:Label id="SubtitleDetail" runat="server" cssClass="SubHead"></asp:Label><BR>
					<asp:Label id="StartDateDetail" runat="server" CssClass="ItemDate">&nbsp;</asp:Label></P>
				<P></P>
				<asp:Label id="Description" runat="server" CssClass="Normal">&nbsp;</asp:Label>
				<P></P>
				<HR noShade SIZE="1">
				<P>
					<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
						<TR>
							<TD>
								<tra:LinkButton id="goBackBottom" runat="server" TextKey="BACK" Text="Back"></tra:LinkButton></TD>
							<TD class="Normal" align="right">
								<tra:Literal id="CreatedLabel" runat="server" TextKey="CREATED_BY" Text="Created by"></tra:Literal>&nbsp;
								<asp:label id="CreatedBy" runat="server"></asp:label>&nbsp;
								<tra:Literal id="OnLabel" runat="server" TextKey="ON" Text="on"></tra:Literal>&nbsp;
								<asp:label id="CreatedDate" runat="server"></asp:label></TD>
						</TR>
					</TABLE>
				</P>
			</TD>
		</TR>
	</TABLE>
</asp:Panel>
