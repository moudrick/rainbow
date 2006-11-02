<%@ Register TagPrefix="foot" TagName="Footer" Src="~/Design/DesktopLayouts/DesktopFooter.ascx" %>
<%@ Register TagPrefix="portal" TagName="Banner" Src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" %>
<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<%@ Page language="c#" Codebehind="ArchiveView.aspx.cs" AutoEventWireup="false" Inherits="Rainbow.DesktopModules.ArchiveView" %>
<html>
	<head runat="server"></head>
	<body runat="server">
		<form id="ArchiveView" method="post" runat="server">
			<div class="zen-main" id="zenpanes">
				<div class="rb_DefaultPortalHeader">
					<portal:banner id="SiteHeader" runat="server"></portal:banner>
				</div>
				<div class="div_ev_Table">
					<div class="div_blog">
						<div class="div_blog_stats">
							<b><tra:Literal id="SyndicationLabel" runat="server" Text="Syndication" TextKey="BLOG_SYNDICATION"></tra:Literal></b><br />
							<a id="lnkRSS" runat="server" href="/DesktopModules/Blog/rss.aspx"><img id="imgRSS" runat="server" src="/rainbow/DesktopModules/Blog/xml.gif" border="0" /></a>
							<br /><br />
							<b><tra:Literal id="StatisticsLabel" runat="server" Text="Statistics" TextKey="BLOG_STATISTICS"></tra:Literal></b><br />
							<asp:Label ID="lblEntryCount" Runat="server"></asp:Label><br />
							<asp:Label ID="lblCommentCount" Runat="server"></asp:Label><br />
							<br />
							<b><tra:Literal id="ArchivesLabel" runat="server" Text="Archives" TextKey="BLOG_ARCHIVES"></tra:Literal></b><br />
							<asp:Repeater id="dlArchive" EnableViewState="False" runat="server">
								<HeaderTemplate>
									<ul>
								</HeaderTemplate>
								<ItemTemplate><li>
									<asp:HyperLink id="Hyperlink4" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"MonthName") 
																+ ", " +  DataBinder.Eval(Container.DataItem,"Year")
																+ " (" + DataBinder.Eval(Container.DataItem,"Count") + ")"%>' Visible='True' NavigateUrl='<%# Rainbow.HttpUrlBuilder.BuildUrl("~/DesktopModules/Blog/ArchiveView.aspx",PageID,
									"&month=" + DataBinder.Eval(Container.DataItem,"Month") 
									+ "&year=" + DataBinder.Eval(Container.DataItem,"Year")
									+ "&mid=" + ModuleID )%>'>
									</asp:HyperLink>&nbsp;</li>
								</ItemTemplate>
								<FooterTemplate>
									</ul>
								</FooterTemplate>
							</asp:Repeater>
						</div>
						<div class="div_blog_messeges">
							<asp:Label id="lblHeader" Visible="True" runat="server" CssClass="BlogTitle" Text='' />
							<br />
							<br />
							<asp:datalist id="myDataList" runat="server" CellPadding="4" EnableViewState="False" Width="100%">
								<HeaderTemplate>
									<div style="border-bottom:4px dotted blue;padding:2px;">
										<table border="0" class="normalbold"><tr>
											<td width="99%">Title</td>
											<td width="200" nowrap><tra:Label id="Label1" runat="server" TextKey="BLOG_POSTED">posted</tra:Label></td>
											<td width="120" nowrap>FeedBack</td>
										</tr></table>
									</div>
								</HeaderTemplate>
								<ItemTemplate>
									<div class="div_blog_archive_row">
										<table border="0"><tr>
											<td width="99%"><asp:HyperLink id="Title" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"Title") %>' Visible="True" NavigateUrl='<%# Rainbow.HttpUrlBuilder.BuildUrl("~/DesktopModules/Blog/BlogView.aspx",PageID,"ItemID=" + DataBinder.Eval(Container.DataItem,"ItemID") + "&amp;mid=" + ModuleID )%>'>
											</asp:HyperLink>
											</td>
											<td width="200" nowrap><asp:HyperLink id="Hyperlink1" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"StartDate", "{0:dddd MMMM d yyyy hh:mm tt}") %>' Visible="True" NavigateUrl='<%# Rainbow.HttpUrlBuilder.BuildUrl("~/DesktopModules/Blog/BlogView.aspx",PageID,"ItemID=" + DataBinder.Eval(Container.DataItem,"ItemID") + "&amp;mid=" + ModuleID )%>'>
											</asp:HyperLink></td>
											<td width="120" nowrap><asp:HyperLink id="Hyperlink2" runat="server" Text='<%# Feedback + DataBinder.Eval(Container.DataItem,"CommentCount") + ")" %>' Visible="True" NavigateUrl='<%# Rainbow.HttpUrlBuilder.BuildUrl("~/DesktopModules/Blog/BlogView.aspx",PageID,"ItemID=" + DataBinder.Eval(Container.DataItem,"ItemID") + "&amp;mid=" + ModuleID )%>'>
											</asp:HyperLink></td>
										</tr></table>
									</div>
								</ItemTemplate>
								<FooterTemplate>
									</table>
								</FooterTemplate>
							</asp:datalist>
						</div>
						<div class="copyright">
							<asp:Label ID="lblCopyright" Runat="server" CssClass="Normal"></asp:Label>
						</div>
					</div>
				</div>
				<div class="rb_AlternatePortalFooter"><foot:Footer id="Footer" runat="server"></foot:Footer></div></foot:Footer>
			</div>
		</form>
	</body>
</html>
