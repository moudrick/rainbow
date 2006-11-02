<%@ Page language="c#" CodeBehind="Register.aspx.cs" AutoEventWireup="false" Inherits="Rainbow.Admin.Register" %>
<%@ Register TagPrefix="foot" TagName="Footer" Src="~/Design/DesktopLayouts/DesktopFooter.ascx" %>
<%@ Register TagPrefix="portal" TagName="Banner" Src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" %>
<html>
	<head runat="server"></head>
	<body runat="server">
		<form runat="server">
		<div class="zen-main" id="zenpanes">
				<div class="rb_DefaultPortalHeader">
							<portal:Banner ShowTabs="false" runat="server" id="Banner1" />
				</div>
				<div class="div_ev_Table">
					<table cellspacing="0" cellpadding="0" width="90%" border="0">
						<tr>
							<td>
								<!-- Start Register control -->
								<asp:PlaceHolder id="register" runat="server"></asp:PlaceHolder>
								<!-- End Register control -->
							</td>
						</tr>
					</table>
				</div>
				<div class="rb_AlternatePortalFooter"><foot:Footer id="Footer" runat="server"></foot:Footer></div>
			</div>
		</form>
	</body>
</html>
