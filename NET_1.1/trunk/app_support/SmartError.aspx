<%@ Import Namespace="Esperantus" %>
<%@ Register TagPrefix="portal" TagName="Banner" Src="~/Design/DesktopLayouts/DesktopPortalBanner.ascx" %>
<%@ Register TagPrefix="foot" TagName="Footer" Src="~/Design/DesktopLayouts/DesktopFooter.ascx" %>
<%@ Page CodeBehind="SmartError.aspx.cs" Language="c#" AutoEventWireup="false" Inherits="Rainbow.Error.SmartError" %>
<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<html>
	<head runat="server">
	</head>
	<body runat="server">
		<form runat="server">
			<div id="zenpanes">
				<div class="rb_DefaultPortalHeader">
					<portal:Banner ShowTabs="false" runat="server" id="Banner2" />
				</div>
				<div class="div_ev_Table">
					<div class="rb_DefaultLayoutDiv SmartError">
						<asp:PlaceHolder ID="PageContent" Runat="server"></asp:PlaceHolder>
					</div>
				</div>
				<foot:Footer id="Footer" runat="server"></foot:Footer>
			</div>
		</form>
	</body>
</html>
