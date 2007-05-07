<%@ Register TagPrefix="foot" TagName="Footer" Src="~/Design/DesktopLayouts/DesktopFooter.ascx" %>
<%@ Register TagPrefix="portal" TagName="Banner" Src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" %>
<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<%@ Page Language="c#" codebehind="ArticlesView.aspx.cs" autoeventwireup="false" Inherits="Rainbow.DesktopModules.ArticlesView" %>
<HTML>
	<HEAD runat="server">
	</HEAD>
	<body runat="server">
		<form id="ArticlesView" method="post" runat="server">
			<div class="zen-main" id="zenpanes">
				<div class="rb_DefaultPortalHeader"><portal:banner id="SiteHeader" runat="server"></portal:banner></div>
				<div class="div_ev_Table">
					<table cellSpacing="0" cellPadding="0" width="80%">
						<tr >
							<TD width="10" rowSpan="4">&nbsp;</TD>
							<td><A class="Normal" href="javascript:history.go(-1)">&lt;&lt;
									<tra:literal id="goBackTop" runat="server" Text="Back" TextKey="BACK"></tra:literal></A></td>
							<td align="right">
								<tra:hyperlink id="editLink" Visible="<%# IsEditable %>" runat="server" Text="Edit" TextKey="EDIT"></tra:hyperlink>
							</td>
						</tr>
						<tr>
							<td class="Normal" colSpan="2">
								<P><asp:label id="Title" runat="server" CssClass="ItemTitle">&nbsp;</asp:label>&nbsp;
									<asp:label id="Subtitle" runat="server" CssClass="ItemTitle">&nbsp;</asp:label>&nbsp;
									<asp:label id="StartDate" runat="server" CssClass="ItemDate">&nbsp;</asp:label></P>
								<P><asp:label id="Description" runat="server" CssClass="Normal">&nbsp;</asp:label></P>
								<hr noShade SIZE="1">
								<P>
									<table cellSpacing="0" cellPadding="0" width="100%" border="0">
										<tr>
											<td><A class="Normal" href="javascript:history.go(-1)">&lt;&lt;
													<tra:literal id="goback" runat="server" Text="Back" TextKey="BACK"></tra:literal></A></td>
											<td class="Normal" align="right"><tra:literal id="CreatedLabel" runat="server" Text="Created by" TextKey="CREATED_BY"></tra:literal>&nbsp;
												<asp:label id="CreatedBy" runat="server"></asp:label>&nbsp;
												<tra:literal id="OnLabel" runat="server" Text="on" TextKey="ON"></tra:literal>&nbsp;
												<asp:label id="CreatedDate" runat="server"></asp:label></td>
										</tr>
									</table>
								<P></P>
							</td>
						</tr>
					</table>
				</div>
				<div class="rb_AlternatePortalFooter"><foot:footer id="Footer" runat="server"></foot:footer></div>
			</div>
		</form>
	</body>
</HTML>
