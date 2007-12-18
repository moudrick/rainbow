<%@ Register TagPrefix="secure" TagName="Footer" Src="Footer.ascx" %>
<%@ Register TagPrefix="secure" TagName="Header" Src="Header.ascx" %>
<%@ Register TagPrefix="tra" Namespace="Esperantus.WebControls" Assembly="Esperantus" %>
<%@ Register TagPrefix="uc1" TagName="FinalizeOrder" Src="FinalizeOrder.ascx" %>
<%@ Page language="c#" AutoEventWireup="true" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Gateway Credit Transfer</title>
		<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1">
		<LINK href="msdefault.css" type="text/css" rel="stylesheet">
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script runat="server" language="C#">
		private void Page_Load(object sender, System.EventArgs e)
		{
			//Prevent back button by Manu
			//We want to avoid using back button of browser....
			Response.Cache.SetCacheability(HttpCacheability.NoCache);

			if (!IsPostBack)
			{
				// Create GATEWAY
				// This page is gateway specific so we should use a specific gateway object
				// We must create it using GatewayManager because we do not know where it is
				Rainbow.ECommerce.Rainbow.ECommerce.Gateways.GatewayBase gateway = Rainbow.ECommerce.GatewayManager.GetGateway("CreditTransfer");
												
				// Get MerchantID from querystring
				gateway.MerchantID = Request["MERCHANT_ID"];

				if (gateway.MerchantID != null)
				{
					// Call FinalizeOrder control for sending emails and showing receipt
					FinalizeOrder1.ProcessCheckOut(gateway);

					// Print a table on screen
					StringBuilder s = new StringBuilder("");
					s.Append("<TABLE WIDTH='50%' ALIGN='CENTER' CELLSPACING='5' CELLSPACING='0'>");
					s.Append("<TR><TD class='SubHead' width='50'>Bank: </TD><TD class='Normal'>" + gateway.CreditInstitute + "</TD></TR>");
				            
					foreach (string setting in gateway.CustomSettings)
					{
						s.Append("<TR><TD class='SubHead' width='50'>" + setting + ": </TD><TD class='Normal'>" + gateway.CustomSettings[setting] + "</TD></TR>");
					}
				
					s.Append("<TR><TD class='Normal' colspan='2'></td>");
					s.Append("</TR></TABLE>");

					MessageLabel.Text = s.ToString();
				}
				else // This can happen only if user tweaks on url
				{
					// Show failed transaction
					FinalizeOrder1.SetError("Transaction failed!");					
				}
			}
		}

		private void ContinueBtn_Click(object sender, System.EventArgs e)
		{
			// Redirect user back to the Portal Home Page
			Rainbow.Security.PortalSecurity.PortalHome();
		}
		</script>
	</HEAD>
	<body leftMargin="0" topMargin="0" marginwidth="0" marginheight="0">
		<form id="GatewayCarigePage" method="post" runat="server">
			<table height="100%" cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr vAlign="top">
					<td><secure:header id="Header1" runat="server"></secure:header></td>
				</tr>
				<TR>
					<td align="middle"><tra:label id="GatewayTitle" runat="server" CssClass="Head">Gateway Credit Transfer</tra:label>
						<HR noShade SIZE="1">
					</td>
				</TR>
				<TR height="*">
					<td class="Normal" align="middle">
						<p><tra:label id="MessageLabel" runat="server" CssClass="SubHead"></tra:label></p>
						<P><uc1:finalizeorder id="FinalizeOrder1" runat="server"></uc1:finalizeorder></P>
						<P>
							<asp:LinkButton id="ContinueBtn" runat="server" CssClass="CommandButton" EnableViewState="False" OnClick="ContinueBtn_Click">Continue</asp:LinkButton></P>
					</td>
				</TR>
				<tr vAlign="top">
					<td><secure:footer id="Footer2" runat="server"></secure:footer></td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
