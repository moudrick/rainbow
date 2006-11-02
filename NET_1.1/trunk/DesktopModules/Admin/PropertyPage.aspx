<%@ Import Namespace="Esperantus" %>
<%@ Register TagPrefix="cc1" Namespace="Rainbow.Configuration" Assembly="Rainbow" %>
<%@ Register TagPrefix="portal" TagName="Banner" Src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" %>
<%@ Register TagPrefix="foot" TagName="Footer" Src="~/Design/DesktopLayouts/DesktopFooter.ascx" %>
<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<%@ Page language="c#" Codebehind="PropertyPage.aspx.cs" AutoEventWireup="false" Inherits="Rainbow.DesktopModules.PagePropertyPage" %>
<HTML>
	<head runat="server"></head>
	<body runat="server">
		<form runat="server" ID="Form1">
			<div class="zen-main" id="zenpanes">
				<div class="rb_DefaultPortalHeader">
					<portal:Banner id="SiteHeader" runat="server" />
				</div>
				<div class="div_ev_Table">
					<table align="center" cellspacing="0" cellpadding="4" border="0">
						<tr valign="top">
							<td>
								<table width="600" cellspacing="0" cellpadding="0">
									<tr>
										<td align="left" class="Head" nowrap>
											<tra:Literal id="Literal1" runat="server" TextKey="MODULESETTINGS_SETTINGS" Text="Module settings"></tra:Literal>
										</td>
										<td align="right">
											<asp:placeholder id="PlaceholderButtons2" runat="server"></asp:placeholder>
										</td>
									</tr>
									<tr>
										<td colspan="2">
											<hr noshade size="1">
											<cc1:SettingsTable ID="EditTable" runat="server"></cc1:SettingsTable>
										</td>
									</tr>
									<tr>
										<td colspan="2">
											<asp:placeholder id="PlaceHolderButtons" runat="server"></asp:placeholder>
										</td>
									</tr>
								</table>
							</td>
						</tr>
					</table>
				</div>
				<div><foot:Footer id="Footer" runat="server"></foot:Footer></div>
			</div>
		</form>
	</body>
</HTML>
