<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<%@ Register TagPrefix="foot" TagName="Footer" Src="~/Design/DesktopLayouts/DesktopFooter.ascx" %>
<%@ Register TagPrefix="portal" TagName="Banner" Src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" %>
<%@ Page language="c#" Codebehind="PictureView.aspx.cs" AutoEventWireup="false" Inherits="Rainbow.DesktopModules.PictureView" %>
<html>
  <head runat="server" />
	<body runat="server">
		<form id="PictureView" method="post" runat="server">
			<div class="zen-main" id="zenpanes">
				<div class="rb_DefaultPortalHeader">
					<portal:Banner id="SiteHeader" runat="server" />
				</div>
				<div align="center">
				<div class="rb_DefaultLayoutDiv">
					<tra:label id="lblError" visible="false" textkey="PICTURES_FAILED_TO_LOAD" text="Failed to load templates. Revise your settings" runat="server" Font-Bold="True" ForeColor="Red"></tra:label>
					<asp:PlaceHolder id="Picture" runat="server"></asp:PlaceHolder>
				</div>
			<div class="rb_AlternatePortalFooter">
				<div class="rb_AlternatePortalFooter"><foot:Footer id="Footer" runat="server"></foot:Footer></div></foot:Footer>
			</div>
			</div>
		</form>
	</body>
</html>
